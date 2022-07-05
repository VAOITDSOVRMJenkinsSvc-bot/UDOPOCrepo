using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using VEIS.Core.Messages;

namespace VEIS.EBenefitEducationService.Api.Messages
{
    [DataContract]
    /// <summary> 
    /// VEIS Enterprise Component for VEIS.EBenefitEducationService.Api,GetEduCourseDetail method, Response.  
    /// </summary>
    /// <param name=none></param>
    /// <returns>none</returns>
    public class VEISgteducrsedtlGetEduCourseDetailResponse : VEISEcResponseBase
    {
        [DataMember]
        public VEISgteducrsedtltrainingTypeMultipleResponse[] VEISgteducrsedtltrainingTypeInfo { get; set; } 
    }
    [DataContract]
    public class VEISgteducrsedtltrainingTypeMultipleResponse
    {
        [DataMember]
        public Int64 mcs_participantID { get; set; }
        [DataMember]
        public string mcs_facilityCode { get; set; }
        [DataMember]
        public string mcs_code { get; set; }
        [DataMember]
        public string mcs_description { get; set; }
        [DataMember]
        public DateTime mcs_effectiveDate { get; set; }
        [DataMember]
        public DateTime mcs_statusDate { get; set; }
        [DataMember]
        public string mcs_courseID { get; set; }
    }
}