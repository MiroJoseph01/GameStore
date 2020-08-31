using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Models;
using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.Web.Controllers
{
    public class GameController : Controller
    {
        private const string NoPublisher = "NoPublisher";

        private readonly IGameService _gameService;
        private readonly IFileService _fileService;
        private readonly IGenreService _genreService;
        private readonly IPlatformService _platformService;
        private readonly IPublisherService _publisherService;
        private readonly IMapper _mapper;

        public GameController(
            IGameService gameService,
            IFileService fileService,
            IGenreService genreService,
            IPlatformService platformService,
            IPublisherService publisherService,
            IMapper mapper)
        {
            _gameService = gameService;
            _fileService = fileService;
            _genreService = genreService;
            _platformService = platformService;
            _publisherService = publisherService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("game/{key}")]
        public IActionResult ViewGameDetails(string key)
        {
            if (!_gameService.IsPresent(key))
            {
                return NotFound();
            }

            Game game = _gameService.GetGameByKey(key);

            return View(_mapper.Map<GameViewModel>(game));
        }

        [HttpGet]
        [Route("games")]
        public ViewResult Index()
        {
            IEnumerable<Game> games = _gameService.GetAllGames();

            var gamesToView = _mapper.Map<IEnumerable<GameViewModel>>(games);

            return View(gamesToView);
        }

        [HttpGet]
        [Route("games/new")]
        public ViewResult CreateNewGame()
        {
            var platforms = _mapper
                .Map<IList<PlatformViewModel>>(
                    _platformService.GetAllPlatforms());

            IEnumerable<Publisher> publishers = _publisherService
                .GetAllPublishers();
            List<SelectListItem> publisherOptions = publishers
                .Select(x => new SelectListItem()
                {
                    Text = x.CompanyName,
                    Value = x.CompanyName,
                })
                .ToList();
            publisherOptions.Insert(0, new SelectListItem()
            {
                Text = "No publisher",
                Value = NoPublisher,
            });

            var genres = _mapper
                .Map<IList<GenreViewModel>>(_genreService.GetAllGenres());
            var genreOptions = genres.Select(x => new
            {
                GenreId = x.GenreId,
                GenreName = x.GenreName,
            }).ToList();

            GameCreateViewModel game = new GameCreateViewModel
            {
                PlatformOptions = platforms.Select(x => new SelectListItem()
                {
                    Text = x.PlatformName,
                    Value = x.PlatformId,
                }).ToList(),

                GenreOptions = new MultiSelectList(
                    genreOptions,
                    "GenreId",
                    "GenreName"),
            };
            game.PublisherOptions = publisherOptions;

            return View(game);
        }

        [HttpPost]
        [Route("games/new")]
        public IActionResult CreateNewGame(GameCreateViewModel viewGame)
        {
            if (!viewGame.PlatformOptions.Any(x => x.Selected == true))
            {
                ModelState
                    .AddModelError("PlatformOptions", "Platform is required");
            }

            if (!ModelState.IsValid)
            {
                var genres = _mapper
                    .Map<IList<GenreViewModel>>(_genreService.GetAllGenres());

                var genreOptions = genres.Select(x => new
                {
                    GenreId = x.GenreId,
                    GenreName = x.GenreName,
                }).ToList();
                viewGame.GenreOptions = new MultiSelectList(
                    genreOptions,
                    "GenreId",
                    "GenreName");

                viewGame.PlatformOptions = _mapper
                    .Map<IList<PlatformViewModel>>(_platformService
                    .GetAllPlatforms())
                    .Select(x => new SelectListItem()
                    {
                        Text = x.PlatformName,
                        Value = x.PlatformId,
                    })
                    .ToList();

                viewGame.PublisherOptions = _publisherService
                    .GetAllPublishers()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.CompanyName,
                        Value = x.CompanyName,
                    })
                    .ToList();

                viewGame.PublisherOptions.Insert(0, new SelectListItem()
                {
                    Text = "No publisher",
                    Value = NoPublisher,
                });

                return View(viewGame);
            }

            IEnumerable<SelectListItem> platformoptions = viewGame
                .PlatformOptions
                .Where(x => x.Selected == true);
            viewGame.PlatformOptions = platformoptions.ToList();

            viewGame.GameGenres = viewGame.Genres
                .Select(x => new GenreViewModel { GenreId = x })
                .ToList();

            var game = _mapper.Map<Game>(viewGame);

            if (viewGame.PublisherName != NoPublisher)
            {
                Publisher publisher = _publisherService
                    .GetPublisherByName(viewGame.PublisherName);
                game.PublisherId = publisher.PublisherId;
            }

            _gameService.CreateGame(game);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("games/update")]
        public IActionResult Update(GameViewModel game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(game);
            }

            var convertedGame = _mapper.Map<Game>(game);

            Game gameToView = _gameService.EditGame(convertedGame);

            return RedirectToAction(nameof(ViewGameDetails), game.Key);
        }

        [HttpPost]
        [Route("games/remove/{key}")]
        public IActionResult Delete(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return View();
            }

            if (!_gameService.IsPresent(key))
            {
                return NotFound();
            }

            Game game = _gameService.GetGameByKey(key);

            _gameService.DeleteGame(game);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("game/{key}/download")]
        public HttpResponseMessage Download()
        {
            return _fileService.CreateFile();
        }

        [HttpGet]
        [Route("publisher/{name}")]
        public IActionResult ViewPublisher(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return View();
            }

            string publisherName = name.Replace('_', ' ');

            Publisher publisher = _publisherService
                .GetPublisherByName(publisherName);

            if (publisher is null)
            {
                return NotFound();
            }

            var publisherForView = _mapper
                .Map<PublisherViewModel>(publisher);

            publisherForView.PublisherGames = _mapper
                .Map<IEnumerable<GameViewModel>>(_gameService
                .GetGamesByPublisher(publisher))
                .ToList();

            return View(publisherForView);
        }

        [HttpGet]
        [Route("/publisher/new")]
        public IActionResult CreateNewPublisher()
        {
            return View();
        }

        [HttpPost]
        [Route("/publisher/new")]
        public IActionResult CreateNewPublisher(PublisherCreateModel publisher)
        {
            if (!ModelState.IsValid)
            {
                return View(publisher);
            }

            var publisherForCreation = _mapper.Map<Publisher>(publisher);

            _publisherService.CreatePublisher(publisherForCreation);

            return RedirectToAction(nameof(Index));
        }
    }
}
