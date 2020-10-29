using System;
using GameStore.DAL.Entities.SupportingModels;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Repositories.Mongo.Util;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace GameStore.DAL
{
    public class EntityStateLogger<TEntity> : IEntityStateLogger<TEntity>
        where TEntity : class
    {
        private readonly IMongoCollection<LogModel> _collection;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public EntityStateLogger(IOptions<MongoSettings> options)
        {
            var database = new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.Name);

            _collection = database.GetCollection<LogModel>(RepositoryHelper.GetDescription(typeof(LogModel)));

            _jsonSerializerSettings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            };
        }

        public void LogDelete(TEntity entity)
        {
            var logModel = new LogModel
            {
                EntityType = typeof(TEntity).ToString(),
                OperationType = "delete",
                Time = DateTime.Now,
                OldObject = JsonConvert.SerializeObject(entity, _jsonSerializerSettings),
            };

            _collection.InsertOne(logModel);
        }

        public void LogInsert(TEntity entity)
        {
            var logModel = new LogModel
            {
                EntityType = typeof(TEntity).ToString(),
                OperationType = "insert",
                Time = DateTime.Now,
                NewObject = JsonConvert.SerializeObject(entity, _jsonSerializerSettings),
            };

            _collection.InsertOne(logModel);
        }

        public void LogUpdate(TEntity oldEntity, TEntity newEntity)
        {
            var logModel = new LogModel
            {
                EntityType = typeof(TEntity).ToString(),
                OperationType = "update",
                Time = DateTime.Now,
                OldObject = JsonConvert.SerializeObject(oldEntity, _jsonSerializerSettings),
                NewObject = JsonConvert.SerializeObject(newEntity, _jsonSerializerSettings),
            };

            _collection.InsertOne(logModel);
        }
    }
}
