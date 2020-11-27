using GameStroe.Mobile.Models;
using GameStroe.Mobile.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GameStroe.Mobile.Services
{
    public class OrderDataStore : IOrderDataStore
    {
        private HttpClient _client;

        public OrderDataStore()
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = delegate { return true; },
            };

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://d2c5a091c2af.ngrok.io")
            };
        }

        public void AddDetail(string gameKey, string userId)
        {
            _client.PostAsync($"api/order/{gameKey}/{userId}",null);
        }

        public void DeleteDetail(string id)
        {
            _client.DeleteAsync($"api/order/{id}");
        }

        public async Task<Order> GetOrderById(string orderId)
        {
            var json = await _client.GetStringAsync($"api/order/{orderId}/true");
            var item = JsonConvert.DeserializeObject<Order>(json);

            return item;
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerId(string customerId)
        {
            var json = await _client.GetStringAsync($"api/order/{customerId}/false");
            var items = JsonConvert.DeserializeObject<IEnumerable<Order>>(json);

            return items;
        }

        public void Pay(string orderId)
        {
            _client.PutAsync($"api/order/{orderId}", null);
        }
    }
}
