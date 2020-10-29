using System.Collections.Generic;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IMongoGenreRepository
    {
        IEnumerable<Genre> GetAll();

        Genre GetById(string id);

        IEnumerable<string> GetGenreIdsByNames(IEnumerable<string> genreNames);

        bool IsPresent(string id);
    }
}
