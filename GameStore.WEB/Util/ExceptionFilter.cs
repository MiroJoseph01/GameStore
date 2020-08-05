using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace GameStore.WEB.Util
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError($"Exception in method {context.ActionDescriptor.DisplayName}. " +
                $"Exception messsage: {context.Exception.Message}\n" +
                $"Trace: {context.Exception.StackTrace}");

            context.ExceptionHandled = true;
        }
    }
}
