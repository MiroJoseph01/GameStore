using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.Web.ViewModels
{
    public class GameCreateViewModel
    {
        public GameCreateViewModel()
        {
            Genres = new List<string>();
        }

        public string GameId { get; set; }

        [Display(Name = "Game's key")]
        [RegularExpression(@"\S*", ErrorMessage = "Use spaces in Game's key is forbidden")]
        public string Key { get; set; }

        [Required]
        [Display(Name = "Game's name")]
        [MaxLength(250)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Game's description")]
        [MinLength(10, ErrorMessage = "Description should be more then 10 letters")]
        public string Description { get; set; }

        public bool Discontinued { get; set; }

        [Required]
        [Range(0, 1, ErrorMessage = "Unvalid Discount")]
        public float Discount { get; set; }

        [Display(Name = "Units in stock")]
        [Range(0, 100000, ErrorMessage = "Incorrect number of units (not more then 100000 units)")]
        public short UnitsInStock { get; set; }

        [Range(0, 10000, ErrorMessage = "Incorrect price (should be between 0 and 10000)")]
        public decimal Price { get; set; }

        public IList<CommentViewModel> Comments { get; set; }

        [Required]
        [Display(Name = "Platforms")]
        public List<SelectListItem> PlatformOptions { get; set; }

        [Display(Name = "Genres")]
        public List<string> Genres { get; set; }

        public MultiSelectList GenreOptions { get; set; }

        public List<GenreViewModel> GameGenres { get; set; }

        public string PublisherName { get; set; }

        public List<SelectListItem> PublisherOptions { get; set; }
    }
}
