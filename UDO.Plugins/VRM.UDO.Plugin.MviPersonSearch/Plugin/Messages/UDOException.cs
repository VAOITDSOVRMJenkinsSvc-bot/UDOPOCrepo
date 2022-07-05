using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRM.Integration.UDO.MVI.Messages
{
    public class UDOException
    {
        public string ExceptionCategory { get; set; }
        public string ExceptionMessage { get; set; }
        public bool ExceptionOccured { get; set; }
    }
}