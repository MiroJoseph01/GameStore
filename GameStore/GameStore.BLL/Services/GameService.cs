using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces.Services;
using GameStore.DAL.Entities.SupportingModels;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using GameStore.DAL.Pipeline;
using GameStore.DAL.Pipeline.Util;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.BLL.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepositoryFacade _gameRepository;
        private readonly IViewRepository _viewRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GameService(
            IGameRepositoryFacade gameRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IViewRepository viewRepository)
        {
            _gameRepository = gameRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _viewRepository = viewRepository;
        }

        public void CreateGame(BusinessModels.Game game)
        {
            if (string.IsNullOrEmpty(game.Key))
            {
                game.Key = game.Name.ToLower().Replace(" ", "_");
            }

            game.GameId = Guid.NewGuid().ToString();

            _gameRepository.Create(Convert(game));

            _unitOfWork.Commit();
        }

        public void DeleteGame(BusinessModels.Game game)
        {
            if (!_gameRepository.IsPresent(game.GameId))
            {
                throw new ArgumentException($"Game with id \'{game.GameId}\' has already been deleted or doesn't exist");
            }

            _gameRepository.Delete(game.GameId);

            _unitOfWork.Commit();
        }

        public BusinessModels.Game EditGame(BusinessModels.Game game, short quantity = 0)
        {
            if (!_gameRepository.IsPresent(game.GameId))
            {
                throw new ArgumentException("Invalid game id");
            }

            var gameToUpdate = Convert(game);

            var quantityIsCorrect = _gameRepository.Update(game.GameId, gameToUpdate, quantity);

            _unitOfWork.Commit();

            if (!quantityIsCorrect)
            {
                return null;
            }

            return game;
        }

        public IEnumerable<BusinessModels.Game> GetAllGames()
        {
            IEnumerable<DbModels.Game> gamesFromDb = _gameRepository.GetAll();

            return Convert(gamesFromDb);
        }

        public BusinessModels.Game GetGameByKey(string key)
        {
            if (!IsPresent(key))
            {
                return null;
            }

            DbModels.Game gameFromDb = _gameRepository.GetByKey(key);

            return Convert(gameFromDb);
        }

        public BusinessModels.Game GetGameById(string id)
        {
            DbModels.Game gameFromDb = _gameRepository.GetById(id);

            return Convert(gameFromDb);
        }

        public IEnumerable<BusinessModels.Game> GetGamesByGenre(
            BusinessModels.Genre genre)
        {
            IEnumerable<DbModels.Game> gamesFromDb = _gameRepository
                .GetGamesOfGenre(genre.GenreId);

            return Convert(gamesFromDb);
        }

        public IEnumerable<BusinessModels.Game> GetGamesByPlatform(
            BusinessModels.Platform platform)
        {
            IEnumerable<DbModels.Game> gamesFromDb = _gameRepository.GetGamesOfPlatform(platform.PlatformId);

            return Convert(gamesFromDb);
        }

        public IEnumerable<BusinessModels.Game> GetGamesByPublisher(
            BusinessModels.Publisher publisher)
        {
            IEnumerable<DbModels.Game> gamesFromDb = _gameRepository.GetGamesOfPublisher(publisher.PublisherId);

            return Convert(gamesFromDb);
        }

        public bool IsPresent(string gameKey)
        {
            DbModels.Game game = _gameRepository.GetByKey(gameKey);

            if (game is null)
            {
                return false;
            }

            return true;
        }

        public int Count()
        {
            int count = _gameRepository.GetAll().Count();

            return count;
        }

        public void AddView(string gameKey)
        {
            _gameRepository.AddView(gameKey);

            _unitOfWork.Commit();
        }

        public IEnumerable<BusinessModels.Game> FilterGames(BusinessModels.QueryModel queryModel)
        {
            var filterModel = _mapper.Map<FilterModel>(queryModel);

            return Convert(
                _gameRepository.Filter(filterModel));
        }

        public IDictionary<OrderOption, OrderOptionModel> GetOrderOptions()
        {
            return _gameRepository.GetOrderOptions();
        }

        public IDictionary<TimePeriod, TimePeriodModel> GetTimePeriods()
        {
            return _gameRepository.GetTimePeriods();
        }

        private IList<BusinessModels.Genre> GetGameGenres(string id)
        {
            IEnumerable<DbModels.Genre> genresFromDb = _gameRepository.GetGameGenres(id);

            var genres = _mapper.Map<IEnumerable<BusinessModels.Genre>>(genresFromDb).ToList();

            return genres;
        }

        private BusinessModels.Publisher GetGamePublisher(string id)
        {
            DbModels.Publisher publisherFromDb = _gameRepository.GetGamePublisher(id);

            var publisher = _mapper.Map<BusinessModels.Publisher>(publisherFromDb);

            return publisher;
        }

        private IList<BusinessModels.Platform> GetGamePlatforms(string id)
        {
            IEnumerable<DbModels.Platform> platformsFromDb = _gameRepository.GetGamePlatforms(id);

            IList<BusinessModels.Platform> platforms = platformsFromDb
                .Select(x => new BusinessModels.Platform
                {
                    PlatformId = x.PlatformId,
                    PlatformName = x.PlatformName,
                })
                .ToList();

            return platforms;
        }

        private IList<BusinessModels.Comment> GetGameComments(string id)
        {
            var comments = new List<BusinessModels.Comment>();

            if (_gameRepository.IsPresent(id))
            {
                IList<DbModels.Comment> commentsFromDb = _gameRepository.GetById(id).Comments;

                comments = _mapper.Map<IList<BusinessModels.Comment>>(commentsFromDb).ToList();
            }

            return comments;
        }

        private IEnumerable<BusinessModels.Game> Convert(
            IEnumerable<DbModels.Game> gamesFromDb)
        {
            var games = _mapper.Map<List<BusinessModels.Game>>(gamesFromDb);

            foreach (BusinessModels.Game g in games)
            {
                g.GameGenres = GetGameGenres(g.GameId);
                g.GamePlatforms = GetGamePlatforms(g.GameId);
                g.Comments = GetGameComments(g.GameId);
                g.Publisher = GetGamePublisher(g.GameId);
                g.Views = _viewRepository.GetViewsByGameId(g.GameId);
            }

            return games;
        }

        private BusinessModels.Game Convert(DbModels.Game gameFromDb)
        {
            BusinessModels.Game game = null;

            if (gameFromDb != null)
            {
                game = _mapper.Map<BusinessModels.Game>(gameFromDb);
                game.GameGenres = GetGameGenres(game.GameId);
                game.GamePlatforms = GetGamePlatforms(game.GameId);
                game.Comments = GetGameComments(game.GameId);
                game.Publisher = GetGamePublisher(game.GameId);
                game.Views = _viewRepository.GetViewsByGameId(game.GameId);
            }

            return game;
        }

        private DbModels.Game Convert(BusinessModels.Game game)
        {
            var gameFromDb = _mapper.Map<DbModels.Game>(game);
            gameFromDb.PlatformGames = GetGamePlatforms(game.GameId, game.GamePlatforms);
            gameFromDb.GenreGames = GetGameGenres(game.GameId, game.GameGenres);

            return gameFromDb;
        }

        private IList<GameGenre> GetGameGenres(string id, IEnumerable<BusinessModels.Genre> genres)
        {
            List<GameGenre> gameGenresList = genres
                .Select(x => new GameGenre
                {
                    GameId = id,
                    GenreId = x.GenreId,
                })
                .ToList();

            return gameGenresList;
        }

        private IList<GamePlatform> GetGamePlatforms(string id, IEnumerable<BusinessModels.Platform> platforms)
        {
            List<GamePlatform> gamePlatformsList = platforms
                .Select(x => new GamePlatform
                {
                    GameId = id,
                    PlatformId = x.PlatformId,
                })
                .ToList();

            return gamePlatformsList;
        }
    }
}
