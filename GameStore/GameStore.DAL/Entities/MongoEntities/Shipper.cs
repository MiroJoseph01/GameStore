using System.ComponentModel;
using GameStore.DAL.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.DAL.Entities.MongoEntities
{
    [Description("shippers")]
    public class Shipper
    {
        public ObjectId Id { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string ShipperID { get; set; }

        public string CompanyName { get; set; }

        public string Phone { get; set; }
    }
}
