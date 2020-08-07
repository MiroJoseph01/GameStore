using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces;

namespace GameStore.BLL.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GameService(IGameRepository gameRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _gameRepository = gameRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void CreateGame(Models.Game game)
        {
            if (string.IsNullOrEmpty(game.Key))
            {
                game.Key = game.Name.ToLower().Replace(" ", "_");
            }

            if (game.GamePlatforms is null || game.GamePlatforms.Count() == 0)
            {
                throw new Exception("Platform is required");
            }

            _gameRepository.Create(ConvertGame(game));

            _unitOfWork.Commit();
        }

        public void DeleteGame(Models.Game game)
        {
            Game gameFromDB = _gameRepository.GetById(game.GameId);

            if (gameFromDB.IsRemoved)
            {
                throw new Exception($"Game with id \'{game.GameId}\' has already deleted");
            }

            gameFromDB.IsRemoved = true;

            _unitOfWork.Commit();
        }

        public Models.Game EditGame(Models.Game game)
        {
            Game gameFromDB = _gameRepository.GetById(game.GameId);

            UpdateGenres(game);
            UpdatePlatforms(game);
            gameFromDB.Key = game.Key;
            gameFromDB.Name = game.Name;
            gameFromDB.Description = game.Description;

            _unitOfWork.Commit();

            return game;
        }

        public IEnumerable<Models.Game> GetAllGames()
        {
            IEnumerable<Game> gamesFromDB = _gameRepository.GetAll();

            return ConvertGames(gamesFromDB);
        }

        public Models.Game GetGameByKey(string key)
        {
            Game gameFromDB = _gameRepository.GetAll().Where(x => x.Key == key).FirstOrDefault();

            if (gameFromDB is null || gameFromDB.IsRemoved)
            {
                return null;
            }

            return ConvertGames(new List<Game> { gameFromDB }).FirstOrDefault();
        }

        public IEnumerable<Models.Game> GetGamesByGenre(Models.Genre genre)
        {
            IEnumerable<Game> gamesFromDB = _gameRepository.GetGamesOfGenre(genre.GenreId);

            return ConvertGames(gamesFromDB);
        }

        public IEnumerable<Models.Game> GetGamesByPlatform(Models.Platform platform)
        {
            IEnumerable<Game> gamesFromDB = _gameRepository.GetGamesOfPlatform(platform.PlatformId);

            return ConvertGames(gamesFromDB);
        }

        public bool IsPresent(string gameKey)
        {
            Game game = _gameRepository.GetByKey(gameKey);

            if (game is null)
            {
                return false;
            }

            return true;
        }

        private IEnumerable<Models.Genre> GetGameGenres(Guid id)
        {
            IEnumerable<Genre> genresFromDB = _gameRepository.GetGameGenres(id);

            IEnumerable<Models.Genre> genres = _mapper.Map<IEnumerable<Models.Genre>>(genresFromDB);

            return genres;
        }

        private IEnumerable<Models.Platform> GetGamePlatforms(Guid id)
        {
            IEnumerable<Platform> platformsFromDB = _gameRepository.GetGamePlatforms(id);

            IEnumerable<Models.Platform> platforms = platformsFromDB.Select(x => new Models.Platform
            {
                PlatformId = x.PlatformId,
                PlatformName = x.PlatformName,
            }).ToList();

            return platforms;
        }

        private IList<Models.Comment> GetGameComments(Guid id)
        {
            IList<Comment> commentsFromDB = _gameRepository.GetById(id).Comments;

            if (commentsFromDB is null)
            {
                return new List<Models.Comment>();
            }
            else
            {
                IList<Models.Comment> comments = _mapper.Map<IList<Models.Comment>>(commentsFromDB);

                return comments;
            }
        }

        private void UpdateGenres(Models.Game game)
        {
            Game gameFromDB = _gameRepository.GetById(game.GameId);

            List<GameGenre> genres = game.GameGenres.Select(x => new GameGenre
            {
                GameId = game.GameId,
                GenreId = x.GenreId,
            }).ToList();

            if (gameFromDB.GenreGames is null)
            {
                gameFromDB.GenreGames = new List<GameGenre>();
            }

            if (game.GameGenres is null)
            {
                game.GameGenres = new List<Models.Genre>();
            }

            gameFromDB.GenreGames.Clear();

            gameFromDB.GenreGames = genres;
        }

        private void UpdatePlatforms(Models.Game game)
        {
            Game gameFromDB = _gameRepository.GetById(game.GameId);

            if (gameFromDB.PlatformGames is null)
            {
                gameFromDB.PlatformGames = new List<GamePlatform>();
            }

            if (game.GamePlatforms is null || game.GamePlatforms.Count() == 0)
            {
                throw new Exception("Platform is requried");
            }
            else
            {
                List<GamePlatform> platforms = game.GamePlatforms.Select(x => new GamePlatform
                {
                    GameId = game.GameId,
                    PlatformId = x.PlatformId,
                }).ToList();

                gameFromDB.PlatformGames.Clear();

                gameFromDB.PlatformGames = platforms;
            }
        }

        private IEnumerable<Models.Game> ConvertGames(IEnumerable<Game> gamesFromDB)
        {
            List<Models.Game> games = gamesFromDB.Select(x => new Models.Game
            {
                GameId = x.GameId,
                Name = x.Name,
                Description = x.Description,
                Key = x.Key,
                GameGenres = GetGameGenres(x.GameId),
                GamePlatforms = GetGamePlatforms(x.GameId),
                Comments = GetGameComments(x.GameId),
            }).ToList();

            return games;
        }

        private Game ConvertGame(Models.Game game)
        {
            Game gameFromDB = new Game
            {
                GameId = game.GameId,
                Name = game.Name,
                Description = game.Description,
                GenreGames = AddGenres(game.GameId, game.GameGenres),
                PlatformGames = AddPlatforms(game.GameId, game.GamePlatforms),
                Key = game.Key,
            };

            return gameFromDB;
        }

        private IList<GameGenre> AddGenres(Guid id, IEnumerable<Models.Genre> genres)
        {
            if (genres is null)
            {
                return new List<GameGenre>();
            }

            List<GameGenre> gameGenresList = genres.Select(x => new GameGenre
            {
                GameId = id,
                GenreId = x.GenreId,
            }).ToList();

            return gameGenresList;
        }

        private IList<GamePlatform> AddPlatforms(Guid id, IEnumerable<Models.Platform> platforms)
        {
            if (platforms is null)
            {
                return new List<GamePlatform>();
            }

            List<GamePlatform> gamePlatformsList = platforms.Select(x => new GamePlatform
            {
                GameId = id,
                PlatformId = x.PlatformId,
            }).ToList();

            return gamePlatformsList;
        }
    }
}
