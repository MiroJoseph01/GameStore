using System.Collections.Generic;
using GameStore.BLL.Models;

namespace GameStore.BLL.Interfaces.Services
{
    public interface IGenreService
    {
        Genre AddGenre(Genre genre);

        void DeleteGenre(Genre genre);

        Genre UpdateGenre(Genre genre);

        Genre GetGenreById(string genreId);

        IEnumerable<Genre> GetAllGenres();
    }
}
