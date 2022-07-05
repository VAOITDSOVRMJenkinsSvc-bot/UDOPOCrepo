SELECT
    serreq.udo_reqnumber,
    CASE WHEN LEN(serreq.udo_filenumber) = 9 AND serreq.udo_dateofdeath IS NOT NULL THEN 'XSS' ELSE '' END +
    CASE WHEN LEN(serreq.udo_filenumber) < 9 AND serreq.udo_dateofdeath IS NOT NULL THEN 'XC' ELSE '' END +
    CASE WHEN LEN(serreq.udo_filenumber) < 9 AND serreq.udo_dateofdeath IS NULL THEN 'C' ELSE '' END +
    CASE WHEN LEN(serreq.udo_filenumber) = 9 AND serreq.udo_dateofdeath IS NULL THEN 'CSS' ELSE '' END
    AS IDprefix,
    serreq.udo_filenumber,
    CASE LEN(serreq.udo_filenumber) 
        WHEN 9 THEN SUBSTRING(serreq.udo_filenumber, 1,3) +' '+ SUBSTRING(serreq.udo_filenumber, 4,2) +' '+ SUBSTRING(serreq.udo_filenumber, 6,4)
		WHEN 8 THEN SUBSTRING(serreq.udo_filenumber, 1,2) +' '+ SUBSTRING(serreq.udo_filenumber, 3,3) +' '+ SUBSTRING(serreq.udo_filenumber, 6,3)
		WHEN 7 THEN SUBSTRING(serreq.udo_filenumber, 1,1) +' '+ SUBSTRING(serreq.udo_filenumber, 2,3) +' '+ SUBSTRING(serreq.udo_filenumber, 5,3)
		WHEN 6 THEN SUBSTRING(serreq.udo_filenumber, 1,3) +' '+ SUBSTRING(serreq.udo_filenumber, 4,3) 
		ELSE NULL 
	END AS VetFileNum,
                
    CONVERT(varchar, serreq.udo_injurydiseasedisableddate, 101) AS InjuryDate,
    CONVERT(varchar, serreq.udo_blindnesseffectivedate, 101) AS BlindDate,
    CONVERT(varchar, serreq.udo_limblosseffectivedate, 101) AS LimbLossDate,
    -- [udo_evaluationconsideredpermanentPLTable].Value AS Evaluation,
	serreq.udo_evaluationconsideredpermanent AS Evaluation,
    ISNULL(UPPER(substring(serreq.udo_srfirstname,1,1)) + SUBSTRING(LOWER(serreq.udo_srfirstname),2,len(serreq.udo_srfirstname)-1), '') + ' '+
    ISNULL(UPPER(substring(serreq.udo_srlastname,1,1)) + SUBSTRING(LOWER(serreq.udo_srlastname),2,len(serreq.udo_srlastname)-1), '') 
	AS c_fullname,
    serreq.udo_ssn AS VetSSN,
    serreq.udo_claimnumber AS ClaimNumber,
    UPPER(serreq.udo_mailing_address1) AS VetAddress1,
    UPPER(serreq.udo_mailing_address2) AS VetAddress2,
    UPPER(serreq.udo_mailing_address3) AS VetAddress3,
    UPPER(serreq.udo_mailing_city) AS VetCity,
    UPPER(serreq.udo_mailing_state) AS VetState,
    serreq.udo_mailing_zip AS VetZip,
    CASE UPPER(serreq.udo_mailingcountry) 
                    WHEN 'US' THEN NULL
                    WHEN 'USA' THEN NULL
                    WHEN 'U.S.' THEN NULL
                    WHEN 'U.S.A.' THEN NULL
                    WHEN 'UNITED STATES' THEN NULL
                    WHEN 'UNITED STATES OF AMERICA' THEN NULL
                    ELSE UPPER(serreq.udo_mailingcountry) END AS VetCountry,
    serreq.udo_branchofservice as VetBranch,
    CONVERT(varchar, serreq.udo_dateofdeath, 107) as VetDOD,
    serreq.udo_description as SRdescription,
    serreq.udo_militaryservicebranch as SRbranch,
    serreq.udo_militaryserviceeoddate as SReoddate,
    serreq.udo_militaryserviceraddate as SRraddate,
    serreq.udo_ratingdegree as SRratingdegree,
    serreq.udo_ratingeffectivedate as SRratingeffectivedate,
    DATENAME(MM, GETDATE()) + RIGHT(CONVERT(VARCHAR(12), GETDATE(), 107), 9) AS CurrentDate,
    serreq.udo_dependentamount AS DependentAmount,
    serreq.udo_netamountpaid AS NetAmount,
    serreq.udo_paymentamount AS PaymentAmount,
    serreq.udo_aaamount AS AAAmount,
    serreq.udo_pensionbenefitamount AS PensionBenefit,
    serreq.udo_currentmonthlyrate AS CurrentMonthlyRate,
    serreq.udo_effectivedate AS EffectiveDate,
    serreq.udo_futureexamdate AS FutureExamDate,
    
	u.va_replyreferto,
    u.va_filenumber AS OwnerFileNum,
    
	vet.lastname + ' ' + UPPER(substring(vet.firstname,1,1)) + ' '+ UPPER(substring(ISNULL(vet.middlename, ''),1,1)) 
	AS ReplyName,
    
	u.parentsystemuseridname AS OwnerManager,
         
    CASE WHEN serreq.udo_LetterAddressing = 953850001 THEN pc.va_Alias --Pension
                                    ELSE ic.va_Alias --Compensation
                    END AS SOJname,
    CASE WHEN serreq.udo_LetterAddressing = 953850001 THEN pc.va_Address1 --Pension
                                    ELSE ic.va_Address1 --Compensation
                    END AS SOJaddress1,
	CASE WHEN serreq.udo_LetterAddressing = 953850001 THEN pc.va_Address2 --Pension
                                    ELSE ic.va_Address2 --Compensation
                    END AS SOJaddress2,
	CASE WHEN serreq.udo_LetterAddressing = 953850001 THEN pc.va_Address3 --Pension
                                    ELSE ic.va_Address3 --Compensation
                    END AS SOJaddress3,
	CASE WHEN serreq.udo_LetterAddressing = 953850001 THEN pc.va_City --Pension
                                    ELSE ic.va_City --Compensation
                    END AS SOJcity,
	CASE WHEN serreq.udo_LetterAddressing = 953850001 THEN pc.va_State --Pension
                                    ELSE ic.va_State --Compensation
                    END AS SOJstate,
	CASE WHEN serreq.udo_LetterAddressing = 953850001 THEN pc.va_ZipCode --Pension
                                    ELSE ic.va_ZipCode --Compensation
                    END AS SOJzip,
	CASE WHEN serreq.udo_LetterAddressing = 953850001 THEN pc.va_FaxNumber --Pension
                                    ELSE ic.va_FaxNumber --Compensation
                    END AS SOJfax,
	CASE WHEN serreq.udo_LetterAddressing = 953850001 THEN pc.va_LocalFax --Pension
                                    ELSE ic.va_LocalFax --Compensation
                    END AS SOJlocalfax,
	CASE WHEN serreq.udo_LetterAddressing = 953850001 THEN pc.udo_returnmailingaddress 
									ELSE ic.udo_returnmailingaddress 
					END AS SOJreturnmailingaddress, 
                                
    serreq.udo_LetterAddressing AS udo_LetterAddressing,

    s.address1_line1 AS CCaddress1,
    s.address1_line2 AS CCaddress2,
    s.address1_city AS CCcity,
    s.address1_stateorprovince AS CCstate,
    s.address1_postalcode AS CCzip,
    manager.firstname AS ManagerFirst,
    manager.lastname AS ManagerLast,
    manager.title AS ManagerTitle,
                
                    --CASE
                    --             WHEN serreq.va_signatureblock = 953850000 THEN 'NCC Manager' --Compensation
                    --             ELSE 'NPCC Manager' --Pension
                    --END AS ManagerTitleLine1,

                    --CASE
                    --             WHEN serreq.va_signatureblock = 953850000 THEN 'VA Regional Office/National Call Center' --Compensation
                    --             ELSE 'VA Regional Office/National Pension Call Center' --Pension
                    --END AS ManagerTitleLine2,
                
    ManagerTitleLine1 = 'RO Director',
    ManagerTitleLine2 = 'VA Regional Office',

    serreq.udo_enclosures AS Enclosures,
    serreq.udo_srfirstname AS ContactFirstName,
    serreq.udo_srlastname AS ContactLastName,
    -- [udo_contactprefixPLTable].Value AS Prefix,
	serreq.udo_contactprefix AS Prefix,
    UPPER(serreq.udo_address1) AS ContactAddress1,
    UPPER(serreq.udo_address2) AS ContactAddress2,
    UPPER(serreq.udo_address3) AS ContactAddress3,
    UPPER(serreq.udo_city) AS ContactCity,
    UPPER(serreq.udo_state) AS ContactState,
    serreq.udo_zipcode AS ContactZip,
    CASE UPPER(serreq.udo_country) 
        WHEN 'US' THEN NULL
        WHEN 'USA' THEN NULL
        WHEN 'U.S.' THEN NULL
        WHEN 'U.S.A.' THEN NULL
        WHEN 'UNITED STATES' THEN NULL
        WHEN 'UNITED STATES OF AMERICA' THEN NULL
        ELSE UPPER(serreq.udo_country) 
	END AS ContactCountry,
    serreq.udo_srfirstname + ' ' + serreq.udo_srlastname AS DepName,
    serreq.udo_characterofdischarge AS DischargeTypes,
    serreq.udo_disabilitylist AS Disabilities,
    serreq.udo_disabilitypercentages AS DisabilityPercent,
    -- [udo_lostalimborblindPLTable].Value AS LostLimbOrBlind,
	serreq.udo_lostalimborblind AS LostLimbOrBlind,
    serreq.udo_discharge AS Discharge,
    serreq.udo_servicedates AS ServiceDates,
    -- [udo_diedinactivedutyPLTable].Value AS DiedInActiveDuty,
	serreq.udo_diedinactiveduty AS DiedInActiveDuty,
    -- [udo_diedduetoserviceconnecteddisabilityPLTable].Value AS DiedToDisability,
	serreq.udo_diedduetoserviceconnecteddisability AS DiedToDisability,
    -- [udo_serviceconnecteddisabilityPLTable].Value AS DisabilityInd,
	serreq.udo_serviceconnecteddisability AS DisabilityInd,
    -- [udo_receivedsahorshagrantPLTable].Value AS ReceivedGrant,
	serreq.udo_receivedsahorshagrant AS ReceivedGrant,
    -- [udo_entitledtohigherlevelofdisabilityPLTable].Value AS EntitledToHigherDisability,
	serreq.udo_entitledtohigherlevelofdisability AS EntitledToHigherDisability,
    serreq.udo_benefittype AS BenefitType,
    serreq.udo_awardbenefittype AS AwardBenefitType,
    serreq.udo_paydate AS PayDate,
    serreq.udo_faxnumber AS FaxNum,
    serreq.udo_faxdescription AS FaxDescription,
    serreq.udo_faxnumberofpages AS FaxPages
                
FROM udo_lettergeneration AS serreq with (nolock)
    INNER JOIN SystemUser u with (nolock) 
		ON serreq.udo_pcrofrecordid = u.systemuserid
    INNER JOIN Site s with (nolock) 
		ON u.siteid = s.siteid
    LEFT JOIN va_regionaloffice soj with (nolock) 
		ON serreq.udo_regionalofficeid = soj.va_regionalofficeid
    LEFT JOIN Filteredva_intakecenter ic with (nolock) 
		ON soj.va_IntakeCenterId = ic.va_intakecenterId
    LEFT JOIN Filteredva_pensioncenter pc with (nolock) 
		ON soj.va_PensionCenterId = pc.va_pensioncenterId
    LEFT JOIN Contact vet with (nolock) 
		ON serreq.udo_relatedveteranid = vet.contactid
    LEFT JOIN SystemUser manager with (nolock) 
		ON u.parentsystemuserid = manager.systemuserid
                
        
		-- UDODEVTN: Below is for getting optionset values and names, we will not use this.
		left outer join StringMap [udo_evaluationconsideredpermanentPLTable] on 
        ([udo_evaluationconsideredpermanentPLTable].AttributeName = 'udo_evaluationconsideredpermanent'
        and [udo_evaluationconsideredpermanentPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='udo_lettergeneration')
        and [udo_evaluationconsideredpermanentPLTable].AttributeValue = [serreq].[udo_evaluationconsideredpermanent])           
        
        left outer join StringMap [udo_contactprefixPLTable] on 
        ([udo_contactprefixPLTable].AttributeName = 'udo_contactprefix'
        and [udo_contactprefixPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='udo_lettergeneration')
        and [udo_contactprefixPLTable].AttributeValue = [serreq].[udo_contactprefix])      

        left outer join StringMap [udo_lostalimborblindPLTable] on 
        ([udo_lostalimborblindPLTable].AttributeName = 'udo_lostalimborblind'
        and [udo_lostalimborblindPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='udo_lettergeneration') 
        and [udo_lostalimborblindPLTable].AttributeValue = [serreq].[udo_lostalimborblind])          

        left outer join StringMap [udo_diedinactivedutyPLTable] on 
        ([udo_diedinactivedutyPLTable].AttributeName = 'udo_diedinactiveduty'
        and [udo_diedinactivedutyPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='udo_lettergeneration') 
        and [udo_diedinactivedutyPLTable].AttributeValue = [serreq].[udo_diedinactiveduty])        

        left outer join StringMap [udo_diedduetoserviceconnecteddisabilityPLTable] on 
        ([udo_diedduetoserviceconnecteddisabilityPLTable].AttributeName = 'udo_diedduetoserviceconnecteddisability'
        and [udo_diedduetoserviceconnecteddisabilityPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='udo_lettergeneration') 
        and [udo_diedduetoserviceconnecteddisabilityPLTable].AttributeValue = [serreq].[udo_diedduetoserviceconnecteddisability])   

        left outer join StringMap [udo_serviceconnecteddisabilityPLTable] on 
        ([udo_serviceconnecteddisabilityPLTable].AttributeName = 'udo_serviceconnecteddisability'
        and [udo_serviceconnecteddisabilityPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='udo_lettergeneration') 
        and [udo_serviceconnecteddisabilityPLTable].AttributeValue = [serreq].[udo_serviceconnecteddisability])
                
                                left outer join StringMap [udo_receivedsahorshagrantPLTable] on 
        ([udo_receivedsahorshagrantPLTable].AttributeName = 'udo_receivedsahorshagrant'
        and [udo_receivedsahorshagrantPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='udo_lettergeneration') 
        and [udo_receivedsahorshagrantPLTable].AttributeValue = [serreq].[udo_receivedsahorshagrant])
                
                                left outer join StringMap [udo_entitledtohigherlevelofdisabilityPLTable] on 
        ([udo_entitledtohigherlevelofdisabilityPLTable].AttributeName = 'udo_entitledtohigherlevelofdisability'
        and [udo_entitledtohigherlevelofdisabilityPLTable].ObjectTypeCode in (SELECT ObjectTypeCode FROM EntityView where Name='udo_lettergeneration') 
        and [udo_entitledtohigherlevelofdisabilityPLTable].AttributeValue = [serreq].[udo_entitledtohigherlevelofdisability])

                
Where serreq.udo_lettergenerationid=@LetterGenerationGUID