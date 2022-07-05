using System;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
//using VRM.Integration.UDO.Appeals.Messages;
using VRMRest;
using System.ServiceModel;
using System.Diagnostics;
using UDO.LOB.Core;
using UDO.Model;
using UDO.LOB.Appeals.Messages;

namespace CustomActions.Plugins.Entities.Appeals
{
    public class UDOGetAppealsRunner : UDOActionRunner
    {
        #region Members
        protected Guid _idproofId = new Guid();
        protected Guid _veteranId = new Guid();
        protected Guid _appealId = new Guid();
        protected Guid _ownerId = new Guid();
        protected string _appealKey = "";
        protected string _ownerType = string.Empty;
        protected string _fileNumber = "";
        protected UDOHeaderInfo _headerInfo;
        #endregion

        public UDOGetAppealsRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_appeallogtimer";
            _logSoapField = "udo_appeallogsoap";
            _debugField = "udo_appeal";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_appealvimttimeout";
            _validEntities = new string[] { "udo_idproof","udo_appeal" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            _method = "DoAction";

            GetSettingValues();
            _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            if (Parent.LogicalName == udo_idproof.EntityLogicalName)
            {
                _idproofId = Parent.Id;

                if (!DidWeNeedIDProofData())
                {
                    DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                    return;
                }

                CreateAppeals();
            }


            if (Parent.LogicalName == udo_appeal.EntityLogicalName)
            {
                _appealId = Parent.Id;

                if (!DidWeNeedAppealsData())
                {
                    DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                    return;
                }

                CreateAppealDetails();
            }

        }

        #region Create Appeals

        private void CreateAppeals()
        {

            var request = new UDOcreateUDOAppealsRequest();

            var veteranReference = new UDOcreateUDOAppealsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var udo_idproofReference = new UDOcreateUDOAppealsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_idproofid",
                RelatedEntityId = _idproofId,
                RelatedEntityName = "udo_idproof"
            };
            var references = new[] { veteranReference, udo_idproofReference };

            request.SSN = _fileNumber;
            request.RelatedParentEntityName = "udo_idproof";
            request.RelatedParentFieldName = "udo_idproofid";
            request.RelatedParentId = _idproofId;            
            request.LegacyServiceHeaderInfo = _headerInfo;           

            
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            request.Debug = _debug;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.idProofId = _idproofId;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            request.UDOcreateUDOAppealsRelatedEntitiesInfo = references;
            request.FileNumber = _fileNumber;
            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = Logger.setModule
            };

            tracer.Trace("calling UDOcreateUDOAppealsRequest");
            var response = Utility.SendReceive<UDOcreateUDOAppealsResponse>(_uri, "UDOcreateUDOAppealsRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateUDOAppealsRequest");

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Appeals LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
            }

        }

        private bool DidWeNeedIDProofData()
        {
            try
            {
                tracer.Trace("DidWeNeedData started");
                Logger.setMethod = "DidWeNeedData";
                var gotData = false;

                var aliasGuid = String.Format("a_{0}", Guid.NewGuid().ToString("N"));
                var getParentFetchXml = String.Format(@" <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                              <entity name='udo_idproof'>
                                                <attribute name='udo_idproofid' />
                                                <attribute name='udo_veteran' />
                                                <attribute name='ownerid' />
                                                <attribute name='udo_appealintegration' />
                                                <filter type='and'>
                                                  <condition attribute='udo_idproofid' operator='eq' value='{0}' />
                                                </filter>
                                                <link-entity name='contact' from='contactid' to='udo_veteran' visible='false' link-type='outer' alias='{1}'>
                                                  <attribute name='udo_filenumber' />
                                                </link-entity>
                                              </entity>
                                            </fetch>", _idproofId, aliasGuid);

                var response = OrganizationService.RetrieveMultiple(new FetchExpression(getParentFetchXml));

                if (response.Entities.Count > 0)
                {
                    var idProof = response.Entities.FirstOrDefault();

                    gotData = true;

                    var veteranRef = idProof.GetAttributeValue<EntityReference>("udo_veteran");
                    var fileNumber = idProof.GetAttributeValue<AliasedValue>(string.Format("{0}.udo_filenumber", aliasGuid));
                    var appealIntegration = idProof.GetAttributeValue<OptionSetValue>("udo_appealintegration");
                    var ownerRef = idProof.GetAttributeValue<EntityReference>("ownerid");

                    if (veteranRef == null)
                    {
                        _responseMessage = "Veteran ID not found. Cannot retrieve appeals.";
                        return false;
                    }
                    else
                        _veteranId = veteranRef.Id;

                    if (fileNumber == null)
                    {
                        _responseMessage = "File Number not found. Cannot retrieve appeals.";
                        return false;
                    }
                    else
                        _fileNumber = fileNumber.Value.ToString();


                    if (appealIntegration != null
                        && appealIntegration.Value == 752280002)
                    {
                        Complete = true;
                        return false;
                    }
                    _ownerId = ownerRef.Id;
                    _ownerType = ownerRef.LogicalName;
                }
                tracer.Trace("didWeNeedData have been retrieved and set");
                Logger.setMethod = "Execute";
                ////Logger.WriteDebugMessage("did not get data the 2nd time");
                return gotData;

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }

        #endregion

        #region Create Appeal Details

        private void CreateAppealDetails()
        {

            var request = new UDOcreateUDOAppealDetailsRequest();
            request.udo_appealId = _appealId;

            var veteranReference = new UDOcreateUDOAppealDetailsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var udo_ratingReference = new UDOcreateUDOAppealDetailsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_appealid",
                RelatedEntityId = request.udo_appealId,
                RelatedEntityName = "udo_appeal"
            };

            var references = new[] { veteranReference, udo_ratingReference };
            request.UDOcreateUDOAppealDetailsRelatedEntitiesInfo = references;
            
            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.AppealKey = _appealKey;
            request.RelatedParentId = request.udo_appealId;
            request.Debug = _debug;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            //  //Logger.WriteDebugMessage("Request Created");
            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = Logger.setModule
            };

            tracer.Trace("calling UDOcreateUDOAppealsRequest");
            var response = Utility.SendReceive<UDOcreateUDOAppealDetailsResponse>(_uri, "UDOcreateUDOAppealDetailsRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateUDOAppealsRequest");

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Appeal Details LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
            }

        }

        private bool DidWeNeedAppealsData()
        {
            try
            {
                tracer.Trace("DidWeNeedData started");
                Logger.setMethod = "DidWeNeedData";
                var gotData = false;

                var getParentResponse = OrganizationService.Retrieve(udo_appeal.EntityLogicalName, _appealId, new ColumnSet("udo_veteranid", "udo_appealkey", "udo_appealcomplete", "udo_callcomplete", "ownerid"));

                if (getParentResponse != null)
                {
                    gotData = true;
                    var getParent = getParentResponse.ToEntity<udo_appeal>();

                    if (getParent.udo_VeteranId == null)
                    {
                        // //Logger.WriteDebugMessage("no veteran id, exiting");
                        _responseMessage = "No Veteran ID found. Cannot get Appeal details.";
                        return false;
                    }
                    else
                    {
                        _veteranId = getParent.udo_VeteranId.Id;
                    }
                    if (getParent.udo_AppealKey == null)
                    {
                        ////Logger.WriteDebugMessage("no veteran id, exiting");
                        _responseMessage = "No Appeal Key found. Cannot get Appeal details.";
                        return false;
                    }
                    else
                    {
                        _appealKey = getParent.udo_AppealKey;
                    }
                    if (getParent.udo_appealcomplete == null)
                    {
                        ////Logger.WriteDebugMessage("no udo_appealcomplete, exiting");
                        _responseMessage = "Appeal Complete is null! Cannot get Appeal details.";
                        return false;
                    }
                    else
                    {
                        if (getParent.udo_appealcomplete.Value)
                        {
                            Complete = true;
                            return false;
                        }
                    }
                    if (getParent.udo_callcomplete == null)
                    {
                        ////Logger.WriteDebugMessage("no udo_callcomplete, exiting");
                        _responseMessage = "Cannot deterimine status of call. Cannot get Appeal details";
                        return false;
                    }
                    else
                    {
                        if (getParent.udo_callcomplete.Value)
                        {
                            _responseMessage = "Call is already complete. Cannot get Appeal details.";
                            return false;
                        }
                    }
                    if (getParent.OwnerId != null)
                    {
                        _ownerType = getParent.OwnerId.LogicalName;
                        _ownerId = getParent.OwnerId.Id;
                    }
                    else
                    {

                        return false;
                    }
                }

                //   //Logger.WriteDebugMessage("Ending didWeNeedData Method");
                Logger.setMethod = "Execute";

                return gotData;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }

        #endregion


    }
}
