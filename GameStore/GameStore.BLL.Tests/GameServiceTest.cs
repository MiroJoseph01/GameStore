using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Services;
using GameStore.DAL.Entities.SupportingModels;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using GameStore.DAL.Pipeline.Util;
using Moq;
using Xunit;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.BLL.Tests
{
    public class GameServiceTest
    {
        private const string GameId = "457a3d66-24f2-487e-a816-a21531e6a019";

        private readonly GameService _gameService;
        private readonly Mock<IGameRepositoryFacade> _gameRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IViewRepository> _viewRepository;

        private readonly List<DbModels.Game> _gameList;
        private readonly List<BusinessModels.Game> _games;

        public GameServiceTest()
        {
            _games = new List<BusinessModels.Game>
            {
                new BusinessModels.Game
                {
                    GameId = "457a3d66-24f2-487e-a816-a21531e6a019",
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
                    GameId = "457a3d66-24f2-487e-a816-a21531e6a019",
                    Key = "mario",
                    Comments = new List<DbModels.Comment>(),
                    GenreGames = new List<GameGenre>(),
                    PlatformGames = new List<GamePlatform>(),
                },
            };

            _gameRepository = new Mock<IGameRepositoryFacade>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();
            _viewRepository = new Mock<IViewRepository>();

            _gameService = new GameService(
                _gameRepository.Object,
                _unitOfWork.Object,
                _mapper.Object,
                _viewRepository.Object);
        }

        [Fact]
        public void GetAllGames_ReturnsListOfGames()
        {
            _gameRepository
                .Setup(g => g.GetAll())
                .Returns(_gameList);
            _gameRepository
                .Setup(g => g.GetById(GameId))
                .Returns(_gameList.First());
            _mapper
                .Setup(m => m
                    .Map<List<BusinessModels.Game>>(
                        It.IsAny<IEnumerable<DbModels.Game>>()))
                .Returns(_games);

            IEnumerable<BusinessModels.Game> res = _gameService.GetAllGames();
            int count = res.Count();

            _gameRepository
                .Verify(
                    x => x.GetAll(), Times.Once);
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
        public void GetGametByKey_PassValidGameKey_ReturnsGameModel()
        {
            _gameRepository
                .Setup(g => g.GetByKey(It.IsAny<string>()))
                .Returns(_gameList.First());
            _gameRepository
                .Setup(g => g.GetById(GameId))
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
                GameId = "457a3d66-24f2-487e-a816-a21531e6a019",
                Key = "mario",
                Comments = new List<BusinessModels.Comment>(),
                Description = It.IsAny<string>(),
                Name = "Mario",
                GamePlatforms = new List<BusinessModels.Platform>
                {
                    new BusinessModels.Platform
                    {
                        PlatformName = "browser", PlatformId = Guid.NewGuid().ToString(),
                    },
                },
                GameGenres = new List<BusinessModels.Genre>
                {
                    new BusinessModels.Genre
                    {
                        GenreName = "action", GenreId = Guid.NewGuid().ToString(),
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
                GameId = GameId,
                Key = gameKey,
                Name = gameName,
                Comments = new List<BusinessModels.Comment>(),
                GameGenres = new List<BusinessModels.Genre>
                {
                    new BusinessModels.Genre
                    {
                        GenreId = "357a3d66-24f2-487e-a816-a21531e6a019",
                        GenreName = "action",
                    },
                },
                GamePlatforms = new List<BusinessModels.Platform>
                {
                    new BusinessModels.Platform
                    {
                        PlatformId = "357a3d66-24f2-487e-a816-a21531e6a019",
                        PlatformName = "mobile",
                    },
                },
            };

            DbModels.Game gameFromDb = new DbModels.Game
            {
                GameId = GameId,
                Key = gameKey,
                Name = gameName,
                GenreGames = new List<GameGenre>
                {
                    new GameGenre
                    {
                        GameId = GameId,
                        GenreId = Guid.NewGuid().ToString(),
                    },
                    new GameGenre
                    {
                        GameId = GameId,
                        GenreId = "357a3d66-24f2-487e-a816-a21531e6a019",
                    },
                },
                PlatformGames = new List<GamePlatform>
                {
                    new GamePlatform
                    {
                        GameId = GameId,
                        PlatformId = "257a3d66-24f2-487e-a816-a21531e6a019",
                    },
                },
                Comments = new List<DbModels.Comment>(),
            };

            _mapper
                .Setup(m => m.Map<DbModels.Game>(It.IsAny<BusinessModels.Game>()))
                .Returns(gameFromDb);
            _gameRepository
                .Setup(g => g.GetByKey(It.IsAny<string>()))
                .Returns(gameFromDb);
            _gameRepository
                .Setup(g => g.IsPresent(It.IsAny<string>()))
                .Returns(true);
            _gameRepository
                .Setup(g => g.Update(It.IsAny<string>(), It.IsAny<DbModels.Game>(), 0))
                .Returns(true);

            var result = _gameService.EditGame(game);

            _gameRepository.Verify(c => c.Update(It.IsAny<string>(), It.IsAny<DbModels.Game>(), 0));

            Assert.NotNull(result);
        }

        [Fact]
        public void EditGame_GameModelWithoutPlatform_ThrowsException()
        {
            BusinessModels.Game game = new BusinessModels.Game
            {
                GameId = GameId,
                Key = "mario",
                Name = "Mario",
                GameGenres = new List<BusinessModels.Genre>
                {
                    new BusinessModels.Genre
                    {
                        GenreId = "357a3d66-24f2-487e-a816-a21531e6a019",
                        GenreName = "action",
                    },
                },
            };

            DbModels.Game gameFromDb = new DbModels.Game
            {
                GameId = GameId,
                Key = "NotMario",
                Name = "NotMario",
                GenreGames = new List<GameGenre>(),
                PlatformGames = new List<GamePlatform>
                {
                    new GamePlatform
                    {
                        GameId = GameId,
                        PlatformId = "257a3d66-24f2-487e-a816-a21531e6a019",
                    },
                },
            };
            _gameRepository.Setup(g => g.GetById(GameId))
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
                GameId = "457a3d66-24f2-487e-a816-a21531e6a019",
                Key = "mario",
                Name = "Mario",
            };

            DbModels.Game gameFromDb = new DbModels.Game
            {
                GameId = "457a3d66-24f2-487e-a816-a21531e6a019",
                Key = "mario",
                IsRemoved = true,
                Name = "Mario",
            };

            _gameRepository
                .Setup(g => g
                    .GetById("457a3d66-24f2-487e-a816-a21531e6a019"))
                .Returns(gameFromDb);

            Assert.Throws<ArgumentException>(() => _gameService.DeleteGame(game));
        }

        [Fact]
        public void GetGamesByGenre_PassGenreModel_ReturnsListOfGames()
        {
            BusinessModels.Genre genre = new BusinessModels.Genre
            {
                GenreId = Guid.NewGuid().ToString(),
            };

            DbModels.Genre genreFromDb = new DbModels.Genre
            {
                GenreId = genre.GenreId,
            };

            DbModels.Game gameFromDb = new DbModels.Game
            {
                GameId = "457a3d66-24f2-487e-a816-a21531e6a019",
                Key = "mario",
                IsRemoved = true,
                Name = "Mario",
                GenreGames = new List<GameGenre>
                {
                    new GameGenre
                    {
                        GameId = "457a3d66-24f2-487e-a816-a21531e6a019",
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
                 .Setup(g => g.GetById(GameId))
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
                PlatformId = Guid.NewGuid().ToString(),
            };

            DbModels.Platform platformFromDb = new DbModels.Platform
            {
                PlatformId = platform.PlatformId,
            };

            DbModels.Game gameFromDb = new DbModels.Game
            {
                GameId = "457a3d66-24f2-487e-a816-a21531e6a019",
                Key = "mario",
                IsRemoved = true,
                Name = "Mario",
                PlatformGames = new List<GamePlatform>
                {
                    new GamePlatform
                    {
                        GameId = "457a3d66-24f2-487e-a816-a21531e6a019",
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
                .Setup(g => g.GetById(GameId))
                .Returns(_gameList.First());

            BusinessModels.Game res = _gameService
                .GetGamesByPlatform(platform).FirstOrDefault();

            Assert.IsType<BusinessModels.Game>(res);
        }

        [Fact]
        public void Count_ReturnCountOfGames()
        {
            _gameRepository
                .Setup(g => g.GetAll())
                .Returns(_gameList);

            var result = _gameService.Count();

            Assert.Equal(_gameList.Count, result);
        }

        [Fact]
        public void GetOrderOptions_ReturnsListOfOptions()
        {
            var dictionary = new Dictionary<OrderOption, OrderOptionModel>();

            _gameRepository.Setup(g => g.GetOrderOptions()).Returns(dictionary);

            var result = _gameService.GetOrderOptions();

            Assert.IsType<Dictionary<OrderOption, OrderOptionModel>>(result);
        }

        [Fact]
        public void GetTimePeriods_ReturnsListOfPeriods()
        {
            var dictionary = new Dictionary<TimePeriod, TimePeriodModel>();

            _gameRepository.Setup(g => g.GetTimePeriods()).Returns(dictionary);

            var result = _gameService.GetTimePeriods();

            Assert.IsType<Dictionary<TimePeriod, TimePeriodModel>>(result);
        }
    }
}
