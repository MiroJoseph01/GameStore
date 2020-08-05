using System;
using System.Collections.Generic;
using GameStore.DAL.Interfaces;

namespace GameStore.DAL.Entities
{
    public class Platform : ISoftDelete
    {
        public Guid PlatformId { get; set; }

        public string PlatformName { get; set; }

        public virtual IList<GamePlatform> PlatformGames { get; set; }

        public bool IsRemoved { get; set; }
    }
}
