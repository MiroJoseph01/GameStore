using System;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        OrderDetail GetOrderDetailByOrderIdAndProductId(Guid orderId, string productId);
    }
}
