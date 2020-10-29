using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GameStore.DAL.Entities;
using GameStore.DAL.Entities.SupportingModels;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using GameStore.DAL.Pipeline;
using GameStore.DAL.Pipeline.Filters;
using GameStore.DAL.Pipeline.Util;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.Repositories.Sql
{
    public class GameRepository : Repository<Game>, IGameRepository
    {
        private readonly GameStoreContext _dbContext;

        private readonly IDictionary<OrderOption, OrderOptionModel> _orderOptions;
        private readonly IDictionary<TimePeriod, TimePeriodModel> _timePeriods;
        private readonly IPipeline _pipeline;
        private readonly IServiceProvider _serviceProvider;

        public GameRepository(
            GameStoreContext dbContext,
            IPipeline pipeline,
            IServiceProvider serviceProvider,
            IEntityStateLogger<Game> stateLogger)
            : base(dbContext, stateLogger)
        {
            _dbContext = dbContext;

            _pipeline = pipeline;
            _serviceProvider = serviceProvider;

            _orderOptions = new Dictionary<OrderOption, OrderOptionModel>
            {
                {
                    OrderOption.New,
                    new OrderOptionModel
                    {
                        Name = "New",
                        Text = "Newest",
                        Func = x => x.OrderByDescending(y => y.Date),
                    }
                },
                {
                    OrderOption.MostCommented,
                    new OrderOptionModel
                    {
                        Name = "MostCommented",
                        Text = "Most Commented",
                        Func = x => x.OrderByDescending(y => y.Comments.Count),
                    }
                },
                {
                    OrderOption.MostPopular,
                    new OrderOptionModel
                    {
                        Text = "Most Popular",
                        Name = "MostPopular",
                        Func = x => x.OrderByDescending(y => y.Date),
                    }
                },
                {
                    OrderOption.PriceAsc,
                    new OrderOptionModel
                    {
                        Text = "Cheap first",
                        Name = "PriceAsc",
                        Func = x => x.OrderBy(y => y.Price),
                    }
                },
                {
                    OrderOption.PriceDesc,
                    new OrderOptionModel
                    {
                        Text = "Expensive First",
                        Name = "PriceDesc",
                        Func = x => x.OrderByDescending(y => y.Price),
                    }
                },
            };

            _timePeriods = new Dictionary<TimePeriod, TimePeriodModel>
            {
                {
                    TimePeriod.LastWeek,
                    new TimePeriodModel
                    {
                        Text = "Last Week",
                        Name = "LastWeek",
                        Date = DateTime.Now.AddDays(-7),
                    }
                },
                {
                    TimePeriod.LastMonths,
                    new TimePeriodModel
                    {
                        Text = "Last Months",
                        Name = "LastMonths",
                        Date = DateTime.Now.AddMonths(-1),
                    }
                },
                {
                    TimePeriod.LastYear,
                    new TimePeriodModel
                    {
                        Text = "Last Year",
                        Name = "LastYear",
                        Date = DateTime.Now.AddYears(-1),
                    }
                },
                {
                    TimePeriod.LastTwoYears,
                    new TimePeriodModel
                    {
                        Text = "Last two years",
                        Name = "LastTwoYears",
                        Date = DateTime.Now.AddYears(-2),
                    }
                },
                {
                    TimePeriod.LastThreeYears,
                    new TimePeriodModel
                    {
                        Text = "Last three years",
                        Name = "LastThreeYears",
                        Date = DateTime.Now.AddYears(-3),
                    }
                },
                {
                    TimePeriod.AllTime,
                    new TimePeriodModel
                    {
                        Text = "All Time",
                        Name = "AllTime",
                        Date = null,
                    }
                },
            };
        }

        public override void Create(Game entity)
        {
            base.Create(entity);

            var views = _dbContext.Views.FirstOrDefault(x => x.GameId == entity.GameId);

            if (views == null)
            {
                _dbContext.Views
                    .Add(new View { Id = Guid.NewGuid().ToString(), GameId = entity.GameId, Views = 0 });
            }
        }

        public override void Delete(string id)
        {
            if (IsPresent(id))
            {
                base.Delete(id);
            }
            else
            {
                var game = GetById(id);

                if (game is null)
                {
                    _dbContext.DeletedGames.Add(new DeletedGame
                    {
                        Id = Guid.NewGuid().ToString(),
                        GameId = id,
                        IsRemoved = true,
                    });
                }
                else
                {
                    throw new InvalidOperationException("Game is already deleted");
                }
            }
        }

        public override IEnumerable<Game> GetAll()
        {
            var gameEntities = _dbContext.Games
                .Include(x => x.GenreGames)
                .Include(y => y.PlatformGames)
                .Include(z => z.Publisher);

            return gameEntities;
        }

        public override Game GetById(string id)
        {
            var gameEntities = _dbContext.Set<Game>()
                .Include(x => x.GenreGames)
                .Include(y => y.PlatformGames)
                .Include(z => z.Comments)
                .Include(v => v.Publisher);

            return gameEntities.FirstOrDefault(x => x.GameId == id);
        }

        public Game GetByKey(string key)
        {
            var gameEntities = _dbContext.Set<Game>()
                .Include(x => x.GenreGames)
                .Include(y => y.PlatformGames)
                .Include(p => p.Publisher)
                .Include(z => z.Comments);

            return gameEntities
                .FirstOrDefault(x => x.Key == key && !x.IsRemoved);
        }

        public IEnumerable<Genre> GetGameGenres(string id)
        {
            var genresId = _dbContext.GamesGenres.Where(x => x.GameId.Equals(id)).Select(y => y.GenreId).ToList();

            return _dbContext.Genres.Where(x => genresId.Contains(x.GenreId)).ToList();
        }

        public IEnumerable<Platform> GetGamePlatforms(string id)
        {
            var platformsId = _dbContext.GamesPlatforms.Where(x => x.GameId.Equals(id)).Select(y => y.PlatformId).ToList();

            return _dbContext.Platforms.Where(x => platformsId.Contains(x.PlatformId)).ToList();
        }

        public Publisher GetGamePublisher(string gameId)
        {
            Publisher publisher = null;

            if (IsPresent(gameId))
            {
                var publisherId = _dbContext.Games.Find(gameId).PublisherId;

                publisher = _dbContext.Publishers.FirstOrDefault(x => x.PublisherId == publisherId);
            }

            return publisher;
        }

        public IEnumerable<Game> GetGamesOfGenre(string genreId)
        {
            var gamesId = _dbContext.GamesGenres.Where(x => x.GenreId.Equals(genreId)).Select(y => y.GameId).ToList();

            return _dbContext.Games.Where(x => gamesId.Contains(x.GameId) && !x.IsRemoved).ToList();
        }

        public IEnumerable<Game> GetGamesOfPlatform(string platformId)
        {
            var gamesId = _dbContext.GamesPlatforms.Where(x => x.PlatformId.Equals(platformId))
                .Select(y => y.GameId)
                .ToList();

            return _dbContext.Games.Where(x => gamesId.Contains(x.GameId) && !x.IsRemoved).ToList();
        }

        public IEnumerable<Game> GetGamesOfPublisher(string publisherId)
        {
            return _dbContext.Games.Where(x => x.PublisherId.Equals(publisherId) && !x.IsRemoved).ToList();
        }

        public void AddView(string id)
        {
            View entityForUpdate = _dbContext.Views.FirstOrDefault(x => x.GameId == id);
            entityForUpdate.Views += 1;

            if (entityForUpdate != null)
            {
                _dbContext.Entry(entityForUpdate).State = EntityState.Detached;
            }

            _dbContext.Entry(entityForUpdate).State = EntityState.Modified;
        }

        public IEnumerable<Game> Filter(FilterModel model)
        {
            IQueryable<Game> query = _dbContext.Set<Game>();

            var expression = ConfigureFilters(model);

            if (expression != null)
            {
                query = query.Where(expression);
            }

            return query.ToList();
        }

        public bool Update(string id, Game entity, short ordered = 0)
        {
            base.Update(id, entity);

            return true;
        }

        public IDictionary<OrderOption, OrderOptionModel> GetOrderOptions()
        {
            return _orderOptions;
        }

        public IDictionary<TimePeriod, TimePeriodModel> GetTimePeriods()
        {
            return _timePeriods;
        }

        private Expression<Func<Game, bool>> ConfigureFilters(FilterModel query)
        {
            var date = GetTimePeriods()[query.DateFilter].Date;

            _pipeline
                .Register(new PlatformFilter(query.PlatformOptions, _serviceProvider))
                .Register(new PublisherFilter(query.PublisherOptions, _serviceProvider))
                .Register(new PriceFilter(query.From, query.To))
                .Register(new NameFilter(query.SearchByGameName))
                .Register(new DateFilter(date));

            _pipeline.Register(new GenreFilter(query.GenresOptions, _serviceProvider));

            return _pipeline.Process(x => true);
        }
    }
}
