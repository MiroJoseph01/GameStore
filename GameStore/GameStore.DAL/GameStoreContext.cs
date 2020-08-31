using System;
using System.Collections.Generic;
using GameStore.DAL.Entities;
using GameStore.DAL.Entities.SupportingModels;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL
{
    public class GameStoreContext : DbContext
    {
        public GameStoreContext()
        {
        }

        public GameStoreContext(DbContextOptions<GameStoreContext> options)
            : base(options)
        {
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
            var publisher1 = new Publisher
            {
                PublisherId = Guid.NewGuid(),
                CompanyName = "Valve",
                Description = "Steam",
                HomePage = "https://store.steampowered.com/",
            };
            var publisher2 = new Publisher
            {
                PublisherId = Guid.NewGuid(),
                CompanyName = "CDProject RED",
                Description = "Steam",
                HomePage = "https://en.cdprojektred.com/",
            };

            var genreWithoutParent = new Genre
            {
                GenreId = Guid.NewGuid(),
                GenreName = "Action",
            };
            var genreWithParent = new Genre
            {
                GenreId = Guid.NewGuid(),
                GenreName = "PTS",
                ParentGenreId = genreWithoutParent.GenreId,
            };
            var genreArcade = new Genre
            {
                GenreId = Guid.NewGuid(),
                GenreName = "Arcade"
            };

            var platform1 = new Platform
            {
                PlatformId = Guid.NewGuid(),
                PlatformName = "mobile"
            };
            var platform2 = new Platform
            {
                PlatformId = Guid.NewGuid(),
                PlatformName = "browser"
            };
            var platform3 = new Platform
            {
                PlatformId = Guid.NewGuid(),
                PlatformName = "desktop"
            };
            var platform4 = new Platform
            {
                PlatformId = Guid.NewGuid(),
                PlatformName = "play station"
            };

            var game1 = new Game
            {
                GameId = Guid.NewGuid(),
                Name = "Mario",
                Description = "OldSchool",
                Key = "mario",
                Price = 21,
                UnitsInStock = 7,
                PublisherId = publisher1.PublisherId
            };
            var game2 = new Game
            {
                GameId = Guid.NewGuid(),
                Name = "Dota2",
                Description = "MMORPG",
                Key = "dota2",
                Price = 9,
                UnitsInStock = 12,
                PublisherId = publisher1.PublisherId
            };
            var game3 = new Game
            {
                GameId = Guid.NewGuid(),
                Name = "Cont Strike",
                Description = "Shooter",
                Key = "contr_strike",
                Price = 10,
                UnitsInStock = 2,
                PublisherId = publisher1.PublisherId
            };
            var game4 = new Game
            {
                GameId = Guid.NewGuid(),
                Name = "My little pony",
                Description = "for children",
                Key = "my_litle_pony",
                Price = 15,
                UnitsInStock = 7,
                PublisherId = publisher2.PublisherId
            };
            var game5 = new Game
            {
                GameId = Guid.NewGuid(),
                Name = "Garden 2",
                Description = "farming",
                Key = "garden_2",
                Price = 2,
                UnitsInStock = 3,
                PublisherId = publisher2.PublisherId,
            };

            var gameGenre = new List<GameGenre>
            {
                new GameGenre
                {
                    GameId = game1.GameId,
                    GenreId = genreArcade.GenreId,
                },
                new GameGenre
                {
                    GameId = game1.GameId,
                    GenreId = genreWithoutParent.GenreId,
                },
                new GameGenre
                {
                    GameId = game2.GameId,
                    GenreId = genreWithoutParent.GenreId,
                },
                new GameGenre
                {
                    GameId = game3.GameId,
                    GenreId = genreWithParent.GenreId,
                },
                new GameGenre
                {
                    GameId = game4.GameId,
                    GenreId = genreArcade.GenreId,
                },
                new GameGenre
                {
                    GameId = game5.GameId,
                    GenreId = genreWithoutParent.GenreId,
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

            Guid customerId = Guid.NewGuid();

            var orderStatus1 = new OrderStatus
            {
                OrderStatusId = Guid.NewGuid(),
                Status = "Open",
            };

            var orderStatus2 = new OrderStatus
            {
                OrderStatusId = Guid.NewGuid(),
                Status = "Submitted",
            };

            var order1 = new Order
            {
                CustomerId = customerId,
                OrderId = Guid.NewGuid(),
                Status = orderStatus1.Status,
            };

            var order2 = new Order
            {
                CustomerId = customerId,
                OrderId = Guid.NewGuid(),
                Status = orderStatus2.Status,
            };

            var orderDetail1 = new OrderDetail
            {
                OrderDetailId = Guid.NewGuid(),
                Price = game1.Price * 2,
                Quantity = 2,
                Discount = 0,
                ProductId = game1.GameId.ToString(),
                OrderId = order1.OrderId,
            };

            var orderDetail2 = new OrderDetail
            {
                OrderDetailId = Guid.NewGuid(),
                Price = game2.Price * 2,
                Quantity = 2,
                Discount = 0,
                ProductId = game2.GameId.ToString(),
                OrderId = order1.OrderId,
            };

            var orderDetail3 = new OrderDetail
            {
                OrderDetailId = Guid.NewGuid(),
                Price = game3.Price * 1,
                Quantity = 1,
                Discount = 0,
                ProductId = game3.GameId.ToString(),
                OrderId = order2.OrderId,
            };

            builder.Entity<Publisher>().HasData(publisher1, publisher2);

            builder.Entity<Genre>()
                .HasData(genreWithoutParent, genreWithParent, genreArcade);

            builder.Entity<Platform>()
                .HasData(platform1, platform2, platform3, platform4);

            builder.Entity<Game>()
                .HasData(game1, game2, game3, game4, game5);

            builder.Entity<GameGenre>().HasData(gameGenre);

            builder.Entity<GamePlatform>().HasData(gamePlatform);

            builder.Entity<OrderStatus>()
                .HasData(orderStatus1, orderStatus2);

            builder.Entity<OrderDetail>()
                .HasData(orderDetail1, orderDetail2, orderDetail3);

            builder.Entity<Order>().HasData(order1, order2);
        }
    }
}
