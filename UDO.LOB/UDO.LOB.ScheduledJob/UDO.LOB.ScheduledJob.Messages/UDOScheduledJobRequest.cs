using System;
using UDO.LOB.Core;
using System.Runtime.Serialization;


namespace UDO.LOB.ScheduledJob.Messages
{
    [DataContract]
    public class UDOScheduledJobRequest : UDORequestBase
    {
        [DataMember]
        public string JobName { get; set; }
        //[DataMember]
        //public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserID { get; set; }
        //[DataMember]
        //public bool Debug { get; set; }
    }
}