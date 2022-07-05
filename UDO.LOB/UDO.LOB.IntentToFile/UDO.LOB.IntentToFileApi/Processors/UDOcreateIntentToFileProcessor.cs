
namespace UDO.LOB.IntentToFile.Processors
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using System;
    using System.Linq;
    using UDO.LOB.Extensions;
    using UDO.LOB.Extensions.Logging;
    using UDO.LOB.Core;
    using UDO.LOB.IntentToFile.Messages;
    using VEIS.Core.Messages;
    using VEIS.Messages.IntentToFileWebService;
    using Microsoft.Xrm.Tooling.Connector;

    class UDOcreateIntentToFileProcessor
    {
        private CrmServiceClient OrgServiceProxy;
        private bool _debug { get; set; }

        private const string method = "UDOcreateIntentToFileProcessor";

        public IMessageBase Execute(UDOcreateIntentToFileRequest request)
        {
            UDOcreateIntentToFileResponse response = new UDOcreateIntentToFileResponse
            {
                MessageId = request.MessageId
            };

            string progressString = "Top of Processor";
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccurred = true;
                return response;
            }

            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = method,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }

            #region connect to CRM
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }
            #endregion

            progressString = "After CRM connection";

            try
            {
                var intentToFileRequest = new VEISfindInt2FilePrtId_findIntentToFileByPtcpntIdRequest
                {
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    mcs_ptcpntid = request.PtcpntId,

                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                };

                Entity intentToFileLoading = new Entity("contact");
                intentToFileLoading.Id = intentToFileRequest.RelatedParentId;
                intentToFileLoading["udo_itfloading"] = new OptionSetValue(752280000);
                OrgServiceProxy.Update(intentToFileLoading);

                // REM: Invoke VEIS Endpoint
                progressString = "Calling VEIS EC...";
                var intentToFileResponse = WebApiUtility.SendReceive<VEISfindInt2FilePrtId_findIntentToFileByPtcpntIdResponse>(intentToFileRequest, WebApiType.VEIS);
                progressString = "After VEIS EC Call";

                if (request.LogSoap || intentToFileResponse.ExceptionOccurred)
                {
                    if (intentToFileResponse.SerializedSOAPRequest != null || intentToFileResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = intentToFileResponse.SerializedSOAPRequest + intentToFileResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfindInt2FilePrtId_findIntentToFileByPtcpntIdRequest Request/Response {requestResponse}", true);
                    }
                }

                response.ExceptionMessage = intentToFileResponse.ExceptionMessage;
                response.ExceptionOccurred = intentToFileResponse.ExceptionOccurred;

                var requestCollection = new OrganizationRequestCollection();

                if (intentToFileResponse.VEISfindInt2FilePrtId_intentToFileDTOInfo != null)
                {
                    var intentToFileintentToFileItem = intentToFileResponse.VEISfindInt2FilePrtId_intentToFileDTOInfo;

                    progressString = "Begin processing each returned ITF record from VEIS EC...";
                    foreach (var intentToFileItem in intentToFileintentToFileItem)
                    {
                        var update = false;

                        var thisNewEntity = new Entity { LogicalName = "va_intenttofile" };
                        thisNewEntity["udo_fromlegacy"] = true;
                        thisNewEntity["udo_intenttofileexternalid"] = intentToFileItem.mcs_intentToFileId.ToString();

                        foreach (var crmITF in request.UDOgeneratedITFRecordsInfo)
                        {
                            if (crmITF.ITFExternalID.Equals(intentToFileItem.mcs_intentToFileId))
                            {
                                update = true;
                                thisNewEntity.Attributes.Remove("udo_fromlegacy");
                                thisNewEntity.Id = crmITF.ITFCrmGuid;
                            }
                        }

                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_jrnUserId))
                        {
                            thisNewEntity["va_userid"] = intentToFileItem.mcs_jrnUserId;
                        }

                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_jrnLctnId))
                        {
                            thisNewEntity["va_stationlocation"] = intentToFileItem.mcs_jrnLctnId;
                        }

                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_itfStatusTypeCd))
                        {
                            thisNewEntity["va_intenttofilestatus"] = intentToFileItem.mcs_itfStatusTypeCd;
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_submtrApplcnTypeCd))
                        {

                            thisNewEntity["va_sourceapplicationname"] = intentToFileItem.mcs_submtrApplcnTypeCd;
                        }
                        if (intentToFileItem.mcs_ptcpntVetIdSpecified)
                        {
                            thisNewEntity["va_participantid"] = intentToFileItem.mcs_ptcpntVetId.ToString();
                        }

                        if (intentToFileItem.mcs_ptcpntClmantIdSpecified)
                        {
                            thisNewEntity["va_claimantparticipantid"] = intentToFileItem.mcs_ptcpntClmantId.ToString();
                        }

                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_itfTypeCd))
                        {
                            switch (intentToFileItem.mcs_itfTypeCd)
                            {
                                case "C":
                                    thisNewEntity["va_generalbenefittype"] = new OptionSetValue(953850000);
                                    break;
                                case "P":
                                    thisNewEntity["va_generalbenefittype"] = new OptionSetValue(953850001);
                                    break;
                                case "S":
                                    thisNewEntity["va_generalbenefittype"] = new OptionSetValue(953850002);
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (intentToFileItem.mcs_createDt != null)
                        {
                            DateTime newDateTime;
                            if (DateTime.TryParse(intentToFileItem.mcs_createDt, out newDateTime))
                            {
                                thisNewEntity["va_createddate"] = newDateTime;
                            }

                        }
                        if (intentToFileItem.mcs_rcvdDt != null)
                        {
                            DateTime newDateTime;
                            if (DateTime.TryParse(intentToFileItem.mcs_rcvdDt, out newDateTime))
                            {
                                thisNewEntity["va_intenttofiledate"] = newDateTime;
                            }
                        }


                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_clmantAddrsOneTxt))
                        {
                            thisNewEntity["va_veteranaddressline1"] = intentToFileItem.mcs_clmantAddrsOneTxt;
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_clmantAddrsTwoTxt))
                        {
                            thisNewEntity["va_veteranaddressline2"] = intentToFileItem.mcs_clmantAddrsTwoTxt;
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_clmantAddrsUnitNbr))
                        {
                            thisNewEntity["va_veteranunitnumber"] = intentToFileItem.mcs_clmantAddrsUnitNbr;
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_clmantCityNm))
                        {
                            thisNewEntity["va_veterancity"] = intentToFileItem.mcs_clmantCityNm;
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_clmantCntryNm))
                        {
                            thisNewEntity["va_veterancountry"] = intentToFileItem.mcs_clmantCntryNm;
                            if (intentToFileItem.mcs_clmantCntryNm == "USA")
                            {
                                thisNewEntity["va_addresstype"] = new OptionSetValue(953850000);
                            }
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_clmantEmailAddrsTxt))
                        {
                            thisNewEntity["va_veteranemail"] = intentToFileItem.mcs_clmantEmailAddrsTxt;
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_clmantFirstNm))
                        {
                            thisNewEntity["va_claimantfirstname"] = intentToFileItem.mcs_clmantFirstNm;
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_clmantLastNm))
                        {
                            thisNewEntity["va_claimantlastname"] = intentToFileItem.mcs_clmantLastNm;
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_clmantMiddleNm))
                        {
                            thisNewEntity["va_claimantmiddleinitial"] = intentToFileItem.mcs_clmantMiddleNm;
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_clmantPhoneAreaNbr) && !string.IsNullOrEmpty(intentToFileItem.mcs_clmantPhoneAreaNbr))
                        {
                            thisNewEntity["va_veteranphone"] = intentToFileItem.mcs_clmantPhoneAreaNbr + intentToFileItem.mcs_clmantPhoneNbr;
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_clmantSsn))
                        {
                            thisNewEntity["va_claimantssn"] = intentToFileItem.mcs_clmantSsn;
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_clmantStateCd))
                        {
                            thisNewEntity["va_veteranstate"] = intentToFileItem.mcs_clmantStateCd;
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_clmantZipCd))
                        {
                            thisNewEntity["va_veteranzip"] = intentToFileItem.mcs_clmantZipCd;
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_genderCd))
                        {
                            if (intentToFileItem.mcs_genderCd == "F")
                            {
                                thisNewEntity["va_veterangender"] = true;
                            }
                        }
                        if (intentToFileItem.mcs_vetBrthdyDtSpecified)
                        {
                            DateTime newDateTime;
                            if (DateTime.TryParse(intentToFileItem.mcs_vetBrthdyDt, out newDateTime))
                            {
                                thisNewEntity["va_veterandateofbirth"] = newDateTime;
                            }
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_vetFileNbr))
                        {
                            thisNewEntity["va_veteranfilenumber"] = intentToFileItem.mcs_vetFileNbr;
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_vetFirstNm))
                        {
                            thisNewEntity["va_veteranfirstname"] = intentToFileItem.mcs_vetFirstNm;
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_vetLastNm))
                        {
                            thisNewEntity["va_veteranlastname"] = intentToFileItem.mcs_vetLastNm;
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_vetMiddleNm))
                        {
                            thisNewEntity["va_veteranmiddleinitial"] = intentToFileItem.mcs_vetMiddleNm;
                        }
                        if (!string.IsNullOrEmpty(intentToFileItem.mcs_vetSsnNbr))
                        {
                            thisNewEntity["va_veteranssn"] = intentToFileItem.mcs_vetSsnNbr;
                        }
                        foreach (var relatedReference in request.UDOcreateITFRelatedEntitiesInfo)
                        {
                            thisNewEntity[relatedReference.RelatedEntityFieldName] = new EntityReference(relatedReference.RelatedEntityName, relatedReference.RelatedEntityId);
                        }

                        if (update)
                        {
                            UpdateRequest updateData = new UpdateRequest
                            {
                                Target = thisNewEntity
                            };
                            requestCollection.Add(updateData);
                        }
                        else
                        {
                            CreateRequest createData = new CreateRequest
                            {
                                Target = thisNewEntity
                            };

                            requestCollection.Add(createData);
                        }
                    }
                }

                progressString = "Finished processing each returned ITF record from VEIS EC.";

                #region Create records

                if (requestCollection.Count() > 0)
                {
                    progressString = "Calling CRM execute multiple command";
                    var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);
                    progressString = "Finished calling CRM execute multiple command";

                    if (result != null)
                    {
                        if (_debug && result.LogDetail != null)
                        {
                            LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, result.LogDetail);
                        }

                        if (result.IsFaulted)
                        {
                            if (result.ErrorDetail != null && result.FriendlyDetail != null)
                            {
                                LogHelper.LogError(request.OrganizationName, request.UserId, method, result.ErrorDetail);
                                response.ExceptionMessage = result.FriendlyDetail;
                            }

                            response.ExceptionOccurred = true;
                            return response;
                        }

                        progressString = "Updating ITF entity...";
                        intentToFileLoading.Id = intentToFileRequest.RelatedParentId;
                        intentToFileLoading["udo_itfloading"] = new OptionSetValue(752280001);
                        OrgServiceProxy.Update(intentToFileLoading);
                        progressString = "ITF entity updated.";
                    }
                }

                string logInfo = string.Format("Intent To File Records Created: {0}", requestCollection.Count());
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Intent To File Records Created", logInfo);
                #endregion

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOcreateUDOIntentToFileProcessor Processor, Progess made before failure: [" + progressString + "]", ExecutionException);
                response.ExceptionMessage = "Failed to process Intent To File Data";
                response.ExceptionOccurred = true;
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

    }
}