using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using Microsoft.Xrm.Sdk;

namespace UDO.LOB.UserTool.Messages
{
    [Serializable]
    [DataContract]
    public class UDOSecurityRequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        [DataMember]
        public string Relationship { get; set; }
        [DataMember]
        public EntityReference One { get; set; }
        [DataMember]
        public EntityReference[] Many { get; set; }
    }
}