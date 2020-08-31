using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Models;
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

        private readonly List<Game> _games;
        private readonly List<GameViewModel> _gamesVM;
        private readonly List<GameCreateViewModel> _gameCreateVM;

        private readonly List<Platform> _platforms;
        private readonly List<PlatformViewModel> _platformsVM;

        private readonly List<Genre> _genres;
        private readonly List<GenreViewModel> _genresVM;

        private readonly List<Publisher> _publishers;

        public GamesControllerTest()
        {
            _gameService = new Mock<IGameService>();
            _fileService = new Mock<IFileService>();
            _platformService = new Mock<IPlatformService>();
            _genreService = new Mock<IGenreService>();
            _publisherService = new Mock<IPublisherService>();
            _mapper = new Mock<IMapper>();

            _gamesController = new GameController(
                _gameService.Object,
                _fileService.Object,
                _genreService.Object,
                _platformService.Object,
                _publisherService.Object,
                _mapper.Object);

            _games = new List<Game>
            {
                new Game
                {
                    GameId = Guid.NewGuid(),
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
                    PlatformId = Guid.NewGuid(),
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
                    GenreId = Guid.NewGuid(),
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
                    PublisherId = Guid.NewGuid(),
                    Description = "Description",
                    CompanyName = "Name",
                    HomePage = "link",
                },
            };
        }

        [Fact]
        public void Index_ReturnsListOfGames()
        {
            _gameService.Setup(g => g.GetAllGames()).Returns(_games);
            _mapper.Setup(m => m.Map<IEnumerable<GameViewModel>>(_games)).Returns(_gamesVM);

            IActionResult result = _gamesController.Index();

            ViewResult view = Assert.IsType<ViewResult>(result);
            IEnumerable<GameViewModel> model = Assert.IsAssignableFrom<IEnumerable<GameViewModel>>(view.Model);
            Assert.Single(model);
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
        public void New_PassGameViewModel_VerifyAdding()
        {
            GameCreateViewModel gameCreateVM = _gameCreateVM.First();
            Game game = _games.First();

            _gameService.Setup(g => g.GetGameByKey(gameCreateVM.Key))
                .Returns(game);

            IActionResult result = _gamesController.CreateNewGame(gameCreateVM);

            _gameService.Verify(g => g.CreateGame(It.IsAny<Game>()));

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void New_ReturnsViewForCreatingGame()
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
        public void Update_PassGameViewModel_VerifyUpdating()
        {
            GameViewModel gameViewModel = _gamesVM.First();
            Game game = _games.First();

            _gameService.Setup(g => g.EditGame(It.IsAny<Game>()))
                .Returns(game);
            _mapper.Setup(m => m.Map<Game>(_gamesVM));

            IActionResult result = _gamesController.Update(gameViewModel);

            _gameService.Verify(g => g.EditGame(It.IsAny<Game>()));

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
            HttpResponseMessage result = _gamesController.Download();

            _fileService.Verify(f => f.CreateFile());
        }
    }
}
