using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using GameStore.Web.ViewModels;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.Web.Util.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<BusinessModels.Comment, CommentViewModel>().ReverseMap();

            CreateMap<DbModels.Comment, BusinessModels.Comment>().ReverseMap();

            CreateMap<BusinessModels.Comment, DbModels.Comment>();

            CreateMap<CommentsViewModel, BusinessModels.Comment>();
        }
    }
}
