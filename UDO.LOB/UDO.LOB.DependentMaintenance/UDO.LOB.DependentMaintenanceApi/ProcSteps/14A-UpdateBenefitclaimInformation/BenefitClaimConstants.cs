namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class BenefitClaimConstants
    {
        public const string EndPrdctTypeCd = "130";
        public const string JrnInsertStatusTypeCd = "I";
        public const string JrnUpdateStatusTypeCd = "U";
        public const string PgmTypeCd = "COMP";
        public const string StatusTypeCd = "CURR";
        public const string SvcTypeCd = "CP";

        // WSCR 1601:
        //  change the benefit claim type code to 130PDA
        //  public const string BnftClaimTypeCd = "130DPNDCYAUT";
        public const string BnftClaimTypeCd = "130PDA";
    }
}