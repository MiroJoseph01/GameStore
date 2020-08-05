using System;
using AutoMapper;
using GameStore.BLL.Models;
using GameStore.WEB.ViewModels;

namespace GameStore.WEB.Util.AutoMapperProfiles
{
    public class GenreProfile : Profile
    {
        public GenreProfile()
        {
            CreateMap<Genre, DAL.Entities.Genre>().ReverseMap();

            CreateMap<Genre, GenreViewModel>()
                .ForMember(x => x.GenreId, y => y.MapFrom(z => z.GenreId.ToString()))
                .ForMember(x => x.ParentGenreId, y => y.MapFrom(z => z.ParentGenreId.ToString()));

            CreateMap<GenreViewModel, Genre>()
                .ForMember(x => x.GenreId, y => y.MapFrom(z => string.IsNullOrEmpty(z.GenreId) ? Guid.Empty : Guid.Parse(z.GenreId)))
                .ForMember(x => x.ParentGenreId, y => y.MapFrom(z => string.IsNullOrEmpty(z.ParentGenreId) ? (Guid?)null : Guid.Parse(z.ParentGenreId)));
        }
    }
}
