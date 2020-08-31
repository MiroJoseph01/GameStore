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
    public class PlatformService : IPlatformService
    {
        private readonly IRepository<DbModels.Platform> _platformRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlatformService(
            IRepository<DbModels.Platform> platformRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _platformRepository = platformRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public BusinessModels.Platform AddPlatform(
            BusinessModels.Platform platform)
        {
            _platformRepository
                .Create(_mapper.Map<DbModels.Platform>(platform));

            _unitOfWork.Commit();

            return platform;
        }

        public void DeletePlatform(BusinessModels.Platform platform)
        {
            if (_platformRepository.IsPresent(platform.PlatformId))
            {
                throw new ArgumentException(
                    "Platfrom doesn't exist or has been already deleted");
            }

            _platformRepository.Delete(platform.PlatformId);

            _unitOfWork.Commit();
        }

        public BusinessModels.Platform UpdatePlatform(
            BusinessModels.Platform platform)
        {
            _platformRepository
                .Update(
                platform.PlatformId,
                _mapper.Map<DbModels.Platform>(platform));

            _unitOfWork.Commit();

            return platform;
        }

        public BusinessModels.Platform GetPlatformById(Guid platformId)
        {
            DbModels.Platform platformFromDb =
                _platformRepository.GetById(platformId);

            var platform = _mapper.Map<BusinessModels.Platform>(platformFromDb);

            return platform;
        }

        public IEnumerable<BusinessModels.Platform> GetAllPlatforms()
        {
            var platforms = _mapper
                .Map<IEnumerable<BusinessModels.Platform>>(
                _platformRepository.GetAll());

            return platforms;
        }
    }
}
