using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces.Repositories;

namespace GameStore.DAL.Repositories
{
    public class GenreRepository : Repository<Genre>, IGenreRepository
    {
        private readonly GameStoreContext _dbContext;

        public GenreRepository(GameStoreContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Guid> GetGenreIdsByNames(IEnumerable<string> genreNames)
        {
            var genreIds = _dbContext.Genres
                .Where(x => genreNames.Contains(x.GenreName))
                .Select(x => x.GenreId)
                .ToList();

            return genreIds;
        }
    }
}
