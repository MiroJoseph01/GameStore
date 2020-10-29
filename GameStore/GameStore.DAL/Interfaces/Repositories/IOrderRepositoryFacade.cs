using System.Collections.Generic;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IOrderRepositoryFacade : IRepository<Order>
    {
        IEnumerable<Order> GetByCustomerId(string customerId);
    }
}
