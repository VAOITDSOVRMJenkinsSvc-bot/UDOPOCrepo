using MCSPlugins;
using Microsoft.Xrm.Sdk;
using System;
using VRM.Integration.Servicebus.AddDependent.Messages;
using VRM.IntegrationServicebus.AddDependent.CrmModel;
using VRMRest;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class CrmeDependentMaintenanceUpdatePostStageRunner : MCSPlugins.PluginRunner
    {
        private const long _Submitted = 935950001;
        private const long _Cancelled = 935950004;
        private const long _Failure = 935950002;
        private const long _Success = 935950003;

		//CSDev
		internal CRMAuthTokenConfiguration _crmAuthTokenConfig;
		internal string _vimtRestEndpointField = "crme_restendpointforvimt";
		internal Uri _uri = null;
		private const int _searchTimeout = 100;

		//CSDev Debug Fields 
		internal string _debugField = "crme_dependentmaintenance";
		internal bool _debug;

		#region Constructor
		public CrmeDependentMaintenanceUpdatePostStageRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }
        #endregion

        #region Inernal Methods/Properties
        internal void Execute(IServiceProvider serviceProvider)
        {
			TracingService.Trace($"| >> Start { this.GetType().FullName}.Execute");

			//Avoid Recursive Calls - Abort if depth is greater than 1
			if (PluginExecutionContext.Depth > 1)
                return;


            if (!GetSecondaryEntity().Attributes.Contains("crme_txnstatus"))
            {
                TracingService.Trace($"| >> Error { this.GetType().FullName} Secondary Entity Does Not Contain crme_txnstatus - Status equal to null - must be save");
                return;
            }

            //Fail/success done in VIMT
            if (ChecForFailureorSuccess())
                return;

            if (CheckForCancel())
                return;

            //TracingService.Trace("About to do orchestration");

            ExecOrchestration();

			//End the timing for the plugin
			//Logger.WriteTxnTimingMessage(String.Format("Ending :{0}", GetType()));

			TracingService.Trace($"| >> End { this.GetType().FullName}.Execute");
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
        private bool CheckForCancel()
        {
            var entity = (Entity)PluginExecutionContext.InputParameters["Target"];

            var optionSetValue = GetTransactionStatus();

            if (optionSetValue != _Cancelled) 
                return false;
           // var childPlugin = new DeleteDependentMaintChildPlugin(OrganizationService, entity.Id);

            //PluginThread.Exec(childPlugin.Exec);
            #region commented out code for changing status or delete
            //TracingService.Trace("About to see if we can change to the cancelled state");
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
            //        TracingService.Trace("We can change");
            //        SetStateRequest state = new SetStateRequest();

            //        // Set the Request Object's Properties
            //        state.State = new OptionSetValue((int)crme_dependentmaintenanceState.Canceled);
            //        state.Status = new OptionSetValue(3);

            //        // Point the Request to the case whose state is being changed
            //        state.EntityMoniker = new EntityReference("crme_dependentmaintenance", entity.Id);

            //        // Execute the Request
            //        SetStateResponse stateSet = (SetStateResponse)OrganizationService.Execute(state);
            //        TracingService.Trace("Change should have worked");

            //    }
            //    else
            //    {
            //        TracingService.Trace("We can NOT change");
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
            var transactionStatus = GetTransactionStatus();

            if (transactionStatus != _Submitted)
                return;

			_debug = McsSettings.GetSingleSetting<bool>(_debugField);

			//CSDEv Rem
			//var addDependentOrchestrationRequest = new Messages.AddDependentOrchestrationRequest
			var addDependentOrchestrationRequest = new AddDependentOrchestrationRequest
            {
                DependentMaintenanceId = GetSecondaryEntity().Id,
                OrganizationName = PluginExecutionContext.OrganizationName,
                UserId = PluginExecutionContext.InitiatingUserId,
				Debug = _debug
			};

			CrmeSettings crmeSetting = SettingsHelper.GetSettingValues(OrganizationService, PluginExecutionContext);
			
			//CSDev
			GetSettingValues();

			//_uri = new Uri("https://dev.integration.d365.va.gov/veis/udo/DependentMaintenanceSvc/api/DependentMaintenance/AddDependentOrchestration");

			TracingService.Trace($"| >> Utility.SendAsync: Invoking Uri: {_uri.ToString()} \r\n RequestBody: {JsonHelper.Serialize(addDependentOrchestrationRequest, addDependentOrchestrationRequest.GetType())}");

			//CSDEv
			//VRMRest.Utility.SendAsync(new Uri(crmeSetting.RestEndPointForVimt), "AddDependent#AddDependentOchestrationRequest", addDependentOrchestrationRequest, crmeSetting.LogSettings, null);

			//This sends ar equest to the void, nothing back
			//This doesn't work 
			//Utility.SendAsync(_uri, "AddDependent#AddDependentOchestrationRequest", addDependentOrchestrationRequest, crmeSetting.LogSettings, null, _searchTimeout, Logger);

			//CSDEV we got rid of the logging method
			//Utility.Send(_uri, "AddDependent#AddDependentOchestrationRequest", addDependentOrchestrationRequest, _searchTimeout, _crmAuthTokenConfig, Logger);
			Utility.Send(_uri, "AddDependent#AddDependentOchestrationRequest", addDependentOrchestrationRequest, _searchTimeout, _crmAuthTokenConfig, TracingService);

			TracingService.Trace($"| >> Utility.SendAsync End: Debug Passed was: {_debug.ToString()}");
			TracingService.Trace($"| >> End Utility.SendAsync");

			//addDependentOrchestrationRequest.SendAsyncThread(TransactionMessageProcessType, true);

			//addDependentOrchestrationRequest.Send(TransactionMessageProcessType);
		}
        #endregion

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

		protected void GetSettingValues()
		{
			TracingService.Trace("getSettingValues started");

			//CSDev
			/*
			//_logTimer = PluginRunner.McsSettings.GetSingleSetting<bool>(_logTimerField);
			//_logSoap = PluginRunner.McsSettings.GetSingleSetting<bool>(_logSoapField);
			//var uri = PluginRunner.McsSettings.GetSingleSetting<string>(_vimtRestEndpointField);

			//if (string.IsNullOrEmpty(uri)) throw new NullReferenceException("NO URI FOUND, cannot call VIMT");
			*/

			_uri = new Uri(McsSettings.GetSingleSetting<string>(_vimtRestEndpointField));

			//CSdev
			/*
			_uri2 = new Uri(uri);
			_debug = McsSettings.GetSingleSetting<bool>(_debugField);
			_timeOutSetting = McsSettings.GetSingleSetting<int>(_vimtTimeoutField);
			_addperson = McsSettings.GetSingleSetting<bool>("udo_addperson");
			_MVICheck = McsSettings.GetSingleSetting<bool>("udo_mvicheck");
			_bypassMvi = McsSettings.GetSingleSetting<bool>("udo_bypassmvi");
			*/

			#region CRMAuthenticationToken
			//Get settings for AuthToken from McsSettings

			//CSDev
			//OAuthResourceid
			//string parentAppId = McsSettings.GetSingleSetting<string>("udo_parentapplicationid");
			string parentAppId = McsSettings.GetSingleSetting<string>("udo_oauthresourceid");
			//OAuthClientId
			//string clientAppId = McsSettings.GetSingleSetting<string>("udo_clientapplicationid");
			string clientAppId = McsSettings.GetSingleSetting<string>("udo_oauthclientid");
			string clientSecret = McsSettings.GetSingleSetting<string>("udo_oauthclientsecret");
			//CSDev
			//string tenentId = McsSettings.GetSingleSetting<string>("udo_tenantId");
			string tenentId = McsSettings.GetSingleSetting<string>("udo_aadtenent");
			//CSDev
			//string apimsubscriptionkey = McsSettings.GetSingleSetting<string>("udo_apimsubscriptionkey");
			string apimsubscriptionkey = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkey");
            
            string apimsubscriotionkeyS = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeysouth");

            string apimsubscriptionkeyE = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeyeast");
			//Create the token from settings
			_crmAuthTokenConfig = new CRMAuthTokenConfiguration
			{
				ParentApplicationId = parentAppId,
				ClientApplicationId = clientAppId,
				ClientSecret = clientSecret,
				TenantId = tenentId,
				ApimSubscriptionKey = apimsubscriptionkey,
                ApimSubscriptionKeyS = apimsubscriotionkeyS,
                ApimSubscriptionKeyE = apimsubscriptionkeyE
			};

            try
            {

                if (_debug)
                {

                    TracingService.Trace("CRMAuthTokenConfiguration : " + JsonHelper.Serialize<CRMAuthTokenConfiguration>(_crmAuthTokenConfig));

                }
            }
            catch (Exception e)
            {
                TracingService.Trace(e.Message);
            }

            #endregion
        }
    }
}
