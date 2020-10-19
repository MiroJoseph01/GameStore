using System;
using System.Collections.Generic;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IPlatformRepository : IRepository<Platform>
    {
        IEnumerable<Guid> GetPlatformIdsByNames(IEnumerable<string> platformNames);
    }
}
