using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TxoutSet.Publisher
{
    public class Zonfig
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string UserAccessToken { get; set; }
        public string UserAccessSecret { get; set; }

        public List<ApiKey> ApiKeys { get; set; }

        public string ApiKeysStr
        {
            get { return ""; }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    return;

                var keys = new List<ApiKey>();
                var split = value.Split(',');
                foreach (var s in split)
                {
                    var nameKey = s.Split('|');
                    keys.Add(new ApiKey { Name = nameKey[0], Key = nameKey[1] });
                }

                ApiKeys = keys;
            }
        }
    }

    public class ApiKey
    {
        public string Name { get; set; }
        public string Key { get; set; }
    }
}
