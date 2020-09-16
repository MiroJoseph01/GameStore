using System;
using AutoMapper;
using GameStore.Web.ViewModels;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.Web.Util.AutoMapperProfiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<BusinessModels.Comment, CommentViewModel>()
                .ForMember(
                    x => x.CommentId,
                    y => y.MapFrom(z => z.CommentId.ToString()))
                .ForMember(
                    x => x.ParentCommentId,
                    y => y.MapFrom(z => z.ParentCommentId.ToString()))
                .ForMember(
                    x => x.GameId,
                    y => y.MapFrom(z => z.GameId.ToString()));

            CreateMap<CommentViewModel, BusinessModels.Comment>()
                .ForMember(
                    x => x.CommentId,
                    y => y.MapFrom(
                        z => string.IsNullOrEmpty(z.CommentId) ?
                            Guid.Empty : Guid.Parse(z.CommentId)))
                .ForMember(
                    x => x.ParentCommentId,
                    y => y.MapFrom(
                        z => string.IsNullOrEmpty(z.ParentCommentId) ?
                            (Guid?)null : Guid.Parse(z.ParentCommentId)))
                .ForMember(
                    x => x.GameId,
                    y => y.MapFrom(
                        z => string.IsNullOrEmpty(z.GameId) ?
                        Guid.Empty : Guid.Parse(z.GameId)));

            CreateMap<DbModels.Comment, BusinessModels.Comment>().ReverseMap();

            CreateMap<CommentsViewModel, BusinessModels.Comment>();
        }
    }
}
