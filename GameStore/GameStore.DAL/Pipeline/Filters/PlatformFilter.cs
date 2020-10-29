using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace GameStore.DAL.Pipeline.Filters
{
    public class PlatformFilter : IFilter<Game>
    {
        private readonly IEnumerable<string> _platformIds;
        private readonly IPlatformRepository _platformRepository;

        public PlatformFilter(IEnumerable<string> platform, IServiceProvider serviceProvider)
        {
            _platformRepository = serviceProvider.GetRequiredService<IPlatformRepository>();
            _platformIds = _platformRepository.GetPlatformIdsByNames(platform);
        }

        public Expression<Func<Game, bool>> Execute(Expression<Func<Game, bool>> expression)
        {
            if (_platformIds is null || _platformIds.Count() == 0)
            {
                return expression;
            }

            return PipelineHelper.CombineTwoExpressions(
                expression,
                x => x.PlatformGames.Where(y => _platformIds.Contains(y.PlatformId)).Any());
        }
    }
}
