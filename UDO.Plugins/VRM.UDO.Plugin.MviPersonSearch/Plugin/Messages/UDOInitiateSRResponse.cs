using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRM.Integration.UDO.MVI.Messages
{
    public class UDOInitiateSRResponse
    {
        public bool ExceptionOccured { get; set; }
        public string ExceptionMessage { get; set; }
        public string UDOServiceRequestId { get; set; }
    }
}