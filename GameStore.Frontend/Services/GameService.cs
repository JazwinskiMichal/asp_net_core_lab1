using GameStore.Frontend.Models;

namespace GameStore.Frontend.Services;

/// <summary>
/// Service for managing game operations with the API
/// </summary>
public class GameService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://localhost:7000";

    public GameService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<GameSummary>> GetGamesAsync()
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/games");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<GameSummary>>();
    }

    public async Task<bool> DeleteGameAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/games/{id}");
        return response.IsSuccessStatusCode;
    }
}
