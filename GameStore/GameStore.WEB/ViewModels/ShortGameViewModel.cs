using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.ViewModels
{
    public class ShortGameViewModel
    {
        public string GameId { get; set; }

        [Display(Name = "Game's key")]
        public string Key { get; set; }

        [Display(Name = "Game's name")]
        public string Name { get; set; }

        [Display(Name = "Game's description")]
        public string Description { get; set; }
    }
}
