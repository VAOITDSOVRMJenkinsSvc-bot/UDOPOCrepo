using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.CpeForeignMedicalProgramAdapterServiceV3.CRMOL.Messages
{
    [DataContract]
    /// <summary>
    /// VEIS Enterprise Component for VEIS.CpeForeignMedicalProgramAdapterServiceV3.CRMOL,getFMPAcceptLetter method, Response.
    /// Code Generated by IMS on: 6/1/2018 11:08:32 AM
    /// Version: 2018.05.09.05
    /// </summary>
    /// <param name=none></param>
    /// <returns>none</returns>
    public class VEISgFMPALgetFMPAcceptLetterResponse : VEISEcResponseBase
    {
        [DataMember]
        public VEISgFMPALCPEForeignMedicalProgramAdapterv3getFMPAcceptLetterResponse VEISgFMPALCPEForeignMedicalProgramAdapterv3getFMPAcceptLetterResponseInfo { get; set; }
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
    }
    [DataContract]
    public class VEISgFMPALCPEForeignMedicalProgramAdapterv3getFMPAcceptLetterResponse
    {
        [DataMember]
        public string mcs_textOfReport { get; set; }
        [DataMember]
        public VEISgFMPALcode VEISgFMPALcodeInfo { get; set; }
        [DataMember]
        public VEISgFMPALfault VEISgFMPALfaultInfo { get; set; }
    }
    [DataContract]
    public enum VEISgFMPALcode
    {
        SUCCESS,
        ERROR,
    }
    [DataContract]
    public class VEISgFMPALfault
    {
        [DataMember]
        public bool mcs_essCodeSpecified { get; set; }
        [DataMember]
        public string mcs_essText { get; set; }
        [DataMember]
        public string mcs_code { get; set; }
        [DataMember]
        public string mcs_text { get; set; }
        [DataMember]
        public VEISgFMPALessCode VEISgFMPALessCodeInfo { get; set; }
        [DataMember]
        public VEISgFMPALnestedFault VEISgFMPALnestedFaultInfo { get; set; }
    }
    [DataContract]
    public enum VEISgFMPALessCode
    {
        SUCCESS,
        ERROR,
        WARNING,
        REFUSED,
        ACCEPT,
        REJECT,
        FAIL,
    }
    [DataContract]
    public class VEISgFMPALnestedFault
    {
        [DataMember]
        public bool mcs_essCodeSpecified { get; set; }
        [DataMember]
        public string mcs_essText { get; set; }
        [DataMember]
        public string mcs_code { get; set; }
        [DataMember]
        public string mcs_text { get; set; }
        [DataMember]
        public VEISgFMPALessCode1 VEISgFMPALessCode1Info { get; set; }
    }
    [DataContract]
    public enum VEISgFMPALessCode1
    {
        SUCCESS,
        ERROR,
        WARNING,
        REFUSED,
        ACCEPT,
        REJECT,
        FAIL,
    }
}
