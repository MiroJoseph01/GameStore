using GameStore.DAL.Entities.Interfaces;

namespace GameStore.DAL.Entities.SupportingModels
{
    public class View : ISoftDelete
    {
        public string Id { get; set; }

        public string GameId { get; set; }

        public int Views { get; set; }

        public bool IsRemoved { get; set; }
    }
}
