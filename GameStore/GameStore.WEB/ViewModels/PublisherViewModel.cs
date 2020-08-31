using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.ViewModels
{
    public class PublisherViewModel
    {
        public string PublisherId { get; set; }

        [Display(Name = "Publisher's Name")]
        public string CompanyName { get; set; }

        public string Description { get; set; }

        public string HomePage { get; set; }

        [Display(Name = "Publisher's Games")]
        public List<GameViewModel> PublisherGames { get; set; }
    }
}
