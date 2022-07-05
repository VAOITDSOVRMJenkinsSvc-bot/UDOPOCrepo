SELECT
    serreq.udo_reqnumber,
		ISNULL(UPPER(substring(vet.firstname,1,1)), '') + ISNULL(SUBSTRING(LOWER(vet.firstname),2,len(vet.firstname)-1), '') + ' '+ ISNULL(UPPER(substring(vet.lastname,1,1)), '') + ISNULL(SUBSTRING(LOWER(vet.lastname),2,len(vet.lastname)-1), '') 
	AS VetFullName,
		ISNULL(UPPER(substring(vet.lastname,1,1)) + SUBSTRING(LOWER(vet.lastname),2,len(vet.lastname)-1), '') + ' ' + ISNULL(UPPER(substring(vet.firstname,1,1)) + SUBSTRING(LOWER(vet.firstname),2,len(vet.firstname)-1), '') + ' '+ ISNULL(UPPER(substring(vet.middlename,1,1)) + SUBSTRING(LOWER(vet.middlename),2,len(vet.firstname)-1), '') 
	AS VetNameLastFirstMid,
                
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

        CASE WHEN LEN(serreq.udo_filenumber) = 9 AND serreq.udo_dateofdeath IS NOT NULL 
			THEN 'XSS' 
			ELSE '' 
		END +
        CASE WHEN LEN(serreq.udo_filenumber) < 9 AND serreq.udo_dateofdeath IS NOT NULL 
			THEN 'XC' 
			ELSE '' 
		END +
        CASE WHEN LEN(serreq.udo_filenumber) < 9 AND serreq.udo_dateofdeath IS NULL 
			THEN 'C' 
			ELSE '' 
		END +
        CASE WHEN LEN(serreq.udo_filenumber) = 9 AND serreq.udo_dateofdeath IS NULL 
			THEN 'CSS' 
			ELSE '' 
		END
    AS IDprefix,
                
    --conditional full name *last first middle--
    CASE WHEN serreq.udo_eccssn IS NOT NULL 
		THEN serreq.udo_ecclastname + ' ' + serreq.udo_eccfirstname + ' ' + ISNULL(serreq.udo_eccmiddlename, '')
		ELSE serreq.udo_srlastname + ' ' + serreq.udo_srfirstname 
	END
    AS [119_LastFirstMiddle],
                                
    --Conditional Address 119-- 
    CASE WHEN serreq.udo_eccssn IS NOT NULL 
		THEN serreq.udo_eccaddress1 
		ELSE serreq.udo_mailing_address1 
	END
    AS [119_Addresss1],
                                
    CASE WHEN serreq.udo_eccssn IS NOT NULL 
		THEN serreq.udo_eccaddress2 
		ELSE serreq.udo_mailing_address2 
	END
    AS [119_Addresss2],
                
    CASE WHEN serreq.udo_eccssn IS NOT NULL 
		THEN serreq.udo_ecccity + ', ' + serreq.udo_eccstate + ' ' + serreq.udo_ecczip
		ELSE serreq.udo_mailing_city + ', ' + serreq.udo_mailing_state + ' ' + serreq.udo_mailing_zip 
	END
    AS [119_CityStateZip],
                
    CASE WHEN serreq.udo_eccssn IS NOT NULL 
		THEN serreq.udo_eccphone1 
		ELSE serreq.udo_dayphone 
	END
    AS [119_Phone],
                
    CASE UPPER(serreq.udo_mailingcountry) 
        WHEN 'US' THEN NULL
        WHEN 'USA' THEN NULL
        WHEN 'U.S.' THEN NULL
        WHEN 'U.S.A.' THEN NULL
        WHEN 'UNITED STATES' THEN NULL
        WHEN 'UNITED STATES OF AMERICA' THEN NULL
        ELSE UPPER(serreq.udo_mailingcountry) 
	END AS [SR_Country],
    Convert(VarChar, GetDate(), 101) AS CurrentDate,
    serreq.udo_description,
    CASE UPPER(serreq.udo_country) 
        WHEN 'US' THEN NULL
        WHEN 'USA' THEN NULL
        WHEN 'U.S.' THEN NULL
        WHEN 'U.S.A.' THEN NULL
        WHEN 'UNITED STATES' THEN NULL
        WHEN 'UNITED STATES OF AMERICA' THEN NULL
        ELSE UPPER(serreq.udo_ecccountry) 
	END AS [ECC_Country],
    u.firstname AS UserFirstName,
    UPPER(SUBSTRING(u.firstname,1,1)) AS UserFirstInitial,
    u.lastname AS UserLastName,
    u.siteidname AS UserOffice,
    u.title AS UserTitle,
    u.va_replyreferto AS ReplyRefer,
                
    serreq.udo_eccssn AS ECCSSN,
    serreq.udo_ecctitle AS ECCTitle

FROM 
    Filteredudo_lettergeneration AS serreq with (nolock)
    INNER JOIN FilteredSystemUser u with (nolock) 
		ON serreq.udo_pcrofrecordid = u.systemuserid
    LEFT JOIN FilteredContact vet with (nolock) 
		ON serreq.udo_relatedveteranid = vet.contactid             
WHERE serreq.udo_lettergenerationid = @LetterGenerationGUID