using GameStore.BLL.Models;
using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.Web.Utill
{
    public static class BanDurationExtension
    {
        public static int ToInt(this BanDuration banDuration)
        {
            return (int)banDuration;
        }
    }
}
