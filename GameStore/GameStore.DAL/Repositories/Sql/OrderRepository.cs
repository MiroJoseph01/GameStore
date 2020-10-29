using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.Repositories.Sql
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly GameStoreContext _dbContext;

        public OrderRepository(GameStoreContext dbContext, IEntityStateLogger<Order> stateLogger)
            : base(dbContext, stateLogger)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Order> GetByCustomerId(string customerId)
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

        public override Order GetById(string id)
        {
            var res = _dbContext.Orders
                .Include(y => y.OrderDetails)
                .Include(z => z.OrderStatus)
                .FirstOrDefault(x => x.OrderId.Equals(id) && !x.IsRemoved);

            res.OrderDetails = res.OrderDetails.Where(x => !x.IsRemoved).ToList();

            return res;
        }
    }
}
