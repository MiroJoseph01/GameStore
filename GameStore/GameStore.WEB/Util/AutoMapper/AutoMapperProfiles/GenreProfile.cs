using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using GameStore.DAL.Entities.MongoEntities;
using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using API = GameStore.Web.ApiModels;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.Web.Util.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class GenreProfile : Profile
    {
        public GenreProfile()
        {
            CreateMap<BusinessModels.Genre, DbModels.Genre>().ReverseMap();

            CreateMap<BusinessModels.Genre, GenreViewModel>().ReverseMap();

            CreateMap<SelectListItem, BusinessModels.Genre>()
                .ForMember(x => x.GenreId, y => y.MapFrom(z => z.Value))
                .ForMember(x => x.GenreName, y => y.MapFrom(z => z.Text));

            CreateMap<Category, DbModels.Genre>()
                .ForMember(x => x.GenreId, y => y.MapFrom(z => z.CategoryID))
                .ForMember(x => x.GenreName, y => y.MapFrom(z => z.CategoryName))
                .ForMember(x => x.ParentGenreId, y => y.Ignore());

            CreateMap<string, BusinessModels.Genre>()
                .ForMember(x => x.GenreId, y => y.MapFrom(z => z));

            CreateMap<BusinessModels.Genre, API.GenreViewModel>().ReverseMap();
        }
    }
}
