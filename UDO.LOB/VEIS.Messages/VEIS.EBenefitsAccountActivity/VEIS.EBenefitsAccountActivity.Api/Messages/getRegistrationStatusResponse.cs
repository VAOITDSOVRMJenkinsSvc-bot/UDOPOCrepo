using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.EBenefitsAccountActivity.Api.Messages
{
    [DataContract]
    /// <summary> 
    /// </summary>
    /// <param name=none></param>
    /// <returns>none</returns>
    public class VEISebenAccgetRegistrationStatusResponse : VEISEcResponseBase
    {
        [DataMember]
        public VEISebenAccgetRegistrationStatusResponseDataMultipleResponse[] VEISebenAccgetRegistrationStatusResponseDataInfo { get; set; }
        [DataMember]
        public bool ExceptionOccured { get; set; }
    }
    [DataContract]
    public class VEISebenAccgetRegistrationStatusResponseDataMultipleResponse
    {
        [DataMember]
        public bool mcs_isRegistered { get; set; }
        [DataMember]
        public Int32 mcs_credLevelAtLastLogin { get; set; }
        [DataMember]
        public Int32 mcs_status { get; set; }
        [DataMember]
        public String[] mcs_errorMessage { get; set; }
    }
}