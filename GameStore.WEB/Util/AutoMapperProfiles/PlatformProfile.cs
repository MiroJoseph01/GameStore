using AutoMapper;
using GameStore.BLL.Models;
using GameStore.WEB.ViewModels;

namespace GameStore.WEB.Util.AutoMapperProfiles
{
    public class PlatformProfile : Profile
    {
        public PlatformProfile()
        {
            CreateMap<Platform, PlatformViewModel>().ReverseMap();
        }
    }
}
