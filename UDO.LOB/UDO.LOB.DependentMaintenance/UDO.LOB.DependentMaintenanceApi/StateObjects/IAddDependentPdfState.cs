namespace UDO.LOB.DependentMaintenance
{
    public interface IAddDependentPdfState
    {
        IAddDependentMaintenanceRequestState AddDependentMaintenanceRequestState { get; }
        IProcRequestState ProcRequestState { get; }
        IVeteranRequestState VeteranRequestState { get; }
        bool HasOrchestrationError { get; set; }
        byte[] WordDocBytes { get; set; }
        string PdfFileName { get; set; }
        byte[] PdfFileBytes { get; set; }
        void Dispose();
    }
}