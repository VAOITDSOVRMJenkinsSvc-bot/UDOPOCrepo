using System;
using Microsoft.Xrm.Sdk;

namespace VRM.Integration.UDO.UserTool.Messages
{
    public class UDOSecurityRequest
    {
        public string MessageId { get; set; }
        public string OrganizationName { get; set; }
        public Guid UserId { get; set; }
        public bool LogTiming { get; set; }
        public bool LogSoap { get; set; }
        public bool Debug { get; set; }
        public string Relationship { get; set; }
        public EntityReference One { get; set; }
        public EntityReference[] Many { get; set; }
    }
}