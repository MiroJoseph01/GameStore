using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Entities.SupportingModels;
using GameStore.DAL.Interfaces.Repositories;
using GameStore.DAL.Pipeline;
using GameStore.DAL.Pipeline.Util;
using MongoDB.Driver;

namespace GameStore.DAL.Repositories.Facade
{
    public class GameRepositoryFacade : IGameRepositoryFacade
    {
        private readonly IGameRepository _firstSourceGameRepository;
        private readonly IGenreRepository _firstSourceGenreRepository;
        private readonly IPublisherRepository _firstSourcePublisherRepository;
        private readonly IMongoGameRepository _secondSourceGameRepository;
        private readonly IMongoGenreRepository _secondSourceGenreRepository;
        private readonly IMongoPublisherRepository _secondSourcePublisherRepository;
        private readonly IViewRepository _viewRepository;

        public GameRepositoryFacade(
            IGameRepository sqlGameRepository,
            IGenreRepository sqlGenreRepository,
            IPublisherRepository sqlPublisherRepository,
            IMongoGameRepository mongoGameRepository,
            IMongoGenreRepository mongoGenreRepository,
            IMongoPublisherRepository mongoPublisherRepository,
            IViewRepository viewRepository)
        {
            _firstSourceGameRepository = sqlGameRepository;
            _firstSourceGenreRepository = sqlGenreRepository;
            _firstSourcePublisherRepository = sqlPublisherRepository;
            _secondSourceGameRepository = mongoGameRepository;
            _secondSourceGenreRepository = mongoGenreRepository;
            _secondSourcePublisherRepository = mongoPublisherRepository;
            _viewRepository = viewRepository;
        }

        public void AddView(string key)
        {
            var firstSourceGame = _firstSourceGameRepository.GetByKey(key);

            if (firstSourceGame != null)
            {
                _firstSourceGameRepository.AddView(firstSourceGame.GameId);
            }
            else
            {
                var secondSourceGame = _secondSourceGameRepository.GetByKey(key);

                _firstSourceGameRepository.AddView(secondSourceGame.GameId);
            }
        }

        public void Create(Game entity)
        {
            UpdatePublisher(entity.PublisherId);
            UpdateGenres(entity.GenreGames);

            _firstSourceGameRepository.Create(entity);
        }

        public void Delete(string id)
        {
            if (_firstSourceGameRepository.IsPresent(id))
            {
                _firstSourceGameRepository.Delete(id);
            }
            else
            {
                _firstSourceGameRepository.Create(new Game
                {
                    GameId = id,
                    IsRemoved = true,
                    FromMongo = true,
                });
            }
        }

        public IEnumerable<Game> Filter(FilterModel filterModel)
        {
            var sqlResult = _firstSourceGameRepository.Filter(filterModel).ToList();

            var sqlGameIds = sqlResult.Select(x => x.GameId).ToList();

            var mongoResult = _secondSourceGameRepository
                .Filter(filterModel)
                .Where(x => !sqlGameIds.Contains(x.GameId))
                .ToList();

            var result = new List<Game>();

            result.AddRange(sqlResult);
            result.AddRange(mongoResult);

            result = result.Where(x => !x.IsRemoved).ToList();

            var order = GetOrderOptions()[filterModel.Filter].Func;

            if (!(order is null))
            {
                if (filterModel.Filter == OrderOption.MostPopular)
                {
                    var views = _viewRepository.GetAll().ToList();
                    var joinResult = from game in result
                                     join view in views
                                     on game.GameId equals view.GameId into joined
                                     from j in joined.DefaultIfEmpty()
                                     orderby j.Views descending
                                     select game;

                    result = joinResult.ToList();
                }
                else
                {
                    result = order(result.AsQueryable()).ToList();
                }
            }

            if (filterModel.Take != 0)
            {
                result = result.Skip(filterModel.Skip).Take(filterModel.Take).ToList();
            }

            return result;
        }

        public IEnumerable<Game> GetAll()
        {
            //edit to make project working faster
            var sqlResult = _firstSourceGameRepository.GetAll().ToList();
            //var mongoResult = _secondSourceGameRepository.GetAll().ToList();

            //mongoResult = DeleteMongoGameCopies(sqlResult, mongoResult).ToList();

            var result = new List<Game>();

            result.AddRange(sqlResult);
            ///result.AddRange(mongoResult);

            result = result.Where(x => !x.IsRemoved).ToList();

            return result;
        }

        public Game GetById(string id)
        {
            var game = _firstSourceGameRepository.GetById(id);

            if (game != null && game.IsRemoved)
            {
                return null;
            }

            if (game is null)
            {
                game = _secondSourceGameRepository.GetById(id);
            }

            return game;
        }

        public Game GetByKey(string key)
        {
            var game = _firstSourceGameRepository.GetByKey(key);

            if (game != null && game.IsRemoved)
            {
                return null;
            }

            if (game is null)
            {
                game = _secondSourceGameRepository.GetByKey(key);
            }

            return game;
        }

        public IEnumerable<Genre> GetGameGenres(string gameId)
        {
            var sqlGenres = _firstSourceGameRepository.GetGameGenres(gameId);

            var sqlGenreIds = sqlGenres.Select(x => x.GenreId).ToList();

            var mongoGenres = _secondSourceGameRepository
                .GetGameGenres(gameId)
                .Where(x => !sqlGenreIds.Contains(x.GenreId))
                .ToList();

            var result = new List<Genre>();

            result.AddRange(sqlGenres);
            result.AddRange(mongoGenres);

            return result;
        }

        public IEnumerable<Platform> GetGamePlatforms(string gameId)
        {
            var sqlPlatforms = _firstSourceGameRepository.GetGamePlatforms(gameId);

            var mongoPlatforms = _secondSourceGameRepository.GetGamePlatforms(gameId);

            var result = new List<Platform>();

            result.AddRange(sqlPlatforms);
            result.AddRange(mongoPlatforms);

            return result;
        }

        public Publisher GetGamePublisher(string gameId)
        {
            var publisher = _firstSourceGameRepository.GetGamePublisher(gameId);

            if (publisher is null)
            {
                publisher = _secondSourceGameRepository.GetGamePublisher(gameId);
            }

            return publisher;
        }

        public IEnumerable<Game> GetGamesOfGenre(string genreId)
        {
            var sqlGames = _firstSourceGameRepository.GetGamesOfGenre(genreId);
            var mongoGames = _secondSourceGameRepository.GetGamesOfGenre(genreId);

            mongoGames = DeleteMongoGameCopies(sqlGames, mongoGames);

            var result = new List<Game>();

            result.AddRange(sqlGames);
            result.AddRange(mongoGames);

            return result;
        }

        public IEnumerable<Game> GetGamesOfPlatform(string platformId)
        {
            var sqlGames = _firstSourceGameRepository.GetGamesOfPlatform(platformId);
            var mongoGames = _secondSourceGameRepository.GetGamesOfPlatform(platformId);

            mongoGames = DeleteMongoGameCopies(sqlGames, mongoGames);

            var result = new List<Game>();

            result.AddRange(sqlGames);
            result.AddRange(mongoGames);

            return result;
        }

        public IEnumerable<Game> GetGamesOfPublisher(string publisherId)
        {
            var sqlGames = _firstSourceGameRepository.GetGamesOfPublisher(publisherId);
            var mongoGames = _secondSourceGameRepository.GetGamesOfPublisher(publisherId);

            mongoGames = DeleteMongoGameCopies(sqlGames, mongoGames);

            var result = new List<Game>();

            result.AddRange(sqlGames);
            result.AddRange(mongoGames);

            return result;
        }

        public bool IsPresent(string id)
        {
            return _firstSourceGameRepository.IsPresent(id) || _secondSourceGameRepository.IsPresent(id);
        }

        public bool Update(string id, Game entity, short ordered = 0)
        {
            var success = true;

            var game = _firstSourceGameRepository.GetById(id);

            if (_firstSourceGameRepository.IsPresent(id))
            {
                UpdatePublisher(entity.PublisherId);
                UpdateGenres(entity.GenreGames);

                if (game.FromMongo)
                {
                    entity.FromMongo = true;
                    success = UpdateQuantity(entity, ordered);
                }

                _firstSourceGameRepository.Update(id, entity);
            }
            else
            {
                entity.GameId = id;
                entity.FromMongo = true;

                success = UpdateQuantity(entity, ordered);

                // In this case we do not need publisher object, only its id, because in other way it tries to create a new Publisher in our database but we create publisher if it is needed futher.
                entity.Publisher = null;
                Create(entity);
            }

            return success;
        }

        public void Update(string id, Game entity)
        {
            Update(id, entity, 0);
        }

        public IDictionary<OrderOption, OrderOptionModel> GetOrderOptions()
        {
            return _firstSourceGameRepository.GetOrderOptions();
        }

        public IDictionary<TimePeriod, TimePeriodModel> GetTimePeriods()
        {
            return _firstSourceGameRepository.GetTimePeriods();
        }

        private IEnumerable<Game> DeleteMongoGameCopies(IEnumerable<Game> sqlGames, IEnumerable<Game> mongoGames)
        {
            if (sqlGames != null || sqlGames.Count() == 0)
            {
                var ids = sqlGames.Select(x => x.GameId);
                mongoGames = mongoGames.Where(x => !ids.Contains(x.GameId));
            }

            return mongoGames;
        }

        private void UpdatePublisher(string publisherId)
        {
            if (publisherId != null && !_firstSourcePublisherRepository.IsPresent(publisherId))
            {
                var publisherToCopy = _secondSourcePublisherRepository.GetById(publisherId);

                if (publisherToCopy != null)
                {
                    _firstSourcePublisherRepository.Create(publisherToCopy);
                }
            }
        }

        private void UpdateGenres(IList<GameGenre> genreList)
        {
            foreach (var g in genreList)
            {
                if (!_firstSourceGenreRepository.IsPresent(g.GenreId))
                {
                    var genreToCopy = _secondSourceGenreRepository.GetById(g.GenreId);

                    if (genreToCopy != null)
                    {
                        _firstSourceGenreRepository.Create(genreToCopy);
                    }
                }
            }
        }

        private bool UpdateQuantity(Game entity, short ordered)
        {
            var gameFromMongo = _secondSourceGameRepository.GetById(entity.GameId);

            short units = (short)(gameFromMongo.UnitsInStock + ordered);

            if (units < 0)
            {
                entity.UnitsInStock = gameFromMongo.UnitsInStock;
                return false;
            }

            entity.UnitsInStock = units;
            _secondSourceGameRepository.SetQuantity(entity.GameId, units);

            return true;
        }
    }
}
