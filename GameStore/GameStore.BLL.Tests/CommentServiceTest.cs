using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Services;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.RepostitoriesInterfaces;
using GameStore.WEB.Util.AutoMapperProfiles;
using Moq;
using Xunit;

namespace GameStore.BLL.Tests
{
    public class CommentServiceTest
    {
        private const string GameKey = "mario";

        private readonly ICommentService _commentService;
        private readonly Mock<ICommentRepository> _commentRepository;
        private readonly Mock<IGameRepository> _gameRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly IMapper _mapper;

        private Guid _gameId = Guid.Parse("457a3d66-24f2-487e-a816-a21531e6a019");

        public CommentServiceTest()
        {
            _commentRepository = new Mock<ICommentRepository>();

            _gameRepository = new Mock<IGameRepository>();

            _unitOfWork = new Mock<IUnitOfWork>();

            MapperConfiguration mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CommentProfile());
                cfg.AddProfile(new GameProfile());
                cfg.AddProfile(new GenreProfile());
            });
            _mapper = mockMapper.CreateMapper();

            _commentService = new CommentService(_gameRepository.Object, _commentRepository.Object, _unitOfWork.Object, _mapper);
        }

        [Fact]
        public void GetAllCommentsByGameKey_PassGameKey_ReturnsListOfComments()
        {
            List<DAL.Entities.Comment> comments = new List<DAL.Entities.Comment>
            {
                new DAL.Entities.Comment
                {
                    Name = "Comment Name",
                    Body = "Comment Body",
                    CommentId = Guid.NewGuid(),
                    CommentingGame = new DAL.Entities.Game
                    {
                        GameId = _gameId,
                        Key = GameKey,
                    },
                    GameId = _gameId,
                    ParentCommentId = null,
                },

                new DAL.Entities.Comment
                {
                    Name = "Comment Name",
                    Body = "Comment Body",
                    CommentId = Guid.NewGuid(),
                    CommentingGame = new DAL.Entities.Game
                    {
                        Key = "mario",
                    },
                    GameId = _gameId,
                    ParentCommentId = null,
                },

                new DAL.Entities.Comment
                {
                    Name = "Comment Name",
                    Body = "Comment Body",
                    CommentId = Guid.NewGuid(),
                    CommentingGame = new DAL.Entities.Game
                    {
                        Key = "lol",
                    },
                    GameId = Guid.NewGuid(),
                    ParentCommentId = null,
                },
            };

            List<DAL.Entities.Game> gameList = new List<DAL.Entities.Game>
            {
                new DAL.Entities.Game
                {
                    GameId = _gameId,
                    Key = GameKey,
                },

                new DAL.Entities.Game
                {
                    GameId = Guid.Empty,
                    Key = "lol",
                },
            };

            List<Models.Comment> commentsToCheck = new List<Models.Comment>
            {
                new Models.Comment
                {
                    Name = "Comment Name",
                    Body = "Comment Body",
                    CommentId = Guid.NewGuid(),
                    GameId = _gameId,
                    ParentCommentId = null,
                },

                new Models.Comment
                {
                    Name = "Comment Name",
                    Body = "Comment Body",
                    CommentId = Guid.NewGuid(),
                    GameId = _gameId,
                    ParentCommentId = null,
                },
            };

            _gameRepository.Setup(g => g.GetByKey(GameKey)).Returns(gameList.First());
            _commentRepository.Setup(c => c.GetCommentsByGameId(_gameId))
                .Returns(comments.Where(y => y.GameId.Equals(_gameId)));

            int res = _commentService.GetAllCommentsByGameKey(GameKey).Count();

            Assert.Equal(gameList.Count(), res);
        }

        [Fact]
        public void GetAllCommentsByGameKey_PassGameKey_ReturnsEmptyList()
        {
            List<DAL.Entities.Comment> comments = new List<DAL.Entities.Comment>
            {
                new DAL.Entities.Comment
                {
                    Name = "Comment Name",
                    Body = "Comment Body",
                    CommentId = Guid.NewGuid(),
                    CommentingGame = new DAL.Entities.Game
                    {
                        GameId = _gameId,
                        Key = GameKey,
                    },
                    GameId = _gameId,
                    ParentCommentId = null,
                },
            };

            List<DAL.Entities.Game> gameList = new List<DAL.Entities.Game>
            {
                new DAL.Entities.Game
                {
                    GameId = _gameId,
                    Key = GameKey,
                },

                new DAL.Entities.Game
                {
                    GameId = Guid.NewGuid(),
                    Key = "lol",
                },
            };

            _gameRepository.Setup(g => g.GetByKey("lol")).Returns(gameList.Last());

            _commentRepository.Setup(c => c.GetAll()).Returns(comments);

            IEnumerable<Models.Comment> res = _commentService.GetAllCommentsByGameKey("lol");

            Assert.Equal(Enumerable.Empty<Models.Comment>(), res);
        }

        [Fact]
        public void AddCommentToGame_PassGameModelAndCommentModel_VerifyAdding()
        {
            Models.Game game = new Models.Game { GameId = _gameId, Key = GameKey };

            Models.Comment newComment = new Models.Comment
            {
                Name = "Comment Name",
                Body = "Comment Body",
                CommentId = Guid.NewGuid(),
                GameId = _gameId,
                ParentCommentId = null,
            };

            _gameRepository.Setup(g => g.GetById(_gameId))
                .Returns(new DAL.Entities.Game
                {
                    GameId = _gameId,
                    Key = GameKey,
                });

            _commentService.AddCommentToGame(game, newComment);

            _commentRepository.Verify(x => x.Create(It.IsAny<DAL.Entities.Comment>()), Times.Once);
        }
    }
}
