using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Models;
using GameStore.WEB.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameStore.WEB.Controllers
{
    public class GamesController : Controller
    {
        private readonly ILogger<GamesController> _logger;
        private readonly IGameService _gameService;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public GamesController(IGameService gameService, IFileService fileService, IMapper mapper, ILogger<GamesController> logger)
        {
            _logger = logger;
            _gameService = gameService;
            _fileService = fileService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("game/{key}")]
        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 60)]
        public IActionResult Index(string key)
        {
            Game game = _gameService.GetGameByKey(key);

            if (game is null)
            {
                return NotFound($"Can't find the game with key: {key}");
            }

            return Ok(game);
        }

        [HttpGet]
        [Route("games")]
        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 60)]
        public IActionResult Index()
        {
            IEnumerable<Game> games = _gameService.GetAllGames();

            if (games.Count() == 0)
            {
                return Ok("No games yet");
            }

            return Ok(_mapper.Map<IEnumerable<GameViewModel>>(games));
        }

        [HttpPost]
        [Route("games/new")]
        public IActionResult New([FromBody] GameViewModel viewGame)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(viewGame);
            }

            Game game = _mapper.Map<Game>(viewGame);

            _gameService.CreateGame(game);

            _logger.LogDebug($"Created game with name: {viewGame.Name}");

            return Ok(_mapper.Map<GameViewModel>(_gameService.GetGameByKey(viewGame.Key)));
        }

        [HttpPost]
        [Route("games/update")]
        public IActionResult Update([FromBody] GameViewModel game)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogDebug($"Game with id \"{game.GameId}\" was not changed");

                return BadRequest(game);
            }

            Game convertedGame = _mapper.Map<Game>(game);

            Game gameToView = _gameService.EditGame(convertedGame);

            _logger.LogDebug($"Game with id \"{game.GameId}\" was changed ");

            return Ok(_mapper.Map<GameViewModel>(gameToView));
        }

        [HttpPost]
        [Route("games/remove/{key}")]
        public IActionResult Delete(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return BadRequest("Can't find the game key");
            }

            Game game = _gameService.GetGameByKey(key);

            if (game is null)
            {
                return NotFound($"Can't find game with key: {key}");
            }

            _gameService.DeleteGame(game);

            _logger.LogDebug($"Game with Id \"{key}\" was soft deleted ");

            return Ok($"Game with key: \'{key}\' is deleted");
        }

        [HttpGet]
        [Route("game/{key}/download")]
        public HttpResponseMessage Download()
        {
            return _fileService.CreateFile();
        }
    }
}
