using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces.Services;
using GameStore.BLL.Services;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using Moq;
using Xunit;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.BLL.Tests
{
    public class PublisherServiceTest
    {
        private readonly IPublisherService _publisherService;
        private readonly Mock<IPublisherRepositoryFacade> _publisherRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;

        private readonly List<BusinessModels.Publisher> _publishers;
        private readonly List<DbModels.Publisher> _publishersFromDb;

        public PublisherServiceTest()
        {
            _publisherRepository = new Mock<IPublisherRepositoryFacade>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();

            _publisherService = new PublisherService(
                _publisherRepository.Object,
                _unitOfWork.Object,
                _mapper.Object);

            _publishers = new List<BusinessModels.Publisher>
            {
                new BusinessModels.Publisher {
                    PublisherId = Guid.NewGuid().ToString(),
                    CompanyName = "Publisher1",
                    Description = "Desc1",
                    HomePage = "store",
                },
                new BusinessModels.Publisher {
                    PublisherId = Guid.NewGuid().ToString(),
                    CompanyName = "Publisher2",
                    Description = "Desc2",
                    HomePage = "store",
                },
            };

            _publishersFromDb = new List<DbModels.Publisher>
            {
                new DbModels.Publisher {
                    PublisherId = _publishers.First().PublisherId,
                    CompanyName = "Publisher1", Description = "Desc1",
                    HomePage = "store",
                },
                new DbModels.Publisher {
                    PublisherId = _publishers.Last().PublisherId,
                    CompanyName = "Publisher2",
                    Description = "Desc2",
                    HomePage = "store",
                },
            };
        }

        [Fact]
        public void GetPublisherByName_PassCorrectId_ReturnsPublisher()
        {
            var publsiher = _publishers.First();
            var publisherFromDb = _publishersFromDb.First();

            _publisherRepository
                .Setup(p => p.GetAll())
                .Returns(_publishersFromDb);
            _mapper
                .Setup(m => m
                    .Map<BusinessModels.Publisher>(
                        It.IsAny<DbModels.Publisher>()))
                .Returns(publsiher);

            var result = _publisherService
                .GetPublisherByName(publsiher.CompanyName);

            Assert.Equal(publsiher, result);
        }

        [Fact]
        public void GetPublisherByName_PassIncorrectIdOrRemovedPublisherId_ReturnsNull()
        {
            var publisher = _publishers.First();

            _publisherRepository
                .Setup(p => p.GetById(It.IsAny<string>()))
                .Returns((DbModels.Publisher)null);

            var result = _publisherService
                .GetPublisherByName(publisher.CompanyName);

            Assert.Null(result);
        }

        [Fact]
        public void GetAllPublishers_ReturnsListOfPublishers()
        {
            _publisherRepository
                .Setup(p => p.GetAll())
                .Returns(_publishersFromDb);
            _mapper
                .Setup(m => m
                    .Map<IEnumerable<BusinessModels.Publisher>>(
                        _publishersFromDb))
                .Returns(_publishers);

            var result = _publisherService.GetAllPublishers().Count();

            Assert.Equal(_publishers.Count(), result);
        }

        [Fact]
        public void GetAllPublishers_ReturnsEmptyList()
        {
            var publishersFromDb = new List<DbModels.Publisher>();
            var publishers = new List<BusinessModels.Publisher>();

            _publisherRepository
                .Setup(g => g.GetAll())
                .Returns(publishersFromDb);
            _mapper
                .Setup(m => m
                    .Map<IEnumerable<BusinessModels.Publisher>>(
                        It.IsAny<IEnumerable<DbModels.Publisher>>()))
                .Returns(publishers);

            var result = _publisherService.GetAllPublishers();

            Assert.Empty(result);
        }

        [Fact]
        public void CreatePublisher_PassCorrectModel_ReturnsPublsiher()
        {
            _mapper
                .Setup(m => m
                    .Map<DbModels.Publisher>(
                        It.IsAny<BusinessModels.Publisher>()))
                .Returns(_publishersFromDb.First());
            _publisherRepository
                .Setup(p => p.GetAll())
                .Returns(_publishersFromDb);

            _publisherService.CreatePublisher(_publishers.First());

            _publisherRepository
                .Verify(p => p.Create(It.IsAny<DbModels.Publisher>()));
        }

        [Fact]
        public void DeletePublisher_PassPublisherModel()
        {
            _publisherRepository
                .Setup(p => p.IsPresent(It.IsAny<string>()))
                .Returns(true);
            _publisherRepository
                .Setup(g => g.GetById(It.IsAny<string>()))
                .Returns(_publishersFromDb.First());

            _publisherService.DeletePublisher(_publishers.First());

            _publisherRepository
                .Verify(g => g.Delete(_publishers.First().PublisherId));
        }

        [Fact]
        public void DeletePublisher_PassNonExistingPublisherModel()
        {
            var publisher = _publishersFromDb.First();
            publisher.IsRemoved = true;

            _publisherRepository
                .Setup(g => g.GetById(It.IsAny<string>()))
                .Returns(publisher);

            Assert
                .Throws<ArgumentException>(() => _publisherService
                    .DeletePublisher(_publishers.First()));

            publisher = null;

            Assert
                .Throws<ArgumentException>(() => _publisherService
                    .DeletePublisher(_publishers.First()));
        }

        [Fact]
        public void UpdatePublisher_PassPublisherModel_ReturnsPublisher()
        {
            _mapper
                .Setup(m => m
                    .Map<DbModels.Publisher>(
                        It.IsAny<BusinessModels.Publisher>()))
                .Returns(_publishersFromDb.First());

            var result = _publisherService.UpdatePublisher(_publishers.First());

            _publisherRepository
                .Verify(c => c
                    .Update(It.IsAny<string>(), It.IsAny<DbModels.Publisher>()));
            Assert.NotNull(result);
        }

        [Fact]
        public void GetpublisherById_PassPublisherId_ReturnsPublisher()
        {
            _publisherRepository.Setup(g => g.IsPresent(It.IsAny<string>())).Returns(true);
            _publisherRepository.Setup(c => c.GetById(It.IsAny<string>())).Returns(_publishersFromDb.First());
            _mapper.Setup(m => m.Map<BusinessModels.Publisher>(It.IsAny<DbModels.Publisher>())).Returns(_publishers.First());

            var result = _publisherService.GetPublisherById(_publishers.First().PublisherId);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetPublisherById_PassNonExistingPublisherId_ReturnsNull()
        {
            _publisherRepository
                .Setup(g => g.IsPresent(It.IsAny<string>()))
                .Returns(false);
            var publisher = _publishersFromDb.First();
            publisher.IsRemoved = true;

            var result = _publisherService
                .GetPublisherById(_publishers.First().PublisherId);

            Assert.Null(result);
        }
    }
}
