using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;

namespace GameStore.DAL.Repositories.Sql
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly GameStoreContext _dbContext;

        public OrderDetailRepository(GameStoreContext dbContext, IEntityStateLogger<OrderDetail> stateLogger)
            : base(dbContext, stateLogger)
        {
            _dbContext = dbContext;
        }

        public OrderDetail GetOrderDetailByOrderIdAndProductId(
            string orderId,
            string productId)
        {
            var detail = _dbContext.OrderDetails
                .FirstOrDefault(x => x.OrderId.Equals(orderId) && x.ProductId.Equals(productId));

            return detail;
        }
    }
}
