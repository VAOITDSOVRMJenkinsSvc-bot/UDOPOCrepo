using System;

namespace VRM.Integration.UDO.ServiceRequest.Messages
{
    public class UDOUpdateSRResponse
    {
        public bool ExceptionOccured { get; set; }
        public string ExceptionMessage { get; set; }
    }
}