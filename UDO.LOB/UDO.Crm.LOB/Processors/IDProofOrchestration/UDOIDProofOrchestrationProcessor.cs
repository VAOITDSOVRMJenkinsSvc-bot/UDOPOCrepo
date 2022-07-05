using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using VRM.Integration.UDO.IDProofOrchestration.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Awards.Messages;
using VRM.Integration.UDO.Payments.Messages;
using VRM.Integration.UDO.Claims.Messages;
using VRM.Integration.UDO.PeoplelistPayeeeCode.Messages;
using VRM.Integration.UDO.Common;
using VRM.Integration.UDO.Contact.Messages;
using VRM.Integration.UDO.Ratings.Messages;

namespace VRM.Integration.UDO.IDProofOrchestration.Processors
{
    public class UDOIDProofOrchestrationProcessor
    {
        public enum LoadStatus
        {
            NotStarted = 752280000,
            InProgress = 752280001,
            Completed = 752280002
        };

        OrganizationServiceProxy OrgServiceProxy;
        private bool _debug { get; set; }


        private const string method = "UDOIDProofOrchestrationProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOIDProofOrchestrationRequest request)
        {


            //var request = message as createAwardsRequest;
            UDOIDProofOrchestrationResponse response = new UDOIDProofOrchestrationResponse();
            _debug = request.Debug;
            LogBuffer = string.Empty;
            //LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOIDProofOrchestrationProcessor Processor", "Top");
            var progressString = "Top of Processor";

            if (request == null)
            {
                return response;
            }

            #region connect to CRM

            try
            {
                var commonFunctions = new CRMConnect();
                OrgServiceProxy = commonFunctions.ConnectToCrm(request.OrganizationName);
            }
            catch (Exception connectException)
            {
                var method = MethodInfo.GetThisMethod().ToString();
                LogHelper.LogError(
                    request.OrganizationName,
                    request.UserId,
                    method + ":CRM Connection Error",
                    connectException.Message);

                return response;
            }

            #endregion

            var udoOrchestrationMessageData = new UDOIDProofOrchestrationMessages();

            #region do Get Ratings

            try
            {
                var veteranReferance = new UDOgetRatingDataRelatedEntitiesMultipleRequest()
                {
                    RelatedEntityFieldName = "udo_veteranid",
                    RelatedEntityId = request.udo_contactId,
                    RelatedEntityName = "contact"
                };

                var ratingRequest = new UDOgetRatingDataRequest();
                var references = new[] { veteranReferance };

                ratingRequest.UDOgetRatingDataRelatedEntitiesInfo = references;
                ratingRequest.vetsnapshotId = request.vetsnapshotId;
                ratingRequest.MessageId = Guid.NewGuid().ToString();
                ratingRequest.Debug = request.Debug;
                ratingRequest.RelatedParentEntityName = "udo_idproof";
                ratingRequest.RelatedParentFieldName = "udo_idproofid";
                ratingRequest.RelatedParentId = request.idProofId;
                ratingRequest.LogSoap = request.LogSoap;
                ratingRequest.LogTiming = request.LogTiming;
                ratingRequest.UserId = request.UserId;
                ratingRequest.OrganizationName = request.OrganizationName;

                ratingRequest.fileNumber = request.fileNumber;

                ratingRequest.LegacyServiceHeaderInfo = new VRM.Integration.UDO.Messages.HeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };

                var ratingResponse = ratingRequest.SendReceive<UDOgetRatingDataResponse>(MessageProcessType.Local);
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Orchestration", "Done with Ratings");
            }
            catch (Exception ex)
            {
                var method = String.Format("{0}:Address", MethodInfo.GetThisMethod().ToString(true));
                var message = String.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
                if (ex.InnerException != null)
                {
                    message += String.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
                }
                message += String.Format("Call Stack:\r\n{0}", ex.StackTrace);
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId,
                        request.RelatedParentEntityName, request.RelatedParentFieldName, method, message);
            }
            #endregion

            #region do awards

            List<UDOcreateAwardsRelatedEntitiesMultipleRequest> newrelatEntites = new List<UDOcreateAwardsRelatedEntitiesMultipleRequest>();
            UDOcreateAwardsResponse CreateAwardsResponse = null;
            try
            {
                foreach (var item in request.UDOcreateAwardsRelatedEntitiesInfo)
                {
                    var thisRelatedData = new UDOcreateAwardsRelatedEntitiesMultipleRequest
                    {
                        RelatedEntityFieldName = item.RelatedEntityFieldName,
                        RelatedEntityId = item.RelatedEntityId,
                        RelatedEntityName = item.RelatedEntityName
                    };
                    if (item.RelatedEntityName == "contact" && request.udo_contactId == Guid.Empty)
                    {
                        request.udo_contactId = item.RelatedEntityId;
                    }
                    else if (item.RelatedEntityName == "udo_idproof" && request.idProofId == Guid.Empty)
                    {
                        request.idProofId = item.RelatedEntityId;
                    }
                    newrelatEntites.Add(thisRelatedData);
                }

                var awardsRequest = new UDOcreateAwardsSyncOrchRequest()
                {
                    LegacyServiceHeaderInfo = new VRM.Integration.UDO.Messages.HeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,

                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                    MessageId = request.MessageId,
                    fileNumber = request.fileNumber,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    Debug = request.Debug,
                    LogSoap = request.LogSoap,
                    LogTiming = request.LogTiming,
                    ptcpntVetId = request.ptcpntVetId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    ownerId = request.ownerId,
                    ownerType = request.ownerType,
                    vetsnapshotId = request.vetsnapshotId,
                    idProofId = request.idProofId,
                    udo_ssn = request.udo_ssidstring,
                    UDOcreateAwardsRelatedEntitiesInfo = newrelatEntites.ToArray()
                };
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOIDProofOrchestrationProcessor Processor", "Sending Award Orch");
                
                CreateAwardsResponse = awardsRequest.SendReceive<UDOcreateAwardsResponse>(MessageProcessType.Local);
                if (CreateAwardsResponse.ExceptionOccured)
                {
                    //this is bad, need to do something and likely return here and get out
                }
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Orchestration", "Done with Awards");
                udoOrchestrationMessageData.findGeneralResponse = CreateAwardsResponse.findGeneralResponse;
            }
            catch (Exception ex)
            {
                var method = String.Format("{0}:Awards", MethodInfo.GetThisMethod().ToString(true));
                var message = String.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
                if (ex.InnerException != null)
                {
                    message += String.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
                }
                message += String.Format("Call Stack:\r\n{0}", ex.StackTrace);
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId,
                        request.RelatedParentEntityName, request.RelatedParentFieldName, method, message);
            }
            #endregion

            #region do AddressStuff
            try
            {
                //RC NEW - moved this to here instead of snapshot, so it can be used by createpayee
                var addressRequest = new UDOcreateAddressRecordsRequest();

                if (!String.IsNullOrEmpty(request.ptcpntVetId))
                {
                    addressRequest.ptcpntId = Int64.Parse(request.ptcpntVetId);
                    addressRequest.LegacyServiceHeaderInfo = new VRM.Integration.UDO.Messages.HeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,

                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                    addressRequest.MessageId = Guid.NewGuid().ToString();
                    addressRequest.Debug = request.Debug;
                    addressRequest.LogSoap = request.LogSoap;
                    addressRequest.LogTiming = request.LogTiming;
                    addressRequest.UserId = request.UserId;
                    addressRequest.OrganizationName = request.OrganizationName;
                    addressRequest.ownerId = request.ownerId;
                    addressRequest.ownerType = request.ownerType;
                    addressRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                    addressRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                    addressRequest.RelatedParentId = request.RelatedParentId;
                    addressRequest.vetsnapshotId = request.vetsnapshotId;

                    var response2 = addressRequest.SendReceive<UDOcreateAddressRecordsResponse>(MessageProcessType.Local);
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Orchestration", "Done with Address");
                }
            }
            catch (Exception ex)
            {
                var method = String.Format("{0}:Address", MethodInfo.GetThisMethod().ToString(true));
                var message = String.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
                if (ex.InnerException != null)
                {
                    message += String.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
                }
                message += String.Format("Call Stack:\r\n{0}", ex.StackTrace);
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId,
                        request.RelatedParentEntityName, request.RelatedParentFieldName, method, message);
            }
        
            #endregion
   
            #region get Claims

            if (string.IsNullOrEmpty(request.fileNumber))
            {
                if (!string.IsNullOrEmpty(request.udo_ssidstring))
                {
                    request.fileNumber = request.udo_ssidstring;
                }
            }
           if (!string.IsNullOrEmpty(request.fileNumber))
           {

               List<UDOcreateUDOClaimsRelatedEntitiesMultipleResponse> newrelatClaimEntites = new List<UDOcreateUDOClaimsRelatedEntitiesMultipleResponse>();
               try
               {
                   foreach (var item in request.UDOcreateClaimsRelatedEntitiesInfo)
                   {
                       var thisRelatedData = new UDOcreateUDOClaimsRelatedEntitiesMultipleResponse
                       {
                           RelatedEntityFieldName = item.RelatedEntityFieldName,
                           RelatedEntityId = item.RelatedEntityId,
                           RelatedEntityName = item.RelatedEntityName
                       };
                       if (item.RelatedEntityName == "contact" && request.udo_contactId == Guid.Empty)
                       {
                           request.udo_contactId = item.RelatedEntityId;
                       }
                       else if (item.RelatedEntityName == "udo_idproof" && request.idProofId == Guid.Empty)
                       {
                           request.idProofId = item.RelatedEntityId;
                       }
                       newrelatClaimEntites.Add(thisRelatedData);
                   }
                   var claimsRequest = new UDOcreateUDOClaimsSyncOrchRequest()
                   {
                       LegacyServiceHeaderInfo = new VRM.Integration.UDO.Messages.HeaderInfo
                       {
                           ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                           ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                           LoginName = request.LegacyServiceHeaderInfo.LoginName,

                           StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                       },
                       MessageId = request.MessageId,
                       fileNumber = request.fileNumber,
                       RelatedParentEntityName = request.RelatedParentEntityName,
                       RelatedParentFieldName = request.RelatedParentFieldName,
                       RelatedParentId = request.RelatedParentId,
                       Debug = request.Debug,
                       LogSoap = request.LogSoap,
                       LogTiming = request.LogTiming,
                       UserId = request.UserId,
                       OrganizationName = request.OrganizationName,
                       ownerId = request.ownerId,
                       ownerType = request.ownerType,
                       vetsnapshotId = request.vetsnapshotId,
                       idProofId = request.idProofId,
                       UDOcreateUDOClaimsRelatedEntitiesInfo = newrelatClaimEntites.ToArray()
                   };
                   LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOIDProofOrchestrationProcessor Processor", "Sending Claim Orch");

                   var CreateClaimsResponse = claimsRequest.SendReceive<UDOcreateUDOClaimsResponse>(MessageProcessType.Local);
                   LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Orchestration", "Done with Claims");
                   if (CreateClaimsResponse.ExceptionOccured)
                   {
                       //this is bad, need to do something and likely return here and get out
                   }

                   udoOrchestrationMessageData.findBenefitClaimResponse = CreateClaimsResponse.VIMTfindBenefitClaimRequestData;
               }
               catch (Exception ex)
               {
                   var method = String.Format("{0}:Claims", MethodInfo.GetThisMethod().ToString(true));
                   var message = String.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
                   if (ex.InnerException != null)
                   {
                       message += String.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
                   }
                   message += String.Format("Call Stack:\r\n{0}", ex.StackTrace);
                   LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId,
                           request.RelatedParentEntityName, request.RelatedParentFieldName, method, message);
               }
           }
            #endregion

            #region PeopleListPayeecode
            
            try {
                var peopleRelatedEntityInfo = new List<PeopleRelatedEntitiesMultipleRequest>
                {
                    new PeopleRelatedEntitiesMultipleRequest {
                        RelatedEntityFieldName = "udo_veteranid",
                        RelatedEntityId = request.udo_contactId,
                        RelatedEntityName = "contact"
                    },
                    new PeopleRelatedEntitiesMultipleRequest { 
                        RelatedEntityFieldName = "udo_idproofid",
                        RelatedEntityId = request.idProofId,
                        RelatedEntityName = "udo_idproof"
                    }
                };

                var payeeRelatedEntityInfo = new List<PayeeCodeRelatedEntitiesMultipleRequest>
                {
                    new PayeeCodeRelatedEntitiesMultipleRequest {
                        RelatedEntityFieldName = "udo_veteranid",
                        RelatedEntityId = request.udo_contactId,
                        RelatedEntityName = "contact"
                    },
                    new PayeeCodeRelatedEntitiesMultipleRequest { 
                        RelatedEntityFieldName = "udo_idproofid",
                        RelatedEntityId = request.idProofId,
                        RelatedEntityName = "udo_idproof"
                    }
                };

                var awardEntities = new List<FromUDOcreateAwardsMultipleResponse>();
                if (CreateAwardsResponse.UDOcreateAwardsInfo != null)
                {
                    foreach (var item in CreateAwardsResponse.UDOcreateAwardsInfo)
                    {
                        var thisRelatedData = new FromUDOcreateAwardsMultipleResponse
                        {
                            mcs_payeeCd = item.mcs_payeeCd,
                            mcs_ptcpntBeneId = item.mcs_ptcpntBeneId,
                            mcs_ptcpntRecipId = item.mcs_ptcpntRecipId,
                            mcs_ptcpntVetId = item.mcs_ptcpntVetId,
                            mcs_awardTypeCd = item.mcs_awardTypeCd,
                            mcs_awardBeneTypeName = item.mcs_awardBeneTypeName,
                            mcs_awardBeneTypeCd = item.mcs_awardBeneTypeCd,
                            newUDOcreateAwardsId = item.newUDOcreateAwardsId

                        };
                        awardEntities.Add(thisRelatedData);
                    }
                }

                var peopleRequest = new UDOCreatePeoplePayeeRequest()
                {
                    LegacyServiceHeaderInfo = new VRM.Integration.UDO.Messages.HeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,

                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                    MessageId = request.MessageId,
                    fileNumber = request.fileNumber,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    Debug = request.Debug,
                    LogSoap = request.LogSoap,
                    LogTiming = request.LogTiming,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    ownerId = request.ownerId,
                    ownerType = request.ownerType,
                    vetsnapshotId = request.vetsnapshotId,
                    idProofId = request.idProofId,
                    UDOcreatePeopleRelatedEntitiesInfo = peopleRelatedEntityInfo.ToArray(),
                    UDOcreatePayeeRelatedEntitiesInfo = payeeRelatedEntityInfo.ToArray(),
                    UDOcreateAwardsInfo = awardEntities.ToArray(),
                    findBenefitClaimResponse = udoOrchestrationMessageData.findBenefitClaimResponse,
                    findGeneralResponse = udoOrchestrationMessageData.findGeneralResponse
                };
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOIDProofOrchestrationProcessor Processor", "Sending People Orch");

                
                var CreatePeoplePayeeResponse = peopleRequest.SendReceive<UDOCreatePeoplePayeeResponse>(MessageProcessType.Local);
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Orchestration", "Done with People");
                if (CreatePeoplePayeeResponse.ExceptionOccured)
                {
                    //this is bad, need to do something and likely return here and get out
                }
            }
            catch (Exception ex)
            {
                var method = String.Format("{0}:PeopleList", MethodInfo.GetThisMethod().ToString(true));
                var message = String.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
                if (ex.InnerException != null)
                {
                    message += String.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
                }
                message += String.Format("Call Stack:\r\n{0}", ex.StackTrace);
                LogHelper.LogError(request.OrganizationName, request.UserId, method, message);
            }
            #endregion

            if (request.idProofId != Guid.Empty)
            {
                try
                {
                    var idProofUpdate = new Entity("udo_idproof");
                    idProofUpdate.Id = request.idProofId;
                    idProofUpdate["udo_orchcomplete"] = new OptionSetValue((int)LoadStatus.Completed);
                    OrgServiceProxy.Update(idProofUpdate);
                }
                catch (Exception ex)
                {
                    var method = String.Format("{0}:IdProof Update", MethodInfo.GetThisMethod().ToString(true));
                    var message = String.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
                    if (ex.InnerException != null)
                    {
                        message += String.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
                    }
                    message += String.Format("Call Stack:\r\n{0}", ex.StackTrace);
                    LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId,
                        request.RelatedParentEntityName, request.RelatedParentFieldName, method, message);
                }
            }
            if (request.vetsnapshotId != Guid.Empty)
            {
                try
                {
                    var vetSnapUpdate = new Entity("udo_veteransnapshot");
                    vetSnapUpdate.Id = request.vetsnapshotId;
                    vetSnapUpdate["udo_orchcomplete"] = new OptionSetValue((int)LoadStatus.Completed);
                    OrgServiceProxy.Update(vetSnapUpdate);
                }
                catch (Exception ex)
                {
                    var method = String.Format("{0}:vetsnapshotId Update", MethodInfo.GetThisMethod().ToString(true));
                    var message = String.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
                    if (ex.InnerException != null)
                    {
                        message += String.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
                    }
                    message += String.Format("Call Stack:\r\n{0}", ex.StackTrace);
                    LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId,
                        request.RelatedParentEntityName, request.RelatedParentFieldName, method, message);
                }
            }

            return response;
        }
    }

}