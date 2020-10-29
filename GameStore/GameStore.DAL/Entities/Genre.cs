using GameStore.DAL.Entities.Interfaces;
using GameStore.DAL.Serializers;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.DAL.Entities
{
    [BsonIgnoreExtraElements]
    public class Genre : ISoftDelete
    {
        [BsonElement("CategoryID")]
        [BsonSerializer(typeof(CustomStringSerializer))]
        public string GenreId { get; set; }

        [BsonElement("CategoryName")]
        public string GenreName { get; set; }

        [BsonIgnore]
        public string ParentGenreId { get; set; }

        [BsonIgnore]
        public bool IsRemoved { get; set; }
    }
}
