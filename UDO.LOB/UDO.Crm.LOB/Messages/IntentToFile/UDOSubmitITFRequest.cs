using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

namespace VRM.Integration.UDO.IntentToFile.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOSubmitITFRequest)]
    [DataContract]
    public class UDOSubmitITFRequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public Guid RelatedParentId { get; set; }
        [DataMember]
        public string RelatedParentEntityName { get; set; }
        [DataMember]
        public string RelatedParentFieldName { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        [DataMember]
        public Claimant Claimant { get; set; }
        [DataMember]
        public Guid va_intenttofileId { get; set; }
        [DataMember]
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
    }

    [DataContract]
    public class Claimant
    {
        [DataMember]
        public string ClaimantParticipantId { get; set; }
        [DataMember]
        public string VeteranParticipantId { get; set; }
        [DataMember]
        public string CompensationType { get; set; }
        [DataMember]
        public string VeteranFirstName { get; set; }
        [DataMember]
        public string VeteranLastName { get; set; }
        [DataMember]
        public string VeteranMiddleInitial { get; set; }
        [DataMember]
        public string VeteranSsn { get; set; }
        [DataMember]
        public string VeteranFileNumber { get; set; }
        [DataMember]
        public string VeteranBirthDate { get; set; }
        [DataMember]
        public string VeteranGender { get; set; }
        [DataMember]
        public string ClaimantFirstName { get; set; }
        [DataMember]
        public string ClaimantLastName { get; set; }
        [DataMember]
        public string ClaimantMiddleInitial { get; set; }
        [DataMember]
        public string ClaimantSsn { get; set; }
        [DataMember]
        public string PhoneAreaCode { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string AddressLine1 { get; set; }
        [DataMember]
        public string AddressLine2 { get; set; }
        [DataMember]
        public string AddressLine3 { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string Zip { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public string StationLocation { get; set; }
    }

    [DataContract]
    public class BgsConfig
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string StationId { get; set; }
        [DataMember]
        public string ApplicationName { get; set; }
        [DataMember]
        public string BepServiceUrl { get; set; }
        [DataMember]
        public string ThumbPrint { get; set; }
    }
}
