using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRM.Integration.UDO.Notes.Messages
{
    public class UDORetrieveNotesResponse
    {
        public bool ExceptionOccured { get; set; }
        public string ExceptionMessage { get; set; }
        public int RecordCount { get; set; }
    }
}
