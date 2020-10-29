using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Repositories.Sql;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace GameStore.DAL.Tests
{
    public class CommentRepositoryTest
    {
        private readonly CommentRepository _commentRepository;
        private readonly Mock<DbSet<Comment>> _dbComments;
        private readonly Mock<GameStoreContext> _gameStoreContext;
        private readonly Mock<IEntityStateLogger<Comment>> _logger;

        public CommentRepositoryTest()
        {
            _gameStoreContext = new Mock<GameStoreContext>();
            _logger = new Mock<IEntityStateLogger<Comment>>();

            _dbComments = new Mock<DbSet<Comment>>();

            _commentRepository = new CommentRepository(_gameStoreContext.Object, _logger.Object);
        }

        [Fact]
        public void Create_PassCommentModel_VerifyAdding()
        {
            var commentId = Guid.NewGuid().ToString();
            var gameId = Guid.NewGuid().ToString();

            Comment comment = new Comment
            {
                CommentId = commentId,
                Name = It.IsAny<string>(),
                Body = It.IsAny<string>(),
                CommentingGame = new Game
                {
                    GameId = gameId,
                    Name = "Game",
                    Key = "Game",
                },
                GameId = gameId,
                ParentCommentId = null,
            };

            _gameStoreContext.Setup(g => g.Set<Comment>()).Returns(_dbComments.Object);

            _commentRepository.Create(comment);

            _gameStoreContext.Verify(x => x.Set<Comment>());
            _dbComments.Verify(x => x.Add(It.Is<Comment>(y => y == comment)));
        }

        [Fact]
        public void Create_PassEmptyCommentModel_ThrowsNullReferenceException()
        {
            Comment comment = new Comment();

            Assert.Throws<NullReferenceException>(() => _commentRepository.Create(comment));
        }

        [Fact]
        public void GetById_PassCorrectGameId_ReturnsCommentModel()
        {
            var gameId = Guid.NewGuid().ToString();
            var commentId = Guid.NewGuid().ToString();

            Game game = new Game
            {
                GameId = gameId,
                Name = "lol",
                Key = "lol",
            };

            Comment comment = new Comment
            {
                CommentId = commentId,
                Name = It.IsAny<string>(),
                Body = It.IsAny<string>(),
                CommentingGame = game,
                GameId = gameId,
                ParentCommentId = null,
            };

            _dbComments.Setup(c => c.Find(commentId)).Returns(comment);

            _gameStoreContext.Setup(g => g.Set<Comment>()).Returns(_dbComments.Object);

            Comment result = _commentRepository.GetById(commentId);

            _gameStoreContext.Verify(g => g.Set<Comment>());
            _dbComments.Verify(g => g.Find(commentId));
        }

        [Fact]
        public void GetById_PassWrongCommentId_ThrowsNullReferenceException()
        {
            var gameId = Guid.NewGuid().ToString();
            var commentId = Guid.NewGuid().ToString();

            Game game = new Game
            {
                GameId = gameId,
                Name = "lol",
                Key = "lol",
            };

            Comment comment = new Comment
            {
                CommentId = commentId,
                Name = It.IsAny<string>(),
                Body = It.IsAny<string>(),
                CommentingGame = game,
                GameId = gameId,
                ParentCommentId = null,
            };

            _dbComments.Setup(c => c.Find(commentId)).Returns(comment);

            Assert.Throws<NullReferenceException>(() => _commentRepository.GetById(Guid.NewGuid().ToString()));
        }

        [Fact]
        public void GetAll_ReturnsListOfComments()
        {
            var gameId = Guid.NewGuid().ToString();
            var commentId = Guid.NewGuid().ToString();

            Game game = new Game
            {
                GameId = gameId,
                Name = "lol",
                Key = "lol",
            };

            Comment comment = new Comment
            {
                CommentId = commentId,
                Name = It.IsAny<string>(),
                Body = It.IsAny<string>(),
                CommentingGame = game,
                GameId = gameId,
                ParentCommentId = null,
            };

            List<Comment> comments = new List<Comment> { comment };

            _dbComments.As<IQueryable<Comment>>().Setup(x => x.Provider).Returns(comments.AsQueryable().Provider);
            _dbComments.As<IQueryable<Comment>>().Setup(x => x.Expression).Returns(comments.AsQueryable().Expression);
            _dbComments.As<IQueryable<Comment>>().Setup(x => x.ElementType).Returns(comments.AsQueryable().ElementType);
            _dbComments.As<IQueryable<Comment>>().Setup(x => x.GetEnumerator()).Returns(comments.AsQueryable().GetEnumerator());

            _gameStoreContext.Setup(g => g.Set<Comment>()).Returns(_dbComments.Object);

            IEnumerable<Comment> result = _commentRepository.GetAll();

            Assert.Equal(comments, result.ToList());
        }

        [Fact]
        public void GetCommentsByGameId_PassCorrectGameId_ReturnsListOfComments()
        {
            var gameId = Guid.NewGuid().ToString();
            var commentId = Guid.NewGuid().ToString();

            Game game = new Game
            {
                GameId = gameId,
                Name = "lol",
                Key = "lol",
            };

            Comment comment = new Comment
            {
                CommentId = commentId,
                Name = It.IsAny<string>(),
                Body = It.IsAny<string>(),
                CommentingGame = game,
                GameId = gameId,
                ParentCommentId = null,
            };

            List<Comment> comments = new List<Comment> { comment };

            _dbComments.As<IQueryable<Comment>>().Setup(x => x.Provider).Returns(comments.AsQueryable().Provider);
            _dbComments.As<IQueryable<Comment>>().Setup(x => x.Expression).Returns(comments.AsQueryable().Expression);
            _dbComments.As<IQueryable<Comment>>().Setup(x => x.ElementType).Returns(comments.AsQueryable().ElementType);
            _dbComments.As<IQueryable<Comment>>().Setup(x => x.GetEnumerator()).Returns(comments.AsQueryable().GetEnumerator());

            _gameStoreContext.Setup(g => g.Set<Comment>()).Returns(_dbComments.Object);

            IEnumerable<Comment> res = _commentRepository.GetCommentsByGameId(gameId);

            Assert.Equal(comment, res.ToList().First());
        }
    }
}
