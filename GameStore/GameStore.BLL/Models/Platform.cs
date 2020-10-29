using System.Collections.Generic;

namespace GameStore.BLL.Models
{
    public class Platform
    {
        public string PlatformId { get; set; }

        public string PlatformName { get; set; }

        public IList<Game> PlatformGames { get; set; }
    }
}
