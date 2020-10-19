using System;
using System.Collections.Generic;
using GameStore.BLL.Models;
using GameStore.DAL.Pipeline.Util;

namespace GameStore.BLL.Interfaces
{
    public interface IGameService
    {
        Game EditGame(Game game);

        void CreateGame(Game game);

        Game GetGameByKey(string key);

        Game GetGameById(Guid gameId);

        IEnumerable<Game> GetAllGames();

        void DeleteGame(Game game);

        IEnumerable<Game> GetGamesByGenre(Genre genre);

        IEnumerable<Game> GetGamesByPlatform(Platform platform);

        IEnumerable<Game> GetGamesByPublisher(Publisher publisher);

        bool IsPresent(string gameKey);

        IEnumerable<Game> FilterGames(QueryModel queryModel);

        int Count();

        void AddView(string gameKey);

        IDictionary<OrderOption, OrderOptionModel> GetOrderOptions();

        IDictionary<TimePeriod, TimePeriodModel> GetTimePeriods();
    }
}
