using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.ViewModels
{
    public class PublisherViewModel
    {
        public string PublisherId { get; set; }

        [Display(Name = "Publisher's Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Contact person")]
        public string ContactName { get; set; }

        [Display(Name = "Contact title")]
        public string ContactTitle { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        [Display(Name = "Postal code")]
        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Description { get; set; }

        [Display(Name = "Site")]
        public string HomePage { get; set; }

        [Display(Name = "Publisher's Games")]
        public List<GameViewModel> PublisherGames { get; set; }
    }
}
