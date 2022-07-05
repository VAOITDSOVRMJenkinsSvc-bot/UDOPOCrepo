using System.Runtime.Serialization;

namespace VEIS.Mvi.Messages
{
    [DataContract]
    public class AddPatientPerson
    {
        [DataMember]
        public Name[] NameList { get; set; }

        [DataMember]
        public PatientAddress Address { get; set; }

        [DataMember]
        public string HomePhoneNumber { get; set; }

        [DataMember]
        public string WorkPhoneNumber { get; set; }

        [DataMember]
        public string CellPhoneNumber { get; set; }

        /// <summary>
        /// M = Male, F = Female
        /// </summary>
        [DataMember]
        public GenderCode GenderCode { get; set; }

        [DataMember]
        public string BirthDate { get; set; }

        [DataMember]
        public BirthPlace BirthPlace { get; set; }

        [DataMember]
        public bool MultipleBirthIndicator { get; set; }

        /// <summary>
        /// 1002-5 = American Indian or Alaska Native, 2054-5 Black or African American, 2028-9 = Asian, 
        /// 2076-8 = Native Hawiian or other Pacific Islander, 2016-3 = White,  2131-1 = Other Race
        /// </summary>
        [DataMember]
        public Race Race { get; set; }

        /// <summary>
        /// 0000-0 = Refused to answer, 2135-2 = Hispanic or Latino, 2186-5 = Not Hispanic or Latino, 9999-4 = Unknown by Patient
        /// </summary>
        [DataMember]
        public EthnicGroup Ethnicity { get; set; }

        [DataMember]
        public string DeceasedDate { get; set; }

        [DataMember]
        public AsOtherId[] AsOtherIds { get; set; }
    }

    [DataContract]
    public class BirthPlace
    {
        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }
    }

    [DataContract]
    public class AsOtherId
    {
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// SSN Verification Status
        /// </summary>
        [DataMember]
        public CorrelationStatusCode StatusCode { get; set; }

        /// <summary>
        /// PAT = Patient Identifier, SSN = Social Security Number, CIT = Citizen, MIL = Military Personnnel, AFFL = Affiliated Person (For DoD, TIN and FIN)
        /// </summary>
        [DataMember]
        public ClassCode ClassCode { get; set; }
    }
}
