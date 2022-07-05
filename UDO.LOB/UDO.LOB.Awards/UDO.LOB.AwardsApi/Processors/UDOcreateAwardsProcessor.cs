using global::UDO.LOB.Core;
using global::UDO.LOB.Extensions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using UDO.LOB.Awards.Messages;
using UDO.LOB.Extensions.Logging;
using VEIS.Messages.ClaimantService;
using VEIS.Messages.VeteranWebService;

namespace UDO.LOB.Awards.Processors
{
    internal class UDOcreateAwardsProcessor
    {
        private bool _debug { get; set; }

        private const string method = "UDOcreateAwardsProcessor";

        private string LogBuffer { get; set; }

        public WhoAmIResponse WhoAmI()
        {
            using (CrmServiceClient organizationService = ConnectionCache.GetProxy())
            {
                WhoAmIResponse whoAmIResponse = null;
                whoAmIResponse = organizationService.Execute(new WhoAmIRequest()) as WhoAmIResponse;
                return whoAmIResponse;
            }
        }
        public IMessageBase Execute(UDOcreateAwardsRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.Execute"); //{JsonHelper.Serialize<UDOcreateAwardsRequest>(request)}");
            UDOcreateAwardsResponse response = new UDOcreateAwardsResponse { MessageId = request.MessageId };
            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }
            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = "UDOcreateAwardsProcessor",
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }
            TraceLogger tLogger = new TraceLogger(method, request);

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDOcreateAwardsRequest>(request)}");

            _debug = request.Debug;
            LogBuffer = string.Empty;

            string progressString = "Top of Processor";

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

            progressString = "After Connection";

            try
            {
                bool blnContinueWithCalls = true;
                string pctpntIdsCreated = "";
                string payeecodesCreated = "";
                OrganizationRequestCollection requestCollection = new OrganizationRequestCollection();
                int awardsCreated = 0;
                string awardTypeCd = "";
                string ptcpntBeneId = "";
                string ptcpntRecipId = "";
                string ptcpntVetId = "";
                Guid awardId = Guid.Empty;
                Guid award00Id = Guid.Empty;
                Guid payeeCodeId = Guid.Empty;
                Guid payeeCode00Id = Guid.Empty;
                bool updateVetSnap = false;
                Entity vetSnapShot = new Entity();


                if (request.vetsnapshotId != Guid.Empty)
                {
                    vetSnapShot.LogicalName = "udo_veteransnapshot";
                    vetSnapShot.Id = request.vetsnapshotId;
                }
                if (!string.IsNullOrEmpty(request.fileNumber))
                {
                    VEISfgenFNfindGeneralInformationByFileNumberRequest findGeneralInformationByFileNumberRequest = new VEISfgenFNfindGeneralInformationByFileNumberRequest();
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

                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "About to call findGeneralInformationByFileNumberResponse, request.fileNumber:" + request.fileNumber);
                    
                    //REM: Invoke VEIS WebApi
                    progressString = "Before VEISfgenFNfindGeneralInformationByFileNumber EC Call";
                    var findGeneralInformationByFileNumberResponse = WebApiUtility.SendReceive<VEISfgenFNfindGeneralInformationByFileNumberResponse>(findGeneralInformationByFileNumberRequest, WebApiType.VEIS);
                    if (request.LogSoap || findGeneralInformationByFileNumberResponse.ExceptionOccurred)
                    {
                        if (findGeneralInformationByFileNumberResponse.SerializedSOAPRequest != null || findGeneralInformationByFileNumberResponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = findGeneralInformationByFileNumberResponse.SerializedSOAPRequest + findGeneralInformationByFileNumberResponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfgenFNfindGeneralInformationByFileNumberRequest Request/Response {requestResponse}", true);
                        }
                    }

                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "after findGeneralInformationByFileNumberResponse, request.fileNumber:" + request.fileNumber);
                    progressString = "After VEISfgenFNfindGeneralInformationByFileNumber EC Call";
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor, Progess:" + progressString, "after general");

                    response.ExceptionMessage = findGeneralInformationByFileNumberResponse.ExceptionMessage;
                    response.ExceptionOccured = findGeneralInformationByFileNumberResponse.ExceptionOccurred;

                    response.findGeneralResponse = findGeneralInformationByFileNumberResponse;

                    #region looking at award response
                    if (findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo != null)
                    {
                        if (findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.VEISfgenFNawardBenesInfo != null)
                        {
                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "many awards to create");
                            #region many awards to create
                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor, Progess:" + progressString, "Creating Multiple Awards");

                            VEISfgenFNawardBenesMultipleResponse[] generalInfoRecord = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.VEISfgenFNawardBenesInfo;

                            //var serviceDTO = findVeteranResponse.VIMTfvetreturnInfo.VIMTfvetvetBirlsRecordInfo.VIMTfvetSERVICEInfo;

                            //var LatestServiceItem = generalInfoRecordUnSorted.OrderByDescending(h => h.mcs_stENTERED_ON_DUTY_DATE).FirstOrDefault();
                            foreach (VEISfgenFNawardBenesMultipleResponse generalInfoSelectionItem in generalInfoRecord)
                            {
                                Entity peopleEntity = new Entity();
                                peopleEntity.LogicalName = "udo_person";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    peopleEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }
                                if (request.vetsnapshotId != System.Guid.Empty)
                                {
                                    peopleEntity["udo_vetsnapshotid"] = request.vetsnapshotId.ToString();
                                    peopleEntity["udo_veteransnapshotid"] = new EntityReference("udo_veteransnapshot", request.vetsnapshotId);
                                }

                                peopleEntity["udo_filenumber"] = request.fileNumber;
                                peopleEntity["udo_ssn"] = request.udo_ssn;
                                peopleEntity["udo_vetssn"] = request.udo_ssn;
                                peopleEntity["udo_awardsexist"] = true;
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    peopleEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }

                                UDOcreateAwardsMultipleResponse responseIds = new UDOcreateAwardsMultipleResponse();
                                //instantiate the new Entity
                                Entity thisNewEntity = new Entity("udo_award");
                                //thisNewEntity.LogicalName =;
                                thisNewEntity["udo_name"] = "Award Summary";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }

                                request.awardTypeCd = generalInfoSelectionItem.mcs_awardTypeCd;
                                request.ptcpntBeneId = generalInfoSelectionItem.mcs_ptcpntBeneId;      
                                request.ptcpntRecipId = generalInfoSelectionItem.mcs_ptcpntRecipId;    
                                request.ptcpntVetId = generalInfoSelectionItem.mcs_ptcpntVetId;

                                if (awardsCreated == 0)
                                {
                                    awardTypeCd = generalInfoSelectionItem.mcs_awardTypeCd;
                                    ptcpntBeneId = generalInfoSelectionItem.mcs_ptcpntBeneId;      
                                    ptcpntRecipId = generalInfoSelectionItem.mcs_ptcpntRecipId; 
                                    ptcpntVetId = generalInfoSelectionItem.mcs_ptcpntVetId;
                                }

                                peopleEntity["udo_ptcpntid"] = request.ptcpntRecipId;

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
                                if (!string.IsNullOrEmpty(generalInfoSelectionItem.mcs_awardBeneTypeName))
                                {
                                    thisNewEntity["udo_benefittypename"] = generalInfoSelectionItem.mcs_awardBeneTypeName;
                                    peopleEntity["udo_benefittypename"] = generalInfoSelectionItem.mcs_awardBeneTypeName;

                                    if (generalInfoSelectionItem.mcs_payeeCd == "00")
                                    {
                                        if (request.vetsnapshotId != Guid.Empty)
                                        {
                                            vetSnapShot["udo_awardtype"] = generalInfoSelectionItem.mcs_awardTypeCd + "-" + generalInfoSelectionItem.mcs_awardBeneTypeName;
                                            vetSnapShot["udo_awardtypecode"] = generalInfoSelectionItem.mcs_awardTypeCd;
                                            updateVetSnap = true;
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(generalInfoSelectionItem.mcs_awardBeneTypeCd))
                                {
                                    thisNewEntity["udo_benefittypecode"] = generalInfoSelectionItem.mcs_awardBeneTypeCd;
                                    peopleEntity["udo_benefittypecode"] = generalInfoSelectionItem.mcs_awardBeneTypeCd;
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
                                    peopleEntity["udo_awardtypecode"] = generalInfoSelectionItem.mcs_awardTypeCd;
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
                                    peopleEntity["udo_payeecode"] = generalInfoSelectionItem.mcs_payeeCd;
                                    peopleEntity["udo_payeename"] = generalInfoSelectionItem.mcs_recipName;

                                    if (generalInfoSelectionItem.mcs_payeeCd == "00")
                                    {
                                        progressString = getFidInformation(OrgServiceProxy, request, response, peopleEntity, thisNewEntity, progressString);
                                    }

                                    if (payeecodesCreated.IndexOf(generalInfoSelectionItem.mcs_payeeCd) == -1)
                                    {
                                        peopleEntity["udo_payeetypecode"] = generalInfoSelectionItem.mcs_payeeCd;
                                        peopleEntity["udo_payeename"] = generalInfoSelectionItem.mcs_recipName;

                                        payeecodesCreated += ":" + generalInfoSelectionItem.mcs_payeeCd;

                                        payeeCodeId = CreatePayeeCode(generalInfoSelectionItem.mcs_payeeCd, request.fileNumber, generalInfoSelectionItem.mcs_ptcpntRecipId, request.UDOcreateAwardsRelatedEntitiesInfo, generalInfoSelectionItem.mcs_recipName, request, OrgServiceProxy);
                                        if (generalInfoSelectionItem.mcs_payeeCd == "00")
                                        {
                                            payeeCode00Id = payeeCodeId;
                                        }
                                        peopleEntity["udo_payeecodeid"] = new EntityReference("udo_payeecode", payeeCodeId);
                                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor, Progess:" + progressString, "Creating Payee Code Record :" + generalInfoSelectionItem.mcs_payeeCd);
                                    }
                                    else
                                    {
                                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor, Progess:" + progressString, "not creating payee" + generalInfoSelectionItem.mcs_payeeCd);
                                    }
                                }
                                else
                                {
                                    peopleEntity["udo_payeecode"] = "00";
                                    peopleEntity["udo_payeename"] = generalInfoSelectionItem.mcs_recipName;
                                    payeeCodeId = CreatePayeeCode(generalInfoSelectionItem.mcs_payeeCd, request.fileNumber, generalInfoSelectionItem.mcs_ptcpntRecipId, request.UDOcreateAwardsRelatedEntitiesInfo, generalInfoSelectionItem.mcs_recipName, request, OrgServiceProxy);
                                    payeeCode00Id = payeeCodeId;
                                    peopleEntity["udo_payeecodeid"] = new EntityReference("udo_payeecode", payeeCodeId);

                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor, Progess:" + progressString, "Creating Payee Code Record :" + generalInfoSelectionItem.mcs_payeeCd);
                                    getVeteranformation(request, response, progressString, pctpntIdsCreated, requestCollection, null);
                                    progressString = getFidInformation(OrgServiceProxy, request, response, peopleEntity, thisNewEntity, progressString);
                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor, Progess:" + progressString, "NO Payee Code Record :" + generalInfoSelectionItem.mcs_payeeCd);

                                }

                                //single fields for all awards

                                MapSingletonFields(request, blnContinueWithCalls, findGeneralInformationByFileNumberResponse, thisNewEntity, peopleEntity);

                                if (request.UDOcreateAwardsRelatedEntitiesInfo != null)
                                {
                                    foreach (UDOcreateAwardsRelatedEntitiesMultipleRequest relatedItem in request.UDOcreateAwardsRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                        peopleEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                }

                                
                                awardId = OrgServiceProxy.Create(TruncateHelper.TruncateFields(thisNewEntity, request.OrganizationName, request.UserId, request.LogTiming, "Create", OrgServiceProxy));
                                awardsCreated += 1;

                                if (generalInfoSelectionItem.mcs_payeeCd == "00")
                                {
                                    award00Id = awardId;
                                }

                                if (peopleEntity.Attributes.Contains("udo_ptcpntid"))
                                {
                                    string thisPID = peopleEntity["udo_ptcpntid"].ToString();
                                    peopleEntity["udo_awardid"] = new EntityReference("udo_award", awardId);
                                    if (pctpntIdsCreated.IndexOf(thisPID) == -1)
                                    {
                                        pctpntIdsCreated += ";" + thisPID;
                                        CreateRequest createPeopleData = new CreateRequest
                                        {
                                            Target = peopleEntity,
                                            RequestName = "create"
                                        };
                                        requestCollection.Add(createPeopleData);

                                    }
                                    else
                                    {
                                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", string.Format("thisPID {0}, pidsCreated: {1}", thisPID, pctpntIdsCreated));
                                    }
                                }
                                else
                                {
                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "no PID for People");
                                }
                            }
                            #endregion
                        }
                        else
                        {

                            #region only 1 award to create
                            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntRecipID))
                            {
                                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "1 award to create");
                                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", string.Format("Recip {0}", findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntRecipID));

                                Entity peopleEntity = new Entity();
                                peopleEntity.LogicalName = "udo_person";
                                peopleEntity["udo_awardsexist"] = true;
                                if (request.vetsnapshotId != System.Guid.Empty)
                                {
                                    peopleEntity["udo_vetsnapshotid"] = request.vetsnapshotId.ToString();
                                    peopleEntity["udo_veteransnapshotid"] = new EntityReference("udo_veteransnapshot", request.vetsnapshotId);
                                }
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    peopleEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }

                                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor, Progess:" + progressString, "creating 1 award");

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
                                    peopleEntity["udo_awardtypecode"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_awardTypeCode;
                                }
                                else
                                {
                                    blnContinueWithCalls = false;
                                }

                                awardTypeCd = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_awardTypeCode;
                                ptcpntBeneId = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntBeneID;
                                ptcpntRecipId = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntRecipID;
                                ptcpntVetId = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntVetID;

                                if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_benefitTypeName))
                                {
                                    thisNewEntity["udo_benefittypename"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_benefitTypeName;
                                    peopleEntity["udo_benefittypename"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_benefitTypeName;

                                    if (findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeCode == "00")
                                    {
                                        if (request.vetsnapshotId != Guid.Empty)
                                        {
                                            vetSnapShot["udo_awardtype"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_awardTypeCode + "-" + findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_benefitTypeName;
                                            vetSnapShot["udo_awardtypecode"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_awardTypeCode;
                                            updateVetSnap = true;
                                        }
                                    }

                                }
                                if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_benefitTypeCode))
                                {
                                    thisNewEntity["udo_benefittypecode"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_benefitTypeCode;
                                    peopleEntity["udo_benefittypecode"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_benefitTypeCode;
                                }
                                if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeCode))
                                {
                                    if (findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeCode == "00")
                                    {
                                        peopleEntity["udo_payeecode"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeCode;
                                        peopleEntity["udo_vetfirstname"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetFirstName;
                                        peopleEntity["udo_vetlastname"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetLastName;
                                        peopleEntity["udo_gender"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetSex;
                                        peopleEntity["udo_payeename"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeName;
                                        peopleEntity["udo_name"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetFirstName + " " + findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetLastName;
                                        thisNewEntity["udo_payeetypecode"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeCode;
                                        progressString = getFidInformation(OrgServiceProxy, request, response, peopleEntity, thisNewEntity, progressString);
                                        payeeCodeId = CreatePayeeCode("00", request.fileNumber, request.ptcpntVetId, request.UDOcreateAwardsRelatedEntitiesInfo, "", request, OrgServiceProxy);
                                        peopleEntity["udo_payeecodeid"] = new EntityReference("udo_payeecode", payeeCodeId);
                                        payeeCode00Id = payeeCodeId;

                                    }
                                    else
                                    {
                                        peopleEntity["udo_vetfirstname"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetFirstName;
                                        peopleEntity["udo_vetlastname"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetLastName;
                                        peopleEntity["udo_payeename"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeName;
                                        peopleEntity["udo_payeetypecode"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeCode;
                                        peopleEntity["udo_payeecode"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeCode;

                                        payeeCodeId = CreatePayeeCode(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeCode, request.fileNumber, findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntRecipID, request.UDOcreateAwardsRelatedEntitiesInfo, "", request, OrgServiceProxy);
                                        peopleEntity["udo_payeecodeid"] = new EntityReference("udo_payeecode", payeeCodeId);
                                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor, Progess:" + progressString, "Creating Single Payee Code Record :" + findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeCode);

                                    }
                                }
                                else
                                {
                                    peopleEntity["udo_name"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetFirstName + " " + findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetLastName;
                                    peopleEntity["udo_payeecode"] = "00";
                                    peopleEntity["udo_vetfirstname"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetFirstName;
                                    peopleEntity["udo_vetlastname"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetLastName;
                                    peopleEntity["udo_gender"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetSex;
                                    peopleEntity["udo_payeename"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeName;
                                    payeeCodeId = CreatePayeeCode("00", request.fileNumber, request.ptcpntVetId, request.UDOcreateAwardsRelatedEntitiesInfo, "", request, OrgServiceProxy);
                                    payeeCode00Id = payeeCodeId;
                                    peopleEntity["udo_payeecodeid"] = new EntityReference("udo_payeecode", payeeCodeId);
                                    progressString = getFidInformation(OrgServiceProxy, request, response, peopleEntity, thisNewEntity, progressString);
                                    blnContinueWithCalls = false;

                                }

                                MapSingletonFields(request, blnContinueWithCalls, findGeneralInformationByFileNumberResponse, thisNewEntity, peopleEntity);

                                if (request.UDOcreateAwardsRelatedEntitiesInfo != null)
                                {
                                    foreach (UDOcreateAwardsRelatedEntitiesMultipleRequest relatedItem in request.UDOcreateAwardsRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                        peopleEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                }
                                
                                awardId = OrgServiceProxy.Create(TruncateHelper.TruncateFields(request.MessageId, thisNewEntity, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                                awardsCreated += 1;
                                if (findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeCode == "00")
                                {
                                    award00Id = awardId;
                                }

                                #endregion

                                if (peopleEntity.Attributes.Contains("udo_ptcpntid"))
                                {
                                    string thisPID = peopleEntity["udo_ptcpntid"].ToString();
                                    peopleEntity["udo_awardid"] = new EntityReference("udo_award", awardId);
                                    pctpntIdsCreated += ";" + thisPID;
                                    CreateRequest createPeopleData = new CreateRequest
                                    {
                                        Target = peopleEntity,
                                        RequestName = "create"
                                    };
                                    requestCollection.Add(createPeopleData);
                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", string.Format("thisPID {0}, pidsCreated: {1}", thisPID, pctpntIdsCreated));

                                }
                                else
                                {
                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "no PID for People");
                                }
                            }
                            else
                            {
                                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "had a record, but we had no recipid");
                            }

                        }
                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor, Progess:" + progressString, "Created this many awards:" + awardsCreated);

                    }
                    #endregion

                    #region - no payee code 00 created
                    if (payeeCode00Id == Guid.Empty)
                    {
                        //requestCollection.Clear();

                        Entity peopleEntity = new Entity();
                        peopleEntity.LogicalName = "udo_person";

                        peopleEntity["udo_awardsexist"] = false;
                        peopleEntity["udo_paymentcomplete"] = true;

                        peopleEntity["udo_fidexists"] = false;
                        payeeCodeId = CreatePayeeCode("00", request.fileNumber, request.ptcpntVetId, request.UDOcreateAwardsRelatedEntitiesInfo, "", request, OrgServiceProxy);
                        payeeCode00Id = payeeCodeId;
                        peopleEntity["udo_payeecodeid"] = new EntityReference("udo_payeecode", payeeCodeId);
                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor, Progess:" + progressString, "No awards for 00, creating payee and people record for it");
                        getVeteranformation(request, response, progressString, pctpntIdsCreated, requestCollection, peopleEntity);

                    }
                    else
                    {
                        getVeteranformation(request, response, progressString, pctpntIdsCreated, requestCollection, null);
                    }
                    #endregion



                    #region creating payeecode defaults


                    CreatePayeeCode("41", request.fileNumber, "000000000", request.UDOcreateAwardsRelatedEntitiesInfo, "", request, OrgServiceProxy);
                    CreatePayeeCode("42", request.fileNumber, "000000000", request.UDOcreateAwardsRelatedEntitiesInfo, "", request, OrgServiceProxy);
                    CreatePayeeCode("43", request.fileNumber, "000000000", request.UDOcreateAwardsRelatedEntitiesInfo, "", request, OrgServiceProxy);
                    CreatePayeeCode("44", request.fileNumber, "000000000", request.UDOcreateAwardsRelatedEntitiesInfo, "", request, OrgServiceProxy);
                    CreatePayeeCode("45", request.fileNumber, "000000000", request.UDOcreateAwardsRelatedEntitiesInfo, "", request, OrgServiceProxy);
                    CreatePayeeCode("50", request.fileNumber, "000000000", request.UDOcreateAwardsRelatedEntitiesInfo, "", request, OrgServiceProxy);
                    CreatePayeeCode("60", request.fileNumber, "000000000", request.UDOcreateAwardsRelatedEntitiesInfo, "", request, OrgServiceProxy);

                    #endregion

                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor, Progess:" + progressString, "Default Payee codes created");

                    progressString = getDependentInformation(request, response, progressString, pctpntIdsCreated, requestCollection);
                    //we only want 1 people entity created per payeetypecode
                    //It is possible for a veteran to have more than 1

                }
                else
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "NO FILENUMBER");
                }

                #region add records to CRM
                if (requestCollection.Count > 0)
                {

                    var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);

                    if (_debug)
                    {

                        LogBuffer += result.LogDetail;
                        //  LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                    }

                    if (result.IsFaulted)
                    {
                        LogHelper.LogError(request.OrganizationName, request.UserId, method, result.ErrorDetail);
                        response.ExceptionMessage = result.FriendlyDetail;
                        response.ExceptionOccured = true;
                        return response;
                    }

                }
                #endregion



                if (request.udo_dependentId != Guid.Empty)
                {
                    Entity dep = new Entity();
                    dep.Id = request.udo_dependentId;
                    dep.LogicalName = "udo_dependant";
                    dep["udo_awardcomplete"] = true;

                    OrgServiceProxy.Update(dep);
                }
                else
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "no dependentId found");
                }
                if (request.idProofId != Guid.Empty)
                {
                    Entity idProof = new Entity();
                    idProof.Id = request.idProofId;
                    idProof.LogicalName = "udo_idproof";
                    idProof["udo_awardintegration"] = new OptionSetValue(752280002);

                    OrgServiceProxy.Update(idProof);
                }
                else
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "no idProofId found");
                }

                if (award00Id != Guid.Empty)
                {
                    //if we have an award for the 00 payee, go
                    UDOretrieveAwardRequest retrieveAwardRequest = new UDOretrieveAwardRequest();
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

                    if (request.LegacyServiceHeaderInfo != null)
                    {
                        retrieveAwardRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
                        {
                            ///Header Info
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber,
                        };
                    }
                    // (TN): Original code has this already commented out
                    //var retrieveResponse = retrieveAwardRequest.SendReceive<UDOretrieveAwardResponse>(MessageProcessType.Local);

                    var retrieveAwardLogic = new UDOretrieveAwardProcessor();
                    var retrieveResponse = (UDORetrieveAwardResponse)retrieveAwardLogic.Execute(retrieveAwardRequest);

                    progressString = "After VIMT EC Call";
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
                //as soon as this is created, kick off the load of that payee code.
                if (payeeCode00Id != Guid.Empty)
                {
                    //loadPayments(payeeCode00Id, OrgServiceProxy, request);
                    
                    //TODO:  Murto - don't find a need for this routine
                    //loadPayments(payeeCode00Id, request);
                }
                if (request.vetsnapshotId != Guid.Empty)
                {
                    if (updateVetSnap)
                    {                        
                        OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, vetSnapShot, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                    }
                }               


                return response;
            }
            catch (Exception ExecutionException)
            {

                var errorString = string.Empty;
                errorString = string.Format("Failed to process Award - Last Progress Step({0})  {1} {2}", progressString, ExecutionException.Message.ToString(), ExecutionException.StackTrace.ToString());
                
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOcreateAwardsProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = errorString;
                response.ExceptionOccured = true;

                try
                {
                    if (request.idProofId != Guid.Empty)
                    {
                        Entity idProof = new Entity();
                        idProof.Id = request.idProofId;
                        idProof.LogicalName = "udo_idproof";
                        //mark it as errored
                        idProof["udo_awardintegration"] = new OptionSetValue(752280003);

                        OrgServiceProxy.Update(idProof);
                    }
                    else
                    {
                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "no idProofId found");
                    }
                }
                catch (Exception updateException)
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOcreateAwardsProcessor Processor, Progess:" + progressString, updateException);
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

        private static string getDependentInformation(UDOcreateAwardsRequest request, UDOcreateAwardsResponse response, string progressString, string pctpntIdsCreated, OrganizationRequestCollection requestCollection)
        {
            #region createdependents

            VEISfedpfindDependentsRequest findDependentsRequest = new VEISfedpfindDependentsRequest();
            findDependentsRequest.LogTiming = request.LogTiming;
            findDependentsRequest.LogSoap = request.LogSoap;
            findDependentsRequest.Debug = request.Debug;
            findDependentsRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            findDependentsRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            findDependentsRequest.RelatedParentId = request.RelatedParentId;
            findDependentsRequest.UserId = request.UserId;
            findDependentsRequest.OrganizationName = request.OrganizationName;

            findDependentsRequest.mcs_filenumber = request.fileNumber;
            if (request.LegacyServiceHeaderInfo != null)
            {
                findDependentsRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }
            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "About to call findDependentsRequest, request.fileNumber:" + request.fileNumber);
            
            var findDependentsResponse = WebApiUtility.SendReceive<VEISfedpfindDependentsResponse>(findDependentsRequest, WebApiType.VEIS);
            if (request.LogSoap || findDependentsResponse.ExceptionOccurred)
            {
                if (findDependentsResponse.SerializedSOAPRequest != null || findDependentsResponse.SerializedSOAPResponse != null)
                {
                    var requestResponse = findDependentsResponse.SerializedSOAPRequest + findDependentsResponse.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfedpfindDependentsRequest Request/Response {requestResponse}", true);
                }
            }

            progressString = "After VIMT EC Call";

            response.ExceptionMessage = findDependentsResponse.ExceptionMessage;
            response.ExceptionOccured = findDependentsResponse.ExceptionOccurred;
            if (findDependentsResponse.VEISfedpreturnInfo != null)
            {
                VEISfedppersonsMultipleResponse[] shrinq3Person = findDependentsResponse.VEISfedpreturnInfo.VEISfedppersonsInfo;
                if (shrinq3Person != null)
                {
                    foreach (VEISfedppersonsMultipleResponse shrinq3PersonItem in shrinq3Person)
                    {

                        string thisPID = shrinq3PersonItem.mcs_ptcpntId;

                        if (pctpntIdsCreated.IndexOf(thisPID) == -1)
                        {
                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "did not find this dependent PID, creating person:" + thisPID);

                            Entity peopleEntity = new Entity();
                            peopleEntity.LogicalName = "udo_person";
                            if (request.ownerId != System.Guid.Empty)
                            {
                                peopleEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }
                            if (request.vetsnapshotId != System.Guid.Empty)
                            {
                                peopleEntity["udo_vetsnapshotid"] = request.vetsnapshotId.ToString();
                                peopleEntity["udo_veteransnapshotid"] = new EntityReference("udo_veteransnapshot", request.vetsnapshotId);
                            }
                            if ((shrinq3PersonItem.mcs_awardIndicator == "N") || string.IsNullOrEmpty(shrinq3PersonItem.mcs_awardIndicator))
                            {
                                peopleEntity["udo_type"] = new OptionSetValue(752280001);
                            }
                            else
                            {
                                peopleEntity["udo_type"] = new OptionSetValue(752280002);
                            }

                            peopleEntity["udo_filenumber"] = shrinq3PersonItem.mcs_ssn;
                            peopleEntity["udo_ssn"] = shrinq3PersonItem.mcs_ssn;
                            peopleEntity["udo_vetssn"] = request.udo_ssn;
                            peopleEntity["udo_ptcpntid"] = shrinq3PersonItem.mcs_ptcpntId;
                            peopleEntity["udo_middle"] = shrinq3PersonItem.mcs_middleName;

                            peopleEntity["udo_last"] = shrinq3PersonItem.mcs_lastName;

                            peopleEntity["udo_gender"] = shrinq3PersonItem.mcs_gender;

                            peopleEntity["udo_first"] = shrinq3PersonItem.mcs_firstName;
                            peopleEntity["udo_name"] = shrinq3PersonItem.mcs_firstName + " " + shrinq3PersonItem.mcs_lastName;
                            if (!string.IsNullOrEmpty(shrinq3PersonItem.mcs_dateOfBirth))
                            {
                                //DateTime newDateTime;
                                //if (DateTime.TryParse(shrinq3PersonItem.mcs_dateOfBirth, out newDateTime))
                                //{

                                //    peopleEntity["udo_dob"] = newDateTime;
                                //}
                                peopleEntity["udo_dobstr"] = shrinq3PersonItem.mcs_dateOfBirth;
                            }
                            if (request.UDOcreateAwardsRelatedEntitiesInfo != null)
                            {
                                foreach (UDOcreateAwardsRelatedEntitiesMultipleRequest relatedItem in request.UDOcreateAwardsRelatedEntitiesInfo)
                                {
                                    peopleEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                }
                            }
                            #region address stuff
                            //var findAllPtcpntAddrsByPtcpntIdRequest = new VIMTfallpidaddpidfindAllPtcpntAddrsByPtcpntIdRequest();
                            //findAllPtcpntAddrsByPtcpntIdRequest.LogTiming = request.LogTiming;
                            //findAllPtcpntAddrsByPtcpntIdRequest.LogSoap = request.LogSoap;
                            //findAllPtcpntAddrsByPtcpntIdRequest.Debug = request.Debug;
                            //findAllPtcpntAddrsByPtcpntIdRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                            //findAllPtcpntAddrsByPtcpntIdRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                            //findAllPtcpntAddrsByPtcpntIdRequest.RelatedParentId = request.RelatedParentId;
                            //findAllPtcpntAddrsByPtcpntIdRequest.UserId = request.UserId;
                            //findAllPtcpntAddrsByPtcpntIdRequest.OrganizationName = request.OrganizationName;

                            //findAllPtcpntAddrsByPtcpntIdRequest.mcs_ptcpntid = Convert.ToInt64(thisPID);
                            //if (request.LegacyServiceHeaderInfo != null)
                            //{
                            //    findAllPtcpntAddrsByPtcpntIdRequest.LegacyServiceHeaderInfo = new VIMT.AddressWebService.Messages.HeaderInfo
                            //    {
                            //        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            //        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            //        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            //        Password = request.LegacyServiceHeaderInfo.Password,
                            //        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                            //    };
                            //}
                            // (TN): Original code already commented out
                            //var findAllPtcpntAddrsByPtcpntIdResponse = findAllPtcpntAddrsByPtcpntIdRequest.SendReceive<VIMTfallpidaddpidfindAllPtcpntAddrsByPtcpntIdResponse>(MessageProcessType.Local);
                            //progressString = "After VIMT EC Call";

                            //response.ExceptionMessage = findAllPtcpntAddrsByPtcpntIdResponse.ExceptionMessage;
                            //response.ExceptionOccured = findAllPtcpntAddrsByPtcpntIdResponse.ExceptionOccured;
                            //if (findAllPtcpntAddrsByPtcpntIdResponse.VIMTfallpidaddpidreturnInfo != null)
                            //{
                            //    var ptcpntAddrsDTO = findAllPtcpntAddrsByPtcpntIdResponse.VIMTfallpidaddpidreturnInfo;
                            //    foreach (var ptcpntAddrsDTOItem in ptcpntAddrsDTO)
                            //    {
                            //        if (ptcpntAddrsDTOItem.mcs_ptcpntAddrsTypeNm.ToLower() == "mailing")
                            //        {


                            //            if (ptcpntAddrsDTOItem.mcs_addrsOneTxt != string.Empty)
                            //            {
                            //                peopleEntity["udo_address1"] = ptcpntAddrsDTOItem.mcs_addrsOneTxt;
                            //            }
                            //            if (ptcpntAddrsDTOItem.mcs_addrsTwoTxt != string.Empty)
                            //            {
                            //                peopleEntity["udo_address2"] = ptcpntAddrsDTOItem.mcs_addrsTwoTxt;
                            //            }
                            //            if (ptcpntAddrsDTOItem.mcs_addrsThreeTxt != string.Empty)
                            //            {
                            //                peopleEntity["udo_address3"] = ptcpntAddrsDTOItem.mcs_addrsThreeTxt;
                            //            }
                            //            if (ptcpntAddrsDTOItem.mcs_cityNm != string.Empty)
                            //            {
                            //                peopleEntity["udo_city"] = ptcpntAddrsDTOItem.mcs_cityNm;
                            //            }
                            //            if (ptcpntAddrsDTOItem.mcs_cntryNm != string.Empty)
                            //            {
                            //                peopleEntity["udo_country"] = ptcpntAddrsDTOItem.mcs_cntryNm;
                            //            }
                            //            if (ptcpntAddrsDTOItem.mcs_emailAddrsTxt != string.Empty)
                            //            {
                            //                peopleEntity["udo_email"] = ptcpntAddrsDTOItem.mcs_emailAddrsTxt;
                            //            }
                            //            //if (ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd != string.Empty)
                            //            //{
                            //            //    thisEntity["udo_milpostal"] = ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd;
                            //            //}
                            //            //if (ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd != string.Empty)
                            //            //{
                            //            //    thisEntity["udo_milpotype"] = ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd;
                            //            //}
                            //            //if (ptcpntAddrsDTOItem.mcs_prvncNm != string.Empty)
                            //            //{
                            //            //    thisEntity["udo_province"] = ptcpntAddrsDTOItem.mcs_prvncNm;
                            //            //}
                            //            if (ptcpntAddrsDTOItem.mcs_countyNm != string.Empty)
                            //            {
                            //                peopleEntity["udo_state"] = ptcpntAddrsDTOItem.mcs_countyNm;
                            //            }
                            //            if (ptcpntAddrsDTOItem.mcs_zipPrefixNbr != string.Empty)
                            //            {
                            //                peopleEntity["udo_zip"] = ptcpntAddrsDTOItem.mcs_zipPrefixNbr;
                            //            }
                            //        }

                            //    }
                            //}
                            #endregion
                            pctpntIdsCreated += ";" + thisPID;
                            CreateRequest createPeopleData = new CreateRequest
                            {
                                Target = peopleEntity,
                                RequestName = "create"
                            };
                            requestCollection.Add(createPeopleData);
                        }
                        else
                        {
                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "DID find this dependent PID, creating person:" + thisPID);

                            foreach (OrganizationRequest item in requestCollection)
                            {
                                if (item.RequestName == "create")
                                {
                                    CreateRequest test = (CreateRequest)item;
                                    Entity thisEntity = (Entity)test["Target"];
                                    if (thisEntity["udo_ptcpntid"].ToString() == thisPID)
                                    {
                                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "Found Person Stub:" + thisPID);

                                        if ((shrinq3PersonItem.mcs_awardIndicator == "N") || string.IsNullOrEmpty(shrinq3PersonItem.mcs_awardIndicator))
                                        {
                                            thisEntity["udo_type"] = new OptionSetValue(752280001);
                                        }
                                        else
                                        {
                                            thisEntity["udo_type"] = new OptionSetValue(752280002);
                                        }
                                        thisEntity["udo_filenumber"] = shrinq3PersonItem.mcs_ssn;
                                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "use this for filenumber and ssn shrinq3PersonItem.mcs_ssn:" + shrinq3PersonItem.mcs_ssn);
                                        thisEntity["udo_ssn"] = shrinq3PersonItem.mcs_ssn;
                                        thisEntity["udo_ptcpntid"] = shrinq3PersonItem.mcs_ptcpntId;
                                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "shrinq3PersonItem.mcs_ptcpntId:" + shrinq3PersonItem.mcs_ptcpntId);
                                        thisEntity["udo_middle"] = shrinq3PersonItem.mcs_middleName;

                                        thisEntity["udo_last"] = shrinq3PersonItem.mcs_lastName;

                                        thisEntity["udo_gender"] = shrinq3PersonItem.mcs_gender;

                                        thisEntity["udo_first"] = shrinq3PersonItem.mcs_firstName;
                                        thisEntity["udo_name"] = shrinq3PersonItem.mcs_firstName + " " + shrinq3PersonItem.mcs_lastName;
                                        if (!string.IsNullOrEmpty(shrinq3PersonItem.mcs_dateOfBirth))
                                        {
                                            //DateTime newDateTime;
                                            //if (DateTime.TryParse(shrinq3PersonItem.mcs_dateOfBirth, out newDateTime))
                                            //{

                                            //    thisEntity["udo_dob"] = newDateTime;

                                            //}
                                            thisEntity["udo_dobstr"] = shrinq3PersonItem.mcs_dateOfBirth;
                                        }
                                        #region addresses
                                        //var findAllPtcpntAddrsByPtcpntIdRequest = new VIMTfallpidaddpidfindAllPtcpntAddrsByPtcpntIdRequest();
                                        //findAllPtcpntAddrsByPtcpntIdRequest.LogTiming = request.LogTiming;
                                        //findAllPtcpntAddrsByPtcpntIdRequest.LogSoap = request.LogSoap;
                                        //findAllPtcpntAddrsByPtcpntIdRequest.Debug = request.Debug;
                                        //findAllPtcpntAddrsByPtcpntIdRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                                        //findAllPtcpntAddrsByPtcpntIdRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                                        //findAllPtcpntAddrsByPtcpntIdRequest.RelatedParentId = request.RelatedParentId;
                                        //findAllPtcpntAddrsByPtcpntIdRequest.UserId = request.UserId;
                                        //findAllPtcpntAddrsByPtcpntIdRequest.OrganizationName = request.OrganizationName;

                                        //findAllPtcpntAddrsByPtcpntIdRequest.mcs_ptcpntid = Convert.ToInt64(thisPID);
                                        //if (request.LegacyServiceHeaderInfo != null)
                                        //{
                                        //    findAllPtcpntAddrsByPtcpntIdRequest.LegacyServiceHeaderInfo = new VIMT.AddressWebService.Messages.HeaderInfo
                                        //    {
                                        //        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                        //        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                        //        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                        //        Password = request.LegacyServiceHeaderInfo.Password,
                                        //        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                                        //    };
                                        //}
                                        // (TN): Original code already commented
                                        //var findAllPtcpntAddrsByPtcpntIdResponse = findAllPtcpntAddrsByPtcpntIdRequest.SendReceive<VIMTfallpidaddpidfindAllPtcpntAddrsByPtcpntIdResponse>(MessageProcessType.Local);
                                        //progressString = "After VIMT EC Call";

                                        //response.ExceptionMessage = findAllPtcpntAddrsByPtcpntIdResponse.ExceptionMessage;
                                        //response.ExceptionOccured = findAllPtcpntAddrsByPtcpntIdResponse.ExceptionOccured;
                                        //if (findAllPtcpntAddrsByPtcpntIdResponse.VIMTfallpidaddpidreturnInfo != null)
                                        //{
                                        //    var ptcpntAddrsDTO = findAllPtcpntAddrsByPtcpntIdResponse.VIMTfallpidaddpidreturnInfo;
                                        //    foreach (var ptcpntAddrsDTOItem in ptcpntAddrsDTO)
                                        //    {
                                        //        if (ptcpntAddrsDTOItem.mcs_ptcpntAddrsTypeNm.ToLower() == "mailing")
                                        //        {
                                        //            if (ptcpntAddrsDTOItem.mcs_addrsOneTxt != string.Empty)
                                        //            {
                                        //                thisEntity["udo_address1"] = ptcpntAddrsDTOItem.mcs_addrsOneTxt;
                                        //            }
                                        //            if (ptcpntAddrsDTOItem.mcs_addrsTwoTxt != string.Empty)
                                        //            {
                                        //                thisEntity["udo_address2"] = ptcpntAddrsDTOItem.mcs_addrsTwoTxt;
                                        //            }
                                        //            if (ptcpntAddrsDTOItem.mcs_addrsThreeTxt != string.Empty)
                                        //            {
                                        //                thisEntity["udo_address3"] = ptcpntAddrsDTOItem.mcs_addrsThreeTxt;
                                        //            }
                                        //            if (ptcpntAddrsDTOItem.mcs_cityNm != string.Empty)
                                        //            {
                                        //                thisEntity["udo_city"] = ptcpntAddrsDTOItem.mcs_cityNm;
                                        //            }
                                        //            if (ptcpntAddrsDTOItem.mcs_cntryNm != string.Empty)
                                        //            {
                                        //                thisEntity["udo_country"] = ptcpntAddrsDTOItem.mcs_cntryNm;
                                        //            }
                                        //            if (ptcpntAddrsDTOItem.mcs_emailAddrsTxt != string.Empty)
                                        //            {
                                        //                thisEntity["udo_email"] = ptcpntAddrsDTOItem.mcs_emailAddrsTxt;
                                        //            }
                                        //            //if (ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd != string.Empty)
                                        //            //{
                                        //            //    thisEntity["udo_milpostal"] = ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd;
                                        //            //}
                                        //            //if (ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd != string.Empty)
                                        //            //{
                                        //            //    thisEntity["udo_milpotype"] = ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd;
                                        //            //}
                                        //            //if (ptcpntAddrsDTOItem.mcs_prvncNm != string.Empty)
                                        //            //{
                                        //            //    thisEntity["udo_province"] = ptcpntAddrsDTOItem.mcs_prvncNm;
                                        //            //}
                                        //            if (ptcpntAddrsDTOItem.mcs_countyNm != string.Empty)
                                        //            {
                                        //                thisEntity["udo_state"] = ptcpntAddrsDTOItem.mcs_countyNm;
                                        //            }
                                        //            if (ptcpntAddrsDTOItem.mcs_zipPrefixNbr != string.Empty)
                                        //            {
                                        //                thisEntity["udo_zip"] = ptcpntAddrsDTOItem.mcs_zipPrefixNbr;
                                        //            }
                                        //        }

                                        //    }
                                        //}
                                        #endregion
                                    }
                                }
                            }
                        }
                        //instantiate the new Entity

                        //if (findDependentsResponse.VIMTfedpreturnclmsInfo.VIMTfedppersonsclmsInfo.mcs_currentRelateStatus ))
                        //{
                        //    thisEntity["udo_currrelatestatus"] = findDependentsResponse.VIMTfedpreturnclmsInfo.VIMTfedppersonsclmsInfo.mcs_currentRelateStatus;
                        //}


                    }
                }
            }
            #endregion
            return progressString;
        }

        private static string getVeteranformation(UDOcreateAwardsRequest request, UDOcreateAwardsResponse response, string progressString, string pctpntIdsCreated, OrganizationRequestCollection requestCollection, Entity existingPeopleEntity)
        {
            #region - process corporate data
            VEIScrpFNfindCorporateRecordByFileNumberRequest findCorporateRecordByFileNumberRequest = new VEIScrpFNfindCorporateRecordByFileNumberRequest();
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
            
            //REM : Invoke VEIS WebApi
            var findCorporateRecordByFileNumberResponse = WebApiUtility.SendReceive<VEIScrpFNfindCorporateRecordByFileNumberResponse>(findCorporateRecordByFileNumberRequest, WebApiType.VEIS);
            if (request.LogSoap || findCorporateRecordByFileNumberResponse.ExceptionOccurred)
            {
                if (findCorporateRecordByFileNumberResponse.SerializedSOAPRequest != null || findCorporateRecordByFileNumberResponse.SerializedSOAPResponse != null)
                {
                    var requestResponse = findCorporateRecordByFileNumberResponse.SerializedSOAPRequest + findCorporateRecordByFileNumberResponse.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEIScrpFNfindCorporateRecordByFileNumberRequest Request/Response {requestResponse}", true);
                }
            }

            progressString = "After VEIScrpFNfindCorporateRecordByFileNumber EC Call";
            response.ExceptionMessage = findCorporateRecordByFileNumberResponse.ExceptionMessage;
            response.ExceptionOccured = findCorporateRecordByFileNumberResponse.ExceptionOccurred;
            if (findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo != null)
            {

                string thisPID = request.ptcpntVetId;
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

                bool blnCreateRecord = false;
                if (thisPID != null && pctpntIdsCreated.IndexOf(thisPID) == -1)
                {
                    blnCreateRecord = true;
                }
                else
                {
                    foreach (OrganizationRequest item in requestCollection)
                    {
                        if (item.RequestName == "create")
                        {
                            CreateRequest test = (CreateRequest)item;
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
                    foreach (UDOcreateAwardsRelatedEntitiesMultipleRequest relatedItem in request.UDOcreateAwardsRelatedEntitiesInfo)
                    {
                        peopleEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                    }
                }
                VEIScrpFNreturn shrinq2Person = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo;

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
                    //DateTime newDateTime;
                    //if (DateTime.TryParse(findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_dateOfBirth, out newDateTime))
                    //{
                    //    peopleEntity["udo_dob"] = newDateTime;
                    //}
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

        private static bool MapSingletonFields(UDOcreateAwardsRequest request, bool blnContinueWithCalls, VEIS.Messages.ClaimantService.VEISfgenFNfindGeneralInformationByFileNumberResponse findGeneralInformationByFileNumberResponse, Entity thisNewEntity, Entity peopleEntity)
        {


            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetLastName))
            {
                thisNewEntity["udo_vetlastname"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetLastName;
                peopleEntity["udo_vetlastname"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetLastName;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetFirstName))
            {
                thisNewEntity["udo_vetfirstname"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetFirstName;
                peopleEntity["udo_vetfirstname"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetFirstName;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetSSN))
            {
                peopleEntity["udo_vetssn"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_vetSSN;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_statusReasonDate))
            {
                DateTime newDateTime;
                string newDate = dateStringFormat(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_statusReasonDate);
                if (DateTime.TryParse(newDate, out newDateTime))
                {
                    thisNewEntity["udo_statusreasondate"] = newDateTime;
                    peopleEntity["udo_statusreasondate"] = newDateTime;
                }
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeName))
            {
                thisNewEntity["udo_payeetypename"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeName;
                peopleEntity["udo_payeetypename"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeTypeName;
            }

            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeName))
            {
                thisNewEntity["udo_payeename"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeName;
                peopleEntity["udo_payeename"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeName;
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
                peopleEntity["udo_ptcpntid"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_ptcpntRecipID;
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
                peopleEntity["udo_ssn"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeSSN;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeSex))
            {
                thisNewEntity["udo_payeesex"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeSex;
                peopleEntity["udo_gender"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeSex;
            }
            if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeBirthDate))
            {
                thisNewEntity["udo_payeedobstr"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeBirthDate; ;
                peopleEntity["udo_dobstr"] = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_payeeBirthDate; ;
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
            string returnField = "";
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

        private static string getFidInformation(IOrganizationService OrgServiceProxy, UDOcreateAwardsRequest request, UDOcreateAwardsResponse response, Entity peopleEntity, Entity thisNewEntity, string progressString)
        {
            VEISfidfindFiduciaryRequest findFiduciaryRequest = new VEISfidfindFiduciaryRequest();
            findFiduciaryRequest.LogTiming = request.LogTiming;
            findFiduciaryRequest.LogSoap = request.LogSoap;
            findFiduciaryRequest.Debug = request.Debug;
            findFiduciaryRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            findFiduciaryRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            findFiduciaryRequest.RelatedParentId = request.RelatedParentId;
            findFiduciaryRequest.UserId = request.UserId;
            findFiduciaryRequest.OrganizationName = request.OrganizationName;

            findFiduciaryRequest.mcs_filenumber = request.fileNumber;

            if (request.LegacyServiceHeaderInfo != null)
            {
                findFiduciaryRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber,
                };
            }

            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "About to call findFiduciaryRequest, request.fileNumber:" + request.fileNumber);
            progressString = "Before VIMTfidfindFiduciary EC Call";

            var findFiduciaryResponse = WebApiUtility.SendReceive<VEISfidfindFiduciaryResponse>(findFiduciaryRequest, WebApiType.VEIS);
            if (request.LogSoap || findFiduciaryResponse.ExceptionOccurred)
            {
                if (findFiduciaryResponse.SerializedSOAPRequest != null || findFiduciaryResponse.SerializedSOAPResponse != null)
                {
                    var requestResponse = findFiduciaryResponse.SerializedSOAPRequest + findFiduciaryResponse.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfidfindFiduciaryRequest Request/Response {requestResponse}", true);
                }
            }

            progressString = "After VIMTfidfindFiduciary EC Call";

            if (findFiduciaryResponse.VEISfidreturnInfo != null)
            {
                response.ExceptionMessage = findFiduciaryResponse.ExceptionMessage;
                response.ExceptionOccured = findFiduciaryResponse.ExceptionOccurred;

                VEISfidreturn fidInfo = findFiduciaryResponse.VEISfidreturnInfo;

                peopleEntity["udo_fidexists"] = true;

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
                    string newDate = dateStringFormat(fidInfo.mcs_endDate);
                    if (DateTime.TryParse(newDate, out newDateTime))
                    {
                        thisNewEntity["udo_fiduciaryenddate"] = newDateTime;
                    }
                }
                if (!string.IsNullOrEmpty(fidInfo.mcs_beginDate))
                {
                    DateTime newDateTime;
                    string newDate = dateStringFormat(fidInfo.mcs_beginDate);
                    if (DateTime.TryParse(newDate, out newDateTime))
                    {
                        thisNewEntity["udo_fiduciarybegindate"] = newDateTime;
                    }
                }
            }
            return progressString;
        }

        private static void loadPayments(Guid payeeCodeId, UDOcreateAwardsRequest request)
        {
            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor", "loadPayments");

            UDORetrievePaymentsRequest thisRequest = new UDORetrievePaymentsRequest();
            thisRequest.payeeCodeId = payeeCodeId;
            thisRequest.OrganizationName = request.OrganizationName;
            thisRequest.Debug = request.Debug;
            thisRequest.UserId = request.UserId;
            thisRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            thisRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            thisRequest.RelatedParentId = request.RelatedParentId;

            WebApiUtility.SendAsync(thisRequest, WebApiType.LOB);

        }

        private Guid CreatePayeeCode(string payeeCode, string fileNumber, string participantId, UDOcreateAwardsRelatedEntitiesMultipleRequest[] udoCreateAwardsRelatedEntitiesMultipleRequest, string recipientName, UDOcreateAwardsRequest request, CrmServiceClient OrgServiceProxy)
        {
            Guid rtnGuid = Guid.Empty;

            Entity payeeCodeDefaults = new Entity();
            payeeCodeDefaults.LogicalName = "udo_payeecode";
            if (request.ownerId != System.Guid.Empty)
            {
                payeeCodeDefaults["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
            }
            payeeCodeDefaults["udo_name"] = GeneratePayeeCodeName(payeeCode, recipientName);
            payeeCodeDefaults["udo_payeecode"] = payeeCode;
            payeeCodeDefaults["udo_filenumber"] = fileNumber;
            payeeCodeDefaults["udo_participantid"] = participantId;

            if (udoCreateAwardsRelatedEntitiesMultipleRequest != null)
            {
                foreach (UDOcreateAwardsRelatedEntitiesMultipleRequest relatedItem in udoCreateAwardsRelatedEntitiesMultipleRequest)
                {
                    payeeCodeDefaults[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                }
            }
            
            rtnGuid = OrgServiceProxy.Create(TruncateHelper.TruncateFields(request.MessageId, payeeCodeDefaults, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));

            return rtnGuid;
        }

        private string GeneratePayeeCodeName(string payeeCode, string recipient)
        {
            string payeeCodeName = string.Empty;

            switch (payeeCode)
            {
                case "00":
                    payeeCodeName = "Veteran - 00";
                    break;
                case "10":
                    payeeCodeName = "Spouse [name] - 10";
                    payeeCodeName = payeeCodeName.Replace("[name]", recipient);
                    break;
                case "11":
                    payeeCodeName = "Child [name] - 11";
                    payeeCodeName = payeeCodeName.Replace("[name]", recipient);
                    break;
                case "12":
                    payeeCodeName = "Child [name] - 12";
                    payeeCodeName = payeeCodeName.Replace("[name]", recipient);
                    break;
                case "13":
                    payeeCodeName = "Child [name] - 13";
                    payeeCodeName = payeeCodeName.Replace("[name]", recipient);
                    break;
                case "14":
                    payeeCodeName = "Child [name] - 14";
                    payeeCodeName = payeeCodeName.Replace("[name]", recipient);
                    break;
                case "15":
                    payeeCodeName = "Child [name] - 15";
                    payeeCodeName = payeeCodeName.Replace("[name]", recipient);
                    break;
                case "16":
                    payeeCodeName = "Child [name] - 16";
                    payeeCodeName = payeeCodeName.Replace("[name]", recipient);
                    break;
                case "17":
                    payeeCodeName = "Child [name] - 17";
                    payeeCodeName = payeeCodeName.Replace("[name]", recipient);
                    break;
                case "18":
                    payeeCodeName = "Child [name] - 18";
                    payeeCodeName = payeeCodeName.Replace("[name]", recipient);
                    break;
                case "19":
                    payeeCodeName = "Child [name] - 19";
                    payeeCodeName = payeeCodeName.Replace("[name]", recipient);
                    break;
                case "20":
                    payeeCodeName = "Child [name] - 20";
                    payeeCodeName = payeeCodeName.Replace("[name]", recipient);
                    break;
                case "41":
                    payeeCodeName = "CH35 First Child - 41";
                    break;
                case "42":
                    payeeCodeName = "CH35 Second Child - 42";
                    break;
                case "43":
                    payeeCodeName = "CH35 Third Child - 43";
                    break;
                case "44":
                    payeeCodeName = "CH35 Fourth Child - 44";
                    break;
                case "45":
                    payeeCodeName = "CH35 Fifth Child - 45";
                    break;
                case "50":
                    payeeCodeName = "Dependent Father - 50";
                    break;
                case "60":
                    payeeCodeName = "Dependent Mother - 60";
                    break;
                default:
                    payeeCodeName = payeeCode;
                    break;
            }

            return payeeCodeName;
        }
    }
}