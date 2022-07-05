using System;
using System.Runtime.Serialization;

namespace UDO.LOB.DependentMaintenance.Messages
{
    [DataContract]
    public class Participant : IParticipant
    {
        [DataMember]
        public string FileNumber { get; set; }
        [DataMember]
        public string MaintenanceType { get; set; }
        [DataMember]
        public string Ssn { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string MiddleName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public DateTime BirthDate { get; set; }
        [DataMember]
        public string BirthCityName { get; set; }
        [DataMember]
        public string BirthStateCode { get; set; }
        [DataMember]
        public string BirthCountryName { get; set; }
        [DataMember]
        public bool IsVetInd { get; set; }
        [DataMember]
        public bool IsSeriouslyDisabled { get; set; }
        [DataMember]
        public bool IsScholdChild { get; set; }
        [DataMember]
        public Address[] Addresses { get; set; }
        [DataMember]
        public PhoneNumber[] PhoneNumbers { get; set; }
        [DataMember]
        public string AddresssType { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public string postalCodePlus4 { get; set; }
        [DataMember]
        public string SuffixName { get; set; }
        [DataMember]
        public string TitleName { get; set; }
        [DataMember]
        public string NoSssnReasonTypeCd { get; set; }
        [DataMember]
        public string CourseName { get; set; }
        [DataMember]
        public string SchoolCode { get; set; }
        [DataMember]
        public string PrevSchoolCode { get; set; }
        [DataMember]
        public string SchoolAddressLine1 { get; set; }
        [DataMember]
        public string SchoolName { get; set; }
        [DataMember]
        public string SchoolAddressCity { get; set; }
        [DataMember]
        public string SchoolAddressState { get; set; }
        [DataMember]
        public string SchoolAddressZip { get; set; }
        [DataMember]
        public DateTime ExpectedStartDate { get; set; }
        [DataMember]
        public DateTime ExpectedGradDate { get; set; }
        [DataMember]
        public DateTime CourseBeginDate { get; set; }
        [DataMember]
        public bool IsAttendedLastTerm { get; set; }
        [DataMember]
        public string FullTimeStudentTypeCode { get; set; }
        [DataMember]
        public DateTime AttendedBeginDate { get; set; }
        [DataMember]
        public DateTime AttendedEndDate { get; set; }
        [DataMember]
        public string AttendedSchoolAddressLine1 { get; set; }
        [DataMember]
        public string AttendedSchoolAddressCity { get; set; }
        [DataMember]
        public string AttendedSchoolAddressState { get; set; }
        [DataMember]
        public string AttendedSchoolAddressZip { get; set; }
        [DataMember]
        public string AttendedSchool { get; set; }
        [DataMember]
        public decimal AttendedHoursPerWeek { get; set; }
        [DataMember]
        public int AttendedSessionsPerWeek { get; set; }
        [DataMember]
        public bool IsPaidByDEA { get; set; }
        [DataMember]
        public DateTime PaidTuitionStartDate { get; set; }
        [DataMember]
        public string AgencyName { get; set; }
        [DataMember]
        public decimal OtherAssests { get; set; }
        [DataMember]
        public decimal RealEstate { get; set; }
        [DataMember]
        public decimal Savings { get; set; }
        [DataMember]
        public decimal Securities { get; set; }
        [DataMember]
        public Guid DepID { get; set; }
    }
}
