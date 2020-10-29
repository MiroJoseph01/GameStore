using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.DAL.Entities;
using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.Interfaces.Repositories;
using GameStore.DAL.Repositories.Mongo.Util;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameStore.DAL.Repositories.Mongo
{
    public class PublisherRepository : IMongoPublisherRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Supplier> _collection;
        private readonly IMapper _mapper;

        public PublisherRepository(
            IOptions<MongoSettings> mongoSettings, IMapper mapper)
        {
            _database = new MongoClient(mongoSettings.Value.ConnectionString).GetDatabase(mongoSettings.Value.Name);
            _collection = _database.GetCollection<Supplier>(RepositoryHelper.GetDescription(typeof(Supplier)));
            _mapper = mapper;
        }

        public IEnumerable<Publisher> GetAll()
        {
            var collection = _collection.Find(_ => true).ToList();

            var publishers = _mapper.Map<IEnumerable<Publisher>>(collection);

            return publishers;
        }

        public Publisher GetById(string id)
        {
            if (!IsPresent(id))
            {
                return null;
            }

            Publisher publisher = _mapper.Map<Publisher>(
                 _collection.Find(x => x.SupplierID == id).FirstOrDefault());

            return publisher;
        }

        public IEnumerable<string> GetPublisherIdsByNames(IEnumerable<string> publisherNames)
        {
            var publisherIds = _collection
                   .Find(x => publisherNames.Contains(x.CompanyName))
                   .ToList()
                   .Select(x => x.SupplierID.ToString())
                   .ToList();

            return publisherIds;
        }

        public bool IsPresent(string id)
        {
            return _collection.CountDocuments(x => x.SupplierID.Equals(id)) > 0;
        }
    }
}
