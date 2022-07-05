using Microsoft.Xrm.Sdk;
using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;

namespace UDO.LOB.VBMS.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOVBMSUploadDocumentRequest)]
    [DataContract]
    public class UDOVBMSUploadDocumentRequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }

        [DataMember]
        public Guid OrganizationId { get; set; }

        [DataMember]
        public Guid UserId { get; set; }

        //[DataMember]
        //public Guid RelatedParentId { get; set; }

        //[DataMember]
        //public string RelatedParentEntityName { get; set; }

        //[DataMember]
        //public string RelatedParentFieldName { get; set; }

        [DataMember]
        public bool LogTiming { get; set; }

        [DataMember]
        public bool LogSoap { get; set; }

        [DataMember]
        public bool Debug { get; set; }

        [DataMember]
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }

        [DataMember]
        public string udo_filenumber { get; set; }
        [DataMember]
        public string udo_vet_firstname { get; set; }
        [DataMember(IsRequired = false)]
        public string udo_vet_middlename { get; set; }
        [DataMember]
        public string udo_vet_lastname { get; set; }
        [DataMember]
        public string udo_base64filecontents { get; set; }
        [DataMember]
        public string udo_filename { get; set; }
        [DataMember]
        public string udo_claimnumber { get; set; }
        // REM: EntityReference - added using Xrm
        [DataMember]
        public EntityReference udo_vbmsdocument { get; set; }
        [DataMember]
        public EntityReference udo_relatedentity { get; set; }

        [DataMember(IsRequired = false)]
        public string udo_edipi { get; set; }

        [DataMember(IsRequired = false)]
        public string udo_ssid { get; set; }

        [DataMember]
        public Guid? udo_doctypeid { get; set; }

        [DataMember(IsRequired = false)]
        public string udo_source { get; set; }

        [DataMember(IsRequired = false)]
        public string udo_subject { get; set; }

        [DataMember(IsRequired = false)]
        public string udo_userName { get; set; }

        [DataMember(IsRequired = false)]
        public string udo_userRole { get; set; }
       
    }
}
