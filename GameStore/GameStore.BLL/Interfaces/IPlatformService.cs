using System;
using System.Collections.Generic;
using GameStore.BLL.Models;

namespace GameStore.BLL.Interfaces
{
    public interface IPlatformService
    {
        Platform AddPlatform(Platform platform);

        void DeletePlatform(Platform platform);

        Platform UpdatePlatform(Platform platform);

        Platform GetPlatformById(Guid platformId);

        IEnumerable<Platform> GetAllPlatforms();
    }
}
