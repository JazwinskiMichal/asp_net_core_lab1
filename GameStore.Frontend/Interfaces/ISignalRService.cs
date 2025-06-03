using GameStore.Frontend.Models;

namespace GameStore.Frontend.Interfaces;

public interface ISignalRService
{
    Task StartAsync();
    Task StopAsync();
    event Action<List<GameSummary>>? OnGamesUpdated;
    event Action<string>? OnConnectionStateChanged;
    bool IsConnected { get; }
}
