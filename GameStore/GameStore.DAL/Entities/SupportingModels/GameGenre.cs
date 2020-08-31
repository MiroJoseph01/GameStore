using System;

namespace GameStore.DAL.Entities.SupportingModels
{
    public class GameGenre
    {
        public Guid GameId { get; set; }

        public virtual Game Game { get; set; }

        public Guid GenreId { get; set; }

        public Genre Genre { get; set; }
    }
}
