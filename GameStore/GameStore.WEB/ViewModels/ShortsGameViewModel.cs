using System.Collections.Generic;
using GameStore.BLL.Interfaces;

namespace GameStore.Web.ViewModels
{
    public class ShortsGameViewModel
    {
        public ShortsGameViewModel(IGameService gameService)
        {
            Query = new QueryViewModel(gameService);
        }

        public IList<ShortGameViewModel> Games { get; set; }

        public QueryViewModel Query { get; set; }

        public PageViewModel PageViewModel { get; set; }
    }
}
