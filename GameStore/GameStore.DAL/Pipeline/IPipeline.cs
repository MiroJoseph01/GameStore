using System;
using System.Linq.Expressions;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.DAL.Pipeline
{
    public interface IPipeline
    {
        IPipeline Register(IFilter<DbModels.Game> filter);

        Expression<Func<DbModels.Game, bool>> Process(Expression<Func<DbModels.Game, bool>> expresion);
    }
}
