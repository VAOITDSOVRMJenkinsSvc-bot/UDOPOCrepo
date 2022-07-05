using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using UDO.LOB.ClaimEstablishment.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VEIS.Messages.AddressWebService;
using VEIS.Messages.BenefitClaimServiceV2;
using VEIS.Messages.VeteranWebService;

namespace UDO.LOB.ClaimEstablishment
{
    public class UDOClaimEstablishmentCommon
    {
        public string ProgressString { get; set; }

        public UDOClaimEstablishmentCommon()
        {
            ProgressString = "Top of Processor";
        }

        public UDObenefitClaimRecordBCS2 ExtractVEISResponse(VEISbenefitClaimRecordBCS2 vimtResponse)
        {
            var udoResponse = new UDObenefitClaimRecordBCS2();

            if (vimtResponse != null)
            {

                #region Benefit Claim Response Info

                udoResponse.fiduciaryInd = vimtResponse.mcs_fiduciaryInd;
                ProgressString = "vimtResponse.mcs_fiduciaryInd";
                udoResponse.gulfWarRegistryPermit = vimtResponse.mcs_gulfWarRegistryPermit;
                ProgressString = "vimtResponse.mcs_gulfWarRegistryPermit";
                udoResponse.homelessIndicator = vimtResponse.mcs_homelessIndicator;
                ProgressString = "vimtResponse.mcs_homelessIndicator";
                udoResponse.powNumberOfDays = vimtResponse.mcs_powNumberOfDays;
                ProgressString = "vimtResponse.mcs_powNumberOfDays";
                udoResponse.returnCode = vimtResponse.mcs_returnCode;
                ProgressString = "vimtResponse.mcs_returnCode";
                udoResponse.returnMessage = vimtResponse.mcs_returnMessage;
                ProgressString = "vimtResponse.mcs_returnMessage";

                #endregion

                #region Benefit Claim Extended Info

                if (udoResponse.UDObenefitClaimRecord1BCS2Info == null)
                {
                    udoResponse.UDObenefitClaimRecord1BCS2Info = new UDObenefitClaimRecord1BCS2();
                }

                udoResponse.UDObenefitClaimRecord1BCS2Info.bddSiteName =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_bddSiteName;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_bddSiteName";
                udoResponse.UDObenefitClaimRecord1BCS2Info.benefitClaimID =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_benefitClaimID;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_benefitClaimID";
                udoResponse.UDObenefitClaimRecord1BCS2Info.benefitClaimReturnLabel =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_benefitClaimReturnLabel;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_benefitClaimReturnLabel";
                udoResponse.UDObenefitClaimRecord1BCS2Info.claimPriorityIndicator =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimPriorityIndicator;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimPriorityIndicator";
                udoResponse.UDObenefitClaimRecord1BCS2Info.claimReceiveDate =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimReceiveDate;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimReceiveDate";
                udoResponse.UDObenefitClaimRecord1BCS2Info.claimStationOfJurisdiction =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimStationOfJurisdiction;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimStationOfJurisdiction";
                udoResponse.UDObenefitClaimRecord1BCS2Info.claimTemporaryStationOfJurisdiction =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimTemporaryStationOfJurisdiction;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimTemporaryStationOfJurisdiction";
                udoResponse.UDObenefitClaimRecord1BCS2Info.claimTypeCode =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimTypeCode;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimTypeCode";
                udoResponse.UDObenefitClaimRecord1BCS2Info.claimTypeName =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimTypeName;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimTypeName";
                udoResponse.UDObenefitClaimRecord1BCS2Info.claimantFirstName =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimantFirstName;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimantFirstName";
                udoResponse.UDObenefitClaimRecord1BCS2Info.claimantLastName =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimantLastName;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimantLastName";
                udoResponse.UDObenefitClaimRecord1BCS2Info.claimantMiddleName =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimantMiddleName;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimantMiddleName";
                udoResponse.UDObenefitClaimRecord1BCS2Info.claimantPersonOrOrganizationIndicator =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimantPersonOrOrganizationIndicator;
                ProgressString =
                    "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimantPersonOrOrganizationIndicator";
                udoResponse.UDObenefitClaimRecord1BCS2Info.claimantSuffix =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimantSuffix;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_claimantSuffix";
                udoResponse.UDObenefitClaimRecord1BCS2Info.cpBenefitClaimID =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_cpBenefitClaimID;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_cpBenefitClaimID";
                udoResponse.UDObenefitClaimRecord1BCS2Info.cpClaimID =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_cpClaimID;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_cpClaimID";
                udoResponse.UDObenefitClaimRecord1BCS2Info.cpClaimReturnLabel =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_cpClaimReturnLabel;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_cpClaimReturnLabel";
                udoResponse.UDObenefitClaimRecord1BCS2Info.cpLocationID =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_cpLocationID;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_cpLocationID";
                udoResponse.UDObenefitClaimRecord1BCS2Info.directDepositAccountID =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_directDepositAccountID;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_directDepositAccountID";
                udoResponse.UDObenefitClaimRecord1BCS2Info.endProductTypeCode =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_endProductTypeCode;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_endProductTypeCode";
                udoResponse.UDObenefitClaimRecord1BCS2Info.informalIndicator =
                    vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_informalIndicator;
                ProgressString = "vimtResponse.VEISbenefitClaimRecord1BCS2Info.mcs_informalIndicator";

                #endregion

                #region Participant Extended Info

                if (vimtResponse.VEISparticipantRecordBCS2Info != null)
                {
                    if (udoResponse.UDOparticipantRecordBCS2Info == null)
                    {
                        udoResponse.UDOparticipantRecordBCS2Info = new UDOparticipantRecordBCS2();
                    }

                    udoResponse.UDOparticipantRecordBCS2Info.bddSiteName =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_bddSiteName;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_bddSiteName";
                    udoResponse.UDOparticipantRecordBCS2Info.benefitClaimID =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_benefitClaimID;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_benefitClaimID";
                    udoResponse.UDOparticipantRecordBCS2Info.benefitClaimReturnLabel =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_benefitClaimReturnLabel;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_benefitClaimReturnLabel";
                    udoResponse.UDOparticipantRecordBCS2Info.claimPriorityIndicator =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimPriorityIndicator;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimPriorityIndicator";
                    udoResponse.UDOparticipantRecordBCS2Info.claimReceiveDate =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimReceiveDate;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimReceiveDate";
                    udoResponse.UDOparticipantRecordBCS2Info.claimStationOfJurisdiction =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimStationOfJurisdiction;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimStationOfJurisdiction";
                    udoResponse.UDOparticipantRecordBCS2Info.claimTemporaryStationOfJurisdiction =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimTemporaryStationOfJurisdiction;
                    ProgressString =
                        "vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimTemporaryStationOfJurisdiction";
                    udoResponse.UDOparticipantRecordBCS2Info.claimTypeCode =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimTypeCode;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimTypeCode";
                    udoResponse.UDOparticipantRecordBCS2Info.claimTypeName =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimTypeName;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimTypeName";
                    udoResponse.UDOparticipantRecordBCS2Info.claimantFirstName =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimantFirstName;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimantFirstName";
                    udoResponse.UDOparticipantRecordBCS2Info.claimantLastName =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimantLastName;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimantLastName";
                    udoResponse.UDOparticipantRecordBCS2Info.claimantMiddleName =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimantMiddleName;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimantMiddleName";
                    udoResponse.UDOparticipantRecordBCS2Info.claimantPersonOrOrganizationIndicator =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimantPersonOrOrganizationIndicator;
                    ProgressString =
                        "vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimantPersonOrOrganizationIndicator";
                    udoResponse.UDOparticipantRecordBCS2Info.claimantSuffix =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimantSuffix;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_claimantSuffix";
                    udoResponse.UDOparticipantRecordBCS2Info.cpBenefitClaimID =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_cpBenefitClaimID;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_cpBenefitClaimI";
                    udoResponse.UDOparticipantRecordBCS2Info.cpClaimID =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_cpClaimID;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_cpClaimID";
                    udoResponse.UDOparticipantRecordBCS2Info.cpClaimReturnLabel =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_cpClaimReturnLabel;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_cpClaimReturnLabel";
                    udoResponse.UDOparticipantRecordBCS2Info.cpLocationID =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_cpLocationID;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_cpLocationID";
                    udoResponse.UDOparticipantRecordBCS2Info.directDepositAccountID =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_directDepositAccountID;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_directDepositAccountID";
                    udoResponse.UDOparticipantRecordBCS2Info.endProductTypeCode =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_endProductTypeCode;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_endProductTypeCode";
                    udoResponse.UDOparticipantRecordBCS2Info.informalIndicator =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_informalIndicator;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_informalIndicator";
                    udoResponse.UDOparticipantRecordBCS2Info.journalDate =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_journalDate;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_journalDate";
                    udoResponse.UDOparticipantRecordBCS2Info.journalObjectID =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_journalObjectID;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_journalObjectID";
                    udoResponse.UDOparticipantRecordBCS2Info.journalStation =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_journalStation;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_journalStation";
                    udoResponse.UDOparticipantRecordBCS2Info.journalStatusTypeCode =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_journalStatusTypeCode;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_journalStatusTypeCode";
                    udoResponse.UDOparticipantRecordBCS2Info.journalUserId =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_journalUserId;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_journalUserId";
                    udoResponse.UDOparticipantRecordBCS2Info.lastPaidDate =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_lastPaidDate;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_lastPaidDate";
                    udoResponse.UDOparticipantRecordBCS2Info.mailingAddressID =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_mailingAddressID;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_mailingAddressID";
                    udoResponse.UDOparticipantRecordBCS2Info.numberOfBenefitClaimRecords =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_numberOfBenefitClaimRecords;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_numberOfBenefitClaimRecords";
                    udoResponse.UDOparticipantRecordBCS2Info.numberOfCPClaimRecords =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_numberOfCPClaimRecords;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_numberOfCPClaimRecords";
                    udoResponse.UDOparticipantRecordBCS2Info.numberOfRecords =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_numberOfRecords;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_numberOfRecords";
                    udoResponse.UDOparticipantRecordBCS2Info.organizationName =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_organizationName;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_organizationName";
                    udoResponse.UDOparticipantRecordBCS2Info.organizationTitleTypeName =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_organizationTitleTypeName;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_organizationTitleTypeName";
                    udoResponse.UDOparticipantRecordBCS2Info.participantClaimantID =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_participantClaimantID;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_participantClaimantID";
                    udoResponse.UDOparticipantRecordBCS2Info.participantVetID =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_participantVetID;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_participantVetID";
                    udoResponse.UDOparticipantRecordBCS2Info.payeeTypeCode =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_payeeTypeCode;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_payeeTypeCode";
                    udoResponse.UDOparticipantRecordBCS2Info.paymentAddressID =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_paymentAddressID;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_paymentAddressID";
                    udoResponse.UDOparticipantRecordBCS2Info.programTypeCode =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_programTypeCode;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_programTypeCode";
                    udoResponse.UDOparticipantRecordBCS2Info.returnCode =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_returnCode;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_returnCode";
                    udoResponse.UDOparticipantRecordBCS2Info.returnMessage =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_returnMessage;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_returnMessage";
                    udoResponse.UDOparticipantRecordBCS2Info.serviceTypeCode =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_serviceTypeCode;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_serviceTypeCode";
                    udoResponse.UDOparticipantRecordBCS2Info.statusTypeCode =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_statusTypeCode;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_statusTypeCode";
                    udoResponse.UDOparticipantRecordBCS2Info.vetFirstName =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_vetFirstName;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_vetFirstName";
                    udoResponse.UDOparticipantRecordBCS2Info.vetLastName =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_vetLastName;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_vetLastName";
                    udoResponse.UDOparticipantRecordBCS2Info.vetMiddleName =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_vetMiddleName;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_vetMiddleName";
                    udoResponse.UDOparticipantRecordBCS2Info.vetSuffix =
                        vimtResponse.VEISparticipantRecordBCS2Info.mcs_vetSuffix;
                    ProgressString = "vimtResponse.VEISparticipantRecordBCS2Info.mcs_vetSuffix";
                    ProgressString =
                        "After findBenefitClaimResponse.benefitClaimRecord.participantRecordResponse Mapping";

                    #region Participant Selection Responses


                    if (vimtResponse.VEISparticipantRecordBCS2Info.VEISselectionBCS2Info != null)
                    {
                        List<UDOselectionBCS2MultipleResponse> selectionBCS2MultipleResponse =
                            new List<UDOselectionBCS2MultipleResponse>();

                        foreach (
                            var selectionBCS2item in vimtResponse.VEISparticipantRecordBCS2Info.VEISselectionBCS2Info)
                        {
                            var udoselectionBCS2 = new UDOselectionBCS2MultipleResponse();

                            udoselectionBCS2.benefitClaimID = selectionBCS2item.mcs_benefitClaimID;
                            ProgressString = "selectionBCS2item.mcs_benefitClaimID";
                            udoselectionBCS2.claimReceiveDate = selectionBCS2item.mcs_claimReceiveDate;
                            ProgressString = "selectionBCS2item.mcs_claimReceiveDate";
                            udoselectionBCS2.claimTypeCode = selectionBCS2item.mcs_claimTypeCode;
                            ProgressString = "selectionBCS2item.mcs_claimTypeCode";
                            udoselectionBCS2.claimTypeName = selectionBCS2item.mcs_claimTypeName;
                            ProgressString = "selectionBCS2item.mcs_claimTypeName";
                            udoselectionBCS2.claimantFirstName = selectionBCS2item.mcs_claimantFirstName;
                            ProgressString = "selectionBCS2item.mcs_claimantFirstName";
                            udoselectionBCS2.claimantLastName = selectionBCS2item.mcs_claimantLastName;
                            ProgressString = "selectionBCS2item.mcs_claimantLastName";
                            udoselectionBCS2.claimantMiddleName = selectionBCS2item.mcs_claimantMiddleName;
                            ProgressString = "selectionBCS2item.mcs_claimantMiddleName";
                            udoselectionBCS2.claimantSuffix = selectionBCS2item.mcs_claimantSuffix;
                            ProgressString = "selectionBCS2item.mcs_claimantSuffix";
                            udoselectionBCS2.endProductTypeCode = selectionBCS2item.mcs_endProductTypeCode;
                            ProgressString = "selectionBCS2item.mcs_endProductTypeCode";
                            udoselectionBCS2.lastActionDate = selectionBCS2item.mcs_lastActionDate;
                            ProgressString = "selectionBCS2item.mcs_lastActionDate";
                            udoselectionBCS2.organizationName = selectionBCS2item.mcs_organizationName;
                            ProgressString = "selectionBCS2item.mcs_organizationName";
                            udoselectionBCS2.organizationTitleTypeName = selectionBCS2item.mcs_organizationTitleTypeName;
                            ProgressString = "selectionBCS2item.mcs_organizationTitleTypeName";
                            udoselectionBCS2.payeeTypeCode = selectionBCS2item.mcs_payeeTypeCode;
                            ProgressString = "selectionBCS2item.mcs_payeeTypeCode";
                            udoselectionBCS2.personOrOrganizationIndicator =
                                selectionBCS2item.mcs_personOrOrganizationIndicator;
                            ProgressString = "selectionBCS2item.mcs_personOrOrganizationIndicator";
                            udoselectionBCS2.programTypeCode = selectionBCS2item.mcs_programTypeCode;
                            ProgressString = "selectionBCS2item.mcs_programTypeCode";
                            udoselectionBCS2.statusTypeCode = selectionBCS2item.mcs_statusTypeCode;
                            ProgressString = "selectionBCS2item.mcs_statusTypeCode";
                            ProgressString = "After benefitClaimRecord.participantRecord.selectionResponse Mapping";
                            selectionBCS2MultipleResponse.Add(udoselectionBCS2);
                        }
                        udoResponse.UDOparticipantRecordBCS2Info.UDOselectionBCS2Info =
                            selectionBCS2MultipleResponse.ToArray();
                    }

                    #endregion
                }

                #endregion

                #region LifeCycleRecordResponse Mapping

                if (vimtResponse.VEISlifeCycleRecordBCS2Info != null)
                {

                    if (udoResponse.UDOlifeCycleRecordBCS2Info == null)
                    {
                        udoResponse.UDOlifeCycleRecordBCS2Info = new UDOlifeCycleRecordBCS2();
                    }

                    ProgressString = "Before lifeCycleRecordResponse Mapping";

                    //udoResponse.UDOlifeCycleRecordBCS2Info.mcs_numberOfRecords = vimtResponse.VIMTlifeCycleRecordBCS2Info.mcs_numberOfRecords;
                    ProgressString = "vimtResponse.VIMTlifeCycleRecordBCS2Info.mcs_numberOfRecords";
                    //udoResponse.UDOlifeCycleRecordBCS2Info.mcs_returnCode = vimtResponse.VIMTlifeCycleRecordBCS2Info.mcs_returnCode;
                    ProgressString = "vimtResponse.VIMTlifeCycleRecordBCS2Info.mcs_returnCode";
                    //udoResponse.UDOlifeCycleRecordBCS2Info.mcs_returnMessage = vimtResponse.VIMTlifeCycleRecordBCS2Info.mcs_returnMessage;
                    ProgressString = "vimtResponse.VIMTlifeCycleRecordBCS2Info.mcs_returnMessage";

                    if (vimtResponse.VEISlifeCycleRecordBCS2Info.VEISlifeCycleRecordsBCS2Info != null)
                    {
                        List<UDOlifeCycleRecordsBCS2MultipleResponse> lifeCycleRecordsBCS2ListRecord =
                            new List<UDOlifeCycleRecordsBCS2MultipleResponse>();
                        foreach (
                            var lifeCycleRecordsBCS2item in
                            vimtResponse.VEISlifeCycleRecordBCS2Info.VEISlifeCycleRecordsBCS2Info)
                        {
                            var udolifeCycleRecordsBCS2 = new UDOlifeCycleRecordsBCS2MultipleResponse();

                            udolifeCycleRecordsBCS2.actionFirstName = lifeCycleRecordsBCS2item.mcs_actionFirstName;
                            ProgressString = "lifeCycleRecordsBCS2item.actionFirstName";
                            udolifeCycleRecordsBCS2.actionLastName = lifeCycleRecordsBCS2item.mcs_actionLastName;
                            ProgressString = "lifeCycleRecordsBCS2item.actionLastName";
                            udolifeCycleRecordsBCS2.actionMiddleName = lifeCycleRecordsBCS2item.mcs_actionMiddleName;
                            ProgressString = "lifeCycleRecordsBCS2item.actionMiddleName";
                            udolifeCycleRecordsBCS2.actionStationNumber =
                                lifeCycleRecordsBCS2item.mcs_actionStationNumber;
                            ProgressString = "lifeCycleRecordsBCS2item.actionStationNumber";
                            udolifeCycleRecordsBCS2.actionSuffix = lifeCycleRecordsBCS2item.mcs_actionSuffix;
                            ProgressString = "lifeCycleRecordsBCS2item.actionSuffix";
                            udolifeCycleRecordsBCS2.benefitClaimID = lifeCycleRecordsBCS2item.mcs_benefitClaimID;
                            ProgressString = "lifeCycleRecordsBCS2item.benefitClaimID";
                            udolifeCycleRecordsBCS2.caseAssignmentLocationID =
                                lifeCycleRecordsBCS2item.mcs_caseAssignmentLocationID;
                            ProgressString = "lifeCycleRecordsBCS2item.caseAssignmentLocationID";
                            udolifeCycleRecordsBCS2.caseAssignmentStatusNumber =
                                lifeCycleRecordsBCS2item.mcs_caseAssignmentStatusNumber;
                            ProgressString = "lifeCycleRecordsBCS2item.caseAssignmentStatusNumber";
                            udolifeCycleRecordsBCS2.caseID = lifeCycleRecordsBCS2item.mcs_caseID;
                            ProgressString = "lifeCycleRecordsBCS2item.caseID";
                            udolifeCycleRecordsBCS2.changedDate = lifeCycleRecordsBCS2item.mcs_changedDate;
                            ProgressString = "lifeCycleRecordsBCS2item.changedDate";
                            udolifeCycleRecordsBCS2.closedDate = lifeCycleRecordsBCS2item.mcs_closedDate;
                            ProgressString = "lifeCycleRecordsBCS2item.closedDate";
                            udolifeCycleRecordsBCS2.journalDate = lifeCycleRecordsBCS2item.mcs_journalDate;
                            ProgressString = "lifeCycleRecordsBCS2item.journalDate";
                            udolifeCycleRecordsBCS2.journalObjectID = lifeCycleRecordsBCS2item.mcs_journalObjectID;
                            ProgressString = "lifeCycleRecordsBCS2item.journalObjectID";
                            udolifeCycleRecordsBCS2.journalStation = lifeCycleRecordsBCS2item.mcs_journalStation;
                            ProgressString = "lifeCycleRecordsBCS2item.journalStation";
                            udolifeCycleRecordsBCS2.journalStatusTypeCode =
                                lifeCycleRecordsBCS2item.mcs_journalStatusTypeCode;
                            ProgressString = "lifeCycleRecordsBCS2item.journalStatusTypeCode";
                            udolifeCycleRecordsBCS2.journalUserID = lifeCycleRecordsBCS2item.mcs_journalUserID;
                            ProgressString = "lifeCycleRecordsBCS2item.journalUserID";
                            udolifeCycleRecordsBCS2.lifeCycleStatusID = lifeCycleRecordsBCS2item.mcs_lifeCycleStatusID;
                            ProgressString = "lifeCycleRecordsBCS2item.lifeCycleStatusID";
                            udolifeCycleRecordsBCS2.lifeCycleStatusTypeName =
                                lifeCycleRecordsBCS2item.mcs_lifeCycleStatusTypeName;
                            ProgressString = "lifeCycleRecordsBCS2item.lifeCycleStatusTypeName";
                            udolifeCycleRecordsBCS2.reasonText = lifeCycleRecordsBCS2item.mcs_reasonText;
                            ProgressString = "lifeCycleRecordsBCS2item.reasonText";
                            udolifeCycleRecordsBCS2.stationofJurisdiction =
                                lifeCycleRecordsBCS2item.mcs_stationofJurisdiction;
                            ProgressString = "lifeCycleRecordsBCS2item.stationofJurisdiction";
                            udolifeCycleRecordsBCS2.statusReasonTypeCode =
                                lifeCycleRecordsBCS2item.mcs_statusReasonTypeCode;
                            ProgressString = "lifeCycleRecordsBCS2item.statusReasonTypeCode";
                            udolifeCycleRecordsBCS2.statusReasonTypeName =
                                lifeCycleRecordsBCS2item.mcs_statusReasonTypeName;
                            ProgressString = "lifeCycleRecordsBCS2item.statusReasonTypeName";
                            ProgressString = "After lifeCycleRecord.lifeCycleRecordsResponse Mapping";
                            lifeCycleRecordsBCS2ListRecord.Add(udolifeCycleRecordsBCS2);
                        }

                        udoResponse.UDOlifeCycleRecordBCS2Info.UDOlifeCycleRecordsBCS2Info =
                            lifeCycleRecordsBCS2ListRecord.ToArray();
                    }
                }

                #endregion

                #region Suspense Record Info

                if (vimtResponse.VEISsuspenceRecordBCS2Info != null)
                {

                    if (udoResponse.UDOsuspenceRecordBCS2Info == null)
                    {
                        udoResponse.UDOsuspenceRecordBCS2Info = new UDOsuspenceRecordBCS2();
                    }

                    udoResponse.UDOsuspenceRecordBCS2Info.numberOfRecords =
                        vimtResponse.VEISsuspenceRecordBCS2Info.mcs_numberOfRecords;
                    ProgressString = "vimtResponse.VIMTsuspenceRecordBCS2Info.mcs_numberOfRecords";
                    udoResponse.UDOsuspenceRecordBCS2Info.returnCode =
                        vimtResponse.VEISsuspenceRecordBCS2Info.mcs_returnCode;
                    ProgressString = "vimtResponse.VIMTsuspenceRecordBCS2Info.mcs_returnCode";
                    udoResponse.UDOsuspenceRecordBCS2Info.returnMessage =
                        vimtResponse.VEISsuspenceRecordBCS2Info.mcs_returnMessage;
                    ProgressString = "vimtResponse.VIMTsuspenceRecordBCS2Info.mcs_returnMessage";
                    ProgressString = "After findBenefitClaimResponse.benefitClaimRecord.suspenceRecordResponse Mapping";


                    if (vimtResponse.VEISsuspenceRecordBCS2Info.VEISsuspenceRecordsBCS2Info != null)
                    {
                        List<UDOsuspenceRecordsBCS2MultipleResponse> suspenceRecordsBCS2MultipleResponse =
                            new List<UDOsuspenceRecordsBCS2MultipleResponse>();

                        foreach (
                            var suspenceRecordBCS2item in
                            vimtResponse.VEISsuspenceRecordBCS2Info.VEISsuspenceRecordsBCS2Info)
                        {

                            var udosuspenceRecordsBCS2 = new UDOsuspenceRecordsBCS2MultipleResponse();

                            udosuspenceRecordsBCS2.claimSuspenceDate = suspenceRecordBCS2item.mcs_claimSuspenceDate;
                            ProgressString = "suspenceRecordBCS2item.mcs_claimSuspenceDate";
                            udosuspenceRecordsBCS2.firstName = suspenceRecordBCS2item.mcs_firstName;
                            ProgressString = "suspenceRecordBCS2item.mcs_firstName";
                            udosuspenceRecordsBCS2.journalDate = suspenceRecordBCS2item.mcs_journalDate;
                            ProgressString = "suspenceRecordBCS2item.mcs_journalDate";
                            udosuspenceRecordsBCS2.journalObjectID = suspenceRecordBCS2item.mcs_journalObjectID;
                            ProgressString = "suspenceRecordBCS2item.mcs_journalObjectID";
                            udosuspenceRecordsBCS2.journalStation = suspenceRecordBCS2item.mcs_journalStation;
                            ProgressString = "suspenceRecordBCS2item.mcs_journalStation";
                            udosuspenceRecordsBCS2.journalStatusTypeCode =
                                suspenceRecordBCS2item.mcs_journalStatusTypeCode;
                            ProgressString = "suspenceRecordBCS2item.mcs_journalStatusTypeCode";
                            udosuspenceRecordsBCS2.journalUserID = suspenceRecordBCS2item.mcs_journalUserID;
                            ProgressString = "suspenceRecordBCS2item.mcs_journalUserID";
                            udosuspenceRecordsBCS2.lastName = suspenceRecordBCS2item.mcs_lastName;
                            ProgressString = "suspenceRecordBCS2item.mcs_lastName";
                            udosuspenceRecordsBCS2.middleName = suspenceRecordBCS2item.mcs_middleName;
                            ProgressString = "suspenceRecordBCS2item.mcs_middleName";
                            udosuspenceRecordsBCS2.suffix = suspenceRecordBCS2item.mcs_suffix;
                            ProgressString = "suspenceRecordBCS2item.mcs_suffix";
                            udosuspenceRecordsBCS2.suspenceActionDate = suspenceRecordBCS2item.mcs_suspenceActionDate;
                            ProgressString = "suspenceRecordBCS2item.mcs_suspenceActionDate";
                            udosuspenceRecordsBCS2.suspenceCode = suspenceRecordBCS2item.mcs_suspenceCode;
                            ProgressString = "suspenceRecordBCS2item.mcs_suspenceCode";
                            udosuspenceRecordsBCS2.suspenceReasonText = suspenceRecordBCS2item.mcs_suspenceReasonText;
                            ProgressString = "suspenceRecordBCS2item.mcs_suspenceReasonText";
                            ProgressString =
                                "After findBenefitClaimResponse.benefitClaimRecord.suspenceRecord.suspenceRecordsResponse Mapping";
                            suspenceRecordsBCS2MultipleResponse.Add(udosuspenceRecordsBCS2);
                        }

                        udoResponse.UDOsuspenceRecordBCS2Info.UDOsuspenceRecordsBCS2Info =
                            suspenceRecordsBCS2MultipleResponse.ToArray();
                    }
                }

                #endregion

            }

            return udoResponse;
        }

        public List<UDOselectionBCS2MultipleResponse> GetClaimEstablishmentByVeteranId(IOrganizationService orgServiceProxy, Guid veteranId)
        {

            var fetchXML = @"<fetch>" +
                           "<entity name='udo_claimestablishment' >" +
                           "<attribute name='udo_veteranid' />" +
                           "<attribute name='udo_lastname' />" +
                           "<attribute name='udo_benefitclaimid' />" +
                           "<attribute name='udo_cpclaimid' />" +
                           "<attribute name='udo_claimreceiveddate' />" +
                           "<attribute name='udo_benefitclaimtype' />" +
                           "<attribute name='udo_benefitclaimtypename' />" +
                           "<attribute name='udo_middlename' />" +
                           "<attribute name='udo_cpbenefitclaimid' />" +
                           "<attribute name='udo_claimestablishmentid' />" +
                           "<attribute name='udo_title' />" +
                           "<attribute name='udo_firstname' />" +
                           "<attribute name='udo_payeecodeid' />" +
                           "<attribute name='udo_lastactiondate' />" +
                           "<filter>" +
                           "<condition attribute='udo_veteranid' operator='eq' value='" + veteranId.ToString() + "' />" +
                           "</filter>" +
                           "<link-entity name='udo_claimestablishmenttypecode' from='udo_claimestablishmenttypecodeid' to='udo_endproduct' link-type='outer' >" +
                           "<attribute name='udo_typecode' />" +
                           "</link-entity>" +
                           "</entity>" +
                           "</fetch>";

            var claims = orgServiceProxy.RetrieveMultiple(new FetchExpression(fetchXML));
            var multipleSelectionBCS2Info = new List<UDOselectionBCS2MultipleResponse>();
            var claimEstablishsmentId = new List<Guid>();

            foreach (var claim in claims.Entities)
            {
                var selectionBCS2Info = new UDOselectionBCS2MultipleResponse();

                if (claim.Contains("udo_claimestablishmenttypecode1.udo_typecode"))
                {
                    var myValue = claim.GetAttributeValue<AliasedValue>("udo_claimestablishmenttypecode1.udo_typecode");
                    selectionBCS2Info.endProductTypeCode = myValue.Value.ToString();
                }



                if (claim.Contains("udo_payeecodeid"))
                {
                    var payeecode = GetClaimEstablishmentPayeeCodetById(orgServiceProxy, claim.GetAttributeValue<EntityReference>("udo_payeecodeid").Id);
                    if (payeecode != null)
                    {
                        selectionBCS2Info.payeeTypeCode = payeecode.GetAttributeValue<string>("udo_payeecode");
                    }
                }

                if (claim.Contains("udo_benefitclaimtype"))
                {
                    selectionBCS2Info.programTypeCode = claim.GetAttributeValue<OptionSetValue>("udo_benefitclaimtype").Value.ToString();
                }

                selectionBCS2Info.statusTypeCode = claim.GetAttributeValue<string>("udo_statustypecode");
                selectionBCS2Info.benefitClaimID = claim.GetAttributeValue<string>("udo_benefitclaimid");
                selectionBCS2Info.claimReceiveDate = claim.GetAttributeValue<string>("udo_claimreceivedate");
                selectionBCS2Info.claimTypeCode = claim.GetAttributeValue<string>("udo_claimtypecode");
                selectionBCS2Info.claimTypeName = claim.GetAttributeValue<string>("udo_claimtypename");
                selectionBCS2Info.claimantFirstName = claim.GetAttributeValue<string>("udo_firstname");
                selectionBCS2Info.claimantMiddleName = claim.GetAttributeValue<string>("udo_middlename");
                selectionBCS2Info.claimantLastName = claim.GetAttributeValue<string>("udo_lastname");
                selectionBCS2Info.claimantSuffix = claim.GetAttributeValue<string>("udo_suffix");
                selectionBCS2Info.lastActionDate = claim.GetAttributeValue<string>("udo_lastactiondate");
                selectionBCS2Info.CRMClaimEstablishmentId = claim.Id;

                multipleSelectionBCS2Info.Add(selectionBCS2Info);
            }

            return multipleSelectionBCS2Info;
        }

        public bool GetClaimEstablishmentTypeCodebyClaimTypeCode(IOrganizationService orgServiceProxy, string claimEstablishmentTypeCode, out Entity claimEstablishmentEntity)
        {
            var fetchXml = @"<fetch top='1' >" +
                           "<entity name='udo_claimestablishmenttypecode' >" +
                           "<attribute name='udo_typecode' />" +
                           "<attribute name='udo_name' />" +
                           "<attribute name='udo_claimestablishmenttypecodeid' />" +
                           "<filter>" +
                           "<condition attribute='udo_typecode' operator='eq' value='" + claimEstablishmentTypeCode + "' />" +
                           "</filter>" +
                           "</entity>" +
                           "</fetch>";

            var entity = orgServiceProxy.RetrieveMultiple(new FetchExpression(fetchXml));

            if (entity != null && entity.Entities.Count > 0)
            {
                claimEstablishmentEntity = entity[0];
                return true;
            }

            claimEstablishmentEntity = null;
            return false;
        }

        public Entity GetClaimEstablishmentById(IOrganizationService orgServiceProxy, Guid claimEstablishmentId)
        {
            return orgServiceProxy.Retrieve("udo_claimestablishment", claimEstablishmentId, new ColumnSet(true));
        }

        public Entity GetIdProofById(IOrganizationService orgServiceProxy, Guid idproofid)
        {
            return orgServiceProxy.Retrieve("udo_idproof", idproofid, new ColumnSet(true));
        }

        public Entity GetInteractiontById(IOrganizationService orgServiceProxy, Guid interactionId)
        {
            return orgServiceProxy.Retrieve("udo_interaction", interactionId, new ColumnSet(true));
        }

        public Entity GetContactById(IOrganizationService orgServiceProxy, Guid contactId)
        {
            var colmnSet = new ColumnSet("udo_filenumber", "udo_ssn");

            return orgServiceProxy.Retrieve("contact", contactId, colmnSet);
        }

        public Entity GetPayeeCodetById(IOrganizationService orgServiceProxy, Guid payeeCodeId)
        {
            return orgServiceProxy.Retrieve("udo_payeecode", payeeCodeId, new ColumnSet(true));
        }

        public Entity GetClaimEstablishmentPayeeCodetById(IOrganizationService orgServiceProxy, Guid claimestablishmentpayeeCodeId)
        {
            return orgServiceProxy.Retrieve("udo_claimestablishmentpayeecode", claimestablishmentpayeeCodeId, new ColumnSet(true));
        }

        public bool GetClaimEstablishmentPayeeCodeByPayeeCode(IOrganizationService orgServiceProxy, string payeeCode, out Entity claimEstablishmentPayeeCodeEntity)
        {
            var fetchXML = @"<fetch top='1' >" +
                           "<entity name='udo_claimestablishmentpayeecode' >" +
                           "<attribute name='udo_claimestablishmentpayeecodeid' />" +
                           "<attribute name='udo_attr2txt' />" +
                           "<attribute name='udo_attr1' />" +
                           "<attribute name='udo_attr3' />" +
                           "<attribute name='udo_attr2' />" +
                           "<attribute name='udo_payeecode' />" +
                           "<attribute name='udo_jrndt' />" +
                           "<attribute name='udo_attr3txt' />" +
                           "<attribute name='udo_deactdt' />" +
                           "<attribute name='udo_description' />" +
                           "<attribute name='udo_name' />" +
                           "<attribute name='udo_attr1txt' />" +
                           "<filter>" +
                           "<condition attribute='udo_payeecode' operator='eq' value='" + payeeCode + "' />" +
                           "</filter>" +
                           "</entity>" +
                           "</fetch>";

            var payeeCodeEntity = orgServiceProxy.RetrieveMultiple(new FetchExpression(fetchXML));

            if (payeeCodeEntity != null && payeeCodeEntity.Entities.Count > 0)
            {
                claimEstablishmentPayeeCodeEntity = payeeCodeEntity[0];
                return true;
            }

            claimEstablishmentPayeeCodeEntity = null;
            return false;
        }

        public bool GetPersonByIdProofIdandPayeeCode(IOrganizationService orgServiceProxy, Guid idProofId, string payeeCode, out Entity personEntity)
        {

            var fetchXML = @"<fetch top='1' >" +
                           "<entity name='udo_person' >" +
                           "<attribute name='udo_first' />" +
                           "<attribute name='udo_vetsnapshotid' />" +
                           "<attribute name='udo_idproofid' />" +
                           "<attribute name='udo_stationofjurisdictionid' />" +
                           "<attribute name='udo_payeecode' />" +
                           "<attribute name='udo_ssn' />" +
                           "<attribute name='udo_middle' />" +
                           "<attribute name='udo_dobstr' />" +
                           "<attribute name='udo_dob' />" +
                           "<attribute name='udo_veteransnapshotid' />" +
                           "<attribute name='udo_vetssn' />" +
                           "<attribute name='udo_payeetypecode' />" +
                           "<attribute name='udo_vetlastname' />" +
                           "<attribute name='udo_veteranid' />" +
                           "<attribute name='udo_payeename' />" +
                           "<attribute name='udo_dayphone' />" +
                           "<attribute name='udo_payeetypename' />" +
                           "<attribute name='udo_ptcpntid' />" +
                           "<attribute name='udo_eveningphone' />" +
                           "<attribute name='udo_last' />" +
                           "<attribute name='udo_personid' />" +
                           "<attribute name='udo_email' />" +
                           "<attribute name='udo_vetfirstname' />" +
                           "<attribute name='udo_filenumber' />" +
                           "<attribute name='udo_payeecodeid' />" +
                           "<link-entity name='contact' from='contactid' to='udo_veteranid' link-type='outer' >" +
                           "<attribute name='udo_filenumber' />" +
                           "</link-entity>" +
                           "<filter>" +
                           "<condition attribute='udo_idproofid' operator='eq' value='" + idProofId + "' />" +
                           "<condition attribute='udo_payeetypecode' operator='eq' value='" + payeeCode + "' />" +
                           "</filter>" +
                           "</entity>" +
                           "</fetch>";

            var person = orgServiceProxy.RetrieveMultiple(new FetchExpression(fetchXML));

            if (person != null && person.Entities.Count > 0)
            {
                personEntity = person[0];
                return true;
            }

            personEntity = null;
            return false;
        }

        public bool GetPayeeCodeByIdProofIdandPayeeCode(IOrganizationService orgServiceProxy, Guid idProofId, string payeeCode, out Entity payeeCodeEentity)
        {

            var fetchXML = @"<fetch top='1' >" +
                           "<entity name='udo_payeecode' >" +
                           "<attribute name='udo_veteranid' />" +
                           "<attribute name='udo_payeecode' />" +
                           "<attribute name='udo_idproofid' />" +
                           "<attribute name='udo_participantid' />" +
                           "<attribute name='udo_filenumber' />" +
                           "<attribute name='udo_payeecodeid' />" +
                           "<filter>" +
                           "<condition attribute='udo_idproofid' operator='eq' value='" + idProofId.ToString() + "' />" +
                           "<condition attribute='udo_payeecode' operator='eq' value='" + payeeCode + "' />" +
                           "</filter>" +
                           "</entity>" +
                           "</fetch>";

            var entity = orgServiceProxy.RetrieveMultiple(new FetchExpression(fetchXML));

            if (entity != null && entity.Entities.Count > 0)
            {
                payeeCodeEentity = entity[0];
                return true;
            }

            payeeCodeEentity = null;
            return false;
        }

        public void UpdateCrmClaimEstablishment( bool exceptionOccurred, string exceptionMessage, Guid claimEstablishmentId, UDObenefitClaimRecordBCS2 benefitClaimRecordBCS2, ClaimEstablishmentStatus claimEstablishmentStatus, IOrganizationService orgServiceProxy)
        {
            var entity = new Entity("udo_claimestablishment")
            {
                Id = claimEstablishmentId
            };

            entity["udo_exceptionoccurred"] = exceptionOccurred;
            entity["udo_exceptionmessage"] = exceptionMessage;

            if (!string.IsNullOrEmpty(benefitClaimRecordBCS2.returnCode))
                entity["udo_returncode"] = benefitClaimRecordBCS2.returnCode;

            if (!string.IsNullOrEmpty(benefitClaimRecordBCS2.returnMessage))
                entity["udo_returncodemessage"] = benefitClaimRecordBCS2.returnMessage;

            if (exceptionOccurred == false)
            {

                switch (claimEstablishmentStatus)
                {
                    case ClaimEstablishmentStatus.Inserted:

                        if (!string.IsNullOrEmpty(benefitClaimRecordBCS2.UDObenefitClaimRecord1BCS2Info.benefitClaimID))
                            entity["udo_benefitclaimid"] =
                                benefitClaimRecordBCS2.UDObenefitClaimRecord1BCS2Info.benefitClaimID;

                        if (
                            !string.IsNullOrEmpty(benefitClaimRecordBCS2.UDObenefitClaimRecord1BCS2Info.claimReceiveDate))
                            entity["udo_claimreceiveddate"] =
                                benefitClaimRecordBCS2.UDObenefitClaimRecord1BCS2Info.claimReceiveDate;

                        if (
                            !string.IsNullOrEmpty(benefitClaimRecordBCS2.UDObenefitClaimRecord1BCS2Info.cpBenefitClaimID))
                            entity["udo_cpbenefitclaimid"] =
                                benefitClaimRecordBCS2.UDObenefitClaimRecord1BCS2Info.cpBenefitClaimID;

                        if (!string.IsNullOrEmpty(benefitClaimRecordBCS2.UDObenefitClaimRecord1BCS2Info.cpClaimID))
                            entity["udo_cpclaimid"] = benefitClaimRecordBCS2.UDObenefitClaimRecord1BCS2Info.cpClaimID;

                        if (string.IsNullOrEmpty(benefitClaimRecordBCS2.UDObenefitClaimRecord1BCS2Info.statusTypeCode))
                        {
                            entity["udo_statustypecode"] = "PEND";
                        }
                        else
                        {
                            entity["udo_statustypecode"] = benefitClaimRecordBCS2.UDObenefitClaimRecord1BCS2Info.statusTypeCode;
                        }

                        break;
                    case ClaimEstablishmentStatus.Cleared:

                        if (string.IsNullOrEmpty(benefitClaimRecordBCS2.UDObenefitClaimRecord1BCS2Info.statusTypeCode))
                        {
                            entity["udo_statustypecode"] = "CLR";
                        }
                        else
                        {
                            entity["udo_statustypecode"] = benefitClaimRecordBCS2.UDObenefitClaimRecord1BCS2Info.statusTypeCode;
                        }

                        entity["udo_datecleared"] = DateTime.Now;

                        break;
                }

            }

            orgServiceProxy.Update(entity);

            ChangeClaimEstablishmentStatus(orgServiceProxy, claimEstablishmentId, claimEstablishmentStatus);

        }

        public void UpdateClaimEstablishmentFromFind(IOrganizationService orgServiceProxy, Guid claimEstablishmentId, Guid idProofId, UDOselectionBCS2MultipleResponse selectionBCS2)
        {
            var changeStatus = false;
            var claimTypeEntity = new Entity();
            //var payeeCode = new Entity();
            ClaimEstablishmentStatus claimEstablishmentStatus = ClaimEstablishmentStatus.Active;

            var claimEntity = GetClaimEstablishmentById(orgServiceProxy, claimEstablishmentId);

            var entity = new Entity("udo_claimestablishment")
            {
                Id = claimEstablishmentId
            };

            if (GetClaimEstablishmentTypeCodebyClaimTypeCode(orgServiceProxy, selectionBCS2.claimTypeCode, out claimTypeEntity))
            {
                //if (GetPayeeCodeByIdProofIdandPayeeCode(orgServiceProxy, idProofId, selectionBCS2.payeeTypeCode, out payeeCode))
                //{
                //  <personOrOrganizationIndicator>P</personOrOrganizationIndicator>

                entity["udo_firstname"] = selectionBCS2.claimantFirstName;
                entity["udo_lastname"] = selectionBCS2.claimantLastName;
                entity["udo_middlename"] = selectionBCS2.claimantMiddleName;
                //entity["udo_suffix"] = selectionBCS2.claimantSuffix;

                if (!claimEntity.Contains("udo_dateofclaim"))
                {
                    var date = new DateTime();
                    if (DateTime.TryParse(selectionBCS2.claimReceiveDate, out date))
                    {
                        entity["udo_dateofclaim"] = date;
                    }
                }

                entity["udo_claimreceiveddate"] = selectionBCS2.claimReceiveDate;

                entity["udo_endproduct"] = new EntityReference("udo_claimestablishmenttypecode", claimTypeEntity.Id);

                var claimEstablishmentPayeeCode = new Entity();
                if (GetClaimEstablishmentPayeeCodeByPayeeCode(orgServiceProxy, selectionBCS2.payeeTypeCode, out claimEstablishmentPayeeCode))
                {
                    entity["udo_payeecodeid"] = new EntityReference("udo_payeecode", claimEstablishmentPayeeCode.Id);
                }

                entity["udo_benefitclaimid"] = selectionBCS2.benefitClaimID;
                entity["udo_cpbenefitclaimid"] = selectionBCS2.benefitClaimID;

                entity["udo_lastactiondate"] = selectionBCS2.lastActionDate;

                entity["udo_benefitclaimtype"] = new OptionSetValue(GetOptionsSetValueForLabel(orgServiceProxy, "udo_claimestablishment", "udo_benefitclaimtype", selectionBCS2.programTypeCode));
                entity["udo_statustypecode"] = selectionBCS2.statusTypeCode;

                switch (selectionBCS2.statusTypeCode)
                {
                    case "PEND":
                        {
                            changeStatus = true;
                            claimEstablishmentStatus = ClaimEstablishmentStatus.Active;

                            break;
                        }
                    case "CAN":
                        {
                            changeStatus = true;
                            claimEstablishmentStatus = ClaimEstablishmentStatus.Cancelled;

                            break;
                        }
                    case "CLR":
                        {

                            var date = new DateTime();
                            if (DateTime.TryParse(selectionBCS2.lastActionDate, out date))
                            {
                                entity["udo_datecleared"] = date;
                            }

                            changeStatus = true;
                            claimEstablishmentStatus = ClaimEstablishmentStatus.Cleared;

                            break;
                        }
                }

                orgServiceProxy.Update(entity);

                if (changeStatus)
                {
                    ChangeClaimEstablishmentStatus(orgServiceProxy, claimEstablishmentId, claimEstablishmentStatus);
                }
                //}
            }
        }

        public void ChangeClaimEstablishmentStatus(IOrganizationService orgServiceProxy, Guid claimEstablishmentId, ClaimEstablishmentStatus claimEstablishmentStatus)
        {
            // var state = new SetStateRequest();

            var request = new UpdateRequest();

            Entity claimestablishment = new Entity("udo_claimestablishment")
            {
                Id = claimEstablishmentId, Attributes = new AttributeCollection()
                {
                    
                }
            };
        
            switch (claimEstablishmentStatus)
            {
                case ClaimEstablishmentStatus.Active:
                case ClaimEstablishmentStatus.Inserted:

                    //state.State = new OptionSetValue((int)ClaimEstablishmentState.Active);
                    //state.Status = new OptionSetValue((int)claimEstablishmentStatus);

                    claimestablishment.Attributes.Add(new KeyValuePair<string, object>("statecode", new OptionSetValue((int)ClaimEstablishmentState.Active)));

                    break;
                case ClaimEstablishmentStatus.Inactive:
                case ClaimEstablishmentStatus.Cleared:
                case ClaimEstablishmentStatus.Cancelled:

                    
                    claimestablishment.Attributes.Add(new KeyValuePair<string, object>("statecode", new OptionSetValue((int)ClaimEstablishmentState.Inactive)));
                    //state.State = new OptionSetValue((int)ClaimEstablishmentState.Inactive);
                    //state.Status = new OptionSetValue((int)claimEstablishmentStatus);
                    break;

                //case ClaimEstablishmentStatus.Inserted:

                //    state.State = new OptionSetValue((int)ClaimEstablishmentState.Active);
                //    state.Status = new OptionSetValue((int)claimEstablishmentStatus);
                //    break;

                //case ClaimEstablishmentStatus.Cleared:

                //    state.State = new OptionSetValue((int)ClaimEstablishmentState.Inactive);
                //    state.Status = new OptionSetValue((int)claimEstablishmentStatus);
                //    break;

                //case ClaimEstablishmentStatus.Cancelled:

                //    state.State = new OptionSetValue((int)ClaimEstablishmentState.Inactive);
                //    state.Status = new OptionSetValue((int)claimEstablishmentStatus);
                //    break;
            }

            claimestablishment.Attributes.Add(new KeyValuePair<string, object>("statuscode", new OptionSetValue((int)claimEstablishmentStatus)));
            //state.EntityMoniker = new EntityReference("udo_claimestablishment", claimEstablishmentId);
            request.Target = claimestablishment;

            var stateSet = (UpdateResponse)orgServiceProxy.Execute(request);
        }

        public string RetrieveValueFromEntityField(Entity thisNewEntity, string fieldName)
        {
            if (thisNewEntity.Attributes.Contains(fieldName))
            {
                return thisNewEntity.GetAttributeValue<string>(fieldName);
            }

            return string.Empty;
        }

        public int RetrieveOptionSetValueFromEntityField(IOrganizationService orgServiceProxy, Entity thisNewEntity, string fieldName)
        {
            if (thisNewEntity.Attributes.Contains(fieldName))
            {
                return thisNewEntity.GetAttributeValue<OptionSetValue>(fieldName).Value;
            }

            return GetOptionsSetValueDefaultValue(orgServiceProxy, thisNewEntity.LogicalName, fieldName);
        }

        public int GetOptionsSetValueForLabel(IOrganizationService service, string entityName, string attributeName, string selectedLabel)
        {

            RetrieveAttributeRequest retrieveAttributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityName,
                LogicalName = attributeName,
                RetrieveAsIfPublished = true
            };
            // Execute the request.
            RetrieveAttributeResponse retrieveAttributeResponse =
                (RetrieveAttributeResponse)service.Execute(retrieveAttributeRequest);
            // Access the retrieved attribute.
            Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata retrievedPicklistAttributeMetadata =
                    (Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata)
                    retrieveAttributeResponse.AttributeMetadata;
            // Get the current options list for the retrieved attribute.

            OptionMetadata[] optionList = retrievedPicklistAttributeMetadata.OptionSet.Options.ToArray();
            int selectedOptionValue = 0;
            foreach (OptionMetadata oMD in optionList)
            {

                if (oMD.Label.LocalizedLabels[0].Label.ToString().ToLower() == selectedLabel.ToLower())
                {
                    selectedOptionValue = oMD.Value.Value;
                    break;
                }
            }
            return selectedOptionValue;
        }

        public int GetOptionsSetValueDefaultValue(IOrganizationService service, string entityName, string attributeName)
        {

            RetrieveAttributeRequest retrieveAttributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityName,
                LogicalName = attributeName,
                RetrieveAsIfPublished = true
            };
            // Execute the request.
            var retrieveAttributeResponse = (RetrieveAttributeResponse)service.Execute(retrieveAttributeRequest);
            // Access the retrieved attribute.
            Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata retrievedPicklistAttributeMetadata =
                    (Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata)
                    retrieveAttributeResponse.AttributeMetadata;
            // Get the current options list for the retrieved attribute.

            return retrievedPicklistAttributeMetadata.DefaultFormValue.Value;
        }

        public bool CheckClaimEstablishment(UDOselectionBCS2MultipleResponse webApi, List<UDOselectionBCS2MultipleResponse> crMrecords, out string rtnVal, out Guid cRmClaimEstablishmentGuid)
        {

            var recordFound = false;
            var updaterecord = false;
            var crmGuid = Guid.Empty;

            foreach (var item in crMrecords)
            {
                if (webApi.benefitClaimID == item.benefitClaimID)
                {
                    crmGuid = item.CRMClaimEstablishmentId;
                    recordFound = true;

                    if (string.IsNullOrEmpty(item.lastActionDate) || string.IsNullOrEmpty(webApi.lastActionDate))
                    {
                        updaterecord = true;
                    }
                    else
                    {
                        if (webApi.lastActionDate != item.lastActionDate)
                        {
                            updaterecord = true;
                        }
                    }

                    break;
                }
            }

            rtnVal = "Bypass";
            if (!recordFound)
            {
                rtnVal = "New";
            }
            else
            {
                if (updaterecord)
                {
                    rtnVal = "Update";
                }
            }

            cRmClaimEstablishmentGuid = crmGuid;
            return recordFound;
        }

        public Guid CreateClaimEstablishment(UDOInitiateClaimEstablishmentRequest request, IOrganizationService organizationService)
        {

            #region Create new Instance Claim Establishment

            Entity thisNewEntity = new Entity();
            thisNewEntity.LogicalName = "udo_claimestablishment";

            #endregion

            #region Set Owner

            if (!String.IsNullOrEmpty(request.OwnerType) && request.OwnerId.HasValue)
            {
                thisNewEntity["ownerid"] = new EntityReference(request.OwnerType, request.OwnerId.Value);
            }
            else
            {
                thisNewEntity["ownerid"] = new EntityReference("systemuser", request.UserId);
            }

            #endregion

            #region Copy Basic Request Information

            UpdateAddress(request, thisNewEntity);

            thisNewEntity["udo_name"] = "Claim Establishment for " + request.FirstName + " " + request.LastName;
            thisNewEntity["udo_filenumber"] = request.vetfileNumber;
            thisNewEntity["udo_participantid"] = request.ptcpntId.ToString();

            thisNewEntity["udo_firstname"] = request.FirstName;
            thisNewEntity["udo_lastname"] = request.LastName;

            thisNewEntity["udo_veteranparticipantid"] = request.vetptcpntId.ToString();
            thisNewEntity["udo_filenumber"] = request.vetfileNumber;
            thisNewEntity["udo_ssn"] = request.SSN;

            if (request.udo_payeecodeid != Guid.Empty)
            {
                var payeeCodeEntity = GetPayeeCodetById(organizationService, request.udo_payeecodeid);
                var claimEstablishmentPayeeCode = new Entity();
                if (GetClaimEstablishmentPayeeCodeByPayeeCode(organizationService, payeeCodeEntity.GetAttributeValue<string>("udo_payeecode"), out claimEstablishmentPayeeCode))
                {
                    thisNewEntity["udo_payeecodeid"] = new EntityReference("udo_claimestablishmentpayeecode", claimEstablishmentPayeeCode.Id);
                }
            }

            thisNewEntity["udo_personid"] = new EntityReference("udo_person", request.udo_personid);
            thisNewEntity["udo_idproofid"] = new EntityReference("udo_idproof", request.udo_idproofid);
            thisNewEntity["udo_veteranid"] = new EntityReference("contact", request.udo_veteranid);
            thisNewEntity["udo_interaction"] = new EntityReference("udo_interaction", request.udo_interaction);
            thisNewEntity["udo_veteransnapshotid"] = new EntityReference("udo_veteransnapshot", request.udo_veteransnapshotid);

            if (!string.IsNullOrEmpty(request.awardtypecode))
            {
                var selectedOption = GetOptionsSetValueForLabel(organizationService, "udo_claimestablishment", "udo_benefitclaimtype", request.awardtypecode);
                thisNewEntity["udo_benefitclaimtype"] = new OptionSetValue(selectedOption);
            }

            #endregion

            #region Retrieve BIRLS data

            GetBirls(request, thisNewEntity);

            #endregion

            #region Create Claim Establishment Record
            
            // organizationService.CallerId = Guid.Empty;
            var claimEstablishmentId =
                organizationService.Create(TruncateHelper.TruncateFields(request.MessageId, thisNewEntity, request.OrganizationName,
                    request.UserId, request.LogTiming, organizationService));

            #endregion

            return claimEstablishmentId;

        }

        public void UpdateAddress(UDOInitiateClaimEstablishmentRequest request, Entity claimEstablishment)
        {
            var addresssType = AddressType.Domestic;

            claimEstablishment["udo_addressline1"] = request.address1;
            claimEstablishment["udo_addressline2"] = request.address2;
            claimEstablishment["udo_addressline3"] = request.address3;
            claimEstablishment["udo_city"] = request.city;
            claimEstablishment["udo_state"] = request.state;
            claimEstablishment["udo_country"] = string.Empty;
            claimEstablishment["udo_postalcode"] = request.postalcode;

            var findAllPtcpntAddrsByPtcpntIdRequest = new VEISfallpidaddpidfindAllPtcpntAddrsByPtcpntIdRequest
            {
                LogTiming = request.LogTiming,
                LogSoap = request.LogSoap,
                Debug = request.Debug,
                RelatedParentEntityName = request.RelatedParentEntityName,
                RelatedParentFieldName = request.RelatedParentFieldName,
                RelatedParentId = request.RelatedParentId,
                UserId = request.UserId,
                OrganizationName = request.OrganizationName,
                mcs_ptcpntid = request.ptcpntId
            };

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

            // VEISfallpidaddpidffindAllPtcpntAddrsByPtcpntIdResponse findAllPtcpntAddrsByPtcpntIdResponse = WebApiUtility.SendReceive<VEISfallpidaddpidffindAllPtcpntAddrsByPtcpntIdResponse, VEISfallpidaddpidffindAllPtcpntAddrsByPtcpntIdRequest>(findAllPtcpntAddrsByPtcpntIdRequest);
            //_progressString = "After VIMT EC Call";
            var findAllPtcpntAddrsByPtcpntIdResponse = WebApiUtility.SendReceive<VEISfallpidaddpidfindAllPtcpntAddrsByPtcpntIdResponse>(findAllPtcpntAddrsByPtcpntIdRequest, WebApiType.VEIS);
            //var findAllPtcpntAddrsByPtcpntIdResponse = WebApiUtility.SendReceive<VEISfallpidaddpidfindAllPtcpntAddrsByPtcpntIdResponse>(veisBaseUri, findAllPtcpntAddrsByPtcpntIdRequest.MessageId, findAllPtcpntAddrsByPtcpntIdRequest, logSettings);

            if (request.LogSoap || findAllPtcpntAddrsByPtcpntIdResponse.ExceptionOccurred)
            {
                if (findAllPtcpntAddrsByPtcpntIdResponse.SerializedSOAPRequest != null || findAllPtcpntAddrsByPtcpntIdResponse.SerializedSOAPResponse != null)
                {
                    var requestResponse = findAllPtcpntAddrsByPtcpntIdResponse.SerializedSOAPRequest + findAllPtcpntAddrsByPtcpntIdResponse.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfallpidaddpidfindAllPtcpntAddrsByPtcpntIdRequest Request/Response {requestResponse}", true);
                }
            }

            if (findAllPtcpntAddrsByPtcpntIdResponse.ExceptionOccurred)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().ToString(), findAllPtcpntAddrsByPtcpntIdResponse.ExceptionMessage);
                return;
            }

            if (findAllPtcpntAddrsByPtcpntIdResponse.VEISfallpidaddpidfreturnInfo != null)
            {
                var ptcpntAddrsDto = findAllPtcpntAddrsByPtcpntIdResponse.VEISfallpidaddpidfreturnInfo;
                foreach (var ptcpntAddrsDtoItem in ptcpntAddrsDto)
                {
                    var isMailingAddress = false;

                    if (ptcpntAddrsDtoItem.mcs_ptcpntAddrsTypeNm != string.Empty)
                    {
                        if (ptcpntAddrsDtoItem.mcs_ptcpntAddrsTypeNm.Equals("mailing", StringComparison.InvariantCultureIgnoreCase))
                        {
                            isMailingAddress = true;
                        }
                    }
                    if (ptcpntAddrsDtoItem.mcs_addrsOneTxt != string.Empty)
                    {
                        if (isMailingAddress)
                        {
                            claimEstablishment["udo_addressline1"] = ptcpntAddrsDtoItem.mcs_addrsOneTxt;
                        }
                    }
                    if (ptcpntAddrsDtoItem.mcs_addrsTwoTxt != string.Empty)
                    {
                        if (isMailingAddress)
                        {
                            claimEstablishment["udo_addressline2"] = ptcpntAddrsDtoItem.mcs_addrsTwoTxt;
                        }
                    }
                    if (ptcpntAddrsDtoItem.mcs_addrsThreeTxt != string.Empty)
                    {
                        if (isMailingAddress)
                        {
                            claimEstablishment["udo_addressline3"] = ptcpntAddrsDtoItem.mcs_addrsThreeTxt;
                        }
                    }

                    if (!string.IsNullOrEmpty(ptcpntAddrsDtoItem.mcs_cityNm))
                    {
                        if (isMailingAddress)
                        {
                            claimEstablishment["udo_city"] = ptcpntAddrsDtoItem.mcs_cityNm;
                        }
                    }
                    else if (!string.IsNullOrEmpty(ptcpntAddrsDtoItem.mcs_mltyPostOfficeTypeCd))
                    {
                        if (isMailingAddress)
                        {
                            addresssType = AddressType.Overseas;
                            claimEstablishment["udo_city"] = ptcpntAddrsDtoItem.mcs_mltyPostOfficeTypeCd;
                        }
                    }

                    if (!string.IsNullOrEmpty(ptcpntAddrsDtoItem.mcs_postalCd))
                    {
                        if (isMailingAddress)
                        {
                            claimEstablishment["udo_state"] = ptcpntAddrsDtoItem.mcs_postalCd;
                        }
                    }
                    else if (!string.IsNullOrEmpty(ptcpntAddrsDtoItem.mcs_mltyPostalTypeCd))
                    {
                        if (isMailingAddress)
                        {
                            addresssType = AddressType.Overseas;
                            claimEstablishment["udo_state"] = ptcpntAddrsDtoItem.mcs_mltyPostalTypeCd;
                        }
                    }

                    if (ptcpntAddrsDtoItem.mcs_cntryNm != string.Empty)
                    {
                        if (isMailingAddress)
                        {
                            claimEstablishment["udo_country"] = ptcpntAddrsDtoItem.mcs_cntryNm;
                        }
                    }

                    if (ptcpntAddrsDtoItem.mcs_zipPrefixNbr != string.Empty)
                    {
                        if (isMailingAddress)
                        {
                            claimEstablishment["udo_postalcode"] = ptcpntAddrsDtoItem.mcs_zipPrefixNbr;
                        }
                    }
                    else if (ptcpntAddrsDtoItem.mcs_frgnPostalCd != string.Empty)
                    {
                        if (isMailingAddress)
                        {
                            addresssType = AddressType.International;
                            claimEstablishment["udo_postalcode"] = ptcpntAddrsDtoItem.mcs_frgnPostalCd;
                        }
                    }

                    switch (addresssType)
                    {
                        case AddressType.Domestic:
                            claimEstablishment["udo_addresstype"] = new OptionSetValue((int)AddressType.Domestic);
                            break;
                        case AddressType.Overseas:
                            claimEstablishment["udo_addresstype"] = new OptionSetValue((int)AddressType.Overseas);
                            break;
                        case AddressType.International:
                            claimEstablishment["udo_addresstype"] = new OptionSetValue((int)AddressType.International);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                }
            }
        }

        public void GetBirls(UDOInitiateClaimEstablishmentRequest request, Entity thisNewEntity)
        {

            thisNewEntity["udo_homelessindicator"] = "N";
            thisNewEntity["udo_persiangulfwarserviceindicator"] = "N";
            thisNewEntity["udo_powdays"] = "0";

            var findBirlsRecordByFileNumberRequest = new VEISbrlsFNfindBirlsRecordByFileNumberRequest
            {
                LogTiming = request.LogTiming,
                LogSoap = request.LogSoap,
                Debug = request.Debug,
                UserId = request.UserId,
                OrganizationName = request.OrganizationName,
                mcs_filenumber = request.fileNumber,
                LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                }
            };

            // var findBirlsRecordByFileNumberResponse = WebApiUtility.SendReceive<VEISbrlsFNfindBirlsRecordByFileNumberResponse, VEISbrlsFNfindBirlsRecordByFileNumberRequest>(findBirlsRecordByFileNumberRequest);
            //_progressString = "After VIMT EC Call";

            var findBirlsRecordByFileNumberResponse = WebApiUtility.SendReceive<VEISbrlsFNfindBirlsRecordByFileNumberResponse>(findBirlsRecordByFileNumberRequest, WebApiType.VEIS);

            if (request.LogSoap || findBirlsRecordByFileNumberResponse.ExceptionOccurred)
            {
                if (findBirlsRecordByFileNumberResponse.SerializedSOAPRequest != null || findBirlsRecordByFileNumberResponse.SerializedSOAPResponse != null)
                {
                    var requestResponse = findBirlsRecordByFileNumberResponse.SerializedSOAPRequest + findBirlsRecordByFileNumberResponse.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISbrlsFNfindBirlsRecordByFileNumberRequest Request/Response {requestResponse}", true);
                }
            }
            if (findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo == null) return;

            #region main BIRLS Update

            if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_HOMELESS_VET_IND))
            {
                thisNewEntity["udo_homelessindicator"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_HOMELESS_VET_IND;
            }
            else
            {
                thisNewEntity["udo_homelessindicator"] = "N";
            }

            if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_PERSIAN_GULF_SVC_IND))
            {
                thisNewEntity["udo_persiangulfwarserviceindicator"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_PERSIAN_GULF_SVC_IND;
            }
            else
            {
                thisNewEntity["udo_persiangulfwarserviceindicator"] = "N";
            }

            if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_POW_NUMBER_OF_DAYS))
            {
                thisNewEntity["udo_powdays"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_POW_NUMBER_OF_DAYS;
            }
            else
            {
                thisNewEntity["udo_powdays"] = "0";
            }

            if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CLAIM_FOLDER_LOCATION))
            {
                thisNewEntity["udo_sectionunitno"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CLAIM_FOLDER_LOCATION;
            }

            #endregion

            #region VIMTbrlsFNSERVICEInfo

            //if (findBirlsRecordByFileNumberResponse.VIMTbrlsFNreturnInfo.VIMTbrlsFNSERVICEInfo != null)
            //{
            //    var fnServiceInfo = findBirlsRecordByFileNumberResponse.VIMTbrlsFNreturnInfo.VIMTbrlsFNSERVICEInfo;

            //    foreach (var fnService in fnServiceInfo)
            //    {

            //    }
            //}

            #endregion

            #region VIMTbrlsFNBIRLS_SELECTIONInfo

            //if (findBirlsRecordByFileNumberResponse.VIMTbrlsFNreturnInfo.VIMTbrlsFNBIRLS_SELECTIONInfo != null)
            //{
            //    var birlsRecordService = findBirlsRecordByFileNumberResponse.VIMTbrlsFNreturnInfo.VIMTbrlsFNBIRLS_SELECTIONInfo;

            //    foreach (var birlsRecordServiceItem in birlsRecordService)
            //    {
            //       birlsRecordServiceItem.
            //    }
            //}

            #endregion

            #region VIMTbrlsFNFOLDERInfo

            //if (findBirlsRecordByFileNumberResponse.VIMTbrlsFNreturnInfo.VIMTbrlsFNFOLDERInfo != null)
            //{
            //    var birlsRecordService = findBirlsRecordByFileNumberResponse.VIMTbrlsFNreturnInfo.VIMTbrlsFNFOLDERInfo;

            //    foreach (var birlsRecordServiceItem in birlsRecordService)
            //    {
            //        //birlsRecordServiceItem.
            //    }
            //}

            #endregion
        }

    }

    public enum ClaimEstablishmentState : long
    {
        Active = 0,
        Inactive = 1
    }

    public enum ClaimEstablishmentStatus : long
    {
        Active = 1,
        Inactive = 2,
        Inserted = 752280001,
        Cleared = 752280002,
        Cancelled = 752280003
    }

    public enum AddressType : long
    {
        Domestic = 1,
        Overseas = 2,
        International = 3
    }

}