using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using GameStore.DAL.Entities.MongoEntities;
using GameStore.Web.ViewModels;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.Web.Util.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
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

            CreateMap<PublisherCreateViewModel, BusinessModels.Publisher>();

            CreateMap<Supplier, DbModels.Publisher>()
                .ForMember(x => x.PublisherId, z => z.MapFrom(y => y.SupplierID));

            CreateMap<DbModels.Publisher, Supplier>()
                .ForMember(x => x.SupplierID, z => z.MapFrom(y => y.PublisherId));
        }
    }
}
