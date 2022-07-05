using System;
using VRM.Integration.Servicebus.Core;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Extensions.Configuration;
using UDO.LOB.DependentMaintenance;
using UDO.LOB.DependentMaintenance.Messages;
using UDO.LOB.DependentMaintenance.ProcSteps;

//using VRM1.Integration.Servicebus.AddDependent.Messages;
//using VRM1.Integration.Servicebus.AddDependent.ProcSteps;
//using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace UDO.LOB.DependentMaintenance.Processors
{
    public class AddDependentOrchestrationProcessor
    {
        private bool _debug { get; set; }

        private const string method = "AddDependentOrchestrationProcessor";

        private string LogBuffer { get; set; }
        public void Execute(AddDependentOrchestrationRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.Execute"); //{JsonHelper.Serialize<UDOcreateAwardsRequest>(request)}");

            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = "AddDependentOrchestrationProcessor",
                    OrganizationName = request.OrganizationName
                    //StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }
            try
            {
                var hasOrchestrationError = false;
                //CSDev This is the wrong SoapLog
                //VRM.Integration.Servicebus.Core.SoapLog.Current.Active = true;
                VEIS.Core.Wcf.SoapLog.Current.Active = true;
                try
                {
                    //request.Debug = true;
                    using (var msg = new AddDependentMaintenanceRequestState(request.OrganizationName,
                        request.DependentMaintenanceId,
                        request.UserId,
                        request.Debug))
                    {
                        try
                        {
                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, $"{ this.GetType().FullName}"
                                , $"| VVV Start {this.GetType().FullName}.ProcessCrmConnect");
                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, $"{ this.GetType().FullName}"
                                , $"| EEE Start {this.GetType().FullName}.ProcessCrmConnect | Debug from Request Value: " + request.Debug.ToString());

                            ////CSDev Use this method? Per Nithin? 
                            //LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.Debug, request.UserId, $"{ this.GetType().FullName}"
                            //	, $"| VVV Start {this.GetType().FullName}.ProcessCrmConnect", true);

                            ProcessCrmConnect(msg);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.LogError(request.OrganizationName, request.UserId, $"| VVV Error { this.GetType().FullName}.ProcessCrmConnect Error", ex.ToString());

                            //CSDev Annotation Logging
                            msg.Exception = ex;
                            var uploadErrorLog = new UploadErrorLog();
                            uploadErrorLog.Execute(msg, "CRM Connection Error Log");

                            //Always Return if CRMConnection Throws an Error
                            return;
                        }

                        try
                        {
                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, $"{ this.GetType().FullName}"
                                , $"| VVV Start {this.GetType().FullName}.ProcessDependents");

                            ProcessDependents(msg);
                        }
                        catch (Exception ex)
                        {
                            hasOrchestrationError = true;
                            //CSDev Annotation Logging
                            LogHelper.LogError(request.OrganizationName, request.UserId, $"| VVV Error { this.GetType().FullName}.ProcessDependents Error", ex.ToString());
                            msg.Exception = ex;
                            var uploadErrorLog = new UploadErrorLog();
                            uploadErrorLog.Execute(msg, "Orchestration Error Log");
                        }

                        try
                        {
                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, $"{ this.GetType().FullName}"
                                , $"| VVV Start {this.GetType().FullName}.ProcessPdf");

                            ProcessPdf(msg, hasOrchestrationError);
                        }
                        catch (Exception ex)
                        {
                            //CSDev Annotation Logging
                            LogHelper.LogError(request.OrganizationName, request.UserId, $"| VVV Error { this.GetType().FullName}.ProcessPdf Error", ex.ToString());
                            msg.Exception = ex;
                            var uploadErrorLog = new UploadErrorLog();
                            uploadErrorLog.Execute(msg, "PDF Error Log");
                        }
                    }
                }
                finally
                {
                    //CSDev This is the wrong SoapLog
                    //SoapLog.Current.Active = false;
                    //SoapLog.Current.ClearLog();
                    VEIS.Core.Wcf.SoapLog.Current.Active = false;
                    VEIS.Core.Wcf.SoapLog.Current.ClearLog();

                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "AddDependentOrchestrationProcessor", $"<< Exited {this.GetType().FullName}.AddDependentOrchestration");
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "AddDependentOrchestrationProcessor", ex.ToString());
            }
        }

        private static void ProcessCrmConnect(IAddDependentMaintenanceRequestState msg)
        {
            new Pipeline<IAddDependentMaintenanceRequestState>()
                .Register(new ConnectToCrm())
                .Register(new GetDependentMaintenanceRecord())
                //.Register(new UpdateVeteranInfo())
                .Register(new GetSystemUserRecord())
                .Register(new GetBgsHeaderInfo())
                .Register(new GetAddDependentRequest())
                .Execute(msg);
        }

        private static void ProcessVeteran()
        {
        }

        private static void ProcessDependents(IAddDependentMaintenanceRequestState request)
        {

            AddDependentRequestState vetMsg = null;
            try
            {
                vetMsg = new AddDependentRequestState(request,
                    request.AddDependentRequest.Veteran,
                    request.AddDependentRequest.MaritalHistories);

                new Pipeline<IAddDependentRequestState>()
                    .Register(new CreateVnpProcId())
                    .Register(new CreateProcForm())
                    .Register(new CreateVeteranParticipant())
                    .Register(new CreateVeteran())
                    .Register(new CreateAddressForVeteran())
                    .Register(new CreatePhoneNumberForVeteran())
                    .Register(new GetStationOfJurisdiction())
                    .Execute(vetMsg);

                foreach (var dependent in request.AddDependentRequest.Dependents)
                {
                    vetMsg.Dependent = dependent;
                    //using (var msg = new AddDependentRequestState(request,
                    //    request.AddDependentRequest.Veteran,
                    //    dependent,
                    //    request.AddDependentRequest.MaritalHistories))
                    //{
                    new Pipeline<IAddDependentRequestState>()
                        //.Register(new CreateVnpProcId())
                        //.Register(new CreateProcForm())
                        //.Register(new CreateVeteranParticipant())
                        //.Register(new CreateVeteran())

                        .Register(new CreateDependentParticipant())
                        .Register(new CreateDependent())

                        //.Register(new CreateAddressForVeteran())
                        //.Register(new CreatePhoneNumberForVeteran())  

                        .Register(new CreateAddressForDependent())
                        .Register(new CreatePhoneNumberForDependent())
                        .Register(new CreateDependentRelationship())

                        .Register(new CreateVnpChildSchool())
                        .Register(new CreateVnpChildStudent())
                        //.Register(new GetStationOfJurisdiction())
                        //.Register(new GetLocationId())
                        //.Register(new CreateVnpBenefitClaimInformation())
                        //.Register(new OffRampProcessing())
                        //.Register(new GetNextAvailBnftClaimIncrement())
                        //.Register(new InsertBenefitClaimInformation())
                        //.Register(new UpdateBenefitClaimInformation())
                        //.Register(new SetVnpProcStateToReady())
                        .Execute(vetMsg);

                }

                new Pipeline<IAddDependentRequestState>()
                 //.Register(new GetStationOfJurisdiction())
                 .Register(new GetLocationId())
                 .Register(new CreateVnpBenefitClaimInformation())
                 .Register(new OffRampProcessing())
                 .Register(new GetNextAvailBnftClaimIncrement())
                 .Register(new InsertBenefitClaimInformation())
                 .Register(new UpdateBenefitClaimInformation())
                 .Register(new SetVnpProcStateToReady())
                 .Execute(vetMsg);
            }
            finally
            {
                //CSDev HERE We need to fix this so the soap log gets added as an annotation to the DepMain record
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.SystemUserId, $"ProcessDependents()", $"| FFF Finally");
                var upSoapLog = new UploadSoapLog();
                upSoapLog.Execute(vetMsg);
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.SystemUserId, $"ProcessDependents()", $"| FFF upSoapLog.Execute(vetMsg) Complete");
            }
        }

        private static void ProcessPdf(IAddDependentMaintenanceRequestState request, bool hasOrchestrationError)
        {
            var is686 = false;
            //code for generatin 686c
            foreach (var dep in request.AddDependentRequest.Dependents)
            {
                if (dep.MaintenanceType == "Add" || dep.MaintenanceType == "Remove")
                {
                    is686 = true;
                    break;
                }
            }
            if (is686 == true)
            {
                using (var msg = new AddDependentPdfState(request, request.AddDependentRequest.Veteran, hasOrchestrationError))
                {
                    new Pipeline<IAddDependentPdfState>()
                        .Register(new CreateMsWordDocument())
                        .Register(new UploadMsWordDocument())
                        .Register(new CreatePdfDocument())
                        //.Register(new LoadPdfToVva())
                        .Register(new LoadPdfToVbms())
                        .Register(new UploadPdfDocument())
                        .Execute(msg);
                }
            }

            using (var msg = new AddDependentPdfState(request, request.AddDependentRequest.Veteran, hasOrchestrationError))
            {
                // msg.AddDependentMaintenanceRequestState.AddDependentRequest.Dependents[0].DepID = dep.DepID;

                ////call the function to generate 674.
                new Pipeline<IAddDependentPdfState>()
                .Register(new CreateUpload674Document())
                //.Register(new UploadMsWordDocument())
                .Execute(msg);

            }
        }
    }
}