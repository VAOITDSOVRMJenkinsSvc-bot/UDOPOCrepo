using System;
using System.Security;
using System.Linq;
using MCSHelperClass;
using MCSPlugins;
using Microsoft.Xrm.Sdk;
using VRMRest;
using Microsoft.Xrm.Sdk.Query;
using UDO.Model;
using UDO.LOB.Core;
using UDO.LOB.VBMSeFolder.Messages;

namespace CustomActions.Plugins.Entities.VBMSeFolder
{
    public class UDOGetVBMSeFolderRunner : UDOActionRunner
    {
        #region Members
        protected Guid _idproofId = new Guid();
        protected Guid _veteranId = new Guid();
        protected Guid _ownerId = new Guid();
        protected string _ownerType = string.Empty;
        protected SecureString _ssid = null;
        protected string _aliasGuid = null;
        protected Entity _idProof = null;
        #endregion

        public UDOGetVBMSeFolderRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_vbmsefolderlogtimer";
            _logSoapField = "udo_vbmsefolderlogsoap";
            _debugField = "udo_vbmsefolder";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_vbmsefoldervimttimeout";
            _validEntities = new string[] { "udo_idproof" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            try
            {
                string msg = "";
                _method = "DoAction";
                _idproofId = Parent.Id;

                OptionSetValue vbmsLoad = null;
                using (var xrm = new UDOContext(OrganizationService))
                {
                    var IDProofs = from idProof in xrm.udo_idproofSet
                                   where idProof.Id == Parent.Id
                                   select new
                                   {
                                       contactid = idProof.Id,
                                       idp = idProof.udo_vbmsloadstate
                                   };
                    vbmsLoad = IDProofs.FirstOrDefault().idp;
                }

                tracer.Trace("Checking VBMS load state...");
                if (vbmsLoad != null)
                {
                    if (vbmsLoad.Value == 752280000) // Still Loading, LOB not finished
                    {
                        tracer.Trace("VBMS load state: still loading");
                        PluginExecutionContext.OutputParameters["Timeout"] = true;
                        return;
                    }
                    else if (vbmsLoad.Value == 752280001) // Loaded, return success
                    {
                        tracer.Trace("VBMS load state: loaded");
                        PluginExecutionContext.OutputParameters["Timeout"] = false;
                        //return;
                    }
                }

                if (!DidWeNeedData())
                {
                    DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                    return;
                }

                GetSettingValues();

                DeleteVBMSeFolder();

                var veteranReference = new UDOCreateVBMSeFolderRelatedEntitiesMultipleRequest()
                {
                    RelatedEntityFieldName = "udo_veteranid",
                    RelatedEntityId = _veteranId,
                    RelatedEntityName = "contact"
                };
                var udo_idproofReference = new UDOCreateVBMSeFolderRelatedEntitiesMultipleRequest()
                {
                    RelatedEntityFieldName = "udo_idproofid",
                    RelatedEntityId = _idproofId,
                    RelatedEntityName = "udo_idproof"
                };

                UDOHeaderInfo _headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

                var request = new UDOCreateVBMSeFolderRequest();
                request.UDOCreateVBMSeFolderRelatedEntitiesInfo = new[] { veteranReference, udo_idproofReference };
                request.LegacyServiceHeaderInfo = _headerInfo;
                request.MessageId = PluginExecutionContext.CorrelationId.ToString();
                request.ownerId = _ownerId;
                request.ownerType = _ownerType;
                request.Debug = _debug;
                request.LogSoap = _logSoap;
                request.LogTiming = _logTimer;
                request.VeteranId = _veteranId;
                request.udo_idProofId = _idproofId;
                request.UserId = PluginExecutionContext.InitiatingUserId;
                request.OrganizationName = PluginExecutionContext.OrganizationName;
                request.udo_ssn = _idProof.GetAttributeValue<AliasedValue>(string.Format("{0}.udo_ssn", _aliasGuid)).Value.ToString();
                
                request.RelatedParentEntityName = "udo_idproof";
                request.RelatedParentFieldName = "udo_idproofid";
                request.RelatedParentId = _idproofId;

                LogSettings _logSettings = new LogSettings()
                {
                    Org = PluginExecutionContext.OrganizationName,
                    ConfigFieldName = "RESTCALL",
                    UserId = PluginExecutionContext.InitiatingUserId,
                    callingMethod = Logger.setModule
                };

                msg = "...calling UDOCreateVBMSeFolderRequest";
                tracer.Trace(msg);

                var response = Utility.SendReceive<UDOCreateVBMSeFolderResponse>(_uri, "UDOCreateVBMSeFolderRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);

                msg = "...returned from UDOcreateVBMSeFolderRequest";
                tracer.Trace(msg);

                if (response.ExceptionOccurred)
                {
                    ExceptionOccurred = true;
                    _responseMessage = "An error occurred while executing the create VBMS eFolder LOB.  ";

                    if (!string.IsNullOrEmpty(response.ExceptionMessage))
                    {
                        _responseMessage += response.ExceptionMessage;
                    }
                    msg = ($"LOB Error message - {_responseMessage}. CorrelationId: {PluginExecutionContext.CorrelationId}");
                    tracer.Trace(msg);
                }
            }
            finally
            {
                tracer.Trace("Entered Finally");
                SetupLogger();
                tracer.Trace("Set up logger done.");
                ExecuteFinally();
                tracer.Trace("Exit Finally");
            }
        }

        private void DeleteVBMSeFolder()
        {
            string msg = "DeleteVBMSeFolder() started";
            tracer.Trace(msg);

            var searchField = "udo_idproofid";
            var searchGuid = _idproofId;

            var fetchAddresses = string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                      <entity name='udo_vbmsefolder'>
                                                        <attribute name='udo_vbmsefolderid' />
                                                        <order attribute='udo_name' descending='false' />
                                                        <filter type='and'>
                                                          <condition attribute='{0}' operator='eq' value='{1}' />
                                                        </filter>
                                                      </entity>
                                                    </fetch>", searchField, searchGuid);

            EntityCollection response = null;
            try
            {
                response = ElevatedOrganizationService.RetrieveMultiple(new FetchExpression(fetchAddresses));

                msg = string.Format("{0} to delete from udo_vbmsefolder.", response.Entities.Count);
                tracer.Trace(msg);

                if (response.Entities.Count > 0)
                {
                    foreach (var address in response.Entities)
                    {
                        ElevatedOrganizationService.Delete("udo_vbmsefolder", address.Id);
                    }
                }
            }
            catch (Exception)
            {
                msg = ($"Unable to delete VBMS eFolder using Elevated Organization Service.  Attempting to use normal Organization Service instead... CorrelationId: {PluginExecutionContext.CorrelationId}");
                tracer.Trace(msg);

                response = OrganizationService.RetrieveMultiple(new FetchExpression(fetchAddresses));

                msg = string.Format("{0} to delete from udo_vbmsefolder.", response.Entities.Count);
                tracer.Trace(msg);

                if (response.Entities.Count > 0)
                {
                    foreach (var address in response.Entities)
                    {
                        OrganizationService.Delete("udo_vbmsefolder", address.Id);
                    }
                }
            }
            finally
            {
                msg = "DeleteVBMSeFolder() finished";
                tracer.Trace(msg);
            }
        }

        internal bool DidWeNeedData()
        {
            string msg = "DidWeNeedData() started";
            tracer.Trace(msg);

            try
            {
                var gotData = false;

                _aliasGuid = String.Format("a_{0}", Guid.NewGuid().ToString("N"));
                var getParentFetchXml = String.Format(@" <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                              <entity name='udo_idproof'>
                                                <attribute name='udo_idproofid' />
                                                <attribute name='udo_veteran' />
                                                <attribute name='ownerid' />
                                                <attribute name='udo_vbmsefoldercomplete' />
                                                <filter type='and'>
                                                  <condition attribute='udo_idproofid' operator='eq' value='{0}' />
                                                </filter>
                                                <link-entity name='contact' from='contactid' to='udo_veteran' visible='false' link-type='outer' alias='{1}'>
                                                  <attribute name='udo_ssn' />
                                                </link-entity>
                                              </entity>
                                            </fetch>", _idproofId, _aliasGuid);

                var response = OrganizationService.RetrieveMultiple(new FetchExpression(getParentFetchXml));

                if (response.Entities.Count > 0)
                {
                    _idProof = response.Entities.FirstOrDefault();

                    gotData = true;

                    var veteranRef = _idProof.GetAttributeValue<EntityReference>("udo_veteran");
                    var ssid = _idProof.GetAttributeValue<AliasedValue>(string.Format("{0}.udo_ssn", _aliasGuid));
                    var status = _idProof.GetAttributeValue<Boolean>("udo_vbmsefoldercomplete");
                    var ownerRef = _idProof.GetAttributeValue<EntityReference>("ownerid");

                    if (veteranRef == null)
                    {
                        msg = "Veteran ID not found. Cannot retrieve eFolders.";
                        _responseMessage = msg;
                        tracer.Trace(msg);
                        return false;
                    }
                    else
                    {
                        _veteranId = veteranRef.Id;
                    }

                    if (ssid == null)
                    {
                        msg = "SSN not found. Cannot retrieve eFolders.";
                        _responseMessage = msg;
                        tracer.Trace(msg);
                        return false;
                    }
                    else
                    {
                        _ssid = MCSHelper.ConvertToSecureString_new(ssid.Value.ToString());
                    }

                    if (status)
                    {
                        msg = "VBMS eFolder is complete.";
                        tracer.Trace(msg);

                        Complete = true;

                        return false;
                    }

                    _ownerId = ownerRef.Id;
                    _ownerType = ownerRef.LogicalName;
                }

                msg = "DidWeNeedData() has been retrieved and set";
                tracer.Trace(msg);

                return gotData;
            }
            catch (Exception ex)
            {
                PluginError = true;
                throw new InvalidPluginExecutionException("Unable to run DidWeNeedData() due to: {0}".Replace("{0}", ex.Message));
            }
            finally
            {
                msg = "DidWeNeedData() finished";
                tracer.Trace(msg);
            }
        }
    }
}
