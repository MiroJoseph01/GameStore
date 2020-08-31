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
    public class PlatformServiceTest
    {
        private readonly IPlatformService _platformService;
        private readonly Mock<IRepository<DbModels.Platform>> _platformRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;

        private readonly List<BusinessModels.Platform> _platforms;
        private readonly List<DbModels.Platform> _platformsFromDb;

        public PlatformServiceTest()
        {
            _platformRepository = new Mock<IRepository<DbModels.Platform>>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();

            _platformService = new PlatformService(
                _platformRepository.Object,
                _unitOfWork.Object,
                _mapper.Object);

            _platforms = new List<BusinessModels.Platform>
            {
                new BusinessModels.Platform
                { PlatformId = Guid.NewGuid(), PlatformName = "Platform1" },
                new BusinessModels.Platform
                { PlatformId = Guid.NewGuid(), PlatformName = "Platform2" },
            };

            _platformsFromDb = new List<DbModels.Platform>
            {
                new DbModels.Platform
                {
                    PlatformId = _platforms.First().PlatformId,
                    PlatformName = _platforms.First().PlatformName,
                },
                new DbModels.Platform
                {
                    PlatformId = _platforms.Last().PlatformId,
                    PlatformName = _platforms.Last().PlatformName,
                },
            };
        }

        [Fact]
        public void GetAllPlatforms_ReturnsListOfPlatforms()
        {
            _platformRepository.Setup(p => p.GetAll()).Returns(_platformsFromDb);
            _mapper
                .Setup(m => m
                    .Map<IEnumerable<BusinessModels.Platform>>(
                    _platformsFromDb))
                .Returns(_platforms);

            int result = _platformService.GetAllPlatforms().Count();

            Assert.Equal(_platforms.Count(), result);
        }

        [Fact]
        public void GetAllPlatforms_ReturnsEmptyList()
        {
            List<DbModels.Platform> platformsFromDb =
                new List<DbModels.Platform>();
            List<BusinessModels.Platform> platforms =
                new List<BusinessModels.Platform>();

            _platformRepository.Setup(g => g.GetAll()).Returns(_platformsFromDb);
            _mapper
                .Setup(m => m
                    .Map<IEnumerable<BusinessModels.Platform>>(
                        It.IsAny<IEnumerable<DbModels.Platform>>()))
                .Returns(platforms);

            IEnumerable<BusinessModels.Platform> result = _platformService
                .GetAllPlatforms();

            Assert.Empty(result);
        }

        [Fact]
        public void AddPlatform_PassPlatformModel_ReturnsPlatform()
        {
            _mapper
                .Setup(m => m.Map<DbModels.Platform>(
                    It.IsAny<BusinessModels.Platform>()))
                .Returns(_platformsFromDb.First());

            var result = _platformService.AddPlatform(_platforms.First());

            _platformRepository.Verify(g => g.Create(It.IsAny<DbModels.Platform>()));
            Assert.NotNull(result);
        }

        [Fact]
        public void DeletePlatform_PassPlatformModel()
        {
            _platformRepository
                .Setup(g => g.GetById(It.IsAny<Guid>()))
                .Returns(_platformsFromDb.First());

            _platformService.DeletePlatform(_platforms.First());

            _platformRepository
                .Verify(g => g.Delete(_platforms.First().PlatformId));
        }

        [Fact]
        public void DeletePlatform_PassNonExistingPlatformModel()
        {
            var platform = _platformsFromDb.First();
            platform.IsRemoved = true;

            _platformRepository
                .Setup(p => p.IsPresent(It.IsAny<Guid>()))
                .Returns(true);
            _platformRepository
                .Setup(g => g.GetById(It.IsAny<Guid>()))
                .Returns(platform);

            Assert
                .Throws<ArgumentException>(() => _platformService
                    .DeletePlatform(_platforms.First()));

            platform = null;

            Assert
                .Throws<ArgumentException>(() => _platformService
                    .DeletePlatform(_platforms.First()));
        }

        [Fact]
        public void UpdatePlatform_PassPlatformModel_ReturnsPlatform()
        {
            _mapper
                .Setup(m => m.Map<DbModels.Platform>(
                    It.IsAny<BusinessModels.Platform>()))
                .Returns(_platformsFromDb.First());

            var result = _platformService.UpdatePlatform(_platforms.First());

            _platformRepository
                .Verify(c => c
                    .Update(It.IsAny<Guid>(), It.IsAny<DbModels.Platform>()));
            Assert.NotNull(result);
        }

        [Fact]
        public void GetPlatformById_PassPlatformId_ReturnsPlatform()
        {
            _platformRepository
                .Setup(g => g.IsPresent(It.IsAny<Guid>()))
                .Returns(true);
            _platformRepository
                .Setup(c => c.GetById(It.IsAny<Guid>()))
                .Returns(_platformsFromDb.First());
            _mapper
                .Setup(m => m
                    .Map<BusinessModels.Platform>(
                        It.IsAny<DbModels.Platform>()))
                .Returns(_platforms.First());

            var result = _platformService
                .GetPlatformById(_platforms.First().PlatformId);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetPlatformById_PassNonExistingPlatformId_ReturnsNull()
        {
            _platformRepository
                .Setup(g => g.IsPresent(It.IsAny<Guid>()))
                .Returns(false);
            var platform = _platformsFromDb.First();
            platform.IsRemoved = true;

            var result = _platformService
                .GetPlatformById(_platforms.First().PlatformId);

            Assert.Null(result);
        }
    }
}
