using AutoMapper;
using GameStore.Web.ViewModels;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.Web.Util.AutoMapperProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<DbModels.Order, BusinessModels.Order>().ReverseMap();

            CreateMap<DbModels.OrderDetail, BusinessModels.OrderDetail>().ReverseMap();

            CreateMap<DbModels.OrderStatus, BusinessModels.OrderStatus>().ReverseMap();

            CreateMap<BusinessModels.Order, BasketViewModel>()
                .ForMember(
                    x => x.CustomerId,
                    y => y.MapFrom(z => z.CustomerId.ToString()))
                .ForMember(
                    x => x.OrderStatus,
                    y => y.MapFrom(z => z.Status))
                .ForMember(
                    x => x.OrderId,
                    y => y.MapFrom(z => z.OrderId.ToString()));

            CreateMap<BusinessModels.OrderDetail, OrderDetailViewModel>()
                .ForMember(
                    x => x.OrderDetailId,
                    y => y.MapFrom(z => z.OrderDetailId.ToString()));
        }
    }
}
