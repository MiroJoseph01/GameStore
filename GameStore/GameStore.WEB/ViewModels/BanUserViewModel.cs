using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.Web.ViewModels
{
    public class BanUserViewModel
    {
        public BanUserViewModel()
        {
            Durations = new List<SelectListItem>();
        }

        public string UserId { get; set; }

        [Display(Name = "Duration")]
        public List<SelectListItem> Durations { get; set; }
    }
}
