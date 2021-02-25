using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Contracts
{
    public interface ILoggerService
    {
        void LogInfo(string message);
        void Logwarn(string message);
        void Logdebug(string message);
        void Logerror(string message);
    }
}
