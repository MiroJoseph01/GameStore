using System.Collections.Generic;
using GameStore.BLL.Models;

namespace GameStore.BLL.Interfaces.Services
{
    public interface IPlatformService
    {
        Platform AddPlatform(Platform platform);

        void DeletePlatform(Platform platform);

        Platform UpdatePlatform(Platform platform);

        Platform GetPlatformById(string platformId);

        IEnumerable<Platform> GetAllPlatforms();
    }
}
