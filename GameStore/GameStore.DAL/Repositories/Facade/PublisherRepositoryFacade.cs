using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces.Repositories;

namespace GameStore.DAL.Repositories.Facade
{
    public class PublisherRepositoryFacade : IPublisherRepositoryFacade
    {
        private readonly IPublisherRepository _firstSourceRepository;
        private readonly IMongoPublisherRepository _secondSourceRepository;

        public PublisherRepositoryFacade(
            IPublisherRepository sqlRepository,
            IMongoPublisherRepository mongoRepository)
        {
            _firstSourceRepository = sqlRepository;
            _secondSourceRepository = mongoRepository;
        }

        public void Create(Publisher entity)
        {
            _firstSourceRepository.Create(entity);
        }

        public void Delete(string id)
        {
            if (_firstSourceRepository.IsPresent(id))
            {
                var publisher = _firstSourceRepository.GetById(id);

                _firstSourceRepository.Delete(id);
            }

            if (_secondSourceRepository.IsPresent(id))
            {
                var publisher = new Publisher
                {
                    PublisherId = id,
                    IsRemoved = true,
                };

                _firstSourceRepository.Create(publisher);
            }
        }

        public IEnumerable<Publisher> GetAll()
        {
            var sqlPublishers = _firstSourceRepository.GetAll();

            var sqlPublisherIds = sqlPublishers.Select(x => x.PublisherId).ToList();

            var mongoPublishers = _secondSourceRepository
                .GetAll()
                .Where(x => !sqlPublisherIds.Contains(x.PublisherId))
                .ToList();

            var result = new List<Publisher>();

            result.AddRange(sqlPublishers);
            result.AddRange(mongoPublishers);

            result = result.Where(x => !x.IsRemoved).ToList();

            return result;
        }

        public Publisher GetById(string id)
        {
            var publisher = _firstSourceRepository.GetById(id);

            if (publisher != null && publisher.IsRemoved)
            {
                return null;
            }

            if (publisher is null)
            {
                publisher = _secondSourceRepository.GetById(id);
            }

            return publisher;
        }

        public IEnumerable<string> GetPublisherIdsByNames(IEnumerable<string> publisherNames)
        {
            var sqlPublisherIds = _firstSourceRepository.GetPublisherIdsByNames(publisherNames);
            var mongoPublisherIds = _secondSourceRepository.GetPublisherIdsByNames(publisherNames);

            var result = new List<string>();

            result.AddRange(sqlPublisherIds);
            result.AddRange(mongoPublisherIds);

            return result;
        }

        public bool IsPresent(string id)
        {
            return _firstSourceRepository.IsPresent(id) || _secondSourceRepository.IsPresent(id);
        }

        public void Update(string id, Publisher entity)
        {
            if (_firstSourceRepository.IsPresent(id))
            {
                var publisher = _firstSourceRepository.GetById(id);

                _firstSourceRepository.Update(id, entity);
            }

            if (_secondSourceRepository.IsPresent(id))
            {
                Create(entity);
            }
        }
    }
}
