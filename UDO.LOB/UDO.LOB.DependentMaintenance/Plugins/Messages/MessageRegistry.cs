namespace VRM.Integration.Servicebus.AddDependent.Messages
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
    }
}