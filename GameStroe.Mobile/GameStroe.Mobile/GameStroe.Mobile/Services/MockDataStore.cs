using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStroe.Mobile.Models;
using GameStroe.Mobile.Services.Interfaces;

namespace GameStroe.Mobile.Services
{
    public class MockDataStore : IDataStore<Game>
    {
        readonly List<Game> items;

        public MockDataStore()
        {
            var publisher1 = new Publisher
            {
                PublisherId = Guid.NewGuid().ToString(),
                CompanyName = "Valve",
                Description = "Steam",
                Address = "Adress",
                City = "New York",
                ContactName = "Gabe",
                ContactTitle = "Manager",
                Country = "USA",
                Fax = "1241-12412-124",
                Phone = "0000000-00",
                PostalCode = "123",
                HomePage = "https://store.steampowered.com/",
            };
            var publisher2 = new Publisher
            {
                PublisherId = Guid.NewGuid().ToString(),
                CompanyName = "CDProject RED",
                Description = "Polish company",
                Address = "Adress",
                City = "New York",
                ContactName = "Gabe",
                ContactTitle = "Manager",
                Country = "USA",
                Fax = "1241-12412-124",
                Phone = "0000000-00",
                PostalCode = "123",
                HomePage = "https://en.cdprojektred.com/",
            };

            var platform1 = new Platform
            {
                PlatformId = Guid.NewGuid().ToString(),
                PlatformName = "mobile"
            };
            var platform2 = new Platform
            {
                PlatformId = Guid.NewGuid().ToString(),
                PlatformName = "browser"
            };

            var action = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "Action",
            };
            var tps = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "TPS",
                ParentGenreId = action.GenreId,
            };
            var fps = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "FPS",
                ParentGenreId = action.GenreId,
            };

            items = new List<Game>()
            {
                new Game
                {
                    GameId = Guid.NewGuid().ToString(),
                    Name = "Mario",
                    Description = "OldSchool",
                    Key = "mario",
                    Price = 21,
                    UnitsInStock = 7,
                    Publisher = publisher1,
                    Date = DateTime.ParseExact("2018-09-12", "yyyy-MM-dd", null).ToShortDateString(),
                    Platforms = new List<Platform>{platform1, platform2},
                    Genres = new List<Genre>{action},
                },
                new Game
                {
                    GameId = Guid.NewGuid().ToString(),
                    Name = "GTA",
                    Description = "Classic",
                    Key = "gta",
                    Price = 1,
                    UnitsInStock = 10,
                    Publisher = publisher2,
                    Date = DateTime.ParseExact("2019-09-12", "yyyy-MM-dd", null).ToShortDateString(),
                    Platforms = new List<Platform>{platform2},
                    Genres = new List<Genre>{tps},
                },
                new Game
                {
                    GameId = Guid.NewGuid().ToString(),
                    Name = "Garden",
                    Description = "For children",
                    Key = "garden",
                    Price = 100,
                    UnitsInStock = 1100,
                    Publisher = null,
                    Date = DateTime.ParseExact("2020-09-12", "yyyy-MM-dd", null).ToShortDateString(),
                    Platforms = new List<Platform>{platform1},
                    Genres = new List<Genre>{fps, action},
                },
                new Game
                {
                    GameId = Guid.NewGuid().ToString(),
                    Name = "League of Legends",
                    Description = "For everyone",
                    Key = "lol",
                    Price = 100,
                    UnitsInStock = 1100,
                    Publisher = publisher1,
                    Date = DateTime.ParseExact("2017-09-12", "yyyy-MM-dd", null).ToShortDateString(),
                    Platforms = new List<Platform>{platform2},
                    Genres = new List<Genre>{fps, tps},
                },
            };
        }

        public async Task<bool> AddItemAsync(Game item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Game item)
        {
            var oldItem = items.Where((Game arg) => arg.GameId == item.GameId).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Game arg) => arg.GameId == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Game> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.GameId == id|| s.Key == id));
        }

        public async Task<IEnumerable<Game>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}