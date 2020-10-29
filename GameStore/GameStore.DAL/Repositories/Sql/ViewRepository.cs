using System.Linq;
using GameStore.DAL.Entities.SupportingModels;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;

namespace GameStore.DAL.Repositories.Sql
{
    public class ViewRepository : Repository<View>, IViewRepository
    {
        private readonly GameStoreContext _dbContext;

        public ViewRepository(GameStoreContext dbContext, IEntityStateLogger<View> stateLogger)
            : base(dbContext, stateLogger)
        {
            _dbContext = dbContext;
        }

        public int GetViewsByGameId(string gameId)
        {
            var result = 0;
            var view = _dbContext.Views.FirstOrDefault(x => x.GameId == gameId);

            if (view != null)
            {
                result = view.Views;
            }

            return result;
        }
    }
}
