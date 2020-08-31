using System;
using System.Collections.Generic;
using GameStore.BLL.Models;

namespace GameStore.BLL.Interfaces
{
    public interface IOrderService
    {
        OrderStatus AddOrderStatus(OrderStatus orderStatus);

        void DeleteOrderStatus(OrderStatus orderStatus);

        OrderStatus UpdateOrderStatus(OrderStatus orderStatus);

        OrderStatus GetOrderStatusById(Guid orderStatusId);

        IEnumerable<OrderStatus> GetAllOrderStatuses();

        void AddOrderDetail(string customerId, string gameKey);

        void DeleteOrderDetail(OrderDetail orderDetail);

        OrderDetail UpdateOrderDetail(OrderDetail orderDetail);

        OrderDetail GetOrderDetailById(Guid orderDetailId);

        IEnumerable<OrderDetail> GetAllOrderDetails();

        Order AddOrder(Order order);

        void DeleteOrder(Order order);

        Order UpdateOrder(Order order);

        Order GetOrderById(Guid orderId);

        IEnumerable<Order> GetAllOrders();

        IEnumerable<Order> GetOrdersByCustomerId(string customerId);
    }
}
