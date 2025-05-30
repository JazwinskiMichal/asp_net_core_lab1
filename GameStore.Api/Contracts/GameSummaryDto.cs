namespace GameStore.Api.Contracts;

public record class GameSumarryDto(
    Guid Id,
    string Name,
    string Genre,
    decimal Price,
    DateOnly ReleaseDate
);
