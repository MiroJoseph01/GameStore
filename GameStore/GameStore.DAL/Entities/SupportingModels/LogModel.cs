using System;
using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.DAL.Entities.SupportingModels
{
    [Description("entity-state-log")]
    [BsonIgnoreExtraElements]
    public class LogModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string OperationType { get; set; }

        public DateTime Time { get; set; }

        public string EntityType { get; set; }

        public string OldObject { get; set; }

        public string NewObject { get; set; }
    }
}
