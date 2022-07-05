namespace UDO.LOB.DependentMaintenance.Messages
{
    public static class MessageRegistry
    {
        public const string AddDependentRequest = "AddDependent#AddDependentRequest";
        public const string AddDependentOrchestrationRequest = "AddDependent#AddDependentOchestrationRequest";

        public const string GetDependentInfoRequest = "AddDependent#GetDependentInfoRequest";
        public const string GetDependentInfoResponse = "AddDependent#GetDependentInfoResponse";

        public const string GetMaritalInfoRequest = "AddDependent#GetMaritalInfoRequest";
        public const string GetMaritalInfoResponse = "AddDependent#GetMaritalInfoResponse";

        public const string GetVeteranInfoRequest = "AddDependent#GetVeteranInfoRequest";
        public const string GetVeteranInfoResponse = "AddDependent#GetVeteranInfoResponse";

		//CSDev Added and aPrefixed with BGS, rename all to avoid conflict 
		public const string GetBGSDependentInfoRequest = "Bgs#GetDependentInfoRequest";
		public const string GetBGSDependentInfoResponse = "Bgs#GetDependentInfoResponse";

		public const string GetBGSMaritalInfoRequest = "Bgs#GetMaritalInfoRequest";
		public const string GetBGSMaritalInfoResponse = "Bgs#GetMaritalInfoResponse";

		public const string GetBGSVeteranInfoRequest = "Bgs#GetVeteranInfoRequest";
		public const string GetBGSVeteranInfoResponse = "Bgs#GetVeteranInfoResponse";

		public const string GetBGSSensitivityLevelRequest = "Bgs#GetSensitivityLevelRequest";
		public const string GetBGSSensitivityLevelResponse = "Bgs#GetSensitivityLevelResponse";


        public const string GetBGSSchoolInfoRequest = "Bgs#GetSchoolInfoRequest";
        public const string GetBGSSchoolInfoResponse = "Bgs#GetSchoolInfoResponse";

        public const string SearchBGSSchoolInfoRequest = "Bgs#SearchSchoolInfoRequest";
        public const string SearchBGSSchoolInfoResponse = "Bgs#SearchSchoolInfoResponse";

    }
}