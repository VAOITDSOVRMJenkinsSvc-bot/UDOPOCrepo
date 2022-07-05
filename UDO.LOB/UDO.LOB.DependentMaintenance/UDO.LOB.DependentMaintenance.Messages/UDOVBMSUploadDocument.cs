using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VRM.Integration.Servicebus.Core;

namespace UDO.LOB.DependentMaintenance.Messages
{
    public class UDOVBMSUploadDocumentRequest : MessageBase
    {
        public string OrganizationName { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid UserId { get; set; }
        public Guid RelatedParentId { get; set; }
        public string RelatedParentEntityName { get; set; }
        public string RelatedParentFieldName { get; set; }
        public bool LogTiming { get; set; }
        public bool LogSoap { get; set; }
        public bool Debug { get; set; }
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        public string udo_filenumber { get; set; }
        public string udo_vet_firstname { get; set; }
        public string udo_vet_middlename { get; set; }
        public string udo_vet_lastname { get; set; }
        public string udo_base64filecontents { get; set; }
        public string udo_filename { get; set; }
        public string udo_claimnumber { get; set; }
        public EntityReference udo_vbmsdocument { get; set; }
        public EntityReference udo_relatedentity { get; set; }
        public string udo_edipi { get; set; }
        public string udo_ssid { get; set; }
        public Guid? udo_doctypeid { get; set; }
        public string udo_source { get; set; }
        public string udo_subject { get; set; }
        public string udo_userName { get; set; }
        public string udo_userRole { get; set; }
    }

    public class UDOVBMSUploadDocumentResponse : MessageBase
    {
        public bool Processing { get; set; }
        public string ExceptionMessage { get; set; }
        public bool ExceptionOccured { get; set; }
    }

    public class HeaderInfo
    {
        public string StationNumber { get; set; }
        public string LoginName { get; set; }
        public string ApplicationName { get; set; }
        public string ClientMachine { get; set; }
    }
}
