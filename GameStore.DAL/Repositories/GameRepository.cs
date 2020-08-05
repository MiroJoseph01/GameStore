﻿using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.Repositories
{
    public class GameRepository : GenericRepository<Game>, IGameRepository
    {
        private readonly GameStoreContext _dbContext;

        public GameRepository(GameStoreContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override IEnumerable<Game> GetAll()
        {
            Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Game, IList<GamePlatform>> gameEntities = _dbContext.Games.Where(x => !x.IsRemoved)
                .Include(x => x.GenreGames)
                .Include(y => y.PlatformGames);

            return gameEntities;
        }

        public override Game GetById(Guid id)
        {
            var gameEntities = _dbContext.Set<Game>()
                .Where(r => r.IsRemoved == false)
                .Include(x => x.GenreGames)
                .Include(y => y.PlatformGames)
                .Include(z => z.Comments);

            return gameEntities.FirstOrDefault(x => x.GameId == id);
        }

        public Game GetByKey(string key)
        {
            var gameEntities = _dbContext.Set<Game>()
                .Include(x => x.GenreGames)
                .Include(y => y.PlatformGames)
                .Include(z => z.Comments);

            return gameEntities.FirstOrDefault(x => x.Key == key);
        }

        public IEnumerable<Genre> GetGameGenres(Guid id)
        {
            var genresId = _dbContext.GamesGenres
                .Where(x => x.GameId.Equals(id))
                .Select(y => y.GenreId)
                .ToList();

            return _dbContext.Genres.Where(x => genresId.Contains(x.GenreId)).ToList();
        }

        public IEnumerable<Platform> GetGamePlatforms(Guid id)
        {
            var platformsId = _dbContext.GamesPlatforms
                .Where(x => x.GameId.Equals(id))
                .Select(y => y.PlatformId)
                .ToList();

            return _dbContext.Platforms.Where(x => platformsId.Contains(x.PlatformId)).ToList();
        }

        public IEnumerable<Game> GetGamesOfGenre(Guid genreId)
        {
            var gamesId = _dbContext.GamesGenres
                .Where(x => x.GenreId.Equals(genreId))
                .Select(y => y.GameId)
                .ToList();

            return _dbContext.Games.Where(x => gamesId.Contains(x.GameId)).ToList();
        }

        public IEnumerable<Game> GetGamesOfPlatform(Guid platformId)
        {
            var gamesId = _dbContext.GamesPlatforms
                .Where(x => x.PlatformId.Equals(platformId))
                .Select(y => y.GameId)
                .ToList();

            return _dbContext.Games.Where(x => gamesId.Contains(x.GameId)).ToList();
        }
    }
}