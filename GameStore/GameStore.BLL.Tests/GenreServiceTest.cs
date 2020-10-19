using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Services;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using Moq;
using Xunit;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.BLL.Tests
{
    public class GenreServiceTest
    {
        private readonly IGenreService _genreService;
        private readonly Mock<IRepository<DbModels.Genre>> _genreRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;

        private readonly List<BusinessModels.Genre> _genres;
        private readonly List<DbModels.Genre> _genresFromDb;

        public GenreServiceTest()
        {
            _genreRepository = new Mock<IRepository<DbModels.Genre>>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();

            _genreService = new GenreService(
                _genreRepository.Object,
                _unitOfWork.Object,
                _mapper.Object);

            _genres = new List<BusinessModels.Genre>
            {
                new BusinessModels.Genre
                {
                    GenreId = Guid.NewGuid(),
                    GenreName = "Genre1 name",
                },
                new BusinessModels.Genre
                {
                    GenreId = Guid.NewGuid(),
                    GenreName = "Genre2 name",
                },
            };

            _genresFromDb = new List<DbModels.Genre>
            {
                new DbModels.Genre
                {
                    GenreId = _genres.First().GenreId,
                    GenreName = _genres.First().GenreName,
                },
                new DbModels.Genre
                {
                    GenreId = _genres.Last().GenreId,
                    GenreName = _genres.Last().GenreName,
                },
            };
        }

        [Fact]
        public void GetAllGenres_ReturnsListOfGenres()
        {
            _genreRepository
                .Setup(p => p.GetAll())
                .Returns(_genresFromDb);
            _mapper
                .Setup(m => m
                    .Map<IEnumerable<BusinessModels.Genre>>(_genresFromDb))
                .Returns(_genres);

            int result = _genreService.GetAllGenres().Count();

            Assert.Equal(_genres.Count(), result);
        }

        [Fact]
        public void GetAllGenres_ReturnsEmptyList()
        {
            List<DbModels.Genre> genresFromDb = new List<DbModels.Genre>();
            List<BusinessModels.Genre> genres =
                new List<BusinessModels.Genre>();

            _genreRepository
                .Setup(g => g.GetAll())
                .Returns(genresFromDb);
            _mapper
                .Setup(m => m
                    .Map<IEnumerable<BusinessModels.Genre>>(
                        It.IsAny<IEnumerable<DbModels.Genre>>()))
                .Returns(genres);

            IEnumerable<BusinessModels.Genre> result = _genreService
                .GetAllGenres();

            Assert.Empty(result);
        }

        [Fact]
        public void AddGenre_PassGenreModel_ReturnsGenre()
        {
            _mapper
                .Setup(m => m
                    .Map<DbModels.Genre>(It.IsAny<BusinessModels.Genre>()))
                .Returns(_genresFromDb.First());

            var result = _genreService.AddGenre(_genres.First());

            _genreRepository.Verify(g => g.Create(It.IsAny<DbModels.Genre>()));
            Assert.NotNull(result);
        }

        [Fact]
        public void DeleteGenre_PassGenreModel()
        {
            _genreRepository
                .Setup(g => g.IsPresent(It.IsAny<Guid>()))
                .Returns(true);

            _genreRepository
                .Setup(g => g.GetById(It.IsAny<Guid>()))
                .Returns(_genresFromDb.First());

            _genreService.DeleteGenre(_genres.First());

            _genreRepository.Verify(g => g.Delete(_genres.First().GenreId));
        }

        [Fact]
        public void DeleteGenre_PassNonExistingGenreModel()
        {
            var genre = _genresFromDb.First();
            genre.IsRemoved = true;

            _genreRepository.Setup(g => g.GetById(It.IsAny<Guid>())).Returns(genre);

            Assert
                .Throws<ArgumentException>(() => _genreService
                .DeleteGenre(_genres.First()));

            genre = null;

            Assert
                .Throws<ArgumentException>(() => _genreService
                .DeleteGenre(_genres.First()));
        }

        [Fact]
        public void UpdateGenre_PassGenreModel_ReturnsGenre()
        {
            _mapper
                .Setup(m => m
                    .Map<DbModels.Genre>(It.IsAny<BusinessModels.Genre>()))
                .Returns(_genresFromDb.First());

            var result = _genreService.UpdateGenre(_genres.First());

            _genreRepository
                .Verify(c => c
                    .Update(It.IsAny<Guid>(), It.IsAny<DbModels.Genre>()));
            Assert.NotNull(result);
        }

        [Fact]
        public void GetGenreById_PassGenreId_ReturnsGenre()
        {
            _genreRepository
                .Setup(g => g.IsPresent(It.IsAny<Guid>()))
                .Returns(true);
            _genreRepository
                .Setup(c => c.GetById(It.IsAny<Guid>()))
                .Returns(_genresFromDb.First());
            _mapper
                .Setup(m => m
                    .Map<BusinessModels.Genre>(It.IsAny<DbModels.Genre>()))
                .Returns(_genres.First());

            var result = _genreService.GetGenreById(_genres.First().GenreId);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetGenreById_PassNonExistingGenreId_ReturnsNull()
        {
            _genreRepository
                .Setup(g => g.IsPresent(It.IsAny<Guid>()))
                .Returns(false);
            var genre = _genresFromDb.First();
            genre.IsRemoved = true;

            var result = _genreService.GetGenreById(_genres.First().GenreId);

            Assert.Null(result);
        }
    }
}
