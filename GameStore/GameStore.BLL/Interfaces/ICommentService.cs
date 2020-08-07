using System.Collections.Generic;
using GameStore.BLL.Models;

namespace GameStore.BLL.Interfaces
{
    public interface ICommentService
    {
        Comment AddCommentToGame(Game game, Comment comment);

        IEnumerable<Comment> GetAllCommentsByGameKey(string gameKey);
    }
}
