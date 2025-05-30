namespace GameStore.Api.Contracts;

public record class GameDetailsDto(
    Guid Id,
    string Name,
    int GenreId,
    decimal Price,
    DateOnly ReleaseDate
);
