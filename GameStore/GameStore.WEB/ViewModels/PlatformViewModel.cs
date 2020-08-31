using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.ViewModels
{
    public class PlatformViewModel
    {
        public string PlatformId { get; set; }

        [Required]
        public string PlatformName { get; set; }
    }
}
