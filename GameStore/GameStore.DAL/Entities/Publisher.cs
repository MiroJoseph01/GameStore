using System;
using System.Collections.Generic;
using GameStore.DAL.Entities.Interfaces;

namespace GameStore.DAL.Entities
{
    public class Publisher : ISoftDelete
    {
        public Guid PublisherId { get; set; }

        public string CompanyName { get; set; }

        public string Description { get; set; }

        public string HomePage { get; set; }

        public bool IsRemoved { get; set; }

        public IList<Game> Games { get; set; }
    }
}
