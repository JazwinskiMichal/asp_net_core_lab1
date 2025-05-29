using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Contracts;

public record class CreateGameDto(
    [Required][StringLength(50)] string Name,
    [Required][StringLength(20)] string Genre,
    [Required][Range(0, 100)] decimal Price,
    DateOnly ReleaseDate
);
