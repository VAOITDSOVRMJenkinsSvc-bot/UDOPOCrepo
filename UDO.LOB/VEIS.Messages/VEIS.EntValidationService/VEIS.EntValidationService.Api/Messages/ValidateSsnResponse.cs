using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.EntValidationService.Api.Messages
{
    [DataContract]
    public class VEISValidateSsnResponse : VEISEcResponseBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; } 
        [DataMember]
        public ValidateSsnResponseData validateSsnResponseData { get; set; }

        public VEISValidateSsnResponse()
        {
            ExceptionOccured = false;
            ExceptionMessage = string.Empty;
            validateSsnResponseData = new ValidateSsnResponseData();
        }
    }
    [DataContract]
    public class ValidateSsnResponseData
    {
        [DataMember]
        public bool isValidationErrorOccured { get; set; }
        [DataMember]
        public List<string> errorList { get; set; }

        public ValidateSsnResponseData()
        {
            isValidationErrorOccured = false;
            errorList = new List<string>();
        }
    }
}