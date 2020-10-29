using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces.Services;
using GameStore.BLL.Services;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using Moq;
using Xunit;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.BLL.Tests
{
    public class CommentServiceTest
    {
        private const string GameKey = "mario";
        private const string GameId = "457a3d66-24f2-487e-a816-a21531e6a019";

        private readonly ICommentService _commentService;
        private readonly Mock<IGameService> _gameService;
        private readonly Mock<ICommentRepository> _commentRepository;
        private readonly Mock<IGameRepositoryFacade> _gameRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;

        private readonly List<DbModels.Comment> _commentsFromDb;
        private readonly List<BusinessModels.Comment> _comments;

        public CommentServiceTest()
        {
            _commentRepository = new Mock<ICommentRepository>();

            _gameRepository = new Mock<IGameRepositoryFacade>();

            _gameService = new Mock<IGameService>();

            _unitOfWork = new Mock<IUnitOfWork>();

            _mapper = new Mock<IMapper>();

            _commentService = new CommentService(
                _gameRepository.Object,
                _commentRepository.Object,
                _unitOfWork.Object,
                _mapper.Object);

            _commentsFromDb = new List<DbModels.Comment>
            {
                new DbModels.Comment
                {
                    Name = "Comment Name",
                    Body = "Comment Body",
                    CommentId = Guid.NewGuid().ToString(),
                    CommentingGame = new DbModels.Game
                    {
                        GameId = GameId,
                        Key = GameKey,
                    },
                    GameId = GameId,
                    ParentCommentId = null,
                },

                new DbModels.Comment
                {
                    Name = "Comment Name",
                    Body = "Comment Body",
                    CommentId = Guid.NewGuid().ToString(),
                    CommentingGame = new DbModels.Game
                    {
                        Key = "contr_strike",
                    },
                    GameId = Guid.NewGuid().ToString(),
                    ParentCommentId = null,
                },

                new DbModels.Comment
                {
                    Name = "Comment Name",
                    Body = "Comment Body",
                    CommentId = Guid.NewGuid().ToString(),
                    CommentingGame = new DbModels.Game
                    {
                        Key = "lol",
                    },
                    GameId = Guid.NewGuid().ToString(),
                    ParentCommentId = null,
                },
            };

            _comments = new List<BusinessModels.Comment>
            {
                new BusinessModels.Comment
                {
                    Name = "Comment Name",
                    Body = "Comment Body",
                    CommentId = Guid.NewGuid().ToString(),
                    GameId = GameId,
                    ParentCommentId = null,
                },

                new BusinessModels.Comment
                {
                    Name = "Comment Name",
                    Body = "Comment Body",
                    CommentId = Guid.NewGuid().ToString(),
                    GameId = Guid.NewGuid().ToString(),
                    ParentCommentId = null,
                },
            };
        }

        [Fact]
        public void GetAllCommentsByGameKey_PassGameKey_ReturnsListOfComments()
        {
            List<DbModels.Game> gameList = new List<DbModels.Game>
            {
                new DbModels.Game
                {
                    GameId = GameId,
                    Key = GameKey,
                },

                new DbModels.Game
                {
                    GameId = Guid.Empty.ToString(),
                    Key = "lol",
                },
            };

            _gameService
                .Setup(c => c.IsPresent(It.IsAny<string>()))
                .Returns(true);
            _gameRepository
                .Setup(g => g.GetByKey(GameKey))
                .Returns(gameList.First());
            _commentRepository
                .Setup(c => c.GetCommentsByGameId(GameId))
                .Returns(_commentsFromDb.Where(y => y.GameId.Equals(GameId)));
            _mapper
                .Setup(m => m
                    .Map<IEnumerable<BusinessModels.Comment>>(
                        It.IsAny<IEnumerable<DbModels.Comment>>()))
                .Returns(
                    new List<BusinessModels.Comment> { _comments.First() });

            int res = _commentService.GetAllCommentsByGameKey(GameKey).Count();

            Assert
                .Equal(
                    _comments.Where(y => y.GameId.Equals(GameId)).Count(),
                    res);
        }

        [Fact]
        public void GetAllCommentsByGameKey_PassGameKey_ReturnsEmptyList()
        {
            List<DbModels.Comment> commentsFromDb = new List<DbModels.Comment>
            {
                new DbModels.Comment
                {
                    Name = "Comment Name",
                    Body = "Comment Body",
                    CommentId = Guid.NewGuid().ToString(),
                    CommentingGame = new DbModels.Game
                    {
                        GameId = GameId,
                        Key = GameKey,
                    },
                    GameId = GameId,
                    ParentCommentId = null,
                },
            };

            List<BusinessModels.Comment> comments =
                new List<BusinessModels.Comment>();

            List<DbModels.Game> gameList = new List<DbModels.Game>
            {
                new DbModels.Game
                {
                    GameId = GameId,
                    Key = GameKey,
                },

                new DbModels.Game
                {
                    GameId = Guid.NewGuid().ToString(),
                    Key = "lol",
                },
            };

            _gameService
                .Setup(c => c.IsPresent(It.IsAny<string>()))
                .Returns(true);

            _mapper
                .Setup(m => m.Map<IEnumerable<BusinessModels.Comment>>(
                    It.IsAny<IEnumerable<DbModels.Comment>>()))
                .Returns(comments);

            _gameRepository.Setup(g => g.GetByKey("lol")).Returns(gameList.Last());

            _commentRepository
                .Setup(c => c.GetAll())
                .Returns(_commentsFromDb);

            IEnumerable<BusinessModels.Comment> res = _commentService
                .GetAllCommentsByGameKey("lol");

            Assert.Empty(res);
        }

        [Fact]
        public void AddCommentToGame_PassGameModelAndCommentModel_VerifyAdding()
        {
            BusinessModels.Game game = new BusinessModels.Game
            {
                GameId = GameId,
                Key = GameKey,
            };

            BusinessModels.Comment newComment = new BusinessModels.Comment
            {
                Name = "Comment Name",
                Body = "Comment Body",
                CommentId = Guid.NewGuid().ToString(),
                GameId = GameId,
                ParentCommentId = null,
            };

            _gameRepository.Setup(g => g.GetById(GameId))
                .Returns(new DbModels.Game
                {
                    GameId = GameId,
                    Key = GameKey,
                });

            _mapper
                .Setup(m => m.Map<DbModels.Comment>(It.IsAny<BusinessModels.Comment>()))
                .Returns(_commentsFromDb.First());

            _commentService.AddCommentToGame(game, newComment);

            _gameRepository
                .Verify(x => x.Update(It.IsAny<string>(), It.IsAny<DbModels.Game>(), 0), Times.Once);
        }

        [Fact]
        public void DeleteComment_PassComment_VerifyDeleting()
        {
            _commentRepository
                .Setup(c => c.IsPresent(It.IsAny<string>()))
                .Returns(true);

            _commentRepository
                .Setup(c => c.GetById(It.IsAny<string>()))
                .Returns(_commentsFromDb.First());

            _commentService.DeleteComment(_comments.First());

            _commentRepository.Verify(c => c.Delete(It.IsAny<string>()));
        }

        [Fact]
        public void DeleteComment_PassDeletedComment_ThrowsException()
        {
            var comment = _commentsFromDb.First();
            comment.IsRemoved = true;

            Assert
                .Throws<ArgumentException>(() => _commentService
                    .DeleteComment(_comments.First()));

            comment = null;

            Assert
                .Throws<ArgumentException>(() => _commentService
                    .DeleteComment(_comments.First()));
        }

        [Fact]
        public void UpdateComment_PassComment_ReturnsComment()
        {
            _mapper
                .Setup(m => m
                    .Map<DbModels.Comment>(It.IsAny<BusinessModels.Comment>()))
                .Returns(_commentsFromDb.First());

            var result = _commentService.UpdateComment(_comments.First());

            _commentRepository.Verify(c => c
                .Update(It.IsAny<string>(), It.IsAny<DbModels.Comment>()));
            Assert.NotNull(result);
        }

        [Fact]
        public void GetCommentById_PassCommentId_ReturnsComment()
        {
            _commentRepository
                .Setup(c => c.GetById(It.IsAny<string>()))
                .Returns(_commentsFromDb.First());
            _mapper
                .Setup(m => m
                    .Map<BusinessModels.Comment>(
                        It.IsAny<DbModels.Comment>()))
                .Returns(_comments.First());

            var result = _commentService.GetCommentById(_comments.First().CommentId);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetCommentById_PassNonExistingCommentId_ReturnsNull()
        {
            var comment = _commentsFromDb.First();
            comment.IsRemoved = true;

            var result = _commentService.GetCommentById(_comments.First().CommentId);

            Assert.Null(result);
        }

        [Fact]
        public void GetCommentBody_PassValidStringAndIndexes_ReturnString()
        {
            var commentWithQuote = "<quote>I like</quote>other text";
            var commentBody = "other text";

            var result = _commentService
                .GetCommentBody(
                    commentWithQuote,
                    commentWithQuote.IndexOf("<quote>"),
                    commentWithQuote.IndexOf("</quote>"));

            Assert.Equal(commentBody, result);
        }

        [Fact]
        public void GetCommentQuote_PassValidStringAndIndexes_ReturnString()
        {
            var commentWithQuote = "<quote>I like</quote>other text";
            var commentQuote = "I like";

            var result = _commentService
                .GetCommentQuote(
                    commentWithQuote,
                    commentWithQuote.IndexOf("<quote>"),
                    commentWithQuote.IndexOf("</quote>"));

            Assert.Equal(commentQuote, result);
        }
    }
}
