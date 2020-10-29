using System;
using System.Collections.Generic;
using AutoMapper;
using GameStore.BLL.Interfaces.Services;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.BLL.Services
{
    public class CommentService : ICommentService
    {
        private readonly IGameRepositoryFacade _gameRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentService(
            IGameRepositoryFacade gameRepository,
            ICommentRepository commentRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _gameRepository = gameRepository;
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public BusinessModels.Comment AddCommentToGame(BusinessModels.Game game, BusinessModels.Comment comment)
        {
            var gameFromDb = _gameRepository.GetById(game.GameId);

            gameFromDb.Comments.Add(new DbModels.Comment
            {
                CommentId = Guid.NewGuid().ToString(),
                Body = comment.Body,
                Quote = comment.Quote,
                Name = comment.Name,
                GameId = game.GameId,
                ParentCommentId = comment.ParentCommentId,
            });

            _gameRepository.Update(game.GameId, gameFromDb);

            _unitOfWork.Commit();

            return comment;
        }

        public void DeleteComment(BusinessModels.Comment comment)
        {
            if (!_commentRepository.IsPresent(comment.CommentId))
            {
                throw new ArgumentException("Comment doesn't exist or has already been deleted");
            }

            _commentRepository.Delete(comment.CommentId);

            _unitOfWork.Commit();
        }

        public BusinessModels.Comment UpdateComment(
            BusinessModels.Comment comment)
        {
            _commentRepository.Update(comment.CommentId, _mapper.Map<DbModels.Comment>(comment));

            _unitOfWork.Commit();

            return comment;
        }

        public BusinessModels.Comment GetCommentById(string commentId)
        {
            DbModels.Comment commentFromDb = _commentRepository.GetById(commentId);

            var comment = _mapper.Map<BusinessModels.Comment>(commentFromDb);

            return comment;
        }

        public IEnumerable<BusinessModels.Comment> GetAllCommentsByGameKey(
            string gameKey)
        {
            DbModels.Game game = _gameRepository.GetByKey(gameKey);

            IEnumerable<DbModels.Comment> commentsFromDb = _commentRepository.GetCommentsByGameId(game.GameId);

            IEnumerable<BusinessModels.Comment> comments = _mapper.Map<IEnumerable<BusinessModels.Comment>>(commentsFromDb);

            return comments;
        }

        public string GetCommentBody(string comment, int start, int end)
        {
            return comment.Remove(start, end - start + 8);
        }

        public string GetCommentQuote(string comment, int start, int end)
        {
            return comment.Substring(start + 7, end - start - 7);
        }
    }
}
