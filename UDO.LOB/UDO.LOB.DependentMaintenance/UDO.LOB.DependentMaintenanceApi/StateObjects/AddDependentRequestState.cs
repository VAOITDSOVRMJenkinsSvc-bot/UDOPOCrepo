using UDO.LOB.DependentMaintenance.Messages;
//using VRM.Integration.Servicebus.AddDependent.Messages;
using VRM.Integration.Servicebus.Core;

namespace UDO.LOB.DependentMaintenance
{
    public class AddDependentRequestState : PipeState, IAddDependentRequestState
    {
        public AddDependentRequestState(IAddDependentMaintenanceRequestState addDependentMaintenanceRequestState, 
            IVeteranParticipant veteran, 
            IDependentParticipant dependent, 
            MaritalHistory[] maritalHistories)
        {
            AddDependentMaintenanceRequestState = addDependentMaintenanceRequestState;

            ProcRequestState = new ProcRequestState();

            VeteranRequestState = new VeteranRequestState { VeteranParticipant = veteran };

            DependentRequestState = new DependentRequestState { DependentParticipant = dependent };

            MaritalHistories = maritalHistories;

            ProcRequestState.CreatedDate =
                AddDependentMaintenanceRequestState.DependentMaintenance.crme_ClaimDate.GetValueOrDefault();

            ProcRequestState.LastModifiedDate = 
                AddDependentMaintenanceRequestState.DependentMaintenance.crme_ClaimDate.GetValueOrDefault();
        }

        public AddDependentRequestState(IAddDependentMaintenanceRequestState addDependentMaintenanceRequestState,
            IVeteranParticipant veteran,
            MaritalHistory[] maritalHistories) : this(addDependentMaintenanceRequestState, veteran, null, maritalHistories)
        {
        }

        public IDependentParticipant Dependent
        {
            set
            {
                DependentRequestState = new DependentRequestState { DependentParticipant = value };
            }
        }


        public IAddDependentMaintenanceRequestState AddDependentMaintenanceRequestState { get; private set; }
        public IProcRequestState ProcRequestState { get; private set; }
        public IVeteranRequestState VeteranRequestState { get; private set; }
        public IDependentRequestState DependentRequestState { get; private set; }
        public IMaritalHistory[] MaritalHistories { get; private set; }
        public long ParticipantMailAddressId { get; set; }
        public long BenefitClaimId { get; set; }
        public long VnpBenefitClaimId { get; set; }
        public insertBenefitClaimBenefitClaimInput BenifitClaim { get; set; }
        public string NextAvailBnftClaimIncrement { get; set; }
        public string StationOfJurisdiction { get; set; }
        public string StationId { get; set; }
        public long LocationId { get; set; }
	}
}
