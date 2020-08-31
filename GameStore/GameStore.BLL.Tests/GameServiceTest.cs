using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Services;
using GameStore.DAL.Entities.SupportingModels;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using Moq;
using Xunit;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.BLL.Tests
{
    public class GameServiceTest
    {
        private readonly GameService _gameService;
        private readonly Mock<IGameRepository> _gameRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;

        private readonly List<DbModels.Game> _gameList;
        private readonly List<BusinessModels.Game> _games;
        private Guid _gameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019");

        public GameServiceTest()
        {
            _games = new List<BusinessModels.Game>
            {
                new BusinessModels.Game
                {
                    GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                    Key = "mario",
                    Comments = new List<BusinessModels.Comment>(),
                    GameGenres = new List<BusinessModels.Genre>(),
                    GamePlatforms = new List<BusinessModels.Platform>(),
                },
            };

            _gameList = new List<DbModels.Game>
            {
                new DbModels.Game
                {
                    GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                    Key = "mario",
                    Comments = new List<DbModels.Comment>(),
                    GenreGames = new List<GameGenre>(),
                    PlatformGames = new List<GamePlatform>(),
                },
            };

            _gameRepository = new Mock<IGameRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();

            _gameService = new GameService(
                _gameRepository.Object,
                _unitOfWork.Object,
                _mapper.Object);
        }

        [Fact]
        public void GetAllGames_ReturnsListOfGames()
        {
            _gameRepository.Setup(g => g.GetAll()).Returns(_gameList);
            _gameRepository
                .Setup(g => g.GetById(_gameId))
                .Returns(_gameList.First());
            _mapper
                .Setup(m => m
                    .Map<List<BusinessModels.Game>>(
                        It.IsAny<IEnumerable<DbModels.Game>>()))
                .Returns(_games);

            IEnumerable<BusinessModels.Game> res = _gameService.GetAllGames();
            int count = res.Count();

            _gameRepository.Verify(x => x.GetAll(), Times.Once);
            Assert.IsType<BusinessModels.Game>(res.First());
            Assert.Equal(_gameList.Count(), count);
        }

        [Fact]
        public void GetAllGames_ReturnsEmptyList()
        {
            List<BusinessModels.Game> games = new List<BusinessModels.Game>();

            _gameRepository.Setup(g => g.GetAll()).Returns(_gameList);

            _mapper
                .Setup(m => m
                    .Map<List<BusinessModels.Game>>(
                        It.IsAny<IEnumerable<DbModels.Game>>()))
                .Returns(games);

            IEnumerable<BusinessModels.Game> res = _gameService.GetAllGames();

            Assert.Empty(res);
        }

        [Fact]
        public void GeGametByKey_PassValidGameKey_ReturnsGameModel()
        {
            _gameRepository
                .Setup(g => g.GetByKey(It.IsAny<string>()))
                .Returns(_gameList.First());
            _gameRepository
                .Setup(g => g.GetById(_gameId))
                .Returns(_gameList.First());
            _mapper
                .Setup(m => m.Map<BusinessModels.Game>(It.IsAny<DbModels.Game>()))
                .Returns(_games.First());

            BusinessModels.Game res = _gameService.GetGameByKey("mario");

            Assert.IsType<BusinessModels.Game>(res);
        }

        [Fact]
        public void GetGameByKey_PassWrongGameKey_ReturnNull()
        {
            _gameRepository
                .Setup(g => g.GetByKey(It.IsAny<string>()))
                .Returns((DbModels.Game)null);

            BusinessModels.Game res = _gameService.GetGameByKey("wrong");

            Assert.Null(res);
        }

        [Fact]
        public void CreateGame_PassCorrectGameModel_VerifyCreating()
        {
            BusinessModels.Game game = new BusinessModels.Game
            {
                GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                Key = "mario",
                Comments = new List<BusinessModels.Comment>(),
                Description = It.IsAny<string>(),
                Name = "Mario",
                GamePlatforms = new List<BusinessModels.Platform>
                {
                    new BusinessModels.Platform
                    {
                        PlatformName = "browser", PlatformId = Guid.NewGuid(),
                    },
                },
                GameGenres = new List<BusinessModels.Genre>
                {
                    new BusinessModels.Genre
                    {
                        GenreName = "action", GenreId = Guid.NewGuid(),
                    },
                },
            };

            _mapper
                .Setup(m => m
                    .Map<DbModels.Game>(
                        It.IsAny<BusinessModels.Game>()))
                .Returns(_gameList.First);

            _gameService.CreateGame(game);

            _gameRepository.Verify(x => x.Create(It.IsAny<DbModels.Game>()));
        }

        [Fact]
        public void CreateGame_PassGameModelWithoutPlatform_ThrowsException()
        {
            BusinessModels.Game game = new BusinessModels.Game
            {
                GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                Key = "mario",
                Comments = null,
                GameGenres = new List<BusinessModels.Genre>(),
                Description = It.IsAny<string>(),
                Name = "Mario",
                GamePlatforms = new List<BusinessModels.Platform>(),
            };

            Assert.Throws<ArgumentException>(() => _gameService.CreateGame(game));
        }

        [Fact]
        public void EditGame_PassEmptyGameModel_ThrowsInvalidOperationException()
        {
            BusinessModels.Game game = new BusinessModels.Game();

            Assert.Throws<ArgumentException>(() => _gameService.EditGame(game));
        }

        [Fact]
        public void EditGame_PassCorrectGameModel_VerifyUpdating()
        {
            string gameKey = "mario";
            string gameName = "Mario";

            BusinessModels.Game game = new BusinessModels.Game
            {
                GameId = _gameId,
                Key = gameKey,
                Name = gameName,
                Comments = new List<BusinessModels.Comment>(),
                GameGenres = new List<BusinessModels.Genre>
                {
                    new BusinessModels.Genre
                    {
                        GenreId = Guid.Parse("357a3d66-24f2-487e-a816-a21531e6a019"),
                        GenreName = "action",
                    },
                },
                GamePlatforms = new List<BusinessModels.Platform>
                {
                    new BusinessModels.Platform
                    {
                        PlatformId = Guid.Parse("357a3d66-24f2-487e-a816-a21531e6a019"),
                        PlatformName = "mobile",
                    },
                },
            };

            DbModels.Game gameFromDb = new DbModels.Game
            {
                GameId = _gameId,
                Key = gameKey,
                Name = gameName,
                GenreGames = new List<GameGenre>
                {
                    new GameGenre
                    {
                        GameId = _gameId,
                        GenreId = Guid.NewGuid(),
                    },
                    new GameGenre
                    {
                        GameId = _gameId,
                        GenreId = Guid.Parse("357a3d66-24f2-487e-a816-a21531e6a019"),
                    },
                },
                PlatformGames = new List<GamePlatform>
                {
                    new GamePlatform
                    {
                        GameId = _gameId,
                        PlatformId = Guid.Parse("257a3d66-24f2-487e-a816-a21531e6a019"),
                    },
                },
                Comments = new List<DbModels.Comment>(),
            };

            _gameRepository
                .Setup(g => g.IsPresent(It.IsAny<Guid>()))
                .Returns(true);
            _gameRepository.Setup(g => g.GetById(_gameId)).Returns(gameFromDb);
            _mapper
                .Setup(m => m.Map<DbModels.Game>(game))
                .Returns(gameFromDb);

            _gameService.EditGame(game);

            Assert.Equal(gameKey, gameFromDb.Key);
            Assert.Equal(gameName, gameFromDb.Name);
            Assert.Single(gameFromDb.GenreGames);
            Assert.Single(gameFromDb.PlatformGames);
        }

        [Fact]
        public void EditGame_GameModelWithoutPlatform_ThrowsException()
        {
            BusinessModels.Game game = new BusinessModels.Game
            {
                GameId = _gameId,
                Key = "mario",
                Name = "Mario",
                GameGenres = new List<BusinessModels.Genre>
                {
                    new BusinessModels.Genre
                    {
                        GenreId = Guid.Parse("357a3d66-24f2-487e-a816-a21531e6a019"),
                        GenreName = "action",
                    },
                },
            };

            DbModels.Game gameFromDb = new DbModels.Game
            {
                GameId = _gameId,
                Key = "NotMario",
                Name = "NotMario",
                GenreGames = new List<GameGenre>(),
                PlatformGames = new List<GamePlatform>
                {
                    new GamePlatform
                    {
                        GameId = _gameId,
                        PlatformId = Guid.Parse("257a3d66-24f2-487e-a816-a21531e6a019"),
                    },
                },
            };
            _gameRepository.Setup(g => g.GetById(_gameId))
                .Returns(_gameList.First());

            Assert.Throws<ArgumentException>(() => _gameService.EditGame(game));
        }

        [Fact]
        public void DeleteGame_PassEmptyGameModel_ThrowsInvalidOperationException()
        {
            BusinessModels.Game game = new BusinessModels.Game();

            Assert.Throws<ArgumentException>(() => _gameService.DeleteGame(game));
        }

        [Fact]
        public void DeleteGame_PassAlreadyDeletedGameModel_ThrowsException()
        {
            BusinessModels.Game game = new BusinessModels.Game
            {
                GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                Key = "mario",
                Name = "Mario",
            };

            DbModels.Game gameFromDb = new DbModels.Game
            {
                GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                Key = "mario",
                IsRemoved = true,
                Name = "Mario",
            };

            _gameRepository
                .Setup(g => g
                    .GetById(Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019")))
                .Returns(gameFromDb);

            Assert.Throws<ArgumentException>(() => _gameService.DeleteGame(game));
        }

        [Fact]
        public void GetGamesByGenre_PassGenreModel_ReturnsListOfGames()
        {
            BusinessModels.Genre genre = new BusinessModels.Genre
            {
                GenreId = Guid.NewGuid(),
            };

            DbModels.Genre genreFromDb = new DbModels.Genre
            {
                GenreId = genre.GenreId,
            };

            DbModels.Game gameFromDb = new DbModels.Game
            {
                GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                Key = "mario",
                IsRemoved = true,
                Name = "Mario",
                GenreGames = new List<GameGenre>
                {
                    new GameGenre
                    {
                        GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                        GenreId = genre.GenreId,
                    },
                },
            };

            _mapper
                .Setup(m => m
                    .Map<List<BusinessModels.Game>>(
                        It.IsAny<IEnumerable<DbModels.Game>>()))
                .Returns(_games);
            _gameRepository
                .Setup(g => g
                    .GetGameGenres(genre.GenreId))
                .Returns(new List<DbModels.Genre> { genreFromDb });
            _gameRepository
                .Setup(g => g.GetGamesOfGenre(genre.GenreId))
                .Returns(new List<DbModels.Game> { gameFromDb });
            _gameRepository
                 .Setup(g => g.GetById(_gameId))
                 .Returns(_gameList.First());

            BusinessModels.Game res = _gameService
                .GetGamesByGenre(genre).FirstOrDefault();

            Assert.IsType<BusinessModels.Game>(res);
        }

        [Fact]
        public void GetGamesByPlatform_PassPlatformModel_ReturnsListOfGames()
        {
            BusinessModels.Platform platform = new BusinessModels.Platform
            {
                PlatformId = Guid.NewGuid(),
            };

            DbModels.Platform platformFromDb = new DbModels.Platform
            {
                PlatformId = platform.PlatformId,
            };

            DbModels.Game gameFromDb = new DbModels.Game
            {
                GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                Key = "mario",
                IsRemoved = true,
                Name = "Mario",
                PlatformGames = new List<GamePlatform>
                {
                    new GamePlatform
                    {
                        GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                        PlatformId = platform.PlatformId,
                    },
                },
            };

            _mapper
                .Setup(m => m
                    .Map<List<BusinessModels.Game>>(
                        It.IsAny<IEnumerable<DbModels.Game>>()))
                .Returns(_games);
            _gameRepository
                .Setup(g => g
                    .GetGamePlatforms(platform.PlatformId))
                .Returns(new List<DbModels.Platform> { platformFromDb });
            _gameRepository
                .Setup(g => g.GetGamesOfPlatform(platform.PlatformId))
                .Returns(new List<DbModels.Game> { gameFromDb });
            _gameRepository
                .Setup(g => g.GetById(_gameId))
                .Returns(_gameList.First());

            BusinessModels.Game res = _gameService
                .GetGamesByPlatform(platform).FirstOrDefault();

            Assert.IsType<BusinessModels.Game>(res);
        }

        [Fact]
        public void Count_ReturnCountOfGames()
        {
            _gameRepository.Setup(g => g.GetAll()).Returns(_gameList);

            var result = _gameService.Count();

            Assert.Equal(_gameList.Count, result);
        }
    }
}
