using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UDO.VASS.POC.Plugins.Entities
{
    public class UDOHeader
    {
        public string StationNumber { get; set; }
        public string LoginName { get; set; }
        public string ApplicationName { get; set; }
        public string ClientMachine { get; set; }
        public string udo_PcrSsn { get; set; }
    }
}
