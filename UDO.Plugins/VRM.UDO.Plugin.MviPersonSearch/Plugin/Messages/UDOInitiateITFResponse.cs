using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRM.Integration.UDO.MVI.Messages
{


    public class UDOInitiateITFResponse
    {

        public string parameter { get; set; }

        public Guid udo_veteranId { get; set; }

        public string ErrorMessage { get; set; }

        public bool ExceptionOccured { get; set; }
    }

}