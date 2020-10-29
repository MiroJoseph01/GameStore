using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.Interfaces.Repositories;
using GameStore.DAL.Repositories.Mongo.Util;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameStore.DAL.Repositories.Mongo
{
    public class GenreRepository : IMongoGenreRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Category> _collection;

        public GenreRepository(
            IOptions<MongoSettings> mongoSettings)
        {
            _database = new MongoClient(mongoSettings.Value.ConnectionString).GetDatabase(mongoSettings.Value.Name);
            _collection = _database.GetCollection<Category>(RepositoryHelper.GetDescription(typeof(Category)));
        }

        public IEnumerable<Genre> GetAll()
        {
            var collection = _collection.Find(_ => true).ToList();

            var genres = collection.Select(x => RepositoryHelper.ConvertCategoryToGenre(x)).ToList();

            return genres;
        }

        public Genre GetById(string id)
        {
            if (!IsPresent(id))
            {
                return null;
            }

            Genre genre = RepositoryHelper.ConvertCategoryToGenre(
                _collection.Find(x => x.CategoryID == id).FirstOrDefault());

            return genre;
        }

        public IEnumerable<string> GetGenreIdsByNames(IEnumerable<string> genreNames)
        {
            var genreIds = _collection
                .Find(x => genreNames.Contains(x.CategoryName))
                .ToList()
                .Select(x => x.CategoryID.ToString())
                .ToList();

            return genreIds;
        }

        public bool IsPresent(string id)
        {
            return _collection.CountDocuments(x => x.CategoryID.Equals(id)) > 0;
        }
    }
}
