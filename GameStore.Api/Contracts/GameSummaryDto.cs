namespace GameStore.Api.Contracts;

/// <summary>
/// Represents a game summary for list views
/// </summary>
public record class GameSummaryDto(
    /// <summary>
    /// The unique identifier of the game
    /// </summary>
    Guid Id,
    
    /// <summary>
    /// The name of the game
    /// </summary>
    string Name,
    
    /// <summary>
    /// The genre of the game
    /// </summary>
    string Genre,
    
    /// <summary>
    /// The price of the game in USD
    /// </summary>
    decimal Price,
    
    /// <summary>
    /// The release date of the game
    /// </summary>
    DateOnly ReleaseDate
);