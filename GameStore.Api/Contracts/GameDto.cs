namespace GameStore.Api.Contracts;

public record class GameDto(
    Guid Id,
    string Name,
    string Genre,
    decimal Price,
    DateOnly ReleaseDate
);
