using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.ViewModels
{
    public class GameViewModel
    {
        public string GameId { get; set; }

        [Display(Name = "Game's key")]
        public string Key { get; set; }

        [Display(Name = "Game's name")]
        public string Name { get; set; }

        [Display(Name = "Game's description")]
        public string Description { get; set; }

        public string Discount { get; set; }

        public IList<CommentViewModel> Comments { get; set; }

        public IList<GenreViewModel> Genres { get; set; }

        public IList<PlatformViewModel> Platforms { get; set; }

        public bool Discontinued { get; set; }

        [Display(Name = "Units in stock")]
        public short UnitsInStock { get; set; }

        public decimal Price { get; set; }

        public string Publisher { get; set; }

        public int Views { get; set; }

        public string Date { get; set; }
    }
}
