using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces.Repositories;
using GameStore.DAL.Repositories.Facade;
using Moq;
using Xunit;

namespace GameStore.DAL.Tests
{
    public class PublisherFacadeRepositoryTest
    {
        private readonly PublisherRepositoryFacade _publisherRepository;
        private readonly Mock<IPublisherRepository> _firstSourceRepository;
        private readonly Mock<IMongoPublisherRepository> _secondSourceRepository;

        private readonly List<Publisher> _sqlPublishers;
        private readonly List<Publisher> _mongoPublishers;

        public PublisherFacadeRepositoryTest()
        {
            _firstSourceRepository = new Mock<IPublisherRepository>();
            _secondSourceRepository = new Mock<IMongoPublisherRepository>();

            _publisherRepository = new PublisherRepositoryFacade(_firstSourceRepository.Object, _secondSourceRepository.Object);

            _sqlPublishers = new List<Publisher>
            {
                new Publisher
                {
                    PublisherId = "1",
                    CompanyName = "Name",
                },
            };

            _mongoPublishers = new List<Publisher>
            {
                new Publisher
                {
                    PublisherId = "2",
                    CompanyName = "Name",
                },
            };
        }

        [Fact]
        public void Create_PassPublisher_VerifyCreate()
        {
            _publisherRepository.Create(It.IsAny<Publisher>());

            _firstSourceRepository.Verify(x => x.Create(It.IsAny<Publisher>()));
        }

        [Fact]
        public void Delete_PassSqlPublisher_VerifyDekete()
        {
            _firstSourceRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(true);

            _publisherRepository.Delete(It.IsAny<string>());

            _firstSourceRepository.Verify(x => x.Delete(It.IsAny<string>()));
        }

        [Fact]
        public void Delete_PassMongoPublisher_VerifyDelete()
        {
            _secondSourceRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(true);

            _publisherRepository.Delete(It.IsAny<string>());

            _firstSourceRepository.Verify(x => x.Create(It.IsAny<Publisher>()));
        }

        [Fact]
        public void GetAll_ReturnsListOfPublishers()
        {
            _firstSourceRepository.Setup(g => g.GetAll()).Returns(_sqlPublishers);
            _secondSourceRepository.Setup(g => g.GetAll()).Returns(_mongoPublishers);

            var expected = _sqlPublishers.Count + _mongoPublishers.Count;

            var result = _publisherRepository.GetAll();

            Assert.Equal(expected, result.Count());
        }

        [Fact]
        public void GetById_PassSqlGenreId_ReturnsPublisher()
        {
            _firstSourceRepository.Setup(g => g.GetById(It.IsAny<string>())).Returns(_sqlPublishers.First());

            var result = _publisherRepository.GetById(It.IsAny<string>());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetById_PassMongoPublisherId_ReturnsPublisher()
        {
            _firstSourceRepository.Setup(g => g.GetById(It.IsAny<string>())).Returns((Publisher)null);
            _secondSourceRepository.Setup(g => g.GetById(It.IsAny<string>())).Returns(_mongoPublishers.First());

            var result = _publisherRepository.GetById(It.IsAny<string>());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetById_PassDeletedPublisherId_ReturnsNull()
        {
            var publisher = _sqlPublishers.First();
            publisher.IsRemoved = true;
            _firstSourceRepository.Setup(g => g.GetById(It.IsAny<string>())).Returns((Publisher)null);

            var result = _publisherRepository.GetById(It.IsAny<string>());

            Assert.Null(result);
        }

        [Fact]
        public void GetPublisherIdsByNames_PassListOfPublisherNames_ReturnsListOfPublishersIds()
        {
            var sqlGenresIds = new List<string> { "1", "2" };
            var mongoGenresIds = new List<string> { "3", "4" };

            var expected = sqlGenresIds.Count + mongoGenresIds.Count;

            _firstSourceRepository
                .Setup(g => g.GetPublisherIdsByNames(It.IsAny<IEnumerable<string>>()))
                .Returns(sqlGenresIds);
            _secondSourceRepository
                .Setup(g => g.GetPublisherIdsByNames(It.IsAny<IEnumerable<string>>()))
                .Returns(mongoGenresIds);

            var result = _publisherRepository.GetPublisherIdsByNames(It.IsAny<IEnumerable<string>>());

            Assert.Equal(expected, result.Count());
        }

        [Fact]
        public void IsPresent_PassPublisherId_ReturnsTrue()
        {
            _firstSourceRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(true);
            _secondSourceRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(true);

            var result = _publisherRepository.IsPresent(It.IsAny<string>());

            Assert.True(result);
        }

        [Fact]
        public void IsPresent_PassPublisherId_ReturnsFalse()
        {
            _firstSourceRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(false);
            _secondSourceRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(false);

            var result = _publisherRepository.IsPresent(It.IsAny<string>());

            Assert.False(result);
        }

        [Fact]
        public void Update_PassPublisherIdAndEntityFromSql_VerifyUpdate()
        {
            _firstSourceRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(true);

            _publisherRepository.Update(It.IsAny<string>(), It.IsAny<Publisher>());

            _firstSourceRepository.Verify(g => g.Update(It.IsAny<string>(), It.IsAny<Publisher>()));
        }

        [Fact]
        public void Update_PassPublisherIdAndEntityFromMongo_VerifyCreate()
        {
            _secondSourceRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(true);

            _publisherRepository.Update(It.IsAny<string>(), It.IsAny<Publisher>());

            _firstSourceRepository.Verify(g => g.Create(It.IsAny<Publisher>()));
        }
    }
}
