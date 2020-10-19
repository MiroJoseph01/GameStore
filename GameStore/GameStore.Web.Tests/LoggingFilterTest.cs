using System;
using System.Collections.Generic;
using GameStore.Web.Util.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Web.Tests
{
    public class LoggingFilterTest
    {
        private readonly Mock<ILogger<LoggingFilter>> _logger;
        private readonly LoggingFilter _loggingFilter;
        private readonly ActionExecutingContext _actionExecutingContext;
        private readonly ActionExecutedContext _actionExecutedContext;

        public LoggingFilterTest()
        {
            _logger = new Mock<ILogger<LoggingFilter>>();

            _loggingFilter = new LoggingFilter(_logger.Object);

            var actionContext = new ActionContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor(),
            };

            _actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>());

            _actionExecutedContext = new ActionExecutedContext(
               actionContext,
               new List<IFilterMetadata>(),
               new Dictionary<string, object>());
        }

        [Fact]
        public void OnActionExecuting_PassContext()
        {
            _loggingFilter.OnActionExecuting(_actionExecutingContext);

            _logger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [Fact]
        public void OnActionExecuted_PassContext()
        {
            _loggingFilter.OnActionExecuted(_actionExecutedContext);

            _logger.Verify(
             x => x.Log(
                 It.IsAny<LogLevel>(),
                 It.IsAny<EventId>(),
                 It.Is<It.IsAnyType>((v, t) => true),
                 It.IsAny<Exception>(),
                 It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }
    }
}
