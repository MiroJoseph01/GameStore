using System;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces.Repositories;

namespace GameStore.DAL.Repositories
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly GameStoreContext _dbContext;

        public OrderDetailRepository(GameStoreContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public OrderDetail GetOrderDetailByOrderIdAndProductId(
            Guid orderId,
            string productId)
        {
            var detail = _dbContext
                .OrderDetails
                .FirstOrDefault(
                    x => x.OrderId
                    .Equals(orderId) && x.ProductId.Equals(productId));

            return detail;
        }
    }
}
