using System.ComponentModel;
using GameStore.DAL.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.DAL.Entities.MongoEntities
{
    [Description("suppliers")]
    [BsonIgnoreExtraElements]
    public class Supplier
    {
        public ObjectId Id { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string SupplierID { get; set; }

        public string CompanyName { get; set; }

        public string ContactName { get; set; }

        public string ContactTitle { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string Address { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string City { get; set; }

        public string Region { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string PostalCode { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string Country { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string Phone { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string Fax { get; set; }

        public string HomePage { get; set; }
    }
}
