namespace GameStore.Frontend.Models;

/// <summary>
/// Client-side model representing a game summary
/// </summary>
public class GameSummary
{
    /// <summary>
    /// The unique identifier of the game
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the game
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The genre of the game
    /// </summary>
    public required string Genre { get; set; }

    /// <summary>
    /// The price of the game in USD
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// The release date of the game
    /// </summary>
    public DateOnly ReleaseDate { get; set; }
}
