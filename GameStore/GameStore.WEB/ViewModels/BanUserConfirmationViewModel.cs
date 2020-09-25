using GameStore.BLL.Models;

namespace GameStore.Web.ViewModels
{
    public class BanUserConfirmationViewModel
    {
        public string UserId { get; set; }

        public BanDuration Duration { get; set; }
    }
}
