using System;
using System.Collections.Generic;
using GameStore.BLL.Models;

namespace GameStore.BLL.Interfaces
{
    public interface IGenreService
    {
        Genre AddGenre(Genre genre);

        void DeleteGenre(Genre genre);

        Genre UpdateGenre(Genre genre);

        Genre GetGenreById(Guid genreId);

        IEnumerable<Genre> GetAllGenres();
    }
}
