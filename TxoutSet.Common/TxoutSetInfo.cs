using System;

namespace TxoutSet.Common
{
    // preserving separate class that's similar to cli output for optional compatiblity down the road
    public class TxoutSetInfo
    {
        public int height { get; set; }
        public string bestblock { get; set; }
        public long transactions { get; set; }
        public long txouts { get; set; }
        //public long bogosize { get; set; }
        public string hash_serialized_2 { get; set; }
        //public long disk_size { get; set; }
        public decimal total_amount { get; set; }
    }

    /*
{
  "height": 569685,
  "bestblock": "0000000000000000000016bf6fe5158d3aaccf13c59b3b672441d5f24d166b1f",
  "transactions": 28556514,
  "txouts": 52495159,
  "bogosize": 3_956_586_169,
  "hash_serialized_2": "009235c3c774d403ba91b72c8998b4fbe148f601617d6de6adb11664a7b5d565",
  "disk_size": 3093118545,
  "total_amount": 17620892.32664461
}
    */
}
