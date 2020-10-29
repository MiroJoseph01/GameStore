using System;
using System.Collections.Generic;

namespace GameStore.Web.ViewModels
{
    public class GeneralBasketViewModel
    {
        public GeneralBasketViewModel()
        {
            FilterModel = new FilterOrdersViewModel
            {
                MaxDate = DateTime.Now,
                MinDate = DateTime.ParseExact("1995-12-31", "yyyy-MM-dd", null),
            };
        }

        public FilterOrdersViewModel FilterModel { get; set; }

        public IEnumerable<BasketViewModel> Orders { get; set; }
    }
}
