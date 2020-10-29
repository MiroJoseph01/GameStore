using System;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.ViewModels
{
    public class FilterOrdersViewModel
    {
        [BindProperty]
        public string CustomerId { get; set; }

        [BindProperty]
        public DateTime MinDate { get; set; }

        [BindProperty]
        public DateTime MaxDate { get; set; }
    }
}
