using GameStore.Frontend.Models;

namespace GameStore.Frontend.Interfaces;

public interface IHubService
{
    Task StartAsync();
    Task StopAsync();
    event Action<List<GameSummary>>? OnGamesUpdated;
    event Action<string>? OnConnectionStateChanged;
    bool IsConnected { get; }
}
