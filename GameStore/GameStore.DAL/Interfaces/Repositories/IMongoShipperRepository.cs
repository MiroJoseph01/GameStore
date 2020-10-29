using System.Collections.Generic;
using GameStore.DAL.Entities.MongoEntities;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IMongoShipperRepository
    {
        IEnumerable<Shipper> GetAll();

        Shipper GetById(string id);
    }
}
