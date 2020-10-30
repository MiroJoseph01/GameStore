using System.ComponentModel;
using GameStore.DAL.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.DAL.Entities.MongoEntities
{
    [Description("categories")]
    [BsonIgnoreExtraElements]
    public class Category
    {
        public ObjectId Id { get; set; }

        [BsonSerializer(typeof(CustomStringSerializer))]
        public string CategoryID { get; set; }

        public string CategoryName { get; set; }

        public string Description { get; set; }

        public string Picture { get; set; }
    }
}
