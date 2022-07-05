using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using UDO.LOB.Awards.Messages;
using UDO.LOB.Claims.Messages;
using UDO.LOB.Contact.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.IDProofOrchestration.Messages;
using UDO.LOB.PeoplelistPayeeCode.Messages;
using UDO.LOB.Ratings.Messages;

namespace UDO.LOB.IDProofOrchestration.Processors
{
    public class UDOIDProofOrchestrationProcessor
    {
        public enum LoadStatus
        {
            NotStarted = 752280000,
            InProgress = 752280001,
            Completed = 752280002
        };

        private bool _debug { get; set; }
        private string LogBuffer { get; set; }
        private const string method = "UDOIDProofOrchestrationProcessor";
        private static readonly string TimestampFormat = "yyyy-MM-dd hh:mm:ss:fff";
        public IMessageBase Execute(UDOIDProofOrchestrationRequest request)
        {
            UDOIDProofOrchestrationResponse response = new UDOIDProofOrchestrationResponse();
            CrmServiceClient OrgServiceProxy = null;
            try
            {
                if (request.DiagnosticsContext == null && request != null)
                {
                    request.DiagnosticsContext = SetDiagnosticsContext(request);
                }
                TraceLogger tLogger = new TraceLogger(method, request);
                response.MessageId = request?.MessageId;
                if (request == null)
                {

                    response.MessageId = Guid.Empty.ToString();
                    return response;
                }

                _debug = request.Debug;
                LogBuffer = string.Empty;

                LogHelper.LogDebug(request.OrganizationName, request.UserId, request.Debug, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOIDProofOrchestrationProcessor Processor", "Top", false);

                #region connect to CRM
                try
                {
                    OrgServiceProxy = ConnectionCache.GetProxy();
                    tLogger.LogEvent($"Authenticated and Connected to CRM", "2");
                }
                catch (Exception connectException)
                {
                    LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                    //response.ExceptionMessage = "Failed to get CRMConnection";
                    //response.ExceptionOccured = true;
                    return response;
                }

                #endregion
                tLogger.LogEvent("Connected to CRM", "001");
                var udoOrchestrationMessageData = new UDOIDProofOrchestrationMessages();


                #region do Get Ratings

                DoRatings(request);

                #endregion
                tLogger.LogEvent("Completed Ratings Call", "002");
                #region do awards

                // TODO: UnComment 
                UDOcreateAwardsResponse CreateAwardsResponse = DoAwards(request, udoOrchestrationMessageData);

                #endregion
                tLogger.LogEvent("Completed Awards", "003");
                #region do AddressStuff

                DoAddress(request);

                #endregion
                tLogger.LogEvent("Completed Address", "004");
                #region get Claims

                DoClaims(request, udoOrchestrationMessageData);

                #endregion
                tLogger.LogEvent("Completed Claims", "005");
                #region PeopleListPayeecode

                // TODO: UnComment 
                DoPeopleList(request, udoOrchestrationMessageData, CreateAwardsResponse);

                #endregion
                tLogger.LogEvent("Completed Payee Code", "006");
                #region Update IdProof

                if (request.idProofId != Guid.Empty)
                {
                    UpdateIdProof(request, OrgServiceProxy);
                }

                #endregion
                tLogger.LogEvent("Update Id Proof", "007");
                #region Update VeteranSnapShot

                if (request.vetsnapshotId != Guid.Empty)
                {
                    UpdateVeteranSnapShot(request, OrgServiceProxy);
                }

                #endregion
                tLogger.LogEvent("Update Veteran Snapshot", "008");
            }
            catch (Exception ex)
            {
                LogHelper.LogInfo($"[LOGERROR] ERROR in UDOIDProofOrchestrationProcessor - Execute :: {WebApiUtility.StackTraceToString(ex)}");
            }
            finally
            {
                if (OrgServiceProxy != null)
                {
                    OrgServiceProxy.Dispose();
                }
            }
            return response;
        }

        private static DiagnosticsContext SetDiagnosticsContext(UDOIDProofOrchestrationRequest request)
        {          
            return new DiagnosticsContext()
            {
                AgentId = request.UserId,   
                IdProof = request.RelatedParentId,
                MessageTrigger = method,
                VeteranId = request.udo_contactId,
                OrganizationName = request.OrganizationName,
                StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
            };
        }

        private void UpdateVeteranSnapShot(UDOIDProofOrchestrationRequest request, CrmServiceClient OrgServiceProxy)
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
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method, ex);
            }
            finally
            {
                if (OrgServiceProxy != null)
                {
                    OrgServiceProxy.Dispose();
                }
            }
        }

        private void UpdateIdProof(UDOIDProofOrchestrationRequest request, CrmServiceClient OrgServiceProxy)
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
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId,
                    request.RelatedParentEntityName, request.RelatedParentFieldName, method, ex);
            }
        }

        private static void DoPeopleList(UDOIDProofOrchestrationRequest request, UDOIDProofOrchestrationMessages udoOrchestrationMessageData, UDOcreateAwardsResponse CreateAwardsResponse)
        {
            try
            {
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
                    LegacyServiceHeaderInfo = new UDOHeaderInfo
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
                    findGeneralResponse = udoOrchestrationMessageData.findGeneralResponse,
                    DiagnosticsContext = SetDiagnosticsContext(request)
                };
                LogHelper.LogDebug(request.OrganizationName, request.UserId, request.Debug, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, 
                    method, "Sending People Orch", false);

                // Replaced: peopleRequest.SendReceive<UDOCreatePeoplePayeeResponse>(MessageProcessType.Local);

                var CreatePeoplePayeeResponse = WebApiUtility.SendReceive<UDOCreatePeoplePayeeResponse>(peopleRequest, WebApiType.LOB);


                //if (request.LogSoap || response.ExceptionOccurred)
                //{
                //    if (CreatePeoplePayeeResponse.SerializedSOAPRequest != null || CreatePeoplePayeeResponse.SerializedSOAPResponse != null)
                //    {
                //        var requestResponse = CreatePeoplePayeeResponse.SerializedSOAPRequest + CreatePeoplePayeeResponse.SerializedSOAPResponse;
                //        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"UDOCreatePeoplePayeeRequest Request/Response {requestResponse}", true);
                //    }
                //}

                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Done with People.UDOCreatePeoplePayeeRequest", request.Debug);
                if (CreatePeoplePayeeResponse.ExceptionOccurred)
                {
                    //this is bad, need to do something and likely return here and get out
                    LogHelper.LogError(request.OrganizationName, request.Debug, request.UserId, method, $"Error in PeoplePayeeCode.LOB.UDOCreatePeoplePayeeResponse: {CreatePeoplePayeeResponse.ExceptionMessage}");
                }
            }
            catch (Exception ex)
            {
                // var method = String.Format("{0}:PeopleList", MethodInfo.GetThisMethod().ToString(true));
                LogHelper.LogError(request.OrganizationName, request.UserId, method, ex);
            }
        }

        private static void DoClaims(UDOIDProofOrchestrationRequest request, UDOIDProofOrchestrationMessages udoOrchestrationMessageData)
        {
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
                        LegacyServiceHeaderInfo = new UDOHeaderInfo
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
                        UDOcreateUDOClaimsRelatedEntitiesInfo = newrelatClaimEntites.ToArray(),
                        DiagnosticsContext = SetDiagnosticsContext(request)
                    };

                    LogHelper.LogDebug(request.OrganizationName, request.UserId, request.Debug, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOIDProofOrchestrationProcessor Processor", "Sending Claim Orch", false);

                    // REM: Invoke UDO Claims Endpoint
                    // Replaced: claimsRequest.SendReceive<UDOcreateUDOClaimsResponse>(MessageProcessType.Local)
                    var CreateClaimsResponse = WebApiUtility.SendReceive<UDOcreateUDOClaimsResponse>(claimsRequest, WebApiType.LOB);

                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, "Done with Claims.UDOcreateUDOClaimsSyncOrchRequest");
                    if (CreateClaimsResponse.ExceptionOccured)
                    {
                        //this is bad, need to do something and likely return here and get out
                        LogHelper.LogError(request.OrganizationName, request.Debug, request.UserId, method, $"Error in Claims.LOB.UDOcreateUDOClaimsSyncOrchResponse: {CreateClaimsResponse.ExceptionMessage}");
                    }

                    udoOrchestrationMessageData.findBenefitClaimResponse = CreateClaimsResponse.VEISfindBenefitClaimRequestData;
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
                    LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId,
                            request.RelatedParentEntityName, request.RelatedParentFieldName, method, ex);
                }
            }
        }

        private static void DoAddress(UDOIDProofOrchestrationRequest request)
        {
            try
            {
                //RC NEW - moved this to here instead of snapshot, so it can be used by createpayee
                var addressRequest = new UDOcreateAddressRecordsRequest();

                if (!String.IsNullOrEmpty(request.ptcpntVetId))
                {
                    addressRequest.ptcpntId = Int64.Parse(request.ptcpntVetId);
                    addressRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
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
                    addressRequest.DiagnosticsContext = SetDiagnosticsContext(request);

                    // var response2 = addressRequest.SendReceive<UDOcreateAddressRecordsResponse>(MessageProcessType.Local);
                    var response2 = WebApiUtility.SendReceive<UDOcreateAddressRecordsResponse>(addressRequest, WebApiType.LOB);
                    LogHelper.LogDebug(request.MessageId,request.OrganizationName, request.UserId, method, $"Done with Address.UDOcreateAddressRecordsRequest. UDOcreateAddressRecordsInfo: {response2?.UDOcreateAddressRecordsInfo?.Length}", request.Debug);
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
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId,
                        request.RelatedParentEntityName, request.RelatedParentFieldName, method, ex);
            }
        }

        private static UDOcreateAwardsResponse DoAwards(UDOIDProofOrchestrationRequest request, UDOIDProofOrchestrationMessages udoOrchestrationMessageData)
        {
            List<UDOcreateAwardsRelatedEntitiesMultipleRequest> newrelatEntites = new List<UDOcreateAwardsRelatedEntitiesMultipleRequest>();
            UDOcreateAwardsResponse CreateAwardsResponse = null;
            try
            {
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Entered DoAwards", request.Debug);
                if (request.UDOcreateClaimsRelatedEntitiesInfo != null)
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
                }


                var awardsRequest = new UDOcreateAwardsSyncOrchRequest()
                {
                    LegacyServiceHeaderInfo = new UDOHeaderInfo
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
                    UDOcreateAwardsRelatedEntitiesInfo = newrelatEntites.ToArray(),
                    DiagnosticsContext = SetDiagnosticsContext(request)
                };
                LogHelper.LogDebug(request.OrganizationName, request.UserId, request.Debug, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOIDProofOrchestrationProcessor Processor", "Sending Award Orch", false);

                // REM: Invoke UDO Awards Endpoint
                // CreateAwardsResponse = awardsRequest.SendReceive<UDOcreateAwardsResponse>(MessageProcessType.Local);
                CreateAwardsResponse = WebApiUtility.SendReceive<UDOcreateAwardsResponse>(awardsRequest, WebApiType.LOB);

                if (CreateAwardsResponse.ExceptionOccured)
                {
                    //this is bad, need to do something and likely return here and get out
                    LogHelper.LogError(request.OrganizationName, request.Debug, request.UserId, method, $"Error in Awards.LOB.CreateAwardsResponse: {CreateAwardsResponse.ExceptionMessage}");
                }
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Done with Awards", request.Debug);
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
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId,
                        request.RelatedParentEntityName, request.RelatedParentFieldName, method, ex);
            }

            return CreateAwardsResponse;
        }

        private static void DoRatings(UDOIDProofOrchestrationRequest request)
        {
            try
            {
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Entered DoRatings", request.Debug);
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
                ratingRequest.MessageId = request.MessageId;
                ratingRequest.Debug = request.Debug;
                ratingRequest.RelatedParentEntityName = "udo_idproof";
                ratingRequest.RelatedParentFieldName = "udo_idproofid";
                ratingRequest.RelatedParentId = request.idProofId;
                ratingRequest.LogSoap = request.LogSoap;
                ratingRequest.LogTiming = request.LogTiming;
                ratingRequest.UserId = request.UserId;
                ratingRequest.OrganizationName = request.OrganizationName;

                ratingRequest.fileNumber = request.fileNumber;

                ratingRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
                ratingRequest.DiagnosticsContext = SetDiagnosticsContext(request);

                // var ratingResponse = ratingRequest.SendReceive<UDOgetRatingDataResponse>(MessageProcessType.Local);
                var ratingResponse = WebApiUtility.SendReceive<UDOgetRatingDataResponse>(ratingRequest, WebApiType.LOB);
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, $"Done with Ratings: SC-Combined: {ratingResponse.sccombined}, NSC-Combined: {ratingResponse.nsccombined}", request.Debug);
                                
            }
            catch (Exception ex)
            {
                var method = String.Format("{0}:DoRatings", MethodInfo.GetThisMethod().ToString(true));
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId,
                        request.RelatedParentEntityName, request.RelatedParentFieldName,  method, ex);
            }
        }

    }
}