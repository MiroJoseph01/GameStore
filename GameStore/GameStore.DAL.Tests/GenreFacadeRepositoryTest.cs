using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces.Repositories;
using GameStore.DAL.Repositories.Facade;
using Moq;
using Xunit;

namespace GameStore.DAL.Tests
{
    public class GenreFacadeRepositoryTest
    {
        private readonly GenreRepositoryFacade _genreRepository;
        private readonly Mock<IGenreRepository> _firstSourceRepository;
        private readonly Mock<IMongoGenreRepository> _secondSourceRepository;

        private readonly List<Genre> _sqlGenres;
        private readonly List<Genre> _mongoGenres;

        public GenreFacadeRepositoryTest()
        {
            _firstSourceRepository = new Mock<IGenreRepository>();
            _secondSourceRepository = new Mock<IMongoGenreRepository>();

            _genreRepository = new GenreRepositoryFacade(_firstSourceRepository.Object, _secondSourceRepository.Object);

            _sqlGenres = new List<Genre>
            {
                new Genre
                {
                    GenreId = "1",
                    GenreName = "Name",
                },
            };

            _mongoGenres = new List<Genre>
            {
                new Genre
                {
                    GenreId = "2",
                    GenreName = "Name",
                },
            };
        }

        [Fact]
        public void Create_PassGenre_VerifyCreate()
        {
            _genreRepository.Create(It.IsAny<Genre>());

            _firstSourceRepository.Verify(x => x.Create(It.IsAny<Genre>()));
        }

        [Fact]
        public void Delete_PassSqlGenre_VerifyDekete()
        {
            _firstSourceRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(true);

            _genreRepository.Delete(It.IsAny<string>());

            _firstSourceRepository.Verify(x => x.Delete(It.IsAny<string>()));
        }

        [Fact]
        public void Delete_PassMongoGenre_VerifyDekete()
        {
            _secondSourceRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(true);

            _genreRepository.Delete(It.IsAny<string>());

            _firstSourceRepository.Verify(x => x.Create(It.IsAny<Genre>()));
        }

        [Fact]
        public void GetAll_ReturnsListOfGenres()
        {
            _firstSourceRepository.Setup(g => g.GetAll()).Returns(_sqlGenres);
            _secondSourceRepository.Setup(g => g.GetAll()).Returns(_mongoGenres);

            var expected = _sqlGenres.Count + _mongoGenres.Count;

            var result = _genreRepository.GetAll();

            Assert.Equal(expected, result.Count());
        }

        [Fact]
        public void GetById_PassSqlGenreId_ReturnsGenre()
        {
            _firstSourceRepository.Setup(g => g.GetById(It.IsAny<string>())).Returns(_sqlGenres.First());

            var result = _genreRepository.GetById(It.IsAny<string>());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetById_PassMongoGenreId_ReturnsGenre()
        {
            _firstSourceRepository.Setup(g => g.GetById(It.IsAny<string>())).Returns((Genre)null);
            _secondSourceRepository.Setup(g => g.GetById(It.IsAny<string>())).Returns(_mongoGenres.First());

            var result = _genreRepository.GetById(It.IsAny<string>());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetById_PassDeletedGenreId_ReturnsNull()
        {
            var genre = _sqlGenres.First();
            genre.IsRemoved = true;
            _firstSourceRepository.Setup(g => g.GetById(It.IsAny<string>())).Returns((Genre)null);

            var result = _genreRepository.GetById(It.IsAny<string>());

            Assert.Null(result);
        }

        [Fact]
        public void GetGenreIdsByNames_PassListOfGenreNames_ReturnsListOfGenresIds()
        {
            var sqlGenresIds = new List<string> { "1", "2" };
            var mongoGenresIds = new List<string> { "3", "4" };

            var expected = sqlGenresIds.Count + mongoGenresIds.Count;

            _firstSourceRepository
                .Setup(g => g.GetGenreIdsByNames(It.IsAny<IEnumerable<string>>()))
                .Returns(sqlGenresIds);
            _secondSourceRepository
                .Setup(g => g.GetGenreIdsByNames(It.IsAny<IEnumerable<string>>()))
                .Returns(mongoGenresIds);

            var result = _genreRepository.GetGenreIdsByNames(It.IsAny<IEnumerable<string>>());

            Assert.Equal(expected, result.Count());
        }

        [Fact]
        public void IsPresent_PassGenreId_ReturnsTrue()
        {
            _firstSourceRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(true);
            _secondSourceRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(true);

            var result = _genreRepository.IsPresent(It.IsAny<string>());

            Assert.True(result);
        }

        [Fact]
        public void IsPresent_PassGenreId_ReturnsFalse()
        {
            _firstSourceRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(false);
            _secondSourceRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(false);

            var result = _genreRepository.IsPresent(It.IsAny<string>());

            Assert.False(result);
        }

        [Fact]
        public void Update_PassGenreIdAndEntityFromSql_VerifyUpdate()
        {
            _firstSourceRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(true);

            _genreRepository.Update(It.IsAny<string>(), It.IsAny<Genre>());

            _firstSourceRepository.Verify(g => g.Update(It.IsAny<string>(), It.IsAny<Genre>()));
        }

        [Fact]
        public void Update_PassGenreIdAndEntityFromMongo_VerifyCreate()
        {
            _secondSourceRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(true);

            _genreRepository.Update(It.IsAny<string>(), It.IsAny<Genre>());

            _firstSourceRepository.Verify(g => g.Create(It.IsAny<Genre>()));
        }
    }
}
