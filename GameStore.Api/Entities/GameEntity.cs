using System;

namespace GameStore.Api.Entities;

public class GameEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int GenreId { get; set; }
    public GenreEntity? Genre { get; set; }
    public decimal Price { get; set; }
    public DateOnly ReleaseDate { get; set; }
}
