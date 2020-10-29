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

        public string GameId { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public float Discount { get; set; }

        public bool Discontinued { get; set; }

        public short UnitsInStock { get; set; }

        public decimal Price { get; set; }

        public int Views { get; set; }

        public DateTime Date { get; set; }

        public string PublisherId { get; set; }

        public Publisher Publisher { get; set; }

        public IList<Comment> Comments { get; set; }

        public IList<Genre> GameGenres { get; set; }

        public IList<Platform> GamePlatforms { get; set; }
    }
}
