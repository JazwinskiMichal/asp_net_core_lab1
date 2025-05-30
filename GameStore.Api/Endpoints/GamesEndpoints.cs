using GameStore.Api.Contracts;
using GameStore.Api.Data;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    public static void MapGamesEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/games").WithParameterValidation();

        // GET /games
        group.MapGet("/", (GameStoreContext dbContext) => Results.Ok(dbContext.Games.Include(game => game.Genre).Select(g => g.ToGameSummaryDto()).AsNoTracking()));

        // GET /games/{id:guid}
        group.MapGet("/{id:guid}", (Guid id, GameStoreContext dbContext) =>
        {
            GameEntity? game = dbContext.Games.Find(id);

            return game is not null ? Results.Ok(game.ToGameDetailsDto()) : Results.NotFound();

        }).WithName("GetGameById");

        // POST /games
        group.MapPost("/", (CreateGameDto createGameDto, GameStoreContext dbContext) =>
        {
            GameEntity game = createGameDto.ToEntity();
            
            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            return Results.CreatedAtRoute(
                "GetGameById",
                new { id = game.Id },
                game.ToGameDetailsDto()
            );
        });

        // PUT /games/{id:guid}
        group.MapPut("/{id:guid}", (Guid id, UpdateGameDto updateGameDto, GameStoreContext dbContext) =>
        {
            GameEntity? existingGame = dbContext.Games.Find(id);
            if (existingGame is null)
            {
                return Results.NotFound();
            }

            GameEntity updatedGame = updateGameDto.ToEntity(id);
            dbContext.Entry(existingGame).CurrentValues.SetValues(updatedGame);
            dbContext.SaveChanges();

            return Results.Ok(updatedGame.ToGameDetailsDto());
        });

        // DELETE /games/{id:guid}
        group.MapDelete("/{id:guid}", (Guid id, GameStoreContext dbContext) =>
        {
            var existingGame = dbContext.Games.Find(id);
            if (existingGame is null)
            {
                return Results.NotFound();
            }

            // Batch delete
            dbContext.Games.Where(game => game.Id == id).ExecuteDelete();

            return Results.NoContent();
        });
    }
}
