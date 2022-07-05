using System;

namespace VRM.Integration.UDO.MVI.Messages
{
    public class UDOgetVBMSDocumentContentResponse 
    {
        public bool ExceptionOccured { get; set; }
        public string ExceptionMessage { get; set; }
        public Guid udo_attachmentId { get; set; }
    }

   
}