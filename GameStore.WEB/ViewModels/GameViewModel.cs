using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameStore.WEB.ViewModels
{
    public class GameViewModel
    {
        public string GameId { get; set; }

        public string Key { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public IList<CommentViewModel> Comments { get; set; }

        public IList<GenreViewModel> Genres { get; set; }

        [Required]
        public IList<PlatformViewModel> Platforms { get; set; }
    }
}
