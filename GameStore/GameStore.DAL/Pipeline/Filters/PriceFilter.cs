using System;
using System.Linq.Expressions;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Pipeline.Filters
{
    public class PriceFilter : IFilter<Game>
    {
        private readonly decimal _minPrice;
        private readonly decimal _maxPrice;

        public PriceFilter(decimal minPrice, decimal maxPrice)
        {
            _minPrice = minPrice;
            _maxPrice = maxPrice;
        }

        public Expression<Func<Game, bool>> Execute(Expression<Func<Game, bool>> expression)
        {
            if (_maxPrice == 0)
            {
                return expression;
            }

            return PipelineHelper.CombineTwoExpressions(
                expression,
                x => x.Price >= _minPrice && x.Price <= _maxPrice);
        }
    }
}
