using GameStore.Api.Contracts;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    private static readonly List<GameDto> games =
    [
        new GameDto(Guid.NewGuid(), "The Witcher 3", "RPG", 49.99m, new DateOnly(2015, 5, 19)),
        new GameDto(Guid.NewGuid(), "Stardew Valley", "Simulation", 14.99m, new DateOnly(2016, 2, 26)),
        new GameDto(Guid.NewGuid(), "Celeste", "Platformer", 19.99m, new DateOnly(2018, 1, 25))
    ];

    public static void MapGamesEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/games").WithParameterValidation();

        // GET /games
        group.MapGet("/", () => Results.Ok(games));

        // GET /games/{id:guid}
        group.MapGet("/{id:guid}", (Guid id) =>
        {
            var game = games.FirstOrDefault(g => g.Id == id);
            return game is not null ? Results.Ok(game) : Results.NotFound();
        }).WithName("GetGameById");

        // POST /games
        group.MapPost("/", (CreateGameDto createGameDto) =>
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

        // PUT /games/{id:guid}
        group.MapPut("/{id:guid}", (Guid id, UpdateGameDto updateGameDto) =>
        {
            var game = games.FirstOrDefault(g => g.Id == id);
            if (game is null)
            {
                return Results.NotFound();
            }

            game = game with
            {
                Name = updateGameDto.Name ?? game.Name,
                Genre = updateGameDto.Genre ?? game.Genre,
                Price = updateGameDto.Price ?? game.Price,
                ReleaseDate = updateGameDto.ReleaseDate ?? game.ReleaseDate
            };

            games[games.FindIndex(g => g.Id == id)] = game;
            return Results.Ok(game);
        });

        // DELETE /games/{id:guid}
        group.MapDelete("/{id:guid}", (Guid id) =>
        {
            var gameIndex = games.FindIndex(g => g.Id == id);
            if (gameIndex == -1)
            {
                return Results.NotFound();
            }

            games.RemoveAt(gameIndex);
            return Results.NoContent();
        });
    }
}
