using System;

using System.Runtime.Serialization;
using VEIS.Core.Core;

namespace VEIS.Mvi.Messages
{

    [DataContract]
    public class AddPersonRequest : MessageBase
    {
        public AddPersonRequest()
        {
        }

        public AddPersonRequest(Guid userId, string userFirstName, string userLastName, string organization, string messageId, ProcessingType processingType = ProcessingType.Test)
        {
            UserId = userId;
            UserFirstName = userFirstName;
            UserLastName = userLastName;
            OrganizationName = organization;
            MessageId = messageId;
            ProcessingCode = processingType;
        }

        [DataMember]
        public ProcessingType ProcessingCode { get; set; }

        [DataMember]
        public string OrganizationName { get; set; }

        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public string UserFirstName { get; set; }

        [DataMember]
        public string UserLastName { get; set; }

        [DataMember]
        public AddSubject1 Subject1 { get; set; }

        [DataMember(IsRequired = false)]
        public bool LogTiming { get; set; }

        [DataMember(IsRequired = false)]
        public bool LogSoap { get; set; }
    }
}
