using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces.Repositories;

namespace GameStore.DAL.Repositories
{
    public class PublisherRepository : Repository<Publisher>, IPublisherRepository
    {
        private readonly GameStoreContext _dbContext;

        public PublisherRepository(GameStoreContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Guid> GetPublisherIdsByNames(IEnumerable<string> publishermNames)
        {
            var publisherIds = _dbContext.Publishers
                .Where(x => publishermNames.Contains(x.CompanyName))
                .Select(x => x.PublisherId)
                .ToList();

            return publisherIds;
        }
    }
}
