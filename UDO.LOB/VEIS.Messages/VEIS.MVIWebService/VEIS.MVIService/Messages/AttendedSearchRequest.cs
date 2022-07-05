using System;

using System.Runtime.Serialization;
using System.Linq;
using VEIS.Core.Core;

namespace VEIS.Mvi.Messages
{

    [DataContract]
    public class AttendedSearchRequest : MessageBase
    {
        private string _birthDate;

        public AttendedSearchRequest()
        {
        }

        /// <summary>
        /// Constructor: AttendedSearchRequest
        /// </summary>
        /// <param name="firstName">Veteran's first name</param>
        /// <param name="middleName">Veteran's middle name</param>
        /// <param name="lastName">Veteran's last name</param>
        /// <param name="ssn">Veteran's social Security Number</param>
        /// <param name="dob">Veteran's Date of Birth</param>
        /// <param name="phoneNo">Veteran's Phone number</param>
        /// <param name="userId">User's first name</param>
        /// <param name="userFirstName">User's middle name</param>
        /// <param name="userLastName">User's last name</param>
        /// <param name="organization">CRM org name</param>
        /// <param name="messageId">Message id</param>
        public AttendedSearchRequest(string firstName, string middleName, string lastName, string ssn, string dob, string phoneNo, Guid userId, string userFirstName, string userLastName, string organization, string messageId)
        {
            FirstName = firstName;
            MiddleName = middleName;
            FamilyName = lastName;
            SocialSecurityNumber = ssn;
            BirthDate = FormatBirthDate(dob);
            PhoneNumber = phoneNo;
            UserId = userId;
            UserFirstName = userFirstName;
            UserLastName = userLastName;            
            OrganizationName = organization;
            MessageId = messageId;           
            IsAttended = true;
            SearchUse = "L";
        }        

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public string FamilyName { get; set; }

        [DataMember]
        public string Edipi { get; set; }

        [DataMember]
        public string SocialSecurityNumber { get; set; }

        [DataMember]
        public string BirthDate 
        {
            get
            {
                return FormatBirthDate(_birthDate);
            }
            set
            {
                _birthDate = value;
            }
        }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string SearchUse { get; set; }

        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public string UserFirstName { get; set; }

        [DataMember]
        public string UserLastName { get; set; }

        [DataMember]
        public string OrganizationName { get; set; }

        /// <summary>
        /// Gets or sets whether the search should be treated as an Attended search. This overrides the unattended search functionalities.
        /// </summary>
        [DataMember]
        public bool IsAttended { get; set; }

        //[DataMember]
        //public MessageProcessType FetchMessageProcessType { get; set; }

        [DataMember(IsRequired = false)]
        public bool LogTiming { get; set; }

        [DataMember(IsRequired=false)]
        public bool LogSoap { get; set; }

        private string FormatBirthDate(string date)
        {
            try 
	        {	        
                if(string.IsNullOrEmpty(date))
                    return "";

                if(date.Contains("/") || date.Contains("-"))
                {
                    var dateTime = Convert.ToDateTime(date);
                    return string.Format("{0:yyyyMMdd}", dateTime);
                }

                return date;
	        }
	        catch (Exception)
	        {
		        return date;
	        }           
        }
    }
}
