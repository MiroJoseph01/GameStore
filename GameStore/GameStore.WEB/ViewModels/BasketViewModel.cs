using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        [Display(Name = "Order Date")]
        public string OrderDate { get; set; }

        [Display(Name = "Required Date")]
        public string RequiredDate { get; set; }

        [Display(Name = "Shipped Date")]
        public string ShippedDate { get; set; }

        [Display(Name = "Avaliable shippers")]
        public List<SelectListItem> ShipOptions { get; set; }

        public string ShipVia { get; set; }

        public double Freight { get; set; }

        [Display(Name = "Name & Surname")]
        [Required(ErrorMessage = "Name and Surname are reqired")]
        [MinLength(3, ErrorMessage = "Name and Surname should be longer than 3 symbols")]
        [MaxLength(200)]
        public string ShipName { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Address is reqired")]
        [MinLength(5, ErrorMessage = "Adress should be longer than 5 symbols")]
        [MaxLength(200)]
        public string ShipAddress { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "City is reqired")]
        [MinLength(2, ErrorMessage = "City should be longer than 2 symbols")]
        [MaxLength(200)]
        public string ShipCity { get; set; }

        [Display(Name = "Region")]
        [Required(ErrorMessage = "Region is reqired")]
        [MinLength(2, ErrorMessage = "Region should be longer than 2 symbols")]
        [MaxLength(200)]
        public string ShipRegion { get; set; }

        [Display(Name = "Postal Code")]
        [Required(ErrorMessage = "Postal Code is reqired")]
        [MinLength(3, ErrorMessage = "Postal Code should be longer than 3 symbols")]
        [MaxLength(200)]
        public string ShipPostalCode { get; set; }

        [Display(Name = "Country")]
        [Required(ErrorMessage = "Country is reqired")]
        [MinLength(3, ErrorMessage = "Country name should be longer than 3 symbols")]
        [MaxLength(200)]
        public string ShipCountry { get; set; }
    }
}
