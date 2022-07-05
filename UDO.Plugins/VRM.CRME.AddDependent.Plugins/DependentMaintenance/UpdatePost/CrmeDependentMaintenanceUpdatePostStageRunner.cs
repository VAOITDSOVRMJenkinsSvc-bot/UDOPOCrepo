using MCSPlugins;
using Microsoft.Xrm.Sdk;
using System;
using UDO.LOB.DependentMaintenance.Messages;
using VRM.IntegrationServicebus.AddDependent.CrmModel;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class CrmeDependentMaintenanceUpdatePostStageRunner : MCSPlugins.PluginRunner
    {
        private const long _Draft = 935950000;
        private const long _Submitted = 935950001;
        private const long _Cancelled = 935950004;
        private const long _Failure = 935950002;
        private const long _Success = 935950003;

        bool _logSoap = false;
        bool _logTimer = false;
        string _pcrspid = "";
        string _uri = "";
        private const string _vimtRestEndpointField = "crme_restendpointforvimt";
        private CRMAuthTokenConfiguration _crmAuthTokenConfig;

        #region Constructor
        public CrmeDependentMaintenanceUpdatePostStageRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }
        #endregion

        #region Inernal Methods/Properties
        internal void Execute(IServiceProvider serviceProvider)
        {

            //Avoid Recursive Calls - Abort if depth is greater than 1
            if (PluginExecutionContext.Depth > 1)
                return;

           // TracingService.Trace("value of status is: " + GetSecondaryEntity().GetAttributeValue<OptionSetValue>("crme_txnstatus").Value);
            if (!GetSecondaryEntity().Attributes.Contains("crme_txnstatus"))
            {
                Logger.WriteDebugMessage("Status equal to null - must be save");
                return;
            }


            //Fail/success done in VIMT
            if (ChecForFailureorSuccess())
                return;

            if (CheckForCancel())
                return;

            if (checkForDraft())
                return;

            Logger.WriteDebugMessage("About to do orchestration");
            TracingService.Trace("Starting Orchestration");
            ExecOrchestration();
            TracingService.Trace("Done with Orchestration");
            //End the timing for the plugin
            Logger.WriteTxnTimingMessage(String.Format("Ending :{0}", GetType()));
        }
        private bool ChecForFailureorSuccess()
        {
            var dependendentMaintenance = GetSecondaryEntity().ToEntity<crme_dependentmaintenance>();

            if (dependendentMaintenance.crme_txnStatus != null)
            {
                if (dependendentMaintenance.crme_txnStatus.Value == _Failure ||
                    dependendentMaintenance.crme_txnStatus.Value == _Success)
                    return true;
            }
            return false;
        }

        private bool checkForDraft()
        {
            var entity = (Entity)PluginExecutionContext.InputParameters["Target"];

            var optionSetValue = GetTransactionStatus();

            if (optionSetValue != _Draft)
                return false;

            return true;
        }


            private bool CheckForCancel()
        {
            var entity = (Entity)PluginExecutionContext.InputParameters["Target"];

            var optionSetValue = GetTransactionStatus();

            if (optionSetValue != _Cancelled) 
                return false;
           // var childPlugin = new DeleteDependentMaintChildPlugin(OrganizationService, entity.Id);

            //PluginThread.Exec(childPlugin.Exec);
            #region commented out code for changing status or delete
            //Logger.WriteDebugMessage("About to see if we can change to the cancelled state");
            //// Create the Request Object
            //IsValidStateTransitionRequest checkState = 
            //        new IsValidStateTransitionRequest();

            //    // Set the transition request to an open case
            //    checkState.Entity = new EntityReference("crme_dependentmaintenance", entity.Id);

            //    // Check to see if a new state of "resolved" and 
            //    // a new status of "problem solved" are valid
            //    checkState.NewState = crme_dependentmaintenanceState.Canceled.ToString();
            //    checkState.NewStatus = 3;

            //    // Execute the request
            //    IsValidStateTransitionResponse checkStateResponse = 
            //        (IsValidStateTransitionResponse)OrganizationService.Execute(checkState);

            //    // Handle the response
            //    if (checkStateResponse.IsValid)
            //    {
            //        Logger.WriteDebugMessage("We can change");
            //        SetStateRequest state = new SetStateRequest();

            //        // Set the Request Object's Properties
            //        state.State = new OptionSetValue((int)crme_dependentmaintenanceState.Canceled);
            //        state.Status = new OptionSetValue(3);

            //        // Point the Request to the case whose state is being changed
            //        state.EntityMoniker = new EntityReference("crme_dependentmaintenance", entity.Id);

            //        // Execute the Request
            //        SetStateResponse stateSet = (SetStateResponse)OrganizationService.Execute(state);
            //        Logger.WriteDebugMessage("Change should have worked");

            //    }
            //    else
            //    {
            //        Logger.WriteDebugMessage("We can NOT change");
            //    }
            #endregion
            return true;
        }

        private int GetTransactionStatus()
        {
            var entity = (Entity)PluginExecutionContext.InputParameters["Target"];

            return entity.GetAttributeValue<OptionSetValue>("crme_txnstatus").Value;
        }

        private void ExecOrchestration()
        {

            TracingService.Trace("Getting Transaction status");
            var transactionStatus = GetTransactionStatus();
            TracingService.Trace("Transaction status is: " + transactionStatus);


            TracingService.Trace("GetSecondaryEntity().Id: " + GetSecondaryEntity().Id);
            TracingService.Trace("USer ID is: " + PluginExecutionContext.InitiatingUserId);
            TracingService.Trace("Organization Name is: " + PluginExecutionContext.OrganizationName);
            

            if (transactionStatus != _Submitted)
                return;


            var addDependentOrchestrationRequest = new AddDependentOrchestrationRequest
            {
                DependentMaintenanceId = GetSecondaryEntity().Id,
                OrganizationName = PluginExecutionContext.OrganizationName,
                UserId = PluginExecutionContext.InitiatingUserId,
                MessageId = PluginExecutionContext.CorrelationId.ToString()
            };

            TracingService.Trace("1");
            CrmeSettings crmeSetting = SettingsHelper.GetSettingValues(OrganizationService, PluginExecutionContext);
            TracingService.Trace("2");
            getSettingValues();
            TracingService.Trace("calling end point");
            // VRMRest.Utility.SendAsync(new Uri(crmeSetting.RestEndPointForVimt), "AddDependent#AddDependentOchestrationRequest", addDependentOrchestrationRequest, crmeSetting.LogSettings, null);
            VRMRest.Utility.SendAsync(new Uri(crmeSetting.RestEndPointForVimt), "AddDependent#AddDependentOchestrationRequest", addDependentOrchestrationRequest, crmeSetting.LogSettings, null, 0, _crmAuthTokenConfig, TracingService);
            //addDependentOrchestrationRequest.SendAsyncThread(TransactionMessageProcessType, true);

            //addDependentOrchestrationRequest.Send(TransactionMessageProcessType);
        }
        #endregion

        internal void getSettingValues()
        {
            _logTimer = McsSettings.GetSingleSetting<bool>("udo_noteslogtimer");
            _logSoap = McsSettings.GetSingleSetting<bool>("udo_noteslogsoap");


            _uri = McsSettings.GetSingleSetting<string>(_vimtRestEndpointField);

            string parentAppId = McsSettings.GetSingleSetting<string>("udo_oauthresourceid");
            string clientAppId = McsSettings.GetSingleSetting<string>("udo_oauthclientid");
            string clientSecret = McsSettings.GetSingleSetting<string>("udo_oauthclientsecret");
            string tenentId = McsSettings.GetSingleSetting<string>("udo_aadtenent");
            string apimsubscriptionkey = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkey");
            string apimsubscriptionkeyS = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeysouth");
            string apimsubscriptionkeyE = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeyeast");
            //Create the token from settings
            _crmAuthTokenConfig = new CRMAuthTokenConfiguration
            {
                ParentApplicationId = parentAppId,
                ClientApplicationId = clientAppId,
                ClientSecret = clientSecret,
                TenantId = tenentId,
                ApimSubscriptionKey = apimsubscriptionkey,
                ApimSubscriptionKeyE = apimsubscriptionkeyE,
                ApimSubscriptionKeyS = apimsubscriptionkeyS
            };
        }

        #region  _veteranRetrievePostStageRunner Methods
        public override string McsSettingsDebugField
        {
            get { return "crme_dependentmaintenance"; }
        }

        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.PreEntityImages["pre"];
        }

        public override Entity GetSecondaryEntity()
        {
            return (Entity)PluginExecutionContext.InputParameters["Target"];
        }
        #endregion
    }
}
