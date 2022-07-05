namespace UDO.LOB.PersonSearch.Constants
{
    public  static class MethodNames
    {
        public const string UDOpsFindPersonProcessor = "UDOpsFindPersonProcessor";
        public const string MVIAttendedSearch = "MVIMessages.AttendedSearch";
        public const string MVIUnattendedSearch = "MVIMessages.UnattendedSearchRequest";
        public const string SearchCorpDb = "SearchCorpDb";
        public const string MVIDeterministicSearch = "MVIMessages.DeterministicSearch";
        public const string UDOpsSelectPersonProcessor = "UDOpsSelectPersonProcessor";
        public const string TryGetCrmPerson = "TryGetCrmPerson";
        public const string TryCreateNewCrmPerson = "TryCreateNewCrmPerson";
    }

    // These should have corresponding names in MethodNames
    public static class MethodDescriptions
    {
        public const string MVIAttendedSearch = "MVI Attended Find Process";
        public const string MVIUnattendedSearch = "MVI Unattended Find Process";
        public const string MVIDeterministicSearch = "MVI Deterministic Search Process";
    }
}
