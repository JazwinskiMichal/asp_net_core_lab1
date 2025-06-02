using GameStore.Api.Contracts;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping;

public static class GenreMapping
{
    public static GenreDto ToDto (this GenreEntity genre)
    {
        ArgumentNullException.ThrowIfNull(genre);

        return new GenreDto(genre.Id, genre.Name);
    }
}
