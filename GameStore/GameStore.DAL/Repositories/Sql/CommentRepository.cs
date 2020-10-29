using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;

namespace GameStore.DAL.Repositories.Sql
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly GameStoreContext _dbContext;

        public CommentRepository(GameStoreContext dbContext, IEntityStateLogger<Comment> stateLogger)
            : base(dbContext, stateLogger)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Comment> GetCommentsByGameId(string gameId)
        {
            List<Comment> commentsByGaneId = _dbContext.Set<Comment>().Where(x => x.GameId.Equals(gameId)).ToList();

            return commentsByGaneId;
        }
    }
}
