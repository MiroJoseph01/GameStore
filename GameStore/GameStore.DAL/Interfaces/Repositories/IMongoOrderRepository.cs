using System.Collections.Generic;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IMongoOrderRepository
    {
        IEnumerable<Order> GetAll();

        IEnumerable<Order> GetByCustomerId(string customerId);

        Order GetById(string customerId);

        bool IsPresent(string id);
    }
}
