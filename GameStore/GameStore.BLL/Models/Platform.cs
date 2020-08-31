using System;
using System.Collections.Generic;

namespace GameStore.BLL.Models
{
    public class Platform
    {
        public Guid PlatformId { get; set; }

        public string PlatformName { get; set; }

        public IList<Game> PlatformGames { get; set; }
    }
}
