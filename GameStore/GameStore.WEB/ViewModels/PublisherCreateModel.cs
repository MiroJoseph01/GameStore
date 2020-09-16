using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.ViewModels
{
    public class PublisherCreateModel
    {
        public string PublisherId { get; set; }

        [Required]
        [Display(Name = "Publisher's Name")]
        public string CompanyName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Home Page")]
        [RegularExpression(
            @"(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|
                            www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.
                            |(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})", ErrorMessage = "Home page is a link")]
        public string HomePage { get; set; }

        [Display(Name = "Publisher's Games")]
        public List<GameViewModel> PublisherGames { get; set; }
    }
}
