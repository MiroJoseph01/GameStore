using System;
using System.Collections.Generic;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        IEnumerable<Order> GetByCustomerId(Guid customerId);
    }
}
