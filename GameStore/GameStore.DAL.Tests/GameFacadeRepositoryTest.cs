using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Entities.SupportingModels;
using GameStore.DAL.Interfaces.Repositories;
using GameStore.DAL.Pipeline;
using GameStore.DAL.Pipeline.Util;
using GameStore.DAL.Repositories.Facade;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace GameStore.DAL.Tests
{
    public class GameFacadeRepositoryTest
    {
        private readonly GameRepositoryFacade _gameRepository;
        private readonly Mock<IGameRepository> _firstSourceGameRepository;
        private readonly Mock<IGenreRepository> _firstSourceGenreRepository;
        private readonly Mock<IPublisherRepository> _firstSourcePublisherRepository;
        private readonly Mock<IMongoGameRepository> _secondSourceGameRepository;
        private readonly Mock<IMongoGenreRepository> _secondSourceGenreRepository;
        private readonly Mock<IMongoPublisherRepository> _secondSourcePublisherRepository;
        private readonly Mock<IViewRepository> _viewRepository;

        private readonly List<Game> _sqlGames;
        private readonly List<Game> _mongoGames;

        public GameFacadeRepositoryTest()
        {
            _firstSourceGameRepository = new Mock<IGameRepository>();
            _firstSourceGenreRepository = new Mock<IGenreRepository>();
            _firstSourcePublisherRepository = new Mock<IPublisherRepository>();
            _secondSourceGameRepository = new Mock<IMongoGameRepository>();
            _secondSourceGenreRepository = new Mock<IMongoGenreRepository>();
            _secondSourcePublisherRepository = new Mock<IMongoPublisherRepository>();
            _viewRepository = new Mock<IViewRepository>();

            _gameRepository = new GameRepositoryFacade(
                _firstSourceGameRepository.Object,
                _firstSourceGenreRepository.Object,
                _firstSourcePublisherRepository.Object,
                _secondSourceGameRepository.Object,
                _secondSourceGenreRepository.Object,
                _secondSourcePublisherRepository.Object,
                _viewRepository.Object);

            var sqlGameId = Guid.NewGuid().ToString();

            _sqlGames = new List<Game>
            {
                new Game
                {
                    GameId = sqlGameId,
                    Date = DateTime.Now,
                    Description = "Game Description",
                    Comments = null,
                    Discontinued = false,
                    Discount = 0,
                    FromMongo = false,
                    Name = "Game",
                    IsRemoved = false,
                    Price = 10,
                    Key = "game",
                    UnitsInStock = 10,
                    GenreGames = new List<GameGenre>
                    {
                        new GameGenre
                        {
                            GameId = sqlGameId,
                            GenreId = "1",
                        },
                    },
                    PublisherId = "1",
                },
            };

            var mongoGameId = Guid.NewGuid().ToString();

            _mongoGames = new List<Game>
            {
                new Game
                {
                    GameId = mongoGameId,
                    Date = DateTime.Now,
                    Description = "Game Description",
                    Comments = null,
                    Discontinued = false,
                    Discount = 0,
                    FromMongo = true,
                    Name = "Game",
                    IsRemoved = false,
                    Price = 10,
                    Key = "game",
                    UnitsInStock = 10,
                    GenreGames = new List<GameGenre>
                    {
                        new GameGenre
                        {
                            GameId = mongoGameId,
                            GenreId = Guid.NewGuid().ToString(),
                        },
                    },
                    PublisherId = Guid.NewGuid().ToString(),
                },
            };
        }

        [Fact]
        public void AddView_PassValidKeyOfSqlGame_VerifyAddingView()
        {
            _firstSourceGameRepository
                .Setup(g => g.GetByKey(It.IsAny<string>()))
                .Returns(_sqlGames.First());

            _gameRepository.AddView(It.IsAny<string>());

            _firstSourceGameRepository.Verify(x => x.AddView(It.IsAny<string>()));
        }

        [Fact]
        public void AddView_PassValidKeyOfMongoGame_VerifyAddingView()
        {
            _firstSourceGameRepository
                .Setup(g => g.GetByKey(It.IsAny<string>()))
                .Returns((Game)null);
            _secondSourceGameRepository
                .Setup(g => g.GetByKey(It.IsAny<string>()))
                .Returns(_mongoGames.First());

            _gameRepository.AddView(It.IsAny<string>());

            _firstSourceGameRepository.Verify(x => x.AddView(It.IsAny<string>()));
        }

        [Fact]
        public void Create_PassGameModel_VerifyCreating()
        {
            var publisher = new Publisher
            {
                PublisherId = "1",
                CompanyName = "Name",
            };

            var genre = new Genre
            {
                GenreId = "1",
                GenreName = "Name",
            };

            var genres = _sqlGames.First().GenreGames;

            _firstSourcePublisherRepository
                .Setup(p => p.IsPresent(publisher.PublisherId))
                .Returns(true);

            _secondSourcePublisherRepository
                .Setup(p => p.GetById(publisher.PublisherId))
                .Returns(publisher);

            _firstSourceGenreRepository
                .Setup(p => p.IsPresent(It.IsAny<string>()))
                .Returns(true);

            _secondSourceGenreRepository
                .Setup(p => p.GetById(It.IsAny<string>()))
                .Returns(genre);

            _gameRepository.Create(_sqlGames.First());

            _firstSourceGameRepository.Setup(g => g.Create(_sqlGames.First()));
        }

        [Fact]
        public void Delete_PassSqlGameId_VerifyDelete()
        {
            _firstSourceGameRepository
                .Setup(g => g.IsPresent(It.IsAny<string>()))
                .Returns(true);

            _gameRepository.Delete(It.IsAny<string>());

            _firstSourceGameRepository.Verify(g => g.Delete(It.IsAny<string>()));
        }

        [Fact]
        public void Delete_PassMongoGameId_VerifyDelete()
        {
            _firstSourceGameRepository
                .Setup(g => g.IsPresent(It.IsAny<string>()))
                .Returns(false);

            _gameRepository.Delete(It.IsAny<string>());

            _firstSourceGameRepository.Verify(g => g.Create(It.IsAny<Game>()));
        }

        [Fact]
        public void Filter_PassFilterModel_ReturnsListOfGames()
        {
            var expected = _sqlGames.Count + _mongoGames.Count;
            var filterModel = new FilterModel
            {
                Filter = OrderOption.New,
                DateFilter = TimePeriod.LastWeek,
            };

            _firstSourceGameRepository
                .Setup(g => g.Filter(It.IsAny<FilterModel>()))
                .Returns(_sqlGames);

            _secondSourceGameRepository
                .Setup(g => g.Filter(It.IsAny<FilterModel>()))
                .Returns(_mongoGames);

            _firstSourceGameRepository
                .Setup(g => g.GetOrderOptions())
                .Returns(new Dictionary<OrderOption, OrderOptionModel>
                {
                    {
                        OrderOption.New,
                        new OrderOptionModel
                        {
                            Func = x => x.OrderByDescending(y => y.Date),
                        }
                    },
                });

            _firstSourceGameRepository
                .Setup(g => g.GetTimePeriods())
                .Returns(new Dictionary<TimePeriod, TimePeriodModel> {
                    {
                        TimePeriod.LastWeek,
                        new TimePeriodModel
                        {
                            Date = DateTime.Now.AddDays(-7),
                        }
                    },
                });

            var result = _gameRepository.Filter(filterModel);

            Assert.Equal(expected, result.Count());
        }

        [Fact]
        public void Filter_PassFilterModelMostPopularOrder_ReturnsListOfGames()
        {
            var expected = _sqlGames.Count + _mongoGames.Count;
            var filterModel = new FilterModel
            {
                Filter = OrderOption.MostPopular,
                DateFilter = TimePeriod.LastWeek,
                Take = 10,
            };

            _firstSourceGameRepository
                .Setup(g => g.Filter(It.IsAny<FilterModel>()))
                .Returns(_sqlGames);

            _secondSourceGameRepository
                .Setup(g => g.Filter(It.IsAny<FilterModel>()))
                .Returns(_mongoGames);

            _firstSourceGameRepository
                .Setup(g => g.GetOrderOptions())
                .Returns(new Dictionary<OrderOption, OrderOptionModel>
                {
                    {
                        OrderOption.MostPopular,
                        new OrderOptionModel
                        {
                            Func = null,
                        }
                    },
                });

            _firstSourceGameRepository
                .Setup(g => g.GetTimePeriods())
                .Returns(new Dictionary<TimePeriod, TimePeriodModel> {
                    {
                        TimePeriod.LastWeek,
                        new TimePeriodModel
                        {
                            Date = DateTime.Now.AddDays(-7),
                        }
                    },
                });

            _viewRepository
                .Setup(v => v.GetAll())
                .Returns(new List<View>
                {
                    new View
                    {
                        GameId = _sqlGames.First().GameId,
                        Views = 10,
                    },

                    new View
                    {
                        GameId = _mongoGames.First().GameId,
                        Views = 10,
                    },
                });

            var result = _gameRepository.Filter(filterModel);

            Assert.Equal(expected, result.Count());
        }

        [Fact]
        public void GetAll_ReturnsListOfGames()
        {
            _firstSourceGameRepository.Setup(g => g.GetAll()).Returns(_sqlGames);
            _secondSourceGameRepository.Setup(g => g.GetAll()).Returns(_mongoGames);

            var expected = _sqlGames.Count + _mongoGames.Count;

            var result = _gameRepository.GetAll();

            Assert.Equal(expected, result.Count());
        }

        [Fact]
        public void GetById_PassSqlGameId_ReturnsGame()
        {
            _firstSourceGameRepository
                .Setup(g => g.GetById(It.IsAny<string>()))
                .Returns(_sqlGames.First());

            var result = _gameRepository.GetById(It.IsAny<string>());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetById_PassSqlDeletedGameId_ReturnsNull()
        {
            var game = _sqlGames.First();
            game.IsRemoved = true;

            _firstSourceGameRepository
                .Setup(g => g.GetById(It.IsAny<string>()))
                .Returns(game);

            var result = _gameRepository.GetById(It.IsAny<string>());

            Assert.Null(result);
        }

        [Fact]
        public void GetById_PassMongoDeletedGameId_ReturnsNull()
        {
            _firstSourceGameRepository
                .Setup(g => g.GetById(It.IsAny<string>()))
                .Returns((Game)null);

            _secondSourceGameRepository
                .Setup(g => g.GetById(It.IsAny<string>()))
                .Returns(_mongoGames.First());

            var result = _gameRepository.GetById(It.IsAny<string>());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetByKey_PassSqlGameId_ReturnsGame()
        {
            _firstSourceGameRepository
                .Setup(g => g.GetByKey(It.IsAny<string>()))
                .Returns(_sqlGames.First());

            var result = _gameRepository.GetByKey(It.IsAny<string>());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetByKey_PassSqlDeletedGameId_ReturnsNull()
        {
            var game = _sqlGames.First();
            game.IsRemoved = true;

            _firstSourceGameRepository
                .Setup(g => g.GetByKey(It.IsAny<string>()))
                .Returns(game);

            var result = _gameRepository.GetByKey(It.IsAny<string>());

            Assert.Null(result);
        }

        [Fact]
        public void GetByKey_PassMongoDeletedGameId_ReturnsNull()
        {
            _firstSourceGameRepository
                .Setup(g => g.GetByKey(It.IsAny<string>()))
                .Returns((Game)null);

            _secondSourceGameRepository
                .Setup(g => g.GetByKey(It.IsAny<string>()))
                .Returns(_mongoGames.First());

            var result = _gameRepository.GetByKey(It.IsAny<string>());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetGameGenres_PassGameId_ReturnsListOfGenres()
        {
            var sqlGenres = new List<Genre>
                {
                    new Genre
                    {
                        GenreId = "1",
                        GenreName = "Name",
                    },
                };

            var mongoGenres = new List<Genre>
                {
                    new Genre
                    {
                        GenreId = "2",
                        GenreName = "Name",
                    },
                };

            _firstSourceGameRepository
                .Setup(g => g.GetGameGenres(It.IsAny<string>()))
                .Returns(sqlGenres);

            _secondSourceGameRepository
                .Setup(g => g.GetGameGenres(It.IsAny<string>()))
                .Returns(mongoGenres
                );

            var expected = mongoGenres.Count + sqlGenres.Count;

            var result = _gameRepository.GetGameGenres(It.IsAny<string>());

            Assert.Equal(expected, result.Count());
        }

        [Fact]
        public void GetGamePlatforms_PassGameId_ReturnsListOfPlatforms()
        {
            var sqlPlatforms = new List<Platform>
                {
                    new Platform
                    {
                        PlatformId = "1",
                        PlatformName = "Name",
                    },
                };

            var mongoPlatforms = new List<Platform>();

            _firstSourceGameRepository
                .Setup(g => g.GetGamePlatforms(It.IsAny<string>()))
                .Returns(sqlPlatforms);

            _secondSourceGameRepository
                .Setup(g => g.GetGamePlatforms(It.IsAny<string>()))
                .Returns(mongoPlatforms);

            var expected = sqlPlatforms.Count + mongoPlatforms.Count;

            var result = _gameRepository.GetGamePlatforms(It.IsAny<string>());

            Assert.Equal(expected, result.Count());
        }

        [Fact]
        public void GetGamePublisher_PassGameId_ReturnsPublisher()
        {
            var sqlPublisher = new Publisher
            {
                PublisherId = "1",
                CompanyName = "Name",
            };

            var mongoPublishers = new List<Platform>();

            _firstSourceGameRepository
                .Setup(g => g.GetGamePublisher(It.IsAny<string>()))
                .Returns(sqlPublisher);

            var result = _gameRepository.GetGamePublisher(It.IsAny<string>());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetGamePublisher_PassGameIdWithPublisherFromMongo_ReturnsPublisher()
        {
            var publisher = new Publisher
            {
                PublisherId = "1",
                CompanyName = "Name",
            };

            var mongoPublishers = new List<Platform>();

            _firstSourceGameRepository
                .Setup(g => g.GetGamePublisher(It.IsAny<string>()))
                .Returns((Publisher)null);

            _secondSourceGameRepository
                .Setup(g => g.GetGamePublisher(It.IsAny<string>()))
                .Returns(publisher);

            var result = _gameRepository.GetGamePublisher(It.IsAny<string>());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetGamesOfGenre_PassGenreId_ReturnsListOfGames()
        {
            _firstSourceGameRepository
                .Setup(g => g.GetGamesOfGenre(It.IsAny<string>()))
                .Returns(_sqlGames);

            _secondSourceGameRepository
                .Setup(g => g.GetGamesOfGenre(It.IsAny<string>()))
                .Returns(_mongoGames);

            var expected = _sqlGames.Count + _mongoGames.Count;

            var result = _gameRepository.GetGamesOfGenre(It.IsAny<string>());

            Assert.Equal(expected, result.Count());
        }

        [Fact]
        public void GetGamesOfPublisher_PassPublisherId_ReturnsListOfGames()
        {
            _firstSourceGameRepository
                .Setup(g => g.GetGamesOfPublisher(It.IsAny<string>()))
                .Returns(_sqlGames);

            _secondSourceGameRepository
                .Setup(g => g.GetGamesOfPublisher(It.IsAny<string>()))
                .Returns(_mongoGames);

            var expected = _sqlGames.Count + _mongoGames.Count;

            var result = _gameRepository.GetGamesOfPublisher(It.IsAny<string>());

            Assert.Equal(expected, result.Count());
        }

        [Fact]
        public void GetGamesOfPlatform_PassPlatformId_ReturnsListOfGames()
        {
            _firstSourceGameRepository
                .Setup(g => g.GetGamesOfPlatform(It.IsAny<string>()))
                .Returns(_sqlGames);

            _secondSourceGameRepository
                .Setup(g => g.GetGamesOfPlatform(It.IsAny<string>()))
                .Returns(new List<Game>());

            var expected = _sqlGames.Count;

            var result = _gameRepository.GetGamesOfPlatform(It.IsAny<string>());

            Assert.Equal(expected, result.Count());
        }

        [Fact]
        public void IsPresent_PassGameId_ReturnsTrue()
        {
            _firstSourceGameRepository
                .Setup(g => g.IsPresent(It.IsAny<string>()))
                .Returns(true);

            _secondSourceGameRepository
                .Setup(g => g.IsPresent(It.IsAny<string>()))
                .Returns(true);

            var result = _gameRepository.IsPresent(It.IsAny<string>());

            Assert.True(result);
        }

        [Fact]
        public void IsPresent_PassGameId_ReturnsFalse()
        {
            _firstSourceGameRepository
                .Setup(g => g.IsPresent(It.IsAny<string>()))
                .Returns(false);

            _secondSourceGameRepository
                .Setup(g => g.IsPresent(It.IsAny<string>()))
                .Returns(false);

            var result = _gameRepository.IsPresent(It.IsAny<string>());

            Assert.False(result);
        }

        [Fact]
        public void Update_PassGameIdEntityNumberOfUnits_ReturnsTrue()
        {
            var game = _sqlGames.First();
            game.FromMongo = true;

            var publisher = new Publisher
            {
                PublisherId = "1",
                CompanyName = "Name",
            };

            var genre = new Genre
            {
                GenreId = "1",
                GenreName = "Name",
            };

            var genres = _sqlGames.First().GenreGames;

            _firstSourcePublisherRepository
                .Setup(p => p.IsPresent(publisher.PublisherId))
                .Returns(true);

            _secondSourcePublisherRepository
                .Setup(p => p.GetById(publisher.PublisherId))
                .Returns(publisher);

            _firstSourceGenreRepository
                .Setup(p => p.IsPresent(It.IsAny<string>()))
                .Returns(true);

            _secondSourceGenreRepository
                .Setup(p => p.GetById(It.IsAny<string>()))
                .Returns(genre);

            _secondSourceGameRepository.Setup(g => g.GetById(It.IsAny<string>())).Returns(_mongoGames.First());

            _firstSourceGameRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(true);

            _firstSourceGameRepository.Setup(g => g.GetById(It.IsAny<string>())).Returns(game);

            var result = _gameRepository.Update(It.IsAny<string>(), game, -2);

            _firstSourceGameRepository.Verify(x => x.Update(It.IsAny<string>(), It.IsAny<Game>(), 0));

            Assert.True(result);
        }

        [Fact]
        public void Update_PassGameIdEntityNumberOfUnits_ReturnsFalse()
        {
            var game = _sqlGames.First();
            game.FromMongo = true;

            var gameFromMongo = _mongoGames.First();
            gameFromMongo.UnitsInStock = 0;

            var publisher = new Publisher
            {
                PublisherId = "1",
                CompanyName = "Name",
            };

            var genre = new Genre
            {
                GenreId = "1",
                GenreName = "Name",
            };

            var genres = _sqlGames.First().GenreGames;

            _firstSourcePublisherRepository
                .Setup(p => p.IsPresent(publisher.PublisherId))
                .Returns(true);

            _secondSourcePublisherRepository
                .Setup(p => p.GetById(publisher.PublisherId))
                .Returns(publisher);

            _firstSourceGenreRepository
                .Setup(p => p.IsPresent(It.IsAny<string>()))
                .Returns(true);

            _secondSourceGenreRepository
                .Setup(p => p.GetById(It.IsAny<string>()))
                .Returns(genre);

            _secondSourceGameRepository.Setup(g => g.GetById(It.IsAny<string>())).Returns(gameFromMongo);

            _firstSourceGameRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(false);

            _firstSourceGameRepository.Setup(g => g.GetById(It.IsAny<string>())).Returns(game);

            var result = _gameRepository.Update(It.IsAny<string>(), game, -2);

            _firstSourceGameRepository.Verify(x => x.Create(It.IsAny<Game>()));

            Assert.False(result);
        }
    }
}
