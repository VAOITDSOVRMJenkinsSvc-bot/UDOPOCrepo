using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;

using VRM.Integration.Servicebus.Core;

namespace VRM.Integration.Servicebus.Bgs.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.GetMaritalInfoResponse)]
    [DataContract]
    public class GetMaritalInfoResponse : MessageBase
    {
        [DataMember]
        public GetMaritalInfoMultipleResponse[] GetMaritalInfo { get; set; }
        [DataMember]
        public string Fault { get; set; }
        [DataMember]
        public string SoapLog { get; set; }

    }
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.GetMaritalInfoResponse)]
    [DataContract]
    public class GetMaritalInfoMultipleResponse : MessageBase
    {
        [DataMember]
        public string crme_State { get; set; }
        [DataMember]
        public string crme_SpouseSSN { get; set; }
        [DataMember]
        public DateTime crme_MarriageStartDate { get; set; }
        [DataMember]
        public DateTime crme_MarriageEndDate { get; set; }
        [DataMember]
        public string crme_LastName { get; set; }
        [DataMember]
        public string crme_FirstName { get; set; }
        [DataMember]
        public DateTime? crme_DOB { get; set; }
        [DataMember]
        public string crme_Country { get; set; }
        [DataMember]
        public string crme_City { get; set; }
        [DataMember]
        public DateTime? crme_RelationshipBeginDate { get; set; }
        [DataMember]
        public DateTime? crme_RelationshipEndDate { get; set; }
        [DataMember]
        public string crme_AwardInd { get; set; }
    }
}
