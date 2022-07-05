using System;
using System.Collections.Generic;

namespace UDO.LOB.LOBAPIDictionary
{

    /// <summary>
    ///    Every LOB Should start with a /UDO/
	///    If the calling client is a Custom Action than the base url is read from the MCS Setting  
	///    If the calling client is another LOB than the base url is in the web.config of the calling lob
    /// 
    /// </summary>
    public class LOBAPIDictionary
    {
        public static Dictionary<string, string> RequestAPI = new Dictionary<string, string>()
        {
            {"UDOcreateAddressRecordsRequest" , "/UDO/ContactsSvc/api/Contacts/createAddress"},     
            {"UDOcreateDependentsRequest","/UDO/ContactsSvc//api/contacts/createDependents"},
            {"UDOcreateFlashesRequest","/UDO/ContactsSvc/api/contacts/createFlashes"},
            {"UDOcreatePastFiduciariesRequest","/UDO/ContactsSvc/api/Contacts/createPastFiduciaries"},
            {"UDOcreateRelationshipsRequest","/UDO/ContactsSvc/api/contacts/createRelationships"},
            {"UDOgetAddressRecordsRequest" , "/UDO/ContactsSvc/api/Contacts/getAddress"},
            {"UDOgetContactRecordsRequest","/UDO/ContactsSvc/api/contacts/getContactRecords"},           
            {"UDOupdateHasBenefitsRequest", "/UDO/ContactsSvc/api/Contacts/hasbenefits"},
            {"UDOValidateAddressRequest","/UDO/ContactsSvc/api/Contacts/validateAddress"},

            {"UDOcreateUDOAppealsRequest", "/UDO/AppealsSvc/api/Appeals/createAppeal" },
            {"UDOcreateUDOAppealDetailsRequest","/UDO/AppealsSvc/api/Appeals/createAppealDetails"},

            {"UDOcreateAwardLinesRequest","/UDO/AwardsSvc/api/Awards/createAwardLines"},
            {"UDOcreateAwardsSyncRequest","/UDO/AwardsSvc/api/awards/createAwardsSyncOrch"},
            {"UDOcreateAwardsSyncOrchRequest","/UDO/AwardsSvc/api/Awards/createAwardsSyncOrch"},
            {"UDOcreateClothingAllowanceRequest","/UDO/AwardsSvc/api/awards/createClothingAllowance"},
            {"UDOcreateDeductionsRequest","/UDO/AwardsSvc/api/awards/createDeductions"},
            {"UDOcreateDiariesRequest","/UDO/AwardsSvc/api/awards/createDiaries"},
            {"UDOcreateEVRsRequest","/UDO/AwardsSvc/api/awards/createEVR"},
            {"UDOcreateIncomeSummaryRequest","/UDO/AwardsSvc/api/awards/createIncomeSummary"},
            {"UDOcreateProceedsRequest","/UDO/AwardsSvc/api/Awards/createProceeds"},
            {"UDOcreateReceivablesRequest","/UDO/AwardsSvc/api/Awards/createReceivables"},
            {"UDOretrieveAwardRequest","/UDO/AwardsSvc/api/awards/retrieveAwards"},

            {"UDOgetBIRLSDataRequest","/UDO/BIRLSSvc/api/BIRLS/getBIRLS"},

            {"UDOFindBankRequest","/UDO/CADDSvc/api/CADD/UDOFindBank"},
            {"UDOInitiateCADDRequest","/UDO/CADDSvc/api/CADD/UDOInitiateCADD"},
            {"UDOupdateCADDAddressRequest","/UDO/CADDSvc/api/CADD/UDOupdateCADDAddress"},

            {"getUDOClaimDocumentsRequest","/UDO/ClaimDocumentsSvc/api/claimDocuments/getUDOClaimDocuments"},
            
            {"UDOClearClaimEstablishmentRequest","/udo/ClaimEstablishmentSvc/api/ClaimEstablishment/clearClaimEstablishment"},
            {"UDOFindClaimEstablishmentRequest","/udo/ClaimEstablishmentSvc/api/ClaimEstablishment/findClaimEstablishment"},
            {"UDOInitiateClaimEstablishmentRequest","/udo/ClaimEstablishmentSvc/api/ClaimEstablishment/initiateClaimEstablishment"},
            {"UDOInsertClaimEstablishmentRequest","/udo/ClaimEstablishmentSvc/api/ClaimEstablishment/insertClaimEstablishment"},

            {"UDOcreateUDOClaimsRequest","/UDO/ClaimsSvc/api/claims/createClaims"},
            {"UDOcreateUDOClaimsSyncRequest","/UDO/ClaimsSvc/api/claims/createClaims"},
            {"UDOcreateUDOClaimsSyncOrchRequest","/UDO/ClaimsSvc/api/claims/createUDOClaimsOrch"},
            {"UDOUpdateUDOClaimsRequest","/UDO/ClaimsSvc/api/claims/updateUDOClaims"},
            {"UDOcreateUDOStatusRequest","/UDO/ClaimsSvc/api/claims/createUDOStatus"},
            {"UDOcreateUDOSuspenseRequest","/UDO/ClaimsSvc/api/claims/createSuspense"},
            {"UDOcreateUDOTrackedItemsRequest","/UDO/ClaimsSvc/api/claims/createTrackedItems"},
            {"UDOcreateUDOLifecyclesRequest","/UDO/ClaimsSvc/api/claims/createLifecycles"},
            {"UDOcreateUDOEvidenceRequest","/UDO/ClaimsSvc/api/claims/createEvidence"},
            {"UDOcreateUdoContentionsRequest","/UDO/ClaimsSvc/api/claims/createContentions"},
            
            {"UDOcreateDenialsRequest","/UDO/DenialsSvc/api/denials/createDenials"},

            {"UDOgetMilitaryInformationRequest","/UDO/eMISSVC/api/emis/getMilitaryInformation"},

            {"UDOcreateUDOAppointmentsRequest","/UDO/ExamAppointmentsSvc/api/ExamsAppointments/createUDOAppointments"},
            {"UDOcreateUDOExamRequest","/UDO/ExamAppointmentsSvc/api/ExamsAppointments/createUDOExam"},
            {"UDOcreateUDOExamRequestRequest","/UDO/ExamAppointmentsSvc/api/ExamsAppointments/createUDOExamRequest"},            
            
            {"UDOgetFlashesRequest","/UDO/FlashesSvc/api/Flashes/getFlashes"},

            {"UDOInitiateFNODRequest","/UDO/FNODSvc/api/FNOD/initiateFNOD"},
            {"UDOSsaDeathMatchInquiryRequest", "/UDO/FNODSvc/api/FNOD/SsaDeathMatchInquiry" },

            {"UDOIDProofOrchestrationRequest","/UDO/IdProofOrchestrationSvc/api/IdProofOrchestration/getUDOIdProofOrchestration"},

            {"UDOcreateIntentToFileRequest","/UDO/IntentToFileSvc/api/IntentToFile/createIntentToFile"},
            {"UDOInitiateITFRequest","/UDO/IntentToFileSvc/api/IntentToFile/initiateITF"},
            {"UDOSubmitITFRequest","/UDO/IntentToFileSvc/api/IntentToFile/submitITF"},
            {"UDOfindZipCodeProcessor","/UDO/IntentToFileSvc/api/IntentToFile/findZipCode"},
            {"UDOvalidateAddressProcessor","/UDO/IntentToFileSvc/api/IntentToFile/validateAddress"},

            {"UDOcreateLegacyPaymentsRequest","/UDO/LegacyPaymentsSvc/api/LegacyPayments/createLegacyPayments"},
            {"UDOcreateLegacyPaymentsDetailsRequest","/UDO/LegacyPaymentsSvc/api/legacypayments/createLegacyPaymentsDetails"},

            {"UDOInitiateLettersRequest","/UDO/LettersSvc/api/Letters/initiateLetters"},

            {"UDOfindMilitaryServiceRequest","/UDO/MilitarySvc/api/MilitaryService/findMilitaryService"},

            {"UDOCreateNoteRequest","/UDO/NotesSvc/api/notes/createNotes"},
            {"UDODeleteNoteRequest","/UDO/NotesSvc/api/notes/deleteNote"},
            {"UDORetrieveNotesRequest","/UDO/NotesSvc/api/notes/getnotes"},
            {"UDOUpdateNoteRequest","/UDO/NotesSvc/api/notes/updateNote"},

            {"UDOcreateAwardAdjustmentRequest","/UDO/PaymentsSvc/api/Payments/createAwardAdjustment"},
            {"UDOcreatePaymentAdjustmentsRequest","/UDO/PaymentsSvc/api/Payments/createPaymentAdjustment"},
            {"UDOcreatePaymentsRequest","/UDO/PaymentsSvc/api/Payments/createPayments"},
            {"UDOgetPaymentDetailsRequest","/UDO/PaymentsSvc/api/payments/getPaymentDetails"},

            {"UDOCloneSRRequest","/UDO/ServiceRequestsSvc/api/ServiceRequest/cloneSR"},
            {"UDOInitiateSRRequest","/UDO/ServiceRequestsSvc/api/ServiceRequest/initiateSR"},
            {"UDOUpdateSRRequest","/UDO/ServiceRequestsSvc/api/ServiceRequest/updateSR"},

            {"UDORunCRMReportRequest", "/UDO/ReportsSvc/api/ReportData/runCRMReport" },

            {"UDOAddPersonRequest", "/UDO/MVISvc/api/MVI/AddPerson"},
            {"UDOBIRLSandOtherSearchRequest","/UDO/MVISvc/api/MVI/BIRLSandOtherSearch"},
            {"UDOCombinedPersonSearchRequest","/UDO/MVISvc/api/MVI/CombinedPersonSearch" },
            //{"UDOCombinedPersonSearchRequest","/UDO/MVISvc/api/MVI/CombinedSecondarySearch" }, Same inbound request; Hence not mapped
            {"UDOCombinedSelectedPersonRequest","/UDO/MVISvc/api/MVI/CombinedSelectedPerson" },
            {"UDOCTIPersonSearchRequest", "/UDO/MVISvc/api/MVI/CTIPersonSearch"},
            {"UDOfindVeteranInfoRequest", "/UDO/MVISvc/api/MVI/findVeteranInfo" },
            {"UDOOpenIDProofAsyncRequest", "/UDO/MVISvc/api/MVI/OpenIDProofAsync"},
            {"UDOPersonSearchRequest", "/UDO/MVISvc/api/MVI/PersonSearch"},
            {"UDOSelectedPersonRequest","/UDO/MVISvc/api/MVI/SelectedPerson"},
            {"UDOgetVeteranIdentifiersRequest","/UDO/MVISvc/api/MVI/UDOgetVeteranIdentifiers"},
            {"UDOCHATPersonSearchRequest",""},
            {"UDOpsFindPersonRequest","UDO/MVISvc/api/MVI/PersonSearch"},
            
            {"UDOCreatePeoplePayeeRequest","/udo/peoplelistpayeecodesvc/api/PeoplelistPayeeCode/createPeoplePayee"},
            {"UDOFiduciaryExistsRequest","/udo/peoplelistpayeecodesvc/api/PeoplelistPayeeCode/findFiduciaryExists"},

            {"UDOfindRatingsRequest","/UDO/RatingsSvc/api/Ratings/findratings"},
            {"UDOgetRatingDataRequest","/UDO/RatingsSvc/api/Ratings/getRatings"},

            {"UDOVBMSUploadDocumentRequest", "/UDO/VBMSSvc/api/VBMS/UDOVBMSUploadDocument"},
            {"UDOVBMSUploadDocumentAsyncRequest", "/UDO/VBMSSvc/api/VBMS/UDOVBMSUploadDocumentAsync"},

            {"UDOgetVBMSDocumentContentRequest","/UDO/VBMSeFolderSvc/api/vbmsefolder/getVBMSDocumentContent"},
            {"UDOCreateVBMSeFolderRequest","/UDO/VBMSeFolderSvc/api/vbmsefolder/createVBMSeFolder"},

            {"UDOcreateUDOVeteranSnapShotRequest", "/UDO/VeteranSnapShotSvc/api/VeteranSnapShot/createUDOVeteranSnapShot"},
            {"UDOCreateVeteranSnapshotRequest", "/UDO/VeteranSnapShotSvc/api/VeteranSnapShot/createVeteranSnapShot"},
            {"UDOLoadVeteranSnapshotAsyncRequest", "/UDO/VeteranSnapShotSvc/api/VeteranSnapShot/loadVeteranSnapshotAsync"},

            {"UDOcreateUDOVirtualVARequest","/udo/VirtualVASvc/api/virtualVA/createUDOVirtualVA"},

            {"UDOSecurityAssocRequest","/udo/SecuritySvc/api/security/Associate"},
            {"UDOSecurityDisassocRequest","/udo/SecuritySvc/api/security/Disassociate"},

			// Dep. Maintenance 
			{"Bgs#GetDependentInfoRequest","/udo/DependentMaintenanceSvc/api/DependentMaintenance/GetDependentInfo"},
			{"Bgs#GetMaritalInfoRequest","/udo/DependentMaintenanceSvc/api/DependentMaintenance/GetMaritalInfo"},
			{"Bgs#GetSensitivityLevelRequest","/udo/DependentMaintenanceSvc/api/DependentMaintenance/GetSensitivityLevel"},
			{"Bgs#GetVeteranInfoRequest","/udo/DependentMaintenanceSvc/api/DependentMaintenance/GetVeteranInfo"},
            {"Bgs#SearchSchoolInfoRequest", "/udo/DependentMaintenanceSvc/api/DependentMaintenance/SearchSchoolInfo" },
            {"Bgs#GetSchoolInfoRequest", "/udo/DependentMaintenanceSvc/api/DependentMaintenance/GetSchoolInfo"},

			{"AddDependent#AddDependentOchestrationRequest","/udo/DependentMaintenanceSvc/api/DependentMaintenance/AddDependentOrchestration"},
            { "VEISDocGenRequest" , "/EC/DocGenSvc/api/VEISDocGen"}

        };
    }
}