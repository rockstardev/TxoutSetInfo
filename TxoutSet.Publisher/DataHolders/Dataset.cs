using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TxoutSet.Common;

namespace TxoutSet.Publisher.DataHolders
{
    public class Dataset
    {
        public Dataset(string source, TxoutSetInfo set)
        {
            Consensus = new List<string> { source };
            Set = set;
        }

        public List<string> Consensus { get; set; }
        public TxoutSetInfo Set { get; set; }

        public void AddSource(string source)
        {
            Consensus.Add(source);
        }
    }
}
