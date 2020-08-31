using System;
using System.Collections.Generic;
using GameStore.BLL.Models;

namespace GameStore.BLL.Interfaces
{
    public interface ICommentService
    {
        Comment AddCommentToGame(Game game, Comment comment);

        void DeleteComment(Comment comment);

        Comment UpdateComment(Comment comment);

        Comment GetCommentById(Guid commentId);

        IEnumerable<Comment> GetAllCommentsByGameKey(string gameKey);
    }
}
