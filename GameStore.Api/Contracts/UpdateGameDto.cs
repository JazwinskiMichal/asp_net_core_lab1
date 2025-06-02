using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Contracts;

/// <summary>
/// Data transfer object for updating an existing game
/// </summary>
public record class UpdateGameDto(
    /// <summary>
    /// The name of the game
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    string Name,
    
    /// <summary>
    /// The unique identifier of the game's genre
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Genre must be selected")]
    int GenreId,
    
    /// <summary>
    /// The price of the game in USD
    /// </summary>
    [Range(0, 999.99)]
    decimal? Price,
    
    /// <summary>
    /// The release date of the game
    /// </summary>
    DateOnly? ReleaseDate
);
