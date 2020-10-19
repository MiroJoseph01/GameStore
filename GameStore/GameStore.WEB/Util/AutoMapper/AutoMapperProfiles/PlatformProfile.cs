using AutoMapper;
using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.Web.Util.AutoMapperProfiles
{
    public class PlatformProfile : Profile
    {
        public PlatformProfile()
        {
            CreateMap<DbModels.Platform, BusinessModels.Platform>()
                .ForMember(x => x.PlatformGames, y => y.Ignore());

            CreateMap<BusinessModels.Platform, PlatformViewModel>().ReverseMap();

            CreateMap<SelectListItem, BusinessModels.Platform>()
                .ForMember(x => x.PlatformId, y => y.MapFrom(z => z.Value))
                .ForMember(x => x.PlatformName, y => y.MapFrom(z => z.Text));
        }
    }
}
