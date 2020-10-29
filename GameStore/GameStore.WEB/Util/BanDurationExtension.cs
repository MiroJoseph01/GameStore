using GameStore.BLL.Models;

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
