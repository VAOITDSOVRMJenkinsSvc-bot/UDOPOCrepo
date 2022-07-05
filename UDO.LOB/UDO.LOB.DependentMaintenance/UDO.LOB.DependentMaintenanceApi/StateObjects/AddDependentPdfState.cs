using UDO.LOB.DependentMaintenance.Messages;
//using VRM.Integration.Servicebus.AddDependent.Messages;
using VRM.Integration.Servicebus.Core;

namespace UDO.LOB.DependentMaintenance
{
    public class AddDependentPdfState : PipeState, IAddDependentPdfState
    {
        public AddDependentPdfState(IAddDependentMaintenanceRequestState addDependentMaintenanceRequestState,
            IVeteranParticipant veteran,
            bool hasOrchestrationError)
        {
            AddDependentMaintenanceRequestState = addDependentMaintenanceRequestState;

            ProcRequestState = new ProcRequestState();

            VeteranRequestState = new VeteranRequestState { VeteranParticipant = veteran };

            HasOrchestrationError = hasOrchestrationError;
        }

        public IAddDependentMaintenanceRequestState AddDependentMaintenanceRequestState { get; private set; }
        public IProcRequestState ProcRequestState { get; private set; }
        public IVeteranRequestState VeteranRequestState { get; private set; }
        public bool HasOrchestrationError { get; set; }
        public byte[] WordDocBytes { get; set; }
        public string PdfFileName { get; set; }
        public byte[] PdfFileBytes { get; set; }
    }
}