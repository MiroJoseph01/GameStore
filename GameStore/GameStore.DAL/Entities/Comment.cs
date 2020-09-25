using System;
using System.ComponentModel.DataAnnotations;
using GameStore.DAL.Entities.Interfaces;

namespace GameStore.DAL.Entities
{
    public class Comment : ISoftDelete
    {
        public Guid CommentId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Body { get; set; }

        public string Quote { get; set; }

        public Guid? ParentCommentId { get; set; }

        public Guid GameId { get; set; }

        public bool IsRemoved { get; set; }

        public Game CommentingGame { get; set; }
    }
}
