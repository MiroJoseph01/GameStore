using System.Collections.Generic;
using GameStore.BLL.Models;

namespace GameStore.BLL.Interfaces
{
    public interface IGameService
    {
        Game EditGame(Game game);

        void CreateGame(Game game);

        Game GetGameByKey(string key);

        IEnumerable<Game> GetAllGames();

        void DeleteGame(Game game);

        IEnumerable<Game> GetGamesByGenre(Genre genre);

        IEnumerable<Game> GetGamesByPlatform(Platform genre);

        bool IsPresent(string gameKey);
    }
}
