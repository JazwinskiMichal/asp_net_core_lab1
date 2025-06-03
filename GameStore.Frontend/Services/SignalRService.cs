using GameStore.Frontend.Interfaces;
using GameStore.Frontend.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace GameStore.Frontend.Services;

public class SignalRService(ILogger<SignalRService> logger, IJSRuntime jsRuntime) : ISignalRService, IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private readonly ILogger<SignalRService> _logger = logger;
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    private bool _disposed = false;

    public event Action<List<GameSummary>>? OnGamesUpdated;
    public event Action<string>? OnConnectionStateChanged;
    
    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public async Task StartAsync()
    {
        if (_disposed || _hubConnection != null) 
            return;

        try
        {
            var apiUrl = "http://localhost:5063"; // API URL
            
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{apiUrl}/datahub", options =>
                {
                    // WebAssembly can use WebSockets without conflicts
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets | 
                                        Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
                })
                .WithAutomaticReconnect([ 
                    TimeSpan.Zero, 
                    TimeSpan.FromSeconds(2), 
                    TimeSpan.FromSeconds(10), 
                    TimeSpan.FromSeconds(30) 
                ])
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .Build();

            SetupEventHandlers();

            _logger.LogInformation("Starting SignalR connection to {Url}", $"{apiUrl}/datahub");
            await _hubConnection.StartAsync();
            _logger.LogInformation("SignalR connection started successfully. State: {State}", _hubConnection.State);
            OnConnectionStateChanged?.Invoke("Connected");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start SignalR connection");
            OnConnectionStateChanged?.Invoke("Failed");
        }
    }

    private void SetupEventHandlers()
    {
        if (_hubConnection == null) return;

        _hubConnection.On<object>("DataUpdated", async (data) =>
        {
            try
            {
                _logger.LogInformation("Received data update from SignalR");
                await HandleDataUpdate(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling data update");
            }
        });

        _hubConnection.Reconnecting += error =>
        {
            _logger.LogWarning("SignalR connection lost: {Error}. Attempting to reconnect...", error?.Message);
            OnConnectionStateChanged?.Invoke("Reconnecting");
            return Task.CompletedTask;
        };

        _hubConnection.Reconnected += connectionId =>
        {
            _logger.LogInformation("SignalR reconnected successfully with ID: {ConnectionId}", connectionId);
            OnConnectionStateChanged?.Invoke("Connected");
            return Task.CompletedTask;
        };

        _hubConnection.Closed += error =>
        {
            _logger.LogError("SignalR connection closed: {Error}", error?.Message);
            OnConnectionStateChanged?.Invoke("Disconnected");
            return Task.CompletedTask;
        };
    }

    private async Task HandleDataUpdate(object data)
    {
        try
        {
            var jsonData = System.Text.Json.JsonSerializer.Serialize(data);
            _logger.LogDebug("Processing data update: {JsonData}", jsonData);
            
            var updateInfo = System.Text.Json.JsonSerializer.Deserialize<DataUpdateInfo>(jsonData);
            
            if (updateInfo?.Type == "Games" && updateInfo.Data != null)
            {
                var gamesJson = System.Text.Json.JsonSerializer.Serialize(updateInfo.Data);
                var games = System.Text.Json.JsonSerializer.Deserialize<List<GameSummary>>(gamesJson);
                
                if (games != null)
                {
                    _logger.LogInformation("Processed {Count} games from SignalR update", games.Count);
                    OnGamesUpdated?.Invoke(games);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing data update");
        }
        
        await Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        if (_hubConnection != null)
        {
            try
            {
                _logger.LogInformation("Stopping SignalR connection");
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error stopping SignalR connection");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        
        _disposed = true;
        await StopAsync();
    }

    private class DataUpdateInfo
    {
        public string? Type { get; set; }
        public object? Data { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
