using System;
using System.Collections.Generic;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IGameRepository : IRepository<Game>
    {
        IEnumerable<Genre> GetGameGenres(Guid gameId);

        IEnumerable<Platform> GetGamePlatforms(Guid gameId);

        IEnumerable<Game> GetGamesOfGenre(Guid genreId);

        IEnumerable<Game> GetGamesOfPlatform(Guid platformId);

        IEnumerable<Game> GetGamesOfPublisher(Guid publisherId);

        Game GetByKey(string key);
    }
}
