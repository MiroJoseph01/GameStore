using System.Collections.Generic;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IPublisherRepositoryFacade : IRepository<Publisher>
    {
        IEnumerable<string> GetPublisherIdsByNames(IEnumerable<string> publishermNames);
    }
}
