using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Models;
using GameStore.Web.Util;
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

        public CommentController(
            IGameService gameService,
            ICommentService commentService,
            IMapper mapper)
        {
            _gameService = gameService;
            _commentService = commentService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("game/{key}/comments")]
        public IActionResult ViewComments(CommentsViewModel comment, string key)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<Comment> comments = _commentService
                    .GetAllCommentsByGameKey(key);

                var commentsForReorder = _mapper.Map<IEnumerable<CommentViewModel>>(comments);

                comment.Comments = ReorderComments(commentsForReorder);

                return View(comment);
            }

            Game game = _gameService.GetGameByKey(key);

            _commentService.AddCommentToGame(
                game,
                _mapper.Map<Comment>(comment));
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

            IEnumerable<Comment> comments = _commentService
                .GetAllCommentsByGameKey(key);

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

        private IEnumerable<CommentViewModel> ReorderComments(
            IEnumerable<CommentViewModel> comments)
        {
            var commentsList = comments.Select(x => x);

            foreach (CommentViewModel c in commentsList)
            {
                c.Replies = commentsList
                    .Where(x => x.ParentCommentId == c.CommentId)
                    .ToList();
            }

            return commentsList;
        }
    }
}
