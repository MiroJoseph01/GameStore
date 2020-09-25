using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Models;
using GameStore.Web.Util;
using GameStore.Web.Util.Logger;
using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers
{
    [CustomController("Comment")]
    public class CommentController : Controller
    {
        private readonly IGameService _gameService;
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;
        private readonly IAppLogger<CommentController> _logger;

        public CommentController(
            IGameService gameService,
            ICommentService commentService,
            IMapper mapper,
            IAppLogger<CommentController> logger)
        {
            _gameService = gameService;
            _commentService = commentService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        [Route("game/{key}/comments")]
        public IActionResult ViewComments(CommentsViewModel comment, string key)
        {
            try
            {
                int startTag = -1;
                int endTag = -1;

                if (!string.IsNullOrWhiteSpace(comment.Body))
                {
                    startTag = comment.Body.IndexOf("<quote>");
                    endTag = comment.Body.IndexOf("</quote>");
                }

                if (startTag > -1 && endTag > -1 && comment.QuoteIsPresent)
                {
                    var checkBody = _commentService.GetCommentBody(comment.Body, startTag, endTag);

                    if (string.IsNullOrWhiteSpace(checkBody))
                    {
                        ModelState.AddModelError("Body", "Comment can not contain only quote");
                    }

                    var checkQuote = _commentService.GetCommentQuote(comment.Body, startTag, endTag);

                    if (string.IsNullOrWhiteSpace(checkQuote))
                    {
                        ModelState.AddModelError("Body", "Comment can not contain empty quote");
                    }

                    comment.Quote = checkQuote;

                    comment.Body = checkBody;
                }
                else if (comment.QuoteIsPresent)
                {
                    ModelState.AddModelError("Body", "Problems with quote tags");
                }
            }
            catch (ArgumentNullException e)
            {
                _logger.LogWarning("Uncorrect quote input", e);

                ModelState.AddModelError("Body", "Uncorrect quote input");
            }

            Game game = _gameService.GetGameByKey(key);

            if (!ModelState.IsValid)
            {
                IEnumerable<Comment> comments = _commentService.GetAllCommentsByGameKey(key);

                var commentsForReorder = _mapper.Map<IEnumerable<CommentViewModel>>(comments);

                comment.Comments = ReorderComments(commentsForReorder);
                comment.GameName = game.Name;
                comment.GameKey = game.Key;
                comment.QuoteIsPresent = true;

                return View(comment);
            }

            var commentToAdd = _mapper.Map<Comment>(comment);

            _commentService.AddCommentToGame(game, commentToAdd);

            return RedirectToAction(nameof(ViewComments));
        }

        [HttpGet]
        [Route("game/{key}/comments")]
        public IActionResult ViewComments(string key)
        {
            if (!_gameService.IsPresent(key))
            {
                return NotFound();
            }

            Game game = _gameService.GetGameByKey(key);

            IEnumerable<Comment> comments = _commentService.GetAllCommentsByGameKey(key);

            if (comments.Count() == 0)
            {
                CommentsViewModel noComments = new CommentsViewModel()
                {
                    GameKey = game.Key,
                    GameName = game.Name,
                };

                return View(noComments);
            }

            var commentsForReorder = _mapper.Map<IEnumerable<CommentViewModel>>(comments);

            CommentsViewModel commentsModel = new CommentsViewModel()
            {
                Comments = ReorderComments(commentsForReorder),
                GameKey = game.Key,
                GameName = game.Name,
            };

            return View(commentsModel);
        }

        [HttpPost]
        [Route("/comment/delete")]
        public IActionResult DeleteComment(CommentsViewModel commentsViewModel)
        {
            var commentToDelete = new Comment { CommentId = Guid.Parse(commentsViewModel.DeleteCommentId) };

            _commentService.DeleteComment(commentToDelete);

            return RedirectToAction(nameof(ViewComments), new { key = commentsViewModel.GameKey });
        }

        private IEnumerable<CommentViewModel> ReorderComments(
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
