using System;


namespace VRM.Integration.UDO.CADD.Messages
{
    
    public class UDOFindBankResponse 
    {
       
        public string bankName { get; set; }
       
        public UDOFindBankExceptions UDOFindBankExceptions { get; set; }
    }
    
   
    public class UDOFindBankExceptions
    {
       
        public bool AppealExceptionOccured { get; set; }
      
        public string AppealExceptionMessage { get; set; }
        
        public bool ClaimantExceptionOccured { get; set; }
       
        public string ClaimantExceptionMessage { get; set; }
       
        public bool VeteranExceptionOccured { get; set; }
        
        public string VeteranExceptionMessage { get; set; }
        
        public bool PaymentExceptionOccured { get; set; }
       
        public string PaymentExceptionMessage { get; set; }
        
        public bool DdeftExceptionOccured { get; set; }
       
        public string DdeftExceptionMessage { get; set; }
    }
}