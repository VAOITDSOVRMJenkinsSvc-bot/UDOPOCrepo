using System;


namespace VRM.Integration.UDO.Notes.Messages
{
    public class UDODeleteNoteResponse 
    {
        
        public bool ExceptionOccured { get; set; }
        
        public string ExceptionMessage { get; set; }
        
        public UDODeleteNoteResponseInfo UDODeleteNoteInfo { get; set; }
    }

    public class UDODeleteNoteResponseInfo
    {
       
        public string udo_ClaimId { get; set; }
       
        public string udo_DateTime { get; set; }
       
        public string udo_Note { get; set; }
     
        public string udo_ParticipantID { get; set; }
       
        public string udo_RO { get; set; }
       
        public DateTime udo_SuspenseDate { get; set; }
      
        public string udo_Type { get; set; }
        
        public string udo_User { get; set; }
    }
}
