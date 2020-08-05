using System;
using System.Collections.Generic;

namespace GameStore.DAL.Interfaces.RepostitoriesInterfaces
{
    public interface ICommentRepository : IGenericRepository<Entities.Comment>
    {
        IEnumerable<Entities.Comment> GetCommentsByGameId(Guid gameId);
    }
}
