using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.ViewModels
{
    public class PaymentErrorModel
    {
        public string Name { get; set; }

        [Display(Name = "Quantity in order")]
        public int OrderQuantity { get; set; }

        [Display(Name = "Units in stock")]
        public int UnitsInStock { get; set; }
    }
}
