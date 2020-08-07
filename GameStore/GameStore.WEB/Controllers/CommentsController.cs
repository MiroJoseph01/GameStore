using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Models;
using GameStore.WEB.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameStore.WEB.Controllers
{
    public class CommentsController : Controller
    {
        private readonly IGameService _gameService;
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;
        private readonly IMapper _mapper;

        public CommentsController(IGameService gameService, ICommentService commentService, ILogger<CommentsController> logger, IMapper mapper)
        {
            _gameService = gameService;
            _commentService = commentService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("game/{key}/newcomment")]
        public IActionResult NewComment([FromBody] CommentViewModel comment, string key)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Problem with creating comment for game with key: \'{key}\'");

                return BadRequest(comment);
            }

            Game game = _gameService.GetGameByKey(key);

            _commentService.AddCommentToGame(game, _mapper.Map<Comment>(comment));

            _logger.LogDebug($"User \'{comment.Name}\' added new comment to the game with key {key}");

            return Ok(_mapper.Map<GameViewModel>(_gameService.GetGameByKey(key)));
        }

        [HttpGet]
        [Route("game/{key}/comments")]
        public IActionResult Comments(string key)
        {
            IEnumerable<Comment> comments = _commentService.GetAllCommentsByGameKey(key);

            if (!_gameService.IsPresent(key))
            {
                return NotFound("Can't find the game");
            }

            if (comments.Count() == 0)
            {
                return Ok("No comments for this game");
            }

            return Ok(_mapper.Map<IEnumerable<CommentViewModel>>(comments));
        }
    }
}
