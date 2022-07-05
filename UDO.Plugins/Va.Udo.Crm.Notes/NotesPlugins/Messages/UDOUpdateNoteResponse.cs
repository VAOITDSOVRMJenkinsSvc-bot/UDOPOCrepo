using System;


namespace VRM.Integration.UDO.Notes.Messages
{
    public class UDOUpdateNoteResponse 
    {
        
        public bool ExceptionOccured { get; set; }
        
        public string ExceptionMessage { get; set; }
        
        public UDOUpdateNoteResponseInfo UDOUpdateNoteInfo { get; set; }
    }

    public class UDOUpdateNoteResponseInfo
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
