using System;
using UDO.LOB.Core;
using System.Runtime.Serialization;

namespace UDO.LOB.ScheduledJob.Messages
{
    [DataContract]
    public class UDOScheduledJobResponse : MessageBase
    {
        [DataMember]
        public bool Received { get; set; }
    }
}