using GameStore.DAL.Entities.SupportingModels;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IViewRepository : IRepository<View>
    {
        int GetViewsByGameId(string gameId);
    }
}
