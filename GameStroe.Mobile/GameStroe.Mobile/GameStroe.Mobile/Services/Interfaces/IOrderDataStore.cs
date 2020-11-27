using GameStroe.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameStroe.Mobile.Services.Interfaces
{
    public interface IOrderDataStore
    {
        void AddDetail(string gameKey, string userId);

        void DeleteDetail(string id);

        Task<IEnumerable<Order>> GetOrdersByCustomerId(string customerId);
            
        Task<Order> GetOrderById(string orderId);

        void Pay(string orderId);
    }
}
