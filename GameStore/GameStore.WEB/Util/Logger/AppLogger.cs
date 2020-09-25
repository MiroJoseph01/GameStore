using System;
using Microsoft.Extensions.Logging;

namespace GameStore.Web.Util.Logger
{
    public class AppLogger<T> : IAppLogger<T>
    {
        private readonly ILogger<T> _logger;

        public AppLogger(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogDebug(string message)
        {
            _logger.LogDebug(message);
        }

        public void LogInfo(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogWarning(string message, Exception e)
        {
            _logger.LogWarning(e, message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public void LogError(string message, Exception e)
        {
            _logger.LogError(e, message);
        }

        public void LogError(string message)
        {
            _logger.LogError(message);
        }
    }
}
