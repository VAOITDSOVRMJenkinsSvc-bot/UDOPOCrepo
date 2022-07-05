using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using VIMT.ClaimantWebService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Awards.Messages;
using VRM.Integration.UDO.Common;

namespace VRM.Integration.UDO.Awards.Processors
{
    class UDOcreateDeductionsProcessor
    {
        private bool _debug { get; set; }


        private const string method = "UDOcreateDeductionsProcessor";
        private string LogBuffer { get; set; }
        
        public IMessageBase Execute(UDOcreateDeductionsRequest request)
        {
            //var request = message as createDeductionsRequest;
            UDOcreateDeductionsResponse response = new UDOcreateDeductionsResponse();
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
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateDeductionsProcessor Processor, Connection Error", connectException.Message);     
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                //not mapped thisNewEntity["udo_awardid"]=??
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

                var requestCollection = new OrganizationRequestCollection();
                var deductionCount = 0;

                if (findOtherAwardInformationResponse.VIMTfoawdinforeturnclmsInfo.VIMTfoawdinfoawardInfoclmsInfo.VIMTfoawdinfodeductionsclmsInfo != null)
                {

                    var deduction = findOtherAwardInformationResponse.VIMTfoawdinforeturnclmsInfo.VIMTfoawdinfoawardInfoclmsInfo.VIMTfoawdinfodeductionsclmsInfo;
                    System.Collections.Generic.List<UDOcreateDeductionsMultipleResponse> UDOcreateDeductionsArray = new System.Collections.Generic.List<UDOcreateDeductionsMultipleResponse>();
                    foreach (var deductionItem in deduction)
                    {
                        var responseIds = new UDOcreateDeductionsMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_deduction";
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        if (deductionItem.mcs_code != string.Empty)
                        {
                            thisNewEntity["udo_type"] = moneyStringFormat(deductionItem.mcs_code);
                        }
                        if (deductionItem.mcs_balance != string.Empty)
                        {
                            thisNewEntity["udo_balanceamount"] = moneyStringFormat(deductionItem.mcs_balance);
                        }
                        if (deductionItem.mcs_amount != string.Empty)
                        {
                            thisNewEntity["udo_deductionamount"] = moneyStringFormat(deductionItem.mcs_amount);
                        }
                        if (deductionItem.mcs_name != string.Empty)
                        {
                            thisNewEntity["udo_description"] = deductionItem.mcs_name;
                        }
                        if (request.UDOcreateDeductionsRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreateDeductionsRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        CreateRequest createData = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createData);
                        deductionCount += 1;
                    }


                }
                if (deductionCount > 0)
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

                #region Log Results
                
                string logInfo = string.Format("Deductions Records Created: {0}", deductionCount);
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Deductions Records Created", logInfo);
                #endregion


                //added to generated code
                if (request.AwardId != null)
                {
                    var parent = new Entity();
                    parent.Id = request.AwardId;
                    parent.LogicalName = "udo_award";
                    parent["udo_deductioncomplete"] = true;
                    //parent["udo_deductionmessage"] = "";
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOcreateDeductionsProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Award Deductions Data";
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
