using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces.Services;
using GameStore.BLL.Models;
using GameStore.DAL.Pipeline;
using GameStore.DAL.Pipeline.Util;
using GameStore.Web.Controllers;
using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Xunit;

namespace GameStore.Web.Tests
{
    public class GamesControllerTest
    {
        private readonly GameController _gamesController;
        private readonly Mock<IGameService> _gameService;
        private readonly Mock<IFileService> _fileService;
        private readonly Mock<IGenreService> _genreService;
        private readonly Mock<IPlatformService> _platformService;
        private readonly Mock<IPublisherService> _publisherService;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IPipeline> _pipeline;

        private readonly List<Game> _games;
        private readonly List<GameViewModel> _gamesVM;
        private readonly List<GameCreateViewModel> _gameCreateVM;

        private readonly List<Platform> _platforms;
        private readonly List<PlatformViewModel> _platformsVM;

        private readonly List<Genre> _genres;
        private readonly List<GenreViewModel> _genresVM;

        private readonly List<Publisher> _publishers;

        private readonly IDictionary<OrderOption, OrderOptionModel> _orderOptions;
        private readonly IDictionary<TimePeriod, TimePeriodModel> _timePeriods;

        public GamesControllerTest()
        {
            _gameService = new Mock<IGameService>();
            _fileService = new Mock<IFileService>();
            _platformService = new Mock<IPlatformService>();
            _genreService = new Mock<IGenreService>();
            _publisherService = new Mock<IPublisherService>();
            _mapper = new Mock<IMapper>();
            _pipeline = new Mock<IPipeline>();

            _gamesController = new GameController(
                _gameService.Object,
                _fileService.Object,
                _genreService.Object,
                _platformService.Object,
                _publisherService.Object,
                _mapper.Object,
                _pipeline.Object);

            _games = new List<Game>
            {
                new Game
                {
                    GameId = Guid.NewGuid().ToString(),
                    Key = "mario",
                    Name = "Mario",
                },
            };

            _gamesVM = new List<GameViewModel>
            {
                new GameViewModel
                {
                   GameId = "f38358a2-4724-403c-981e-ffcd3511e805",
                   Key = "mario",
                   Name = "Mario",
                   Description = "Game Description",
                   Comments = null,
                   Platforms = new List<PlatformViewModel>
                    {
                        new PlatformViewModel
                        {
                            PlatformId = Guid.NewGuid().ToString(),
                            PlatformName = "browser",
                        },
                    },
                },
            };

            _gameCreateVM = new List<GameCreateViewModel>
            {
                new GameCreateViewModel
                {
                    Key = "mario",
                    Name = "Mario",
                    Description = It.IsAny<string>(),
                    Comments = null,
                    PublisherName = "NoPublisher",
                    PlatformOptions = new List<SelectListItem>
                    {
                        new SelectListItem
                        {
                            Value = Guid.NewGuid().ToString(),
                            Text = "browser",
                            Selected = true,
                        },
                    },
                },
            };

            _platforms = new List<Platform>
            {
                new Platform
                {
                    PlatformId = Guid.NewGuid().ToString(),
                    PlatformName = "platform",
                },
            };

            _platformsVM = new List<PlatformViewModel>
            {
                new PlatformViewModel
                {
                    PlatformId = _platforms.First().PlatformId.ToString(),
                    PlatformName = "platform",
                },
            };

            _genres = new List<Genre>
            {
                new Genre
                {
                    GenreId = Guid.NewGuid().ToString(),
                    GenreName = "Genre",
                },
            };

            _genresVM = new List<GenreViewModel>
            {
                new GenreViewModel
                {
                    GenreId = _genres.First().GenreId.ToString(),
                    GenreName = "Genre",
                },
            };

            _publishers = new List<Publisher>
            {
                new Publisher
                {
                    PublisherId = Guid.NewGuid().ToString(),
                    Description = "Description",
                    CompanyName = "Name",
                    HomePage = "link",
                },
            };

            _orderOptions = new Dictionary<OrderOption, OrderOptionModel>
            {
                {
                    OrderOption.New,
                    new OrderOptionModel
                    {
                        Name = "New",
                        Text = "Newest",
                        Func = x => x.OrderByDescending(y => y.Date),
                    }
                },
                {
                    OrderOption.MostCommented,
                    new OrderOptionModel
                    {
                        Name = "MostCommented",
                        Text = "Most Commented",
                        Func = x => x.OrderByDescending(y => y.Comments.Count),
                    }
                },
            };

            _timePeriods = new Dictionary<TimePeriod, TimePeriodModel>
            {
                {
                    TimePeriod.LastWeek,
                    new TimePeriodModel
                    {
                        Text = "Last Week",
                        Name = "LastWeek",
                        Date = DateTime.Now.AddDays(-7),
                    }
                },
                {
                    TimePeriod.LastMonths,
                    new TimePeriodModel
                    {
                        Text = "Last Months",
                        Name = "LastMonths",
                        Date = DateTime.Now.AddMonths(-1),
                    }
                },
            };
        }

        [Fact]
        public void Index_ReturnsListOfGames()
        {
            var query = new BLL.Models.QueryModel
            {
                PlatformOptions = new List<string>
                {
                    "browser",
                },

                GenresOptions = new List<string>
                {
                   "Action"
                },

                PublisherOptions = new List<string>
                {
                    "Valve"
                },

                DateFilter = TimePeriod.LastYear,
                Filter = OrderOption.New,
                From = 10,
                To = 30,
                SearchByGameName = "Game Name",
                Skip = 1,
                Take = 10,
            };

            var shortGames = new List<ShortGameViewModel>
            {
                new ShortGameViewModel()
            };

            _gameService
                .Setup(g => g.GetOrderOptions())
                .Returns(_orderOptions);
            _gameService
                .Setup(g => g.GetTimePeriods())
                .Returns(_timePeriods);

            _pipeline
                .Setup(p => p.Register(It.IsAny<IFilter<DAL.Entities.Game>>()))
                .Returns(_pipeline.Object);

            _mapper
                .Setup(
                    m => m.Map<IEnumerable<ShortGameViewModel>>(
                        It.IsAny<List<Game>>()))
                .Returns(shortGames);
            _mapper
                .Setup(m => m.Map<BLL.Models.QueryModel>(It.IsAny<ViewModels.QueryModel>()))
                .Returns(query);

            IActionResult result = _gamesController.Index(new ViewModels.QueryModel());

            ViewResult view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ShortsGameViewModel>(view.Model);
        }

        [Fact]
        public void Index_PassQueryViewModelWitnInvalidValues_ReturnsFilteredListOfGames()
        {
            var query = new BLL.Models.QueryModel()
            {
                To = 10,
                From = 20,
                SearchByGameName = "2",
                Take = 0,
                Skip = 1,
            };

            var shortGames = new List<ShortGameViewModel>
            {
                new ShortGameViewModel(),
                new ShortGameViewModel(),
            };

            _gameService
                .Setup(g => g.GetAllGames())
                .Returns(_games);
            _gameService
                .Setup(g => g.GetOrderOptions())
                .Returns(_orderOptions);
            _gameService
                .Setup(g => g.GetTimePeriods())
                .Returns(_timePeriods);
            _mapper
                .Setup(m => m.Map<IEnumerable<ShortGameViewModel>>(
                    It.IsAny<IEnumerable<Game>>()))
                .Returns(shortGames);
            _mapper
                .Setup(m => m.Map<BLL.Models.QueryModel>(It.IsAny<ViewModels.QueryModel>()))
                .Returns(query);

            IActionResult result = _gamesController.Index(new ViewModels.QueryModel());

            ViewResult view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ShortsGameViewModel>(view.Model);
            Assert.Equal(model.Games.Count, shortGames.Count);
        }

        [Fact]
        public void ViewGameDetails_PassCorrectGameKey_ReturnsGameModel()
        {
            var game = _games.First();

            _gameService.Setup(g => g.IsPresent(game.Key)).Returns(true);
            _gameService.Setup(g => g.GetGameByKey(game.Key)).Returns(game);
            _mapper.Setup(m => m.Map<GameViewModel>(game)).Returns(_gamesVM.First());

            IActionResult result = _gamesController.ViewGameDetails(game.Key);

            ViewResult view = Assert.IsType<ViewResult>(result);
            GameViewModel model = Assert.IsAssignableFrom<GameViewModel>(
               view.Model);
            Assert.IsType<GameViewModel>(model);
        }

        [Fact]
        public void ViewGameDetails_PassWrongGameKey_ReturnsNotFoundObjectResult()
        {
            Game game = _games.First();
            const string WrongKey = "WRONG";

            _gameService.Setup(g => g.IsPresent(game.Key)).Returns(false);
            _gameService.Setup(g => g.GetGameByKey(game.Key)).Returns(game);

            IActionResult result = _gamesController.ViewGameDetails(WrongKey);

            NotFoundResult view = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, view.StatusCode);
        }

        [Fact]
        public void CreateNewGame_PassGameViewModel_VerifyAdding()
        {
            GameCreateViewModel gameCreateVM = _gameCreateVM.First();
            Game game = _games.First();

            _gameService.Setup(g => g.GetGameByKey(gameCreateVM.Key))
                .Returns(game);
            _mapper
                .Setup(m => m.Map<Game>(It.IsAny<GameCreateViewModel>()))
                .Returns(game);

            IActionResult result = _gamesController.CreateNewGame(gameCreateVM);

            _gameService.Verify(g => g.CreateGame(It.IsAny<Game>()));

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void CreateNewGame_ReturnsViewForCreatingGame()
        {
            _platformService.Setup(p => p.GetAllPlatforms()).Returns(_platforms);
            _mapper.Setup(m => m.Map<IList<PlatformViewModel>>(_platforms)).Returns(_platformsVM);
            _publisherService.Setup(p => p.GetAllPublishers()).Returns(_publishers);
            _mapper.Setup(m => m.Map<IList<GenreViewModel>>(_genres)).Returns(_genresVM);
            _genreService.Setup(g => g.GetAllGenres()).Returns(_genres);

            var result = _gamesController.CreateNewGame();

            ViewResult view = Assert.IsType<ViewResult>(result);
            GameCreateViewModel model = Assert.IsAssignableFrom<GameCreateViewModel>(view.Model);
        }

        [Fact]
        public void CreateNewGame_PassInvalidModel_ReturnView()
        {
            var game = _gameCreateVM.First();
            game.PlatformOptions.First().Selected = false;

            _platformService.Setup(p => p.GetAllPlatforms()).Returns(_platforms);
            _mapper.Setup(m => m.Map<IList<PlatformViewModel>>(_platforms)).Returns(_platformsVM);
            _publisherService.Setup(p => p.GetAllPublishers()).Returns(_publishers);
            _mapper.Setup(m => m.Map<IList<GenreViewModel>>(_genres)).Returns(_genresVM);
            _genreService.Setup(g => g.GetAllGenres()).Returns(_genres);

            var result = _gamesController.CreateNewGame(game);

            ViewResult view = Assert.IsType<ViewResult>(result);
            GameCreateViewModel model = Assert.IsAssignableFrom<GameCreateViewModel>(view.Model);
        }

        [Fact]
        public void Update_PassGameViewModel_VerifyUpdating()
        {
            GameViewModel gameViewModel = _gamesVM.First();
            Game game = _games.First();

            _gameService.Setup(g => g.EditGame(It.IsAny<Game>(), 0))
                .Returns(game);
            _mapper.Setup(m => m.Map<Game>(_gamesVM));

            IActionResult result = _gamesController.Update(gameViewModel);

            _gameService.Verify(g => g.EditGame(It.IsAny<Game>(), 0));

            RedirectToActionResult view = Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Delete_PassGameViewModel_VerifyDeliting()
        {
            var game = _games.First();

            _gameService.Setup(g => g.IsPresent(game.Key)).Returns(true);
            _gameService.Setup(g => g.GetGameByKey(game.Key)).Returns(game);

            IActionResult result = _gamesController.Delete(game.Key);

            _gameService.Verify(g => g.DeleteGame(It.IsAny<Game>()));

            RedirectToActionResult view = Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Delete_PassNonExistingGameViewModel_ReturnsNotFound()
        {
            var game = _games.First();

            _gameService.Setup(g => g.IsPresent(game.Key)).Returns(false);
            _gameService.Setup(g => g.GetGameByKey(game.Key)).Returns((Game)null);

            IActionResult result = _gamesController.Delete(game.Key);

            NotFoundResult view = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Download_ReturnsHttpResponseMessage()
        {
            var result = _gamesController.Download();

            _fileService.Verify(f => f.CreateFile(_gamesController));
        }

        [Fact]
        public void CreateNewPublisher_PassInalidModel_ReturnsRedirect()
        {
            _gamesController.ModelState.AddModelError("HomePage", "Invalid");
            var result = _gamesController
                .CreateNewPublisher(It.IsAny<PublisherCreateViewModel>());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ViewPublisher_PassEmptyOrInvalidName_ReturnsNotFound()
        {
            string wrongPublisherName = "wrong";
            _publisherService
                .Setup(p => p.IsPresent(wrongPublisherName))
                .Returns(false);

            var result = _gamesController.ViewPublisher("");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ViewPublisher_PassName_ReturnsNotFound()
        {
            var publisherView = new PublisherViewModel
            {
                CompanyName = "company name",
                Description = "description",
                HomePage = "home page",
                PublisherId = _publishers.First().PublisherId.ToString(),
            };

            _publisherService
                .Setup(p => p.IsPresent(_publishers.First().CompanyName))
                .Returns(true);
            _publisherService
                .Setup(p => p.GetPublisherByName(It.IsAny<string>()))
                .Returns(_publishers.First());
            _gameService
                .Setup(g => g.GetGamesByPublisher(It.IsAny<Publisher>()))
                .Returns((IEnumerable<Game>)null);
            _mapper
                .Setup(m => m.Map<PublisherViewModel>(_publishers.First()))
                .Returns(publisherView);
            _mapper
                .Setup(m => m.Map<IEnumerable<GameViewModel>>(_publishers.First()))
                .Returns((IEnumerable<GameViewModel>)null);

            var result = _gamesController
                .ViewPublisher(_publishers.First().CompanyName);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void CreateNewPublisher_ReturnsView()
        {
            var result = _gamesController.CreateNewPublisher();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void CreateNewPublisher_PassValidModel_ReturnsRedirect()
        {
            _mapper
                .Setup(m => m.Map<Publisher>(It.IsAny<PublisherCreateViewModel>()))
                .Returns(_publishers.First());

            var result = _gamesController
                .CreateNewPublisher(It.IsAny<PublisherCreateViewModel>());

            _publisherService.Verify(x => x.CreatePublisher(_publishers.First()));
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
