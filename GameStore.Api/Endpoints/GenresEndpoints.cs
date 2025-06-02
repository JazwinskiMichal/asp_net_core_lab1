using GameStore.Api.Contracts;
using GameStore.Api.Data;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GenresEndpoints
{
    public static RouteGroupBuilder MapGenresEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/genres")
            .WithTags("Genres")
            .WithOpenApi();

        group.MapGet("/", async (GameStoreContext dbContext) =>
            await dbContext.Genres
                            .Select(genre => genre.ToDto())
                            .AsNoTracking()
                            .ToListAsync())
        .WithName("GetAllGenres")
        .WithSummary("Get all genres")
        .WithDescription("Retrieves a list of all available game genres")
        .Produces<List<GenreDto>>(StatusCodes.Status200OK)
        .WithOpenApi();
        
        return group;        
    }
}