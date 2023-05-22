
namespace ServiceGuard.AppLibs {

    public static class LoggerExtensions {

        public static bool LogInfoAndReturn(this ILogger logger, bool result, string logStr) {
            logger.LogInformation(logStr);
            return result;
        }

    }

}
