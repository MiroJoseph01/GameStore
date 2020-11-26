﻿using System.Collections.Generic;

namespace GameStroe.Mobile.Models
{
    public class Game
    {
        public string GameId { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Discount { get; set; }

        public IList<Comment> Comments { get; set; }

        public IList<Genre> Genres { get; set; }

        public IList<Platform> Platforms { get; set; }

        public bool Discontinued { get; set; }

        public short UnitsInStock { get; set; }

        public decimal Price { get; set; }

        public Publisher Publisher { get; set; }

        public int Views { get; set; }

        public string Date { get; set; }

        public List<string> Images { get; set; }
    }
}
