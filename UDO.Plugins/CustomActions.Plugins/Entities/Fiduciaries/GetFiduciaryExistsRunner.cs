using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCSPlugins;
using UDO.Model;
//using CustomActions.Plugins.Messages.Fiduciary;
//using VRM.Integration.UDO.Common.Messages;
using UDO.LOB.Core;
using UDO.LOB.PeoplelistPayeeCode.Messages;
using VRMRest;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
//using VRM.Integration.UDO.PeoplelistPayeeeCode.Messages;

namespace CustomActions.Plugins.Entities.Fiduciaries
{
    public class GetFiduciaryExistsRunner : UDOActionRunner
    {
        protected udo_person udoPerson = null;
        protected Guid _parentId = new Guid();
        protected Guid _veteranId = new Guid();

        public GetFiduciaryExistsRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_contactlogtimer";
            _logSoapField = "udo_contactlogsoap";
            _debugField = "udo_contact";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_contacttimeout";
            _validEntities = new string[] { "udo_person" };
        }

        public override void DoAction()
        {

            try
            {
                _method = "DoAction";
                _parentId = Parent.Id;

                if (!GetData(Parent))
                {
                    DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                    return;
                }

                GetSettingValues();

                var request = new UDOFiduciaryExistsRequest();

                UDOHeaderInfo _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

                request.LegacyServiceHeaderInfo = _headerInfo;
                request.MessageId = PluginExecutionContext.CorrelationId.ToString();
                request.RelatedParentEntityName = "contact";
                request.RelatedParentFieldName = "udo_contactid";
                request.RelatedParentId = udoPerson.udo_veteranId != null ? udoPerson.udo_veteranId.Id : Guid.Empty;
                request.Debug = _debug;
                request.LogSoap = _logSoap;
                request.LogTiming = _logTimer;
                request.UserId = PluginExecutionContext.InitiatingUserId;
                request.fileNumber = udoPerson.udo_FileNumber;
                //request.udo_ssn = udoPerson.udo_SSN;
                request.OrganizationName = PluginExecutionContext.OrganizationName;
                request.MessageId = PluginExecutionContext.CorrelationId.ToString();

                LogSettings _logSettings = new LogSettings()
                {
                    Org = PluginExecutionContext.OrganizationName,
                    ConfigFieldName = "RESTCALL",
                    UserId = PluginExecutionContext.InitiatingUserId,
                    callingMethod = "Get Fiduciary Exists Custom Action Runner"
                };

                UDOFiduciaryExistsResponse response = new UDOFiduciaryExistsResponse();

                if (!String.IsNullOrEmpty(request.fileNumber))
                    response = Utility.SendReceive<UDOFiduciaryExistsResponse>(_uri, "UDOFiduciaryExistsRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
                else
                {
                    DataIssue = true;
                    _responseMessage = "No identifier availalbe for updating Fiduciary Exists";

                    return;
                }

                if (response.ExceptionOccurred)
                {
                    ExceptionOccurred = true;
                    _responseMessage = "An error occurred while executing the Fiduciary Exists LOB.";

                    if (!string.IsNullOrEmpty(response.ExceptionMessage))
                    {
                        _responseMessage = response.ExceptionMessage;
                    }

                    Logger.WriteToFile(string.Format("Error message - {0}", _responseMessage));
                    tracer.Trace(string.Format("Error message - {0}", _responseMessage));
                    Trace(string.Format("Error message - {0}", _responseMessage));
                }
                else
                {
                    if (udoPerson.udo_fidexists == response.FiduciaryExists)
                    {
                        Complete = true;
                        PluginExecutionContext.OutputParameters["FiduciaryExists"] = response.FiduciaryExists;

                        return;
                    }

                    var updatedRecord = new Entity(Parent.LogicalName);
                    updatedRecord.Id = Parent.Id;
                    updatedRecord["udo_fidexists"] = response.FiduciaryExists;

                    OrganizationService.Update(updatedRecord);

                    PluginExecutionContext.OutputParameters["FiduciaryExists"] = response.FiduciaryExists;
                }
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

        private bool GetData(EntityReference entityRef)
        {            
            ColumnSet udoPersonColumns = new ColumnSet(
            //"udo_ssn",
            "udo_filenumber",
            "udo_fidexists",
            "udo_veteranid",
            "udo_dependentid");

            var found = false;

            try
            {
                var response = OrganizationService.Retrieve(udo_person.EntityLogicalName, Parent.Id, udoPersonColumns);

                if (response == null)
                {
                    _responseMessage = "Unable to retrieve Person record";
                }
                else
                {
                    udoPerson = response.ToEntity<udo_person>();
                    found = true;
                }
            }
            catch (Exception ex)
            {
                PluginError = true;
                ExceptionOccurred = true;
                _responseMessage = String.Format("An error occurred while retrieiving UDO Person. {0}{1}", Environment.NewLine, ex.Message);

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
            }

            return found;
        }
    }
}
