using System.Runtime.Serialization;

namespace VEIS.Mvi.Messages
{
    /// <summary>
    /// Specifies the types of acknowledgements that can be received from a MVI request.
    /// </summary>
    [DataContract]
    public enum AcknowledgementType
    {
        [EnumMember]
        Unspecified = 0,

        [EnumMember]
        OK = 1,

        [EnumMember]
        MessageRejectedByReceiver = 2,

        [EnumMember]
        ApplicationErrorInReceiver = 3,

        [EnumMember]
        ApplicationErrorInSender = 4,

        [EnumMember]
        Unknown = 5
    }

    /// <summary>
    /// Specifies the usage of a person address object.
    /// </summary>
    [DataContract]
    public enum AddressUse
    {
        [EnumMember]
        Unspecified = 0,

        [EnumMember]
        Bad = 1,

        [EnumMember]
        Confidential = 2,

        [EnumMember]
        Home = 3,

        [EnumMember]
        PrimaryHome = 4,

        [EnumMember]
        OtherHome = 5,

        [EnumMember]
        Temporary = 6,

        [EnumMember]
        Workplace = 7,

        [EnumMember]
        Other = 8
    }

    /// <summary>
    /// Specifies the usage of a person telecommunications object.
    /// </summary>
    [DataContract]
    public enum TelecomUse
    {
        [EnumMember]
        Unspecified = 0,

        [EnumMember]
        Bad = 1,

        [EnumMember]
        DirectLine = 2,

        [EnumMember]
        Home = 3,

        [EnumMember]
        PrimaryHome = 4,

        [EnumMember]
        OtherHome = 5,

        [EnumMember]
        Temporary = 6,

        [EnumMember]
        Workplace = 7,

        [EnumMember]
        Mobile = 8,

        [EnumMember]
        Pager = 9,

        [EnumMember]
        Other = 10
    }

    /// <summary>
    /// Specifies the VA accepted administrative gender codes.
    /// </summary>
    [DataContract]
    public enum Gender
    {
        [EnumMember]
        Unspecified = 0,

        [EnumMember]
        Male = 1,

        [EnumMember]
        Female = 2
    }

    /// <summary>
    /// Specifies the VA accepted identifier types.
    /// </summary>
    [DataContract]
    public enum IdentifierType
    {
        Unspecified = 0,

        SocialSecurityNumber = 1,

        NationalIdentifier = 2,

        EmployeeIdentifier = 3,

        PatientIdentifier = 4,

        ParticipantNumber = 5
    }

    /// <summary>
    /// Specifies the usage of a person name object.
    /// </summary>
    [DataContract]
    public enum NameUse
    {
        Unspecified = 0,

        /// <summary>
        /// Use this value for name aliases.
        /// </summary>
        Assigned = 1,

        /// <summary>
        /// Use this value for a name on an official document that differs from the legal name.
        /// </summary>
        Certificate = 2,

        /// <summary>
        /// Use this value for a maiden name.
        /// </summary>
        OfficialRegistry = 3,

        /// <summary>
        /// Use this value to indicate a tribal or other indigenous name form.
        /// </summary>
        Indigenous = 4,

        /// <summary>
        /// Use this value for the legal name associated with a person.
        /// </summary>
        Legal = 5,

        /// <summary>
        /// Use this value for stage names, etc...
        /// </summary>
        Pseudoymn = 6,

        /// <summary>
        /// Use this value for names associated with a religious organization (e.g. Father Tim).
        /// </summary>
        Religous = 7,
        Maiden,
        Alias
    }

    /// <summary>
    /// The type of processing associated with the message.
    /// </summary>
    [DataContract]
    public enum ProcessingType
    {
        Test = 0,

        Production = 1,

        Development = 2
    }

    /// <summary>
    /// SSN Verification Status
    /// </summary>
    public enum CorrelationStatusCode
    {
        New = 0, InProgress = 1, Invalid = 2, Resend = 3, Verified = 4
    };

    /// <summary>
    /// PAT = Patient Identifier, SSN = Social Security Number, CIT = Citizen, MIL = Military Personnnel, AFFL = Affiliated Person (For DoD, TIN and FIN), PATCORP = CorpDB Patient
    /// </summary>
    public enum ClassCode
    {
        PAT, SSN, CIT, MIL, AFFL, PATCORP
    };

    public enum GenderCode
    {
        M, F, NotSpecified
    };

    public enum Race
    {
        AmericanIndianOrAlaskaNative, BlackOrAfricanAmerican, Asian, NativeHawiianOrOtherPacificIslander, White, OtherRace
    };

    public enum EthnicGroup
    {
        RefusedToAnswer, HispanicOrLatino, NotHispanicOrLatino, UnknownByPatient, NotSpecified
    }
}
