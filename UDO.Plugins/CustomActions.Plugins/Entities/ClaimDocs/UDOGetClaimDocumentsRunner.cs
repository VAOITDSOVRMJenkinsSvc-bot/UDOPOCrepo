using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using UDO.LOB.ClaimDocuments.Messages;
//using VRM.Integration.UDO.ClaimDocuments.Messages;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.FNOD.Messages;
//using VRM.Integration.UDO.Letters.Messages;
using UDO.LOB.Core;
using VRMRest;

namespace CustomActions.Plugins.Entities.ClaimDocs
{
    public class UDOGetClaimDocumentsRunner : UDOActionRunner
    {

        #region Members
        protected UDOHeaderInfo _headerInfo;
        protected long _documentId;
        #endregion

        private const string FAILURE_MESSAGE = "{0} Unable to retrieve claim documents. CorrelationId: {1}";

        public UDOGetClaimDocumentsRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_claimdocslogtimer";
            _logSoapField = "udo_claimdocslogsoap";
            _debugField = "udo_claimdocs";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_claimdocstimeout";
            _validEntities = new string[] { "udo_mapdletter" };
        }

        public override void DoAction()
        {
            _method = "DoAction";

            GetSettingValues();
            _headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            if (DataIssue = !DidWeFindData()) return;

            #region Build Request
            var request = new getUDOClaimDocumentsRequest
            {
                MessageId = PluginExecutionContext.CorrelationId.ToString(),
                OrganizationName = PluginExecutionContext.OrganizationName,
                UserId = PluginExecutionContext.InitiatingUserId,
                Debug = _debug,
                LogSoap = _logSoap,
                LogTiming = _logTimer,
                LegacyServiceHeaderInfo = _headerInfo,
                MAPDLetterId = Parent.Id,
                documentId = _documentId
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

            tracer.Trace("calling getUDOClaimDocumentsRequest");
            var response = Utility.SendReceive<getUDOClaimDocumentsResponse>(_uri, "getUDOClaimDocumentsRequest", request, logSettings, _timeOutSetting,_crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from getUDOClaimDocumentsRequest");

            if (response.ExceptionOccured)
            {
                _responseMessage = response.ExceptionMessage;
                return; // don't set result if exception occurred
            }

            if (response.getUDOClaimDocumentsResponseInfo.Length < 1 ||
                response.getUDOClaimDocumentsResponseInfo[0] == null ||
                response.getUDOClaimDocumentsResponseInfo[0].udo_attachmentId == Guid.Empty)
            {
                _responseMessage = String.Format(FAILURE_MESSAGE, "Unable to find or access MAPD Letter.",PluginExecutionContext.CorrelationId.ToString());
                ExceptionOccurred = true;
                return;
            }


            PluginExecutionContext.OutputParameters["result"] = new EntityReference("annotation",
                response.getUDOClaimDocumentsResponseInfo[0].udo_attachmentId);

            PluginExecutionContext.OutputParameters["lettertxt"] = response.getUDOClaimDocumentsResponseInfo[0].udo_lettertxt;
        }

        internal bool DidWeFindData()
        {
            try
            {
                Logger.setMethod = "DidWeFindData";
                ////Logger.WriteDebugMessage("Starting didWeNeedData Method");

                var entity = OrganizationService.Retrieve(Parent.LogicalName, Parent.Id, new ColumnSet("udo_documentid"));

                if (entity == null)
                {
                    _responseMessage = String.Format(FAILURE_MESSAGE, "Unable to find or access MAPD Letter.", PluginExecutionContext.CorrelationId.ToString());
                    return false;
                }

                var docIdstr = entity.GetAttributeValue<string>("udo_documentid");

                if (!Int64.TryParse(docIdstr, out _documentId))
                {
                    _responseMessage = String.Format(FAILURE_MESSAGE, String.Format("No Document ID found - {0}. CorrelationId: {1}", docIdstr,PluginExecutionContext.CorrelationId.ToString()));
                }

                tracer.Trace("DidWeFindData: Fields have been retrieved and set.");
                Logger.setMethod = "Execute";

                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to execute didWeFindData due to: {0}".Replace("{0}", ex.Message));
            }
        }


    }
}
