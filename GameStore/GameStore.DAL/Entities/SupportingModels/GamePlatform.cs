using System;

namespace GameStore.DAL.Entities.SupportingModels
{
    public class GamePlatform
    {
        public Guid GameId { get; set; }

        public virtual Game Game { get; set; }

        public Guid PlatformId { get; set; }

        public Platform Platform { get; set; }
    }
}
