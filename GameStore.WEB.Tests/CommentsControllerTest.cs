using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CommentsControllerTest
    {
        private readonly CommentsController _commentsController;
        private readonly Mock<IGameService> _gameService;
        private readonly Mock<ICommentService> _commentService;
        private readonly Mock<ILogger<CommentsController>> _logger;
        private readonly IMapper _mapper;

        public CommentsControllerTest()
        {
            _gameService = new Mock<IGameService>();
            _commentService = new Mock<ICommentService>();
            _logger = new Mock<ILogger<CommentsController>>();

            MapperConfiguration mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CommentProfile());
                cfg.AddProfile(new GameProfile());
                cfg.AddProfile(new GenreProfile());
                cfg.AddProfile(new PlatformProfile());
            });
            _mapper = mockMapper.CreateMapper();

            _commentsController = new CommentsController(_gameService.Object, _commentService.Object, _logger.Object, _mapper);
        }

        [Fact]
        public void NewComment_PassCommentModelAndGameModel_VerifyAdding()
        {
            Guid gameId = Guid.NewGuid();

            Game game = new Game
            {
                GameId = gameId,
                Key = "mario",
                Name = "Mario",
                Description = "Game Description",
                Comments = null,
                GamePlatforms = new List<Platform>
                {
                    new Platform
                    {
                        PlatformId = Guid.NewGuid(),
                        PlatformName = "browser",
                    },
                },
            };

            CommentViewModel comment = new CommentViewModel
            {
                Name = "Comment Name",
                Body = "Comment Body",
            };

            _gameService.Setup(g => g.GetGameByKey(game.Key)).Returns(game);

            IActionResult result = _commentsController.NewComment(comment, game.Key);

            _commentService.Verify(c => c.AddCommentToGame(game, It.IsAny<Comment>()));

            OkObjectResult view = Assert.IsType<OkObjectResult>(result);
            GameViewModel model = Assert.IsAssignableFrom<GameViewModel>(
                view.Value);
            Assert.IsType<GameViewModel>(model);
        }

        [Fact]
        public void Comments_PassValidGameKey_ReturnsListOfComments()
        {
            string gameKey = "lol";

            Game game = new Game
            {
                GameId = Guid.NewGuid(),
                Key = gameKey,
            };

            List<Comment> commentList = new List<Comment>
            {
               new Comment
               {
                    Name = "Comment Name",
                    Body = "Comment Body",
                    GameId = game.GameId,
                    CommentId = Guid.NewGuid(),
               },

               new Comment
                {
                    Name = "Comment Name",
                    Body = "Comment Body",
                    GameId = game.GameId,
                    CommentId = Guid.NewGuid(),
                },
            };

            _commentService.Setup(c => c.GetAllCommentsByGameKey(gameKey))
                .Returns(commentList);
            _gameService.Setup(g => g.IsPresent(gameKey)).Returns(true);

            IActionResult result = _commentsController.Comments(gameKey);

            OkObjectResult view = Assert.IsType<OkObjectResult>(result);
            IEnumerable<CommentViewModel> model = Assert.IsAssignableFrom<IEnumerable<CommentViewModel>>(
                view.Value);
            Assert.Equal(commentList.Count(), model.Count());
        }
    }
}
