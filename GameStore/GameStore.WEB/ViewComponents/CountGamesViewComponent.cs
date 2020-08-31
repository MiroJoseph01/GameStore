using System.Threading.Tasks;
using GameStore.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.ViewComponents
{
    public class CountGamesViewComponent : ViewComponent
    {
        private readonly IGameService _gameService;

        public CountGamesViewComponent(IGameService gameService)
        {
            _gameService = gameService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(_gameService.Count());
        }
    }
}
