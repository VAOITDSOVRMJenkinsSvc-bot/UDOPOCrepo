﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Linq;
using VEIS.AddressWebService.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreateAddressRecords,createAddressRecords method, Processor.
/// Code Generated by IMS on: 5/27/2015 11:21:44 AM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace UDO.Crm.LOB.Processors.Address
{
    using global::UDO.LOB.Core;
    using global::UDO.LOB.Extensions;
    using global::UDO.LOB.Extensions.Logging;
    using UDO.Crm.LOB.Messages.Address;

    class UDOcreateAddressRecordsProcessor
    {
        private const string method = "UDOcreateAddressRecordsProcessor";
        private bool _debug { get; set; }
        private string LogBuffer { get; set; }
        private Entity vetSnapShot = new Entity("udo_veteransnapshot");
        private OrganizationRequestCollection requestCollection = new OrganizationRequestCollection();
        private string mailingaddress = string.Empty;

        public IMessageBase Execute(UDOcreateAddressRecordsRequest request)
        {
            UDOcreateAddressRecordsResponse response = new UDOcreateAddressRecordsResponse();
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
                CrmConnection connection = new CrmConnection();
                if (request.UserId != Guid.Empty)
                {
                    connection.OrgServiceProxy.CallerId = request.UserId;
                }
                OrgServiceProxy = (OrganizationServiceProxy)connection.Connect();
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateAwardsProcessor Processor, Connection Error", connectException.Message);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }

            #endregion

            progressString = "After Connection, deleting data";


            requestCollection.Clear();

            try
            {

                if (request.ptcpntId > 0)
                {
                    var findAllPtcpntAddrsByPtcpntIdResponse = FindAddressByPtcpntId(request);

                    response.ExceptionMessage = findAllPtcpntAddrsByPtcpntIdResponse.ExceptionMessage;
                    response.ExceptionOccured = findAllPtcpntAddrsByPtcpntIdResponse.ExceptionOccurred;

                    if (findAllPtcpntAddrsByPtcpntIdResponse.VEISfallpidaddpidfreturnInfo != null)
                    {
                        progressString = "Starting on mapping";
                        MapFindAddressResponse(findAllPtcpntAddrsByPtcpntIdResponse.VEISfallpidaddpidfreturnInfo, request);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.fileNumber))
                    {
                        var findAllPtcpntAddrsByFNResponse = FindAddressByFileNumber(request);

                        response.ExceptionMessage = findAllPtcpntAddrsByFNResponse.ExceptionMessage;
                        response.ExceptionOccured = findAllPtcpntAddrsByFNResponse.ExceptionOccurred;

                        if (findAllPtcpntAddrsByFNResponse.VEISfallpidaddFNreturnInfo != null)
                        {
                            progressString = "Starting on mapping";
                            MapFindAddressResponse(findAllPtcpntAddrsByFNResponse.VEISfallpidaddFNreturnInfo, request);
                        }
                    }

                }

                if (requestCollection.Count() > 0)
                {
                    var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, request.UserId, request.Debug);

                    if (_debug)
                    {
                        LogBuffer += result.LogDetail;
                        LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer, _debug);
                    }

                    if (result.IsFaulted)
                    {
                        LogHelper.LogError(request.OrganizationName, request.UserId, method, result.ErrorDetail);
                        response.ExceptionMessage = result.FriendlyDetail;
                        response.ExceptionOccured = true;
                        return response;
                    }
                }

                if (request.vetsnapshotId == Guid.Empty)
                {
                    #region Log Results
                    string logInfo = string.Format("Address Records Created: {0}", requestCollection.Count());
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Address Records Created", logInfo);
                    #endregion
                }

                if (request.vetsnapshotId != Guid.Empty)
                {
                    vetSnapShot.Id = request.vetsnapshotId;
                    vetSnapShot["udo_mailingaddress"] = mailingaddress;
                    //RC NEW - added this flag here
                    vetSnapShot["udo_addresscomplete"] = true;
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(vetSnapShot, request.OrganizationName, request.UserId, request.LogTiming, "", OrgServiceProxy));
                }

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOcreateAddressRecordsProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Map EC data to LOB";
                response.ExceptionOccured = true;
                return response;
            }
        }

        private void MapFindAddressResponse(VEISfallpidaddFNreturnMultipleResponse[] ptcpntAddrsDTO, UDOcreateAddressRecordsRequest request)
        {
            foreach (var ptcpntAddrsDTOItem in ptcpntAddrsDTO)
            {
                var thisisMailing = false;
                //instantiate the new Entity
                Entity thisNewEntity = new Entity();
                thisNewEntity.LogicalName = "udo_address";
                if (request.ownerId != System.Guid.Empty)
                {
                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                }
                else
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, "Create Addresses", "No Owner");
                }

                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_ptcpntAddrsTypeNm))
                {
                    thisNewEntity["udo_addresstypestring"] = ptcpntAddrsDTOItem.mcs_ptcpntAddrsTypeNm;
                    if (ptcpntAddrsDTOItem.mcs_ptcpntAddrsTypeNm.Equals("mailing", StringComparison.InvariantCultureIgnoreCase))
                    {
                        thisisMailing = true;
                    }
                }
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_addrsOneTxt))
                {
                    thisNewEntity["udo_addressline1"] = ptcpntAddrsDTOItem.mcs_addrsOneTxt;

                    if (thisisMailing)
                    {
                        vetSnapShot["udo_address1"] = ptcpntAddrsDTOItem.mcs_addrsOneTxt;
                    }
                }
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_addrsTwoTxt))
                {
                    thisNewEntity["udo_addressline2"] = ptcpntAddrsDTOItem.mcs_addrsTwoTxt;
                    if (thisisMailing)
                    {
                        vetSnapShot["udo_address2"] = ptcpntAddrsDTOItem.mcs_addrsTwoTxt;
                    }
                }
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_addrsThreeTxt))
                {
                    thisNewEntity["udo_addressline3"] = ptcpntAddrsDTOItem.mcs_addrsThreeTxt;
                    vetSnapShot["udo_address3"] = ptcpntAddrsDTOItem.mcs_addrsThreeTxt;
                }

                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_cityNm))
                {
                    thisNewEntity["udo_city"] = ptcpntAddrsDTOItem.mcs_cityNm;
                    if (thisisMailing)
                    {
                        vetSnapShot["udo_mailingcity"] = ptcpntAddrsDTOItem.mcs_cityNm;
                    }
                }
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_postalCd))
                {
                    thisNewEntity["udo_state"] = ptcpntAddrsDTOItem.mcs_postalCd;
                    if (thisisMailing)
                    {
                        vetSnapShot["udo_mailingstate"] = ptcpntAddrsDTOItem.mcs_postalCd;
                    }
                }

                // 09-27-2016 - Added military city/state to vetsnapshot and mailing info
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd))
                {
                    if (thisisMailing)
                    {
                        vetSnapShot["udo_mailingcity"] = ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd;
                    }
                }
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd))
                {
                    if (thisisMailing)
                    {
                        vetSnapShot["udo_mailingstate"] = ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd;
                    }
                }

                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_cntryNm))
                {
                    thisNewEntity["udo_country"] = ptcpntAddrsDTOItem.mcs_cntryNm;
                    if (thisisMailing)
                    {
                        vetSnapShot["udo_mailingcountry"] = ptcpntAddrsDTOItem.mcs_cntryNm;

                    }
                }

                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_zipPrefixNbr))
                {
                    thisNewEntity["udo_zip"] = ptcpntAddrsDTOItem.mcs_zipPrefixNbr;
                    if (thisisMailing)
                    {
                        vetSnapShot["udo_mailingzip"] = ptcpntAddrsDTOItem.mcs_zipPrefixNbr;
                    }
                }

                //Build Mailing address string
                if (thisisMailing)
                    mailingaddress = string.Format("{0} {1} {2} {3} {4}",
                                            vetSnapShot.GetAttributeValue<string>("udo_address1"),
                                            vetSnapShot.GetAttributeValue<string>("udo_address2"),
                                            vetSnapShot.GetAttributeValue<string>("udo_mailingcity"),
                                            vetSnapShot.GetAttributeValue<string>("udo_mailingstate"),
                                            vetSnapShot.GetAttributeValue<string>("udo_mailingzip"));

                if (ptcpntAddrsDTOItem.mcs_efctvDt != System.DateTime.MinValue)
                {
                    thisNewEntity["udo_effectivedate"] = ptcpntAddrsDTOItem.mcs_efctvDt;
                }
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_emailAddrsTxt))
                {
                    thisNewEntity["udo_emailaddress"] = ptcpntAddrsDTOItem.mcs_emailAddrsTxt;
                    vetSnapShot["udo_email"] = ptcpntAddrsDTOItem.mcs_emailAddrsTxt;
                }

                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_frgnPostalCd))
                {
                    thisNewEntity["udo_foreignpostalcode"] = ptcpntAddrsDTOItem.mcs_frgnPostalCd;
                }
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_group1VerifdTypeCd))
                {
                    //Valide N is correct for this IND field
                    var thisValue = ptcpntAddrsDTOItem.mcs_group1VerifdTypeCd;
                    if (thisValue == "N")
                    {
                        thisNewEntity["udo_group1verified"] = false;
                    }
                    else
                    {
                        thisNewEntity["udo_group1verified"] = true;
                    }
                }
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd))
                {
                    thisNewEntity["udo_milpostal"] = ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd;
                }
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd))
                {
                    thisNewEntity["udo_milpotype"] = ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd;
                }
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_prvncNm))
                {
                    thisNewEntity["udo_province"] = ptcpntAddrsDTOItem.mcs_prvncNm;
                }
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_sharedAddrsInd))
                {
                    //Valide N is correct for this IND field
                    var thisValue = ptcpntAddrsDTOItem.mcs_sharedAddrsInd;
                    if (thisValue == "N")
                    {
                        thisNewEntity["udo_sharedaddressind"] = false;
                    }
                    else
                    {
                        thisNewEntity["udo_sharedaddressind"] = true;
                    }
                }

                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_trtryNm))
                {
                    thisNewEntity["udo_territory"] = ptcpntAddrsDTOItem.mcs_trtryNm;
                }
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_trsuryAddrsOneTxt))
                {
                    thisNewEntity["udo_treasury1"] = ptcpntAddrsDTOItem.mcs_trsuryAddrsOneTxt;
                }
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_trsuryAddrsTwoTxt))
                {
                    thisNewEntity["udo_treasury2"] = ptcpntAddrsDTOItem.mcs_trsuryAddrsTwoTxt;
                }
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_trsuryAddrsThreeTxt))
                {
                    thisNewEntity["udo_treasury3"] = ptcpntAddrsDTOItem.mcs_trsuryAddrsThreeTxt;
                }
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_trsuryAddrsFourTxt))
                {
                    thisNewEntity["udo_treasury4"] = ptcpntAddrsDTOItem.mcs_trsuryAddrsFourTxt;
                }
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_trsuryAddrsFiveTxt))
                {
                    thisNewEntity["udo_treasury5"] = ptcpntAddrsDTOItem.mcs_trsuryAddrsFiveTxt;
                }
                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_trsuryAddrsSixTxt))
                {
                    thisNewEntity["udo_treasury6"] = ptcpntAddrsDTOItem.mcs_trsuryAddrsSixTxt;
                }


                if (request.UDOcreateAddressRecordsRelatedEntitiesInfo != null)
                {
                    foreach (var relatedItem in request.UDOcreateAddressRecordsRelatedEntitiesInfo)
                    {
                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                    }
                }
                if (request.vetsnapshotId == Guid.Empty)
                {
                    CreateRequest createData = new CreateRequest
                    {
                        Target = thisNewEntity
                    };
                    requestCollection.Add(createData);
                }
            }
        }

        private void MapFindAddressResponse(VEISfallpidaddpidfreturnMultipleResponse[] ptcpntAddrsDTO, UDOcreateAddressRecordsRequest request)
        {
            foreach (var ptcpntAddrsDTOItem in ptcpntAddrsDTO)
            {
                var thisisMailing = false;
                
                //instantiate the new Entity
                Entity thisNewEntity = new Entity();
                thisNewEntity.LogicalName = "udo_address";
                if (request.ownerId != System.Guid.Empty)
                {
                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                }
                else
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, "Create Addresses", "No Owner");
                }

                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_ptcpntAddrsTypeNm))
                {
                    thisNewEntity["udo_addresstypestring"] = ptcpntAddrsDTOItem.mcs_ptcpntAddrsTypeNm;
                    if (ptcpntAddrsDTOItem.mcs_ptcpntAddrsTypeNm.Equals("mailing", StringComparison.InvariantCultureIgnoreCase))
                    {
                        thisisMailing = true;
                    }
                }
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_addrsOneTxt))
                {
                    thisNewEntity["udo_addressline1"] = ptcpntAddrsDTOItem.mcs_addrsOneTxt;

                    if (thisisMailing)
                    {
                        vetSnapShot["udo_address1"] = ptcpntAddrsDTOItem.mcs_addrsOneTxt;
                    }
                }
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_addrsTwoTxt))
                {
                    thisNewEntity["udo_addressline2"] = ptcpntAddrsDTOItem.mcs_addrsTwoTxt;
                    if (thisisMailing)
                    {
                        vetSnapShot["udo_address2"] = ptcpntAddrsDTOItem.mcs_addrsTwoTxt;
                    }
                }
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_addrsThreeTxt))
                {
                    thisNewEntity["udo_addressline3"] = ptcpntAddrsDTOItem.mcs_addrsThreeTxt;
                    vetSnapShot["udo_address3"] = ptcpntAddrsDTOItem.mcs_addrsThreeTxt;
                }

                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_cityNm))
                {
                    thisNewEntity["udo_city"] = ptcpntAddrsDTOItem.mcs_cityNm;
                    if (thisisMailing)
                    {
                        vetSnapShot["udo_mailingcity"] = ptcpntAddrsDTOItem.mcs_cityNm;
                    }
                }
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_postalCd))
                {
                    thisNewEntity["udo_state"] = ptcpntAddrsDTOItem.mcs_postalCd;
                    if (thisisMailing)
                    {
                        vetSnapShot["udo_mailingstate"] = ptcpntAddrsDTOItem.mcs_postalCd;
                    }
                }

                // 09-27-2016 - Added military city/state to vetsnapshot and mailing info
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd))
                {
                    if (thisisMailing)
                    {
                        vetSnapShot["udo_mailingcity"] = ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd;
                    }
                }
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd))
                {
                    if (thisisMailing)
                    {
                        vetSnapShot["udo_mailingstate"] = ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd;
                    }
                }

                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_cntryNm))
                {
                    thisNewEntity["udo_country"] = ptcpntAddrsDTOItem.mcs_cntryNm;
                    if (thisisMailing)
                    {
                        vetSnapShot["udo_mailingcountry"] = ptcpntAddrsDTOItem.mcs_cntryNm;
                    }
                }

                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_zipPrefixNbr))
                {
                    thisNewEntity["udo_zip"] = ptcpntAddrsDTOItem.mcs_zipPrefixNbr;
                    if (thisisMailing)
                    {
                        vetSnapShot["udo_mailingzip"] = ptcpntAddrsDTOItem.mcs_zipPrefixNbr;
                    }
                }

                ///Build Mailing address string
                if (thisisMailing)
                    mailingaddress = string.Format("{0} {1} {2} {3} {4}",
                                            vetSnapShot.GetAttributeValue<string>("udo_address1"),
                                            vetSnapShot.GetAttributeValue<string>("udo_address2"),
                                            vetSnapShot.GetAttributeValue<string>("udo_mailingcity"),
                                            vetSnapShot.GetAttributeValue<string>("udo_mailingstate"),
                                            vetSnapShot.GetAttributeValue<string>("udo_mailingzip"));

                if (ptcpntAddrsDTOItem.mcs_efctvDt != System.DateTime.MinValue)
                {
                    thisNewEntity["udo_effectivedate"] = ptcpntAddrsDTOItem.mcs_efctvDt;
                }
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_emailAddrsTxt))
                {
                    thisNewEntity["udo_emailaddress"] = ptcpntAddrsDTOItem.mcs_emailAddrsTxt;
                    vetSnapShot["udo_email"] = ptcpntAddrsDTOItem.mcs_emailAddrsTxt;
                }

                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_frgnPostalCd))
                {
                    thisNewEntity["udo_foreignpostalcode"] = ptcpntAddrsDTOItem.mcs_frgnPostalCd;
                }
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_group1VerifdTypeCd))
                {
                    //Valide N is correct for this IND field
                    var thisValue = ptcpntAddrsDTOItem.mcs_group1VerifdTypeCd;
                    if (thisValue == "N")
                    {
                        thisNewEntity["udo_group1verified"] = false;
                    }
                    else
                    {
                        thisNewEntity["udo_group1verified"] = true;
                    }
                }
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd))
                {
                    thisNewEntity["udo_milpostal"] = ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd;
                }
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd))
                {
                    thisNewEntity["udo_milpotype"] = ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd;
                }
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_prvncNm))
                {
                    thisNewEntity["udo_province"] = ptcpntAddrsDTOItem.mcs_prvncNm;
                }
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_sharedAddrsInd))
                {
                    //Valide N is correct for this IND field
                    var thisValue = ptcpntAddrsDTOItem.mcs_sharedAddrsInd;
                    if (thisValue == "N")
                    {
                        thisNewEntity["udo_sharedaddressind"] = false;
                    }
                    else
                    {
                        thisNewEntity["udo_sharedaddressind"] = true;
                    }
                }

                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_trtryNm))
                {
                    thisNewEntity["udo_territory"] = ptcpntAddrsDTOItem.mcs_trtryNm;
                }
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_trsuryAddrsOneTxt))
                {
                    thisNewEntity["udo_treasury1"] = ptcpntAddrsDTOItem.mcs_trsuryAddrsOneTxt;
                }
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_trsuryAddrsTwoTxt))
                {
                    thisNewEntity["udo_treasury2"] = ptcpntAddrsDTOItem.mcs_trsuryAddrsTwoTxt;
                }
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_trsuryAddrsThreeTxt))
                {
                    thisNewEntity["udo_treasury3"] = ptcpntAddrsDTOItem.mcs_trsuryAddrsThreeTxt;
                }
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_trsuryAddrsFourTxt))
                {
                    thisNewEntity["udo_treasury4"] = ptcpntAddrsDTOItem.mcs_trsuryAddrsFourTxt;
                }
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_trsuryAddrsFiveTxt))
                {
                    thisNewEntity["udo_treasury5"] = ptcpntAddrsDTOItem.mcs_trsuryAddrsFiveTxt;
                }
                if (!String.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_trsuryAddrsSixTxt))
                {
                    thisNewEntity["udo_treasury6"] = ptcpntAddrsDTOItem.mcs_trsuryAddrsSixTxt;
                }
                if (request.UDOcreateAddressRecordsRelatedEntitiesInfo != null)
                {
                    foreach (var relatedItem in request.UDOcreateAddressRecordsRelatedEntitiesInfo)
                    {
                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                    }
                }
                if (request.vetsnapshotId == Guid.Empty)
                {
                    CreateRequest createData = new CreateRequest
                    {
                        Target = thisNewEntity
                    };
                    requestCollection.Add(createData);
                }
            }
        }

        private VEISfallpidaddpidffindAllPtcpntAddrsByPtcpntIdResponse FindAddressByPtcpntId(UDOcreateAddressRecordsRequest request)
        {
            var findAllPtcpntAddrsByPtcpntIdRequest = new VEISfallpidaddpidffindAllPtcpntAddrsByPtcpntIdRequest();

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
            
            return WebApiUtility.SendReceive<VEISfallpidaddpidffindAllPtcpntAddrsByPtcpntIdResponse>(findAllPtcpntAddrsByPtcpntIdRequest, WebApiType.VEIS);
        }

        private VEISfallpidaddFNfindAllPtcpntAddrsByFileNumberResponse FindAddressByFileNumber(UDOcreateAddressRecordsRequest request)
        {
            var findAllPtcpntAddrsByPtcpntIdRequest = new VEISfallpidaddFNfindAllPtcpntAddrsByFileNumberRequest();

            findAllPtcpntAddrsByPtcpntIdRequest.LogTiming = request.LogTiming;
            findAllPtcpntAddrsByPtcpntIdRequest.LogSoap = request.LogSoap;
            findAllPtcpntAddrsByPtcpntIdRequest.Debug = request.Debug;
            findAllPtcpntAddrsByPtcpntIdRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            findAllPtcpntAddrsByPtcpntIdRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            findAllPtcpntAddrsByPtcpntIdRequest.RelatedParentId = request.RelatedParentId;
            findAllPtcpntAddrsByPtcpntIdRequest.UserId = request.UserId;
            findAllPtcpntAddrsByPtcpntIdRequest.OrganizationName = request.OrganizationName;

            findAllPtcpntAddrsByPtcpntIdRequest.mcs_filenumber = request.fileNumber;
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

            return WebApiUtility.SendReceive<VEISfallpidaddFNfindAllPtcpntAddrsByFileNumberResponse>(findAllPtcpntAddrsByPtcpntIdRequest, WebApiType.VEIS);
        }
    }
}