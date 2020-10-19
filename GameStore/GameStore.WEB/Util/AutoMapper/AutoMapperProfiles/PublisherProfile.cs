using System;
using AutoMapper;
using GameStore.Web.ViewModels;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.Web.Util.AutoMapperProfiles
{
    public class PublisherProfile : Profile
    {
        public PublisherProfile()
        {
            CreateMap<DbModels.Publisher, BusinessModels.Publisher>()
                .ReverseMap();

            CreateMap<BusinessModels.Publisher, PublisherViewModel>()
                .ForMember(
                    x => x.PublisherId,
                    y => y.MapFrom(z => z.PublisherId.ToString()));

            CreateMap<PublisherCreateViewModel, BusinessModels.Publisher>()
                .ForMember(
                    x => x.PublisherId,
                    y => y.MapFrom(z => string.IsNullOrEmpty(z.PublisherId) ?
                        Guid.Empty : Guid.Parse(z.PublisherId)));
        }
    }
}
