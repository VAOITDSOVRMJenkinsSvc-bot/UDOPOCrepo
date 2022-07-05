
namespace VRM.Integration.UDO.Awards.Processors
{
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Crm.Sdk.Messages;
    using System;
    using System.Collections.Generic;
    using VRM.Integration.Servicebus.Core;
    using VRM.Integration.UDO.Awards.Messages;
    using VRM.Integration.Servicebus.Logging.CRM.Util;
    using Logger = VRM.Integration.Servicebus.Core.Logger;
    using VRM.Integration.UDO.Common;
    using VIMT.ClaimantWebService.Messages;

    class UDOcreateDiariesProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOcreateDiariesProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateDiariesRequest request)
        {
            //var request = message as createDiariesRequest;
            UDOcreateDiariesResponse response = new UDOcreateDiariesResponse();
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
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateDiariesProcessor Processor, Connection Error", connectException.Message); 
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                // prefix = fgenFNfindGeneralInformationByFileNumberRequest();
                var findGeneralInformationByFileNumberRequest = new VIMTfgenFNfindGeneralInformationByFileNumberRequest();
                findGeneralInformationByFileNumberRequest.LogTiming = request.LogTiming;
                findGeneralInformationByFileNumberRequest.LogSoap = request.LogSoap;
                findGeneralInformationByFileNumberRequest.Debug = request.Debug;
                findGeneralInformationByFileNumberRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findGeneralInformationByFileNumberRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findGeneralInformationByFileNumberRequest.RelatedParentId = request.RelatedParentId;
                findGeneralInformationByFileNumberRequest.UserId = request.UserId;
                findGeneralInformationByFileNumberRequest.OrganizationName = request.OrganizationName;

                findGeneralInformationByFileNumberRequest.mcs_filenumber = request.fileNumber;
                if (request.LegacyServiceHeaderInfo != null)
                {
                    findGeneralInformationByFileNumberRequest.LegacyServiceHeaderInfo = new VIMT.ClaimantWebService.Messages.HeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }
                // TODO(TN): Commented to remediate
                var findGeneralInformationByFileNumberResponse = new VIMTfgenFNfindGeneralInformationByFileNumberResponse();
                // var findGeneralInformationByFileNumberResponse = findGeneralInformationByFileNumberRequest.SendReceive<VIMTfgenFNfindGeneralInformationByFileNumberResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call";

                response.ExceptionMessage = findGeneralInformationByFileNumberResponse.ExceptionMessage;
                response.ExceptionOccured = findGeneralInformationByFileNumberResponse.ExceptionOccured;

                var diariesCount = 0;
                var requestCollection = new OrganizationRequestCollection();

                if (findGeneralInformationByFileNumberResponse.VIMTfgenFNreturnclmsInfo.VIMTfgenFNdiariesclmsInfo != null)
                {
                    var diary = findGeneralInformationByFileNumberResponse.VIMTfgenFNreturnclmsInfo.VIMTfgenFNdiariesclmsInfo;
                    foreach (var diaryItem in diary)
                    {
                        var responseIds = new UDOcreateDiariesMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_diaries";
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        if (diaryItem.mcs_date != string.Empty)
                        {
                            DateTime newDateTime;
                            var newDate = dateStringFormat(diaryItem.mcs_date);
                            if (DateTime.TryParse(newDate, out newDateTime))
                            {
                                thisNewEntity["udo_date"] = newDateTime;
                            }
                        }
                        if (diaryItem.mcs_description != string.Empty)
                        {
                            thisNewEntity["udo_desc"] = diaryItem.mcs_description.Trim();
                        }
                        if (diaryItem.mcs_reasonCd != string.Empty)
                        {
                            thisNewEntity["udo_reasoncd"] = diaryItem.mcs_reasonCd;
                        }
                        if (diaryItem.mcs_reasonName != string.Empty)
                        {
                            thisNewEntity["udo_reasonname"] = diaryItem.mcs_reasonName;
                        }
                        if (request.UDOcreateDiariesRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreateDiariesRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        CreateRequest createData = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createData);
                        diariesCount += 1;

                    }

                }
                #region Create records

                if (diariesCount > 0)
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
                
                string logInfo = string.Format("Diary Records Created: {0}", diariesCount);
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Diary Records Created", logInfo);

                #endregion

                //added to generated code
                if (request.AwardId != null)
                {
                    var parent = new Entity();
                    parent.Id = request.AwardId;
                    parent.LogicalName = "udo_award";
                    parent["udo_diariescomplete"] = true;
                    //parent["udo_diariesmessage"] = "";
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOcreateDiariesProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Award Diary Data";
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
    }
}
