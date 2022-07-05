using System;


namespace VRM.Integration.UDO.Notes.Messages
{
    public class UDOCreateNoteResponse 
    {
        
        public bool ExceptionOccured { get; set; }
        
        public string ExceptionMessage { get; set; }
        
        public UDOCreateNoteResponseInfo UDOCreateNoteInfo { get; set; }
    }
   
    public class UDOCreateNoteResponseInfo
    {
       
        public string udo_ClaimId { get; set; }
       
        public string udo_DateTime { get; set; }
       
        public string udo_Note { get; set; }
     
        public string udo_ParticipantID { get; set; }
       
        public string udo_RO { get; set; }
       
        public string udo_SuspenseDate { get; set; }
      
        public string udo_Type { get; set; }
        
        public string udo_User { get; set; }

        public string udo_UserId { get; set; }

        public string udo_legacynoteid { get; set; }
    }
}
