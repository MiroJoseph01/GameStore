using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces.Services;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.BLL.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly IPublisherRepositoryFacade _publisherRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PublisherService(
            IPublisherRepositoryFacade publisherRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _publisherRepository = publisherRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public BusinessModels.Publisher CreatePublisher(BusinessModels.Publisher publisher)
        {
            var publisherForDB = _mapper
                .Map<DbModels.Publisher>(publisher);

            publisherForDB.PublisherId = Guid.NewGuid().ToString();

            _publisherRepository.Create(publisherForDB);
            _unitOfWork.Commit();

            return GetPublisherByName(publisher.CompanyName);
        }

        public void DeletePublisher(BusinessModels.Publisher publisher)
        {
            if (!_publisherRepository.IsPresent(publisher.PublisherId))
            {
                throw new ArgumentException("Publisher doesn't exist or has already been deleted");
            }

            _publisherRepository.Delete(publisher.PublisherId);

            _unitOfWork.Commit();
        }

        public BusinessModels.Publisher UpdatePublisher(
            BusinessModels.Publisher publisher)
        {
            _publisherRepository.Update(publisher.PublisherId, _mapper.Map<DbModels.Publisher>(publisher));

            _unitOfWork.Commit();

            return publisher;
        }

        public BusinessModels.Publisher GetPublisherById(string publisherId)
        {
            DbModels.Publisher publisherFromDb = _publisherRepository.GetById(publisherId);

            return _mapper.Map<BusinessModels.Publisher>(publisherFromDb);
        }

        public IEnumerable<BusinessModels.Publisher> GetAllPublishers()
        {
            IEnumerable<DbModels.Publisher> publishers = _publisherRepository.GetAll();

            var publishersResult = _mapper.Map<IEnumerable<BusinessModels.Publisher>>(publishers);

            return publishersResult;
        }

        public BusinessModels.Publisher GetPublisherByName(string publisherName)
        {
            DbModels.Publisher publisher = _publisherRepository
                .GetAll()
                .FirstOrDefault(
                x => x.CompanyName.ToLower() == publisherName.ToLower());

            var publisherResult = _mapper.Map<BusinessModels.Publisher>(publisher);

            return publisherResult;
        }

        public bool IsPresent(string name)
        {
            return GetPublisherByName(name) != null;
        }
    }
}
