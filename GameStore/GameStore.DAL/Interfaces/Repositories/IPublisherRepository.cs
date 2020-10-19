using System;
using System.Collections.Generic;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IPublisherRepository : IRepository<Publisher>
    {
        IEnumerable<Guid> GetPublisherIdsByNames(IEnumerable<string> publishermNames);
    }
}
