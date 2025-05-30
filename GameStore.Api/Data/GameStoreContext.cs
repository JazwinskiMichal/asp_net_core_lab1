using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
{
    public DbSet<GameEntity> Games => Set<GameEntity>();
    public DbSet<GenreEntity> Genres => Set<GenreEntity>();

    // Seeding example data
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GenreEntity>().HasData(
            new GenreEntity { Id = 1, Name = "Action" },
            new GenreEntity { Id = 2, Name = "Adventure" },
            new GenreEntity { Id = 3, Name = "Role-Playing" }
        );
    }
}
