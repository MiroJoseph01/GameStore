using System;

namespace GameStore.DAL.Entities
{
    public class GamePlatform
    {
        public Guid GameId { get; set; }

        public virtual Game Game { get; set; }

        public Guid PlatformId { get; set; }

        public virtual Platform Platform { get; set; }
    }
}
