using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Models;
using GameStore.Web.Controllers;
using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Web.Tests
{
    public class CommentsControllerTest
    {
        private readonly CommentController _commentsController;
        private readonly Mock<IGameService> _gameService;
        private readonly Mock<ICommentService> _commentService;
        private readonly Mock<ILogger<CommentController>> _logger;
        private readonly Mock<IMapper> _mapper;

        public CommentsControllerTest()
        {
            _gameService = new Mock<IGameService>();
            _commentService = new Mock<ICommentService>();
            _logger = new Mock<ILogger<CommentController>>();
            _mapper = new Mock<IMapper>();

            _commentsController = new CommentController(
                _gameService.Object,
                _commentService.Object,
                _mapper.Object);
        }

        [Fact]
        public void ViewComments_PassCommentModelAndGameModel_VerifyAdding()
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

            CommentsViewModel comment = new CommentsViewModel
            {
                Name = "Comment Name",
                Body = "Comment Body",
            };

            _gameService.Setup(g => g.GetGameByKey(game.Key)).Returns(game);

            IActionResult result = _commentsController
                .ViewComments(comment, game.Key);

            _commentService
                .Verify(c => c.AddCommentToGame(game, It.IsAny<Comment>()));

            RedirectToActionResult view = Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void ViewComments_PassValidGameKey_ReturnsListOfComments()
        {
            string gameKey = "lol";

            Game game = new Game
            {
                GameId = Guid.NewGuid(),
                Key = gameKey,
                Name = "Name",
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

            List<CommentViewModel> viewCommentList = new List<CommentViewModel>
            {
               new CommentViewModel
               {
                    Name = "Comment Name",
                    Body = "Comment Body",
               },

               new CommentViewModel
                {
                    Name = "Comment Name",
                    Body = "Comment Body",
                },
            };

            _commentService.Setup(c => c.GetAllCommentsByGameKey(gameKey))
                .Returns(commentList);
            _mapper
                .Setup(m => m.Map<IEnumerable<CommentViewModel>>(
                    It.IsAny<IEnumerable<Comment>>()))
                .Returns(viewCommentList);
            _gameService.Setup(g => g.IsPresent(gameKey)).Returns(true);
            _gameService.Setup(g => g.GetGameByKey(It.IsAny<string>())).Returns(game);

            IActionResult result = _commentsController.ViewComments(gameKey);

            ViewResult view = Assert.IsType<ViewResult>(result);
            CommentsViewModel model = Assert.IsAssignableFrom<CommentsViewModel>(view.Model);
            Assert.Equal(commentList.Count(), model.Comments.Count());
        }
    }
}
