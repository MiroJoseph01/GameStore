using System;
using System.Collections.Generic;
using GameStore.DAL.Entities;
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

            builder.Entity<Game>()
                .Property(x => x.IsRemoved)
                .HasDefaultValue(false);

            builder.Entity<Platform>()
                .Property(x => x.IsRemoved)

                .HasDefaultValue(false);
            builder.Entity<Genre>()
                .Property(x => x.IsRemoved)
                .HasDefaultValue(false);

            builder.Entity<Comment>()
                .Property(x => x.IsRemoved)
                .HasDefaultValue(false);

            builder.Entity<Game>()
                .HasIndex(u => u.Key)
                .IsUnique();

            Seed(builder);
        }

        private void Seed(ModelBuilder builder)
        {
            Genre genreWithoutParent = new Genre { GenreId = Guid.NewGuid(), GenreName = "Action" };
            Genre genreWithParent = new Genre { GenreId = Guid.NewGuid(), GenreName = "PTS", ParentGenreId = genreWithoutParent.GenreId };
            Genre genreArcade = new Genre { GenreId = Guid.NewGuid(), GenreName = "Arcade" };

            Platform platform1 = new Platform { PlatformId = Guid.NewGuid(), PlatformName = "mobile" };
            Platform platform2 = new Platform { PlatformId = Guid.NewGuid(), PlatformName = "browser" };
            Platform platform3 = new Platform { PlatformId = Guid.NewGuid(), PlatformName = "desktop" };
            Platform platform4 = new Platform { PlatformId = Guid.NewGuid(), PlatformName = "play station" };

            Game game1 = new Game { GameId = Guid.NewGuid(), Name = "Mario", Description = "OldSchool", Key = "mario" };
            Game game2 = new Game { GameId = Guid.NewGuid(), Name = "Dota2", Description = "MMORPG", Key = "dota2" };
            Game game3 = new Game { GameId = Guid.NewGuid(), Name = "Cont Strike", Description = "Shooter", Key = "contr_strike" };
            Game game4 = new Game { GameId = Guid.NewGuid(), Name = "My little pony", Description = "for children", Key = "my_litle_pony" };
            Game game5 = new Game { GameId = Guid.NewGuid(), Name = "Garden 2", Description = "farming", Key = "garden_2" };

            List<GameGenre> gameGenre = new List<GameGenre>
            {
                new GameGenre { GameId = game1.GameId, GenreId = genreArcade.GenreId },
                new GameGenre { GameId = game1.GameId, GenreId = genreWithoutParent.GenreId },
                new GameGenre { GameId = game2.GameId, GenreId = genreWithoutParent.GenreId },
                new GameGenre { GameId = game3.GameId, GenreId = genreWithParent.GenreId },
                new GameGenre { GameId = game4.GameId, GenreId = genreArcade.GenreId },
                new GameGenre { GameId = game5.GameId, GenreId = genreWithoutParent.GenreId },
            };

            List<GamePlatform> gamePlatform = new List<GamePlatform>
            {
                new GamePlatform { GameId = game1.GameId, PlatformId = platform1.PlatformId},
                new GamePlatform { GameId = game1.GameId, PlatformId = platform2.PlatformId},
                new GamePlatform { GameId = game2.GameId, PlatformId = platform3.PlatformId},
                new GamePlatform { GameId = game3.GameId, PlatformId = platform4.PlatformId},
                new GamePlatform { GameId = game4.GameId, PlatformId = platform2.PlatformId},
                new GamePlatform { GameId = game5.GameId, PlatformId = platform1.PlatformId},
            };

            builder.Entity<Genre>().HasData(genreWithoutParent, genreWithParent, genreArcade);

            builder.Entity<Platform>().HasData(platform1, platform2, platform3, platform4);

            builder.Entity<Game>().HasData(game1, game2, game3, game4, game5);

            builder.Entity<GameGenre>().HasData(gameGenre);

            builder.Entity<GamePlatform>().HasData(gamePlatform);
        }
    }
}
