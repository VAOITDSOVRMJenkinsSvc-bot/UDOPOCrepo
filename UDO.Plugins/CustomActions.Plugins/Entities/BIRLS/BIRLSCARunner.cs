using System;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
//using VRM.Integration.UDO.BIRLS.Messages;
//using VRM.Integration.UDO.Common.Messages;
using VRMRest;
using System.ServiceModel;
using System.Diagnostics;
using UDO.Model;
using UDO.LOB.Core;
using UDO.LOB.BIRLS.Messages;

/// <summary>
/// Custom Action control to load BIRLS data in grid and on form
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace CustomActions.Plugins.Entities.BIRLS
{
    public class BIRLSCARunner : UDOActionRunner
    {
        protected Guid _ownerId = new Guid();
        protected string _ownerType;
        protected Guid _veteranId = new Guid();
        protected Guid _idproofId = new Guid();
        protected Guid _birlsId = new Guid();
        protected string _fileNumber = "";

        public BIRLSCARunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_birlslogtimer";
            _logSoapField = "udo_birlslogsoap";
            _debugField = "udo_birls";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_birlsvimttimeout";
            _validEntities = new string[] { "udo_idproof" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            _method = "DoAction";
            _idproofId = Parent.Id;

            if (!didWeNeedData())
            {
                DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                return;
            }

            GetSettingValues();
            UDOHeaderInfo _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            var request = new UDOgetBIRLSDataRequest();
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.udo_birlsId = _birlsId;

            var veteranReference = new UDOgetBIRLSDataRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var udo_birlsReference = new UDOgetBIRLSDataRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_birlsid",
                RelatedEntityId = _birlsId,
                RelatedEntityName = "udo_birls"
            };

            var references = new[] { veteranReference, udo_birlsReference };
            request.UDOgetBIRLSDataRelatedEntitiesInfo = references;
            request.LegacyServiceHeaderInfo = _headerInfo;
            request.RelatedParentEntityName = "udo_birls";
            request.RelatedParentFieldName = "udo_birlsid";
            request.RelatedParentId = _birlsId;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.fileNumber = _fileNumber;
            request.Debug = _debug;
            request.LogSoap = _logSoap;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            request.IDProofId = _idproofId;
            request.LogTiming = _logTimer;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            LogSettings _logSettings = new LogSettings() { Org = PluginExecutionContext.OrganizationName, ConfigFieldName = "RESTCALL", UserId = PluginExecutionContext.InitiatingUserId };

            tracer.Trace("calling UDOgetBIRLSDataRequest");
            var response = Utility.SendReceive<UDOgetBIRLSDataResponse>(_uri, "UDOgetBIRLSDataRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOgetBIRLSDataRequest");

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Get BIRLS Data LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
            }
        }

        private bool didWeNeedData()
        {
            try
            {
                tracer.Trace("DidWeNeedData started");
                Logger.setMethod = "DidWeNeedData";
                var gotData = false;

                using (var xrm = new UDOContext(OrganizationService))
                {
                    var birl = (from idp in xrm.udo_idproofSet
                                join vet in xrm.ContactSet on idp.udo_Veteran.Id equals vet.ContactId.Value
                                join awd in xrm.udo_birlsSet on idp.udo_idproofId.Value equals awd.udo_IDProofId.Id
                                where idp.udo_idproofId.Value == _idproofId
                                select new
                                {
                                    idp.udo_birlscomplete,
                                    vet.OwnerId,
                                    vet.udo_FileNumber,
                                    vet.ContactId,
                                    awd.udo_birlsId,
                                }).FirstOrDefault();


                    if (birl != null)
                    {
                        gotData = true;

                        if (birl.udo_birlsId.HasValue)
                        {
                            _birlsId = birl.udo_birlsId.Value;
                        }
                        else
                        {
                            _responseMessage = "No BIRLS ID retrieved from  IDProof recrod. Cannot get BIRLS info.";
                            return false;
                        }

                        if (!string.IsNullOrEmpty(birl.udo_FileNumber))
                        {
                            _fileNumber = birl.udo_FileNumber;
                        }
                        else
                        {
                            _responseMessage = "No file number found for this veteran; unable to retrieve BIRLS info.";
                            return false;
                        }
                        if (birl.OwnerId != null)
                        {
                            _ownerType = birl.OwnerId.LogicalName;
                            _ownerId = birl.OwnerId.Id;
                        }
                        else
                        {
                            _responseMessage = "Owner was not found for birls record. Cannot get BIRLS info.";
                            return false;
                        }
                        if (birl.udo_birlscomplete.HasValue)
                        {
                            if (birl.udo_birlscomplete.Value)
                            {
                                Complete = true;
                                return false;
                            }
                        }

                        if (birl.ContactId.HasValue)
                        {
                            _veteranId = birl.ContactId.Value;
                        }
                    }
                }

                Logger.setMethod = "Execute";

                return gotData;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(string.Format("Unable to DidWeNeedData due to: {0} \n InnerException: {1}", ex.Message, ex.InnerException));
            }
        }
    }
}
