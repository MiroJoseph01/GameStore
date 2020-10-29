using System.Collections.Generic;
using GameStore.BLL.Models;

namespace GameStore.BLL.Interfaces.Services
{
    public interface ICommentService
    {
        Comment AddCommentToGame(Game game, Comment comment);

        void DeleteComment(Comment comment);

        Comment UpdateComment(Comment comment);

        Comment GetCommentById(string commentId);

        IEnumerable<Comment> GetAllCommentsByGameKey(string gameKey);

        string GetCommentBody(string comment, int start, int end);

        string GetCommentQuote(string comment, int start, int end);
    }
}
