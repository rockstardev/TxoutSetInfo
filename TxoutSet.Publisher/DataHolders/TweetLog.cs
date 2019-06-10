using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TxoutSet.Publisher.DataHolders
{
    public interface ITweetLog
    {
        void Log(int height, string message);
    }

    public class TweetLog : ITweetLog
    {
        private readonly ILogger _logger;

        public TweetLog(ILogger logger)
        {
            _logger = logger;
        }

        public void Log(int height, string message)
        {
            _logger.Info(message);
        }
    }
}
