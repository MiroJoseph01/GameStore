using System.Collections.Generic;
using GameStore.DAL.Entities;
using GameStore.DAL.Pipeline;
using GameStore.DAL.Pipeline.Util;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IGameRepository : IRepository<Game>
    {
        bool Update(string id, Game entity, short ordered = 0);

        IEnumerable<Genre> GetGameGenres(string gameId);

        IEnumerable<Platform> GetGamePlatforms(string gameId);

        Publisher GetGamePublisher(string gameId);

        IEnumerable<Game> GetGamesOfGenre(string genreId);

        IEnumerable<Game> GetGamesOfPlatform(string platformId);

        IEnumerable<Game> GetGamesOfPublisher(string publisherId);

        Game GetByKey(string key);

        void AddView(string id);

        IEnumerable<Game> Filter(FilterModel model);

        IDictionary<OrderOption, OrderOptionModel> GetOrderOptions();

        IDictionary<TimePeriod, TimePeriodModel> GetTimePeriods();
    }
}
