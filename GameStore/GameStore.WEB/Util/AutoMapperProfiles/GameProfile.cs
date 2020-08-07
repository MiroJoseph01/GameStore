using System;
using AutoMapper;
using GameStore.BLL.Models;
using GameStore.WEB.ViewModels;

namespace GameStore.WEB.Util.AutoMapperProfiles
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            CreateMap<Game, GameViewModel>()
                .ForMember(x => x.GameId, y => y.MapFrom(z => z.GameId.ToString()))
                .ForMember(x => x.Genres, y => y.MapFrom(z => z.GameGenres))
                .ForMember(x => x.Platforms, y => y.MapFrom(z => z.GamePlatforms));

            CreateMap<GameViewModel, Game>()
                .ForMember(x => x.GameId, y => y.MapFrom(z => string.IsNullOrEmpty(z.GameId) ? Guid.Empty : Guid.Parse(z.GameId)))
                .ForMember(x => x.GameGenres, y => y.MapFrom(z => z.Genres))
                .ForMember(x => x.GamePlatforms, y => y.MapFrom(z => z.Platforms));
        }
    }
}
