using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Common.Messages;

namespace UDO.LOB.ClaimEstablishment.Messages
{
    [Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOInitiateClaimEstablishmentRequest)]
    [DataContract]
    public class UDOInitiateClaimEstablishmentRequest : UDORequestBase
    {
        [DataMember]
        public string fileNumber { get; set; }
        [DataMember]
        public string awardtypecode { get; set; }
        [DataMember]
        public string vetfileNumber { get; set; }
        [DataMember]
        public string SSN { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public Int64 ptcpntId { get; set; }
        [DataMember]
        public Int64 vetptcpntId { get; set; }
        [DataMember]
        public string address1 { get; set; }
        [DataMember]
        public string address2 { get; set; }
        [DataMember]
        public string address3 { get; set; }
        [DataMember]
        public string city { get; set; }
        [DataMember]
        public string state { get; set; }
        [DataMember]
        public string postalcode { get; set; }
        [DataMember]
        public string PayeeCode { get; set; }
        [DataMember]
        public Guid udo_payeecodeid { get; set; }
        [DataMember]
        public Guid udo_personid { get; set; }
        [DataMember]
        public Guid udo_idproofid { get; set; }
        [DataMember]
        public Guid udo_veteranid { get; set; }
        [DataMember]
        public Guid udo_veteransnapshotid { get; set; }
        [DataMember]
        public Guid udo_interaction { get; set; }
    }
}
