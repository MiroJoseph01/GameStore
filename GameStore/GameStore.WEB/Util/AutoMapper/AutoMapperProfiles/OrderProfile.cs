using System;
using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using GameStore.Web.ViewModels;
using API = GameStore.Web.ApiModels;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;
using MongoModels = GameStore.DAL.Entities.MongoEntities;

namespace GameStore.Web.Util.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<DbModels.Order, BusinessModels.Order>().ReverseMap();

            CreateMap<DbModels.OrderDetail, BusinessModels.OrderDetail>().ReverseMap();

            CreateMap<DbModels.OrderStatus, BusinessModels.OrderStatus>().ReverseMap();

            CreateMap<BusinessModels.Order, BasketViewModel>()
                .ForMember(
                    x => x.OrderStatus,
                    y => y.MapFrom(z => z.Status))
                .ForMember(
                    x => x.OrderId,
                    y => y.MapFrom(z => z.OrderId.ToString()))
                .ForMember(x => x.OrderDate, y => y.MapFrom(z => z.OrderDate.ToShortDateString()))
                .ForMember(x => x.RequiredDate, y => y.MapFrom(z => z.RequierdDate.ToShortDateString()))
                .ForMember(x => x.ShippedDate, y => y.MapFrom(z => z.ShippedDate.ToShortDateString()));

            CreateMap<BusinessModels.OrderDetail, OrderDetailViewModel>();

            CreateMap<MongoModels.Order, DbModels.Order>()
                .ForMember(x => x.CustomerId, y => y.MapFrom(z => z.CustomerID))
                .ForMember(x => x.Status, y => y.MapFrom(z => "Paid"))
                .ForMember(x => x.OrderId, y => y.MapFrom(z => z.OrderID.ToString()))
                .ForMember(
                    x => x.OrderDate,
                    y => y.MapFrom(z => z.OrderDate == "NULL" ? DateTime.MinValue : DateTime.Parse(z.OrderDate)))
                .ForMember(
                    x => x.RequiredDate,
                    y => y.MapFrom(z => z.RequiredDate == "NULL" ? DateTime.MinValue : DateTime.Parse(z.RequiredDate)))
                .ForMember(
                    x => x.ShippedDate,
                    y => y.MapFrom(z => z.ShippedDate == "NULL" ? DateTime.MinValue : DateTime.Parse(z.ShippedDate)));

            CreateMap<MongoModels.OrderDetail, DbModels.OrderDetail>()
                .ForMember(x => x.OrderId, y => y.MapFrom(z => z.OrderID.ToString()))
                .ForMember(x => x.ProductId, y => y.MapFrom(z => z.ProductID.ToString()))
                .ForMember(x => x.Price, y => y.MapFrom(z => Convert.ToDecimal(z.UnitPrice)))
                .ForMember(x => x.Discount, y => y.MapFrom(z => Convert.ToSingle(z.Discount)))
                .ForMember(x => x.Order, y => y.Ignore())
                .ForMember(x => x.OrderDetailId, y => y.Ignore())
                .ForMember(x => x.IsRemoved, y => y.MapFrom(z => false));

            CreateMap<MongoModels.Shipper, BusinessModels.Shipper>();

            CreateMap<BusinessModels.Order, API.OrderViewModel>()
                .ForMember(x => x.OrderDate, y => y.MapFrom(z => z.OrderDate.ToShortDateString()))
                .ForMember(x => x.OrderStatus, y => y.MapFrom(z => z.Status));
        }
    }
}
