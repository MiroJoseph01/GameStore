using System;
using AutoMapper;
using GameStore.BLL.Models;
using GameStore.WEB.ViewModels;

namespace GameStore.WEB.Util.AutoMapperProfiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentViewModel>()
                .ForMember(x => x.CommentId, y => y.MapFrom(z => z.CommentId.ToString()))
                .ForMember(x => x.ParentCommentId, y => y.MapFrom(z => z.ParentCommentId.ToString()))
                .ForMember(x => x.GameId, y => y.MapFrom(z => z.GameId.ToString()));

            CreateMap<CommentViewModel, Comment>()
                .ForMember(x => x.CommentId, y => y.MapFrom(z => string.IsNullOrEmpty(z.CommentId) ? Guid.Empty : Guid.Parse(z.CommentId)))
                .ForMember(x => x.ParentCommentId, y => y.MapFrom(z => string.IsNullOrEmpty(z.ParentCommentId) ? (Guid?)null : Guid.Parse(z.ParentCommentId)))
                .ForMember(x => x.GameId, y => y.MapFrom(z => string.IsNullOrEmpty(z.GameId) ? Guid.Empty : Guid.Parse(z.GameId)));

            CreateMap<DAL.Entities.Comment, Comment>();
        }
    }
}
