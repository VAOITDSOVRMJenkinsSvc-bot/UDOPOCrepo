
namespace VRM.Integration.UDO.Awards.Processors
{
    using Microsoft.Xrm.Sdk.Messages;
    using VRM.Integration.UDO.Common;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using System;
    using VRM.Integration.Servicebus.Core;
    using VRM.Integration.UDO.Awards.Messages;
    using VRM.Integration.Servicebus.Logging.CRM.Util;
    using VIMT.ClaimantWebService.Messages;

    class UDOcreateEVRsProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOcreateEVRsProcessor";
        private string LogBuffer { get; set; }
        
        public IMessageBase Execute(UDOcreateEVRsRequest request)
        {
            //var request = message as createEVRsRequest;
            UDOcreateEVRsResponse response = new UDOcreateEVRsResponse();
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
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateEVRsProcessor Processor, Connection Error", connectException.Message); 
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

                var EVRCount = 0;
                var requestCollection = new OrganizationRequestCollection();

                if (findGeneralInformationByFileNumberResponse.VIMTfgenFNreturnclmsInfo.VIMTfgenFNevrsclmsInfo != null)
                {
                    var evr = findGeneralInformationByFileNumberResponse.VIMTfgenFNreturnclmsInfo.VIMTfgenFNevrsclmsInfo;
                    foreach (var evrItem in evr)
                    {
                        var responseIds = new UDOcreateEVRsMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_evr";
                        thisNewEntity["udo_name"] = "Appeal Summary";
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        if (evrItem.mcs_type != string.Empty)
                        {
                            thisNewEntity["udo_type"] = evrItem.mcs_type;
                        }
                        if (evrItem.mcs_status != string.Empty)
                        {
                            thisNewEntity["udo_status"] = evrItem.mcs_status;
                        }
                        if (evrItem.mcs_lastReported != string.Empty)
                        {
                            DateTime newDateTime;
                            var newDate = dateStringFormat(evrItem.mcs_lastReported);
                            if (DateTime.TryParse(newDate, out newDateTime))
                            {
                                thisNewEntity["udo_lastreported"] = newDateTime;
                            }
                        }
                        if (evrItem.mcs_exempt != string.Empty)
                        {
                            thisNewEntity["udo_exempt"] = evrItem.mcs_exempt;
                        }
                        if (evrItem.mcs_control != string.Empty)
                        {
                            thisNewEntity["udo_control"] = evrItem.mcs_control;
                        }
                        if (request.UDOcreateEVRsRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreateEVRsRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        CreateRequest createData = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createData);
                        EVRCount += 1;

                    }

                }
                #region Create records

                if (EVRCount > 0)
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

                string logInfo = string.Format("EVR Records Created: {0}", EVRCount);
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "EVR Records Created", logInfo);
                #endregion


                //added to generated code
                if (request.AwardId != null)
                {
                    var parent = new Entity();
                    parent.Id = request.AwardId;
                    parent.LogicalName = "udo_award";
                    parent["udo_evrcomplete"] = true;
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOcreateEVRsProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Award EVR Data";
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
