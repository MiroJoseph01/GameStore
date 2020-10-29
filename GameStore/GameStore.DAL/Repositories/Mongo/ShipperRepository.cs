using System.Collections.Generic;
using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.Interfaces.Repositories;
using GameStore.DAL.Repositories.Mongo.Util;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameStore.DAL.Repositories.Mongo
{
    public class ShipperRepository : IMongoShipperRepository
    {
        private readonly IMongoCollection<Shipper> _collection;

        public ShipperRepository(IOptions<MongoSettings> mongoSettings)
        {
            _collection = new MongoClient(mongoSettings.Value.ConnectionString)
                .GetDatabase(mongoSettings.Value.Name)
                .GetCollection<Shipper>(RepositoryHelper.GetDescription(typeof(Shipper)));
        }

        public IEnumerable<Shipper> GetAll()
        {
            return _collection.Find(x => true).ToEnumerable();
        }

        public Shipper GetById(string id)
        {
            return _collection.Find(x => x.ShipperID == id).FirstOrDefault();
        }
    }
}
