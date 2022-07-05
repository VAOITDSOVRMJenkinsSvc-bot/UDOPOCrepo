﻿using global::UDO.LOB.Core;
using global::UDO.LOB.Extensions;
using global::UDO.LOB.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using UDO.LOB.Contact.Messages;
using VEIS.Messages.AddressWebService;

/// <summary>
/// VIMT LOB Component for UDOcreateAddressRecords,createAddressRecords method, Processor.
/// </summary>
namespace UDO.LOB.Contact.Processors
{
    class UDOgetAddressRecordsProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOgetAddressRecordsProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOgetAddressRecordsRequest request)
        {
            UDOgetAddressRecordsResponse response = new UDOgetAddressRecordsResponse();
            response.MessageId = request.MessageId;

            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

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
                    MessageTrigger = method,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }

            #region connect to CRM
            CrmServiceClient OrgServiceProxy = null;
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            using (OrgServiceProxy)
            {
                progressString = "After Connection, deleting data";

                if (request.udo_personid == Guid.Empty)
                {
                    throw new ArgumentException(this.GetType().Name + " is missing Person ID (udo_personid) value");
                }

                try
                {
                    Entity person = new Entity();
                    person.LogicalName = "udo_person";
                    person.Id = request.udo_personid;
                    person["udo_address1"] = string.Empty;
                    person["udo_address2"] = string.Empty;
                    person["udo_address3"] = string.Empty;
                    person["udo_city"] = string.Empty;
                    person["udo_state"] = string.Empty;
                    person["udo_country"] = string.Empty;
                    person["udo_zip"] = string.Empty;
                    var findAllPtcpntAddrsByPtcpntIdRequest = new VEISfallpidaddpidfindAllPtcpntAddrsByPtcpntIdRequest();
                    findAllPtcpntAddrsByPtcpntIdRequest.MessageId = request.MessageId;
                    findAllPtcpntAddrsByPtcpntIdRequest.LogTiming = request.LogTiming;
                    findAllPtcpntAddrsByPtcpntIdRequest.LogSoap = request.LogSoap;
                    findAllPtcpntAddrsByPtcpntIdRequest.Debug = request.Debug;
                    findAllPtcpntAddrsByPtcpntIdRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                    findAllPtcpntAddrsByPtcpntIdRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                    findAllPtcpntAddrsByPtcpntIdRequest.RelatedParentId = request.RelatedParentId;
                    findAllPtcpntAddrsByPtcpntIdRequest.UserId = request.UserId;
                    findAllPtcpntAddrsByPtcpntIdRequest.OrganizationName = request.OrganizationName;

                    findAllPtcpntAddrsByPtcpntIdRequest.mcs_ptcpntid = request.ptcpntId;
                    if (request.LegacyServiceHeaderInfo != null)
                    {
                        findAllPtcpntAddrsByPtcpntIdRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                    }

                    var findAllPtcpntAddrsByPtcpntIdResponse = WebApiUtility.SendReceive<VEISfallpidaddpidfindAllPtcpntAddrsByPtcpntIdResponse>(findAllPtcpntAddrsByPtcpntIdRequest, WebApiType.VEIS);
                    if (request.LogSoap || findAllPtcpntAddrsByPtcpntIdResponse.ExceptionOccurred)
                    {
                        if (findAllPtcpntAddrsByPtcpntIdResponse.SerializedSOAPRequest != null || findAllPtcpntAddrsByPtcpntIdResponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = findAllPtcpntAddrsByPtcpntIdResponse.SerializedSOAPRequest + findAllPtcpntAddrsByPtcpntIdResponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfallpidaddpidfindAllPtcpntAddrsByPtcpntIdRequest Request/Response {requestResponse}", true);
                        }
                    }

                    progressString = "After VIMT EC Call";

                    response.ExceptionMessage = findAllPtcpntAddrsByPtcpntIdResponse.ExceptionMessage;
                    response.ExceptionOccured = findAllPtcpntAddrsByPtcpntIdResponse.ExceptionOccurred;

                    var mailingaddress = "";
                    if (findAllPtcpntAddrsByPtcpntIdResponse.VEISfallpidaddpidfreturnInfo != null)
                    {
                        var ptcpntAddrsDTO = findAllPtcpntAddrsByPtcpntIdResponse.VEISfallpidaddpidfreturnInfo;
                        System.Collections.Generic.List<UDOcreateAddressRecordsMultipleResponse> UDOcreateAddressRecordsArray = new System.Collections.Generic.List<UDOcreateAddressRecordsMultipleResponse>();
                        foreach (var ptcpntAddrsDTOItem in ptcpntAddrsDTO)
                        {
                            var thisisMailing = false;
                            var responseIds = new UDOcreateAddressRecordsMultipleResponse();

                            if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_ptcpntAddrsTypeNm))
                            {
                                if (ptcpntAddrsDTOItem.mcs_ptcpntAddrsTypeNm.Equals("mailing", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    thisisMailing = true;
                                }
                            }

                            if (thisisMailing)
                            {
                                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_addrsOneTxt))
                                {
                                    mailingaddress = ptcpntAddrsDTOItem.mcs_addrsOneTxt;
                                    person["udo_address1"] = ptcpntAddrsDTOItem.mcs_addrsOneTxt;
                                }

                                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_addrsTwoTxt))
                                {
                                    mailingaddress += " " + ptcpntAddrsDTOItem.mcs_addrsTwoTxt;
                                    person["udo_address2"] = ptcpntAddrsDTOItem.mcs_addrsTwoTxt;
                                }

                                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_addrsThreeTxt))
                                {
                                    person["udo_address3"] = ptcpntAddrsDTOItem.mcs_addrsThreeTxt;
                                }

                                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_cityNm))
                                {
                                    mailingaddress += " " + ptcpntAddrsDTOItem.mcs_cityNm;
                                    person["udo_city"] = ptcpntAddrsDTOItem.mcs_cityNm;
                                }

                                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd))
                                {
                                    mailingaddress += " " + ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd;
                                    person["udo_city"] = ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd;
                                }

                                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_postalCd))
                                {
                                    mailingaddress += " " + ptcpntAddrsDTOItem.mcs_postalCd;
                                    person["udo_state"] = ptcpntAddrsDTOItem.mcs_postalCd;
                                }

                                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd))
                                {
                                    mailingaddress += " " + ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd;
                                    person["udo_state"] = ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd;
                                }

                                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_cntryNm))
                                {
                                    mailingaddress += ", " + ptcpntAddrsDTOItem.mcs_countyNm;
                                    person["udo_country"] = ptcpntAddrsDTOItem.mcs_cntryNm;
                                }

                                if (ptcpntAddrsDTOItem.mcs_emailAddrsTxt != string.Empty)
                                {
                                    person["udo_email"] = ptcpntAddrsDTOItem.mcs_emailAddrsTxt;
                                }

                                if (ptcpntAddrsDTOItem.mcs_zipPrefixNbr != string.Empty)
                                {
                                    person["udo_zip"] = ptcpntAddrsDTOItem.mcs_zipPrefixNbr;
                                }

                            }
                        }

                        OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, person, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));

                        string logInfo = "Person Record Updated";
                        LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, logInfo, logInfo);
                    }

                    return response;
                }
                catch (Exception ExecutionException)
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOgetAddressRecordsProcessor Processor, Progess:" + progressString, ExecutionException);
                    response.ExceptionMessage = "Failed to Map EC data to LOB";
                    response.ExceptionOccured = true;
                    return response;
                }
            }
        }
    }
}