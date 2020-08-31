using System;
using System.Collections.Generic;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.BLL.Services
{
    public class CommentService : ICommentService
    {
        private readonly IGameRepository _gameRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentService(
            IGameRepository gameRepository,
            ICommentRepository commentRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _gameRepository = gameRepository;
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public BusinessModels.Comment AddCommentToGame(
            BusinessModels.Game game,
            BusinessModels.Comment comment)
        {
            DbModels.Game gameFromDb = _gameRepository
                .GetById(game.GameId);

            DbModels.Comment commentFromDb = new DbModels.Comment
            {
                GameId = game.GameId,
                CommentingGame = gameFromDb,
                Body = comment.Body,
                Name = comment.Name,
                ParentCommentId = comment.ParentCommentId,
            };

            _commentRepository.Create(commentFromDb);

            _unitOfWork.Commit();

            return comment;
        }

        public void DeleteComment(BusinessModels.Comment comment)
        {
            if (!_commentRepository.IsPresent(comment.CommentId))
            {
                throw new ArgumentException("" +
                    "Comment doesn't exist or has been already deleted");
            }

            _commentRepository.Delete(comment.CommentId);

            _unitOfWork.Commit();
        }

        public BusinessModels.Comment UpdateComment(
            BusinessModels.Comment comment)
        {
            _commentRepository
                .Update(comment.CommentId, _mapper
                .Map<DbModels.Comment>(comment));

            _unitOfWork.Commit();

            return comment;
        }

        public BusinessModels.Comment GetCommentById(Guid commentId)
        {
            DbModels.Comment commentFromDb = _commentRepository
                .GetById(commentId);

            var comment = _mapper.Map<BusinessModels.Comment>(commentFromDb);

            return comment;
        }

        public IEnumerable<BusinessModels.Comment> GetAllCommentsByGameKey(
            string gameKey)
        {
            DbModels.Game game = _gameRepository.GetByKey(gameKey);

            IEnumerable<DbModels.Comment> commentsFromDb =
                _commentRepository.GetCommentsByGameId(game.GameId);

            IEnumerable<BusinessModels.Comment> comments = _mapper
                .Map<IEnumerable<BusinessModels.Comment>>(commentsFromDb);

            return comments;
        }
    }
}
