SELECT serreq.udo_reqnumber
	,CASE 
		WHEN LEN(serreq.udo_filenumber) = 9
			AND serreq.udo_dateofdeath IS NOT NULL
			THEN 'XSS'
		ELSE ''
		END + CASE 
		WHEN LEN(serreq.udo_filenumber) < 9
			AND serreq.udo_dateofdeath IS NOT NULL
			THEN 'XC'
		ELSE ''
		END + CASE 
		WHEN LEN(serreq.udo_filenumber) < 9
			AND serreq.udo_dateofdeath IS NULL
			THEN 'C'
		ELSE ''
		END + CASE 
		WHEN LEN(serreq.udo_filenumber) = 9
			AND serreq.udo_dateofdeath IS NULL
			THEN 'CSS'
		ELSE ''
		END AS IDprefix
	,serreq.udo_FileNumber
	,CASE LEN(serreq.udo_filenumber)
		WHEN 9
			THEN SUBSTRING(serreq.udo_filenumber, 1, 3) + ' ' + SUBSTRING(serreq.udo_filenumber, 4, 2) + ' ' + SUBSTRING(serreq.udo_filenumber, 6, 4)
		WHEN 8
			THEN SUBSTRING(serreq.udo_filenumber, 1, 2) + ' ' + SUBSTRING(serreq.udo_filenumber, 3, 3) + ' ' + SUBSTRING(serreq.udo_filenumber, 6, 3)
		WHEN 7
			THEN SUBSTRING(serreq.udo_filenumber, 1, 1) + ' ' + SUBSTRING(serreq.udo_filenumber, 2, 3) + ' ' + SUBSTRING(serreq.udo_filenumber, 5, 3)
		WHEN 6
			THEN SUBSTRING(serreq.udo_filenumber, 1, 3) + ' ' + SUBSTRING(serreq.udo_filenumber, 4, 3)
		ELSE NULL
		END AS VetFileNum
	,CONVERT(VARCHAR, serreq.udo_InjuryDiseaseDisabledDate, 101) AS InjuryDate
	,CONVERT(VARCHAR, serreq.udo_BlindnessEffectiveDate, 101) AS BlindDate
	,CONVERT(VARCHAR, serreq.udo_LimbLossEffectiveDate, 101) AS LimbLossDate
	,udo_evaluationconsideredpermanentPLTable.Value AS Evaluation
	,ISNULL(UPPER(SUBSTRING(vet.FirstName, 1, 1)) + SUBSTRING(LOWER(vet.FirstName), 2, LEN(vet.FirstName) - 1), '') + ' ' + ISNULL(UPPER(SUBSTRING(vet.MiddleName, 1, 1)), '') + (
		CASE 
			WHEN vet.middlename IS NULL
				THEN ''
			ELSE ' '
			END
		) + ISNULL(UPPER(SUBSTRING(vet.LastName, 1, 1)) + SUBSTRING(LOWER(vet.LastName), 2, LEN(vet.LastName) - 1), '') AS c_fullname
	,serreq.udo_SSN AS VetSSN
	,serreq.udo_ClaimNumber AS ClaimNumber
	,UPPER(serreq.udo_mailing_address1) AS VetAddress1
	,UPPER(serreq.udo_mailing_address2) AS VetAddress2
	,UPPER(serreq.udo_mailing_address3) AS VetAddress3
	,UPPER(serreq.udo_mailing_city) AS VetCity
	,UPPER(serreq.udo_mailing_state) AS VetState
	,serreq.udo_mailing_zip AS VetZip
	,CASE UPPER(serreq.udo_mailingcountry)
		WHEN 'US'
			THEN NULL
		WHEN 'USA'
			THEN NULL
		WHEN 'U.S.'
			THEN NULL
		WHEN 'U.S.A.'
			THEN NULL
		WHEN 'UNITED STATES'
			THEN NULL
		WHEN 'UNITED STATES OF AMERICA'
			THEN NULL
		ELSE UPPER(serreq.udo_mailingcountry)
		END AS VetCountry
	,serreq.udo_BranchofService AS VetBranch
	,CONVERT(VARCHAR, serreq.udo_DateofDeath, 107) AS VetDOD
	,serreq.udo_Description AS SRdescription
	,serreq.udo_MilitaryServiceBranch AS SRbranch
	,serreq.udo_MilitaryServiceEODDate AS SReoddate
	,serreq.udo_MilitaryServiceRADDate AS SRraddate
	,serreq.udo_RatingDegree AS SRratingdegree
	,serreq.udo_RatingEffectiveDate AS SRratingeffectivedate
	,DATENAME(MM, GETDATE()) + RIGHT(CONVERT(VARCHAR(12), GETDATE(), 107), 9) AS CurrentDate
	,serreq.udo_DependentAmount AS DependentAmount
	,serreq.udo_NetAmountPaid AS NetAmount
	,serreq.udo_PaymentAmount AS PaymentAmount
	,serreq.udo_AAAmount AS AAAmount
	,serreq.udo_PensionBenefitAmount AS PensionBenefit
	,serreq.udo_CurrentMonthlyRate AS CurrentMonthlyRate
	,serreq.udo_EffectiveDate AS EffectiveDate
	,serreq.udo_FutureExamDate AS FutureExamDate
	,u.va_ReplyReferTo
	,u.va_FileNumber AS OwnerFileNum
	,vet.LastName + ' ' + UPPER(SUBSTRING(vet.FirstName, 1, 1)) + ' ' + UPPER(SUBSTRING(ISNULL(vet.MiddleName, ''), 1, 1)) AS ReplyName
	,u.ParentSystemUserIdName AS OwnerManager
	,CASE 
		WHEN serreq.udo_LetterAddressing = 953850001
			THEN pc.va_Alias
		ELSE ic.va_Alias
		END AS SOJname
	,CASE 
		WHEN serreq.udo_LetterAddressing = 953850001
			THEN pc.va_Address1
		ELSE ic.va_Address1
		END AS SOJaddress1
	,CASE 
		WHEN serreq.udo_LetterAddressing = 953850001
			THEN pc.va_Address2
		ELSE ic.va_Address2
		END AS SOJaddress2
	,CASE 
		WHEN serreq.udo_LetterAddressing = 953850001
			THEN pc.va_Address3
		ELSE ic.va_Address3
		END AS SOJaddress3
	,CASE 
		WHEN serreq.udo_LetterAddressing = 953850001
			THEN pc.va_City
		ELSE ic.va_City
		END AS SOJcity
	,CASE 
		WHEN serreq.udo_LetterAddressing = 953850001
			THEN pc.va_State
		ELSE ic.va_State
		END AS SOJstate
	,CASE 
		WHEN serreq.udo_LetterAddressing = 953850001
			THEN pc.va_ZipCode
		ELSE ic.va_ZipCode
		END AS SOJzip
	,CASE 
		WHEN serreq.udo_LetterAddressing = 953850001
			THEN pc.va_FaxNumber
		ELSE ic.va_FaxNumber
		END AS SOJfax
	,CASE 
		WHEN serreq.udo_LetterAddressing = 953850001
			THEN pc.va_LocalFax
		ELSE ic.va_LocalFax
		END AS SOJlocalfax
	,CASE 
		WHEN serreq.udo_LetterAddressing = 953850001
			THEN pc.udo_returnmailingaddress
		ELSE ic.udo_returnmailingaddress
		END AS SOJreturnmailingaddress
	,serreq.udo_LetterAddressing
	,s.Address1_Line1 AS CCaddress1
	,s.Address1_Line2 AS CCaddress2
	,s.Address1_City AS CCcity
	,s.Address1_StateOrProvince AS CCstate
	,s.Address1_PostalCode AS CCzip
	,manager.FirstName AS ManagerFirst
	,manager.LastName AS ManagerLast
	,manager.Title AS ManagerTitle
	,'RO Director' AS ManagerTitleLine1
	,'VA Regional Office' AS ManagerTitleLine2
	,serreq.udo_Enclosures AS Enclosures
	,serreq.udo_FirstName AS ContactFirstName
	,serreq.udo_LastName AS ContactLastName
	,udo_contactprefixPLTable.Value AS Prefix
	,UPPER(serreq.udo_Address1) AS ContactAddress1
	,UPPER(serreq.udo_Address2) AS ContactAddress2
	,UPPER(serreq.udo_Address3) AS ContactAddress3
	,UPPER(serreq.udo_City) AS ContactCity
	,UPPER(serreq.udo_State) AS ContactState
	,serreq.udo_ZipCode AS ContactZip
	,CASE UPPER(serreq.udo_country)
		WHEN 'US'
			THEN NULL
		WHEN 'USA'
			THEN NULL
		WHEN 'U.S.'
			THEN NULL
		WHEN 'U.S.A.'
			THEN NULL
		WHEN 'UNITED STATES'
			THEN NULL
		WHEN 'UNITED STATES OF AMERICA'
			THEN NULL
		ELSE UPPER(serreq.udo_country)
		END AS ContactCountry
	,serreq.udo_DepFirstName + ' ' + serreq.udo_DepLastName AS DepName
	,serreq.udo_CharacterOfDischarge AS DischargeTypes
	,serreq.udo_DisabilityList AS Disabilities
	,serreq.udo_DisabilityPercentages AS DisabilityPercent
	,udo_lostalimborblindPLTable.Value AS LostLimbOrBlind
	,serreq.udo_Discharge AS Discharge
	,serreq.udo_ServiceDates AS ServiceDates
	,udo_diedinactivedutyPLTable.Value AS DiedInActiveDuty
	,udo_diedduetoserviceconnecteddisabilityPLTable.Value AS DiedToDisability
	,udo_serviceconnecteddisabilityPLTable.Value AS DisabilityInd
	,udo_receivedsahorshagrantPLTable.Value AS ReceivedGrant
	,udo_entitledtohigherlevelofdisabilityPLTable.Value AS EntitledToHigherDisability
	,serreq.udo_BenefitType AS BenefitType
	,serreq.udo_AwardBenefitType AS AwardBenefitType
	,serreq.udo_PayDate AS PayDate
	,serreq.udo_FaxNumber AS FaxNum
	,serreq.udo_FaxDescription AS FaxDescription
	,serreq.udo_FaxNumberOfPages AS FaxPages
FROM udo_lettergeneration AS serreq WITH (NOLOCK)
INNER JOIN SystemUser AS u WITH (NOLOCK)
	ON serreq.udo_PCRofRecordId = u.SystemUserId
INNER JOIN Site AS s WITH (NOLOCK)
	ON u.SiteId = s.SiteId
LEFT OUTER JOIN va_regionaloffice AS soj WITH (NOLOCK)
	ON serreq.udo_RegionalOfficeId = soj.va_regionalofficeId
LEFT OUTER JOIN Filteredva_intakecenter AS ic WITH (NOLOCK)
	ON soj.va_IntakeCenterId = ic.va_intakecenterid
LEFT OUTER JOIN Filteredva_pensioncenter AS pc WITH (NOLOCK)
	ON soj.va_PensionCenterId = pc.va_pensioncenterid
LEFT OUTER JOIN Contact AS vet WITH (NOLOCK)
	ON serreq.udo_RelatedVeteranId = vet.ContactId
LEFT OUTER JOIN SystemUser AS manager WITH (NOLOCK)
	ON u.ParentSystemUserId = manager.SystemUserId
LEFT OUTER JOIN StringMap AS udo_evaluationconsideredpermanentPLTable
	ON udo_evaluationconsideredpermanentPLTable.AttributeName = 'udo_evaluationconsideredpermanent'
		AND udo_evaluationconsideredpermanentPLTable.ObjectTypeCode IN (
			SELECT ObjectTypeCode
			FROM EntityView
			WHERE (Name = 'udo_lettergeneration')
			)
		AND udo_evaluationconsideredpermanentPLTable.AttributeValue = serreq.udo_EvaluationConsideredPermanent
LEFT OUTER JOIN StringMap AS udo_contactprefixPLTable
	ON udo_contactprefixPLTable.AttributeName = 'udo_contactprefix'
		AND udo_contactprefixPLTable.ObjectTypeCode IN (
			SELECT ObjectTypeCode
			FROM EntityView
			WHERE (Name = 'udo_lettergeneration')
			)
		AND udo_contactprefixPLTable.AttributeValue = serreq.udo_ContactPrefix
LEFT OUTER JOIN StringMap AS udo_lostalimborblindPLTable
	ON udo_lostalimborblindPLTable.AttributeName = 'udo_lostalimborblind'
		AND udo_lostalimborblindPLTable.ObjectTypeCode IN (
			SELECT ObjectTypeCode
			FROM EntityView
			WHERE (Name = 'udo_lettergeneration')
			)
		AND udo_lostalimborblindPLTable.AttributeValue = serreq.udo_LostaLimborBlind
LEFT OUTER JOIN StringMap AS udo_diedinactivedutyPLTable
	ON udo_diedinactivedutyPLTable.AttributeName = 'udo_diedinactiveduty'
		AND udo_diedinactivedutyPLTable.ObjectTypeCode IN (
			SELECT ObjectTypeCode
			FROM EntityView
			WHERE (Name = 'udo_lettergeneration')
			)
		AND udo_diedinactivedutyPLTable.AttributeValue = serreq.udo_DiedinActiveDuty
LEFT OUTER JOIN StringMap AS udo_diedduetoserviceconnecteddisabilityPLTable
	ON udo_diedduetoserviceconnecteddisabilityPLTable.AttributeName = 'udo_diedduetoserviceconnecteddisability'
		AND udo_diedduetoserviceconnecteddisabilityPLTable.ObjectTypeCode IN (
			SELECT ObjectTypeCode
			FROM EntityView
			WHERE (Name = 'udo_lettergeneration')
			)
		AND udo_diedduetoserviceconnecteddisabilityPLTable.AttributeValue = serreq.udo_DiedDuetoServiceConnectedDisability
LEFT OUTER JOIN StringMap AS udo_serviceconnecteddisabilityPLTable
	ON udo_serviceconnecteddisabilityPLTable.AttributeName = 'udo_serviceconnecteddisability'
		AND udo_serviceconnecteddisabilityPLTable.ObjectTypeCode IN (
			SELECT ObjectTypeCode
			FROM EntityView
			WHERE (Name = 'udo_lettergeneration')
			)
		AND udo_serviceconnecteddisabilityPLTable.AttributeValue = serreq.udo_ServiceConnectedDisability
LEFT OUTER JOIN StringMap AS udo_receivedsahorshagrantPLTable
	ON udo_receivedsahorshagrantPLTable.AttributeName = 'udo_receivedsahorshagrant'
		AND udo_receivedsahorshagrantPLTable.ObjectTypeCode IN (
			SELECT ObjectTypeCode
			FROM EntityView
			WHERE (Name = 'udo_lettergeneration')
			)
		AND udo_receivedsahorshagrantPLTable.AttributeValue = serreq.udo_ReceivedSAHorSHAgrant
LEFT OUTER JOIN StringMap AS udo_entitledtohigherlevelofdisabilityPLTable
	ON udo_entitledtohigherlevelofdisabilityPLTable.AttributeName = 'udo_entitledtohigherlevelofdisability'
		AND udo_entitledtohigherlevelofdisabilityPLTable.ObjectTypeCode IN (
			SELECT ObjectTypeCode
			FROM EntityView
			WHERE (Name = 'udo_lettergeneration')
			)
		AND udo_entitledtohigherlevelofdisabilityPLTable.AttributeValue = serreq.udo_EntitledtoHigherLevelofDisability
WHERE (serreq.udo_lettergenerationId = @LetterGenerationGUID)