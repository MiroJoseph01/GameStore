using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces.Services;
using GameStore.BLL.Models;
using GameStore.Web.Controllers;
using GameStore.Web.Util;
using GameStore.Web.Util.Logger;
using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests
{
    public class CommentsControllerTest
    {
        private const string GameKey = "game_key";
        private readonly string _gameId;

        private readonly CommentController _commentsController;
        private readonly Mock<IGameService> _gameService;
        private readonly Mock<ICommentService> _commentService;
        private readonly Mock<IAppLogger<CommentController>> _logger;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<ICommentHelper> _commentHelper;

        private readonly Game _game;
        private readonly List<Comment> _comments;
        private readonly List<CommentViewModel> _viewComments;
        private readonly List<CommentsViewModel> _commentsForAdd;

        public CommentsControllerTest()
        {
            _gameService = new Mock<IGameService>();
            _commentService = new Mock<ICommentService>();
            _logger = new Mock<IAppLogger<CommentController>>();
            _mapper = new Mock<IMapper>();
            _commentHelper = new Mock<ICommentHelper>();

            _commentsController = new CommentController(
                _gameService.Object,
                _commentService.Object,
                _mapper.Object,
                _logger.Object,
                _commentHelper.Object);

            _gameId = Guid.NewGuid().ToString();

            _game = new Game
            {
                GameId = _gameId,
                Key = GameKey,
                Name = "Name",
            };

            _comments = new List<Comment>
            {
               new Comment
               {
                    Name = "Comment Name",
                    Body = "Comment Body",
                    GameId = _gameId,
                    CommentId = Guid.NewGuid().ToString(),
               },

               new Comment
                {
                    Name = "Comment Name",
                    Body = "Comment Body",
                    GameId = _gameId,
                    CommentId = Guid.NewGuid().ToString(),
                },
            };

            _viewComments = new List<CommentViewModel>
            {
               new CommentViewModel
               {
                    Name = "Comment Name",
                    Body = "Comment Body",
                    Replies = null,
               },
            };

            _commentsForAdd = new List<CommentsViewModel>
            {
                new CommentsViewModel
                {
                    GameKey = GameKey,
                    Body = "CommentBody",
                    Comments = new List<CommentViewModel>(),
                    GameName = "Game Name",
                    Name = "Author",
                    DeleteCommentId = Guid.NewGuid().ToString(),
                    QuoteIsPresent = false,
                },

                new CommentsViewModel
                {
                    GameKey = GameKey,
                    Body = "<QUte>CommentBody</quote>",
                    Comments = new List<CommentViewModel>(),
                    GameName = "Game Name",
                    Name = "Author",
                    DeleteCommentId = Guid.NewGuid().ToString(),
                    QuoteIsPresent = true,
                },

                new CommentsViewModel
                {
                    GameKey = GameKey,
                    Body = "</quote>CommentBody<quote>",
                    Comments = new List<CommentViewModel>(),
                    GameName = "Game Name",
                    Name = "Author",
                    DeleteCommentId = Guid.NewGuid().ToString(),
                    QuoteIsPresent = true,
                },

                new CommentsViewModel
                {
                    GameKey = GameKey,
                    Body = "<quote>CommentBody</quote>",
                    Comments = new List<CommentViewModel>(),
                    GameName = "Game Name",
                    Name = "Author",
                    DeleteCommentId = Guid.NewGuid().ToString(),
                    QuoteIsPresent = true,
                },
            };
        }

        [Fact]
        public void ViewComments_PassCommentModelAndGameModel_VerifyAdding()
        {
            _gameService.Setup(g => g.GetGameByKey(GameKey)).Returns(_game);

            IActionResult result = _commentsController
                .ViewComments(_commentsForAdd.First(), GameKey);

            _commentService
                .Verify(c => c.AddCommentToGame(_game, It.IsAny<Comment>()));

            RedirectToActionResult view = Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void ViewComments_PassCommentModelWithQuoteAndEmptyBodyAndGameModel_ReturnsView()
        {
            _gameService.Setup(g => g.GetGameByKey(GameKey)).Returns(_game);
            _mapper
                .Setup(m => m.Map<IEnumerable<CommentViewModel>>(
                    It.IsAny<IEnumerable<Comment>>()))
                .Returns(_viewComments);
            _commentService.Setup(c => c.GetAllCommentsByGameKey(GameKey))
               .Returns(_comments);
            _commentHelper
                .Setup(c => c.QuoteIsPresent(It.IsAny<CommentsViewModel>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(true);
            _commentHelper
                .Setup(c => c.ReorderComments(It.IsAny<IEnumerable<CommentViewModel>>()))
                .Returns(_viewComments);

            IActionResult result = _commentsController
                .ViewComments(_commentsForAdd.Last(), GameKey);

            var view = Assert.IsType<ViewResult>(result);
            CommentsViewModel model = Assert.IsAssignableFrom<CommentsViewModel>(view.Model);
        }

        [Fact]
        public void ViewComments_PassCommentModelWithQuote_HandleExceptionAndReturnView()
        {
            _gameService.Setup(g => g.GetGameByKey(GameKey)).Returns(_game);
            _mapper
                .Setup(m => m.Map<IEnumerable<CommentViewModel>>(
                    It.IsAny<IEnumerable<Comment>>()))
                .Returns(_viewComments);
            _commentService.Setup(c => c.GetAllCommentsByGameKey(GameKey))
               .Returns(_comments);
            _commentHelper
                .Setup(c => c.QuoteIsPresent(It.IsAny<CommentsViewModel>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(true);
            _commentHelper
                .Setup(c => c.ReorderComments(It.IsAny<IEnumerable<CommentViewModel>>()))
                .Returns(_viewComments);

            IActionResult result = _commentsController
                .ViewComments(_commentsForAdd.ElementAt(2), GameKey);

            var view = Assert.IsType<ViewResult>(result);
            CommentsViewModel model = Assert.IsAssignableFrom<CommentsViewModel>(view.Model);
        }

        [Fact]
        public void ViewComments_PassCommentModelWithQuoteAndInvalidTagsAndGameModel_ReturnsView()
        {
            _gameService.Setup(g => g.GetGameByKey(GameKey)).Returns(_game);
            _mapper
                .Setup(m => m.Map<IEnumerable<CommentViewModel>>(
                    It.IsAny<IEnumerable<Comment>>()))
                .Returns(_viewComments);
            _commentService.Setup(c => c.GetAllCommentsByGameKey(GameKey))
               .Returns(_comments);
            _commentHelper
                .Setup(c => c.QuoteIsPresent(It.IsAny<CommentsViewModel>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(true);
            _commentHelper
                .Setup(c => c.ReorderComments(It.IsAny<IEnumerable<CommentViewModel>>()))
                .Returns(_viewComments);

            IActionResult result = _commentsController
                .ViewComments(_commentsForAdd.ElementAt(1), GameKey);

            var view = Assert.IsType<ViewResult>(result);
            CommentsViewModel model = Assert.IsAssignableFrom<CommentsViewModel>(view.Model);
            Assert.Equal(_viewComments.Count(), model.Comments.Count());
        }

        [Fact]
        public void ViewComments_PassValidGameKey_ReturnsListOfComments()
        {
            _commentService.Setup(c => c.GetAllCommentsByGameKey(GameKey))
                .Returns(_comments);
            _mapper
                .Setup(m => m.Map<IEnumerable<CommentViewModel>>(
                    It.IsAny<IEnumerable<Comment>>()))
                .Returns(_viewComments);
            _gameService.Setup(g => g.IsPresent(GameKey)).Returns(true);
            _gameService.Setup(g => g.GetGameByKey(It.IsAny<string>())).Returns(_game);
            _commentHelper
                .Setup(c => c.QuoteIsPresent(It.IsAny<CommentsViewModel>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(true);
            _commentHelper
                .Setup(c => c.ReorderComments(It.IsAny<IEnumerable<CommentViewModel>>()))
                .Returns(_viewComments);

            IActionResult result = _commentsController.ViewComments(GameKey);

            ViewResult view = Assert.IsType<ViewResult>(result);
            CommentsViewModel model = Assert.IsAssignableFrom<CommentsViewModel>(view.Model);
            Assert.Equal(_viewComments.Count(), model.Comments.Count());
        }

        [Fact]
        public void ViewComments_PassNotValidGameKey_ReturnsNotFound()
        {
            _gameService.Setup(g => g.IsPresent(GameKey)).Returns(false);

            IActionResult result = _commentsController.ViewComments(GameKey);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ViewComments_PassValidGameKey_ReturnsViewWithEmptyListOfComments()
        {
            var empty = new List<CommentViewModel>();

            _commentService.Setup(c => c.GetAllCommentsByGameKey(GameKey))
                .Returns(_comments);
            _mapper
                .Setup(m => m.Map<IEnumerable<CommentViewModel>>(
                    It.IsAny<IEnumerable<Comment>>()))
                .Returns(empty);
            _gameService.Setup(g => g.IsPresent(GameKey)).Returns(true);
            _gameService.Setup(g => g.GetGameByKey(It.IsAny<string>())).Returns(_game);

            IActionResult result = _commentsController.ViewComments(GameKey);

            ViewResult view = Assert.IsType<ViewResult>(result);
            CommentsViewModel model = Assert.IsAssignableFrom<CommentsViewModel>(view.Model);
            Assert.Empty(model.Comments);
        }

        [Fact]
        public void DeleteComment_PassCommentsViewModel_ReturnsRedirectToAction()
        {
            IActionResult result = _commentsController.DeleteComment(_commentsForAdd.First());

            _commentService.Verify(x => x.DeleteComment(It.IsAny<Comment>()));

            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
