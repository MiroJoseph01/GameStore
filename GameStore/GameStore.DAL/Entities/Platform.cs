using System;
using System.Collections.Generic;
using GameStore.DAL.Entities.Interfaces;
using GameStore.DAL.Entities.SupportingModels;

namespace GameStore.DAL.Entities
{
    public class Platform : ISoftDelete
    {
        public Guid PlatformId { get; set; }

        public string PlatformName { get; set; }

        public bool IsRemoved { get; set; }

        public IList<GamePlatform> PlatformGames { get; set; }
    }
}
