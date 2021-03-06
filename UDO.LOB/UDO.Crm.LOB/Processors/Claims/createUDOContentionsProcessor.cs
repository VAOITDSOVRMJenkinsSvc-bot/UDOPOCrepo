using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Linq;
using VIMT.ContentionServiceRemote.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Claims.Messages;
using VRM.Integration.UDO.Common;

/// <summary>
/// VIMT LOB Component for UDOcreateUdoContentions,createUdoContentions method, Processor.
/// Code Generated by IMS on: 5/29/2015 3:12:46 PM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace VRM.Integration.UDO.Claims.Processors
{
    class UDOcreateUdoContentionsProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOcreateUdoContentionsProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateUdoContentionsRequest request)
        {
            //var request = message as createUdoContentionsRequest;
            UDOcreateUdoContentionsResponse response = new UDOcreateUdoContentionsResponse();
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
                LogHelper.LogError(request.OrganizationName, request.UserId, string.Format("{0} Processor, Connection Error", method), connectException.Message);                
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                // prefix = fcontfindContentionsRequest();
                var findContentionsRequest = new VIMTfcontfindContentionsRequest();
                findContentionsRequest.LogTiming = request.LogTiming;
                findContentionsRequest.LogSoap = request.LogSoap;
                findContentionsRequest.Debug = request.Debug;
                findContentionsRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findContentionsRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findContentionsRequest.RelatedParentId = request.RelatedParentId;
                findContentionsRequest.UserId = request.UserId;
                findContentionsRequest.OrganizationName = request.OrganizationName;
                findContentionsRequest.LegacyServiceHeaderInfo = new VIMT.ContentionServiceRemote.Messages.HeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
                findContentionsRequest.mcs_claimid = request.claimId;

                // TODO(TN): Comment to remediate
                var findContentionsResponse = new VIMTfcontfindContentionsResponse();
                // var findContentionsResponse = findContentionsRequest.SendReceive<VIMTfcontfindContentionsResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call";
                var requestCollection = new OrganizationRequestCollection();
                response.ExceptionMessage = findContentionsResponse.ExceptionMessage;
                response.ExceptionOccured = findContentionsResponse.ExceptionOccured;
                if (findContentionsResponse.VIMTfcontBenefitClaimctnInfo.VIMTfcontcontentionsctnInfo != null)
                {
                    var contention = findContentionsResponse.VIMTfcontBenefitClaimctnInfo.VIMTfcontcontentionsctnInfo;
                    System.Collections.Generic.List<UDOcreateUdoContentionsMultipleResponse> UDOcreateUdoContentionsArray = new System.Collections.Generic.List<UDOcreateUdoContentionsMultipleResponse>();
                    foreach (var contentionItem in contention)
                    {
                        var responseIds = new UDOcreateUdoContentionsMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_contention";
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }

                        if (contentionItem.mcs_cntntnStatusTc != string.Empty)
                        {
                            //ok
                            thisNewEntity["udo_status"] = contentionItem.mcs_cntntnStatusTc;
                        }
                        if (contentionItem.mcs_dgnstcTc != string.Empty)
                        {
                            thisNewEntity["udo_diagnosticcode"] = contentionItem.mcs_dgnstcTc;
                        }
                        if (contentionItem.mcs_dgnstcTn != string.Empty)
                        {
                            thisNewEntity["udo_description"] = contentionItem.mcs_clmntTxt;
                        }
                        if (contentionItem.mcs_clsfcnTxt != string.Empty)
                        {
                            thisNewEntity["udo_contentionclassification"] = contentionItem.mcs_clsfcnTxt;
                        }
                        if (contentionItem.mcs_medInd != string.Empty)
                        {
                            thisNewEntity["udo_codesheetdiagnosis"] = contentionItem.mcs_medInd;
                        }
                        if (findContentionsResponse.VIMTfcontBenefitClaimctnInfo.mcs_claimRcvdDt != System.DateTime.MinValue)
                        {
                            thisNewEntity["udo_claimreceived"] = findContentionsResponse.VIMTfcontBenefitClaimctnInfo.mcs_claimRcvdDt;
                        }
                        if (request.UDOcreateUdoContentionsRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreateUdoContentionsRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }

                        CreateRequest createData = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createData);

                        //responseIds.newUDOcreateUdoContentionsId = OrgServiceProxy.Create(TruncateHelper.TruncateFields(thisNewEntity, request.OrganizationName, request.UserId, request.LogTiming));
                        //UDOcreateUdoContentionsArray.Add(responseIds);
                    }

                    if (requestCollection.Count() > 0)
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
                    string logInfo = string.Format("Contention Records Created: {0}", requestCollection.Count());
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Contention Records Created", logInfo);
                    #endregion
                }

                //added to generated code
                if (request.udo_claimId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_claimId;
                    parent.LogicalName = "udo_claim";
                    parent["udo_contentioncomplete"] = true;
                    //parent["udo_contentionmessage"] = "";
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to process claim data";
                response.ExceptionOccured = true;
                return response;
            }
        }
    }
}