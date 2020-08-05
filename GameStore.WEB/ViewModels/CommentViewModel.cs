using System.ComponentModel.DataAnnotations;

namespace GameStore.WEB.ViewModels
{
    public class CommentViewModel
    {
        public string CommentId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Body { get; set; }

        public string ParentCommentId { get; set; }

        public string GameId { get; set; }
    }
}
