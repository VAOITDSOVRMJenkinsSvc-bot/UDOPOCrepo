using System;

using System.Runtime.Serialization;
using VEIS.Core.Core;

namespace VEIS.Mvi.Messages
{

    [DataContract]
    public class CorrespondingIdsResponse : MessageBase
    {
        [DataMember]
        public UnattendedSearchRequest[] CorrespondingIdList { get; set; }

        [DataMember]
        public UnattendedSearchRequest[] ReplacementIdList { get; set; }

        [DataMember]
        public bool ExceptionOccured { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string RawMviExceptionMessage { get; set; }

        [DataMember]
        public string OrganizationName { get; set; }

        [DataMember]
        public string Url { get; set; }

        //[DataMember]
        //public MessageProcessType FetchMessageProcessType { get; set; }

        [DataMember]
        public string SocialSecurityNumber { get; set; }

        [DataMember]
        public string Edipi { get; set; }

        [DataMember]
        public string ParticipantId { get; set; }

        [DataMember]
        public int? StationId { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public string FamilyName { get; set; }

        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public string FullAddress { get; set; }
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string DateofBirth { get; set; }

        public string RecordSource { get; set; }

        /// <summary>
        /// Sets or get the MVI Acknowlegment details that contain MVIerror codes and texts.
        /// </summary>
        [DataMember]
        public Acknowledgement Acknowledgement { get; set; }

        [DataMember]
        public QueryAcknowledgement QueryAcknowledgement { get; set; }

    }
}
