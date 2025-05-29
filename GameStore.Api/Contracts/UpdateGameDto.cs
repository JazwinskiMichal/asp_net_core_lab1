namespace GameStore.Api.Contracts;

public record class UpdateGameDto(
    string? Name = null,
    string? Genre = null,
    decimal? Price = null,
    DateOnly? ReleaseDate = null
);
