using System;
using System.Linq.Expressions;

namespace GameStore.DAL.Pipeline
{
    public interface IFilter<T>
    {
        Expression<Func<T, bool>> Execute(Expression<Func<T, bool>> expression);
    }
}
