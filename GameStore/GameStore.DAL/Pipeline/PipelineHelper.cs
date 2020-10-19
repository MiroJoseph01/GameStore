using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Pipeline
{
    public static class PipelineHelper
    {
        public static Expression<Func<Game, bool>> Combine(
            List<Expression<Func<Game, bool>>> expressions)
        {
            if (expressions.Count == 0)
            {
                return null;
            }

            if (expressions.Count == 1)
            {
                return expressions.First();
            }

            for (int i = 1; i < expressions.Count; i++)
            {
                expressions[0] = CombineTwoExpressions(expressions[0], expressions[i]);
            }

            return expressions[0];
        }

        public static Expression<Func<Game, bool>> CombineTwoExpressions(
            Expression<Func<Game, bool>> firstExpression,
            Expression<Func<Game, bool>> secondExpression)
        {
            var invokedExpression = Expression.Invoke(
                    secondExpression,
                    firstExpression.Parameters);

            var combinedExpression = Expression
                .AndAlso(firstExpression.Body, invokedExpression);

            return Expression
                .Lambda<Func<Game, bool>>(
                    combinedExpression, firstExpression.Parameters);
        }
    }
}
