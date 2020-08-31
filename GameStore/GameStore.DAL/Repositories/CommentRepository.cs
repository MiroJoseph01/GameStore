using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces.Repositories;

namespace GameStore.DAL.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly GameStoreContext _dbContext;

        public CommentRepository(GameStoreContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Comment> GetCommentsByGameId(Guid gameId)
        {
            List<Comment> commentsByGaneId = _dbContext.Set<Comment>()
                .Where(x => x.GameId.Equals(gameId))
                .ToList();

            return commentsByGaneId;
        }
    }
}
