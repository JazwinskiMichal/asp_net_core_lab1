using System.Net.Http.Json;
using GameStore.Frontend.Models;

namespace GameStore.Frontend.Services;

public class GameService(IHttpClientFactory httpClientFactory, ILogger<GameService> logger)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("GameStoreAPI");
    private readonly ILogger<GameService> _logger = logger;

    public async Task<List<GameSummary>?> GetGamesAsync()
    {
        try
        {
            _logger.LogInformation("Fetching games from API...");
            
            var response = await _httpClient.GetAsync("games");
            
            if (response.IsSuccessStatusCode)
            {
                var games = await response.Content.ReadFromJsonAsync<List<GameSummary>>();
                _logger.LogInformation("Successfully fetched {Count} games", games?.Count ?? 0);
                return games;
            }
            else
            {
                _logger.LogWarning("Failed to fetch games. Status: {StatusCode}, Reason: {ReasonPhrase}", 
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching games from API. BaseAddress: {BaseAddress}", 
                _httpClient.BaseAddress);
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timeout fetching games from API");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching games from API");
            return null;
        }
    }

    public async Task<GameSummary?> GetGameAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Fetching game {GameId} from API...", id);
            return await _httpClient.GetFromJsonAsync<GameSummary>($"games/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching game {GameId}", id);
            return null;
        }
    }

    public async Task<bool> DeleteGameAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting game {GameId}...", id);
            var response = await _httpClient.DeleteAsync($"games/{id}");
            var success = response.IsSuccessStatusCode;
            
            if (success)
            {
                _logger.LogInformation("Successfully deleted game {GameId}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete game {GameId}. Status: {StatusCode}, Reason: {ReasonPhrase}", 
                    id, response.StatusCode, response.ReasonPhrase);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting game {GameId}", id);
            return false;
        }
    }
}