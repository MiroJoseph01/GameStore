using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces.Repositories;

namespace GameStore.DAL.Repositories
{
    public class PlatformRepository : Repository<Platform>, IPlatformRepository
    {
        private readonly GameStoreContext _dbContext;

        public PlatformRepository(GameStoreContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Guid> GetPlatformIdsByNames(IEnumerable<string> platformNames)
        {
            var platformIds = _dbContext.Platforms
                .Where(x => platformNames.Contains(x.PlatformName))
                .Select(x => x.PlatformId)
                .ToList();

            return platformIds;
        }
    }
}
