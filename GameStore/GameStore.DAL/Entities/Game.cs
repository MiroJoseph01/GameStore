using System;
using System.Collections.Generic;
using GameStore.DAL.Entities.Interfaces;
using GameStore.DAL.Entities.SupportingModels;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.DAL.Entities
{
    [BsonIgnoreExtraElements]
    public class Game : ISoftDelete
    {
        public Game()
        {
            GenreGames = new List<GameGenre>();

            PlatformGames = new List<GamePlatform>();

            Comments = new List<Comment>();
        }

        public string GameId { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsRemoved { get; set; }

        public float Discount { get; set; }

        public bool Discontinued { get; set; }

        public short UnitsInStock { get; set; }

        public decimal Price { get; set; }

        public DateTime Date { get; set; }

        public bool FromMongo { get; set; }

        public string PublisherId { get; set; }

        public Publisher Publisher { get; set; }

        public IList<Comment> Comments { get; set; }

        public IList<GameGenre> GenreGames { get; set; }

        public IList<GamePlatform> PlatformGames { get; set; }
    }
}
