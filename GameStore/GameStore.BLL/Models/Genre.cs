using System;

namespace GameStore.BLL.Models
{
    public class Genre
    {
        public Guid GenreId { get; set; }

        public string GenreName { get; set; }

        public Guid? ParentGenreId { get; set; }
    }
}
