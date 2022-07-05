using System;
using UDO.LOB.Core;

namespace VRM.Integration.UDO.MVI.Messages
{


    public class UDOInitiateLettersRequest
    {

        public string MessageId { get; set; }
        public string OrganizationName { get; set; }

        public Guid UserId { get; set; }

        public Guid RelatedParentId { get; set; }

        public string RelatedParentEntityName { get; set; }

        public string RelatedParentFieldName { get; set; }

        public bool LogTiming { get; set; }

        public bool LogSoap { get; set; }

        public bool Debug { get; set; }
        public Guid udo_vetsnapshotId { get; set; }

        public Guid udo_veteranId { get; set; }
        public Guid udo_personId { get; set; }

        public string fileNumber { get; set; }

        public string SSN { get; set; }
        

        public Int64 ptcpntId { get; set; }

        public string vetfileNumber { get; set; }

        public string vetSSN { get; set; }


        public Int64 vetptcpntId { get; set; }

        public string vetFirstName { get; set; }
        public string vetLastName { get; set; }
        public string vetMiddleInitial { get; set; }
        public string vetDOB { get; set; }
        public string vetGender { get; set; }

        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        public Guid? OwnerId { get; set; }
        public string OwnerType { get; set; }
        public UDORelatedEntity[] RelatedEntities { get; set; }
    }
}