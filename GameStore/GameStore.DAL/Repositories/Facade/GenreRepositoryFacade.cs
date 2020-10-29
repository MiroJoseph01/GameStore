using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces.Repositories;

namespace GameStore.DAL.Repositories.Facade
{
    public class GenreRepositoryFacade : IGenreRepositoryFacade
    {
        private readonly IGenreRepository _firstSourceRepository;
        private readonly IMongoGenreRepository _secondSourceRepository;

        public GenreRepositoryFacade(
            IGenreRepository sqlRepository,
            IMongoGenreRepository mongoRepository)
        {
            _firstSourceRepository = sqlRepository;
            _secondSourceRepository = mongoRepository;
        }

        public void Create(Genre entity)
        {
            _firstSourceRepository.Create(entity);
        }

        public void Delete(string id)
        {
            if (_firstSourceRepository.IsPresent(id))
            {
                _firstSourceRepository.Delete(id);
            }

            if (_secondSourceRepository.IsPresent(id))
            {
                var entity = new Genre
                {
                    GenreId = id,
                    IsRemoved = true,
                };

                _firstSourceRepository.Create(entity);
            }
        }

        public IEnumerable<Genre> GetAll()
        {
            var sqlGenres = _firstSourceRepository.GetAll();

            var sqlGenreIds = sqlGenres.Select(x => x.GenreId).ToList();

            var mongoPublishers = _secondSourceRepository
                .GetAll()
                .Where(x => !sqlGenreIds.Contains(x.GenreId))
                .ToList();

            var result = new List<Genre>();

            result.AddRange(sqlGenres);
            result.AddRange(mongoPublishers);

            result = result.Where(x => !x.IsRemoved).ToList();

            return result;
        }

        public Genre GetById(string id)
        {
            var genre = _firstSourceRepository.GetById(id);

            if (genre != null && genre.IsRemoved)
            {
                return null;
            }

            if (genre is null)
            {
                genre = _secondSourceRepository.GetById(id);
            }

            return genre;
        }

        public IEnumerable<string> GetGenreIdsByNames(IEnumerable<string> genreNames)
        {
            var sqlGenreIds = _firstSourceRepository.GetGenreIdsByNames(genreNames);
            var mongoGenreIds = _secondSourceRepository.GetGenreIdsByNames(genreNames);

            var result = new List<string>();

            result.AddRange(sqlGenreIds);
            result.AddRange(mongoGenreIds);

            return result;
        }

        public bool IsPresent(string id)
        {
            return _firstSourceRepository.IsPresent(id) || _secondSourceRepository.IsPresent(id);
        }

        public void Update(string id, Genre entity)
        {
            if (_firstSourceRepository.IsPresent(id))
            {
                _firstSourceRepository.Update(id, entity);
            }

            if (_secondSourceRepository.IsPresent(id))
            {
                Create(entity);
            }
        }
    }
}
