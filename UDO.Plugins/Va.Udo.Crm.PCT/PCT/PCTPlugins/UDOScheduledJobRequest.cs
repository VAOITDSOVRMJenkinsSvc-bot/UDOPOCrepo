using System;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;

namespace VRM.Integration.UDO.ScheduledJob.Messages
{
    public class UDOScheduledJobRequest
    {
        public string MessageId { get; set; }
        public string OrganizationName { get; set; }
        public Guid UserId { get; set; }
        public bool Debug { get; set; }
        public string JobName { get; set; }
    }

    public class UDOScheduledJobResponse
    {
        public bool Received { get; set; }
    }
}