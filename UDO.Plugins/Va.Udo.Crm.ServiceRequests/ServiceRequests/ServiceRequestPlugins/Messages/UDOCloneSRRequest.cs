using System;

namespace VRM.Integration.UDO.ServiceRequest.Messages
{
    public class UDOCloneSRRequest
    {
        public string MessageId { get; set; }
        public string OrganizationName { get; set; }
        public Guid UserId { get; set; }
        public bool Debug { get; set; }
        public Guid udo_ServiceRequestId { get; set; }
    }
}