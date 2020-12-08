using GameStroe.Mobile.Models;
using GameStroe.Mobile.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GameStroe.Mobile.Services
{
    public class GameDataStore : IDataStore<Game>
    {
        private HttpClient _client;
        private IEnumerable<Game> _items;

        public GameDataStore()
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = delegate { return true; },
            };

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://ea58bfae79be.ngrok.io")
            };

            _items = new List<Game>();
        }

        public async Task<Game> GetItemAsync(string id)
        {
            if (id != null)
            {
                var json = await _client.GetStringAsync($"api/game/{id}");
                return await Task.Run(() => JsonConvert.DeserializeObject<Game>(json));
            }

            return null;
        }

        public async Task<IEnumerable<Game>> GetItemsAsync(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                var json = await _client.GetStringAsync("api/game");
                _items = JsonConvert.DeserializeObject<IEnumerable<Game>>(json);
            }

            return _items;
        }

        public Task<bool> UpdateItemAsync(Game item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddItemAsync(Game item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

    }
}
