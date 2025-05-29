using GameStore.Api.Contracts;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<GameDto> games =
[
    new GameDto(Guid.NewGuid(), "The Witcher 3", "RPG", 49.99m, new DateOnly(2015, 5, 19)),
    new GameDto(Guid.NewGuid(), "Stardew Valley", "Simulation", 14.99m, new DateOnly(2016, 2, 26)),
    new GameDto(Guid.NewGuid(), "Celeste", "Platformer", 19.99m, new DateOnly(2018, 1, 25))
];

// GET /games
app.MapGet("/games", () => Results.Ok(games));
app.MapGet("/games/{id:guid}", (Guid id) =>
{
    var game = games.FirstOrDefault(g => g.Id == id);
    return game is not null ? Results.Ok(game) : Results.NotFound();
}).WithName("GetGameById");

// POST /games
app.MapPost("/games", (CreateGameDto createGameDto) =>
{
    var newGame = new GameDto(
        Guid.NewGuid(),
        createGameDto.Name,
        createGameDto.Genre,
        createGameDto.Price,
        createGameDto.ReleaseDate
    );
    games.Add(newGame);
    return Results.CreatedAtRoute(
        "GetGameById",
        new { id = newGame.Id },
        newGame
    );
});

app.Run();
