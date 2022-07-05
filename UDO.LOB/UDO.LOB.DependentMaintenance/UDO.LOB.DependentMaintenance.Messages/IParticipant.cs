using System;
using System.Runtime.Serialization;

namespace UDO.LOB.DependentMaintenance.Messages
{
    public interface IParticipant
    {
        [DataMember]
        string FileNumber { get; set; }
        [DataMember]
        string MaintenanceType { get; set; }
        [DataMember]
        string Ssn { get; set; }

        [DataMember]
        string FirstName { get; set; }

        [DataMember]
        string MiddleName { get; set; }

        [DataMember]
        string LastName { get; set; }

        [DataMember]
        DateTime BirthDate { get; set; }

        [DataMember]
        string BirthCityName { get; set; }

        [DataMember]
        string BirthStateCode { get; set; }

        [DataMember]
        string BirthCountryName { get; set; }

        [DataMember]
        bool IsVetInd { get; set; }

        [DataMember]
        bool IsSeriouslyDisabled { get; set; }

        [DataMember]
        bool IsScholdChild { get; set; }

        [DataMember]
        Address[] Addresses { get; set; }

        [DataMember]
        PhoneNumber[] PhoneNumbers { get; set; }

        [DataMember]
        string AddresssType { get; set; }
        [DataMember]
        string EmailAddress { get; set; }
        [DataMember]
        string postalCodePlus4 { get; set; }
        [DataMember]
        string SuffixName { get; set; }
        [DataMember]
        string TitleName { get; set; }
        [DataMember]
        string NoSssnReasonTypeCd { get; set; }
        [DataMember]
        string CourseName { get; set; }
        [DataMember]
        string SchoolCode { get; set; }
        [DataMember]
        string PrevSchoolCode { get; set; }
        [DataMember]
        string SchoolAddressLine1 { get; set; }
        [DataMember]
        string SchoolName { get; set; }
        [DataMember]
        string SchoolAddressCity { get; set; }
        [DataMember]
        string SchoolAddressState { get; set; }
        [DataMember]
        string SchoolAddressZip { get; set; }
        [DataMember]
        DateTime ExpectedStartDate { get; set; }
        [DataMember]
        DateTime ExpectedGradDate { get; set; }
        [DataMember]
        DateTime CourseBeginDate { get; set; }
        [DataMember]
        bool IsAttendedLastTerm { get; set; }
        [DataMember]
        string FullTimeStudentTypeCode { get; set; }
        [DataMember]
        DateTime AttendedBeginDate { get; set; }
        [DataMember]
        DateTime AttendedEndDate { get; set; }
        [DataMember]
        string AttendedSchoolAddressLine1 { get; set; }
        [DataMember]
        string AttendedSchoolAddressCity { get; set; }
        [DataMember]
        string AttendedSchoolAddressState { get; set; }
        [DataMember]
        string AttendedSchoolAddressZip { get; set; }
        [DataMember]
        string AttendedSchool { get; set; }
        [DataMember]
        decimal AttendedHoursPerWeek { get; set; }
        [DataMember]
        int AttendedSessionsPerWeek { get; set; }
        [DataMember]
        bool IsPaidByDEA { get; set; }
        [DataMember]
        DateTime PaidTuitionStartDate { get; set; }
        [DataMember]
        string AgencyName { get; set; }
        [DataMember]
        decimal OtherAssests { get; set; }
        [DataMember]
        decimal RealEstate { get; set; }
        [DataMember]
        decimal Savings { get; set; }
        [DataMember]
        decimal Securities { get; set; }
        [DataMember]
        Guid DepID { get; set; }
    }
}