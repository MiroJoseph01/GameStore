using GameStore.DAL.Entities.Interfaces;

namespace GameStore.DAL.Entities.SupportingModels
{
    public class DeletedGame : ISoftDelete
    {
        public string Id { get; set; }

        public string GameId { get; set; }

        public bool IsRemoved { get; set; }
    }
}
