using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.EBenefitEducationService.Api.Messages
{
    [DataContract]
    /// <summary>
    /// VEIS Enterprise Component for VEIS.EBenefitEducationService.Api,GetEduCourseDetail method, Request.  
    /// </summary>
    /// <param name=none></param>
    /// <returns>none</returns>
    public class VEISgteducrsedtlGetEduCourseDetailRequest : VEISEcRequestBase
    { 
        [DataMember]
        public Guid RelatedParentId { get; set; }
        [DataMember]
        public string RelatedParentEntityName { get; set; }
        [DataMember]
        public string RelatedParentFieldName { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        [DataMember]
        public LegacyHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public VEISgteducrsedtlgeteducoursedetailsearchstring geteducoursedetailsearchstringInfo { get; set; }
        [DataMember]
        public VEISgteducrsedtltrainingtype trainingtypeInfo { get; set; }
        [DataMember]
        public string mcs_fullcoursecode { get; set; }
        [DataMember]
        public string mcs_courseid { get; set; }
    }
    public class VEISgteducrsedtlgeteducoursedetailsearchstring
    {
        [DataMember]
        public string mcs_facilityCode { get; set; }
    }
    public enum VEISgteducrsedtltrainingtype
    {
        FLIGHT,
        CORRESPONDENCE,
        INSTITUTIONOFHIGHERLEARNING,
        NONCOLLEGEDEGREE,
        ONJOBTRAINING
    }
}