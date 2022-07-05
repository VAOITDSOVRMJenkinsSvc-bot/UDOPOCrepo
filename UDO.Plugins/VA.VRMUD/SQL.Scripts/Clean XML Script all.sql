/*
ContactExtensionBase
PhoneCallBase
va_bankaccountExtensionBase
va_servicerequestExtensionBase
*/

----------------------------------------------------------------------------------------------
-- Purge non dependent 
-- va_bankaccountExtensionBase

print ''
print '--------------------------------------------------------------'
print 'Processing va_bankaccountExtensionBase no dependencies'

 declare @rows int
 declare @affectedDate as date
 set @affectedDate = CAST(dateadd(D,-30,getdate()) as Date)
 
 select @rows = COUNT(*)
 FROM dbo.va_bankaccountExtensionBase  AS ext
 INNER JOIN dbo.va_bankaccountBase AS base ON ext.va_bankaccountId = base.va_bankaccountId
 where base.CreatedOn < @affectedDate 
	and ext.va_originatingphonecallid is null 
	and ext.va_accountownerid is null

 print 'Processing ' + cast(@rows as varchar(50)) + ' rows va_bankaccountExtensionBase no dependencies'

update ext
set
 va_AppellantAddressResponse = null
,va_FindAwardAddressesResponse = null
,va_FindDepositAccountResponse = null
,va_FindMailingAddressResponse = null
,va_FindPaymentAddressResponse = null
,va_UpdateAddressRequest = null
,va_UpdateAddressResponse = null
,va_UpdateApellantAddressRequest = null
,va_UpdateApellantAddressResponse = null
,va_UpdateAppellantAddressResponse = null
,va_validateaddressresponse = null
,va_ValidateApellantAddressResponse = null
 FROM dbo.va_bankaccountExtensionBase  AS ext
 INNER JOIN dbo.va_bankaccountBase AS base ON ext.va_bankaccountId = base.va_bankaccountId
 where base.CreatedOn < @affectedDate 
	and ext.va_originatingphonecallid is null 
	and ext.va_accountownerid is null

go


print ''
print '--------------------------------------------------------------'
print 'Processing va_servicerequestExtensionBase no dependencies'

 declare @rows int
 declare @affectedDate as date
 set @affectedDate = CAST(dateadd(D,-30,getdate()) as Date)
 
 select @rows = COUNT(*)
FROM dbo.va_servicerequestExtensionBase  AS ext
 INNER JOIN dbo.va_servicerequestBase AS base ON ext.va_servicerequestId = base.va_servicerequestId
 where base.CreatedOn < @affectedDate
	and ext.va_originatingcallid is null 
	and ext.va_relatedveteranid is null

 print 'Processing ' + cast(@rows as varchar(50)) + ' rows va_servicerequestExtensionBase no dependencies'

-- va_servicerequestExtensionBase
update ext
set
 va_NoteCreateRequest = null
,va_ROXML = null
 FROM dbo.va_servicerequestExtensionBase  AS ext
 INNER JOIN dbo.va_servicerequestBase AS base ON ext.va_servicerequestId = base.va_servicerequestId
 where base.CreatedOn < @affectedDate
	and ext.va_originatingcallid is null 
	and ext.va_relatedveteranid is null

go


-------------------------------------------------------------------------------------
-- ContactExtensionBase
-------------------------------------------------------------------------------------
-- Purge va_bankaccountExtensionBase depending to ContactExtensionBase

print ''
print '--------------------------------------------------------------'
print 'Processing va_bankaccountExtensionBase depending to ContactExtensionBase'

 declare @rows int
 declare @affectedDate as date
 set @affectedDate = CAST(dateadd(D,-30,getdate()) as Date)
 
 select @rows = COUNT(*)
FROM dbo.va_bankaccountExtensionBase AS ext
where ext.va_originatingphonecallid is null and ext.va_accountownerid in 
	(select contactId from dbo.ContactBase 
		WHERE CreatedOn < @affectedDate)

 print 'Processing ' + cast(@rows as varchar(50)) + ' rows va_bankaccountExtensionBase depending to ContactExtensionBase'

UPDATE ext
 SET 
va_AppellantAddressResponse = null
,va_FindAwardAddressesResponse = null
,va_FindDepositAccountResponse = null
,va_FindMailingAddressResponse = null
,va_FindPaymentAddressResponse = null
,va_UpdateAddressRequest = null
,va_UpdateAddressResponse = null
,va_UpdateApellantAddressRequest = null
,va_UpdateApellantAddressResponse = null
,va_UpdateAppellantAddressResponse = null
,va_validateaddressresponse = null
,va_ValidateApellantAddressResponse = null
FROM dbo.va_bankaccountExtensionBase AS ext
where ext.va_originatingphonecallid is null and ext.va_accountownerid in 
	(select contactId from dbo.ContactBase 
		WHERE CreatedOn < @affectedDate)

go


print ''
print '--------------------------------------------------------------'
print 'Processing va_servicerequestExtensionBase depending to ContactExtensionBase'

 declare @rows int
 declare @affectedDate as date
 set @affectedDate = CAST(dateadd(D,-30,getdate()) as Date)
 
 select @rows = COUNT(*)
FROM dbo.va_servicerequestExtensionBase AS ext
where ext.va_originatingcallid is null and ext.va_relatedveteranid in 
	(select contactId from dbo.ContactBase 
		WHERE CreatedOn < @affectedDate)

 print 'Processing ' + cast(@rows as varchar(50)) + ' rows va_servicerequestExtensionBase depending to ContactExtensionBase'

-- Purge va_servicerequestExtensionBase depending to ContactExtensionBase
UPDATE ext
 SET 
va_NoteCreateRequest = null
,va_ROXML = null
FROM dbo.va_servicerequestExtensionBase AS ext
where ext.va_originatingcallid is null and ext.va_relatedveteranid in 
	(select contactId from dbo.ContactBase 
		WHERE CreatedOn < @affectedDate)

go

print ''
print '--------------------------------------------------------------'
print 'Processing ContactExtensionBase'

 declare @rows int
 declare @affectedDate as date
 set @affectedDate = CAST(dateadd(D,-30,getdate()) as Date)
 
 select @rows = COUNT(*)
 FROM dbo.ContactExtensionBase AS ext
 INNER JOIN dbo.ContactBase AS base ON ext.ContactId = base.ContactId
 WHERE base.CreatedOn < @affectedDate

 print 'Processing ' + cast(@rows as varchar(50)) + ' rows ContactExtensionBase'

-- Purge ContactExtensionBase
UPDATE ext
 SET 
  va_FindBIRLSResponse = null
,va_FindCorpRecordResponse = null
,va_FindMilitaryRecordbyptcpntidResponse = null
,va_GeneralInformationResponse = null
,va_GeneralInformationResponsebyPID = null
,va_MVIResponse = null
,va_AppellantAddressResponse = null
,va_AwardFiduciaryResponse = null
,va_BenefitClaimResponse = null
,va_CreateNoteResponse = null
,va_findaddressresponse = null
,va_FindAllRelationshipsResponse = null
,va_FindAppealsResponse = null
,va_FindAwardCompensationResponse = null
,va_FindBenefitDetailResponse = null
,va_FindClaimantLettersResponse = null
,va_FindClaimStatusResponse = null
,va_FindContentionsResponse = null
,va_FindDenialsResponse = null
,va_FindDependentsResponse = null
,va_FindDevelopmentNotesResponse = null
,va_FindFiduciaryPOAResponse = null
,va_findgetdocumentlist = null
,va_FindIncomeExpenseResponse = null
,va_FindIndividualAppealsResponse = null
,va_FindMonthOfDeathResponse = null
,va_FindOtherAwardInformationResponse = null
,va_FindPaymentHistoryResponse = null
,va_FindRatingDataResponse = null
,va_findReasonsByRbaIssueIdResponse = null
,va_FindTrackedItemsResponse = null
,va_findunsolvedevidenceresponse = null
,va_FindVeteranResponse = null
,va_GetRegistrationStatus = null
,va_IsAliveResponse = null
,va_ReadDataAppointmentResponse = null
,va_ReadDataExamResponse = null
,va_Response = null
,va_RetrievePaymentDetailResponse = null
,va_RetrievePaymentSummaryResponse = null
,va_UpdateAppellantAddressResponse = null
 FROM dbo.ContactExtensionBase AS ext
 INNER JOIN dbo.ContactBase AS base ON ext.ContactId = base.ContactId
 WHERE base.CreatedOn < @affectedDate

 go

 -------------------------------------------------------------------------------------
-- PhoneCallBase
-------------------------------------------------------------------------------------
-- Purge va_bankaccountExtensionBase depending to PhoneCallBase

print ''
print '--------------------------------------------------------------'
print 'Processing va_bankaccountExtensionBase depending to PhoneCallBase'

 declare @rows int
 declare @affectedDate as date
 set @affectedDate = CAST(dateadd(D,-30,getdate()) as Date)
 
 select @rows = COUNT(*)
FROM dbo.va_bankaccountExtensionBase AS ext
where ext.va_originatingphonecallid in 
	(select activityId FROM dbo.ActivityPointerBase 
		where ActivityTypeCode = 4210 
			and CreatedOn < @affectedDate
			and statecode != 0)

 print 'Processing ' + cast(@rows as varchar(50)) + ' rows va_bankaccountExtensionBase depending to PhoneCallBase'

UPDATE ext
 SET 
va_AppellantAddressResponse = null
,va_FindAwardAddressesResponse = null
,va_FindDepositAccountResponse = null
,va_FindMailingAddressResponse = null
,va_FindPaymentAddressResponse = null
,va_UpdateAddressRequest = null
,va_UpdateAddressResponse = null
,va_UpdateApellantAddressRequest = null
,va_UpdateApellantAddressResponse = null
,va_UpdateAppellantAddressResponse = null
,va_validateaddressresponse = null
,va_ValidateApellantAddressResponse = null
FROM dbo.va_bankaccountExtensionBase AS ext
where ext.va_originatingphonecallid in 
	(select activityId FROM dbo.ActivityPointerBase 
		where ActivityTypeCode = 4210 
			and CreatedOn < @affectedDate
			and statecode != 0)

go


print ''
print '--------------------------------------------------------------'
print 'Processing va_servicerequestExtensionBase depending to PhoneCallBase'

 declare @rows int
 declare @affectedDate as date
 set @affectedDate = CAST(dateadd(D,-30,getdate()) as Date)
 
 select @rows = COUNT(*)
FROM dbo.va_servicerequestExtensionBase AS ext
where ext.va_originatingcallid in 
	(select activityId FROM dbo.ActivityPointerBase 
		where ActivityTypeCode = 4210 
			and CreatedOn < @affectedDate
			and statecode != 0)

 print 'Processing ' + cast(@rows as varchar(50)) + ' rows va_servicerequestExtensionBase depending to PhoneCallBase'

-- Purge va_servicerequestExtensionBase depending to PhoneCallBase
UPDATE ext
 SET 
va_NoteCreateRequest = null
,va_ROXML = null
FROM dbo.va_servicerequestExtensionBase AS ext
where ext.va_originatingcallid in 
	(select activityId FROM dbo.ActivityPointerBase 
		where ActivityTypeCode = 4210 
			and CreatedOn < @affectedDate
			and statecode != 0)

go

print ''
print '--------------------------------------------------------------'
print 'Processing PhoneCallBase'

 declare @rows int
 declare @affectedDate as date
 set @affectedDate = CAST(dateadd(D,-30,getdate()) as Date)
 
 select @rows = COUNT(*)
 FROM dbo.PhoneCallBase AS ext
 INNER JOIN dbo.ActivityPointerBase AS base ON ext.ActivityId = base.ActivityId and base.ActivityTypeCode = 4210
 where base.ActivityTypeCode = 4210 
	and base.CreatedOn < @affectedDate
	and base.statecode != 0

 print 'Processing ' + cast(@rows as varchar(50)) + ' rows PhoneCallBase'

-- Purge va_servicerequestExtensionBase depending to PhoneCallBase
update ext
set
va_FindBIRLSResponse = null
,va_FindCorpRecordResponse = null
,va_findMilitaryRecordByPtcpntIdResponse = null
,va_GeneralInformationResponse = null
,va_GeneralInformationResponsebyPID = null
,va_MVIResponse = null
,va_AppellantAddressResponse = null
,va_AwardFiduciaryResponse = null
,va_BenefitClaimResponse = null
,va_CreateNoteResponse = null
,va_findaddressresponse = null
,va_FindAllRelationshipsResponse = null
,va_FindAppealsResponse = null
,va_FindAwardCompensationResponse = null
,va_FindBenefitDetailResponse = null
,va_findclaimantlettersresponse = null
,va_FindClaimStatusResponse = null
,va_FindContentionsResponse = null
,va_FindDenialsResponse = null
,va_FindDependentsResponse = null
,va_finddevelopmentnotesresponse = null
,va_FindFiduciaryPOAResponse = null
,va_findGetDocumentList = null
,va_FindIncomeExpenseResponse = null
,va_FindIndividualAppealsResponse = null
,va_FindMonthOfDeathResponse = null
,va_FindOtherAwardInformationResponse = null
,va_FindPaymentHistoryResponse = null
,va_FindRatingDataResponse = null
,va_findReasonsByRbaIssueIdResponse = null
,va_findtrackeditemsresponse = null
,va_FindUnsolvedEvidenceResponse = null
,va_FindVeteranResponse = null
,va_GetRegistrationStatus = null
,va_IsAliveResponse = null
,va_ReadDataAppointmentResponse = null
,va_ReadDataExamResponse = null
,va_RetrievePaymentDetailResponse = null
,va_RetrievePaymentSummaryResponse = null
,va_UpdateAppellantAddressResponse = null
 FROM dbo.PhoneCallBase AS ext
 INNER JOIN dbo.ActivityPointerBase AS base ON ext.ActivityId = base.ActivityId and base.ActivityTypeCode = 4210
 where base.ActivityTypeCode = 4210 
	and base.CreatedOn < @affectedDate
	and base.statecode != 0

go