using MCSPlugins;
using Microsoft.Xrm.Sdk;
using System;
using UDO.LOB.Core;
using UDO.LOB.FNOD.Messages;
using VRMRest;

namespace UDO.CustomActions.FNOD.Plugins.SsaInquiry
{
    public class UDOSsaDeathMatchInquiryRunner : UDOActionRunner
    {
        #region Members
        protected UDOHeaderInfo _headerInfo;
        protected string _parententityname = string.Empty;
        protected Guid _parententityid = Guid.Empty;
        protected string _filenumber = string.Empty;
        protected string _dob = string.Empty;
        protected string _firstname = string.Empty;
        protected string _lastname = string.Empty;
        protected string _vetfilenumber = string.Empty;
        #endregion

        #region constructor
        public UDOSsaDeathMatchInquiryRunner(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            // Set configuration values
            _logTimerField = "udo_fnodlogtimer";
            _logSoapField = "udo_fnodlogsoap";
            _debugField = "udo_fnod";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_fnodtimeout";
            _validEntities = new string[] { "va_fnod", "udo_person" };
        }
        #endregion

        #region DoAction method
        public override void DoAction()
        {
            try
            {
                // Set settings and class member values
                tracer.Trace("Setting settings and class member values");
                _method = "DoAction";
                GetSettingValues();
                _headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

                // Confirm there is no data issue
                if (DataIssue = !GetInputParameters())
                {
                    return;
                }

                // Build request
                tracer.Trace("Building request object");
                var ssaInquiryRequest = new UDOSsaDeathMatchInquiryRequest
                {
                    MessageId = PluginExecutionContext.CorrelationId.ToString(),
                    OrganizationName = PluginExecutionContext.OrganizationName,
                    UserId = PluginExecutionContext.InitiatingUserId,
                    Debug = _debug,
                    LogSoap = _logSoap,
                    LogTiming = _logTimer,
                    LegacyServiceHeaderInfo = _headerInfo,
                    RelatedParentId = _parententityid,
                    RelatedParentEntityName = _parententityname,
                    mcs_SsaInquiryInput = new UDOSsaInquiryInput
                    {
                        mcs_fileNumberField = _filenumber,
                        mcs_dobField = _dob,
                        mcs_firstNameField = _firstname,
                        mcs_lastNameField = _lastname,
                        mcs_vetFileNumberField = _vetfilenumber
                    }
                };

                // Setup Log Settings
                tracer.Trace("Setting up log settings");
                LogSettings logSettings = new LogSettings()
                {
                    Org = PluginExecutionContext.OrganizationName,
                    ConfigFieldName = "RESTCALL",
                    UserId = PluginExecutionContext.InitiatingUserId,
                    callingMethod = Logger.setModule
                };

                // Send request
                tracer.Trace("Calling UDOSsaDeathMatchInquiryRequest");
                var ssaInquiryResponse = Utility.SendReceive<UDOSsaDeathMatchInquiryResponse>(_uri, "UDOSsaDeathMatchInquiryRequest", ssaInquiryRequest, logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
                tracer.Trace("Returned from UDOSsaDeathMatchInquiryRequest");

                // Check if error occurred
                tracer.Trace("Checking response to validate if error occurred");
                if (ssaInquiryResponse.SsaDeathMatchException != null && ssaInquiryResponse.SsaDeathMatchException.ExceptionOccurred)
                {
                    _responseMessage = SetResponseMessage(ssaInquiryResponse.SsaDeathMatchException.ExceptionMessage);
                    ExceptionOccurred = true;
                    return; // don't set result if exception occurred
                }

                // Return output params
                tracer.Trace("Returning output parameteters to custom action");
                PluginExecutionContext.OutputParameters["result"] = ssaInquiryResponse.DateOfDeath;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(SetResponseMessage(ex.Message));
            }
        }
        #endregion

        private bool GetInputParameters()
        {
            try
            {
                tracer.Trace("Retrieving ParentEntityId from input parameters");
                _parententityid = ((EntityReference)PluginExecutionContext.InputParameters["ParentEntityReference"]).Id;

                tracer.Trace("Retrieving ParentEntityName from input parameters");
                _parententityname = ((EntityReference)PluginExecutionContext.InputParameters["ParentEntityReference"]).LogicalName;

                tracer.Trace("Retrieving FileNumber from input parameters");
                _filenumber = (string)PluginExecutionContext.InputParameters["FileNumber"];

                tracer.Trace("Retrieving DOB from input parameters");
                _dob = (string)PluginExecutionContext.InputParameters["DOB"];

                tracer.Trace("Retrieving FirstName from input parameters");
                _firstname = (string)PluginExecutionContext.InputParameters["FirstName"];

                tracer.Trace("Retrieving LastName from input parameters");
                _lastname = (string)PluginExecutionContext.InputParameters["LastName"];

                tracer.Trace("Retrieving VetFileNumber from input parameters");
                _vetfilenumber = (string)PluginExecutionContext.InputParameters["VetFileNumber"];

                if (string.IsNullOrEmpty(_filenumber) || string.IsNullOrEmpty(_dob) || string.IsNullOrEmpty(_firstname) || string.IsNullOrEmpty(_lastname) || string.IsNullOrEmpty(_vetfilenumber))
                {
                    _responseMessage = SetResponseMessage($"One or more of the required input parameters for SSA Inquiry was empty.");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(SetResponseMessage($"One or more of the required input parameters for SSA Inquiry was missing. {e.Message}"));
            }
        }

        private string SetResponseMessage(string message)
        {
            const string FAILURE_MESSAGE = "Unable to perform SSA Death Match Inquiry. Message: {0}; CorrelationId: {1}";
            return String.Format(FAILURE_MESSAGE, message, PluginExecutionContext.CorrelationId.ToString());
        }
    }
}
