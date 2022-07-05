using System;

namespace VRM.Integration.UDO.ServiceRequest.Messages
{
    public class UDOCloneSRResponse
    {
        public bool ExceptionOccured { get; set; }
        public string ExceptionMessage { get; set; }
        public Guid udo_ServiceRequestId { get; set; }
    }
}