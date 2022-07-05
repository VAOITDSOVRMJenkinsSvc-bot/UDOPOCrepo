SELECT
    serreq.udo_reqnumber,
    serreq.udo_srfirstname AS ContactFirstName,
    serreq.udo_srlastname AS ContactLastName,
    serreq.udo_faxnumber AS FaxNum,
    serreq.udo_faxdescription AS FaxDescription,
    serreq.udo_faxnumberofpages AS FaxPages
                
FROM 
    udo_lettergeneration AS serreq with (nolock)
                
Where serreq.udo_lettergenerationid=@LetterGenerationGUID