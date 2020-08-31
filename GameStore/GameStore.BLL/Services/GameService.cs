using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DAL.Entities.SupportingModels;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.BLL.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GameService(
            IGameRepository gameRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _gameRepository = gameRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void CreateGame(BusinessModels.Game game)
        {
            if (string.IsNullOrEmpty(game.Key))
            {
                game.Key = game.Name.ToLower().Replace(" ", "_");
            }

            if (!game.GamePlatforms.Any())
            {
                throw new ArgumentException("Platform is required");
            }

            _gameRepository.Create(Convert(game));

            _unitOfWork.Commit();
        }

        public void DeleteGame(BusinessModels.Game game)
        {
            if (!_gameRepository.IsPresent(game.GameId))
            {
                throw new ArgumentException(
                    $"Game with id \'{game.GameId}\' " +
                    $"has already been deleted or doesn't exist");
            }

            DbModels.Game gameFromDb = _gameRepository.GetById(game.GameId);

            gameFromDb.IsRemoved = true;

            _unitOfWork.Commit();
        }

        public BusinessModels.Game EditGame(BusinessModels.Game game)
        {
            if (!_gameRepository.IsPresent(game.GameId))
            {
                throw new ArgumentException("Invalid game id");
            }

            var gameToUpdate = Convert(game);

            _gameRepository.Update(game.GameId, gameToUpdate);

            _unitOfWork.Commit();

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

        public BusinessModels.Game GetGameById(Guid id)
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
            IEnumerable<DbModels.Game> gamesFromDb = _gameRepository
                .GetGamesOfPlatform(platform.PlatformId);

            return Convert(gamesFromDb);
        }

        public IEnumerable<BusinessModels.Game> GetGamesByPublisher(
            BusinessModels.Publisher publisher)
        {
            IEnumerable<DbModels.Game> gamesFromDb = _gameRepository
                .GetGamesOfPublisher(publisher.PublisherId);

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
            return _gameRepository.GetAll().Count();
        }

        private IList<BusinessModels.Genre> GetGameGenres(Guid id)
        {
            IEnumerable<DbModels.Genre> genresFromDb = _gameRepository
                .GetGameGenres(id);

            var genres = _mapper
                .Map<IEnumerable<BusinessModels.Genre>>(genresFromDb).ToList();

            return genres;
        }

        private IList<BusinessModels.Platform> GetGamePlatforms(Guid id)
        {
            IEnumerable<DbModels.Platform> platformsFromDb = _gameRepository
                .GetGamePlatforms(id);

            IList<BusinessModels.Platform> platforms = platformsFromDb
                .Select(x => new BusinessModels.Platform
                {
                    PlatformId = x.PlatformId,
                    PlatformName = x.PlatformName,
                })
                .ToList();

            return platforms;
        }

        private IList<BusinessModels.Comment> GetGameComments(Guid id)
        {
            IList<DbModels.Comment> commentsFromDb = _gameRepository
                .GetById(id).Comments;

            var comments = _mapper
                .Map<IList<BusinessModels.Comment>>(commentsFromDb);

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
            }

            return games;
        }

        private BusinessModels.Game Convert(DbModels.Game gameFromDb)
        {
            var game = _mapper.Map<BusinessModels.Game>(gameFromDb);
            game.GameGenres = GetGameGenres(game.GameId);
            game.GamePlatforms = GetGamePlatforms(game.GameId);
            game.Comments = GetGameComments(game.GameId);

            return game;
        }

        private DbModels.Game Convert(BusinessModels.Game game)
        {
            var gameFromDb = _mapper.Map<DbModels.Game>(game);
            gameFromDb.PlatformGames = GetGamePlatforms(
                game.GameId,
                game.GamePlatforms);
            gameFromDb.GenreGames = GetGameGenres(game.GameId, game.GameGenres);

            return gameFromDb;
        }

        private IList<GameGenre> GetGameGenres(
            Guid id,
            IEnumerable<BusinessModels.Genre> genres)
        {
            List<GameGenre> gameGenresList = genres.Select(x => new GameGenre
            {
                GameId = id,
                GenreId = x.GenreId,
            }).ToList();

            return gameGenresList;
        }

        private IList<GamePlatform> GetGamePlatforms(
            Guid id,
            IEnumerable<BusinessModels.Platform> platforms)
        {
            List<GamePlatform> gamePlatformsList = platforms.Select(x => new GamePlatform
            {
                GameId = id,
                PlatformId = x.PlatformId,
            }).ToList();

            return gamePlatformsList;
        }
    }
}
