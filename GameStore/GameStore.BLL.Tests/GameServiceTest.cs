using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Services;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces;
using Moq;
using Xunit;

namespace GameStore.BLL.Tests
{
    public class GameServiceTest
    {
        private readonly GameService _gameService;
        private readonly Mock<IGameRepository> _gameRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;

        private readonly List<Game> _gameList;
        private Guid _gameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019");

        public GameServiceTest()
        {
            _gameList = new List<Game>
            {
                new Game
                {
                    GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                    Key = "mario",
                    Comments = null,
                    GenreGames = null,
                    PlatformGames = null,
                },

                new Game
                {
                    GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                    Key = "mario",
                    Comments = null,
                    GenreGames = null,
                    PlatformGames = null,
                },
            };

            _gameRepository = new Mock<IGameRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();

            _gameService = new GameService(_gameRepository.Object, _unitOfWork.Object, _mapper.Object);
        }

        [Fact]
        public void GetAllGames_ReturnsListOfGames()
        {
            _gameRepository.Setup(g => g.GetAll()).Returns(_gameList);
            _gameRepository.Setup(g => g.GetById(_gameId).Comments)
                .Returns(new List<Comment>
                {
                    new Comment
                    {
                        CommentId = Guid.Parse("357a3d66-24f2-487e-a816-a21531e6a019"),
                        Body = It.IsAny<string>(),
                        Name = It.IsAny<string>(),
                        GameId = _gameId,
                        ParentCommentId = null,
                    },
                });

            IEnumerable<Models.Game> res = _gameService.GetAllGames();
            int count = res.Count();

            _gameRepository.Verify(x => x.GetAll(), Times.Once);
            Assert.IsType<Models.Game>(res.First());
            Assert.Equal(_gameList.Count(), count);
        }

        [Fact]
        public void GetAllGames_ReturnsEmptyList()
        {
            List<Game> games = new List<Game>();

            _gameRepository.Setup(g => g.GetAll()).Returns(games);

            IEnumerable<Models.Game> res = _gameService.GetAllGames();

            Assert.Empty(res);
        }

        [Fact]
        public void GetByKey_PassValidGameKey_ReturnsGameModel()
        {
            _gameRepository.Setup(g => g.GetAll()).Returns(_gameList);
            _gameRepository.Setup(g => g.GetById(_gameId).Comments)
                .Returns(new List<Comment>
                {
                    new Comment
                    {
                        CommentId = Guid.Parse("357a3d66-24f2-487e-a816-a21531e6a019"),
                        Body = It.IsAny<string>(),
                        Name = It.IsAny<string>(),
                        GameId = _gameId,
                        ParentCommentId = null,
                    },
                });

            Models.Game res = _gameService.GetGameByKey("mario");

            Assert.IsType<Models.Game>(res);
        }

        [Fact]
        public void GetByKey_PassWrongGameKey_ReturnNull()
        {
            _gameRepository.Setup(g => g.GetAll()).Returns(_gameList);

            Models.Game res = _gameService.GetGameByKey("wrong");

            Assert.Null(res);
        }

        [Fact]
        public void GetByKeyShould_PassGameKey_ReturnNullBecauseGameIsDeleted()
        {
            _gameList.First().IsRemoved = true;

            _gameRepository.Setup(g => g.GetAll()).Returns(_gameList);

            Models.Game res = _gameService.GetGameByKey("mario");

            Assert.Null(res);
        }

        [Fact]
        public void CreateGame_PassCorrectGameModel_VerifyCreating()
        {
            Models.Game game = new Models.Game
            {
                GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                Key = "mario",
                Comments = null,
                GameGenres = null,
                Description = It.IsAny<string>(),
                Name = "Mario",
                GamePlatforms = new List<Models.Platform>
                    { new Models.Platform { PlatformName = "action", PlatformId = Guid.NewGuid() } },
            };

            _gameService.CreateGame(game);

            _gameRepository.Verify(x => x.Create(It.IsAny<Game>()));
        }

        [Fact]
        public void CreateGame_PassGameModelWithoutPlatform_ThrowsException()
        {
            Models.Game game = new Models.Game
            {
                GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                Key = "mario",
                Comments = null,
                GameGenres = null,
                Description = It.IsAny<string>(),
                Name = "Mario",
                GamePlatforms = null,
            };

            Assert.Throws<Exception>(() => _gameService.CreateGame(game));
        }

        [Fact]
        public void EditGame_PassEmptyGameModel_ThrowsInvalidOperationException()
        {
            Models.Game game = new Models.Game();

            Assert.Throws<ArgumentNullException>(() => _gameService.EditGame(game));
        }

        [Fact]
        public void EditGame_PassCorrectGameModel_VerifyUpdating()
        {
            string gameKey = "mario";
            string gameName = "Mario";

            Models.Game game = new Models.Game
            {
                GameId = _gameId,
                Key = gameKey,
                Name = gameName,
                GameGenres = new List<Models.Genre>
                {
                    new Models.Genre
                    {
                        GenreId = Guid.Parse("357a3d66-24f2-487e-a816-a21531e6a019"),
                        GenreName = "action",
                    },
                },
                GamePlatforms = new List<Models.Platform>
                {
                    new Models.Platform
                    {
                        PlatformId = Guid.Parse("357a3d66-24f2-487e-a816-a21531e6a019"),
                        PlatformName = "mobile",
                    },
                },
            };

            Game gameFromDB = new Game
            {
                GameId = _gameId,
                Key = "NotMario",
                Name = "NotMario",
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
            };

            _gameRepository.Setup(g => g.GetById(_gameId).Comments)
                .Returns(new List<Comment>
                {
                    new Comment
                    {
                        CommentId = Guid.NewGuid(),
                        Body = It.IsAny<string>(),
                        Name = It.IsAny<string>(),
                        GameId = _gameId,
                        ParentCommentId = null,
                    },
                });
            _gameRepository.Setup(g => g.GetById(_gameId)).Returns(gameFromDB);
            _gameService.EditGame(game);

            Assert.Equal(gameKey, gameFromDB.Key);
            Assert.Equal(gameName, gameFromDB.Name);
            Assert.Single(gameFromDB.GenreGames);
            Assert.Single(gameFromDB.PlatformGames);
        }

        [Fact]
        public void EditGame_GameModelWithoutPlatform_ThrowsException()
        {
            Models.Game game = new Models.Game
            {
                GameId = _gameId,
                Key = "mario",
                Name = "Mario",
                GameGenres = new List<Models.Genre>
                {
                    new Models.Genre
                    {
                        GenreId = Guid.Parse("357a3d66-24f2-487e-a816-a21531e6a019"),
                        GenreName = "action",
                    },
                },
            };

            Game gameFromDB = new Game
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

            _gameRepository.Setup(g => g.GetById(_gameId).Comments)
                .Returns(new List<Comment>
                {
                    new Comment
                    {
                        CommentId = Guid.NewGuid(),
                        Body = It.IsAny<string>(),
                        Name = It.IsAny<string>(),
                        GameId = _gameId,
                        ParentCommentId = null,
                    },
                });
            _gameRepository.Setup(g => g.GetById(_gameId)).Returns(gameFromDB);

            Assert.Throws<Exception>(() => _gameService.EditGame(game));
        }

        [Fact]
        public void DeleteGame_PassEmptyGameModel_ThrowsInvalidOperationException()
        {
            Models.Game game = new Models.Game();

            Assert.Throws<NullReferenceException>(() => _gameService.DeleteGame(game));
        }

        [Fact]
        public void DeleteGame_PassAlreadyDeletedGameModel_ThrowsException()
        {
            Models.Game game = new Models.Game
            {
                GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                Key = "mario",
                Name = "Mario",
            };

            Game gameFromDB = new Game
            {
                GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                Key = "mario",
                IsRemoved = true,
                Name = "Mario",
            };

            _gameRepository.Setup(g => g.GetById(Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019")))
                .Returns(gameFromDB);

            Assert.Throws<Exception>(() => _gameService.DeleteGame(game));
        }

        [Fact]
        public void GetGamesByGenre_PassGenreModel_ReturnsListOfGames()
        {
            Models.Genre genre = new Models.Genre
            {
                GenreId = Guid.NewGuid(),
            };

            Genre genreFromDB = new Genre
            {
                GenreId = genre.GenreId,
            };

            Game gameFromDB = new Game
            {
                GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                Key = "mario",
                IsRemoved = true,
                Name = "Mario",
                GenreGames = new List<GameGenre> { new GameGenre { GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"), GenreId = genre.GenreId } },
            };

            _gameRepository.Setup(g => g.GetGameGenres(genre.GenreId)).Returns(new List<Genre> { genreFromDB });
            _gameRepository.Setup(g => g.GetGamesOfGenre(genre.GenreId)).Returns(new List<Game> { gameFromDB });
            _gameRepository.Setup(g => g.GetById(_gameId).Comments)
                .Returns(new List<Comment>
                {
                    new Comment
                    {
                        CommentId = Guid.Parse("357a3d66-24f2-487e-a816-a21531e6a019"),
                        Body = It.IsAny<string>(),
                        Name = It.IsAny<string>(),
                        GameId = _gameId,
                        ParentCommentId = null,
                    },
                });

            Models.Game res = _gameService.GetGamesByGenre(genre).FirstOrDefault();

            Assert.IsType<Models.Game>(res);
        }

        [Fact]
        public void GetGamesByPlatform_PassPlatformModel_ReturnsListOfGames()
        {
            Models.Platform platform = new Models.Platform
            {
                PlatformId = Guid.NewGuid(),
            };

            Platform platformFromDB = new Platform
            {
                PlatformId = platform.PlatformId,
            };

            Game gameFromDB = new Game
            {
                GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"),
                Key = "mario",
                IsRemoved = true,
                Name = "Mario",
                PlatformGames = new List<GamePlatform>
                {
                    new GamePlatform
                    { GameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019"), PlatformId = platform.PlatformId },
                },
            };

            _gameRepository.Setup(g => g.GetGamePlatforms(platform.PlatformId)).Returns(new List<Platform> { platformFromDB });
            _gameRepository.Setup(g => g.GetGamesOfPlatform(platform.PlatformId)).Returns(new List<Game> { gameFromDB });
            _gameRepository.Setup(g => g.GetById(_gameId).Comments)
                .Returns(new List<Comment>
                {
                    new Comment
                    {
                        CommentId = Guid.Parse("357a3d66-24f2-487e-a816-a21531e6a019"),
                        Body = It.IsAny<string>(),
                        Name = It.IsAny<string>(),
                        GameId = _gameId,
                        ParentCommentId = null,
                    },
                });

            Models.Game res = _gameService.GetGamesByPlatform(platform).FirstOrDefault();

            Assert.IsType<Models.Game>(res);
        }
    }
}
