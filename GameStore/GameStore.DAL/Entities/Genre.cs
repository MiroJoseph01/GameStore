using System;
using GameStore.DAL.Entities.Interfaces;

namespace GameStore.DAL.Entities
{
    public class Genre : ISoftDelete
    {
        public Guid GenreId { get; set; }

        public string GenreName { get; set; }

        public Guid? ParentGenreId { get; set; }

        public bool IsRemoved { get; set; }
    }
}
