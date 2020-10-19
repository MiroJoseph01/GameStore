using System;
using System.Linq.Expressions;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Pipeline.Filters
{
    public class DateFilter : IFilter<Game>
    {
        private readonly DateTime? _date;

        public DateFilter(DateTime? date)
        {
            _date = date;
        }

        public Expression<Func<Game, bool>> Execute(Expression<Func<Game, bool>> expression)
        {
            if (_date is null)
            {
                return expression;
            }

            return PipelineHelper.CombineTwoExpressions(expression, x => x.Date >= _date);
        }
    }
}
