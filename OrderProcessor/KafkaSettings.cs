using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor
{
    public class KafkaSettings
    {
        public string Server { get; set; }
        public int NumPartitions { get; set; }
        public short ReplicationFactor { get; set; }
    }
}
