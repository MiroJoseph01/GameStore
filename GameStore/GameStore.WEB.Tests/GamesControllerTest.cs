using System;
using System.Collections.Generic;
using System.Net.Http;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Models;
using GameStore.WEB.Controllers;
using GameStore.WEB.Util.AutoMapperProfiles;
using GameStore.WEB.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.WEB.Tests
{
    public class GamesControllerTest
    {
        private readonly GamesController _gamesController;
        private readonly Mock<IGameService> _gameService;
        private readonly Mock<IFileService> _fileService;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<GamesController>> _logger;

        public GamesControllerTest()
        {
            _gameService = new Mock<IGameService>();
            _fileService = new Mock<IFileService>();
            _logger = new Mock<ILogger<GamesController>>();

            MapperConfiguration mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CommentProfile());
                cfg.AddProfile(new GameProfile());
                cfg.AddProfile(new GenreProfile());
                cfg.AddProfile(new PlatformProfile());
            });
            _mapper = mockMapper.CreateMapper();

            _gamesController = new GamesController(_gameService.Object, _fileService.Object, _mapper, _logger.Object);
        }

        [Fact]
        public void Index_ReturnsListOfGames()
        {
            Guid gameId = Guid.NewGuid();
            List<Game> games = new List<Game>
            {
                new Game
                {
                    GameId = gameId,
                    Key = "mario",
                    Name = "Mario",
                },
            };

            _gameService.Setup(g => g.GetAllGames()).Returns(games);

            IActionResult result = _gamesController.Index();

            OkObjectResult view = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void IndexByKey_PassCorrectGameKey_ReturnsGameModel()
        {
            Guid gameId = Guid.NewGuid();
            Game game = new Game
            {
                GameId = gameId,
                Key = "mario",
                Name = "Mario",
            };

            _gameService.Setup(g => g.GetGameByKey("mario")).Returns(game);

            IActionResult result = _gamesController.Index("mario");

            OkObjectResult view = Assert.IsType<OkObjectResult>(result);
            Game model = Assert.IsAssignableFrom<Game>(
                view.Value);
            Assert.IsType<Game>(model);
        }

        [Fact]
        public void Index_PassWrongGameKey_ReturnsNotFoundObjectResult()
        {
            Guid gameId = Guid.NewGuid();
            Game game = new Game
            {
                GameId = gameId,
                Key = "mario",
                Name = "Mario",
            };

            _gameService.Setup(g => g.GetGameByKey("mario")).Returns(game);

            IActionResult result = _gamesController.Index("wrong");

            NotFoundObjectResult view = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void New_PassGameViewModel_VerifyAdding()
        {
            GameViewModel gameViewModel = new GameViewModel
            {
                Key = "mario",
                Name = "Mario",
                Description = It.IsAny<string>(),
                Comments = null,
                Platforms = new List<PlatformViewModel>
                {
                    new PlatformViewModel
                    {
                        PlatformId = Guid.NewGuid().ToString(),
                        PlatformName = "browser",
                    },
                },
            };

            Game game = new Game
            {
                Key = gameViewModel.Key,
                Name = gameViewModel.Name,
                Description = gameViewModel.Description,
                Comments = _mapper.Map<IList<Comment>>(gameViewModel.Comments),
                GameGenres = _mapper.Map<IList<Genre>>(gameViewModel.Genres),
                GamePlatforms = _mapper.Map<IList<Platform>>(gameViewModel.Platforms),
            };

            _gameService.Setup(g => g.GetGameByKey(gameViewModel.Key))
                .Returns(game);

            IActionResult result = _gamesController.New(gameViewModel);

            _gameService.Verify(g => g.CreateGame(It.IsAny<Game>()));

            OkObjectResult view = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Update_PassGameViewModel_VerifyUpdating()
        {
            GameViewModel gameViewModel = new GameViewModel
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
            };

            Game game = new Game
            {
                Key = gameViewModel.Key,
                Name = gameViewModel.Name,
                Description = gameViewModel.Description,
                Comments = _mapper.Map<IList<Comment>>(gameViewModel.Comments),
                GamePlatforms = _mapper.Map<IList<Platform>>(gameViewModel.Platforms),
            };

            _gameService.Setup(g => g.EditGame(It.IsAny<Game>()))
                .Returns(game);

            IActionResult result = _gamesController.Update(gameViewModel);

            _gameService.Verify(g => g.EditGame(It.IsAny<Game>()));

            OkObjectResult view = Assert.IsType<OkObjectResult>(result);
            GameViewModel model = Assert.IsAssignableFrom<GameViewModel>(
                view.Value);
            Assert.IsType<GameViewModel>(model);
        }

        [Fact]
        public void Delete_PassGameViewModel_VerifyDeliting()
        {
            GameViewModel gameViewModel = new GameViewModel
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
            };

            Game gameBLL = new Game
            {
                Key = gameViewModel.Key,
                Name = gameViewModel.Name,
                Description = gameViewModel.Description,
                Comments = _mapper.Map<IList<Comment>>(gameViewModel.Comments),
                GamePlatforms = _mapper.Map<IList<Platform>>(gameViewModel.Platforms),
            };

            _gameService.Setup(g => g.GetGameByKey("mario")).Returns(gameBLL);

            IActionResult result = _gamesController.Delete(gameViewModel.Key);

            _gameService.Verify(g => g.DeleteGame(It.IsAny<Game>()));

            OkObjectResult view = Assert.IsType<OkObjectResult>(result);
            Assert.Equal($"Game with key: \'{gameViewModel.Key}\' is deleted", view.Value);
        }

        [Fact]
        public void Download_ReturnsHttpResponseMessage()
        {
            HttpResponseMessage result = _gamesController.Download();

            _fileService.Verify(f => f.CreateFile());
        }
    }
}
