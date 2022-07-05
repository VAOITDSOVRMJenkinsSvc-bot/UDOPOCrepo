-- Datasource 2
SELECT Name, SiteId
FROM Site
UNION ALL
   SELECT 'All' AS Name, NULL AS SiteId
ORDER BY Name





-- Datasource 1
SELECT va_bankaccount.CreatedOn
	,va_bankaccount.OwnerIdName
	,CASE 
		WHEN va_bankaccount.udo_CADDStatus = 752280000
			THEN 'Incomplete'
		WHEN va_bankaccount.udo_CADDStatus = 752280001
			THEN 'Find Bank'
		WHEN va_bankaccount.udo_CADDStatus = 752280002
			THEN 'Complete'
		WHEN va_bankaccount.udo_CADDStatus = 752280003
			THEN 'Submit'
		ELSE ''
		END AS udo_CADDStatus
	,CASE 
		WHEN va_bankaccount.udo_sectionsupdated = 752280000
			THEN 'Updated Address'
		WHEN va_bankaccount.udo_sectionsupdated = 752280001
			THEN 'Updated Account'
		WHEN va_bankaccount.udo_sectionsupdated = 752280002
			THEN 'Updated Both'
		WHEN va_bankaccount.udo_sectionsupdated = 752280003
			THEN 'n/a'
		ELSE ''
		END AS udo_sectionsupdated
	,SiteBase.Name AS Site
	,va_bankaccount.va_FailedIDProofing
	,va_bankaccount.va_GeneralChanged
	,va_bankaccount.va_MailingAddressChanged
	,va_bankaccount.va_PaymentAddressChanged
	,va_bankaccount.va_DepositAccountChanged
	,va_bankaccount.va_AppellantAddressChanged
	,va_bankaccount.va_UpdateAddressRequest
	,va_bankaccount.va_UpdateAddressResponse
FROM SiteBase
INNER JOIN SystemUserBase
	ON SiteBase.SiteId = SystemUserBase.SiteId
RIGHT OUTER JOIN va_bankaccount
	ON SystemUserBase.SystemUserId = va_bankaccount.OwningUser
WHERE (
		va_bankaccount.CreatedOn BETWEEN @Created_On_From
			AND @Created_On_To
		)
	AND (
		(@CADD_Status = - 1)
		OR (va_bankaccount.udo_CADDStatus = @CADD_Status)
		)
	AND (
		(@Section_Updated = - 1)
		OR (va_bankaccount.udo_SectionsUpdated = @Section_Updated)
		)
	AND (
		(@Site IS NULL)
		OR (SystemUserBase.SiteId = @Site)
		)
ORDER BY Site
	,va_bankaccount.CreatedOn DESC