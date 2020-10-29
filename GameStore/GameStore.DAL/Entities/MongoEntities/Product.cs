using System.ComponentModel;
using GameStore.DAL.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.DAL.Entities.MongoEntities
{
    [Description("products")]
    public class Product
    {
        public ObjectId Id { get; set; }

        public string ProductId { get; set; }

        public int ProductID { get; set; }

        public string ProductName { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string SupplierID { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string CategoryID { get; set; }

        public string QuantityPerUnit { get; set; }

        public double UnitPrice { get; set; }

        public int UnitsInStock { get; set; }

        public int UnitsOnOrder { get; set; }

        public int ReorderLevel { get; set; }

        public int Discontinued { get; set; }
    }
}
