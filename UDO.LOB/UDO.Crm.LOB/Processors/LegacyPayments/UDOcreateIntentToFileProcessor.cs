using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Linq;
using VIMT.IntentToFileWebService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Common;
using VRM.Integration.UDO.IntentToFile.Messages;
using UDO.Model;

namespace VRM.Integration.UDO.IntentToFile.Processors
{
    class UDOcreateIntentToFileProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOcreateIntentToFileProcessor";
        private string LogBuffer { get; set; }
        
        public IMessageBase Execute(UDOcreateIntentToFileRequest request)
        {
            //var request = message as createUDOLegacyPaymentDataRequest;
            UDOcreateIntentToFileResponse response = new UDOcreateIntentToFileResponse();
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
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateUDOLegacyPaymentDataProcessor Processor, Connection Error", connectException.Message);                
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                var intentToFileRequest = new VIMTfindInt2FilePrtId_findIntentToFileByPtcpntIdRequest
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
                    LegacyServiceHeaderInfo = new VIMT.IntentToFileWebService.Messages.HeaderInfo
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

                var intentToFileResponse = intentToFileRequest.SendReceive<VIMTfindInt2FilePrtId_findIntentToFileByPtcpntIdResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call";

                response.ExceptionMessage = intentToFileResponse.ExceptionMessage;
                response.ExceptionOccured = intentToFileResponse.ExceptionOccured;

                var requestCollection = new OrganizationRequestCollection();

                if (intentToFileResponse.VIMTfindInt2FilePrtId_intentToFileDTOInfo != null)
                {
                    var intentToFileintentToFileItem = intentToFileResponse.VIMTfindInt2FilePrtId_intentToFileDTOInfo;

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

                        //
                        //thisNewEntity["va_mailingmilitarypostaltypecode"]
                        //thisNewEntity["va_mailingmilitarypostofficetypecode"]
                        //thisNewEntity["va_militarypostalcodevalue"]
                        //thisNewEntity["va_militarypostaltypecode"]
                        //thisNewEntity["va_militarypostofficetypecodevalue"]
                        //thisNewEntity["va_militaryzipcodelookupvalue"]

                        //thisNewEntity["va_stationlocation"]

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
                            thisNewEntity["va_createddate"] = intentToFileItem.mcs_createDt;
                        }
                        if (intentToFileItem.mcs_rcvdDt != null)
                        {
                            thisNewEntity["va_intenttofiledate"] = intentToFileItem.mcs_rcvdDt;
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
                            thisNewEntity["va_veterandateofbirth"] = intentToFileItem.mcs_vetBrthdyDt;
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

                #region Create records

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

                    intentToFileLoading.Id = intentToFileRequest.RelatedParentId;
                    intentToFileLoading["udo_itfloading"] = new OptionSetValue(752280001);
                    OrgServiceProxy.Update(intentToFileLoading);
                }

                string logInfo = string.Format("Intent To File Records Created: {0}", requestCollection.Count());
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Intent To File Records Created", logInfo);
                #endregion

                //added to generated code
                //if (request.RelatedParentId != System.Guid.Empty)
                //{
                //    var parent = new Entity();
                //    parent.Id = request.RelatedParentId;
                //    parent.LogicalName = request.RelatedParentEntityName;
                //    parent["udo_itfcomplete"] = true;
                //    OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                //}
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOcreateUDOIntentToFileProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to process Intent To File Data";
                response.ExceptionOccured = true;
                return response;
            }
        }
    }
}