using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.ViewModels
{
    public class BasketViewModel
    {
        public string OrderId { get; set; }

        public string CustomerId { get; set; }

        [Display(Name = "Details")]
        public IList<OrderDetailViewModel> OrderDetails { get; set; }

        [Display(Name = "Status")]
        public string OrderStatus { get; set; }

        public decimal Total { get; set; }
    }
}
