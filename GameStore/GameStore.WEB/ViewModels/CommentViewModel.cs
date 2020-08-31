using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.ViewModels
{
    public class CommentViewModel
    {
        public string CommentId { get; set; }

        [Required]
        [Display(Name = "Author")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Text")]
        public string Body { get; set; }

        public string ParentCommentId { get; set; }

        public string GameId { get; set; }

        public IList<CommentViewModel> Replies { get; set; }
    }
}
