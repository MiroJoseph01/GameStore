using GameStore.BLL.Models;

namespace GameStore.BLL.Interfaces
{
    public interface IUserService
    {
        void BanUser(string userId, BanDuration banDuration);
    }
}
