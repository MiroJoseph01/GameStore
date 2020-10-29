using System.Collections.Generic;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IGenreRepositoryFacade : IRepository<Genre>
    {
        IEnumerable<string> GetGenreIdsByNames(IEnumerable<string> genreNames);
    }
}
