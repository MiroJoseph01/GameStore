using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Entities.SupportingModels;
using GameStore.DAL.Repositories.Mongo.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoEntities = GameStore.DAL.Entities.MongoEntities;

namespace GameStore.DAL
{
    public class GameStoreContext : DbContext
    {
        private readonly IMongoDatabase _mongoDatabase;

        public GameStoreContext()
        {
        }

        public GameStoreContext(DbContextOptions<GameStoreContext> options, IOptions<MongoSettings> mongoOptions)
            : base(options)
        {
            _mongoDatabase = new MongoClient(mongoOptions.Value.ConnectionString).GetDatabase(mongoOptions.Value.Name);
        }

        public DbSet<Game> Games { get; set; }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Platform> Platforms { get; set; }

        public DbSet<GameGenre> GamesGenres { get; set; }

        public DbSet<GamePlatform> GamesPlatforms { get; set; }

        public DbSet<Publisher> Publishers { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderStatus> OrderStatus { get; set; }

        public DbSet<DeletedGame> DeletedGames { get; set; }

        public DbSet<View> Views { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Unique colums
            builder.Entity<Platform>()
                .HasIndex(i => i.PlatformName)
                .IsUnique();

            builder.Entity<Genre>()
                .HasIndex(i => i.GenreName)
                .IsUnique();

            builder.Entity<Game>()
                .HasIndex(i => i.Name)
                .IsUnique();

            // Create MtM relation for platforms
            builder.Entity<GamePlatform>()
                .HasKey(i => new { i.GameId, i.PlatformId });

            builder.Entity<GamePlatform>()
                .HasOne(x => x.Game)
                .WithMany(y => y.PlatformGames)
                .HasForeignKey(z => z.GameId);

            builder.Entity<GamePlatform>()
                .HasOne(x => x.Platform)
                .WithMany(y => y.PlatformGames)
                .HasForeignKey(z => z.PlatformId);

            // Create MtM relation for genres
            builder.Entity<GameGenre>()
                .HasKey(i => new { i.GameId, i.GenreId });

            builder.Entity<GameGenre>()
                .HasOne(x => x.Game)
                .WithMany(y => y.GenreGames)
                .HasForeignKey(z => z.GameId);

            // Create OtM relation
            builder.Entity<Comment>()
                .HasOne(x => x.CommentingGame)
                .WithMany(y => y.Comments);

            builder.Entity<Publisher>()
                .HasMany(x => x.Games)
                .WithOne(y => y.Publisher);

            builder.Entity<Order>()
                .HasOne(x => x.OrderStatus)
                .WithMany(y => y.Orders);

            builder.Entity<Order>()
                .HasOne(x => x.OrderStatus)
                .WithMany(y => y.Orders);

            builder.Entity<Game>()
                .HasIndex(u => u.Key)
                .IsUnique();

            builder.Entity<Game>()
                .Property(x => x.Price)
                .HasColumnType("money");

            builder.Entity<Publisher>()
                .Property(x => x.CompanyName)
                .HasColumnType("nvarchar(40)");

            builder.Entity<OrderDetail>()
                .Property(x => x.ProductId)
                .HasColumnType("nvarchar(40)");

            builder.Entity<OrderDetail>()
                .Property(x => x.Price)
                .HasColumnType("money");

            Seed(builder);
        }

        private void Seed(ModelBuilder builder)
        {
            #region Publisher Initialization

            var publisher1 = new Publisher
            {
                PublisherId = Guid.NewGuid().ToString(),
                CompanyName = "Valve",
                Description = "Steam",
                HomePage = "https://store.steampowered.com/",
            };
            var publisher2 = new Publisher
            {
                PublisherId = Guid.NewGuid().ToString(),
                CompanyName = "CDProject RED",
                Description = "Steam",
                HomePage = "https://en.cdprojektred.com/",
            };

            #endregion

            #region Genre Initialization

            var action = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "Action",
            };
            var tps = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "TPS",
                ParentGenreId = action.GenreId,
            };
            var fps = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "FPS",
                ParentGenreId = action.GenreId,
            };
            var misc = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "Misc.",
                ParentGenreId = action.GenreId,
            };
            var strategy = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "Strategy",
            };
            var rts = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "RTS",
                ParentGenreId = strategy.GenreId,
            };
            var tbs = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "TBS",
                ParentGenreId = strategy.GenreId,
            };
            var rpg = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "RPG",
            };
            var sports = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "Sports",
            };
            var races = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "Races",
            };
            var rally = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "Rally",
                ParentGenreId = races.GenreId,
            };
            var arcade = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "Arcade",
                ParentGenreId = races.GenreId,
            };
            var formula = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "Formula",
                ParentGenreId = races.GenreId,
            };
            var offRoad = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "Off road",
                ParentGenreId = races.GenreId,
            };

            var adventrue = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "Adventrue",
            };

            var puzzleAndSkill = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                GenreName = "PuzzleAndSkill",
            };

            #endregion

            #region Platform Initialization

            var platform1 = new Platform
            {
                PlatformId = Guid.NewGuid().ToString(),
                PlatformName = "mobile"
            };
            var platform2 = new Platform
            {
                PlatformId = Guid.NewGuid().ToString(),
                PlatformName = "browser"
            };
            var platform3 = new Platform
            {
                PlatformId = Guid.NewGuid().ToString(),
                PlatformName = "desktop"
            };
            var platform4 = new Platform
            {
                PlatformId = Guid.NewGuid().ToString(),
                PlatformName = "play station"
            };

            #endregion

            #region Game Initialization

            var gameViews = new List<View>();

            var game1 = new Game
            {
                GameId = Guid.NewGuid().ToString(),
                Name = "Mario",
                Description = "OldSchool",
                Key = "mario",
                Price = 21,
                UnitsInStock = 7,
                PublisherId = publisher1.PublisherId,
                Date = DateTime.ParseExact("2018-09-12", "yyyy-MM-dd", null),
            };
            gameViews.Add(new View { Id = Guid.NewGuid().ToString(), GameId = game1.GameId, Views = 10, });
            var game2 = new Game
            {
                GameId = Guid.NewGuid().ToString(),
                Name = "Dota2",
                Description = "MMORPG",
                Key = "dota2",
                Price = 9,
                UnitsInStock = 12,
                PublisherId = publisher1.PublisherId,
                Date = DateTime.ParseExact("2017-09-12", "yyyy-MM-dd", null),
            };
            gameViews.Add(new View { Id = Guid.NewGuid().ToString(), GameId = game2.GameId, Views = 20, });
            var game3 = new Game
            {
                GameId = Guid.NewGuid().ToString(),
                Name = "Cont Strike",
                Description = "Shooter",
                Key = "contr_strike",
                Price = 10,
                UnitsInStock = 2,
                PublisherId = publisher1.PublisherId,
                Date = DateTime.ParseExact("2019-09-12", "yyyy-MM-dd", null),
            };
            gameViews.Add(new View { Id = Guid.NewGuid().ToString(), GameId = game3.GameId, Views = 30, });
            var game4 = new Game
            {
                GameId = Guid.NewGuid().ToString(),
                Name = "My little pony",
                Description = "for children",
                Key = "my_litle_pony",
                Price = 15,
                UnitsInStock = 7,
                PublisherId = publisher2.PublisherId,
                Date = DateTime.ParseExact("2020-08-12", "yyyy-MM-dd", null),
            };
            gameViews.Add(new View { Id = Guid.NewGuid().ToString(), GameId = game4.GameId, Views = 40, });
            var game5 = new Game
            {
                GameId = Guid.NewGuid().ToString(),
                Name = "Garden 2",
                Description = "farming",
                Key = "garden_2",
                Price = 2,
                UnitsInStock = 3,
                PublisherId = publisher2.PublisherId,
                Date = DateTime.ParseExact("2018-09-05", "yyyy-MM-dd", null),
            };
            gameViews.Add(new View { Id = Guid.NewGuid().ToString(), GameId = game5.GameId, Views = 50, });

            #region GameGenre and GamePlatform Initialization

            var gameGenre = new List<GameGenre>
            {
                new GameGenre
                {
                    GameId = game1.GameId,
                    GenreId = arcade.GenreId,
                },
                new GameGenre
                {
                    GameId = game1.GameId,
                    GenreId = action.GenreId,
                },
                new GameGenre
                {
                    GameId = game2.GameId,
                    GenreId = action.GenreId,
                },
                new GameGenre
                {
                    GameId = game3.GameId,
                    GenreId = tps.GenreId,
                },
                new GameGenre
                {
                    GameId = game4.GameId,
                    GenreId = arcade.GenreId,
                },
                new GameGenre
                {
                    GameId = game5.GameId,
                    GenreId = action.GenreId,
                },
            };

            var gamePlatform = new List<GamePlatform>
            {
                new GamePlatform
                {
                    GameId = game1.GameId,
                    PlatformId = platform1.PlatformId,
                },
                new GamePlatform
                {
                    GameId = game1.GameId,
                    PlatformId = platform2.PlatformId,
                },
                new GamePlatform
                {
                    GameId = game2.GameId,
                    PlatformId = platform3.PlatformId,
                },
                new GamePlatform
                {
                    GameId = game3.GameId,
                    PlatformId = platform4.PlatformId,
                },
                new GamePlatform
                {
                    GameId = game4.GameId,
                    PlatformId = platform2.PlatformId,
                },
                new GamePlatform{
                    GameId = game5.GameId,
                    PlatformId = platform1.PlatformId,
                },
            };

            #endregion

            var games = new List<Game>();

            //for (var i = 1; i <= 20; i++)
            //{
            //    var g = new Game
            //    {
            //        GameId = Guid.NewGuid().ToString(),
            //        Name = "Game " + i,
            //        Description = "Game Description " + i,
            //        Key = "game" + i,
            //        Price = i,
            //        UnitsInStock = (short)i,
            //        PublisherId = publisher1.PublisherId,
            //        Date = DateTime.Now.AddMonths(-i),
            //    };

            //    games.Add(g);

            //    gameGenre.Add(new GameGenre
            //    {
            //        GameId = g.GameId,
            //        GenreId = arcade.GenreId,
            //    });

            //    gamePlatform.Add(new GamePlatform
            //    {
            //        GameId = g.GameId,
            //        PlatformId = platform1.PlatformId,
            //    });

            //    gameViews.Add(new View { Id = Guid.NewGuid().ToString(), GameId = g.GameId, Views = 100 + i, });
            //}

            #endregion

            #region OrderStatus Initialization

            var customerId = Guid.NewGuid().ToString();

            var orderStatus1 = new OrderStatus
            {
                OrderStatusId = Guid.NewGuid().ToString(),
                Status = "Open",
            };

            OrderStatus orderStatus2 = new OrderStatus
            {
                OrderStatusId = Guid.NewGuid().ToString(),
                Status = "Paid",
            };

            OrderStatus orderStatus3 = new OrderStatus
            {
                OrderStatusId = Guid.NewGuid().ToString(),
                Status = "NotPaid",
            };
            #endregion

            #region Order and OrderDetail Initialization

            var order1 = new Order
            {
                CustomerId = customerId,
                OrderId = Guid.NewGuid().ToString(),
                Status = orderStatus1.Status,
            };

            var order2 = new Order
            {
                CustomerId = customerId,
                OrderId = Guid.NewGuid().ToString(),
                Status = orderStatus2.Status,
            };

            var orderDetail1 = new OrderDetail
            {
                OrderDetailId = Guid.NewGuid().ToString(),
                Price = game1.Price * 2,
                Quantity = 2,
                Discount = 0,
                ProductId = game1.GameId.ToString(),
                OrderId = order1.OrderId,
            };

            var orderDetail2 = new OrderDetail
            {
                OrderDetailId = Guid.NewGuid().ToString(),
                Price = game2.Price * 2,
                Quantity = 2,
                Discount = 0,
                ProductId = game2.GameId.ToString(),
                OrderId = order1.OrderId,
            };

            var orderDetail3 = new OrderDetail
            {
                OrderDetailId = Guid.NewGuid().ToString(),
                Price = game3.Price * 1,
                Quantity = 1,
                Discount = 0,
                ProductId = game3.GameId.ToString(),
                OrderId = order2.OrderId,
            };

            #endregion

            #region View Initialization for Entities from Mongo

            var gamesCollection = _mongoDatabase
                .GetCollection<MongoEntities.Product>(
                    RepositoryHelper.GetDescription(typeof(MongoEntities.Product)));

            var gamesForViews = gamesCollection.Find(x => true).ToList();

            var views = gamesForViews
                .Select(x => new View { Id = Guid.NewGuid().ToString(), GameId = x.ProductId.ToString() });

            #endregion

            #region Apply Initialization

            builder.Entity<Publisher>().HasData(publisher1, publisher2);

            builder.Entity<Genre>()
                .HasData(
                strategy, rts, tbs, rpg, sports, races, rally, arcade,
                formula, offRoad, action, fps, tps, misc, adventrue, puzzleAndSkill);

            builder.Entity<Platform>()
                .HasData(platform1, platform2, platform3, platform4);

            builder.Entity<Game>()
                .HasData(game1, game2, game3, game4, game5);

            //too many data
            //builder.Entity<Game>()
            //    .HasData(games);

            builder.Entity<GameGenre>().HasData(gameGenre);

            builder.Entity<GamePlatform>().HasData(gamePlatform);

            builder.Entity<OrderStatus>()
                .HasData(orderStatus1, orderStatus2, orderStatus3);

            builder.Entity<OrderDetail>()
                .HasData(orderDetail1, orderDetail2, orderDetail3);

            builder.Entity<Order>().HasData(order1, order2);

            builder.Entity<View>().HasData(gameViews);

            builder.Entity<View>().HasData(views);

            #endregion
        }
    }
}