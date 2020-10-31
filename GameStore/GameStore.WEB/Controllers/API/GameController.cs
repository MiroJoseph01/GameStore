using System.Collections.Generic;
using AutoMapper;
using GameStore.BLL.Interfaces.Services;
using GameStore.BLL.Models;
using GameStore.Web.ApiModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GameStore.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IPublisherService _publisherService;
        private readonly IMapper _mapper;

        public GameController(
            IGameService gameService,
            IPublisherService publisherService,
            IMapper mapper)
        {
            _gameService = gameService;
            _publisherService = publisherService;
            _mapper = mapper;
        }

        // GET: api/<GameController>
        public IActionResult Get()
        {
            var games = _gameService.GetAllGames();

            return Ok(_mapper.Map<IEnumerable<GameViewModel>>(games));
        }

        // GET api/<GameController>/5
        [HttpGet("{keyOrId}")]
        public IActionResult Get(string keyOrId)
        {
            var game = _gameService.GetGameById(keyOrId);

            if (game is null)
            {
                game = _gameService.GetGameByKey(keyOrId);
            }

            if (game is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<GameViewModel>(game));
        }

        // POST api/<GameController>
        [HttpPost]
        public IActionResult Post([FromBody] GameCreateAndUpdateModel value)
        {
            if (value.GameId != null && _gameService.IsPresent(value.GameId))
            {
                ModelState.AddModelError("GameId", "Game is already exist");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var game = _mapper.Map<Game>(value);

            if (!string.IsNullOrWhiteSpace(value.PublisherName) || !(value.PublisherName == "No Publisher"))
            {
                var publisher = _publisherService.GetPublisherByName(value.PublisherName);
                game.PublisherId = publisher?.PublisherId;
            }

            _gameService.CreateGame(game);

            return Ok();
        }

        // PUT api/<GameController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] GameCreateAndUpdateModel value)
        {
            if (!_gameService.IsPresent(id))
            {
                ModelState.AddModelError("GameId", "Game does not exist");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var game = _mapper.Map<Game>(value);

            if (!string.IsNullOrWhiteSpace(value.PublisherName) || !(value.PublisherName == "No Publisher"))
            {
                var publisher = _publisherService.GetPublisherByName(value.PublisherName);
                game.PublisherId = publisher?.PublisherId;
            }

            _gameService.EditGame(game);

            return Ok();
        }

        // DELETE api/<GameController>/5
        [HttpDelete("{key}")]
        public IActionResult Delete(string key)
        {
            if (!_gameService.IsPresent(key))
            {
                return NotFound();
            }

            _gameService.DeleteGame(_gameService.GetGameByKey(key));

            return Ok();
        }
    }
}
