using System.Collections.Generic;

namespace GameStore.Web.ApiModels
{
    public class Comment
    {
        public string CommentId { get; set; }

        public string Name { get; set; }

        public string Body { get; set; }

        public string ParentCommentId { get; set; }

        public string GameId { get; set; }

        public string Quote { get; set; }

        public bool IsRemoved { get; set; }

        public bool IsDisplayed { get; set; }

        public IList<Comment> Replies { get; set; }
    }
}
