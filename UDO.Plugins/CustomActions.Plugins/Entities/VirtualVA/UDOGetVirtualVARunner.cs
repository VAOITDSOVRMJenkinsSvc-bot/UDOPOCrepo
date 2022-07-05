using System;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using VRMRest;
using System.ServiceModel;
using System.Diagnostics;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.VirtualVA.Messages;
using UDO.Model;
using UDO.LOB.Core;
using UDO.LOB.VirtualVA.Messages;

namespace CustomActions.Plugins.Entities.VirtualVA
{
    public class UDOGetVirtualVARunner : UDOActionRunner
    {
        Guid _veteranId = new Guid();
        Guid _ownerId = new Guid();
        string _ownerType;
        string _fileNumber;
        Guid _idproofid = new Guid();

        public UDOGetVirtualVARunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_virtualvalogtimer";
            _logSoapField = "udo_virtualvalogsoap";
            _debugField = "udo_virtualva";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_virtualvavimttimeout";
            _validEntities = new string[] { "udo_idproof" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            _method = "DoAction";
            _idproofid = Parent.Id;

            if (!DidWeNeedData())
            {
                DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                return;
            }
            CreateVirtualVA();
        }

        #region Create VirtualVA
        private void CreateVirtualVA()
        {
            var request = new UDOcreateUDOVirtualVARequest();

            GetSettingValues();

            ////Logger.WriteDebugMessage("getDebug:" + _debug);
            var idReference = new UDOcreateUDOVirtualVARelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_idproofid",
                RelatedEntityId = _idproofid,
                RelatedEntityName = "udo_idproof"
            };
            var veteranReference = new UDOcreateUDOVirtualVARelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var references = new[] { veteranReference, idReference };

            request.UDOcreateUDOVirtualVARelatedEntitiesInfo = references;

            VirtualVAHeaderInfo HeaderInfo = GetHeaderInfo();
            request.LegacyServiceHeaderInfo = HeaderInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.Debug = _debug;
            request.RelatedParentEntityName = "udo_idproof";
            request.RelatedParentFieldName = "udo_idproofid";
            request.RelatedParentId = _idproofid;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            request.udo_idproofid = _idproofid;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            request.ClaimNbr = _fileNumber;
            ////Logger.WriteDebugMessage("VirtualVARetrieveMultiple,login" + HeaderInfo.HCValue1);
            tracer.Trace("VirtualVARetrieveMultiple,login" + HeaderInfo.HCValue1);
            ////Logger.WriteDebugMessage("VirtualVARetrieveMultiple, pwd" + HeaderInfo.HCValue2);
            tracer.Trace("VirtualVARetrieveMultiple, pwd" + HeaderInfo.HCValue2);

            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = Logger.setModule
            };

            tracer.Trace($"Calling UDOcreateUDOVirtualVARequest: {_uri}");
            var response = Utility.SendReceive<UDOcreateUDOVirtualVAResponse>(_uri, "UDOcreateUDOVirtualVARequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace($"Returned from UDOcreateUDOVirtualVARequest: VirtualVA records created: {response.UDOcreateUDOVirtualVAInfo?.Length}");

            if (response.ExceptionOccurred)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Virtual VA LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
            }
        }
        internal bool DidWeNeedData()
        {
            try
            {
                tracer.Trace("DidWeNeedData started");
                Logger.setMethod = "DidWeNeedData";
                var gotData = false;

                using (var xrm = new UDOContext(OrganizationService))
                {
                    var getParent = (from awd in xrm.udo_idproofSet
                                     join vet in xrm.ContactSet on awd.udo_Veteran.Id equals vet.ContactId.Value
                                     where awd.udo_idproofId.Value == _idproofid
                                     select new
                                     {
                                         awd.udo_virtualvacomplete,
                                         awd.udo_Veteran,
                                         vet.udo_FileNumber,
                                         vet.OwnerId
                                     }).FirstOrDefault();
                    if (getParent != null)
                    {
                        gotData = true;
                        if (getParent.udo_virtualvacomplete != null)
                        {
                            if (getParent.udo_virtualvacomplete.Value)
                            {
                                Complete = true;
                                return false;
                            }
                        }

                        if (getParent.udo_Veteran != null)
                        {
                            _veteranId = getParent.udo_Veteran.Id;
                        }
                        if (getParent.udo_FileNumber != null)
                        {
                            _fileNumber = getParent.udo_FileNumber;
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
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }
        #endregion


        internal VirtualVAHeaderInfo GetHeaderInfo()
        {
            ColumnSet userCols = new ColumnSet("va_stationnumber", "va_wsloginname", "va_applicationname", "va_ipaddress");
            Entity thisUser = OrganizationService.Retrieve("systemuser", PluginExecutionContext.InitiatingUserId, userCols);

            const string stationNumberIsNotAssignedForCrmUser = "Station Number is not assigned for CRM User.";
            const string vaStationnumber = "va_stationnumber";

            if (!thisUser.Attributes.ContainsKey(vaStationnumber))
                throw new Exception(stationNumberIsNotAssignedForCrmUser);

            const string wsLoginIsNotAssignedForCrmUser = "WS Login is not assigned for CRM User.";
            const string vaWsloginname = "va_wsloginname";

            if (!thisUser.Attributes.ContainsKey(vaWsloginname))
                throw new Exception(wsLoginIsNotAssignedForCrmUser);

            const string applicationNameIsNotAssignedForCrmUser = "Application Name is not assigned for CRM User.";
            const string vaApplicationname = "va_applicationname";

            if (!thisUser.Attributes.ContainsKey(vaApplicationname))
                throw new Exception(applicationNameIsNotAssignedForCrmUser);

            const string clientMachineIsNotAssignedForCrmUser = "Client Machine is not assigned for CRM User   .";
            const string vaIpAddress = "va_ipaddress";

            if (!thisUser.Attributes.ContainsKey(vaIpAddress))
                throw new Exception(clientMachineIsNotAssignedForCrmUser);

            var stationNumber = (string)thisUser[vaStationnumber];

            var loginName = (string)thisUser[vaWsloginname];
            // loginName = "CRMDAC";

            var applicationName = (string)thisUser[vaApplicationname];

            var clientMachine = (string)thisUser[vaIpAddress];

            var passId = string.Empty;

            using (var xrm = new UDOContext(OrganizationService))
            {
                var getsettings = from awd in xrm.va_systemsettingsSet
                                  select new
                                  {
                                      awd.va_name,
                                      awd.va_Description
                                  };
                foreach (var awd in getsettings)
                {
                    if (awd.va_name == "VVAPassword")
                    {
                        passId = awd.va_Description;
                    }
                    if (awd.va_name == "VVAUser")
                    {
                        loginName = awd.va_Description;
                    }
                }
            }

            return new VirtualVAHeaderInfo
            {
                StationNumber = stationNumber,

                HCValue1 = loginName,

                ApplicationName = applicationName,

                ClientMachine = clientMachine,

                HCValue2 = passId
            };
        }


    }
}
