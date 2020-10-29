using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.Repositories.Mongo.Util;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameStore.DAL
{
    public class MongoSeed
    {
        private readonly IMongoDatabase _mongoDatabase;

        public MongoSeed(IOptions<MongoSettings> options)
        {
            _mongoDatabase = new MongoClient(
                options.Value.ConnectionString).GetDatabase(options.Value.Name);
        }

        public void Initialize()
        {
            var gamesCollection = _mongoDatabase
                .GetCollection<Product>(
                    RepositoryHelper.GetDescription(typeof(Product)));

            var products = gamesCollection.Find(x => true).ToList();
            foreach (var p in products)
            {
                var update = Builders<Product>.Update
                    .Set(x => x.ProductId, p.ProductName.Replace(' ', '_').ToLower());

                gamesCollection.UpdateOne(x => x.ProductID == p.ProductID, update);
            }
        }
    }
}
