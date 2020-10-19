using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace GameStore.DAL.Pipeline.Filters
{
    public class GenreFilter : IFilter<Game>
    {
        private readonly IEnumerable<Guid> _genreIds;
        private readonly IGenreRepository _genreRepository;

        public GenreFilter(IEnumerable<string> genres, IServiceProvider serviceProvider)
        {
            _genreRepository = serviceProvider.GetRequiredService<IGenreRepository>();
            _genreIds = _genreRepository.GetGenreIdsByNames(genres);
        }

        public Expression<Func<Game, bool>> Execute(Expression<Func<Game, bool>> expression)
        {
            if (_genreIds is null || _genreIds.Count() == 0)
            {
                return expression;
            }

            return PipelineHelper.CombineTwoExpressions(
                expression,
                x => x.GenreGames.Where(y => _genreIds.Contains(y.GenreId)).Any());
        }
    }
}
