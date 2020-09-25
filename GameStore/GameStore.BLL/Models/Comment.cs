using System;

namespace GameStore.BLL.Models
{
    public class Comment
    {
        public Guid CommentId { get; set; }

        public string Name { get; set; }

        public string Body { get; set; }

        public string Quote { get; set; }

        public bool IsRemoved { get; set; }

        public Guid? ParentCommentId { get; set; }

        public Guid GameId { get; set; }
    }
}
