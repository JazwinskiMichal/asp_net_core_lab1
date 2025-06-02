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
        group.MapGet("/", async (GameStoreContext dbContext) => 
            await dbContext.Games
                    .Include(game => game.Genre)
                    .Select(game => game.ToGameSummaryDto())
                    .AsNoTracking()
                    .ToListAsync());

        // GET /games/{id:guid}
        group.MapGet("/{id:guid}", async (Guid id, GameStoreContext dbContext) =>
        {
            GameEntity? game = await dbContext.Games.FindAsync(id);

            return game is not null ? Results.Ok(game.ToGameDetailsDto()) : Results.NotFound();

        }).WithName("GetGameById");

        // POST /games
        group.MapPost("/", async (CreateGameDto createGameDto, GameStoreContext dbContext) =>
        {
            GameEntity game = createGameDto.ToEntity();
            
            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(
                "GetGameById",
                new { id = game.Id },
                game.ToGameDetailsDto()
            );
        });

        // PUT /games/{id:guid}
        group.MapPut("/{id:guid}", async (Guid id, UpdateGameDto updateGameDto, GameStoreContext dbContext) =>
        {
            GameEntity? existingGame = await dbContext.Games.FindAsync(id);
            if (existingGame is null)
            {
                return Results.NotFound();
            }

            GameEntity updatedGame = updateGameDto.ToEntity(id);
            dbContext.Entry(existingGame).CurrentValues.SetValues(updatedGame);
            await dbContext.SaveChangesAsync();

            return Results.Ok(updatedGame.ToGameDetailsDto());
        });

        // DELETE /games/{id:guid}
        group.MapDelete("/{id:guid}", async (Guid id, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);
            if (existingGame is null)
            {
                return Results.NotFound();
            }

            // Batch delete
            await dbContext.Games.Where(game => game.Id == id).ExecuteDeleteAsync();

            return Results.NoContent();
        });
    }
}
