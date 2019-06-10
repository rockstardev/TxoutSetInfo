using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TxoutSet.Publisher.DataHolders;

namespace TxoutSet.Tests.Mocks
{
    public class TweetLogMock : ITweetLog
    {
        public static Dictionary<int, List<string>> LogDict { get; set; } = new Dictionary<int, List<string>>();

        public void Log(int height, string message)
        {
            if (!LogDict.ContainsKey(height))
                LogDict.Add(height, new List<string>());

            LogDict[height].Add(message);
        }
    }
}