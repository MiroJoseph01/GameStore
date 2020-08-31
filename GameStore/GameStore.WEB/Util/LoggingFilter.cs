
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace GameStore.Web.Filters
{
    public class LoggingFilter : IActionFilter
    {
        private readonly ILogger<LoggingFilter> _logger;

        public LoggingFilter(ILogger<LoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation(
                $" Go to {context.HttpContext.Request.Method}: " +
                $"\'{context.HttpContext.Request.Path}\'. " +
                $"User id: " +
                $"\'{context.HttpContext.Connection.RemoteIpAddress}\'");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation(
                $"{context.HttpContext.Request.Method}: " +
                $"\'{context.HttpContext.Request.Path}\' executed");
        }
    }
}
