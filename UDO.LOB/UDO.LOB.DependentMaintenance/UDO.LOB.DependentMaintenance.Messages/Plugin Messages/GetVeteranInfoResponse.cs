using System;

using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
//using VRM.Integration.Servicebus.Core;


//CSDEv
//namespace VRM.Integration.Servicebus.Bgs.Messages
namespace UDO.LOB.DependentMaintenance.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.GetBGSVeteranInfoResponse)]
    [DataContract]
    public class GetVeteranInfoResponse : MessageBase
    {
        [DataMember]
        public GetVeteranInfoMultipleResponse[] GetVeteranInfo { get; set; }
        [DataMember]
        public string Fault { get; set; }
        [DataMember]
        public string SoapLog { get; set; }

    }
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.GetBGSVeteranInfoResponse)]
    [DataContract]
    public class GetVeteranInfoMultipleResponse : MessageBase
    {
        [DataMember]
        public string crme_ZIP { get; set; }
        [DataMember]
        public string crme_VAFileNumber { get; set; }
        [DataMember]
        public string crme_StoredSSN { get; set; }
        [DataMember]
        public string crme_State { get; set; }
        [DataMember]
        public string crme_SSN { get; set; }
        [DataMember]
        public string crme_SecondaryPhone { get; set; }
        [DataMember]
        public string crme_PrimaryPhone { get; set; }
        [DataMember]
        public string crme_ParticipantID { get; set; }
        [DataMember]
        public string crme_MiddleName { get; set; }
        [DataMember]
        public string crme_LastName { get; set; }
        [DataMember]
        public string crme_FirstName { get; set; }
        [DataMember]
        public string crme_Email { get; set; }
        [DataMember]
        public string crme_EDIP { get; set; }
        [DataMember]
        public string crme_DOB { get; set; }
        [DataMember]
        public bool crme_DataFromApplication { get; set; }
        [DataMember]
        public string crme_Country { get; set; }
        [DataMember]
        public string crme_City { get; set; }
        [DataMember]
        public string crme_Address3 { get; set; }
        [DataMember]
        public string crme_Address2 { get; set; }
        [DataMember]
        public string crme_Address1 { get; set; }
        [DataMember]
        public string crme_AddressType { get; set; }
        [DataMember]
        public string crme_AllowPOAAccess { get; set; }
        [DataMember]
        public string crme_AllowPOACADD { get; set; }
        [DataMember]
        public string crme_beneficiarydateofbirth { get; set; }
        [DataMember]
        public string crme_DayTimeAreaCode { get; set; }
        [DataMember]
        public string crme_NightTimeAreaCode { get; set; }
        [DataMember]
        public string crme_ZipPlus4 { get; set; }
        [DataMember]
        public string crme_SuffixName { get; set; }
        [DataMember]
        public string crme_Title { get; set; }
    }
}
