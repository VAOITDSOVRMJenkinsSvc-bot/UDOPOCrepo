using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UDO.LOB.Awards.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Payments.Messages;
using VEIS.Messages.ClaimantService;
using VEIS.Messages.VeteranWebService;

namespace UDO.LOB.Awards.Processors
{
    class UDOcreateAwardsOrchProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOcreateAwardsOrchProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateAwardsRequest request)
        {
            //var request = message as createAwardsRequest;
            UDOcreateAwardsResponse response = new UDOcreateAwardsResponse { MessageId = request.MessageId };
            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = ConfigureDiagnosticsContext(request);
            }
            TraceLogger tLogger = new TraceLogger(method, request);
            _debug = request.Debug;
            LogBuffer = string.Empty;

            var progressString = "Top of Processor";

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            #region connect to CRM
            CrmServiceClient OrgServiceProxy;
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
                tLogger.LogEvent($"Authenticated and Connected to CRM", "2");
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            tLogger.LogEvent("Connected To CRM", "001");

            progressString = "After Connection";

            try
            {
                var blnContinueWithCalls = true;
                var requestCollection = new OrganizationRequestCollection();
                var awardsCreated = 0;
                var awardTypeCd = "";
                var ptcpntBeneId = "";
                var ptcpntRecipId = "";
                var ptcpntVetId = "";
                var awardId = Guid.Empty;
                var award00Id = Guid.Empty;
                var payeeCodeId = Guid.Empty;
                var payeeCode00Id = Guid.Empty;
                var updateVetSnap = false;
                var firstVetClaim = false;
                Entity vetSnapShot = new Entity();


                if (request.vetsnapshotId != Guid.Empty)
                {
                    vetSnapShot.LogicalName = "udo_veteransnapshot";
                    vetSnapShot.Id = request.vetsnapshotId;
                }
                if (!string.IsNullOrEmpty(request.fileNumber))
                {
                    var findGeneralInformationByFileNumberRequest = new VEISfgenFNfindGeneralInformationByFileNumberRequest();
                    findGeneralInformationByFileNumberRequest.MessageId = request.MessageId;
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
                        findGeneralInformationByFileNumberRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                    }

                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor", "About to call findGeneralInformationByFileNumberResponse, request.fileNumber:" + request.fileNumber);

                    // REM: Invoke VEIS Endpoint
                    var findGeneralInformationByFileNumberResponse = WebApiUtility.SendReceive<VEISfgenFNfindGeneralInformationByFileNumberResponse>(findGeneralInformationByFileNumberRequest, WebApiType.VEIS);
                    if (request.LogSoap || findGeneralInformationByFileNumberResponse.ExceptionOccurred)
                    {
                        if (findGeneralInformationByFileNumberResponse.SerializedSOAPRequest != null || findGeneralInformationByFileNumberResponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = findGeneralInformationByFileNumberResponse.SerializedSOAPRequest + findGeneralInformationByFileNumberResponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfgenFNfindGeneralInformationByFileNumberRequest Request/Response {requestResponse}", true);
                        }
                    }

                    LogHelper.LogDebug(request.OrganizationName,  request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor", "after findGeneralInformationByFileNumberResponse, request.fileNumber:" + request.fileNumber);
                    tLogger.LogEvent("Retrieved VEISfgenFNfindGeneralInformationByFileNumberResponse", "002");
                    progressString = "After VIMT EC Call";
                    LogHelper.LogDebug(request.OrganizationName,  request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor, Progess:" + progressString,"after general");

                    response.ExceptionMessage = findGeneralInformationByFileNumberResponse.ExceptionMessage;
                    response.ExceptionOccured = findGeneralInformationByFileNumberResponse.ExceptionOccurred;

                    response.findGeneralResponse = findGeneralInformationByFileNumberResponse;

                    #region looking at award response

                    List<OrganizationRequest> Requests = new List<OrganizationRequest>();
                    List<UDOcreateAwardsMultipleResponse> UDOcreateAwardsArray = new List<UDOcreateAwardsMultipleResponse>();

                    if (findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo != null)
                    {
                        if (findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.VEISfgenFNawardBenesInfo != null)
                        {
                            //LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor", "many awards to create");
                            #region many awards to create
                            // LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor, Progess:" + progressString, "Creating Multiple Awards");

                            var generalInfoRecord = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.VEISfgenFNawardBenesInfo;
                            //var serviceDTO = findVeteranResponse.VIMTfvetreturnInfo.VIMTfvetvetBirlsRecordInfo.VIMTfvetSERVICEInfo;

                            //var LatestServiceItem = generalInfoRecordUnSorted.OrderByDescending(h => h.mcs_stENTERED_ON_DUTY_DATE).FirstOrDefault();
                            foreach (var generalInfoSelectionItem in generalInfoRecord)
                            {

                                bool createNow = false;
                                //instantiate the new Entity
                                Entity thisNewEntity = new Entity("udo_award");

                                thisNewEntity["udo_name"] = "Award Summary";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }

                                if (!string.IsNullOrEmpty(generalInfoSelectionItem.mcs_ptcpntBeneId))
                                {
                                    thisNewEntity["udo_ptcpntbeneid"] = generalInfoSelectionItem.mcs_ptcpntBeneId;
                                }
                                if (!string.IsNullOrEmpty(generalInfoSelectionItem.mcs_ptcpntRecipId))
                                {
                                    thisNewEntity["udo_ptcpntrecipid"] = generalInfoSelectionItem.mcs_ptcpntRecipId;
                                }
                                if (!string.IsNullOrEmpty(generalInfoSelectionItem.mcs_recipName))
                                {
                                    thisNewEntity["udo_recipient"] = generalInfoSelectionItem.mcs_recipName;
                                }

                                // the 00 veteran claims is determined by payeecd = 00 and the recipId must == vet PID
                                if (!string.IsNullOrEmpty(generalInfoSelectionItem.mcs_awardBeneTypeName))
                                {
                                    thisNewEntity["udo_benefittypename"] = generalInfoSelectionItem.mcs_awardBeneTypeName;

                                    if (generalInfoSelectionItem.mcs_payeeCd == "00")
                                    {
                                        if (!string.IsNullOrEmpty(generalInfoSelectionItem.mcs_ptcpntRecipId))
                                        {
                                            if (generalInfoSelectionItem.mcs_ptcpntRecipId == request.ptcpntVetId)
                                            {
                                                //if we already  had an award for the veteran, we can't have 2 firsts....
                                                if (!firstVetClaim)
                                                {
                                                    if (request.vetsnapshotId != Guid.Empty)
                                                    {
                                                        if (generalInfoSelectionItem.mcs_awardTypeCd == "CPL")
                                                        {
                                                            vetSnapShot["udo_awardtype"] = generalInfoSelectionItem.mcs_awardTypeCd + "-" + generalInfoSelectionItem.mcs_awardBeneTypeName;
                                                            vetSnapShot["udo_awardtypecode"] = generalInfoSelectionItem.mcs_awardTypeCd;
                                                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor", "multi vetSnapShot[udo_awardtype]:" + vetSnapShot["udo_awardtype"]);
                                                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor", "multi  vetSnapShot[udo_awardtypecode]:" + vetSnapShot["udo_awardtypecode"]);

                                                            updateVetSnap = true;
                                                            firstVetClaim = true;

                                                            awardTypeCd = generalInfoSelectionItem.mcs_awardTypeCd;
                                                            ptcpntBeneId = generalInfoSelectionItem.mcs_ptcpntBeneId;
                                                            ptcpntRecipId = generalInfoSelectionItem.mcs_ptcpntRecipId;
                                                            ptcpntVetId = generalInfoSelectionItem.mcs_ptcpntVetId;
                                                            progressString = getFidInformation(OrgServiceProxy, request, response, thisNewEntity, progressString);
                                                        }
                                                        createNow = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(generalInfoSelectionItem.mcs_awardBeneTypeCd))
                                {
                                    thisNewEntity["udo_benefittypecode"] = generalInfoSelectionItem.mcs_awardBeneTypeCd;
                                }
                                if (!string.IsNullOrEmpty(generalInfoSelectionItem.mcs_awardBeneTypeCd))
                                {
                                    thisNewEntity["udo_benefitcode"] = generalInfoSelectionItem.mcs_awardBeneTypeCd;
                                }
                                if (!string.IsNullOrEmpty(generalInfoSelectionItem.mcs_beneName))
                                {
                                    thisNewEntity["udo_beneficiary"] = generalInfoSelectionItem.mcs_beneName;
                                }
                                if (!string.IsNullOrEmpty(generalInfoSelectionItem.mcs_awardTypeName))
                                {
                                    thisNewEntity["udo_awardtypename"] = generalInfoSelectionItem.mcs_awardTypeName;
                                }
                                if (!string.IsNullOrEmpty(generalInfoSelectionItem.mcs_awardTypeCd))
                                {
                                    thisNewEntity["udo_awardtypecode"] = generalInfoSelectionItem.mcs_awardTypeCd;
                                }
                                else
                                {
                                    blnContinueWithCalls = false;
                                }
                                if (!string.IsNullOrEmpty(generalInfoSelectionItem.mcs_awardBeneTypeCd))
                                {
                                    thisNewEntity["udo_awardcode"] = generalInfoSelectionItem.mcs_awardBeneTypeCd;
                                }

                                if (!string.IsNullOrEmpty(generalInfoSelectionItem.mcs_vetName))
                                {
                                    thisNewEntity["udo_vetname"] = generalInfoSelectionItem.mcs_vetName;
                                }
                                if (!string.IsNullOrEmpty(generalInfoSelectionItem.mcs_payeeCd))
                                {
                                    thisNewEntity["udo_payeetypecode"] = generalInfoSelectionItem.mcs_payeeCd;
                                }
                                //single fields for all awards

                                MapSingletonFields(request, blnContinueWithCalls, findGeneralInformationByFileNumberResponse, thisNewEntity);

                                if (request.UDOcreateAwardsRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreateAwardsRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                }

                                if (createNow)
                                {

                                    award00Id = OrgServiceProxy.Create(TruncateHelper.TruncateFields(request.MessageId, thisNewEntity, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));

                                    var award_rsp = new UDOcreateAwardsMultipleResponse
                                    {
                                        mcs_payeeCd = generalInfoSelectionItem.mcs_payeeCd,
                                        mcs_ptcpntBeneId = generalInfoSelectionItem.mcs_ptcpntBeneId,
                                        mcs_ptcpntRecipId = generalInfoSelectionItem.mcs_ptcpntRecipId,
                                        mcs_ptcpntVetId = ptcpntVetId,
                                        mcs_awardTypeCd = generalInfoSelectionItem.mcs_awardTypeCd,
                                        mcs_awardBeneTypeName = generalInfoSelectionItem.mcs_awardBeneTypeName,
                                        mcs_awardBeneTypeCd = generalInfoSelectionItem.mcs_awardBeneTypeCd,
                                        newUDOcreateAwardsId = award00Id
                                    };

                                    awardsCreated += 1;

                                    UDOcreateAwardsArray.Add(award_rsp);
                                }
                                else
                                    Requests.Add(new CreateRequest { Target = thisNewEntity });
                            }

                            #endregion
                        }
                        else
                        {

                            #region only 1 award to create
                            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntRecipID))
                            {
                                //LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor", "1 award to create");
                                //LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor", string.Format("Recip {0}", findGeneralInformationByFileNumberResponse.VIMTfgenFNreturnclmsInfo.mcs_ptcpntRecipID));


                                Entity thisNewEntity = new Entity();
                                thisNewEntity.LogicalName = "udo_award";
                                thisNewEntity["udo_name"] = "Award Summary";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }
                                //these fields are mapped differently for singleton
                                if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_awardTypeCode))
                                {
                                    thisNewEntity["udo_awardtypecode"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_awardTypeCode;
                                    request.awardTypeCd = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_awardTypeCode;
                                }
                                else
                                {
                                    blnContinueWithCalls = false;
                                }


                                if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_benefitTypeName))
                                {
                                    thisNewEntity["udo_benefittypename"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_benefitTypeName;

                                    if (findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeCode == "00")
                                    {
                                        if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntRecipID))
                                        {
                                            if (findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntRecipID == request.ptcpntVetId)
                                            {
                                                if (request.vetsnapshotId != Guid.Empty)
                                                {
                                                    if (findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_awardTypeCode == "CPL")
                                                    {
                                                        vetSnapShot["udo_awardtype"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_awardTypeCode + "-" + findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_benefitTypeName;
                                                        vetSnapShot["udo_awardtypecode"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_awardTypeCode;
                                                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor", "single vetSnapShot[udo_awardtype]:" + vetSnapShot["udo_awardtype"]);
                                                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor", "single vetSnapShot[udo_awardtypecode]:" + vetSnapShot["udo_awardtypecode"]);
                                                        updateVetSnap = true;
                                                        firstVetClaim = true;
                                                        awardTypeCd = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_awardTypeCode;
                                                        ptcpntBeneId = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntBeneID;
                                                        ptcpntRecipId = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntRecipID;
                                                        ptcpntVetId = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntVetID;
                                                        progressString = getFidInformation(OrgServiceProxy, request, response, thisNewEntity, progressString);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                                if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_benefitTypeCode))
                                {
                                    thisNewEntity["udo_benefittypecode"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_benefitTypeCode;
                                }
                                if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeCode))
                                {
                                    thisNewEntity["udo_payeetypecode"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeCode;
                                }

                                MapSingletonFields(request, blnContinueWithCalls, findGeneralInformationByFileNumberResponse, thisNewEntity);

                                if (request.UDOcreateAwardsRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreateAwardsRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                }

                                ///TODO (Original Comment): Update this to create record directly rather then using ExecuteMultiple request. 
                                Requests.Add(new CreateRequest { Target = thisNewEntity });


                            }
                            else
                            {
                                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "had a record, but we had no recipid");
                            }
                            #endregion
                        }

                        var emresponse = new ExecuteMultipleHelperResponse();
                        emresponse.IsFaulted = false;
                        emresponse = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, Requests, request.OrganizationName, Guid.Empty, request.Debug, true, 0, true, false);
                        tLogger.LogEvent("Execute Multiple Completed", "003");
                        if (emresponse.Responses.Count > 0)
                        {
                            var count = emresponse.Responses.Count;
                            awardsCreated += count;

                            for (var i = 0; i < count; i++)
                            {
                                var req = emresponse.Requests[i] as CreateRequest;
                                var rsp = emresponse.Responses[i] as CreateResponse;

                                if (req != null)
                                {
                                    var t = req.Target;
                                    var award_rsp = new UDOcreateAwardsMultipleResponse
                                    {
                                        mcs_payeeCd = t.GetAttributeValue<string>("udo_payeetypecode"),
                                        mcs_ptcpntBeneId = t.GetAttributeValue<string>("udo_ptcpntbeneid"),
                                        mcs_ptcpntRecipId = t.GetAttributeValue<string>("udo_ptcpntrecipid"),
                                        mcs_awardTypeCd = t.GetAttributeValue<string>("udo_awardtypename"),
                                        mcs_awardBeneTypeName = t.GetAttributeValue<string>("udo_benefittypename"),
                                        mcs_awardBeneTypeCd = t.GetAttributeValue<string>("udo_benefittypecode"),
                                        mcs_ptcpntVetId = ptcpntVetId
                                    };
                                    if (rsp != null) award_rsp.newUDOcreateAwardsId = rsp.id;

                                    UDOcreateAwardsArray.Add(award_rsp);
                                }
                            }

                            if (firstVetClaim)
                            {
                                if (award00Id == Guid.Empty)
                                {
                                    award00Id = ((CreateResponse)emresponse.Responses[0]).id;
                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor", "Got awardid00");
                                }

                            }

                            //*************************    
                        }
                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor, Progess:" + progressString, "Created this many awards:" + awardsCreated);
                        response.UDOcreateAwardsInfo = UDOcreateAwardsArray.ToArray();
                    }
                    #endregion


                }
                else
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor", "NO FILENUMBER");
                }

                if (request.udo_dependentId != Guid.Empty)
                {
                    var dep = new Entity();
                    dep.Id = request.udo_dependentId;
                    dep.LogicalName = "udo_dependant";
                    dep["udo_awardcomplete"] = true;
                    //OrgServiceProxy.Update(TruncateHelper.TruncateFields(dep, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                    OrgServiceProxy.Update(dep);
                }
                else
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor", "no dependentId found");
                }
                if (request.idProofId != Guid.Empty)
                {
                    var idProof = new Entity();
                    idProof.Id = request.idProofId;
                    idProof.LogicalName = "udo_idproof";
                    idProof["udo_awardintegration"] = new OptionSetValue(752280002);
                    //OrgServiceProxy.Update(TruncateHelper.TruncateFields(idProof, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                    OrgServiceProxy.Update(idProof);
                }
                else
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor", "no idProofId found");
                }

                if (award00Id != Guid.Empty)
                {
                    //if we have an CPL award for the 00 payee, go get the details
                    var retrieveAwardRequest = new UDOretrieveAwardRequest();
                    retrieveAwardRequest.LogTiming = request.LogTiming;
                    retrieveAwardRequest.LogSoap = request.LogSoap;
                    retrieveAwardRequest.Debug = request.Debug;
                    retrieveAwardRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                    retrieveAwardRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                    retrieveAwardRequest.RelatedParentId = request.RelatedParentId;
                    retrieveAwardRequest.UserId = request.UserId;
                    retrieveAwardRequest.OrganizationName = request.OrganizationName;
                    retrieveAwardRequest.ptcpntVetId = ptcpntVetId;
                    retrieveAwardRequest.ptcpntBeneId = ptcpntBeneId;
                    retrieveAwardRequest.ptcpntRecipId = ptcpntRecipId;
                    retrieveAwardRequest.fileNumber = request.fileNumber;
                    retrieveAwardRequest.awardTypeCd = awardTypeCd;
                    retrieveAwardRequest.vetSnapShotId = request.vetsnapshotId;
                    retrieveAwardRequest.AwardId = award00Id;
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor", "Going after award details for awardid00");

                    retrieveAwardRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
                    {
                        ///Header Info
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber,
                    };
                    // (TN): Already commented out in original code
                    //var retrieveResponse = retrieveAwardRequest.SendReceive<UDOretrieveAwardResponse>(MessageProcessType.Local);

                    var retrieveAwardLogic = new UDOretrieveAwardProcessor();
                    var retrieveResponse = retrieveAwardLogic.Execute(retrieveAwardRequest);
                    tLogger.LogEvent("UDOretrieveAwardProcessor Complete", "004");
                    progressString = "After VIMT EC Call";

                    #region do PaymentsStuff if I have a 00 award
                    try
                    {

                        var create00PaymentsRequest = new UDOcreatePaymentsRequest
                        {
                            RelatedParentEntityName = "contact",
                            RelatedParentFieldName = "udo_contactid",
                            RelatedParentId = request.udo_contactId,
                            Debug = request.Debug,
                            LogSoap = request.LogSoap,
                            LogTiming = request.LogTiming,
                            ownerId = request.ownerId,
                            ownerType = request.ownerType,
                            UserId = request.UserId,
                            OrganizationName = request.OrganizationName,
                            vetsnapshotId = request.vetsnapshotId,
                            ParticipantId = Int64.Parse(request.ptcpntVetId),
                            PayeeCode = "00",
                            IDProofOrchestration = true,
                            LegacyServiceHeaderInfo = new UDOHeaderInfo
                            {
                                ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                LoginName = request.LegacyServiceHeaderInfo.LoginName,

                                StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                            },
                            MessageId = request.MessageId,
                            DiagnosticsContext = ConfigureDiagnosticsContext(request)
                        };
                        
                        var paymentResponse = WebApiUtility.SendReceive<UDOcreatePaymentsResponse>(create00PaymentsRequest, WebApiType.LOB);
                        tLogger.LogEvent("Call UDOcreatePaymentsResponse Complete", "005");
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "Create Awards", "Done with Payments", request.Debug);

                    }
                    catch (Exception ex)
                    {
                        var method = String.Format("{0}:Payment", MethodInfo.GetThisMethod().ToString(true));
                        var message = String.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
                        if (ex.InnerException != null)
                        {
                            message += String.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
                        }
                        message += String.Format("Call Stack:\r\n{0}", ex.StackTrace);
                        LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId,
                                request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method, ex);
                    }
                    #endregion
                }
                else
                {
                    //this is normally updated in the retrieveawardrequest for payee00, but if we are here, then we have no payee 00 so we have to close awards
                    //and payments
                    if (request.vetsnapshotId != Guid.Empty)
                    {
                        vetSnapShot["udo_awardscompleted"] = new OptionSetValue(752280002);
                        vetSnapShot["udo_awardscomplete"] = true;
                        vetSnapShot["udo_paymentscompleted"] = new OptionSetValue(752280002);
                        updateVetSnap = true;
                    }
                }

                if (request.vetsnapshotId != Guid.Empty)
                {
                    if (updateVetSnap)
                    {
                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor", "Update Snapshot");
                        OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, vetSnapShot, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                        tLogger.LogEvent("Update Snapshot Complete", "005");
                    }
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId,
                    "UDOcreateAwardsOrchProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Award  Data";
                response.ExceptionOccured = true;
                if (request.idProofId != Guid.Empty)
                {
                    var idProof = new Entity();
                    idProof.Id = request.idProofId;
                    idProof.LogicalName = "udo_idproof";
                    //mark it as errored
                    idProof["udo_awardintegration"] = new OptionSetValue(752280003);
                    //OrgServiceProxy.Update(TruncateHelper.TruncateFields(idProof, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                    OrgServiceProxy.Update(idProof);
                }
                else
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsOrchProcessor Processor", "no idProofId found");
                }
                return response;
            }
            finally
            {
                if (OrgServiceProxy != null)
                {
                    OrgServiceProxy.Dispose();
                }
            }
        }

        private DiagnosticsContext ConfigureDiagnosticsContext(UDOcreateAwardsRequest request)
        {
            return new DiagnosticsContext()
            {
                AgentId = request.UserId,
                IdProof = request.idProofId,
                MessageTrigger = "UDOCreateAwardsOrchProcessor",
                OrganizationName = request.OrganizationName,
                VeteranId = request.udo_contactId,
                StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
            };
        }


        // REM: Removed static from Method
        private string getVeteranformation(UDOcreateAwardsRequest request, UDOcreateAwardsResponse response, string progressString, string pctpntIdsCreated, OrganizationRequestCollection requestCollection, Entity existingPeopleEntity)
        {
            #region - process corporate data
            var findCorporateRecordByFileNumberRequest = new VEIScrpFNfindCorporateRecordByFileNumberRequest();
            findCorporateRecordByFileNumberRequest.MessageId = request.MessageId;
            findCorporateRecordByFileNumberRequest.LogTiming = request.LogTiming;
            findCorporateRecordByFileNumberRequest.LogSoap = request.LogSoap;
            findCorporateRecordByFileNumberRequest.Debug = request.Debug;
            findCorporateRecordByFileNumberRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            findCorporateRecordByFileNumberRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            findCorporateRecordByFileNumberRequest.RelatedParentId = request.RelatedParentId;
            findCorporateRecordByFileNumberRequest.UserId = request.UserId;
            findCorporateRecordByFileNumberRequest.OrganizationName = request.OrganizationName;

            if (request.LegacyServiceHeaderInfo != null)
            {
                findCorporateRecordByFileNumberRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }
            
            //non standard fields
            findCorporateRecordByFileNumberRequest.mcs_filenumber = request.fileNumber;

            //REM: Note this method getVeteranformation has 0 references; 
            // var findCorporateRecordByFileNumberResponse = findCorporateRecordByFileNumberRequest.SendReceive<VIMTcrpFNfindCorporateRecordByFileNumberResponse>(MessageProcessType.Local);

            // REM: Invoke VEIS Endpoint
            var findCorporateRecordByFileNumberResponse = WebApiUtility.SendReceive<VEIScrpFNfindCorporateRecordByFileNumberResponse>(findCorporateRecordByFileNumberRequest, WebApiType.VEIS);
            if (request.LogSoap || findCorporateRecordByFileNumberResponse.ExceptionOccurred)
            {
                if (findCorporateRecordByFileNumberResponse.SerializedSOAPRequest != null || findCorporateRecordByFileNumberResponse.SerializedSOAPResponse != null)
                {
                    var requestResponse = findCorporateRecordByFileNumberResponse.SerializedSOAPRequest + findCorporateRecordByFileNumberResponse.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEIScrpFNfindCorporateRecordByFileNumberRequest Request/Response {requestResponse}", true);
                }
            }

            progressString = "After VIMT EC Call";
            response.ExceptionMessage = findCorporateRecordByFileNumberResponse.ExceptionMessage;
            response.ExceptionOccured = findCorporateRecordByFileNumberResponse.ExceptionOccurred;
            if (findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo != null)
            {

                var thisPID = request.ptcpntVetId;
                Entity peopleEntity = new Entity();

                if (existingPeopleEntity != null)
                {

                    peopleEntity = existingPeopleEntity;
                }
                if (request.ownerId != System.Guid.Empty)
                {
                    peopleEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                }
                if (request.vetsnapshotId != System.Guid.Empty)
                {
                    peopleEntity["udo_vetsnapshotid"] = request.vetsnapshotId.ToString();
                    peopleEntity["udo_veteransnapshotid"] = new EntityReference("udo_veteransnapshot", request.vetsnapshotId);
                } 

                var blnCreateRecord = false;
                if (pctpntIdsCreated.IndexOf(thisPID) == -1)
                {

                    blnCreateRecord = true;
                }
                else
                {
                    foreach (var item in requestCollection)
                    {
                        if (item.RequestName == "create")
                        {
                            var test = (CreateRequest)item;
                            peopleEntity = (Entity)test["Target"];
                            if (peopleEntity["udo_ptcpntid"].ToString() == request.ptcpntVetId)
                            {
                                break;
                            }
                        }
                    }
                }
                peopleEntity.LogicalName = "udo_person";
                if (request.ownerId != System.Guid.Empty)
                {
                    peopleEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                }

                peopleEntity["udo_filenumber"] = request.fileNumber;
                peopleEntity["udo_ptcpntid"] = request.ptcpntVetId;
                peopleEntity["udo_payeecode"] = "00";
                if (request.UDOcreateAwardsRelatedEntitiesInfo != null)
                {
                    foreach (var relatedItem in request.UDOcreateAwardsRelatedEntitiesInfo)
                    {
                        peopleEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                    }
                }
                var shrinq2Person = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo;
                if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_ssn))
                {
                    peopleEntity["udo_ssn"] = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_ssn;
                    peopleEntity["udo_vetssn"] = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_ssn;
                }
                if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_firstName))
                {
                    peopleEntity["udo_first"] = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_firstName;

                }
                if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_lastName))
                {
                    peopleEntity["udo_last"] = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_lastName;
                }
                peopleEntity["udo_name"] = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_firstName + " " + findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_lastName;
                if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_middleName))
                {
                    peopleEntity["udo_middle"] = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_middleName;
                }
                if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_dateOfBirth))
                {
                    peopleEntity["udo_dobstr"] = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_dateOfBirth;
                }
                if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_emailAddress))
                {
                    peopleEntity["udo_email"] = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_emailAddress;
                }
                var telephone1 = "";
                var telephone2 = "";
                if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_phoneNumberOne))
                {
                    telephone1 = "(" + findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_areaNumberOne + ") " + findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_phoneNumberOne;
                }
                if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_phoneNumberTwo))
                {
                    telephone2 = "(" + findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_areaNumberTwo + ") " + findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_phoneNumberTwo;
                }

                if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_phoneTypeNameOne))
                {
                    if (findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_phoneTypeNameOne.Equals("daytime", StringComparison.InvariantCultureIgnoreCase))
                    {
                        peopleEntity["udo_dayphone"] = telephone1;
                        peopleEntity["udo_eveningphone"] = telephone2;
                    }
                    else
                    {
                        peopleEntity["udo_dayphone"] = telephone2;
                        peopleEntity["udo_eveningphone"] = telephone1;
                    }
                }
                if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_addressLine1))
                {
                    peopleEntity["udo_address1"] = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_addressLine1;
                }
                if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_addressLine2))
                {
                    peopleEntity["udo_address2"] = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_addressLine2;
                }
                if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_addressLine3))
                {
                    peopleEntity["udo_address3"] = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_addressLine3;
                }
                if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_city))
                {
                    peopleEntity["udo_city"] = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_city;
                }
                if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_country))
                {
                    peopleEntity["udo_country"] = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_country;
                }
                if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_state))
                {
                    peopleEntity["udo_state"] = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_state;
                }
                if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_zipCode))
                {
                    peopleEntity["udo_zip"] = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_zipCode;
                }
                

                if (blnCreateRecord)
                {
                    pctpntIdsCreated += ";" + thisPID;
                    CreateRequest createPeopleData = new CreateRequest
                    {
                        Target = peopleEntity,
                        RequestName = "create"
                    };
                    requestCollection.Add(createPeopleData);
                }
            }

            #endregion
            return progressString;
        }

        private static bool MapSingletonFields(UDOcreateAwardsRequest request, bool blnContinueWithCalls, VEISfgenFNfindGeneralInformationByFileNumberResponse findGeneralInformationByFileNumberResponse, Entity thisNewEntity)
        {


            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetLastName))
            {
                thisNewEntity["udo_vetlastname"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetLastName;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetFirstName))
            {
                thisNewEntity["udo_vetfirstname"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetFirstName;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_statusReasonDate))
            {
                DateTime newDateTime;
                var newDate = dateStringFormat(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_statusReasonDate);
                if (DateTime.TryParse(newDate, out newDateTime))
                {
                    thisNewEntity["udo_statusreasondate"] = newDateTime;
                }
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeName))
            {
                thisNewEntity["udo_payeetypename"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeName;
            }

            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeName))
            {
                thisNewEntity["udo_payeename"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeName;
            }

            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_statusReasonTypeName))
            {
                thisNewEntity["udo_statusreasonname"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_statusReasonTypeName;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_statusReasonTypeCode))
            {
                thisNewEntity["udo_statusreasoncode"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_statusReasonTypeCode;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_stationOfJurisdiction))
            {
                thisNewEntity["udo_soj"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_stationOfJurisdiction;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntRecipID))
            {
                thisNewEntity["udo_ptcpntrecipid"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntRecipID;
                request.ptcpntRecipId = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntRecipID;
            }
            else
            {
                blnContinueWithCalls = false;
            }

            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntBeneID))
            {
                thisNewEntity["udo_ptcpntbeneid"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntBeneID;
                request.ptcpntBeneId = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntBeneID;
            }
            else
            {
                blnContinueWithCalls = false;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_powerOfAttorney))
            {
                thisNewEntity["udo_poa"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_powerOfAttorney;
            }

            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payStatusTypeName))
            {
                thisNewEntity["udo_paystatusname"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payStatusTypeName;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payStatusTypeCode))
            {
                thisNewEntity["udo_paystatuscode"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payStatusTypeCode;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_paymentAddressID))
            {
                thisNewEntity["udo_paymentaddressid"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_paymentAddressID;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeSSN))
            {
                thisNewEntity["udo_payeessn"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeSSN;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeSex))
            {
                thisNewEntity["udo_payeesex"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeSex;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeBirthDate))
            {
                //ssn 555700866 - DOB 01/01/1899 - can't be a date
                DateTime newDateTime;
                var newDate = dateStringFormat(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeBirthDate);
                if (DateTime.TryParse(newDate, out newDateTime))
                {
                    thisNewEntity["udo_payeedob"] = newDateTime;
                }
                thisNewEntity["udo_payeedobstr"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeBirthDate; ;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_paidThroughDate))
            {
                thisNewEntity["udo_paidthrough"] = dateStringFormat(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_paidThroughDate);
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_personalFundsOfPatientBalance))
            {
                thisNewEntity["udo_fundsofpatient"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_personalFundsOfPatientBalance;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_fundsDueIncompetentBalance))
            {
                thisNewEntity["udo_fundsincompetent"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_fundsDueIncompetentBalance;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_fiduciaryDecisionTypeCode))
            {
                thisNewEntity["udo_fiduciary"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_fiduciaryDecisionTypeCode;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_directDepositAccountID))
            {
                thisNewEntity["udo_directdepositid"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_directDepositAccountID;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_currentMonthlyRate))
            {
                thisNewEntity["udo_currmonthlyrate"] = moneyStringFormat(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_currentMonthlyRate);
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_competencyDecisionTypeCode))
            {
                thisNewEntity["udo_competency"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_competencyDecisionTypeCode + " - " + findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_competencyDecisionTypeName;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_clothingAllowanceTypeCode))
            {
                thisNewEntity["udo_clothingallowancecode"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_clothingAllowanceTypeCode;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_clothingAllowanceTypeName))
            {
                thisNewEntity["udo_clothingallowancename"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_clothingAllowanceTypeName;
            }
            return blnContinueWithCalls;
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
        // REM: removed static from method.
        private string getFidInformation(IOrganizationService OrgServiceProxy, UDOcreateAwardsRequest request, UDOcreateAwardsResponse response, Entity thisNewEntity, string progressString)
        {
            var findFiduciaryRequest = new VEISfidfindFiduciaryRequest();
            findFiduciaryRequest.MessageId = request.MessageId;
            findFiduciaryRequest.LogTiming = request.LogTiming;
            findFiduciaryRequest.LogSoap = request.LogSoap;
            findFiduciaryRequest.Debug = request.Debug;
            findFiduciaryRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            findFiduciaryRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            findFiduciaryRequest.RelatedParentId = request.RelatedParentId;
            findFiduciaryRequest.UserId = request.UserId;
            findFiduciaryRequest.OrganizationName = request.OrganizationName;
            findFiduciaryRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                           {
                               ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                               ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                               LoginName = request.LegacyServiceHeaderInfo.LoginName,

                               StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                           };
                        

            findFiduciaryRequest.mcs_filenumber = request.fileNumber;

            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "About to call findFiduciaryRequest, request.fileNumber:" + request.fileNumber);
            
            // var findFiduciaryResponse = findFiduciaryRequest.SendReceive<VIMTfidfindFiduciaryResponse>(MessageProcessType.Local);
            //REM: Invoke VEIS Endpoint
            //var findFiduciaryResponse = WebApiUtility.SendReceive<VEISfidfindFiduciaryResponse>(veisBaseUri, findFiduciaryRequest.MessageId, findFiduciaryRequest, logSettings);
            var findFiduciaryResponse = WebApiUtility.SendReceive<VEISfidfindFiduciaryResponse>(findFiduciaryRequest, WebApiType.VEIS);
            if (request.LogSoap || findFiduciaryResponse.ExceptionOccurred)
            {
                if (findFiduciaryResponse.SerializedSOAPRequest != null || findFiduciaryResponse.SerializedSOAPResponse != null)
                {
                    var requestResponse = findFiduciaryResponse.SerializedSOAPRequest + findFiduciaryResponse.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfidfindFiduciaryRequest Request/Response {requestResponse}", true);
                }
            }

            progressString = "After VIMTfidfindFiduciaryRequest EC Call";

            response.ExceptionMessage = findFiduciaryResponse.ExceptionMessage;
            response.ExceptionOccured = findFiduciaryResponse.ExceptionOccurred;
            if (findFiduciaryResponse.VEISfidreturnInfo != null)
            {
                var fidInfo = findFiduciaryResponse.VEISfidreturnInfo;

                response.fidExists = true;

                //if (!string.IsNullOrEmpty(fidInfo.mcs_personOrganizationName))
                //{
                //    //thisNewEntity["udo_name"] = fidInfo.mcs_personOrganizationName;
                //}
                if (!string.IsNullOrEmpty(fidInfo.mcs_relationshipName))
                {
                    thisNewEntity["udo_fiduciaryrelationshipname"] = fidInfo.mcs_relationshipName;
                }
                if (!string.IsNullOrEmpty(fidInfo.mcs_personOrgName))
                {
                    thisNewEntity["udo_fiduciaryname"] = fidInfo.mcs_personOrgName;
                }
                if (!string.IsNullOrEmpty(fidInfo.mcs_endDate))
                {
                    DateTime newDateTime;
                    var newDate = dateStringFormat(fidInfo.mcs_endDate);
                    if (DateTime.TryParse(newDate, out newDateTime))
                    {
                        thisNewEntity["udo_fiduciaryenddate"] = newDateTime;
                    }
                }
                if (!string.IsNullOrEmpty(fidInfo.mcs_beginDate))
                {
                    DateTime newDateTime;
                    var newDate = dateStringFormat(fidInfo.mcs_beginDate);
                    if (DateTime.TryParse(newDate, out newDateTime))
                    {
                        thisNewEntity["udo_fiduciarybegindate"] = newDateTime;
                    }
                }
            }
            return progressString;
        }

    }
}