using System.Collections.Generic;
using GameStore.DAL.Entities;
using GameStore.DAL.Pipeline;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IMongoGameRepository
    {
        IEnumerable<Game> GetAll();

        bool IsPresent(string id);

        Game GetById(string id);

        IEnumerable<Genre> GetGameGenres(string gameId);

        IEnumerable<Platform> GetGamePlatforms(string gameId);

        Publisher GetGamePublisher(string gameId);

        IEnumerable<Game> GetGamesOfGenre(string genreId);

        IEnumerable<Game> GetGamesOfPlatform(string platformId);

        IEnumerable<Game> GetGamesOfPublisher(string publisherId);

        IEnumerable<Game> Filter(FilterModel filterModel);

        Game GetByKey(string key);

        void SetQuantity(string id, short quantity);
    }
}
