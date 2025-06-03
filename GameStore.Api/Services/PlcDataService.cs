using GameStore.Api.Data;
using GameStore.Api.Hubs;
using GameStore.Api.Interfaces;
using GameStore.Api.Mapping;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Services;

public class PlcDataService(IHubContext<DataUpdateHub> hubContext,
                            ILogger<PlcDataService> logger,
                            IServiceScopeFactory scopeFactory) : BackgroundService, IPlcDataService
{
    private readonly IHubContext<DataUpdateHub> _hubContext = hubContext;
    private readonly ILogger<PlcDataService> _logger = logger;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private Timer? _timer;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await StartMonitoring();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken); // Check every 5 seconds
            await CheckForPlcDataUpdates();
        }
    }

    public Task StartMonitoring()
    {
        _logger.LogInformation("PLC Data monitoring started");
        return Task.CompletedTask;
    }

    public Task StopMonitoring()
    {
        _timer?.Dispose();
        _logger.LogInformation("PLC Data monitoring stopped");
        return Task.CompletedTask;
    }

    public async Task NotifyClientsAsync<T>(string method, T data)
    {
        await _hubContext.Clients.All.SendAsync(method, data);
    }

    private async Task CheckForPlcDataUpdates()
    {
        try
        {
            // Simulate PLC data check - replace with actual PLC communication
            var hasNewData = await SimulateCheckPlcData();

            if (hasNewData)
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();

                // Example: Get updated games data
                var updatedGames = await dbContext.Games
                    .Include(g => g.Genre)
                    .Select(g => g.ToGameSummaryDto())
                    .ToListAsync();

                // Notify all connected clients
                await NotifyClientsAsync("DataUpdated", new
                {
                    Type = "Games",
                    Data = updatedGames,
                    Timestamp = DateTime.UtcNow
                });

                _logger.LogInformation("Notified clients about data update");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking PLC data");
        }
    }

    private async Task<bool> SimulateCheckPlcData()
    {
        // Simulate random data updates
        await Task.Delay(100);
        return Random.Shared.Next(1, 10) == 1; // 10% chance of update
    }
}
