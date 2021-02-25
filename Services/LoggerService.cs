using BookStore_API.Contracts;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Services
{
    public class LoggerService : ILoggerService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public void Logdebug(string message)
        {
            logger.Debug(message);
        }

        public void Logerror(string message)
        {
            logger.Error(message);
        }

        public void LogInfo(string message)
        {
            logger.Info(message);
        }

        public void Logwarn(string message)
        {
            logger.Warn(message);
        }
    }
}
