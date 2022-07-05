using System;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using VRMRest;
using UDO.LOB.Core;
using UDO.LOB.Ratings.Messages;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.Ratings.Messages;
using UDO.Model;

namespace CustomActions.Plugins.Entities.Ratings
{
    public class UDOGetRatingsRunner : UDOActionRunner
    {
        protected Guid _ratingId = new Guid();
        protected Guid _ownerId = new Guid();
        protected string _ownerType;
        protected Guid _veteranId = new Guid();
        protected string _fileNumber = "";

        public UDOGetRatingsRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_ratingslogtimer";
            _logSoapField = "udo_ratingslogsoap";
            _debugField = "udo_ratings";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_ratingsvimttimeout";
            _validEntities = new string[] { "udo_rating" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            try
            {
                _method = "DoAction";
                _ratingId = Parent.Id;

                if (!didWeNeedData())
                {
                    DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                    return;
                }

                GetSettingValues();
                UDOHeaderInfo _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

                var request = new UDOfindRatingsRequest();

                var veteranReference = new UDOfindRatingsRelatedEntitiesMultipleRequest()
                {
                    RelatedEntityFieldName = "udo_veteranid",
                    RelatedEntityId = _veteranId,
                    RelatedEntityName = "contact"
                };
                var udo_ratingReference = new UDOfindRatingsRelatedEntitiesMultipleRequest()
                {
                    RelatedEntityFieldName = "udo_ratingid",
                    RelatedEntityId = _ratingId,
                    RelatedEntityName = "udo_rating"
                };
                var references = new[] { veteranReference, udo_ratingReference };

                request.UDOfindRatingsRelatedEntitiesInfo = references;
                request.LegacyServiceHeaderInfo = _headerInfo;
                request.RelatedParentEntityName = "udo_rating";
                request.RelatedParentFieldName = "udo_relatedratingid";
                request.RelatedParentId = _ratingId;
                request.MessageId = PluginExecutionContext.CorrelationId.ToString();
                request.fileNumber = _fileNumber;
                request.Debug = _debug;
                request.LogSoap = _logSoap;
                request.ownerId = _ownerId;
                request.ownerType = _ownerType;
                request.LogTiming = _logTimer;
                request.udo_ratingId = _ratingId;
                request.UserId = PluginExecutionContext.InitiatingUserId;
                request.OrganizationName = PluginExecutionContext.OrganizationName;
                request.MessageId = PluginExecutionContext.CorrelationId.ToString();
                LogSettings _logSettings = new LogSettings() { Org = PluginExecutionContext.OrganizationName, ConfigFieldName = "RESTCALL", UserId = PluginExecutionContext.InitiatingUserId };


                tracer.Trace("calling UDOfindRatingsRequest");
                Trace("calling UDOfindRatingsRequest");
                var response = Utility.SendReceive<UDOfindRatingsResponse>(_uri, "UDOfindRatingsRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
                tracer.Trace("Returned from UDOfindRatingsRequest");
                Trace("Returned from UDOfindRatingsRequest");

                if (response.ExceptionOccurred)
                {
                    ExceptionOccurred = true;
                    _responseMessage = "An error occurred while executing the Find Ratings Request LOB.";

                    if (!string.IsNullOrEmpty(response.ExceptionMessage))
                    {
                        _responseMessage = response.ExceptionMessage;
                    }

                    Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                    tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                    Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
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

        private bool didWeNeedData()
        {
            try
            {
                tracer.Trace("DidWeNeedData started");
                Trace("DidWeNeedData started");
                Logger.setMethod = "DidWeNeedData";
                var gotData = false;

                using (var xrm = new UDOContext(OrganizationService))
                {
                    var getParent = (from ra in xrm.udo_ratingSet
                                     join vet in xrm.ContactSet on ra.udo_VeteranId.Id equals vet.ContactId.Value
                                     where ra.udo_ratingId.Value == _ratingId
                                     select new
                                     {
                                         vet.udo_ParticipantId,
                                         vet.udo_FileNumber,
                                         vet.OwnerId,
                                         ra.udo_ratingcomplete,
                                         ra.udo_CallComplete,
                                         ra.udo_VeteranId
                                     }).FirstOrDefault();

                    if (getParent != null)
                    {
                        gotData = true;

                        if (getParent.udo_CallComplete.HasValue)
                        {
                            if (getParent.udo_CallComplete.Value)
                            {
                                _responseMessage = "Call complete. Cannot get Ratings.";
                                return false;
                            }
                        }

                        if (getParent.udo_ratingcomplete.HasValue)
                        {
                            if (getParent.udo_ratingcomplete.Value)
                            {
                                Complete = true;
                                return false;
                            }
                        }

                        if (getParent.udo_VeteranId != null)
                        {
                            _veteranId = getParent.udo_VeteranId.Id;
                        }

                        else
                        {
                            _responseMessage = "No Veteran ID found. Cannot get Ratings.";
                            return false;
                        }

                        if (getParent.udo_FileNumber != null)
                        {
                            _fileNumber = getParent.udo_FileNumber;
                        }
                        else
                        {
                            _responseMessage = "No File Number found. Cannot get Ratings.";
                            return false;
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
                }
                Logger.setMethod = "Execute";

                return gotData;
            }
            catch (Exception ex)
            {
                PluginError = true;
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }
    }
}