using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.DAL.Entities;
using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.Entities.SupportingModels;
using GameStore.DAL.Interfaces.Repositories;
using GameStore.DAL.Pipeline;
using GameStore.DAL.Repositories.Mongo.Util;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameStore.DAL.Repositories.Mongo
{
    public class GameRepository : IMongoGameRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Product> _collection;
        private readonly IMapper _mapper;

        public GameRepository(
            IOptions<MongoSettings> mongoSettings,
            IMapper mapper)
        {
            _database = new MongoClient(mongoSettings.Value.ConnectionString).GetDatabase(mongoSettings.Value.Name);
            _collection = _database.GetCollection<Product>(RepositoryHelper.GetDescription(typeof(Product)));
            _mapper = mapper;
        }

        public IEnumerable<Game> GetAll()
        {
            var collection = _collection.Find(_ => true).ToList();

            var games = collection.Select(x => ConvertProductToGame(x)).ToList();

            return games;
        }

        public Game GetById(string id)
        {
            if (!IsPresent(id))
            {
                return null;
            }

            Game game = ConvertProductToGame(
                _collection.Find(x => x.ProductId == id).FirstOrDefault());

            return game;
        }

        public Game GetByKey(string key)
        {
            var keyTofind = key.Replace('_', ' ');
            Game game = ConvertProductToGame(
                _collection.Find(x => x.ProductName.ToLower() == keyTofind).FirstOrDefault());

            if (!(game is null))
            {
                game.Key = key;
            }

            return game;
        }

        public IEnumerable<Genre> GetGameGenres(string gameId)
        {
            var genresCollection = _database
                .GetCollection<Category>(RepositoryHelper.GetDescription(typeof(Category)));

            var genres = new List<Genre>();

            if (IsPresent(gameId))
            {
                var categoryId = _collection
                    .Find(x => x.ProductId.Equals(gameId))
                    .FirstOrDefault()
                    .CategoryID
                    .ToString();

                var genre = RepositoryHelper.ConvertCategoryToGenre(genresCollection
                    .Find(x => x.CategoryID == categoryId)
                    .FirstOrDefault());

                genres.Add(genre);
            }

            return genres;
        }

        public IEnumerable<Platform> GetGamePlatforms(string gameId)
        {
            return new List<Platform>();
        }

        public Publisher GetGamePublisher(string gameId)
        {
            var publisherCollection = _database
                .GetCollection<Supplier>(RepositoryHelper.GetDescription(typeof(Supplier)));

            var supplierId = _collection
                .Find(x => x.ProductId.Equals(gameId))
                .FirstOrDefault()
                .SupplierID;

            return _mapper.Map<Publisher>(
                publisherCollection.Find(x => x.SupplierID.Equals(supplierId)).FirstOrDefault());
        }

        public IEnumerable<Game> GetGamesOfGenre(string genreId)
        {
            int id = 0;

            var result = new List<Game>();

            var games = _collection.Find(x => x.CategoryID == genreId).ToList();

            if (games != null)
            {
                result = games.Select(z => ConvertProductToGame(z)).ToList();
            }

            return result;
        }

        public IEnumerable<Game> GetGamesOfPlatform(string platformId)
        {
            return new List<Game>();
        }

        public IEnumerable<Game> GetGamesOfPublisher(string publisherId)
        {
            var result = new List<Game>();

            var games = _collection.Find(x => x.SupplierID == publisherId).ToList();

            if (games != null)
            {
                result = games.Select(z => ConvertProductToGame(z)).ToList();
            }

            return result;
        }

        public bool IsPresent(string id)
        {
            return _collection.CountDocuments(x => x.ProductId.Equals(id)) > 0;
        }

        public void SetQuantity(string id, short quantity)
        {
            var update = Builders<Product>.Update
                        .Set(x => x.UnitsInStock, quantity);

            _collection.UpdateOne(x => x.ProductId == id, update);
        }

        public IEnumerable<Game> Filter(FilterModel filterModel)
        {
            var builder = Builders<Product>.Filter;

            var genreFilter = GetGenreFilter(filterModel.GenresOptions, builder);

            var publisherFilter = GetPublisherFilter(filterModel.PublisherOptions, builder);

            var priceFilter = GetPriceFilter(filterModel.From, filterModel.To, builder);

            var nameFilter = GetNameFilter(filterModel.SearchByGameName, builder);

            if (filterModel.PlatformOptions.Count > 0)
            {
                return new List<Game>();
            }

            return _collection
                .Find(
                    builder.And(
                        new List<FilterDefinition<Product>> { publisherFilter, genreFilter, priceFilter, nameFilter }))
                .ToList()
                .Select(x => ConvertProductToGame(x));
        }

        private FilterDefinition<Product> GetGenreFilter(List<string> genreList, FilterDefinitionBuilder<Product> builder)
        {
            FilterDefinition<Product> genreFilter = builder.Empty;

            if (genreList.Count != 0)
            {
                var genreCollection = _database
                    .GetCollection<Category>(RepositoryHelper.GetDescription(typeof(Category)));

                var genres = genreCollection
                    .Find(x => genreList.Contains(x.CategoryName)).ToList();

                if (genres != null)
                {
                    var genresIds = genres.Select(x => x.CategoryID);

                    genreFilter = builder.In(x => x.CategoryID, genresIds);
                }
            }

            return genreFilter;
        }

        private FilterDefinition<Product> GetPublisherFilter(List<string> publisherList, FilterDefinitionBuilder<Product> builder)
        {
            FilterDefinition<Product> publisherFilter = builder.Empty;

            if (publisherList.Count != 0)
            {
                var publisherCollection = _database
                    .GetCollection<Supplier>(RepositoryHelper.GetDescription(typeof(Supplier)));

                var publishers = publisherCollection
                    .Find(x => publisherList.Contains(x.CompanyName)).ToList();

                if (publishers != null)
                {
                    var publishersIds = publishers.Select(x => x.SupplierID);

                    publisherFilter = builder.In(x => x.CategoryID, publishersIds);
                }
            }

            return publisherFilter;
        }

        private FilterDefinition<Product> GetPriceFilter(decimal minPrice, decimal maxPrice, FilterDefinitionBuilder<Product> builder)
        {
            FilterDefinition<Product> priceFilter = builder.Empty;

            if (maxPrice != 0)
            {
                var priceFilterGte = builder.Gte(x => x.UnitPrice, (double)minPrice);
                var priceFilterLte = builder.Lte(x => x.UnitPrice, (double)maxPrice);

                priceFilter = builder.And(new List<FilterDefinition<Product>> { priceFilterGte, priceFilterLte });
            }

            return priceFilter;
        }

        private FilterDefinition<Product> GetNameFilter(string searchByGameName, FilterDefinitionBuilder<Product> builder)
        {
            FilterDefinition<Product> nameFilter = builder.Empty;

            if (searchByGameName != null)
            {
                nameFilter = builder
                    .Regex(x => x.ProductName, new BsonRegularExpression($"/{searchByGameName}/is"));
            }

            return nameFilter;
        }

        private Game ConvertProductToGame(Product product)
        {
            Game game = null;

            if (product != null)
            {
                var genres = GetGameGenres(product.ProductId)
                        .Select(x => new GameGenre { GameId = product.ProductId, GenreId = x.GenreId })
                        .ToList();

                var platforms = GetGamePlatforms(product.ProductId)
                            .Select(x => new GamePlatform { GameId = product.ProductId, PlatformId = x.PlatformId })
                            .ToList();

                var publisher = GetGamePublisher(product.ProductId);

                game = new Game
                {
                    GameId = product.ProductId,
                    Key = product.ProductId,
                    Name = product.ProductName,
                    Description = product.QuantityPerUnit,
                    Discontinued = Convert.ToBoolean(product.Discontinued),
                    UnitsInStock = (short)product.UnitsInStock,
                    Price = (decimal)product.UnitPrice,
                    PublisherId = product.SupplierID.ToString(),
                    GenreGames = genres,
                    PlatformGames = platforms,
                    Publisher = publisher,
                    FromMongo = true,
                };
            }

            return game;
        }
    }
}
