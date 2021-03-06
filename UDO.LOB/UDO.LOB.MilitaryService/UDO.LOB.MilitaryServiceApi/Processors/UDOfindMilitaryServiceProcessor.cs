using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Specialized;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Configuration;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MilitaryService.Messages;
using VEIS.Core.Messages;
using VEIS.Messages.ClaimantService;

namespace UDO.LOB.MilitaryService.Processors
{
    class UDOfindMilitaryServiceProcessor
    {

        private CrmServiceClient OrgServiceProxy;
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOfindMilitaryServiceProcessor";
        
        public IMessageBase Execute(UDOfindMilitaryServiceRequest request)
        {
            //var request = message as UDOcreateSMCRatingsRequest;
            UDOfindMilitaryServiceResponse response = new UDOfindMilitaryServiceResponse { MessageId = request?.MessageId };
            TraceLogger tLogger = null;
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
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

            tLogger = new TraceLogger(method, request);

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
            tLogger.LogEvent("Connected to CRM", "001");
            progressString = "After Connection";

            try
            {
                // prefix = fnrtngdtfindRatingDataRequest();
                var findMilitaryServiceDataRequest = new VEISfmirecpicfindMilitaryRecordByPtcpntIdRequest();
                findMilitaryServiceDataRequest.MessageId = request.MessageId;
                findMilitaryServiceDataRequest.LogTiming = request.LogTiming;
                findMilitaryServiceDataRequest.LogSoap = request.LogSoap;
                findMilitaryServiceDataRequest.Debug = request.Debug;
                findMilitaryServiceDataRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findMilitaryServiceDataRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findMilitaryServiceDataRequest.RelatedParentId = request.RelatedParentId;
                findMilitaryServiceDataRequest.UserId = request.UserId;
                findMilitaryServiceDataRequest.OrganizationName = request.OrganizationName;
                findMilitaryServiceDataRequest.mcs_ptcpntid = request.ptcpntId;
                findMilitaryServiceDataRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
                progressString = "Request PTCPNT id is: " + findMilitaryServiceDataRequest.mcs_ptcpntid + ", Request Org is: " + findMilitaryServiceDataRequest.OrganizationName + ", Request User ID is: " + findMilitaryServiceDataRequest.UserId;

                // REM: findMiliatryServiceDataRequest.SendReceive<VEISfmirecpicfindMilitaryRecordByPtcpntIdResponse>(MessageProcessType.Local)
                var findMilitaryServiceDataResponse = WebApiUtility.SendReceive<VEISfmirecpicfindMilitaryRecordByPtcpntIdResponse>(findMilitaryServiceDataRequest, WebApiType.VEIS);

                if (request.LogSoap || findMilitaryServiceDataResponse.ExceptionOccurred)
                {
                    if (findMilitaryServiceDataResponse.SerializedSOAPRequest != null || findMilitaryServiceDataResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findMilitaryServiceDataResponse.SerializedSOAPRequest + findMilitaryServiceDataResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfmirecpicfindMilitaryRecordByPtcpntIdRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VEIS EC Call";
                tLogger.LogEvent("Web Service Call VEISfmirecpicfindMilitaryRecordByPtcpntIdResponse", "002");
                response.ExceptionMessage = findMilitaryServiceDataResponse.ExceptionMessage;
                response.ExceptionOccurred = findMilitaryServiceDataResponse.ExceptionOccurred;

                progressString = "Beginning Creation of Child Records";

                var requestCollection = new OrganizationRequestCollection();
                var miltheatreCount = 0;
                var decorationCount = 0;
                var POWCount = 0;
                var retPayCount = 0;
                var sevPayCount = 0;
                var sevBalCount = 0;
                var sepBalCount = 0;
                var sepPayCount = 0;
                var readBalCount = 0;
                var readPayCount = 0;
                var milPersonCount = 0;
                var tourHistCount = 0;

                if (findMilitaryServiceDataResponse != null)
                {
                    response.ExceptionMessage = findMilitaryServiceDataResponse.ExceptionMessage;
                    response.ExceptionOccurred = findMilitaryServiceDataResponse.ExceptionOccurred;

                    if (findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo != null)
                    {

                        #region Military Theater
                        if (findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryTheatresInfo != null)
                        {

                            var shrinq1By2MilitaryTheatre = findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryTheatresInfo;
                            foreach (var shrinq1By2MilitaryTheatreItem in shrinq1By2MilitaryTheatre)
                            {
                                //instantiate the new Entity
                                Entity thisNewEntity = new Entity("udo_militarytheater");
                                thisNewEntity["udo_name"] = "Military Theater";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryTheatreItem.mcs_beginDate))
                                {
                                    thisNewEntity["udo_begindate"] = dateStringFormat(shrinq1By2MilitaryTheatreItem.mcs_beginDate);
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryTheatreItem.mcs_days))
                                {
                                    thisNewEntity["udo_days"] = shrinq1By2MilitaryTheatreItem.mcs_days;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryTheatreItem.mcs_ptcpntId))
                                {
                                    thisNewEntity["udo_participantid"] = shrinq1By2MilitaryTheatreItem.mcs_ptcpntId;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryTheatreItem.mcs_militaryTheatreTypeName))
                                {
                                    thisNewEntity["udo_theatertype"] = shrinq1By2MilitaryTheatreItem.mcs_militaryTheatreTypeName;
                                    thisNewEntity["udo_name"] = shrinq1By2MilitaryTheatreItem.mcs_militaryTheatreTypeName;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryTheatreItem.mcs_militaryPersonTourNbr))
                                {
                                    thisNewEntity["udo_tournumber"] = shrinq1By2MilitaryTheatreItem.mcs_militaryPersonTourNbr;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryTheatreItem.mcs_verifiedInd))
                                {
                                    thisNewEntity["udo_verified"] = shrinq1By2MilitaryTheatreItem.mcs_verifiedInd;
                                }
                                if (request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                }
                                CreateRequest createMilitaryTheater = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createMilitaryTheater);
                                miltheatreCount += 1;
                            }
                        }
                        #endregion

                        #region Decoration

                        if (findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryPersonDecorationsInfo != null)
                        {
                            var shrinq1By2MilitaryPersonDecoration = findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryPersonDecorationsInfo;
                            foreach (var shrinq1By2MilitaryPersonDecorationItem in shrinq1By2MilitaryPersonDecoration)
                            {
                                Entity thisNewEntity = new Entity("udo_decoration");
                                thisNewEntity["udo_name"] = "Decoration";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonDecorationItem.mcs_militaryDecorationId))
                                {
                                    thisNewEntity["udo_decorationidentifier"] = shrinq1By2MilitaryPersonDecorationItem.mcs_militaryDecorationId;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonDecorationItem.mcs_militaryDecorationName))
                                {
                                    thisNewEntity["udo_name"] = shrinq1By2MilitaryPersonDecorationItem.mcs_militaryDecorationName;
                                    thisNewEntity["udo_decorationname"] = shrinq1By2MilitaryPersonDecorationItem.mcs_militaryDecorationName;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonDecorationItem.mcs_ptcpntId))
                                {
                                    thisNewEntity["udo_participantid"] = shrinq1By2MilitaryPersonDecorationItem.mcs_ptcpntId;
                                }
                                if (request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                }
                                CreateRequest createDecoration = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createDecoration);
                                decorationCount += 1;
                            }
                        }
                        #endregion

                        #region POW Information
                        if (findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryPersonPowsInfo != null)
                        {
                            var shrinq1By2MilitaryPersonPow = findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryPersonPowsInfo;
                            foreach (var shrinq1By2MilitaryPersonPowItem in shrinq1By2MilitaryPersonPow)
                            {
                                //instantiate the new Entity
                                Entity thisNewEntity = new Entity("udo_powinformation");
                                thisNewEntity["udo_name"] = "POW Information";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }

                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonPowItem.mcs_campSectorTxt))
                                {
                                    thisNewEntity["udo_campsector"] = shrinq1By2MilitaryPersonPowItem.mcs_campSectorTxt;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonPowItem.mcs_captorTxt))
                                {
                                    thisNewEntity["udo_captor"] = shrinq1By2MilitaryPersonPowItem.mcs_captorTxt;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonPowItem.mcs_captureDate))
                                {
                                    thisNewEntity["udo_capturedate"] = dateStringFormat(shrinq1By2MilitaryPersonPowItem.mcs_captureDate);
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonPowItem.mcs_days))
                                {
                                    thisNewEntity["udo_days"] = shrinq1By2MilitaryPersonPowItem.mcs_days;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonPowItem.mcs_ptcpntId))
                                {
                                    thisNewEntity["udo_participantid"] = shrinq1By2MilitaryPersonPowItem.mcs_ptcpntId;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonPowItem.mcs_powCountryTypeCd))
                                {
                                    thisNewEntity["udo_powcountry"] = shrinq1By2MilitaryPersonPowItem.mcs_powCountryTypeCd;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonPowItem.mcs_militaryPersonPowSeqNbr))
                                {
                                    thisNewEntity["udo_powseqnum"] = shrinq1By2MilitaryPersonPowItem.mcs_militaryPersonPowSeqNbr;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonPowItem.mcs_releaseDate))
                                {
                                    thisNewEntity["udo_releasedate"] = dateStringFormat(shrinq1By2MilitaryPersonPowItem.mcs_releaseDate);
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonPowItem.mcs_militaryTheatreTypeName))
                                {
                                    thisNewEntity["udo_theater"] = shrinq1By2MilitaryPersonPowItem.mcs_militaryTheatreTypeName;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonPowItem.mcs_underThirtyDaysInd))
                                {
                                    thisNewEntity["udo_under30days"] = shrinq1By2MilitaryPersonPowItem.mcs_underThirtyDaysInd;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonPowItem.mcs_verifiedInd))
                                {
                                    thisNewEntity["udo_verified"] = shrinq1By2MilitaryPersonPowItem.mcs_verifiedInd;
                                }
                                if (request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                }
                                CreateRequest createPOW = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createPOW);
                                POWCount += 1;
                            }
                        }
                        #endregion

                        #region Retirement Pay
                        if (findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryRetirementPaysInfo != null)
                        {
                            var shrinq1By2MilitaryRetirementPay = findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryRetirementPaysInfo;
                            foreach (var shrinq1By2MilitaryRetirementPayItem in shrinq1By2MilitaryRetirementPay)
                            {
                                //instantiate the new Entity
                                Entity thisNewEntity = new Entity("udo_retirementpay");
                                thisNewEntity["udo_name"] = "Retirement Pay";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }
                                if (shrinq1By2MilitaryRetirementPayItem.mcs_retirementPayTypeCd != string.Empty)
                                {
                                    thisNewEntity["udo_paytypecode"] = shrinq1By2MilitaryRetirementPayItem.mcs_retirementPayTypeCd;
                                }
                                if (shrinq1By2MilitaryRetirementPayItem.mcs_retirementWaivedDate != string.Empty)
                                {
                                    thisNewEntity["udo_retirementwaiveddate"] = dateStringFormat(shrinq1By2MilitaryRetirementPayItem.mcs_retirementWaivedDate);
                                }
                                if (shrinq1By2MilitaryRetirementPayItem.mcs_sbpOverpaymentAmount != string.Empty)
                                {
                                    thisNewEntity["udo_sbpoverpaymentamount"] = moneyStringFormat(shrinq1By2MilitaryRetirementPayItem.mcs_sbpOverpaymentAmount);
                                }
                                if (shrinq1By2MilitaryRetirementPayItem.mcs_effectiveDate != string.Empty)
                                {
                                    thisNewEntity["udo_effectivedate"] = dateStringFormat(shrinq1By2MilitaryRetirementPayItem.mcs_effectiveDate);
                                }
                                if (shrinq1By2MilitaryRetirementPayItem.mcs_fullWaiverInd != string.Empty)
                                {
                                    thisNewEntity["udo_fullwaiver"] = shrinq1By2MilitaryRetirementPayItem.mcs_fullWaiverInd;
                                }
                                if (shrinq1By2MilitaryRetirementPayItem.mcs_grossMonthlyAmount != string.Empty)
                                {
                                    thisNewEntity["udo_grossmonthlyamount"] = moneyStringFormat(shrinq1By2MilitaryRetirementPayItem.mcs_grossMonthlyAmount);
                                }
                                if (shrinq1By2MilitaryRetirementPayItem.mcs_lessFedTaxAmount != string.Empty)
                                {
                                    thisNewEntity["udo_lessfederaltax"] = moneyStringFormat(shrinq1By2MilitaryRetirementPayItem.mcs_lessFedTaxAmount);
                                }
                                if (shrinq1By2MilitaryRetirementPayItem.mcs_lineItemNbr != string.Empty)
                                {
                                    thisNewEntity["udo_lineitem"] = shrinq1By2MilitaryRetirementPayItem.mcs_lineItemNbr;
                                }
                                if (shrinq1By2MilitaryRetirementPayItem.mcs_ptcpntId != string.Empty)
                                {
                                    thisNewEntity["udo_participantid"] = shrinq1By2MilitaryRetirementPayItem.mcs_ptcpntId;
                                }
                                if (request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                }
                                CreateRequest createRetirementPay = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createRetirementPay);
                                retPayCount += 1;
                            }
                        }
                        #endregion

                        #region Severance Payment

                        if (findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitarySeverancePaysInfo != null)
                        {
                            var severancePay = findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitarySeverancePaysInfo;
                            foreach (var payment in severancePay)
                            {
                                Entity thisNewEntity = new Entity();
                                thisNewEntity.LogicalName = "udo_severancepayment";
                                thisNewEntity["udo_name"] = "Severance Payment";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }
                                if (!string.IsNullOrEmpty(payment.mcs_grossAmount))
                                {
                                    thisNewEntity["udo_grossamount"] = moneyStringFormat(payment.mcs_grossAmount);
                                }
                                if (!string.IsNullOrEmpty(payment.mcs_lessFedTaxAmount))
                                {
                                    thisNewEntity["udo_lessfederaltax"] = moneyStringFormat(payment.mcs_lessFedTaxAmount);
                                }
                                if (!string.IsNullOrEmpty(payment.mcs_lineItemNbr))
                                {
                                    thisNewEntity["udo_lineitem"] = payment.mcs_lineItemNbr;
                                }
                                if (!string.IsNullOrEmpty(payment.mcs_ptcpntId))
                                {
                                    thisNewEntity["udo_participantid"] = payment.mcs_ptcpntId;
                                }
                                if (!string.IsNullOrEmpty(payment.mcs_disabilityTxt))
                                {
                                    thisNewEntity["udo_disability"] = payment.mcs_disabilityTxt;
                                }
                                if (request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                }
                                CreateRequest createSeverancePay = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createSeverancePay);
                                sevPayCount += 1;
                            }
                        }
                        #endregion

                        #region Severance Balance

                        if (findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitarySeveranceBalancesInfo != null)
                        {
                            var severanceBalance = findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitarySeveranceBalancesInfo;
                            foreach (var balance in severanceBalance)
                            {
                                Entity thisNewEntity = new Entity();
                                thisNewEntity.LogicalName = "udo_severancebalance";
                                thisNewEntity["udo_name"] = "Severance Balance";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }

                                if (!string.IsNullOrEmpty(balance.mcs_currentBalance))
                                {
                                    thisNewEntity["udo_currentbalance"] = moneyStringFormat(balance.mcs_currentBalance);
                                }
                                if (!string.IsNullOrEmpty(balance.mcs_dateOfZeroBalance))
                                {
                                    thisNewEntity["udo_dateofzerobalance"] = dateStringFormat(balance.mcs_dateOfZeroBalance);
                                }
                                if (!string.IsNullOrEmpty(balance.mcs_originalBalance))
                                {
                                    thisNewEntity["udo_originalbalance"] = moneyStringFormat(balance.mcs_originalBalance);
                                }
                                if (request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                }
                                CreateRequest createSeveranceBalance = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createSeveranceBalance);
                                sevBalCount += 1;
                            }
                        }
                        #endregion

                        #region Seperation Balance

                        if (findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitarySeperationBalancesInfo != null)
                        {
                            var seperationBalance = findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitarySeperationBalancesInfo;
                            foreach (var balance in seperationBalance)
                            {
                                Entity thisNewEntity = new Entity();
                                thisNewEntity.LogicalName = "udo_seperationbalance";
                                thisNewEntity["udo_name"] = "Seperation Balance";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }
                                if (!string.IsNullOrEmpty(balance.mcs_currentBalance))
                                {
                                    thisNewEntity["udo_currentbalance"] = moneyStringFormat(balance.mcs_currentBalance);
                                }
                                if (!string.IsNullOrEmpty(balance.mcs_dateOfZeroBalance))
                                {
                                    thisNewEntity["udo_dateofzerobalance"] = dateStringFormat(balance.mcs_dateOfZeroBalance);
                                }
                                if (!string.IsNullOrEmpty(balance.mcs_originalBalance))
                                {
                                    thisNewEntity["udo_originalbalance"] = moneyStringFormat(balance.mcs_originalBalance);
                                }
                                if (request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                }
                                CreateRequest createSeperationBalance = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createSeperationBalance);
                                sepBalCount += 1;
                            }
                        }
                        #endregion

                        #region Seperation Payment

                        if (findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitarySeperationPaysInfo != null)
                        {
                            var seperantionPay = findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitarySeperationPaysInfo;
                            foreach (var payment in seperantionPay)
                            {
                                Entity thisNewEntity = new Entity("udo_seperationpayment");
                                thisNewEntity["udo_name"] = "Separation Payment";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }
                                if (!string.IsNullOrEmpty(payment.mcs_grossAmount))
                                {
                                    thisNewEntity["udo_grossamount"] = moneyStringFormat(payment.mcs_grossAmount);
                                }
                                if (!string.IsNullOrEmpty(payment.mcs_lessFedTaxAmount))
                                {
                                    thisNewEntity["udo_lessfederaltax"] = moneyStringFormat(payment.mcs_lessFedTaxAmount);
                                }
                                if (!string.IsNullOrEmpty(payment.mcs_lineItemNbr))
                                {
                                    thisNewEntity["udo_lineitem"] = payment.mcs_lineItemNbr;
                                }
                                if (!string.IsNullOrEmpty(payment.mcs_ptcpntId))
                                {
                                    thisNewEntity["udo_participantid"] = payment.mcs_ptcpntId;
                                }
                                if (!string.IsNullOrEmpty(payment.mcs_receiptDate))
                                {
                                    thisNewEntity["udo_receiptdate"] = dateStringFormat(payment.mcs_receiptDate);
                                }
                                if (request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                }
                                CreateRequest createSeperationePay = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createSeperationePay);
                                sepPayCount += 1;
                            }
                        }
                        #endregion

                        #region Readjustment Balance

                        if (findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryReadjustmentBalancesInfo != null)
                        {
                            var readjustmentBalance = findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryReadjustmentBalancesInfo;
                            foreach (var balance in readjustmentBalance)
                            {
                                Entity thisNewEntity = new Entity("udo_readjustmentbalance");
                                thisNewEntity["udo_name"] = "Readjustment Balance";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }
                                if (!string.IsNullOrEmpty(balance.mcs_currentBalance))
                                {
                                    thisNewEntity["udo_currentbalance"] = moneyStringFormat(balance.mcs_currentBalance);
                                }
                                if (!string.IsNullOrEmpty(balance.mcs_dateOfZeroBalance))
                                {
                                    thisNewEntity["udo_dateofzerobalance"] = dateStringFormat(balance.mcs_dateOfZeroBalance);
                                }
                                if (!string.IsNullOrEmpty(balance.mcs_originalBalance))
                                {
                                    thisNewEntity["udo_originalbalance"] = moneyStringFormat(balance.mcs_originalBalance);
                                }
                                if (request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                }
                                CreateRequest createReadjustmentBalance = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createReadjustmentBalance);
                                readBalCount += 1;
                            }
                        }
                        #endregion

                        #region Readjustment Payment

                        if (findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryReadjustmentPaysInfo != null)
                        {
                            var readjustmentPay = findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryReadjustmentPaysInfo;
                            foreach (var payment in readjustmentPay)
                            {
                                Entity thisNewEntity = new Entity("udo_readjustmentpayment");
                                thisNewEntity["udo_name"] = "Readjustment Payment";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }
                                if (!string.IsNullOrEmpty(payment.mcs_grossAmount))
                                {
                                    thisNewEntity["udo_grossamount"] = moneyStringFormat(payment.mcs_grossAmount);
                                }
                                if (!string.IsNullOrEmpty(payment.mcs_lessFedTaxAmount))
                                {
                                    thisNewEntity["udo_lessfederaltax"] = moneyStringFormat(payment.mcs_lessFedTaxAmount);
                                }
                                if (!string.IsNullOrEmpty(payment.mcs_lineItemNbr))
                                {
                                    thisNewEntity["udo_lineitem"] = payment.mcs_lineItemNbr;
                                }
                                if (!string.IsNullOrEmpty(payment.mcs_ptcpntId))
                                {
                                    thisNewEntity["udo_participantid"] = payment.mcs_ptcpntId;
                                }
                                if (!string.IsNullOrEmpty(payment.mcs_reasonTxt))
                                {
                                    thisNewEntity["udo_reason"] = payment.mcs_reasonTxt;
                                }
                                if (!string.IsNullOrEmpty(payment.mcs_usCodeReasonTxt))
                                {
                                    thisNewEntity["udo_codereason"] = payment.mcs_usCodeReasonTxt;
                                }
                                if (!string.IsNullOrEmpty(payment.mcs_receiptDate))
                                {
                                    thisNewEntity["udo_receiptdate"] = dateStringFormat(payment.mcs_receiptDate);
                                }
                                if (request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                }
                                CreateRequest createSeperationePay = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createSeperationePay);
                                readPayCount += 1;
                            }
                        }
                        #endregion

                        #region Military Person

                        if (findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryPersonsInfo != null)
                        {
                            var militaryPerson = findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryPersonsInfo;
                            foreach (var person in militaryPerson)
                            {
                                Entity thisNewEntity = new Entity("udo_militaryperson");
                                thisNewEntity["udo_name"] = "Military Person";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }
                                if (!string.IsNullOrEmpty(person.mcs_totalActiveSvcDays))
                                {
                                    thisNewEntity["udo_totalactivityservicedays"] = person.mcs_totalActiveSvcDays;
                                }
                                if (!string.IsNullOrEmpty(person.mcs_totalActiveSvcMonths))
                                {
                                    thisNewEntity["udo_totalactivityservicemonths"] = person.mcs_totalActiveSvcMonths;
                                }
                                if (!string.IsNullOrEmpty(person.mcs_totalActiveSvcYears))
                                {
                                    thisNewEntity["udo_totalactivityserviceyears"] = person.mcs_totalActiveSvcYears;
                                }
                                if (!string.IsNullOrEmpty(person.mcs_ptcpntId))
                                {
                                    thisNewEntity["udo_participantid"] = person.mcs_ptcpntId;
                                }
                                if (!string.IsNullOrEmpty(person.mcs_reserveInd))
                                {
                                    thisNewEntity["udo_reserve"] = person.mcs_reserveInd;
                                }
                                if (!string.IsNullOrEmpty(person.mcs_lgyEntitlementAmount))
                                {
                                    thisNewEntity["udo_lgyentitlementamount"] = person.mcs_lgyEntitlementAmount;
                                }
                                if (!string.IsNullOrEmpty(person.mcs_insurancePolicyNumber))
                                {
                                    thisNewEntity["udo_insurancepolicy"] = person.mcs_insurancePolicyNumber;
                                }
                                if (!string.IsNullOrEmpty(person.mcs_insuranceFileNumber))
                                {
                                    thisNewEntity["udo_insurancefile"] = person.mcs_insuranceFileNumber;
                                }
                                if (!string.IsNullOrEmpty(person.mcs_incompetentInd))
                                {
                                    thisNewEntity["udo_incompetent"] = person.mcs_incompetentInd;
                                }
                                if (!string.IsNullOrEmpty(person.mcs_gulfWarRegistryInd))
                                {
                                    thisNewEntity["udo_gulfwarregistry"] = person.mcs_gulfWarRegistryInd;
                                }
                                if (!string.IsNullOrEmpty(person.mcs_disabilitySvcInd))
                                {
                                    thisNewEntity["udo_disabilityservice"] = person.mcs_disabilitySvcInd;
                                }
                                if (!string.IsNullOrEmpty(person.mcs_deathInSvcInd))
                                {
                                    thisNewEntity["udo_deathinservice"] = person.mcs_deathInSvcInd;
                                }
                                if (!string.IsNullOrEmpty(person.mcs_activeDutyStatusInd))
                                {
                                    thisNewEntity["udo_activeduty"] = person.mcs_activeDutyStatusInd;
                                }
                                if (request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                }
                                CreateRequest createMilitaryPerson = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createMilitaryPerson);
                                milPersonCount += 1;
                            }
                        }
                        #endregion

                        #region Tour History

                        if (findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryPersonToursInfo != null)
                        {

                            var shrinq1By2MilitaryPersonTour = findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryPersonToursInfo;
                            foreach (var shrinq1By2MilitaryPersonTourItem in shrinq1By2MilitaryPersonTour)
                            {
                                //LogHelper.LogInfo(request.OrganizationName, true, request.UserId, "MilitaryHistory: Tour History", String.Format("Found Tour: {0}",
                                //    shrinq1By2MilitaryPersonTourItem.mcs_militarySvcBranchTypeName ?? "unknown branch"));
                                //instantiate the new Entity
                                Entity thisNewEntity = new Entity("udo_tourhistory");
                                thisNewEntity["udo_name"] = "Tour Information";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_militarySvcBranchTypeName))
                                {
                                    thisNewEntity["udo_branch"] = shrinq1By2MilitaryPersonTourItem.mcs_militarySvcBranchTypeName;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_daysActiveQty))
                                {
                                    thisNewEntity["udo_daysactive"] = shrinq1By2MilitaryPersonTourItem.mcs_daysActiveQty;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_mpDischargeAuthorityTypeName))
                                {
                                    thisNewEntity["udo_dischargeauthority"] = shrinq1By2MilitaryPersonTourItem.mcs_mpDischargeAuthorityTypeName;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_dischargePayGradeName))
                                {
                                    thisNewEntity["udo_dischargepaygrade"] = shrinq1By2MilitaryPersonTourItem.mcs_dischargePayGradeName;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_mpDischargeCharTypeName))
                                {
                                    thisNewEntity["udo_dischargetype"] = shrinq1By2MilitaryPersonTourItem.mcs_mpDischargeCharTypeName;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_eodDate))
                                {
                                    thisNewEntity["udo_enteredactiveduty"] = dateStringFormat(shrinq1By2MilitaryPersonTourItem.mcs_eodDate);
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_lostTimeDaysNbr))
                                {
                                    thisNewEntity["udo_losttimedays"] = shrinq1By2MilitaryPersonTourItem.mcs_lostTimeDaysNbr;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_militaryDutyVaPurposeTypeCd))
                                {
                                    thisNewEntity["udo_mildutyvapurpose"] = shrinq1By2MilitaryPersonTourItem.mcs_militaryDutyVaPurposeTypeCd;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_militarySvcBranchTypeName))
                                {
                                    thisNewEntity["udo_militarybranch"] = shrinq1By2MilitaryPersonTourItem.mcs_militarySvcBranchTypeName;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_militarySeperationReasonTypeName))
                                {
                                    thisNewEntity["udo_militaryseperationreason"] = shrinq1By2MilitaryPersonTourItem.mcs_militarySeperationReasonTypeName;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_militarySeperationNarritiveTypeCd))
                                {
                                    thisNewEntity["udo_milsepnarrative"] = shrinq1By2MilitaryPersonTourItem.mcs_militarySeperationNarritiveTypeCd;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_militarySeperationReasonTypeName))
                                {
                                    thisNewEntity["udo_milsepreason"] = shrinq1By2MilitaryPersonTourItem.mcs_militarySeperationReasonTypeName;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_militarySvcOtherBranchTypeName))
                                {
                                    thisNewEntity["udo_otherbranch"] = shrinq1By2MilitaryPersonTourItem.mcs_militarySvcOtherBranchTypeName;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_payGradeTypeName))
                                {
                                    thisNewEntity["udo_paygrade"] = shrinq1By2MilitaryPersonTourItem.mcs_payGradeTypeName;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_radDate))
                                {
                                    thisNewEntity["udo_releasedactiveduty"] = dateStringFormat(shrinq1By2MilitaryPersonTourItem.mcs_radDate);
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_serviceNbr))
                                {
                                    thisNewEntity["udo_servicenumber"] = shrinq1By2MilitaryPersonTourItem.mcs_serviceNbr;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_sixYearObligationDate))
                                {
                                    thisNewEntity["udo_sixyearobligation"] = dateStringFormat(shrinq1By2MilitaryPersonTourItem.mcs_sixYearObligationDate);
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_militaryPersonTourNbr))
                                {
                                    thisNewEntity["udo_tournumber"] = shrinq1By2MilitaryPersonTourItem.mcs_militaryPersonTourNbr;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_militaryTourSvcStatusTypeName))
                                {
                                    thisNewEntity["udo_tourservicestatus"] = shrinq1By2MilitaryPersonTourItem.mcs_militaryTourSvcStatusTypeName;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_travelTimeDaysNbr))
                                {
                                    thisNewEntity["udo_traveltimedays"] = shrinq1By2MilitaryPersonTourItem.mcs_travelTimeDaysNbr;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_travelTimeVerifiedInd))
                                {
                                    thisNewEntity["udo_traveltimeverified"] = shrinq1By2MilitaryPersonTourItem.mcs_travelTimeVerifiedInd;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_vadsCd))
                                {
                                    thisNewEntity["udo_vadscode"] = shrinq1By2MilitaryPersonTourItem.mcs_vadsCd;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_varInd))
                                {
                                    thisNewEntity["udo_var"] = shrinq1By2MilitaryPersonTourItem.mcs_varInd;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_verifiedInd))
                                {
                                    thisNewEntity["udo_verified"] = shrinq1By2MilitaryPersonTourItem.mcs_verifiedInd;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_warTimeSvcCountryName))
                                {
                                    thisNewEntity["udo_wartimecountry"] = shrinq1By2MilitaryPersonTourItem.mcs_warTimeSvcCountryName;
                                }
                                if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_warTimeSvcInd))
                                {
                                    thisNewEntity["udo_wartimeservice"] = shrinq1By2MilitaryPersonTourItem.mcs_warTimeSvcInd;
                                }
                                if (request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                }
                                CreateRequest createTour = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createTour);
                                tourHistCount += 1;
                            }
                        }
                        #endregion
                    }
                }
                #region Create records
                if (requestCollection.Count > 0)
                {
                    var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);
                    tLogger.LogEvent("ExecuteMultipleHelper.ExecuteMultiple Complete", "003");

                    if (_debug)
                    {
                        LogBuffer += result.LogDetail;
                        LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                    }

                    if (result.IsFaulted)
                    {
                        tLogger.LogEvent($"LOGERROR ExecuteMultipleHelper.ExecuteMultiple Faulted. Id: {request.MessageId} Error Details: {result.ErrorDetail}", "004");
                        LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, result.ErrorDetail);
                        response.ExceptionMessage = result.FriendlyDetail;
                        response.ExceptionOccurred = true;
                        return response;
                    }
                }
                string logInfo = string.Format("milTheater Added:{0}; decorations Added:{1}; POW Added:{2}; RetPay Added:{3}; Sev Pay Added:{4}; Sev Balance Added:{5}; Sep Balances Added:{6}; Readadjustment Bal Added:{7}; Readadjustment Pay Added:{8}; Mil Perons Added:{9}; Tour Hist Added:{10}",
                    miltheatreCount, decorationCount, POWCount, retPayCount, sevPayCount, sevBalCount, sepBalCount, readBalCount, readPayCount, milPersonCount, tourHistCount);
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, $"Military History Records Processed. {logInfo}", request.Debug);
                tLogger.LogEvent($"Military History Records Processed. {logInfo}", "005");
                #endregion

                //added to generated code
                if (request.udo_militaryserviceId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_militaryserviceId;
                    parent.LogicalName = "udo_militaryservice";
                    parent["udo_militaryservicecomplete"] = true;
                    //OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                    OrgServiceProxy.Update(parent);
                    tLogger.LogEvent($"Military Service Process Complete.", "006");
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                tLogger.LogException(ExecutionException, "999");
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, $"Execution Progress: {progressString} {Environment.NewLine} Exception: {WebApiUtility.StackTraceToString(ExecutionException)}");
                response.ExceptionMessage = "Failed to Process Military History Data";
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

        private string dateStringFormat(string date)
        {
            if (date != null)
            {
                if (date.Length == 10) return date;

                date = date.Insert(2, "/");
                date = date.Insert(5, "/");
            }
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
    }
}
