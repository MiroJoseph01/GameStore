using System.Collections.Generic;
using System.Linq;
using GameStore.Web.ViewModels;

namespace GameStore.Web.Util
{
    public class CommentHelper : ICommentHelper
    {
        public bool QuoteIsPresent(CommentsViewModel comment, int startTag, int endTag)
        {
            return startTag > -1 && endTag > -1 && comment.QuoteIsPresent;
        }

        public IEnumerable<CommentViewModel> ReorderComments(
            IEnumerable<CommentViewModel> commentsToReorder)
        {
            var comments = commentsToReorder.ToList();

            // create replies and check if comment is displayed
            foreach (var c in comments)
            {
                if (!c.IsRemoved)
                {
                    c.IsDisplayed = true;
                }

                c.Replies = comments.Where(x => x.ParentCommentId == c.CommentId).ToList();
            }

            // change comments which are already deleted but have answears and quotes
            foreach (var c in comments)
            {
                var list = new List<CommentViewModel>();
                var check = FlatReplies(c.Replies, list).Where(x => !x.IsRemoved).Any();

                if (c.IsRemoved && check)
                {
                    c.Body = "This comment is deleted by moderator";
                    c.IsDisplayed = true;

                    var replies = c.Replies.Where(r => !(r.Quote is null));

                    foreach (var r in replies)
                    {
                        r.Quote = "This comment is deleted by moderator";
                    }
                }
            }

            // clear replies of each comment
            foreach (var c in comments)
            {
                var replies = new List<CommentViewModel>();

                foreach (var r in c.Replies)
                {
                    if (r.IsDisplayed || !r.IsRemoved)
                    {
                        replies.Add(r);
                    }
                }

                c.Replies = replies;
            }

            comments.RemoveAll(x => x.Replies.Count == 0 && !x.IsDisplayed);

            return comments;
        }

        private List<CommentViewModel> FlatReplies(IList<CommentViewModel> replies, List<CommentViewModel> result)
        {
            if (replies == null)
            {
                return result;
            }

            result.AddRange(replies);

            foreach (var r in replies)
            {
                if (r.Replies != null)
                {
                    FlatReplies(r.Replies, result);
                }
            }

            return result;
        }
    }
}
