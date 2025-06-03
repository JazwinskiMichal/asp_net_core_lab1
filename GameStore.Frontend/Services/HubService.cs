using GameStore.Frontend.Interfaces;
using GameStore.Frontend.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace GameStore.Frontend.Services;

public class HubService : IHubService, IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private readonly ILogger<HubService> _logger;
    private readonly IJSRuntime _jsRuntime;
    private bool _disposed = false;

    public event Action<List<GameSummary>>? OnGamesUpdated;
    public event Action<string>? OnConnectionStateChanged;
    
    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public HubService(ILogger<HubService> logger, IJSRuntime jsRuntime)
    {
        _logger = logger;
        _jsRuntime = jsRuntime;
    }

    public async Task StartAsync()
    {
        if (_disposed || _hubConnection != null) 
            return;

        try
        {
            // Use a different port/URL for your API
            var apiUrl = "http://localhost:5063"; // API URL
            
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{apiUrl}/datahub", options =>
                {
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
                    // Use long polling to avoid WebSocket conflicts with Blazor Server
                })
                .WithAutomaticReconnect([ 
                    TimeSpan.FromSeconds(0), 
                    TimeSpan.FromSeconds(5), 
                    TimeSpan.FromSeconds(10) 
                ])
                .Build();

            SetupEventHandlers();

            await _hubConnection.StartAsync();
            _logger.LogInformation("Hub connection started successfully");
            OnConnectionStateChanged?.Invoke("Connected");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start hub connection");
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
                await HandleDataUpdate(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling data update");
            }
        });

        _hubConnection.Reconnecting += error =>
        {
            _logger.LogWarning("Connection lost, reconnecting...");
            OnConnectionStateChanged?.Invoke("Reconnecting");
            return Task.CompletedTask;
        };

        _hubConnection.Reconnected += connectionId =>
        {
            _logger.LogInformation("Reconnected with ID: {ConnectionId}", connectionId);
            OnConnectionStateChanged?.Invoke("Connected");
            return Task.CompletedTask;
        };

        _hubConnection.Closed += error =>
        {
            _logger.LogError("Connection closed: {Error}", error?.Message);
            OnConnectionStateChanged?.Invoke("Disconnected");
            return Task.CompletedTask;
        };
    }

    private async Task HandleDataUpdate(object data)
    {
        await Task.Run(() =>
        {
            try
            {
                var jsonData = System.Text.Json.JsonSerializer.Serialize(data);
                var updateInfo = System.Text.Json.JsonSerializer.Deserialize<DataUpdateInfo>(jsonData);
                
                if (updateInfo?.Type == "Games" && updateInfo.Data != null)
                {
                    var gamesJson = System.Text.Json.JsonSerializer.Serialize(updateInfo.Data);
                    var games = System.Text.Json.JsonSerializer.Deserialize<List<GameSummary>>(gamesJson);
                    
                    if (games != null)
                    {
                        OnGamesUpdated?.Invoke(games);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing data update");
            }
        });
    }

    public async Task StopAsync()
    {
        if (_hubConnection != null)
        {
            try
            {
                await _hubConnection.StopAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error stopping hub connection");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        
        _disposed = true;
        
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }

    private class DataUpdateInfo
    {
        public string? Type { get; set; }
        public object? Data { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
