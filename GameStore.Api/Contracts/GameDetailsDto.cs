namespace GameStore.Api.Contracts;

/// <summary>
/// Represents detailed information about a game
/// </summary>
public record class GameDetailsDto(
    /// <summary>
    /// The unique identifier of the game
    /// </summary>
    Guid Id,
    
    /// <summary>
    /// The name of the game
    /// </summary>
    string Name,
    
    /// <summary>
    /// The unique identifier of the game's genre
    /// </summary>
    int GenreId,
    
    /// <summary>
    /// The price of the game in USD
    /// </summary>
    decimal Price,
    
    /// <summary>
    /// The release date of the game
    /// </summary>
    DateOnly ReleaseDate
);
