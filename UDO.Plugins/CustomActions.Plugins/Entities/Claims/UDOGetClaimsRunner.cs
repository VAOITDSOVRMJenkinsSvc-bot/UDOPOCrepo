using System;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Linq;
using VRM.Integration.UDO.Claims.Messages;
using VRMRest;
using System.Net.Http;
using Microsoft.Xrm.Sdk.Query;
using UDO.Model;
using System.ServiceModel;
using System.Diagnostics;
using System.Threading;
using UDO.LOB.Core;
//using VRM.Integration.UDO.Common.Messages;
using UDO.LOB.Claims.Messages;

namespace CustomActions.Plugins.Entities.Claims
{
    public class UDOGetClaimsRunner : UDOActionRunner
    {
        #region Members
        protected string _fileNumber = "";
        protected string _PID = "";
        protected Guid _ownerId = new Guid();
        protected string _ownerType;
        protected Guid _veteranId = new Guid();
        protected Guid _idproofid = new Guid();
        protected Guid _dependentId = new Guid();
        protected Guid _claimId = new Guid();
        protected string _benefitClaimID = String.Empty;
        protected bool getClaims = false;
        protected UDOHeaderInfo _headerInfo;
        #endregion

        public UDOGetClaimsRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_claimlogtimer";
            _logSoapField = "udo_claimlogsoap";
            _debugField = "udo_claim";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_claimvimttimeout";
            _validEntities = new string[] { "udo_claim", "udo_idproof", "udo_dependant" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            try
            {
                _method = "DoAction";

                GetSettingValues();
                _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);


                if (Parent.LogicalName == udo_idproof.EntityLogicalName ||
                        Parent.LogicalName == udo_dependant.EntityLogicalName)
                {
                    if (Parent.LogicalName == udo_idproof.EntityLogicalName)
                    {
                        _idproofid = Parent.Id;
                        getClaims = true;
                    }

                    if (Parent.LogicalName == udo_dependant.EntityLogicalName)
                        _dependentId = Parent.Id;

                    if (!DidWeNeedClaimsData())
                    {
                        DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                        return;
                    }

                    CreateClaims();
                }


                if (Parent.LogicalName == udo_claim.EntityLogicalName)
                {
                    _claimId = Parent.Id;

                    if (!DidWeNeedClaimDetailData())
                    {
                        DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                        return;
                    }

                    CreateClaimDetail();
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

        #region Create Claims
        private void CreateClaims()
        {
            #region get data

            var request = new UDOcreateUDOClaimsSyncRequest();
            ////Logger.WriteDebugMessage("Need to get data");
            tracer.Trace("Need to get data");
            Trace("Need to get data");
            GetSettingValues();

            if (getClaims)
            {
                #region get claims for veterans
                var idReference = new UDOcreateUDOClaimsRelatedEntitiesMultipleResponse()
                {
                    RelatedEntityFieldName = "udo_idproofid",
                    RelatedEntityId = _idproofid,
                    RelatedEntityName = "udo_idproof"
                };

                var veteranReference = new UDOcreateUDOClaimsRelatedEntitiesMultipleResponse()
                {
                    RelatedEntityFieldName = "udo_veteranid",
                    RelatedEntityId = _veteranId,
                    RelatedEntityName = "contact"
                };

                var references = new[] { veteranReference, idReference };

                var traceContext = new DiagnosticsContext()
                {
                    AgentId = PluginExecutionContext.InitiatingUserId,
                    OrganizationName = PluginExecutionContext.OrganizationName,
                    MessageTrigger = "CustomActions.Plugins.Entities.Claims.UDO.GetClaimsRunner",
                    VeteranId = _veteranId,
                    StationNumber = _headerInfo.StationNumber
                };
                var requestVet = new UDOcreateUDOClaimsRequest()
                {
                    MessageId = PluginExecutionContext.CorrelationId.ToString(),
                    fileNumber = _fileNumber,
                    RelatedParentEntityName = "udo_idproof",
                    RelatedParentFieldName = "udo_idproofid",
                    RelatedParentId = _idproofid,
                    Debug = _debug,
                    LogSoap = _logSoap,
                    LogTiming = _logTimer,
                    UserId = PluginExecutionContext.InitiatingUserId,
                    OrganizationName = PluginExecutionContext.OrganizationName,
                    UDOcreateUDOClaimsRelatedEntitiesInfo = references,
                    LegacyServiceHeaderInfo = _headerInfo,
                    ownerId = _ownerId,
                    ownerType = _ownerType,
                    idProofId = _idproofid,
                    DiagnosticsContext=traceContext

                };

                LogSettings _logSettings = new LogSettings()
                {
                    Org = PluginExecutionContext.OrganizationName,
                    ConfigFieldName = "RESTCALL",
                    UserId = PluginExecutionContext.InitiatingUserId,
                    callingMethod = Logger.setModule
                };

                tracer.Trace("calling UDOcreateUDOClaimsRequest");
                Trace("calling UDOcreateUDOClaimsRequest");
                var response = Utility.SendReceive<UDOcreateUDOClaimsResponse>(_uri, "UDOcreateUDOClaimsRequest", requestVet, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
                tracer.Trace("Returned from UDOcreateUDOClaimsRequest");
                Trace("Returned from UDOcreateUDOClaimsRequest");

                if (response.ExceptionOccured)
                {
                    ExceptionOccurred = true;
                    _responseMessage = "An error occurred while executing the Create Claims Request LOB.";

                    if (!string.IsNullOrEmpty(response.ExceptionMessage))
                    {
                        _responseMessage = response.ExceptionMessage;
                    }

                    Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                }

                #endregion
            }
            else
            {
                #region get dependent claims
                _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);
                //TODO: identify where udo_dependentId went and uncomment                
                //request.udo_dependentId = _dependentId;

                var veteranReference = new UDOcreateUDOClaimsRelatedEntitiesMultipleResponse()
                {
                    RelatedEntityFieldName = "udo_veteranid",
                    RelatedEntityId = _veteranId,
                    RelatedEntityName = "contact"
                };
                var udo_claimReference = new UDOcreateUDOClaimsRelatedEntitiesMultipleResponse()
                {
                    RelatedEntityFieldName = "udo_dependentid",
                    RelatedEntityId = _dependentId,
                    RelatedEntityName = "udo_dependant"
                };
                var references = new[] { veteranReference, udo_claimReference };
                request.UDOcreateUDOClaimsRelatedEntitiesInfo = references;
                request.LegacyServiceHeaderInfo = _headerInfo;
                request.MessageId = PluginExecutionContext.CorrelationId.ToString();
                request.Debug = _debug;

                request.LogSoap = _logSoap;
                request.LogTiming = _logTimer;
                request.UserId = PluginExecutionContext.InitiatingUserId;
                request.OrganizationName = PluginExecutionContext.OrganizationName;

                request.fileNumber = _fileNumber;
                request.ownerId = _ownerId;
                request.ownerType = _ownerType;

                LogSettings _logSettings = new LogSettings()
                {
                    Org = PluginExecutionContext.OrganizationName,
                    ConfigFieldName = "RESTCALL",
                    UserId = PluginExecutionContext.InitiatingUserId,
                    callingMethod = Logger.setModule
                };


                tracer.Trace("calling UDOcreateUDOClaimsSyncRequest");
                Trace("calling UDOcreateUDOClaimsSyncRequest");
                var response = Utility.SendReceive<UDOcreateUDOClaimsResponse>(_uri, "UDOcreateUDOClaimsSyncRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
                tracer.Trace("Returned from UDOcreateUDOClaimsSyncRequest");
                Trace("Returned from UDOcreateUDOClaimsSyncRequest");

                if (response.ExceptionOccured)
                {
                    ExceptionOccurred = true;
                    _responseMessage = "An error occurred while executing the Create Evidence LOB.";

                    if (!string.IsNullOrEmpty(response.ExceptionMessage))
                    {
                        _responseMessage = response.ExceptionMessage;
                    }

                    Logger.WriteToFile(string.Format("Error message - {0}", _responseMessage));
                    tracer.Trace(string.Format("Error message - {0}", _responseMessage));
                    Trace(string.Format("Error message - {0}", _responseMessage));
                }
                #endregion
            }

            #endregion
        }

        private bool DidWeNeedClaimsData()
        {
            try
            {
                tracer.Trace("DidWeNeedData started");
                Trace("DidWeNeedData started");
                Logger.setMethod = "DidWeNeedData";
                var gotData = false;

                using (var xrm = new UDOContext(OrganizationService))
                {
                    if (getClaims)
                    {
                        ////Logger.WriteDebugMessage("getClaims==true");
                        tracer.Trace("getClaims==true");
                        Trace("getClaims==true");
                        var getParent = from awd in xrm.udo_idproofSet
                                        join vet in xrm.ContactSet on awd.udo_Veteran.Id equals vet.ContactId.Value
                                        where awd.udo_idproofId.Value == _idproofid
                                        select new
                                        {
                                            vet.ContactId,
                                            vet.udo_ParticipantId,
                                            vet.udo_FileNumber,
                                            vet.OwnerId,
                                            awd.udo_claimIntegration
                                        };
                        foreach (var awd in getParent)
                        {
                            gotData = true;
                            if (awd.udo_claimIntegration != null)
                            {
                                var claimInt = awd.udo_claimIntegration.Value;
                                ////Logger.WriteDebugMessage("claimInt==" + claimInt);

                                if (claimInt != 752280000)
                                {
                                    //anything but not started means we don't do anything
                                    Complete = true;
                                    return false;
                                }
                            }
                            if (awd.ContactId.HasValue)
                            {
                                _veteranId = awd.ContactId.Value;
                            }
                            if (awd.udo_ParticipantId != null)
                            {
                                _PID = awd.udo_ParticipantId;
                            }

                            if (awd.udo_FileNumber != null)
                            {
                                _fileNumber = awd.udo_FileNumber;
                            }
                            else
                            {
                                _responseMessage = "No File Number found. Cannot get claims";
                                return false;
                            }
                            if (awd.OwnerId != null)
                            {
                                _ownerType = awd.OwnerId.LogicalName;
                                _ownerId = awd.OwnerId.Id;
                            }
                            else
                            {
                                return false;
                            }

                        }
                    }
                    else
                    {
                        ////Logger.WriteDebugMessage("getClaims==false")
                        tracer.Trace("getClaims==false");
                        Trace("getClaims==false");

                        var getParent = from awd in xrm.udo_dependantSet
                                        where awd.Id == _dependentId
                                        select new
                                        {
                                            awd.udo_SSN,
                                            awd.udo_VeteranId,
                                            awd.udo_claimscomplete,
                                            awd.OwnerId
                                        };
                        foreach (var awd in getParent)
                        {
                            gotData = true;
                            if (awd.udo_SSN != null)
                            {
                                _fileNumber = awd.udo_SSN;
                            }
                            else
                            {
                                _responseMessage = "No File Number found. Cannot get claims";
                                return false;
                            }
                            if (awd.udo_VeteranId != null)
                            {
                                _veteranId = awd.udo_VeteranId.Id;
                            }
                            if (!awd.udo_claimscomplete.HasValue)
                            {
                                if (awd.udo_claimscomplete.Value)
                                {
                                    Complete = true;
                                    return false;
                                }
                            }

                            if (awd.OwnerId != null)
                            {
                                _ownerId = awd.OwnerId.Id;
                                _ownerType = awd.OwnerId.LogicalName;
                            }
                        }
                    }
                }
                Logger.setMethod = "Execute";

                return gotData;
            }
            catch (Exception ex)
            {
                PluginError = true;
                throw new InvalidPluginExecutionException("Unable to DidWeNeedClaimsData due to: {0}".Replace("{0}", ex.Message));
            }
        }
        #endregion

        #region Update Claim Details

        private void CreateClaimDetail()
        {

            var request = new UDOUpdateUDOClaimsRequest();
            request.udo_ClaimID = _claimId;
            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.Debug = _debug;
            request.udo_benefitClaimID = _benefitClaimID;
            request.RelatedParentEntityName = "udo_claim";
            request.RelatedParentFieldName = "udo_claimid";
            request.RelatedParentId = request.udo_ClaimID;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;

            // //Logger.WriteDebugMessage("Request Created");

            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = Logger.setModule
            };

            tracer.Trace("calling UDOUpdateUDOClaimsRequest");
            Trace("calling UDOUpdateUDOClaimsRequest");
            var response = Utility.SendReceive<UDOUpdateUDOClaimsResponse>(_uri, "UDOUpdateUDOClaimsRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOUpdateUDOClaimsRequest");
            Trace("Returned from UDOUpdateUDOClaimsRequest");

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Claims Request LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}", _responseMessage));
                tracer.Trace(string.Format("Error message - {0}", _responseMessage));
                Trace(string.Format("Error message - {0}", _responseMessage));
            }
        }

        private bool DidWeNeedClaimDetailData()
        {
            try
            {
                tracer.Trace("DidWeNeedData started");
                Trace("DidWeNeedData started");
                Logger.setMethod = "DidWeNeedData";
                var gotData = false;

                var retrievedClaim = OrganizationService.Retrieve(udo_claim.EntityLogicalName, _claimId, new ColumnSet("udo_claimidstring", "udo_benefitstatuscomplete"));

                if (retrievedClaim != null)
                {
                    gotData = true;
                    var claim = retrievedClaim.ToEntity<udo_claim>();
                    if (claim.udo_benefitstatuscomplete != null)
                    {
                        if (claim.udo_benefitstatuscomplete.Value)
                        {
                            Complete = true;
                            return false;
                        }
                    }
                    if (claim.udo_ClaimIDString != null)
                    {
                        _benefitClaimID = claim.udo_ClaimIDString;
                    }
                    else
                    {
                        _responseMessage = "Missing Claim ID. Cannot get claim details";
                        return false;
                    }
                }
                Logger.setMethod = "Execute";

                return gotData;
            }
            catch (Exception ex)
            {
                PluginError = true;
                throw new InvalidPluginExecutionException("Unable to DidWeNeedClaimDetailData due to: {0}".Replace("{0}", ex.Message));
            }
        }

        #endregion
    }
}
