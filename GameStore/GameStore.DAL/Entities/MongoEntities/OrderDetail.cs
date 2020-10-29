using System.ComponentModel;
using GameStore.DAL.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.DAL.Entities.MongoEntities
{
    [Description("order-details")]
    public class OrderDetail
    {
        public ObjectId Id { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string OrderID { get; set; }

        public int ProductID { get; set; }

        public double UnitPrice { get; set; }

        public int Quantity { get; set; }

        public double Discount { get; set; }
    }
}
