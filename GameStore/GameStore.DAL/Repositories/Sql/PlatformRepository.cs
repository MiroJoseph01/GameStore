using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;

namespace GameStore.DAL.Repositories.Sql
{
    public class PlatformRepository : Repository<Platform>, IPlatformRepository
    {
        private readonly GameStoreContext _dbContext;

        public PlatformRepository(GameStoreContext dbContext, IEntityStateLogger<Platform> stateLogger)
            : base(dbContext, stateLogger)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<string> GetPlatformIdsByNames(IEnumerable<string> platformNames)
        {
            var platformIds = _dbContext.Platforms
                .Where(x => platformNames.Contains(x.PlatformName))
                .Select(x => x.PlatformId)
                .ToList();

            return platformIds;
        }
    }
}
