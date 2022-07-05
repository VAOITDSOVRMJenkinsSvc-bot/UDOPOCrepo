using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRM.Integration.UDO.MVI.Messages
{
    public class UDOInitiateCADDResponse
    {
        public Guid CADDId { get; set; }
        public string ExceptionMessage { get; set; }
        public bool ExceptionOccured { get; set; }
        public UDOException[] InnerExceptions { get; set; }
    }    
}