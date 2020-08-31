using System;
using AutoMapper;
using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.Web.Util.AutoMapperProfiles
{
    public class GenreProfile : Profile
    {
        public GenreProfile()
        {
            CreateMap<BusinessModels.Genre, DbModels.Genre>().ReverseMap();

            CreateMap<BusinessModels.Genre, GenreViewModel>()
                .ForMember(
                    x => x.GenreId,
                    y => y.MapFrom(z => z.GenreId.ToString()))
                .ForMember(
                    x => x.ParentGenreId,
                    y => y.MapFrom(z => z.ParentGenreId.ToString()));

            CreateMap<GenreViewModel, BusinessModels.Genre>()
                .ForMember(
                    x => x.GenreId,
                    y => y.MapFrom(z => string.IsNullOrEmpty(z.GenreId) ?
                    Guid.Empty : Guid.Parse(z.GenreId)))
                .ForMember(
                    x => x.ParentGenreId,
                    y => y.MapFrom(z => string.IsNullOrEmpty(z.ParentGenreId) ?
                    (Guid?)null : Guid.Parse(z.ParentGenreId)));

            CreateMap<SelectListItem, BusinessModels.Genre>()
                .ForMember(x => x.GenreId, y => y.MapFrom(z => z.Value))
                .ForMember(x => x.GenreName, y => y.MapFrom(z => z.Text));
        }
    }
}
