using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.ViewModels
{
    public class CreditCardViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Incorrect Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Card Number")]
        [StringLength(19, MinimumLength = 16, ErrorMessage = "The card number must be 16 digits")]
        [RegularExpression(@"^[\d\s]+$", ErrorMessage = "Incorrect card number")]
        public string CardNumber { get; set; }

        [Required]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Incorrect Date")]
        [RegularExpression(@"[0-9]{2}\/[0-9]{2}", ErrorMessage = "Incorrect Date")]
        public string Date { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 3, ErrorMessage = "Incorrect CVV")]
        public string CVV { get; set; }
    }
}
