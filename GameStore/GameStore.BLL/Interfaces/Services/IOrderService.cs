using System;
using System.Collections.Generic;
using GameStore.BLL.Models;

namespace GameStore.BLL.Interfaces.Services
{
    public interface IOrderService
    {
        OrderStatus AddOrderStatus(OrderStatus orderStatus);

        void DeleteOrderStatus(OrderStatus orderStatus);

        OrderStatus UpdateOrderStatus(OrderStatus orderStatus);

        OrderStatus GetOrderStatusByName(string orderStatus);

        IEnumerable<OrderStatus> GetAllOrderStatuses();

        bool AddOrderDetail(string customerId, string gameKey);

        void DeleteOrderDetail(OrderDetail orderDetail);

        OrderDetail UpdateOrderDetail(OrderDetail orderDetail);

        OrderDetail GetOrderDetailById(string orderDetailId);

        IEnumerable<OrderDetail> GetAllOrderDetails();

        Order AddOrder(Order order);

        void DeleteOrder(Order order);

        Order UpdateOrder(Order order);

        Order GetOrderById(string orderId);

        IEnumerable<Order> GetAllOrders();

        IEnumerable<Order> GetOrdersByCustomerId(string customerId);

        bool DateIsCorrect(string dateFromCard);

        string GenrerateShortPaymentId(string id);

        void UpdateStatusOfOrder(string paymentId, string status);

        bool OrderIsPaid(string id);

        IEnumerable<Order> FilterByDate(DateTime minDate, DateTime maxDate, string customerId);
    }
}
