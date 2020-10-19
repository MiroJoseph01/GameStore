using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.Web.Util;
using GameStore.Web.ViewModels;
using Xunit;

namespace GameStore.Web.Tests
{
    public class CommentHelperTest
    {
        private readonly CommentHelper _commentHelper;

        public CommentHelperTest()
        {
            _commentHelper = new CommentHelper();
        }

        [Fact]
        public void QuoteIsPresent_PassCommentsViewModelStarTagEndTag_ReturnTrue()
        {
            var comment = new CommentsViewModel
            {
                QuoteIsPresent = true,
            };

            var startTag = 1;
            var endTag = 10;

            var result = _commentHelper.QuoteIsPresent(comment, startTag, endTag);

            Assert.True(result);
        }

        [Fact]
        public void QuoteIsPresent_PassCommentsViewModelStarTagEndTag_ReturnFalse()
        {
            var comment = new CommentsViewModel
            {
                QuoteIsPresent = false,
            };

            var startTag = -1;
            var endTag = -1;

            var result = _commentHelper.QuoteIsPresent(comment, startTag, endTag);

            Assert.False(result);
        }

        [Fact]
        public void ReorderComments_PassListOfComments_ReturnListOfCommentsWithReplies()
        {
            var comment1 = new CommentViewModel
            {
                Body = "Body1",
                IsRemoved = false,
                Quote = "Quote1",
                CommentId = Guid.NewGuid().ToString(),
                Name = "Author1",
                ParentCommentId = null,
            };

            var comment2 = new CommentViewModel
            {
                Body = "Body2",
                IsRemoved = false,
                Quote = "Quote2",
                CommentId = Guid.NewGuid().ToString(),
                Name = "Author2",
                ParentCommentId = comment1.CommentId,
            };

            var comment3 = new CommentViewModel
            {
                Body = "Body3",
                IsRemoved = false,
                Quote = "Quote3",
                CommentId = Guid.NewGuid().ToString(),
                Name = "Author3",
                ParentCommentId = comment2.CommentId,
            };

            var comments = new List<CommentViewModel> { comment1, comment2, comment3 };

            var result = _commentHelper.ReorderComments(comments).ToList();

            Assert.Equal(comment2.Body, result.First().Replies.First().Body);
        }
    }
}
