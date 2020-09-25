using System;
using System.Collections.Generic;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.BLL.Services
{
    public class GenreService : IGenreService
    {
        private readonly IRepository<DbModels.Genre> _genreRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GenreService(IRepository<DbModels.Genre> genreRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _genreRepository = genreRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public BusinessModels.Genre AddGenre(BusinessModels.Genre genre)
        {
            _genreRepository.Create(_mapper.Map<DbModels.Genre>(genre));

            _unitOfWork.Commit();

            return genre;
        }

        public void DeleteGenre(BusinessModels.Genre genre)
        {
            if (!_genreRepository.IsPresent(genre.GenreId))
            {
                throw new ArgumentException("Genre doesn't exist or has already been deleted");
            }

            _genreRepository.Delete(genre.GenreId);

            _unitOfWork.Commit();
        }

        public BusinessModels.Genre UpdateGenre(BusinessModels.Genre genre)
        {
            _genreRepository.Update(genre.GenreId, _mapper.Map<DbModels.Genre>(genre));

            _unitOfWork.Commit();

            return genre;
        }

        public BusinessModels.Genre GetGenreById(Guid genreId)
        {
            var genreFromDb = _genreRepository.GetById(genreId);

            var genre = _mapper.Map<BusinessModels.Genre>(genreFromDb);

            return genre;
        }

        public IEnumerable<BusinessModels.Genre> GetAllGenres()
        {
            var genres = _mapper.Map<IEnumerable<BusinessModels.Genre>>(_genreRepository.GetAll());

            return genres;
        }
    }
}
