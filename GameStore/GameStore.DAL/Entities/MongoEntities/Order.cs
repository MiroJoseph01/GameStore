using System.ComponentModel;
using GameStore.DAL.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.DAL.Entities.MongoEntities
{
    [Description("orders")]
    [BsonIgnoreExtraElements]
    public class Order
    {
        public ObjectId Id { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string OrderID { get; set; }

        public string CustomerID { get; set; }

        public int EmployeeID { get; set; }

        public string OrderDate { get; set; }

        public string RequiredDate { get; set; }

        public string ShippedDate { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string ShipVia { get; set; }

        public double Freight { get; set; }

        public string ShipName { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string ShipAddress { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string ShipCity { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string ShipRegion { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string ShipPostalCode { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string ShipCountry { get; set; }
    }
}
