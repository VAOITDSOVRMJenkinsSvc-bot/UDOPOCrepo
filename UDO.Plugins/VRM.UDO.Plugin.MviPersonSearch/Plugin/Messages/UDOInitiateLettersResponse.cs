using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRM.Integration.UDO.MVI.Messages
{


    public class UDOInitiateLettersResponse
    {

        public Guid newUDOInitiateLetterId { get; set; }

        public UDOInitiateLettersExceptions UDOInitiateLettersExceptions { get; set; }
    }



    public class UDOInitiateLettersExceptions
    {
        
      
        
        public bool VeteranExceptionOccured { get; set; }
        
        public string VeteranExceptionMessage { get; set; }
        
       
       
    }
}