using System;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Models;

namespace GameStore.BLL.Services
{
    public class UserService : IUserService
    {
        public void BanUser(string userId, BanDuration banDuration)
        {
            Console.WriteLine($"User with Id {userId} is banned. Duration: {banDuration}");
        }
    }
}
