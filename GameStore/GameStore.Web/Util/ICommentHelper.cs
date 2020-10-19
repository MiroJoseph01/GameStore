using System.Collections.Generic;
using GameStore.Web.ViewModels;

namespace GameStore.Web.Util
{
    public interface ICommentHelper
    {
        IEnumerable<CommentViewModel> ReorderComments(IEnumerable<CommentViewModel> commentsToReorder);

        bool QuoteIsPresent(CommentsViewModel model, int startTag, int endTag);
    }
}
