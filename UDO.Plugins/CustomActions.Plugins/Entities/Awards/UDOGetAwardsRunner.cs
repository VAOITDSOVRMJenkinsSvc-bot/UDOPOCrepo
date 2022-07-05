using System;
using System.Linq;
using MCSPlugins;
using Microsoft.Xrm.Sdk;
using VRMRest;
using UDO.Model;
using UDO.LOB.Awards.Messages;
using UDO.LOB.Core;

namespace CustomActions.Plugins.Entities.Awards
{
    public class UDOGetAwardsRunner : UDOActionRunner
    {
        protected Guid _awardId = new Guid();
        protected Guid _ownerId = new Guid();
        protected string _ownerType = string.Empty;
        protected string _awardTypeCode = string.Empty;
        protected string _beneId = string.Empty;
        protected string _recipId = string.Empty;
        protected string _PID = string.Empty;
        protected string _FileNumber = string.Empty;

        public UDOGetAwardsRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_awardlogtimer";
            _logSoapField = "udo_awardlogsoap";
            _debugField = "udo_award";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_awardvimttimeout";
            _validEntities = new string[] { "udo_award" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            try
            {
                _method = "DoAction";
                _awardId = Parent.Id;
                if (!DidWeNeedData())
                {
                    DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                    return;
                }

                GetSettingValues();
                UDOHeaderInfo _headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

                var request = new UDOretrieveAwardRequest();
                request.AwardId = _awardId;


                request.LegacyServiceHeaderInfo = _headerInfo;
                request.MessageId = PluginExecutionContext.CorrelationId.ToString();

                request.Debug = _debug;

                request.RelatedParentEntityName = "udo_award";
                request.RelatedParentFieldName = "udo_awardid";
                request.RelatedParentId = _awardId;
                request.LogSoap = _logSoap;
                request.LogTiming = _logTimer;
                request.UserId = PluginExecutionContext.InitiatingUserId;
                request.OrganizationName = PluginExecutionContext.OrganizationName;

                request.awardTypeCd = _awardTypeCode;
                request.ptcpntBeneId = _beneId;
                request.ptcpntRecipId = _recipId;
                request.ptcpntVetId = _PID;
                request.ownerId = _ownerId;
                request.ownerType = _ownerType;
                request.fileNumber = _FileNumber;

                LogSettings _logSettings = new LogSettings()
                {
                    Org = PluginExecutionContext.OrganizationName,
                    ConfigFieldName = "RESTCALL",
                    UserId = PluginExecutionContext.InitiatingUserId,
                    callingMethod = Logger.setModule
                };

                tracer.Trace("calling UDOretrieveAwardRequest");
                Trace("calling UDOretrieveAwardRequest");

                var response = Utility.SendReceive<UDORetrieveAwardResponse>(_uri, "UDOretrieveAwardRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
                tracer.Trace("Returned from UDOretrieveAwardRequest");
                Trace("Returned from UDOretrieveAwardRequest");

                if (response.ExceptionOccured)
                {
                    ExceptionOccurred = true;
                    _responseMessage = "An error occurred while executing the Retrieve Awards LOB.";

                    if (!string.IsNullOrEmpty(response.ExceptionMessage))
                    {
                        _responseMessage = response.ExceptionMessage;
                    }

                    Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
                    tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
                    Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
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

        private bool DidWeNeedData()
        {
            try
            {
                tracer.Trace("DidWeNeedData started");
                Trace("DidWeNeedData started");
                Logger.setMethod = "DidWeNeedData";
                var gotData = false;

                using (var xrm = new UDOContext(OrganizationService))
                {

                    var getParent = (from awd in xrm.udo_awardSet
                                     join vet in xrm.ContactSet on awd.udo_VeteranId.Id equals vet.ContactId.Value
                                     where awd.udo_awardId.Value == _awardId
                                     select new
                                     {
                                         id = awd.udo_awardId,
                                         complete = awd.udo_awardinfocomplete,
                                         awardTypeCd = awd.udo_AwardTypeCode,
                                         ptcpntRecipId = awd.udo_PtcpntRecipID,
                                         ptcpntBeneId = awd.udo_PtcpntBeneID,
                                         ptcpntVetId = vet.udo_ParticipantId,
                                         vet.OwnerId,
                                         vet.udo_FileNumber
                                     }).FirstOrDefault();

                    if (getParent != null)
                    {
                        gotData = true;

                        //// nothing returned
                        //if (!awd.id.HasValue) return false;
                        //// already ran
                        if (getParent.complete.HasValue && getParent.complete.Value)
                        {
                            ////Logger.WriteDebugMessage("Award Retrieve already done");
                            Complete = true;
                            return false;
                        }

                        _awardTypeCode = getParent.awardTypeCd;
                        _beneId = getParent.ptcpntBeneId;
                        _recipId = getParent.ptcpntRecipId;
                        _PID = getParent.ptcpntVetId;

                        if (getParent.OwnerId != null)
                        {
                            _ownerType = getParent.OwnerId.LogicalName;
                            _ownerId = getParent.OwnerId.Id;
                        }
                        else
                        {
                            ////Logger.WriteDebugMessage("no owner");
                            return false;
                        }
                        if (getParent.udo_FileNumber != null)
                        {
                            _FileNumber = getParent.udo_FileNumber;
                        }
                        else
                        {

                            ////Logger.WriteDebugMessage("no filenumber");
                            _responseMessage = "No File Number. Cannot retrieve Award details";
                            return false;
                        }
                        return true;
                    }
                }
                tracer.Trace("didWeNeedData have been retrieved and set");
                Trace("didWeNeedData have been retrieved and set");
                Logger.setMethod = _method;
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
