using System;

namespace GameStore.Web.Util.Logger
{
    public interface IAppLogger<T>
    {
        void LogDebug(string message);

        void LogInfo(string message);

        void LogWarning(string message);

        void LogWarning(string message, Exception e);

        void LogError(string message);

        void LogError(string message, Exception e);
    }
}
