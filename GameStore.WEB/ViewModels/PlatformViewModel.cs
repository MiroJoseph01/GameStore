using System.ComponentModel.DataAnnotations;

namespace GameStore.WEB.ViewModels
{
    public class PlatformViewModel
    {
        public string PlatformId { get; set; }

        [Required]
        public string PlatformName { get; set; }
    }
}
