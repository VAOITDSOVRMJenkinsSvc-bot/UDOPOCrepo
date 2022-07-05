
SELECT        
	serreq.udo_reqnumber, 
		ISNULL(UPPER(SUBSTRING(vet.FirstName, 1, 1)),'') + ISNULL(SUBSTRING(LOWER(vet.FirstName), 2, LEN(vet.FirstName) - 1), '') + ' ' + ISNULL(UPPER(SUBSTRING(vet.LastName, 1, 1)),'') + ISNULL(SUBSTRING(LOWER(vet.LastName), 2, LEN(vet.LastName) - 1), '') 
	AS VetFullName, 
		ISNULL(UPPER(SUBSTRING(vet.LastName, 1, 1)) + SUBSTRING(LOWER(vet.LastName), 2, LEN(vet.LastName) - 1),'') + ' ' + ISNULL(UPPER(SUBSTRING(vet.FirstName, 1, 1)) + SUBSTRING(LOWER(vet.FirstName), 2, LEN(vet.FirstName) - 1), '') + ' ' + ISNULL(UPPER(SUBSTRING(vet.MiddleName, 1, 1))	+ SUBSTRING(LOWER(vet.MiddleName), 2, LEN(vet.FirstName) - 1), '') 
	AS VetNameLastFirstMid,
    serreq.udo_poadata AS POA,

    CASE LEN(serreq.udo_filenumber) 
		WHEN 9 
			THEN SUBSTRING(serreq.udo_filenumber, 1, 3) + ' ' + SUBSTRING(serreq.udo_filenumber, 4, 2) + ' ' + SUBSTRING(serreq.udo_filenumber, 6, 4) 
		WHEN 8 
			THEN SUBSTRING(serreq.udo_filenumber, 1, 2) + ' ' + SUBSTRING(serreq.udo_filenumber, 3, 3) + ' ' + SUBSTRING(serreq.udo_filenumber, 6, 3)
        WHEN 7 
			THEN SUBSTRING(serreq.udo_filenumber, 1, 1) + ' ' + SUBSTRING(serreq.udo_filenumber, 2, 3) + ' ' + SUBSTRING(serreq.udo_filenumber, 5, 3) 
		WHEN 6 
			THEN SUBSTRING(serreq.udo_filenumber, 1, 3) + ' ' + SUBSTRING(serreq.udo_filenumber, 4, 3) 
		ELSE NULL 
	END AS FileNumber, 
	
	CASE 
		WHEN LEN(serreq.udo_filenumber) = 9 AND serreq.udo_dateofdeath IS NOT NULL
			THEN 'XSS' 
		ELSE '' 
		END + CASE 
			WHEN LEN(serreq.udo_filenumber) < 9 AND serreq.udo_dateofdeath IS NOT NULL 
				THEN 'XC' 
			ELSE '' 
			END + CASE 
				WHEN LEN(serreq.udo_filenumber) < 9 AND serreq.udo_dateofdeath IS NULL 
					THEN 'C' 
					ELSE '' 
				END + CASE 
					WHEN LEN(serreq.udo_filenumber) = 9 AND serreq.udo_dateofdeath IS NULL 
						THEN 'CSS' 
						ELSE '' 
	END AS IDprefix,
	serreq.udo_SREmail AS VetEmail, 
	serreq.udo_mailing_address1 AS VetAddress1, 
	serreq.udo_mailing_address2 AS VetAddress2, 
	serreq.udo_mailing_address3 AS VetAddress3,
    serreq.udo_mailing_city AS VetCity, 
	serreq.udo_mailing_state AS VetState, 
	serreq.udo_mailing_zip AS VetZip, 
	
	CASE UPPER(serreq.udo_mailingcountry) 
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
	END AS VetCountry,
    serreq.udo_DayPhone AS DayPhone, 
	serreq.udo_EveningPhone AS EveningPhone, 
	CONVERT(VarChar, GETDATE(), 101) AS CurrentDate, 
	'Yes' AS VetVerified, 
	serreq.udo_Phone AS CallerPhone,
    serreq.udo_Description, 
	serreq.udo_FirstName AS CallerFirstName, 
	serreq.udo_LastName AS CallerLastName, 
	serreq.udo_Address1 AS CallerStreet1, 
	serreq.udo_Address2 AS CallerStreet2,
	serreq.udo_Address3 AS CallerStreet3, 
	serreq.udo_City AS CallerCity, 
	serreq.udo_State AS CallerState, 
	serreq.udo_ZipCode AS CallerZip, 
	
	CASE UPPER(serreq.udo_country) 
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
    END AS CallerCountry, 
	u.va_StationNumber, 
	u.FirstName AS UserFirstName, 
	u.LastName AS UserLastName, 
	u.SiteIdName AS UserOffice, 
	u.Title AS UserTitle, 
	u.va_ReplyReferTo AS ReplyRefer,
    
	CASE 
		WHEN serreq.udo_readscript = 1 
			THEN 'Yes' 
		ELSE 'No' 
	END AS ReadScript, 
	vet.FirstName AS firstname, 
	vet.LastName AS lastname, 
	vet.MiddleName AS middlename
FROM            
	udo_servicerequest AS serreq WITH (nolock) 
	INNER JOIN
         udo_interaction AS pc WITH (nolock) 
		ON serreq.udo_originatinginteractionid = pc.udo_interactionId 
	INNER JOIN
        SystemUser AS u WITH (nolock) 
		ON serreq.udo_pcrofrecordid = u.SystemUserId 
	LEFT OUTER JOIN
          Contact AS vet WITH (nolock) 
		  ON serreq.udo_RelatedVeteranId = vet.ContactId
WHERE (serreq.udo_servicerequestId = @ServiceRequestGUID)
        