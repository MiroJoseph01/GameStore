using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;

namespace GameStore.DAL.Repositories.Sql
{
    public class GenreRepository : Repository<Genre>, IGenreRepository
    {
        private readonly GameStoreContext _dbContext;

        public GenreRepository(GameStoreContext dbContext, IEntityStateLogger<Genre> stateLogger)
            : base(dbContext, stateLogger)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<string> GetGenreIdsByNames(IEnumerable<string> genreNames)
        {
            var genreIds = _dbContext.Genres
                .Where(x => genreNames.Contains(x.GenreName))
                .Select(x => x.GenreId)
                .ToList();

            return genreIds;
        }

        public override IEnumerable<Genre> GetAll()
        {
            return _dbContext.Set<Genre>()
                 .ToList();
        }

        public override Genre GetById(string id)
        {
            Genre entity = _dbContext.Set<Genre>().Find(id);

            return entity;
        }
    }
}
