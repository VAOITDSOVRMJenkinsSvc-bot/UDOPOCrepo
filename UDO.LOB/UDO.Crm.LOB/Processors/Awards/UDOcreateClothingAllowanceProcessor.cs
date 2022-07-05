
namespace VRM.Integration.UDO.Awards.Processors
{
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using System;
    using VRM.Integration.Servicebus.Core;
    using VRM.Integration.UDO.Awards.Messages;
    using VRM.Integration.Servicebus.Logging.CRM.Util;
    using VRM.Integration.UDO.Common;
    using VIMT.ClaimantWebService.Messages;

    class UDOcreateClothingAllowanceProcessor
    {
        private bool _debug { get; set; }


        private const string method = "UDOcreateClothingAllowanceProcessor";
        private string LogBuffer { get; set; }
        
        public IMessageBase Execute(UDOcreateClothingAllowanceRequest request)
        {
            //var request = message as createClothingAllowanceRequest;
            UDOcreateClothingAllowanceResponse response = new UDOcreateClothingAllowanceResponse();
            LogBuffer = string.Empty;
            _debug = request.Debug;
            var progressString = "Top of Processor";

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
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateClothingAllowanceProcessor Processor, Connection Error", connectException.Message); 
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
                findOtherAwardInformationRequest.mcs_ptcpntbeneid = String.IsNullOrEmpty(request.ptcpntBeneId) ? request.ptcpntVetId : request.ptcpntBeneId;
                findOtherAwardInformationRequest.mcs_ptcpntrecipid = String.IsNullOrEmpty(request.ptcpntRecipId) ? request.ptcpntVetId : request.ptcpntRecipId;
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

                var clothingCount = 0;
                var requestCollection = new OrganizationRequestCollection();
                if (findOtherAwardInformationResponse.VIMTfoawdinforeturnclmsInfo.VIMTfoawdinfoclothingAllowanceInfoclmsInfo != null)
                {
                    var clothingAllowanceInfo = findOtherAwardInformationResponse.VIMTfoawdinforeturnclmsInfo.VIMTfoawdinfoclothingAllowanceInfoclmsInfo;
                    System.Collections.Generic.List<UDOcreateClothingAllowanceMultipleResponse> UDOcreateClothingAllowanceArray = new System.Collections.Generic.List<UDOcreateClothingAllowanceMultipleResponse>();
                    foreach (var clothingAllowanceInfoItem in clothingAllowanceInfo)
                    {
                        var responseIds = new UDOcreateClothingAllowanceMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity("udo_clothingallowance");
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        if (clothingAllowanceInfoItem.mcs_eligibilityYear != string.Empty)
                        {
                            thisNewEntity["udo_eligibilityyear"] = clothingAllowanceInfoItem.mcs_eligibilityYear;
                        }
                        if (clothingAllowanceInfoItem.mcs_grossAward != string.Empty)
                        {
                            thisNewEntity["udo_grossaward"] = moneyStringFormat(clothingAllowanceInfoItem.mcs_grossAward);
                        }
                        if (clothingAllowanceInfoItem.mcs_incarcerationAdjustment != string.Empty)
                        {
                            thisNewEntity["udo_incarcerationadjustment"] = moneyStringFormat(clothingAllowanceInfoItem.mcs_incarcerationAdjustment);
                        }
                        if (clothingAllowanceInfoItem.mcs_netAward != string.Empty)
                        {
                            thisNewEntity["udo_netaward"] = moneyStringFormat(clothingAllowanceInfoItem.mcs_netAward);
                        }
                        if (request.UDOcreateClothingAllowanceRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreateClothingAllowanceRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        CreateRequest createExamData = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createExamData);
                        clothingCount += 1;
                    }
                }

                if (clothingCount > 0)
                {
                    #region Execute Multiple

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

                    #endregion
                }

                string logInfo = string.Format("Number of Clothing Allowances Created: {0}", clothingCount);
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Clothing Allowances Records Created", logInfo);
                //added to generated code
                if (request.AwardId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.AwardId;
                    parent.LogicalName = "udo_award";
                    parent["udo_clothingallowancecomplete"] = true;
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOcreateClothingAllowanceProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process clothingallowance Data";
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
