using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Contracts;

public record class CreateGameDto(
    [Required][StringLength(50)] string Name,
    int GenreId,
    [Required][Range(0, 100)] decimal Price,
    DateOnly ReleaseDate
);
