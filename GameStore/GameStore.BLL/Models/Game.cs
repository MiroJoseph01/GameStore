using System;
using System.Collections.Generic;

namespace GameStore.BLL.Models
{
    public class Game
    {
        public Game()
        {
            GameGenres = new List<Genre>();
            GamePlatforms = new List<Platform>();
        }

        public Guid GameId { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public float Discount { get; set; }

        public bool Discontinued { get; set; }

        public short UnitsInStock { get; set; }

        public decimal Price { get; set; }

        public Guid? PublisherId { get; set; }

        public Publisher Publisher { get; set; }

        public IList<Comment> Comments { get; set; }

        public IList<Genre> GameGenres { get; set; }

        public IList<Platform> GamePlatforms { get; set; }
    }
}
