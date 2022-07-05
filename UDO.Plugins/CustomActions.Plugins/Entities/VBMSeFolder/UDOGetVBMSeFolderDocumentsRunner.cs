using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using UDO.LOB.Core;
using UDO.LOB.VBMSeFolder.Messages;
using VRMRest;

namespace CustomActions.Plugins.Entities.VBMSeFolder
{
    public class UDOGetVBMSeFolderDocumentsRunner : UDOActionRunner
    {

        #region Members
        protected UDOHeaderInfo _headerInfo;
        /*protected byte[] _ssid;
        protected string _pid = string.Empty;
        protected string _filenumber = string.Empty;
        protected string _first = string.Empty;
        protected string _last = string.Empty;
        
        protected string _vet_first = string.Empty;
        protected string _vet_middle = string.Empty;
        protected string _vet_last = string.Empty;
        protected string _vet_dobstr = string.Empty;
        protected string _vet_gender = string.Empty;
        protected string _vet_pid = string.Empty;
        protected string _vet_filenumber = string.Empty;
        protected byte[] _vet_ssid;
        

        protected EntityReference _idproof;
        protected EntityReference _vet;
        protected EntityReference _owner;
         */
        protected string _documentVersionRefId;
        #endregion

        private const string FAILURE_MESSAGE = "{0} Unable to retrieve VBMS eFolder Documents. CorrelationId: {1}";

        public UDOGetVBMSeFolderDocumentsRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_vbmsefolderlogtimer";
            _logSoapField = "udo_vbmsefolderlogsoap";
            _debugField = "udo_vbmsefolder";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_vbmsefoldertimeout";
            _validEntities = new string[] { "udo_vbmsefolder" };
        }

        public override void DoAction()
        {
            try
            {
                _method = "DoAction";

                GetSettingValues();
                _headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

                if (DataIssue = !DidWeFindData()) return;

                #region Build Request

                var request = new UDOgetVBMSDocumentContentRequest
                {
                    MessageId = PluginExecutionContext.CorrelationId.ToString(),
                    OrganizationName = PluginExecutionContext.OrganizationName,
                    UserId = PluginExecutionContext.InitiatingUserId,
                    Debug = _debug,
                    LogSoap = _logSoap,
                    LogTiming = _logTimer,
                    LegacyServiceHeaderInfo = _headerInfo,
                    udo_VBMSeFolderId = Parent.Id,
                    udo_DocumentVersionRefId = _documentVersionRefId
                };

                #endregion

                #region Setup Log Settings

                LogSettings logSettings = new LogSettings()
                {
                    Org = PluginExecutionContext.OrganizationName,
                    ConfigFieldName = "RESTCALL",
                    UserId = PluginExecutionContext.InitiatingUserId,
                    callingMethod = Logger.setModule
                };

                #endregion

                tracer.Trace($" >> Invoking UDOgetVBMSDocumentContentRequest: Uri: {_uri?.ToString()}");
                Trace($" >> Invoking UDOgetVBMSDocumentContentRequest: Uri: {_uri?.ToString()}");
                var response = Utility.SendReceive<UDOgetVBMSDocumentContentResponse>(_uri, "UDOgetVBMSDocumentContentRequest",
                                    request, logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
                tracer.Trace($" << Invoked UDOgetVBMSDocumentContentRequest.");
                Trace($" << Invoked UDOgetVBMSDocumentContentRequest.");

                if (response.ExceptionOccurred)
                {
                    _responseMessage = String.Format(FAILURE_MESSAGE, response.ExceptionMessage, PluginExecutionContext.CorrelationId.ToString());
                    ExceptionOccurred = true;
                    tracer.Trace($" ## ExceptionMessage: {response.ExceptionMessage}");
                    Trace($" ## ExceptionMessage: {response.ExceptionMessage}");
                    return; // don't set result if exception occurred
                }

                PluginExecutionContext.OutputParameters["result"] = new EntityReference("annotation", response.udo_attachmentId);
                tracer.Trace($" ## Created Note with Attachment: {response.udo_attachmentId.ToString()}");
                Trace($" ## Created Note with Attachment: {response.udo_attachmentId.ToString()}");

            }
            finally
            {
                Trace("Entered Finally");
                SetupLogger();
                Trace("Set up logger done.");
                ExecuteFinally();
                Trace("Exit Finally");
            }
        }

        internal bool DidWeFindData()
        {
            
            if (PluginExecutionContext.InputParameters.ContainsKey("DocumentVersionRefId") && (PluginExecutionContext.InputParameters["DocumentVersionRefId"] != null))
            {
                _documentVersionRefId = PluginExecutionContext.InputParameters["DocumentVersionRefId"].ToString();
            }
            else
            {
                var entity = OrganizationService.Retrieve(Parent.LogicalName, Parent.Id, new ColumnSet("udo_documentversionrefid"));
                if (entity != null)
                {
                    _documentVersionRefId = entity.GetAttributeValue<string>("udo_documentversionrefid");
                }
            }

            if (String.IsNullOrEmpty(_documentVersionRefId))
            {
                _responseMessage = String.Format(FAILURE_MESSAGE, "Document Version Ref ID was not specified or is missing.", PluginExecutionContext.CorrelationId.ToString());
                return false;
            }
            return true;
        }
    }
}
