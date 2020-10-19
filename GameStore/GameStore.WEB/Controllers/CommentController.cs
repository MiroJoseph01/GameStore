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
        private readonly ICommentHelper _commentHelper;

        public CommentController(
            IGameService gameService,
            ICommentService commentService,
            IMapper mapper,
            IAppLogger<CommentController> logger,
            ICommentHelper commentHelper)
        {
            _gameService = gameService;
            _commentService = commentService;
            _mapper = mapper;
            _logger = logger;
            _commentHelper = commentHelper;
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

                if (_commentHelper.QuoteIsPresent(comment, startTag, endTag))
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

                comment.Comments = _commentHelper.ReorderComments(commentsForReorder);
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
                Comments = _commentHelper.ReorderComments(commentsForReorder),
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
    }
}
