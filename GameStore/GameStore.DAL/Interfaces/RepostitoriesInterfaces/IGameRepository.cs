using System;
using System.Collections.Generic;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Interfaces
{
    public interface IGameRepository : IGenericRepository<Game>
    {
        IEnumerable<Genre> GetGameGenres(Guid gameId);

        IEnumerable<Platform> GetGamePlatforms(Guid gameId);

        IEnumerable<Game> GetGamesOfGenre(Guid genreId);

        IEnumerable<Game> GetGamesOfPlatform(Guid platformId);

        Game GetByKey(string key);
    }
}
