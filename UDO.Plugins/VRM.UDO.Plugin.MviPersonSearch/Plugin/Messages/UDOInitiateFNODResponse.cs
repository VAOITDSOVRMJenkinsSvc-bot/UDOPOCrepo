using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRM.Integration.UDO.MVI.Messages
{
    public class UDOInitiateFNODResponse
    {
        public Guid newUDOInitiateFNODId { get; set; }
        public UDOInitiateFNODException UDOInitiateFNODExceptions { get; set; }
    }

    public class UDOInitiateFNODException
    {
        public bool ExceptionOccured { get; set; }
        public string ExceptionMessage { get; set; }
    }
}
