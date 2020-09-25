using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly GameStoreContext _dbContext;

        public OrderRepository(GameStoreContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Order> GetByCustomerId(Guid customerId)
        {
            List<Order> res = _dbContext.Orders
                .Include(y => y.OrderDetails)
                .Include(z => z.OrderStatus)
                .Where(x => x.CustomerId.Equals(customerId) && x.IsRemoved == false)
                .ToList();

            foreach (var o in res)
            {
                o.OrderDetails = o.OrderDetails.Where(x => x.IsRemoved == false).ToList();
            }

            return res;
        }

        public override Order GetById(Guid id)
        {
            var res = _dbContext.Orders
                .Include(y => y.OrderDetails)
                .Include(z => z.OrderStatus)
                .FirstOrDefault(x => x.OrderId.Equals(id) && x.IsRemoved == false);

            res.OrderDetails = res.OrderDetails.Where(x => x.IsRemoved == false).ToList();

            return res;
        }
    }
}
