using System;
using System.Collections.Generic;
using GameStore.DAL.Interfaces;

namespace GameStore.DAL.Entities
{
    public class Game : ISoftDelete
    {
        public Guid GameId { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual IList<Comment> Comments { get; set; }

        public virtual IList<GameGenre> GenreGames { get; set; }

        public virtual IList<GamePlatform> PlatformGames { get; set; }

        public bool IsRemoved { get; set; }
    }
}
