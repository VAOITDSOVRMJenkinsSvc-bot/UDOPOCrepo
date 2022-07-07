using System;
using System.Runtime.Serialization;

namespace UDO.LOB.UserTool.Messages
{
    [DataContract]
    public class UDOSecurityDisassocRequest : UDOSecurityRequest
    {
        //[DataMember]
        //public Guid RelatedParentId { get; set; }
        //[DataMember]
        //public string RelatedParentEntityName { get; set; }
        //[DataMember]
        //public string RelatedParentFieldName { get; set; }
    }
}