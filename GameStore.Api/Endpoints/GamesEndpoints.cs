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
        var group = routes.MapGroup("/games")
            .WithParameterValidation()
            .WithTags("Games")
            .WithOpenApi();

        // GET /games
        group.MapGet("/", async (GameStoreContext dbContext) => 
            await dbContext.Games
                    .Include(game => game.Genre)
                    .Select(game => game.ToGameSummaryDto())
                    .AsNoTracking()
                    .ToListAsync())
        .WithName("GetAllGames")
        .WithSummary("Get all games")
        .WithDescription("Retrieves a list of all games with their genres")
        .Produces<List<GameSummaryDto>>(StatusCodes.Status200OK)
        .WithOpenApi();

        // GET /games/{id:guid}
        group.MapGet("/{id:guid}", async (Guid id, GameStoreContext dbContext) =>
        {
            GameEntity? game = await dbContext.Games.FindAsync(id);
            return game is not null ? Results.Ok(game.ToGameDetailsDto()) : Results.NotFound();
        })
        .WithName("GetGameById")
        .WithSummary("Get game by ID")
        .WithDescription("Retrieves a specific game by its unique identifier")
        .Produces<GameDetailsDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

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
        })
        .WithName("CreateGame")
        .WithSummary("Create a new game")
        .WithDescription("Creates a new game with the provided details")
        .Accepts<CreateGameDto>("application/json")
        .Produces<GameDetailsDto>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .WithOpenApi();

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
        })
        .WithName("UpdateGame")
        .WithSummary("Update an existing game")
        .WithDescription("Updates an existing game with the provided details")
        .Accepts<UpdateGameDto>("application/json")
        .Produces<GameDetailsDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem()
        .WithOpenApi();

        // DELETE /games/{id:guid}
        group.MapDelete("/{id:guid}", async (Guid id, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);
            if (existingGame is null)
            {
                return Results.NotFound();
            }

            await dbContext.Games.Where(game => game.Id == id).ExecuteDeleteAsync();
            return Results.NoContent();
        })
        .WithName("DeleteGame")
        .WithSummary("Delete a game")
        .WithDescription("Deletes a game by its unique identifier")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();
    }
}