using VRM.Integration.UDO.Common;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Awards.Messages;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VIMT.ClaimantWebService.Messages;
namespace VRM.Integration.UDO.Awards.Processors
{
    class UDOcreateProceedsProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOcreateProceedsProcessor";
        private string LogBuffer { get; set; }
        
        public IMessageBase Execute(UDOcreateProceedsRequest request)
        {
            //var request = message as createProceedsRequest;
            UDOcreateProceedsResponse response = new UDOcreateProceedsResponse();
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;
            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

       

            OrganizationServiceProxy OrgServiceProxy;

            #region connect to CRM
            try
            {
                var CommonFunctions = new CRMConnect();

                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateProceedsProcessor Processor, Connection Error", connectException.Message); 
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                // prefix = foawdinfofindOtherAwardInformationRequest();
                var findOtherAwardInformationRequest = new VIMTfoawdinfofindOtherAwardInformationRequest();
                findOtherAwardInformationRequest.LogTiming = request.LogTiming;
                findOtherAwardInformationRequest.LogSoap = request.LogSoap;
                findOtherAwardInformationRequest.Debug = request.Debug;
                findOtherAwardInformationRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findOtherAwardInformationRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findOtherAwardInformationRequest.RelatedParentId = request.RelatedParentId;
                findOtherAwardInformationRequest.UserId = request.UserId;
                findOtherAwardInformationRequest.OrganizationName = request.OrganizationName;

                findOtherAwardInformationRequest.mcs_ptcpntvetid = request.ptcpntVetId;
                findOtherAwardInformationRequest.mcs_ptcpntbeneid = request.ptcpntBeneId;
                findOtherAwardInformationRequest.mcs_ptcpntrecipid = request.ptcpntRecipId;
                findOtherAwardInformationRequest.mcs_awardtypecd = request.awardTypeCd;

                if (request.LegacyServiceHeaderInfo != null)
                {
                    findOtherAwardInformationRequest.LegacyServiceHeaderInfo = new VIMT.ClaimantWebService.Messages.HeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }
                // TODO(TN): Commented to remediate
                var findOtherAwardInformationResponse = new VIMTfoawdinfofindOtherAwardInformationResponse();
                // var findOtherAwardInformationResponse = findOtherAwardInformationRequest.SendReceive<VIMTfoawdinfofindOtherAwardInformationResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call";

                response.ExceptionMessage = findOtherAwardInformationResponse.ExceptionMessage;
                response.ExceptionOccured = findOtherAwardInformationResponse.ExceptionOccured;

                var proceedsCount = 0;
                var requestCollection = new OrganizationRequestCollection();

                if (findOtherAwardInformationResponse.VIMTfoawdinforeturnclmsInfo.VIMTfoawdinfoawardInfoclmsInfo.VIMTfoawdinfoaccountBalancesclmsInfo != null)
                {
                    var accountBalance = findOtherAwardInformationResponse.VIMTfoawdinforeturnclmsInfo.VIMTfoawdinfoawardInfoclmsInfo.VIMTfoawdinfoaccountBalancesclmsInfo;
                    System.Collections.Generic.List<UDOcreateProceedsMultipleResponse> UDOcreateProceedsArray = new System.Collections.Generic.List<UDOcreateProceedsMultipleResponse>();
                    foreach (var accountBalanceItem in accountBalance)
                    {
                        var responseIds = new UDOcreateProceedsMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_proceed";
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        if (accountBalanceItem.mcs_balance != string.Empty)
                        {
                            thisNewEntity["udo_balanceamount"] = moneyStringFormat(accountBalanceItem.mcs_balance);
                        }
                        if (accountBalanceItem.mcs_code != string.Empty)
                        {
                            thisNewEntity["udo_type"] = accountBalanceItem.mcs_code;
                        }
                        if (!string.IsNullOrEmpty(accountBalanceItem.mcs_name))
                        {
                            thisNewEntity["udo_description"] = accountBalanceItem.mcs_name;
                        }
                        if (request.UDOcreateProceedsRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreateProceedsRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        CreateRequest createData = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createData);
                        proceedsCount += 1;

                    }
                }
                #region Create records

                if (proceedsCount > 0)
                {
                    var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, request.UserId, request.Debug);

                    if (_debug)
                    {
                        LogBuffer += result.LogDetail;
                        LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                    }

                    if (result.IsFaulted)
                    {
                        LogHelper.LogError(request.OrganizationName, request.UserId, method, result.ErrorDetail);
                        response.ExceptionMessage = result.FriendlyDetail;
                        response.ExceptionOccured = true;
                        return response;
                    }
                }

                string logInfo = string.Format("proceed Records Created: {0}", proceedsCount);
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "proceed Records Created", logInfo);
                #endregion

                //added to generated code
                if (request.AwardId != null)
                {
                    var parent = new Entity();
                    parent.Id = request.AwardId;
                    parent.LogicalName = "udo_award";
                    parent["udo_proceedscomplete"] = true;
                    //parent["udo_proceedsmessage"] = "";
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOcreateProceedsProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Award Proceeds Data";
                response.ExceptionOccured = true;
                return response;
            }
        }
        private static string dateStringFormat(string date)
        {
            if (date == null) return null;
            if (date.Length == 10) return date;
            if (date.Length < 8) return date;

            date = date.Insert(2, "/");
            date = date.Insert(5, "/");
            return date;
        }
        private static string moneyStringFormat(string thisField)
        {
            var returnField = "";
            try
            {
                Double newValue = 0;
                if (Double.TryParse(thisField, out newValue))
                {
                    returnField = string.Format("{0:C}", newValue);
                }
                else
                {
                    returnField = "$0.00";
                }
            }
            catch (Exception ex)
            {
                returnField = ex.Message;
            }
            return returnField;

        }
    }
}
