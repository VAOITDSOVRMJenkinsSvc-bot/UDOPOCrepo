SELECT
	serreq.udo_reqnumber,
	ISNULL(UPPER(substring(vet.firstname,1,1)), '') + ISNULL(SUBSTRING(LOWER(vet.firstname),2,len(vet.firstname)-1), '') + ' '+
	ISNULL(UPPER(substring(vet.lastname,1,1)), '') + ISNULL(SUBSTRING(LOWER(vet.lastname),2,len(vet.lastname)-1), '') 
	AS VetFullName,
	
	ISNULL(UPPER(substring(vet.lastname,1,1)) + SUBSTRING(LOWER(vet.lastname),2,len(vet.lastname)-1), '') + ' ' +
	ISNULL(UPPER(substring(vet.firstname,1,1)) + SUBSTRING(LOWER(vet.firstname),2,len(vet.firstname)-1), '') + ' '+
	ISNULL(UPPER(substring(vet.middlename,1,1)) + SUBSTRING(LOWER(vet.middlename),2,len(vet.firstname)-1), '') 
	AS VetNameLastFirstMid,

	CASE WHEN serreq.udo_haspoa = 1 
		THEN 
		CASE WHEN serreq.udo_poadata IS NOT NULL  
				  AND SUBSTRING(serreq.udo_poadata, 0, 6) = 'Name:'  
				  AND CHARINDEX(CHAR(10), serreq.udo_poadata) <> 0
			THEN SUBSTRING(serreq.udo_poadata, 7, CHARINDEX(CHAR(10), serreq.udo_poadata)-7)
			ELSE '' END 
		ELSE '' 
	END AS POA,
	
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
				 SUBSTRING(serreq.udo_filenumber, 4,3) 
	 ELSE NULL 
	END AS FileNumber,

	CASE WHEN LEN(serreq.udo_filenumber) = 9 AND serreq.udo_dateofdeath IS NOT NULL THEN 'XSS' ELSE '' END +
	CASE WHEN LEN(serreq.udo_filenumber) < 9 AND serreq.udo_dateofdeath IS NOT NULL THEN 'XC' ELSE '' END +
	CASE WHEN LEN(serreq.udo_filenumber) < 9 AND serreq.udo_dateofdeath IS NULL THEN 'C' ELSE '' END +
	CASE WHEN LEN(serreq.udo_filenumber) = 9 AND serreq.udo_dateofdeath IS NULL THEN 'CSS' ELSE '' END
	AS IDprefix,
	
	serreq.udo_sremail AS VetEmail,
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
		ELSE UPPER(serreq.udo_mailingcountry) 
	END AS VetCountry,
	serreq.udo_dayphone AS DayPhone,
	serreq.udo_eveningphone AS EveningPhone,
	Convert(VarChar, GetDate(), 101) AS CurrentDate,
	--CASE WHEN pc.va_calleridentityverified = 1 
	--		OR pc.va_filessnverified = 1 
	--		OR pc.va_bosverified = 1 THEN 'Yes' ELSE 'No' END AS VetVerified,
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
		ELSE UPPER(serreq.udo_country) 
	END AS CallerCountry,
	u.va_stationnumber,
	u.firstname AS UserFirstName,
	u.lastname AS UserLastName,
	u.siteidname AS UserOffice,
	u.title AS UserTitle,
	u.va_replyreferto AS ReplyRefer,

	-- [va_ReadScriptPLTable].Value AS ReadScript,
	serreq.udo_ReadScript AS ReadScript,
	
	vet.firstname AS firstname,
	vet.lastname AS lastname,
	vet.middlename AS middlename,
	
	--[va_relationtoveteranPLTable].Value AS VetRelation,
	serreq.udo_relationtoveteran AS VetRelation,

	Convert(VarChar, serreq.udo_dateofdeath, 101) AS DOD,

	-- [va_enroutetovaPLTable].Value AS EnRoute,
	serreq.udo_EnroutetoVA AS EnRoute,

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

	-- [va_benefitsstopfirstofmonthPLTable].Value AS BenefitsStopped,
	serreq.udo_benefitsstopfirstofmonth AS BenefitsStopped,
	-- [va_possibleburialinnationalcemeteryPLTable].Value AS BurialBenefits,
	serreq.udo_possibleburialinnationalcemetery AS BurialBenefits,
	-- [va_willroutereportofdeathPLTable].Value AS WillRouteReport,
	serreq.udo_willroutereportofdeath AS WillRouteReport,

	Convert(VarChar, serreq.udo_dateofmissingpayment, 101) AS DateOfMissingPayment,
	serreq.udo_amtofpayments AS AmountOfPayment,

	-- [va_paymentissuedviaPLTable].Value AS PaymentIssuedVia,
	serreq.udo_paymentissuedvia AS PaymentIssuedVia,
	-- [va_paymentmethodPLTable].Value AS PaymentMethod,
	serreq.udo_paymentmethod AS PaymentMethod,
	-- [va_typeofpaymentPLTable].Value AS TypeOfPayment,
	serreq.udo_typeofpayment AS TypeOfPayment,
	-- [va_checkendorsedandlostPLTable].Value AS CheckLost,
	serreq.udo_checkendorsedandlost AS CheckLost,
	-- [va_checkendorsedandstolenPLTable].Value AS CheckStolen,
	serreq.udo_checkendorsedandstolen AS CheckStolen,
	-- [va_addresschangedPLTable].Value AS AddressChanged,
	serreq.udo_addresschanged AS AddressChanged,

	serreq.udo_description AS ActionToBeCompleted,
	serreq.udo_depfirstname AS DependentFirstName,
	serreq.udo_deplastname AS DependentLastName,
	Convert(VarChar, serreq.udo_depdateofbirth, 101)  AS DependentDOB,
	serreq.udo_depssn AS DependentSSN,
	serreq.udo_depaddress AS DependentAddress,
	serreq.udo_depcity AS DependentCity,
	serreq.udo_depstate AS DependentState,
	serreq.udo_depzipcode AS DependentZip,
	serreq.udo_deceasedisnvb AS DeceasedNVB,
	serreq.udo_namenvb AS NameNVB,

	-- [va_fnodreportingforPLTable].Value AS FNODReportingFor,
	serreq.udo_fnodreportingfor AS FNODReportingFor,

	serreq.udo_payment AS Payment
FROM 
	udo_servicerequest  AS serreq with (nolock)
	INNER JOIN udo_interaction  pc with (nolock) 
		ON serreq.udo_originatinginteractionid = pc.udo_interactionId
	INNER JOIN SystemUser u with (nolock) 
		ON serreq.udo_pcrofrecordid = u.systemuserid
	LEFT JOIN Contact vet with (nolock) 
		ON serreq.udo_relatedveteranid = vet.contactid

		-- BELOW is getting the label for the option set we will replace with field for name of option set
		left outer join StringMap [va_ReadScriptPLTable] on 
        ([va_ReadScriptPLTable].AttributeName = 'va_readscript'
        and [va_ReadScriptPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='va_servicerequest')
        and [va_ReadScriptPLTable].AttributeValue = [serreq].[udo_ReadScript])	
        
        left outer join StringMap [va_relationtoveteranPLTable] on 
        ([va_relationtoveteranPLTable].AttributeName = 'va_relationtoveteran'
        and [va_relationtoveteranPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='va_servicerequest')
        and [va_relationtoveteranPLTable].AttributeValue = [serreq].[udo_relationtoveteran])	

        left outer join StringMap [va_enroutetovaPLTable] on 
        ([va_enroutetovaPLTable].AttributeName = 'va_enroutetova'
        and [va_enroutetovaPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='va_servicerequest') 
        and [va_enroutetovaPLTable].AttributeValue = [serreq].[udo_EnroutetoVA])	

        left outer join StringMap [va_benefitsstopfirstofmonthPLTable] on 
        ([va_benefitsstopfirstofmonthPLTable].AttributeName = 'va_benefitsstopfirstofmonth'
        and [va_benefitsstopfirstofmonthPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='va_servicerequest') 
        and [va_benefitsstopfirstofmonthPLTable].AttributeValue = [serreq].[udo_benefitsstopfirstofmonth])	

        left outer join StringMap [va_possibleburialinnationalcemeteryPLTable] on 
        ([va_possibleburialinnationalcemeteryPLTable].AttributeName = 'va_possibleburialinnationalcemetery'
        and [va_possibleburialinnationalcemeteryPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='va_servicerequest') 
        and [va_possibleburialinnationalcemeteryPLTable].AttributeValue = [serreq].[udo_possibleburialinnationalcemetery])	

        left outer join StringMap [va_willroutereportofdeathPLTable] on 
        ([va_willroutereportofdeathPLTable].AttributeName = 'va_willroutereportofdeath'
        and [va_willroutereportofdeathPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='va_servicerequest') 
        and [va_willroutereportofdeathPLTable].AttributeValue = [serreq].[udo_willroutereportofdeath])
        
        left outer join StringMap [va_fnodreportingforPLTable] on 
        ([va_fnodreportingforPLTable].AttributeName = 'va_fnodreportingfor'
        and [va_fnodreportingforPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='va_servicerequest') 
        and [va_fnodreportingforPLTable].AttributeValue = [serreq].[udo_fnodreportingfor])
        
        left outer join StringMap [va_paymentissuedviaPLTable] on 
        ([va_paymentissuedviaPLTable].AttributeName = 'va_paymentissuedvia'
        and [va_paymentissuedviaPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='va_servicerequest') 
        and [va_paymentissuedviaPLTable].AttributeValue = [serreq].[udo_paymentissuedvia])
        
        left outer join StringMap [va_paymentmethodPLTable] on 
        ([va_paymentmethodPLTable].AttributeName = 'va_paymentmethod'
        and [va_paymentmethodPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='va_servicerequest') 
        and [va_paymentmethodPLTable].AttributeValue = [serreq].[udo_paymentmethod])
        
        left outer join StringMap [va_typeofpaymentPLTable] on 
        ([va_typeofpaymentPLTable].AttributeName = 'va_typeofpayment'
        and [va_typeofpaymentPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='va_servicerequest') 
        and [va_typeofpaymentPLTable].AttributeValue = [serreq].[udo_typeofpayment])
        
        left outer join StringMap [va_checkendorsedandlostPLTable] on 
        ([va_checkendorsedandlostPLTable].AttributeName = 'va_checkendorsedandlost'
        and [va_checkendorsedandlostPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='va_servicerequest') 
        and [va_checkendorsedandlostPLTable].AttributeValue = [serreq].[udo_checkendorsedandlost])
        
        left outer join StringMap [va_checkendorsedandstolenPLTable] on 
        ([va_checkendorsedandstolenPLTable].AttributeName = 'va_checkendorsedandstolen'
        and [va_checkendorsedandstolenPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='va_servicerequest') 
        and [va_checkendorsedandstolenPLTable].AttributeValue = [serreq].[udo_checkendorsedandstolen])
	
		left outer join StringMap [va_addresschangedPLTable] on 
        ([va_addresschangedPLTable].AttributeName = 'va_addresschanged'
        and [va_addresschangedPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='va_servicerequest') 
        and [va_addresschangedPLTable].AttributeValue = [serreq].[udo_addresschanged])
	

WHERE serreq.udo_servicerequestid = @ServiceRequestGUID