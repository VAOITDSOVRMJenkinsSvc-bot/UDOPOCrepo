using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRM.Integration.UDO.Contact.Messages;
using System.Runtime.Serialization;

namespace VRM.Integration.UDO.Contact.Messages
{
    public class UDOupdateHasBenefitsRequest
    {
        public string MessageId { get; set; }
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        public string Edipi { get; set; }
        public string ContactId { get; set; }
        public Guid UserId { get; set; }
        public string OrganizationName { get; set; }
        public bool Debug { get; set; }
        public Guid RelatedParentId { get; set; }
        public string RelatedParentEntityName { get; set; }
        public string RelatedParentFieldName { get; set; }
        public bool LogTiming { get; set; }
        public bool LogSoap { get; set; }

    }
    public class UDOupdateHasBenefitsResponse
    {
        public bool ExceptionOccurred { get; set; }
        public string ExceptionMessage { get; set; }
        public string Edipi { get; set; }
    }

    public class HeaderInfo
    {
        public string StationNumber { get; set; }
        public string LoginName { get; set; }
        public string ApplicationName { get; set; }
        public string ClientMachine { get; set; }
    }
}
