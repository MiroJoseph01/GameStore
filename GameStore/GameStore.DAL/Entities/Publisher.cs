﻿using System.Collections.Generic;
using GameStore.DAL.Entities.Interfaces;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.DAL.Entities
{
    [BsonIgnoreExtraElements]
    public class Publisher : ISoftDelete
    {
        public string PublisherId { get; set; }

        public string CompanyName { get; set; }

        public string ContactName { get; set; }

        public string ContactTitle { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Description { get; set; }

        public string HomePage { get; set; }

        public bool IsRemoved { get; set; }

        public IList<Game> Games { get; set; }
    }
}
