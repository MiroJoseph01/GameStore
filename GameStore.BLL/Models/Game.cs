using System;
using System.Collections.Generic;

namespace GameStore.BLL.Models
{
    public class Game
    {
        public Guid GameId { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IList<Comment> Comments { get; set; }

        public IEnumerable<Genre> GameGenres { get; set; }

        public IEnumerable<Platform> GamePlatforms { get; set; }
    }
}
