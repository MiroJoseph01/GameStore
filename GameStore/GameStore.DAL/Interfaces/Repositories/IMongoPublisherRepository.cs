using System.Collections.Generic;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IMongoPublisherRepository
    {
        IEnumerable<Publisher> GetAll();

        Publisher GetById(string id);

        IEnumerable<string> GetPublisherIdsByNames(IEnumerable<string> publisherNames);

        bool IsPresent(string id);
    }
}
