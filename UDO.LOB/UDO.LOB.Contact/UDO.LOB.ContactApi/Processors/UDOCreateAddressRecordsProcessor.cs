using global::UDO.LOB.Contact.Messages;
using global::UDO.LOB.Core;
using global::UDO.LOB.Extensions;
using global::UDO.LOB.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;
using VEIS.Core.Messages;
using VEIS.Messages.AddressWebService;

/// <summary>
/// LOB Component for UDOcreateAddressRecords,createAddressRecords method, Processor.
/// </summary>
namespace UDO.LOB.Contact.Processors
{
    class UDOcreateAddressRecordsProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOcreateAddressRecordsProcessor";
        private string LogBuffer { get; set; }
        private Entity vetSnapShot = new Entity("udo_veteransnapshot");
        private OrganizationRequestCollection requestCollection = new OrganizationRequestCollection();
        private string mailingaddress = string.Empty;

        public IMessageBase Execute(UDOcreateAddressRecordsRequest request)
        {
            // LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, ">> Entered UDOcreateAddressRecordsProcessor", request.Debug);
            UDOcreateAddressRecordsResponse response = new UDOcreateAddressRecordsResponse { MessageId = request.MessageId };

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
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Invoking ExecuteMultipleHelper.ExecuteMultiple", request.Debug);
                    var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, $"Invoked ExecuteMultipleHelper.ExecuteMultiple: Result.IsFaulted?: {result?.IsFaulted}", request.Debug);
                    
                    if (_debug)
                    {
                        LogBuffer += result.LogDetail;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, LogBuffer, _debug);
                    }

                    if (result.IsFaulted)
                    {
                        LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, result.ErrorDetail);
                        response.ExceptionMessage = result.FriendlyDetail;
                        response.ExceptionOccured = true;
                        return response;
                    }
                }

                if (request.vetsnapshotId == Guid.Empty)
                {
                    #region Log Results
                    string logInfo = string.Format("Address Records Created: {0}", requestCollection.Count());
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, method, logInfo);
                    #endregion
                }

                if (request.vetsnapshotId != Guid.Empty)
                {
                    vetSnapShot.Id = request.vetsnapshotId;
                    vetSnapShot["udo_mailingaddress"] = mailingaddress;
                    //RC NEW - added this flag here
                    vetSnapShot["udo_addresscomplete"] = true;
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, vetSnapShot, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));

                }

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOcreateAddressRecordsProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = $"{method}: Failed to Map EC data to LOB";
                response.ExceptionOccured = true;
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

                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_efctvDt))
                {
                    var effDate = new DateTime();
                    if (DateTime.TryParse(ptcpntAddrsDTOItem.mcs_efctvDt, out effDate))
                    {
                        thisNewEntity["udo_effectivedate"] = effDate;
                    }
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

                if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_efctvDt) && DateTime.TryParse(ptcpntAddrsDTOItem.mcs_efctvDt, out DateTime effDate))
                {
                    thisNewEntity["udo_effectivedate"] = effDate; 
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

        private VEISfallpidaddpidfindAllPtcpntAddrsByPtcpntIdResponse FindAddressByPtcpntId(UDOcreateAddressRecordsRequest request)
        {
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
                findAllPtcpntAddrsByPtcpntIdRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber

                };
            }

            var response = WebApiUtility.SendReceive<VEISfallpidaddpidfindAllPtcpntAddrsByPtcpntIdResponse>(findAllPtcpntAddrsByPtcpntIdRequest, WebApiType.VEIS);
            if (request.LogSoap || response.ExceptionOccurred)
            {
                if (response.SerializedSOAPRequest != null || response.SerializedSOAPResponse != null)
                {
                    var requestResponse = response.SerializedSOAPRequest + response.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfallpidaddpidfindAllPtcpntAddrsByPtcpntIdRequest Request/Response {requestResponse}", true);
                }
            }
            return response;
        }

        private VEISfallpidaddFNfindAllPtcpntAddrsByFileNumberResponse FindAddressByFileNumber(UDOcreateAddressRecordsRequest request)
        {
            var findAllPtcpntAddrsByPtcpntIdRequest = new VEISfallpidaddFNfindAllPtcpntAddrsByFileNumberRequest();
            findAllPtcpntAddrsByPtcpntIdRequest.MessageId = request.MessageId;
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
                findAllPtcpntAddrsByPtcpntIdRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber

                };
            }

            var response = WebApiUtility.SendReceive<VEISfallpidaddFNfindAllPtcpntAddrsByFileNumberResponse>(findAllPtcpntAddrsByPtcpntIdRequest, WebApiType.VEIS);
            if (request.LogSoap || response.ExceptionOccurred)
            {
                if (response.SerializedSOAPRequest != null || response.SerializedSOAPResponse != null)
                {
                    var requestResponse = response.SerializedSOAPRequest + response.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfallpidaddFNfindAllPtcpntAddrsByFileNumberRequest Request/Response {requestResponse}", true);
                }
            }

            return response;
        }
    }
}