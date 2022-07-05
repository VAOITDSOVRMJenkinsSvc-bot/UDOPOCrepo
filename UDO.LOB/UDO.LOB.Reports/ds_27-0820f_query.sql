SELECT
	serreq.udo_reqnumber,
	ISNULL(UPPER(substring(vet.firstname,1,1)), '') + ISNULL(SUBSTRING(LOWER(vet.firstname),2,len(vet.firstname)-1), '') + ' '+
	ISNULL(UPPER(substring(vet.lastname,1,1)), '') + ISNULL(SUBSTRING(LOWER(vet.lastname),2,len(vet.lastname)-1), '') AS VetFullName,
	
	ISNULL(UPPER(substring(vet.lastname,1,1)) + SUBSTRING(LOWER(vet.lastname),2,len(vet.lastname)-1), '') + ' ' +
	ISNULL(UPPER(substring(vet.firstname,1,1)) + SUBSTRING(LOWER(vet.firstname),2,len(vet.firstname)-1), '') + ' '+
	ISNULL(UPPER(substring(vet.middlename,1,1)) + SUBSTRING(LOWER(vet.middlename),2,len(vet.firstname)-1), '') AS VetNameLastFirstMid,
	
	serreq.udo_mailing_address1 + ' ' +
	ISNULL(serreq.udo_mailing_address2, '') + ' '+
	ISNULL(serreq.udo_mailing_address3, '') AS DependentAddress,

	CASE WHEN serreq.udo_haspoa = 1 THEN 
		CASE WHEN serreq.udo_poadata IS NOT NULL  
				  AND SUBSTRING(serreq.udo_poadata, 0, 6) = 'Name:'  
				  AND CHARINDEX(CHAR(10), serreq.udo_poadata) <> 0
			THEN SUBSTRING(serreq.udo_poadata, 7, CHARINDEX(CHAR(10), serreq.udo_poadata)-7)
		ELSE '' END 
	ELSE '' END AS POA,
	
	CASE LEN(serreq.udo_filenumber) 
	 WHEN 9 THEN SUBSTRING(serreq.udo_filenumber, 1,3) +' '+
			     SUBSTRING(serreq.udo_filenumber, 4,2) +' '+ 
			     SUBSTRING(serreq.udo_filenumber, 6,4)
	 WHEN 8 THEN SUBSTRING(serreq.udo_filenumber, 1,2) +' '+
				 SUBSTRING(serreq.udo_filenumber, 3,3) +' '+ 
				 SUBSTRING(serreq.udo_filenumber, 6,3)
	 WHEN 7 THEN SUBSTRING(serreq.udo_filenumber, 1,1) +' '+
				 SUBSTRING(serreq.udo_filenumber, 2,3) +' '+ 
				 SUBSTRING(serreq.udo_filenumber, 5,3)
	 WHEN 6 THEN SUBSTRING(serreq.udo_filenumber, 1,3) +' '+
				 SUBSTRING(serreq.udo_filenumber, 4,3) ELSE NULL END AS FileNumber,

	CASE WHEN LEN(serreq.udo_filenumber) = 9 AND serreq.udo_dateofdeath IS NOT NULL THEN 'XSS' ELSE '' END +
	CASE WHEN LEN(serreq.udo_filenumber) < 9 AND serreq.udo_dateofdeath IS NOT NULL THEN 'XC' ELSE '' END +
	CASE WHEN LEN(serreq.udo_filenumber) < 9 AND serreq.udo_dateofdeath IS NULL THEN 'C' ELSE '' END +
	CASE WHEN LEN(serreq.udo_filenumber) = 9 AND serreq.udo_dateofdeath IS NULL THEN 'CSS' ELSE '' END
	AS IDprefix,
	
	serreq.udo_email AS VetEmail,
	serreq.udo_mailing_address1 AS VetAddress1,
	serreq.udo_mailing_address2 AS VetAddress2,
	serreq.udo_mailing_address3 AS VetAddress3,
	serreq.udo_mailing_city AS VetCity,
	serreq.udo_mailing_state AS VetState,
	serreq.udo_mailing_zip AS VetZip,
	CASE UPPER(serreq.udo_mailingcountry) 
		WHEN 'US' THEN NULL
		WHEN 'USA' THEN NULL
		WHEN 'U.S.' THEN NULL
		WHEN 'U.S.A.' THEN NULL
		WHEN 'UNITED STATES' THEN NULL
		WHEN 'UNITED STATES OF AMERICA' THEN NULL
		ELSE UPPER(serreq.udo_mailingcountry) END AS VetCountry,
	serreq.udo_dayphone AS DayPhone,
	serreq.udo_eveningphone AS EveningPhone,
	Convert(VarChar, GetDate(), 101) AS CurrentDate,
	--CASE WHEN pc.va_calleridentityverified = 1 
	--		OR pc.va_filessnverified = 1 
	--		OR pc.va_bosverified = 1 THEN 'Yes' ELSE 'No' END 
			'Yes' AS VetVerified,
	serreq.udo_phone AS CallerPhone,
	serreq.udo_description,
	serreq.udo_firstname AS CallerFirstName,
	serreq.udo_lastname AS CallerLastName,
	serreq.udo_address1 AS CallerStreet1,
	serreq.udo_address2 AS CallerStreet2,
	serreq.udo_address3 AS CallerStreet3,
	serreq.udo_city AS CallerCity,
	serreq.udo_state AS CallerState,
	serreq.udo_zipcode AS CallerZip,
	CASE UPPER(serreq.udo_country) 
		WHEN 'US' THEN NULL
		WHEN 'USA' THEN NULL
		WHEN 'U.S.' THEN NULL
		WHEN 'U.S.A.' THEN NULL
		WHEN 'UNITED STATES' THEN NULL
		WHEN 'UNITED STATES OF AMERICA' THEN NULL
		ELSE UPPER(serreq.udo_country) END AS CallerCountry,
	u.va_stationnumber,
	u.firstname AS UserFirstName,
	u.lastname AS UserLastName,
	u.siteidname AS UserOffice,
	u.title AS UserTitle,
	u.va_replyreferto AS ReplyRefer,

	[udo_ReadScriptPLTable].Value AS ReadScript,
	
	vet.firstname AS firstname,
	vet.lastname AS lastname,
	vet.middlename AS middlename,
	
	[udo_relationtoveteranPLTable].Value AS VetRelation,
	Convert(VarChar, serreq.udo_dateofdeath, 101) AS DOD,
	[udo_enroutetovaPLTable].Value AS EnRoute,
	serreq.udo_placeofdeath AS PlaceOfDeath,
	serreq.udo_dependentnames AS DependentNames,
	serreq.udo_dependentaddresses AS DependentAddresses,
	ISNULL(serreq.udo_benefitsstopped,0) AS BeneStopped,
	ISNULL(serreq.udo_lookedupvetrecord,0) AS VetLookedUp,
	ISNULL(serreq.udo_deathrelatedinformationchecklists,0) AS DeathChecklist,
	ISNULL(serreq.udo_processedfnodinshare,0) AS ProcessedFnod,
	serreq.udo_processedfnodinshareexplanation AS ProcessedFnodExp,
	ISNULL(serreq.udo_pmc,0) AS PMC,
	ISNULL(serreq.udo_nokletter,0) AS NOKLetter,
	ISNULL(serreq.udo_21530,0) AS _21530,
	ISNULL(serreq.udo_21534,0) AS _21534,
	ISNULL(serreq.udo_401330,0) AS _401330,
	ISNULL(serreq.udo_other,0) AS Other,
	serreq.udo_otherspecification AS OtherSpec,
	[udo_benefitsstopfirstofmonthPLTable].Value AS BenefitsStopped,
	[udo_possibleburialinnationalcemeteryPLTable].Value AS BurialBenefits,
	[udo_willroutereportofdeathPLTable].Value AS WillRouteReport,
	
	--Convert(VarChar, serreq.udo_dateofmissingpayment, 101) AS DateOfMissingPayment,
	--serreq.udo_amountofpayments AS AmountOfPayment,
	--serreq.udo_paymentissuedvianame AS PaymentIssuedVia,
	--serreq.udo_paymentmethodname AS PaymentMethod,
	--serreq.udo_typeofpaymentname AS TypeOfPayment,
	--serreq.udo_checkendorsedandlostname AS CheckLost,
	--serreq.udo_checkendorsedandstolenname AS CheckStolen,
	--serreq.udo_addresschangedname AS AddressChanged,

	serreq.udo_description AS ActionToBeCompleted,
	serreq.udo_srfirstname AS DependentFirstName,
	serreq.udo_srlastname AS DependentLastName,
	serreq.udo_srdobtext AS DependentDOB,
	serreq.udo_srssn AS DependentSSN,
	serreq.udo_mailing_city AS DependentCity,
	serreq.udo_mailing_state AS DependentState,
	serreq.udo_mailing_zip AS DependentZip,
	serreq.udo_deceasedisnvb AS DeceasedNVB,
	serreq.udo_namenvb AS NameNVB,
	--serreq.udo_fnodreportingforname AS FNODReportingFor,
	serreq.udo_payment AS Payment
FROM 
	udo_servicerequest  AS serreq with (nolock)
	INNER JOIN udo_interaction  pc with (nolock) ON serreq.udo_originatinginteractionid = pc.udo_interactionId
	INNER JOIN SystemUser u with (nolock) ON serreq.udo_pcrofrecordid = u.systemuserid
	LEFT JOIN Contact vet with (nolock) ON serreq.udo_relatedveteranid = vet.contactid
		
		-- UDODEVTN: BELOW is used to get option set values and names, we will not use this. 
		left outer join StringMap [udo_ReadScriptPLTable] on 
        ([udo_ReadScriptPLTable].AttributeName = 'udo_readscript'
        and [udo_ReadScriptPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='udo_servicerequest')
        and [udo_ReadScriptPLTable].AttributeValue = [serreq].[udo_ReadScript])	
        
        left outer join StringMap [udo_relationtoveteranPLTable] on 
        ([udo_relationtoveteranPLTable].AttributeName = 'udo_relationtoveteran'
        and [udo_relationtoveteranPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='udo_servicerequest')
        and [udo_relationtoveteranPLTable].AttributeValue = [serreq].[udo_relationtoveteran])	

        left outer join StringMap [udo_enroutetovaPLTable] on 
        ([udo_enroutetovaPLTable].AttributeName = 'udo_enroutetova'
        and [udo_enroutetovaPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='udo_servicerequest') 
        and [udo_enroutetovaPLTable].AttributeValue = [serreq].[udo_enroutetova])	

        left outer join StringMap [udo_benefitsstopfirstofmonthPLTable] on 
        ([udo_benefitsstopfirstofmonthPLTable].AttributeName = 'udo_benefitsstopfirstofmonth'
        and [udo_benefitsstopfirstofmonthPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='udo_servicerequest') 
        and [udo_benefitsstopfirstofmonthPLTable].AttributeValue = [serreq].[udo_benefitsstopfirstofmonth])	

        left outer join StringMap [udo_possibleburialinnationalcemeteryPLTable] on 
        ([udo_possibleburialinnationalcemeteryPLTable].AttributeName = 'udo_possibleburialinnationalcemetery'
        and [udo_possibleburialinnationalcemeteryPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='udo_servicerequest') 
        and [udo_possibleburialinnationalcemeteryPLTable].AttributeValue = [serreq].[udo_possibleburialinnationalcemetery])	

        left outer join StringMap [udo_willroutereportofdeathPLTable] on 
        ([udo_willroutereportofdeathPLTable].AttributeName = 'udo_willroutereportofdeath'
        and [udo_willroutereportofdeathPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='udo_servicerequest') 
        and [udo_willroutereportofdeathPLTable].AttributeValue = [serreq].[udo_willroutereportofdeath])
	
WHERE serreq.udo_servicerequestid = @ServiceRequestGUID