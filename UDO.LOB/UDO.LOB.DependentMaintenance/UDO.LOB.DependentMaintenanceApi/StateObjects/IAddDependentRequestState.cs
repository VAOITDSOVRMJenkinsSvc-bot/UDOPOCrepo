using UDO.LOB.DependentMaintenance.Messages;
//using VRM.Integration.Servicebus.AddDependent.Messages;

namespace UDO.LOB.DependentMaintenance
{
    public interface IAddDependentRequestState
    {
        IAddDependentMaintenanceRequestState AddDependentMaintenanceRequestState { get; }
        IProcRequestState ProcRequestState { get; }
        IVeteranRequestState VeteranRequestState { get; }
        IDependentRequestState DependentRequestState { get; }
        IMaritalHistory[] MaritalHistories { get; }
        long ParticipantMailAddressId { get; set; }
        long BenefitClaimId { get; set; }
        long VnpBenefitClaimId { get; set; }
        insertBenefitClaimBenefitClaimInput BenifitClaim { get; set; }
        string NextAvailBnftClaimIncrement { get; set; }
        string StationOfJurisdiction { get; set; }
        long LocationId { get; set; }
        void Dispose();
    }
}