using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.BLL.Interfaces.Services;
using GameStore.DAL.Pipeline;
using GameStore.Web.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessModels = GameStore.BLL.Models;
using WebModels = GameStore.Web.ViewModels;

namespace GameStore.Web.Controllers
{
    [CustomController("Game")]
    public class GameController : Controller
    {
        private const string NoPublisher = "NoPublisher";

        private readonly IGameService _gameService;
        private readonly IFileService _fileService;
        private readonly IGenreService _genreService;
        private readonly IPlatformService _platformService;
        private readonly IPublisherService _publisherService;
        private readonly IMapper _mapper;
        private readonly IPipeline _pipeline;

        private readonly List<int> _pageSizes;

        public GameController(
            IGameService gameService,
            IFileService fileService,
            IGenreService genreService,
            IPlatformService platformService,
            IPublisherService publisherService,
            IMapper mapper,
            IPipeline pipeline
            )
        {
            _gameService = gameService;
            _fileService = fileService;
            _genreService = genreService;
            _platformService = platformService;
            _publisherService = publisherService;
            _mapper = mapper;
            _pipeline = pipeline;

            _pageSizes = new List<int>
            {
                10, 20, 50, 100, -1,
            };
        }

        [HttpGet]
        [Route("game/{key}")]
        public IActionResult ViewGameDetails(string key)
        {
            if (!_gameService.IsPresent(key))
            {
                return NotFound();
            }

            _gameService.AddView(key);

            BusinessModels.Game game = _gameService.GetGameByKey(key);

            return View(_mapper.Map<WebModels.GameViewModel>(game));
        }

        [HttpGet]
        [Route("games")]
        [Route("")]
        public IActionResult Index([FromQuery] WebModels.QueryModel queryModel)
        {
            var platformOptions = _platformService.GetAllPlatforms()
                .Select(x =>
                {
                    var result = new SelectListItem
                    {
                        Value = x.PlatformName,
                        Text = x.PlatformName,
                    };

                    if (!(queryModel.PlatformOptions is null))
                    {
                        if (queryModel.PlatformOptions.Contains(x.PlatformName))
                        { result.Selected = true; }
                    }

                    return result;
                })
                .ToList();

            var genreOptions = _genreService.GetAllGenres()
                .Select(x =>
                {
                    var result = new SelectListItem
                    {
                        Value = x.GenreName,
                        Text = x.GenreName,
                    };

                    if (!(queryModel.GenresOptions is null))
                    {
                        if (queryModel.GenresOptions.Contains(x.GenreName))
                        { result.Selected = true; }
                    }

                    return result;
                })
                .ToList();

            var publisherOptions = _publisherService.GetAllPublishers()
                .Select(x =>
                {
                    var result = new SelectListItem
                    {
                        Value = x.CompanyName,
                        Text = x.CompanyName,
                    };

                    if (!(queryModel.PublisherOptions is null))
                    {
                        if (queryModel.PublisherOptions.Contains(x.CompanyName))
                        { result.Selected = true; }
                    }

                    return result;
                })
                .ToList();

            if (queryModel.To < queryModel.From)
            {
                ModelState
                    .AddModelError("Query.To", "Max price must be more than Min price");
            }

            if (!string.IsNullOrWhiteSpace(queryModel.SearchByGameName)
                && queryModel.SearchByGameName.Length <= 2)
            {
                ModelState
                    .AddModelError(
                        "Query.SearchByGameName",
                        "If you want to search by name, use more than 3 symbols");
            }

            queryModel.PlatformOptions = ClearValues(platformOptions, queryModel.PlatformOptions);
            queryModel.GenresOptions = ClearValues(genreOptions, queryModel.GenresOptions);
            queryModel.PublisherOptions = ClearValues(publisherOptions, queryModel.PublisherOptions);

            List<WebModels.ShortGameViewModel> games;

            var queryViewModel = new WebModels.QueryViewModel(_gameService)
            {
                Filter = queryModel.Filter,
                DateFilter = queryModel.DateFilter,
                From = queryModel.From,
                To = queryModel.To,
                IsFiltered = queryModel.IsFiltered,
                Page = queryModel.Page,
                PageSize = queryModel.PageSize.ToString(),
                SearchByGameName = queryModel.SearchByGameName,
                PlatformOptions = platformOptions,
                PublisherOptions = publisherOptions,
                GenresOptions = genreOptions,
            };

            if (!_pageSizes.Contains(queryModel.PageSize))
            {
                queryViewModel.PageSize = "10";
                queryModel.PageSize = 10;
            }

            if (queryModel.PageSize == -1)
            {
                queryModel.PageSize = _gameService.Count();
                queryViewModel.PageSize = "All";
            }

            if (!ModelState.IsValid)
            {
                var qm = new BusinessModels.QueryModel
                {
                    Skip = (queryModel.Page - 1) * queryModel.PageSize,
                    Take = queryModel.PageSize,
                };

                var check = _gameService.FilterGames(qm);

                games = _mapper
                        .Map<IEnumerable<WebModels.ShortGameViewModel>>(
                           check)
                        .ToList();

                var invalidGameView = new WebModels.ShortsGameViewModel(_gameService)
                {
                    Games = games,
                    Query = queryViewModel,
                    PageViewModel = new WebModels.PageViewModel(
                        queryModel.Page,
                        GetTotlaPages(_gameService.Count(), queryModel.PageSize)),
                };

                return View(invalidGameView);
            }

            if (queryModel.IsFiltered)
            {
                queryViewModel.Page = 1;
                queryModel.Page = 1;
            }

            var queryToFilter = _mapper.Map<BusinessModels.QueryModel>(queryModel);

            queryToFilter.Skip = 0;
            queryToFilter.Take = 0;

            var totalPages = GetTotlaPages(_gameService.FilterGames(queryToFilter).Count(), queryModel.PageSize);

            if (totalPages < queryViewModel.Page)
            {
                return View(new WebModels.ShortsGameViewModel(_gameService)
                {
                    Games = new List<WebModels.ShortGameViewModel>(),
                    Query = queryViewModel,

                    PageViewModel = new WebModels.PageViewModel(1, 1),
                });
            }

            queryToFilter.Take = queryModel.PageSize;
            queryToFilter.Skip = (queryModel.Page - 1) * queryModel.PageSize;

            var fromDB = _gameService.FilterGames(queryToFilter);

            games = _mapper
                        .Map<IEnumerable<WebModels.ShortGameViewModel>>(
                           fromDB)
                            .ToList();

            var gameView = new WebModels.ShortsGameViewModel(_gameService)
            {
                Games = games,
                Query = queryViewModel,

                PageViewModel = new WebModels.PageViewModel(
                    queryViewModel.Page,
                    totalPages),
            };

            return View(gameView);
        }

        [HttpGet]
        [Route("games/new")]
        public ViewResult CreateNewGame()
        {
            var platforms = _platformService.GetAllPlatforms();

            IEnumerable<BusinessModels.Publisher> publishers = _publisherService.GetAllPublishers();
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

            var genres = _mapper.Map<IList<WebModels.GenreViewModel>>(_genreService.GetAllGenres());
            var genreOptions = genres.Select(x => new
            {
                GenreId = x.GenreId,
                GenreName = x.GenreName,
            }).ToList();

            var game = new WebModels.GameCreateViewModel
            {
                PlatformOptions = platforms.Select(x => new SelectListItem()
                {
                    Text = x.PlatformName,
                    Value = x.PlatformId.ToString(),
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
        public IActionResult CreateNewGame(WebModels.GameCreateViewModel viewGame)
        {
            if (!viewGame.PlatformOptions.Any(x => x.Selected == true))
            {
                ModelState
                    .AddModelError("PlatformOptions", "Platform is required");
            }

            if (!ModelState.IsValid)
            {
                var genres = _mapper.Map<IList<WebModels.GenreViewModel>>(_genreService.GetAllGenres());

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
                    .Map<IList<WebModels.PlatformViewModel>>(_platformService
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

            IEnumerable<SelectListItem> platformoptions = viewGame.PlatformOptions.Where(x => x.Selected == true);
            viewGame.PlatformOptions = platformoptions.ToList();

            viewGame.GameGenres = viewGame.Genres
                .Select(x => new WebModels.GenreViewModel { GenreId = x })
                .ToList();

            var game = _mapper.Map<BusinessModels.Game>(viewGame);

            if (viewGame.PublisherName != NoPublisher)
            {
                BusinessModels.Publisher publisher = _publisherService.GetPublisherByName(viewGame.PublisherName);
                game.PublisherId = publisher.PublisherId;
            }

            _gameService.CreateGame(game);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("games/update")]
        public IActionResult Update([FromBody] WebModels.GameViewModel game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(game);
            }

            var convertedGame = _mapper.Map<BusinessModels.Game>(game);

            BusinessModels.Game gameToView = _gameService.EditGame(convertedGame);

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

            BusinessModels.Game game = _gameService.GetGameByKey(key);

            _gameService.DeleteGame(game);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("game/{key}/download")]
        public async Task<IActionResult> Download()
        {
            return await _fileService.CreateFile(this);
        }

        [HttpGet]
        [Route("publisher/{name}")]
        public IActionResult ViewPublisher(string name)
        {
            string publisherName = name.Replace('_', ' ');

            if (!_publisherService.IsPresent(publisherName))
            {
                return NotFound();
            }

            BusinessModels.Publisher publisher = _publisherService
                .GetPublisherByName(publisherName);

            var publisherForView = _mapper
                .Map<WebModels.PublisherViewModel>(publisher);

            publisherForView.PublisherGames = _mapper
                .Map<IEnumerable<WebModels.GameViewModel>>(
                     _gameService
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
        public IActionResult CreateNewPublisher(WebModels.PublisherCreateViewModel publisher)
        {
            if (!ModelState.IsValid)
            {
                return View(publisher);
            }

            var publisherForCreation = _mapper.Map<BusinessModels.Publisher>(publisher);

            _publisherService.CreatePublisher(publisherForCreation);

            return RedirectToAction(nameof(Index));
        }

        private int GetTotlaPages(int count, int pageSize)
        {
            return (int)Math.Ceiling(count / (double)pageSize);
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
            };
        }

        private List<string> ClearValues(List<SelectListItem> allowedValues, List<string> listToClear)
        {
            var listOfNames = allowedValues.Select(x => x.Text).ToList();

            if (listToClear != null)
            {
                listToClear = listToClear.Where(x => listOfNames.Contains(x)).ToList();
            }

            return listToClear;
        }
    }
}
