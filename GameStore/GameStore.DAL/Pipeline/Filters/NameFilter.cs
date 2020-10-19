using System;
using System.Linq.Expressions;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Pipeline.Filters
{
    public class NameFilter : IFilter<Game>
    {
        private readonly string _searchByName;

        public NameFilter(string searchByName)
        {
            _searchByName = searchByName;
        }

        public Expression<Func<Game, bool>> Execute(Expression<Func<Game, bool>> expression)
        {
            if (_searchByName is null)
            {
                return expression;
            }

            return PipelineHelper.CombineTwoExpressions(expression, x => x.Name.ToLower().Contains(_searchByName.ToLower()));
        }
    }
}
