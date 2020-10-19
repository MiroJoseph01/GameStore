using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.DAL.Pipeline
{
    public class SelectionPipeline : IPipeline
    {
        private readonly List<IFilter<DbModels.Game>> _filters;

        public SelectionPipeline()
        {
            _filters = new List<IFilter<DbModels.Game>>();
        }

        public Expression<Func<DbModels.Game, bool>> Process(Expression<Func<DbModels.Game, bool>> expression)
        {
            foreach (var f in _filters)
            {
                expression = f.Execute(expression);
            }

            return expression;
        }

        public IPipeline Register(IFilter<DbModels.Game> filter)
        {
            _filters.Add(filter);
            return this;
        }
    }
}
