using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace GameStore.DAL.Pipeline.Filters
{
    public class PublisherFilter : IFilter<Game>
    {
        private readonly IEnumerable<string> _publisherIds;
        private readonly IPublisherRepository _publisherRepository;

        public PublisherFilter(IEnumerable<string> publishers, IServiceProvider serviceProvider)
        {
            _publisherRepository = serviceProvider.GetRequiredService<IPublisherRepository>();
            _publisherIds = _publisherRepository.GetPublisherIdsByNames(publishers);

            if (publishers.Count() == 0 && _publisherIds.Count() == 0)
            {
                _publisherIds = null;
            }
        }

        public Expression<Func<Game, bool>> Execute(Expression<Func<Game, bool>> expression)
        {
            if (_publisherIds is null)
            {
                return expression;
            }

            return PipelineHelper.CombineTwoExpressions(
                expression,
                x => _publisherIds.Contains(x.Publisher.PublisherId));
        }
    }
}
