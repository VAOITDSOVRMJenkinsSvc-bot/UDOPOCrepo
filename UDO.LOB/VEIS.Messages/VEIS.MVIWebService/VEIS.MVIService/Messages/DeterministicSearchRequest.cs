using System;
using System.Runtime.Serialization;
using VEIS.Core.Core;

namespace VEIS.Mvi.Messages
{
    public enum DeterministicSearchType
    {
        SocialSecurity, Edipi
    }

    [DataContract]
    public class DeterministicSearchRequest : MessageBase
    {
        public DeterministicSearchRequest()
        {

        }

        public DeterministicSearchRequest(string searchId, DeterministicSearchType searchType, string birthDate, Guid userId, string userFirstName, string userLastName, string organization)
        {
            switch (searchType)
            {
                case DeterministicSearchType.SocialSecurity:
                    SocialSecurityNumber = searchId;
                    break;
                case DeterministicSearchType.Edipi:
                    EdiPi = searchId;
                    break;
                default:
                    break;
            }
            
            BirthDate = birthDate;
            UserId = userId;
            UserFirstName = userFirstName;
            UserLastName = userLastName;
            OrganizationName = organization;
            SearchType = searchType;
        }

        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public string UserFirstName { get; set; }

        [DataMember]
        public string UserLastName { get; set; }

        [DataMember]
        public string OrganizationName { get; set; }

        [DataMember]
        public string BirthDate { get; set; }

        [DataMember]
        public string SocialSecurityNumber { get; set; }

        [DataMember(IsRequired = false)]
        public bool LogTiming { get; set; }

        [DataMember(IsRequired = false)]
        public bool LogSoap { get; set; }
        public string EdiPi { get; set; }

        public DeterministicSearchType SearchType { get; set; }

    }
}
