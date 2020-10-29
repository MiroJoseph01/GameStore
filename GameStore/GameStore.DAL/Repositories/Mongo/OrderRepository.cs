using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.DAL.Interfaces.Repositories;
using GameStore.DAL.Repositories.Mongo.Util;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoEntities = GameStore.DAL.Entities.MongoEntities;
using SqlEntities = GameStore.DAL.Entities;

namespace GameStore.DAL.Repositories.Mongo
{
    public class OrderRepository : IMongoOrderRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<MongoEntities.Order> _collection;
        private readonly IMongoCollection<MongoEntities.OrderDetail> _detailsCollection;
        private readonly IMapper _mapper;

        public OrderRepository(
            IOptions<MongoSettings> mongoSettings, IMapper mapper)
        {
            _database = new MongoClient(mongoSettings.Value.ConnectionString).GetDatabase(mongoSettings.Value.Name);
            _collection = _database
                .GetCollection<MongoEntities.Order>(
                    RepositoryHelper.GetDescription(typeof(MongoEntities.Order)));
            _detailsCollection = _database
                .GetCollection<MongoEntities.OrderDetail>(
                    RepositoryHelper.GetDescription(typeof(MongoEntities.OrderDetail)));
            _mapper = mapper;
        }

        public IEnumerable<SqlEntities.Order> GetAll()
        {
            var orders = _collection.Find(x => true).ToList();

            var result = _mapper.Map<IEnumerable<SqlEntities.Order>>(orders);

            foreach (var o in result)
            {
                var details = _detailsCollection.Find(x => x.OrderID.Equals(o.OrderId)).ToList();
                var mappedDetails = _mapper.Map<IEnumerable<SqlEntities.OrderDetail>>(details);

                o.OrderDetails = mappedDetails.ToList();
            }

            return result;
        }

        public IEnumerable<SqlEntities.Order> GetByCustomerId(string customerId)
        {
            var orders = _collection.Find(x => x.CustomerID == customerId).ToList();

            var result = _mapper.Map<IEnumerable<SqlEntities.Order>>(orders);

            foreach (var o in result)
            {
                var details = _detailsCollection.Find(x => x.OrderID.Equals(o.OrderId)).ToList();
                var mappedDetails = _mapper.Map<IEnumerable<SqlEntities.OrderDetail>>(details);

                o.OrderDetails = mappedDetails.ToList();
            }

            return result;
        }

        public SqlEntities.Order GetById(string id)
        {
            var order = _collection.Find(x => x.OrderID == id).FirstOrDefault();

            if (order != null)
            {
                var result = _mapper.Map<SqlEntities.Order>(order);

                var details = _detailsCollection.Find(x => x.OrderID.Equals(result.OrderId)).ToList();
                var mappedDetails = _mapper.Map<IEnumerable<SqlEntities.OrderDetail>>(details);

                result.OrderDetails = mappedDetails.ToList();

                return result;
            }

            return null;
        }

        public bool IsPresent(string id)
        {
            return _collection.CountDocuments(x => x.OrderID.Equals(id)) > 0;
        }
    }
}
