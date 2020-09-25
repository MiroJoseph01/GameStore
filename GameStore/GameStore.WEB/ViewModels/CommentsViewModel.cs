using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.ViewModels
{
    public class CommentsViewModel
    {
        [Required]
        [Display(Name = "Author")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Text")]
        public string Body { get; set; }

        public bool QuoteIsPresent { get; set; }

        public string Quote { get; set; }

        public string ParentCommentId { get; set; }

        public string DeleteCommentId { get; set; }

        public IEnumerable<CommentViewModel> Comments { get; set; }

        public string GameName { get; set; }

        public string GameKey { get; set; }
    }
}
