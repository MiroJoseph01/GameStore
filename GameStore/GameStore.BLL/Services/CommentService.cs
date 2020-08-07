using System.Collections.Generic;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Models;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.RepostitoriesInterfaces;

namespace GameStore.BLL.Services
{
    public class CommentService : ICommentService
    {
        private readonly IGameRepository _gameRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentService(IGameRepository gameRepository, ICommentRepository commentRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _gameRepository = gameRepository;
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Comment AddCommentToGame(Game game, Comment comment)
        {
            DAL.Entities.Game gameFromDB = _gameRepository.GetById(game.GameId);

            if (gameFromDB.Comments is null)
            {
                gameFromDB.Comments = new List<DAL.Entities.Comment>();
            }

            DAL.Entities.Comment commentFromDB = new DAL.Entities.Comment
            {
                GameId = game.GameId,
                CommentingGame = gameFromDB,
                Body = comment.Body,
                Name = comment.Name,
                ParentCommentId = comment.ParentCommentId,
            };

            _commentRepository.Create(commentFromDB);

            _unitOfWork.Commit();

            return comment;
        }

        public IEnumerable<Comment> GetAllCommentsByGameKey(string gameKey)
        {
            DAL.Entities.Game game = _gameRepository.GetByKey(gameKey);

            if (game is null)
            {
                return null;
            }

            IEnumerable<DAL.Entities.Comment> commentsFromDB = _commentRepository.GetCommentsByGameId(game.GameId);

            IEnumerable<Comment> comments = _mapper.Map<IEnumerable<Comment>>(commentsFromDB);

            return comments;
        }
    }
}
