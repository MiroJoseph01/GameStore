using System.Collections.Generic;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IPlatformRepository : IRepository<Platform>
    {
        IEnumerable<string> GetPlatformIdsByNames(IEnumerable<string> platformNames);
    }
}
