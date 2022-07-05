using System;

namespace VRM.Integration.UDO.MVI.Messages
{
    public class getUDOClaimDocumentsResponse
    {
        public bool ExceptionOccured { get; set; }
        public string ExceptionMessage { get; set; }
        public getUDOClaimDocumentsResponseData[] getUDOClaimDocumentsResponseInfo { get; set; }
    }

    public class getUDOClaimDocumentsResponseData
    {
        public Guid udo_attachmentId { get; set; }
        public string udo_lettertxt { get; set; }
    }
}
