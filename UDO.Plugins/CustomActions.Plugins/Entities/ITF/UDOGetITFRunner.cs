using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using VRMRest;
using System.ServiceModel;
using UDO.Model;
using UDO.LOB.Core;
using UDO.LOB.IntentToFile.Messages;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.ITF.Messages;


//using VRM.Integration.UDO.Contact.Messages;

namespace CustomActions.Plugins.Entities.ITF
{
    public class UDOGetITFRunner : UDOActionRunner
    {
        Guid _ownerId = new Guid();
        string _ownerType;
        Guid _veteranId = new Guid();
        List<UDOgeneratedITFRecord> _crmGeneratedRecords = new List<UDOgeneratedITFRecord>();

        string _PID = "";

        Guid parentId = Guid.Empty;

        public UDOGetITFRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_itflogtimer";
            _logSoapField = "udo_itflogsoap";
            _debugField = "udo_itf";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_itfvimttimeout";
            _validEntities = new string[] { "contact" };
        }

        public override void DoAction()
        {

            parentId = Parent.Id;

            OptionSetValue itfLoading = null;
            using (var xrm = new UDOContext(OrganizationService))
            {
                var Contacts = from contact in xrm.ContactSet
                                 where contact.Id == Parent.Id
                                 select new
                                 {
                                     contactid = contact.Id,
                                     itf = contact.udo_itfloading
                                 };
                itfLoading = Contacts.FirstOrDefault().itf;
            }
            if (itfLoading != null)
            {
                if (itfLoading.Value == 752280000) //Still Loading; LOB not finished
                {
                    PluginExecutionContext.OutputParameters["Timeout"] = true;
                    tracer.Trace("still loading,  timeout set to true, exiting");
                    return;
                }
                else if (itfLoading.Value == 752280001) //Loaded; return success
                {
                    PluginExecutionContext.OutputParameters["Timeout"] = false;
                    tracer.Trace("loaded,  timeout set to false, exiting");
                    return;
                }
            }

            if (!didWeNeedData())
            {
                DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                tracer.Trace("Data issue, exiting");
                return;
            }

            GetSettingValues();


            var request = new UDOcreateIntentToFileRequest();
            DeleteExistingITFs();
            tracer.Trace("done deleting ITFS");

            request.UDOgeneratedITFRecordsInfo = _crmGeneratedRecords.ToArray();

            //var idProofReference = new UDOcreateITFRelatedEntitiesMultipleRequest()
            //{
            //    RelatedEntityFieldName = "udo_idproofid",
            //    RelatedEntityId = Guid.Empty,
            //    RelatedEntityName = "udo_idproof"
            //};
            var veteranReference = new UDOcreateITFRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            //var references = new[] { veteranReference, idProofReference };
            var references = new[] { veteranReference };

            request.UDOcreateITFRelatedEntitiesInfo = references;
            UDOHeaderInfo HeaderInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);
            request.LegacyServiceHeaderInfo = HeaderInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.RelatedParentEntityName = "contact";
            request.RelatedParentFieldName = "udo_contactid";
            request.RelatedParentId = _veteranId;
            request.Debug = _debug;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            ////Logger.WriteDebugMessage("_PID:" + _PID);
            tracer.Trace("_PID:" + _PID);
            request.PtcpntId = Convert.ToInt64(_PID);
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;

            LogSettings _logSettings = new LogSettings() { Org = PluginExecutionContext.OrganizationName, ConfigFieldName = "RESTCALL", UserId = PluginExecutionContext.InitiatingUserId };
            tracer.Trace("calling UDOcreateIntentToFileRequest");
            var response = Utility.SendReceive<UDOcreateIntentToFileResponse>(_uri, "UDOcreateIntentToFileRequest", request, _logSettings,_timeOutSetting,_crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateIntentToFileRequest");

            if (response.ExceptionOccurred)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create ITF LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
            }
        }

        private void DeleteExistingITFs()
        {
            var itfFetchXML = string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                  <entity name='va_intenttofile'>
                                                    <attribute name='va_intenttofileid' />
                                                    <attribute name='udo_intenttofileexternalid' />
                                                    <attribute name='udo_fromlegacy' />
                                                    <filter type='and'>
                                                      <condition attribute='udo_veteranid' operator='eq' value='{0}' />
                                                    </filter>
                                                  </entity>
                                                </fetch>", parentId);

            var results = OrganizationService.RetrieveMultiple(new FetchExpression(itfFetchXML));

            if (results.Entities.Count > 0)
            {
                foreach (var itf in results.Entities)
                {
                    var fromLegacy = itf.GetAttributeValue<Boolean>("udo_fromlegacy");
                    var exteranlID = itf.GetAttributeValue<string>("udo_intenttofileexternalid");

                    if (fromLegacy)
                        ElevatedOrganizationService.Delete(va_intenttofile.EntityLogicalName, itf.Id);
                    else
                    {
                        if (!string.IsNullOrEmpty(exteranlID))
                        {
                            var existingITf = new UDOgeneratedITFRecord();
                            existingITf.ITFCrmGuid = itf.Id;
                            existingITf.ITFExternalID = long.Parse(exteranlID);

                            _crmGeneratedRecords.Add(existingITf);
                        }
                        else
                            ElevatedOrganizationService.Delete(va_intenttofile.EntityLogicalName, itf.Id);
                    }
                }
            }
        }

        private bool didWeNeedData()
        {
            try
            {
                Logger.setMethod = "didWeNeedData";
                // //Logger.WriteDebugMessage("Starting didWeNeedData Method");

                var results = OrganizationService.Retrieve(UDO.Model.Contact.EntityLogicalName, parentId, new ColumnSet("udo_participantid", "ownerid"));

                if (results == null)
                {
                    _responseMessage = "Cannot find contact. Unable to retrieve ITF.";
                    return false;
                }

                var contact = results.ToEntity<Entity>();

                _veteranId = contact.Id;
                _ownerType = contact.GetAttributeValue<EntityReference>("ownerid").LogicalName;
                _ownerId = contact.GetAttributeValue<EntityReference>("ownerid").Id;

                if (contact.GetAttributeValue<String>("udo_participantid") == null)
                {
                    _responseMessage = "No PID cannot retrieve ITF.";
                    return false;
                }
                else
                    _PID = contact.GetAttributeValue<String>("udo_participantid");

                Logger.setMethod = "Execute";

                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }
    }
}
