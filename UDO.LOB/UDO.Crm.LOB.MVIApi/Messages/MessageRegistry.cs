
namespace VRM.Integration.UDO.MVI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
    public static class MessageRegistry
    {

        public const string UDOCTIPersonSearchRequest = "UDOCTIPersonSearchRequest";
        
        public const string UDOCHATPersonSearchRequest = "UDOCHATPersonSearchRequest";
        public const string UDOBIRLSandOtherSearchRequest = "UDOBIRLSandOtherSearchRequest";

        public const string UDOOpenIDProofAsyncRequest = "UDOOpenIDProofAsyncRequest";
        public const string UDOOpenIDProofRequest = "UDOOpenIDProofRequest";
        public const string UDOOpenIDProofResponse = "UDOOpenIDProofResponse";
        
        public const string UDOAddPersonRequest = "UDOAddPersonRequest";
        public const string UDOAddPersonResponse = "UDOAddPersonResponse";

        public const string UDOPersonSearchRequest = "UDOPersonSearchRequest";
        public const string UDOPersonSearchResponse = "UDOPersonSearchResponse";

        public const string UDOSelectedPersonRequest = "UDOSelectedPersonRequest";
        public const string UDOSelectedPersonResponse = "UDOSelectedPersonResponse";

        public const string UDOCombinedPersonSearchRequest = "UDOCombinedPersonSearchRequest";
        public const string UDOCombinedPersonSearchResponse = "UDOCombinedPersonSearchResponse";

        public const string UDOCombinedSelectedPersonRequest = "UDOCombinedSelectedPersonRequest";
        public const string UDOCombinedSelectedPersonResponse = "UDOCombinedSelectedPersonResponse";

        //public const string UDOGetVeteranInfoRequest = "UDOGetVeteranInfoRequest";
        //public const string UDOGetVeteranInfoResponse = "UDOGetVeteranInfoResponse";

//        public const string UDOGetSensitivityLevelRequest = "UDOGetSensitivityLevelRequest";
        //public const string UDOGetSensitivityLevelResponse = "UDOGetSensitivityLevelResponse";

        public const string UDOfindVeteranInfoRequest = "UDOfindVeteranInfoRequest";
        public const string UDOfindVeteranInfoResponse = "UDOfindVeteranInfoResponse";

        public const string UDOfindVeteranInfoByPidRequest = "UDOfindVeteranInfoByPidRequest";
        public const string UDOfindVeteranInfoByPidResponse = "UDOfindVeteranInfoByPidResponse";

        public const string UDOgetVeteranIdentifiersRequest = "UDOgetVeteranIdentifiersRequest";
        public const string UDOgetVeteranIdentifiersResponse = "UDOgetVeteranIdentifiersResponse";

        public const string UDOHandleDupCorpRecordResponse = "UDOHandleDupCorpRecordResponse";
    }
}
