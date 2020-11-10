using System;
using System.Collections.Generic;

namespace GameStore.Mobile.Models
{
    public class GameCreateAndUpdateModel
    {
        public string GameId { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public float Discount { get; set; }

        public IList<string> Genres { get; set; }

        public IList<string> Platforms { get; set; }

        public bool Discontinued { get; set; }

        public short UnitsInStock { get; set; }

        public decimal Price { get; set; }

        public string PublisherName { get; set; }

        public DateTime? Date { get; set; }
    }
}
