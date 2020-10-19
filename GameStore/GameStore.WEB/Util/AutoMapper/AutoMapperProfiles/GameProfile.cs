using System;
using AutoMapper;
using WebModels = GameStore.Web.ViewModels;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.Web.Util.AutoMapperProfiles
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            CreateMap<BusinessModels.Game, WebModels.GameViewModel>()
                .ForMember(
                    x => x.GameId,
                    y => y.MapFrom(z => z.GameId.ToString()))
                .ForMember(
                    x => x.Genres,
                    y => y.MapFrom(z => z.GameGenres))
                .ForMember(
                    x => x.Platforms,
                    y => y.MapFrom(z => z.GamePlatforms))
                .ForMember(
                    x => x.Publisher,
                    y => y.MapFrom(z => z.Publisher.CompanyName))
                .ForMember(
                    x => x.Discount,
                    y => y.MapFrom(z => (z.Discount * 100).ToString() + " %"))
                .ForMember(
                    x => x.Date,
                    y => y.MapFrom(z => z.Date.ToShortDateString()));

            CreateMap<WebModels.GameViewModel, BusinessModels.Game>()
                .ForMember(
                    x => x.GameId,
                    y => y.MapFrom(
                        z => string.IsNullOrEmpty(z.GameId) ?
                            Guid.Empty : Guid.Parse(z.GameId)))
                .ForMember(
                    x => x.GameGenres,
                    y => y.MapFrom(z => z.Genres))
                .ForMember(
                    x => x.GamePlatforms,
                    y => y.MapFrom(z => z.Platforms));

            CreateMap<WebModels.GameCreateViewModel, BusinessModels.Game>()
                .ForMember(
                    x => x.GameId,
                    y => y.MapFrom(
                        z => string.IsNullOrEmpty(z.GameId) ?
                            Guid.Empty : Guid.Parse(z.GameId)))
                .ForMember(
                    x => x.GameGenres,
                    y => y.MapFrom(z => z.GameGenres))
                .ForMember(
                    x => x.GamePlatforms,
                    y => y.MapFrom(z => z.PlatformOptions));

            CreateMap<DbModels.Game, BusinessModels.Game>()
                .ForMember(x => x.GameGenres, y => y.Ignore())
                .ForMember(x => x.GamePlatforms, y => y.Ignore())
                .ForMember(x => x.Comments, y => y.Ignore());

            CreateMap<BusinessModels.Game, DbModels.Game>()
                .ForMember(x => x.GenreGames, y => y.Ignore())
                .ForMember(x => x.PlatformGames, y => y.Ignore());

            CreateMap<BusinessModels.Game, WebModels.ShortGameViewModel>();

            CreateMap<WebModels.QueryModel, BusinessModels.QueryModel>()
                .ForMember(x => x.Take, y => y.MapFrom(z => z.PageSize))
                .ForMember(x => x.GenresOptions, y => y.MapFrom(z => z.GenresOptions))
                .ForMember(x => x.PlatformOptions, y => y.MapFrom(z => z.PlatformOptions))
                .ForMember(x => x.PublisherOptions, y => y.MapFrom(z => z.PublisherOptions))
                .ForMember(x => x.From, y => y.MapFrom(z => Convert.ToDecimal(z.From)))
                .ForMember(x => x.To, y => y.MapFrom(z => Convert.ToDecimal(z.To)));
        }
    }
}
