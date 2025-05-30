using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Contracts;

public record class UpdateGameDto(
    [Required][StringLength(50)] string Name,
    int GenreId,
    [Range(0, 100)] decimal? Price,
    DateOnly? ReleaseDate
);
