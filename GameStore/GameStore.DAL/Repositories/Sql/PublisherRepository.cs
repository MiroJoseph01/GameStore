using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;

namespace GameStore.DAL.Repositories.Sql
{
    public class PublisherRepository : Repository<Publisher>, IPublisherRepository
    {
        private readonly GameStoreContext _dbContext;

        public PublisherRepository(GameStoreContext dbContext, IEntityStateLogger<Publisher> stateLogger)
            : base(dbContext, stateLogger)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<string> GetPublisherIdsByNames(IEnumerable<string> publishermNames)
        {
            var publisherIds = _dbContext.Publishers
                .Where(x => publishermNames.Contains(x.CompanyName))
                .Select(x => x.PublisherId)
                .ToList();

            return publisherIds;
        }

        public override IEnumerable<Publisher> GetAll()
        {
            return _dbContext.Set<Publisher>()
                 .ToList();
        }

        public override Publisher GetById(string id)
        {
            Publisher entity = _dbContext.Set<Publisher>().Find(id);

            return entity;
        }
    }
}
