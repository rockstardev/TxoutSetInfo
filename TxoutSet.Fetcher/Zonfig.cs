using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace TxoutSet.Fetcher
{
    // Loaded from zonfig.json
    public class Zonfig
    {
        public string Filename { get; set; }
        public string Arguments { get; set; }
        public string PublisherUrl { get; set; }
        public string PublisherApiKey { get; set; }
        public bool ReadlineAtExit { get; set; }

        public string BitcoindUri { get; set; }
        public string BitcoindCred { get; set; }
        public string BitcoindNetwork { get; set; }
    }
}
