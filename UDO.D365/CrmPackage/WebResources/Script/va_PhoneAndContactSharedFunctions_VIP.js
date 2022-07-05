/**
 * Created by VHAISDBLANCW on 2/20/2015.
 */
/// <reference path="../Intellisense/XrmPage-vsdoc.js" />
/// <reference path="PhoneCall_Onload.js" />
/// <reference path="ContactPCRForm.js" />

_ViewContact = null;
_IntentToFile = null;
_claimStatus = null;
_paymentHistoryStatus = null;
_responseAttributes = null;
_responseAttributesWithAggregation = null;  // allow multiple nodes of same type eg Contentions
_male = 953850000;
_female = 953850001;
_unknown = 953850002;
noteId = null;
_birlsResult = false;
_searchCounter = 0;

/***************Phone Call***************/
var ClaimantInfo = {};
ClaimantInfo.AddressLine1 = "";
ClaimantInfo.AddressLine2 = "";
ClaimantInfo.AddressLine3 = "";
ClaimantInfo.Email = "";
ClaimantInfo.City = "";
ClaimantInfo.State = "";
ClaimantInfo.Zip = "";
ClaimantInfo.Country = "";
ClaimantInfo.FirstName = "";
ClaimantInfo.LastName = "";
ClaimantInfo.MiddleInitial = "";
ClaimantInfo.ParticipantId = "";
ClaimantInfo.Ssn = "";
ClaimantInfo.MilitaryPostalTypeCode = "";
ClaimantInfo.MilitaryPostOfficeTypeCode = "";

var VeteranInfo = {};
VeteranInfo.Ssn = "";
VeteranInfo.FileNumber = "";

var exCon = null;
var formContext = null;

function IntentToFile(selection) {
    getClaimantInfo();
    var formType = Xrm.Page.ui.getFormType();

    var callSubTypeOptionSet = Xrm.Page.data.entity.attributes.get("va_dispositionsubtype");
    var callSubType = callSubTypeOptionSet.getText();
    if (callSubType == "Claim:Intent To File") {
        if (formType == 2) {
            var canCreateItf = true;//CanCreateIntentToFile();
            if (canCreateItf) {
                var id = Xrm.Page.data.entity.getId();
                var name = Xrm.Page.getAttribute('subject').getValue();
                var va_ssn = VeteranInfo.Ssn;
                var va_veteranfilenumber = VeteranInfo.FileNumber;
                var va_firstname = Xrm.Page.getAttribute('va_firstname').getValue();
                var va_lastname = Xrm.Page.getAttribute('va_lastname').getValue();
                var va_middleinitial = Xrm.Page.getAttribute('va_middleinitial').getValue();
                var va_participantid = Xrm.Page.getAttribute('va_participantid').getValue();
                var va_identifycallerphone = Xrm.Page.getAttribute('va_identifycallerphone').getValue();
                var va_dobtext = Xrm.Page.getAttribute('va_dobtext').getValue();
                var va_callerrelationtoveteran = Xrm.Page.getAttribute("va_callerrelationtoveteran").getSelectedOption();

                if (selection.hasOwnProperty("firstname")) {
                    ClaimantInfo.FirstName = selection.firstname;
                    ClaimantInfo.LastName = selection.lastname;
                    ClaimantInfo.MiddleInitial = selection.middlename;
                    ClaimantInfo.ParticipantId = selection.ptcpntId;
                    ClaimantInfo.Ssn = selection.SSN;
                }

                if (va_ssn && va_firstname && va_lastname) {
                    if (va_participantid) {
                        var parameters = {};
                        parameters["va_phonecallid"] = id;
                        parameters["va_phonecallidname"] = name;
                        parameters["va_participantid"] = va_participantid;

                        if (va_callerrelationtoveteran && (va_callerrelationtoveteran.text == "Self" || va_callerrelationtoveteran.text == "VSO" ||
                            va_callerrelationtoveteran.text == "Fiduciary" || va_callerrelationtoveteran.text || "Other")) {
                            parameters["va_claimantparticipantid"] = va_participantid
                        }
                        if (va_firstname) {
                            parameters["va_veteranfirstname"] = va_firstname;
                            if (va_callerrelationtoveteran && va_callerrelationtoveteran.text == "Self") {
                                parameters["va_claimantfirstname"] = va_firstname;
                            }
                        }
                        if (va_lastname) {
                            parameters["va_veteranlastname"] = va_lastname;
                            if (va_callerrelationtoveteran && va_callerrelationtoveteran.text == "Self") {
                                parameters["va_claimantlastname"] = va_lastname;
                            }
                        }
                        if (va_middleinitial) {
                            parameters["va_veteranmiddleinitial"] = va_middleinitial;
                            if (va_callerrelationtoveteran && va_callerrelationtoveteran.text == "Self") {
                                parameters["va_claimantmiddleinitial"] = va_middleinitial;
                            }
                        }
                        if (va_ssn) {
                            parameters["va_veteranssn"] = va_ssn;
                            if (va_callerrelationtoveteran && va_callerrelationtoveteran.text == "Self") {
                                parameters["va_claimantssn"] = va_ssn;
                            }
                        }
                        if (va_veteranfilenumber) {
                            parameters["va_veteranfilenumber"] = va_veteranfilenumber;
                        }
                        if (va_identifycallerphone) {
                            parameters["va_veteranphone"] = va_identifycallerphone;
                        }
                        if (va_dobtext) {
                            parameters["va_veterandateofbirth"] = va_dobtext;
                        }

                        if (ClaimantInfo.AddressLine1) {
                            parameters["va_veteranaddressline1"] = ClaimantInfo.AddressLine1;
                        }
                        if (ClaimantInfo.AddressLine2) {
                            parameters["va_veteranaddressline2"] = ClaimantInfo.AddressLine2;
                        }
                        if (ClaimantInfo.AddressLine3) {
                            parameters["va_veteranunitnumber"] = ClaimantInfo.AddressLine3;
                        }
                        if (ClaimantInfo.City) {
                            parameters["va_veterancity"] = ClaimantInfo.City;
                        }
                        if (ClaimantInfo.State) {
                            parameters["va_veteranstate"] = ClaimantInfo.State;
                        }
                        if (ClaimantInfo.Zip) {
                            parameters["va_veteranzip"] = ClaimantInfo.Zip;
                        }
                        if (ClaimantInfo.Country) {
                            parameters["va_veterancountry"] = ClaimantInfo.Country;
                        }
                        if (ClaimantInfo.Email) {
                            parameters["va_veteranemail"] = ClaimantInfo.Email;
                        }
                        if (ClaimantInfo.Email) {
                            parameters["va_veteranemail"] = ClaimantInfo.Email;
                        }
                        if (ClaimantInfo.FirstName) {
                            parameters["va_claimantfirstname"] = ClaimantInfo.FirstName;
                        }
                        if (ClaimantInfo.LastName) {
                            parameters["va_claimantlastname"] = ClaimantInfo.LastName;
                        }
                        if (ClaimantInfo.MiddleInitial) {
                            parameters["va_claimantmiddleinitial"] = ClaimantInfo.MiddleInitial;
                        }
                        if (ClaimantInfo.ParticipantId) {
                            parameters["va_claimantparticipantid"] = ClaimantInfo.ParticipantId;
                        }
                        if (ClaimantInfo.Ssn) {
                            parameters["va_claimantssn"] = ClaimantInfo.Ssn;
                        }
                        if (ClaimantInfo.MilitaryPostalTypeCode) {
                            parameters["va_militarypostalcodevalue"] = ClaimantInfo.MilitaryPostalTypeCode;
                        }
                        if (ClaimantInfo.MilitaryPostOfficeTypeCode) {
                            parameters["va_militarypostofficetypecodevalue"] = ClaimantInfo.MilitaryPostOfficeTypeCode;
                        }
                        Xrm.Utility.openEntityForm("va_intenttofile", null, parameters);
                    }
                    else {
                        alert("Veteran Participant Id not found.")
                    }

                }
                else {
                    alert("Veteran not found. Please do a search.");
                }
            }
            else {
                alert("There is already an active Intent to File Claim in process");
            }
        }
        else if (formType == 1) {
            alert("Please start call.")
        }
    }
    else {
        alert("Please select Claim as the Call Type and Intent To File as the Call Subtype.")
    }
}
_IntentToFile = IntentToFile;

function SetContext() {
    exCon = GetLoadExecutionContext();
    formContext = exCon.getFormContext();
}

function getClaimantInfo() {
    var claimantXml = Xrm.Page.getAttribute('va_findcorprecordresponse').getValue();
    if (claimantXml != null) {
        claimantXmlObject = _XML_UTIL.parseXmlObject(claimantXml);
        ClaimantInfo.AddressLine1 = claimantXmlObject.selectSingleNode('//return/addressLine1') != null ? claimantXmlObject.selectSingleNode('//return/addressLine1').text : null;
        ClaimantInfo.AddressLine2 = claimantXmlObject.selectSingleNode('//return/addressLine2') != null ? claimantXmlObject.selectSingleNode('//return/addressLine2').text : null;
        ClaimantInfo.AddressLine3 = claimantXmlObject.selectSingleNode('//return/addressLine3') != null ? claimantXmlObject.selectSingleNode('//return/addressLine3').text : null;
        ClaimantInfo.City = claimantXmlObject.selectSingleNode('//return/city') != null ? claimantXmlObject.selectSingleNode('//return/city').text : null;
        ClaimantInfo.State = claimantXmlObject.selectSingleNode('//return/state') != null ? claimantXmlObject.selectSingleNode('//return/state').text : null;
        ClaimantInfo.Zip = claimantXmlObject.selectSingleNode('//return/zipCode') != null ? claimantXmlObject.selectSingleNode('//return/zipCode').text : null;
        ClaimantInfo.Country = claimantXmlObject.selectSingleNode('//return/country') != null ? claimantXmlObject.selectSingleNode('//return/country').text : null;
        ClaimantInfo.Email = claimantXmlObject.selectSingleNode('//return/emailAddress') != null ? claimantXmlObject.selectSingleNode('//return/emailAddress').text : null;
        ClaimantInfo.MilitaryPostOfficeTypeCode = claimantXmlObject.selectSingleNode('//return/militaryPostOfficeTypeCode') != null ? claimantXmlObject.selectSingleNode('//return/militaryPostOfficeTypeCode').text : null;
        ClaimantInfo.MilitaryPostalTypeCode = claimantXmlObject.selectSingleNode('//return/militaryPostalTypeCode') != null ? claimantXmlObject.selectSingleNode('//return/militaryPostalTypeCode').text : null;
    }
    var birlsXml = Xrm.Page.getAttribute('va_findbirlsresponse').getValue();
    if (birlsXml) {
        var birlsXmlObject = _XML_UTIL.parseXmlObject(birlsXml);
        VeteranInfo.Ssn = birlsXmlObject.selectSingleNode('//return/SOC_SEC_NUMBER') != null ? birlsXmlObject.selectSingleNode('//return/SOC_SEC_NUMBER').text : null;
        VeteranInfo.FileNumber = birlsXmlObject.selectSingleNode('//return/CLAIM_NUMBER') != null ? birlsXmlObject.selectSingleNode('//return/CLAIM_NUMBER').text : null;
    }
}

//=============================================
//  ViewContact()
//=============================================
function ViewContact(selection) {
    //ShowProgress('Retrieving Contact...');
    //
    var cols = ["va_SSN", "va_ParticipantID", "ContactId"];
    var filter = "";

    if (!selection.SSN || selection.SSN.length == 0) {
        filter = "va_ParticipantID eq '" + selection.ptcpntId + "'";
    } else {
        filter = "va_SSN eq '" + selection.SSN + "'";
    }
    var contacts = CrmRestKit2011.ByQueryAll("Contact", cols, filter);
    contacts.fail(function (err) {
        UTIL.restKitError(err, 'Failed to retrieve standard Contact response:');
    }
    )
        .done(function (data) {
            try {
                var rtContact = new contact();
                //getting values from the selected record of all the values we will store in CRM
                rtContact.contactAutoSearch = true;
                rtContact.firstName = selection.firstname;
                rtContact.middleName = selection.middlename;
                rtContact.lastName = selection.lastname;
                rtContact.ssn = selection.SSN;
                rtContact.participantId = selection.ptcpntId;
                rtContact.dod = selection.dod;
                rtContact.dobtext = selection.dobtext;
                rtContact.ICN = Xrm.Page.getAttribute('va_icn').getValue();
                rtContact.callerRelationToVeteran = (Xrm.Page.getAttribute('va_callerrelationtoveteran').getValue() != null) ? Xrm.Page.getAttribute('va_callerrelationtoveteran').getValue() : null;
                if (data && data.length > 0) {
                    //ShowProgress('Updating Contact...');
                    rtContact.id = data[0].ContactId;
                    //textValue = collection.results[0].FullName;
                    rtContact.update();
                    var url = GetServerUrl() + "/main.aspx?etc=2&id=%7b" + data[0].ContactId + "%7d&pagetype=entityrecord";
                    var win = window.open(url);
                    win.focus();
                } else {
                    //ShowProgress('Creating Contact...');
                    rtContact.contactAutoSearch = true;
                    //adding contact via RestKit
                    var contacts = rtContact.create();
                    //open newly added record
                    var url = GetServerUrl() + "/main.aspx?etc=2&id=%7b" + contacts.ContactId + "%7d&pagetype=entityrecord";
                    var width = 1024;
                    var height = 768;
                    var top = (screen.height - height) / 2;
                    var left = (screen.width - width) / 2;
                    var params = "width=" + width + ",height=" + height + ",location=0,menubar=0,toolbar=0,top=" + top + ",left=" + left + ",status=0,titlebar=no";
                    var win = window.open(url, 'Contact', params);
                    win.focus();
                }
            }
            catch (e) {
                //CloseProgress();
                alert("Error occurred.\n" + e.description);
            }
        });
}
_ViewContact = ViewContact;
//=============================================
//  HardcodeXmlResponse()
//=============================================
function HardcodeXmlResponse() { // skipMinSearchFields
    //   if (skipMinSearchFields != true) {
    //Corp Response
    xmlstring = '<return></return>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findcorprecordresponse"));

    //Birls Response
    xmlstring = '<return><INSURANCE_POLICY></INSURANCE_POLICY><SERVICE></SERVICE>'
        + '<ALTERNATE_NAME></ALTERNATE_NAME><FOLDER></FOLDER><FLASH></FLASH>'
        + '<SERVICEDIAGNOSTICS></SERVICEDIAGNOSTICS><RECURING_DISCLOSURE></RECURING_DISCLOSURE></return>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findbirlsresponse"));

    //Dependent Response
    xmlstring = '<persons></persons>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_finddependentsresponse"));

    //All Relations Response
    xmlstring = '<dependents></dependents>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findallrelationshipsresponse"));

    //General Information Response
    xmlstring = '<return><awardBenes></awardBenes></return>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_generalinformationresponse"));

    //General Information By PID Response
    xmlstring = '<return><evrs></evrs><flashes></flashes><diaries></diaries></return>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_generalinformationresponsebypid"));

    //Fudiciary Response
    xmlstring = '<return><currentFiduciary></currentFiduciary><currentPowerOfAttorney>'
        + '</currentPowerOfAttorney><pastFiduciaries></pastFiduciaries>'
        + '<pastPowerOfAttorneys></pastPowerOfAttorneys></return>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findfiduciarypoaresponse"));

    // Address
    xmlstring = '<return></return>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findaddressresponse"));

    // va_awardfiduciaryresponse
    xmlstring = '<return></return>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute('va_awardfiduciaryresponse'));
    //    }

    //Military Response
    var xmlstring = '<return><militaryReadjustmentBalances></militaryReadjustmentBalances>' +
        '<militaryReadjustmentPays></militaryReadjustmentPays><militaryRetirementPays></militaryRetirementPays>' +
        '<militarySeperationBalances></militarySeperationBalances><militarySeperationPays></militarySeperationPays>' +
        '<militarySeveranceBalances></militarySeveranceBalances><militaryPersonDecorations></militaryPersonDecorations>' +
        '<militaryPersonPows></militaryPersonPows><militaryPersonTours></militaryPersonTours><militaryPersons></militaryPersons>' +
        '<militarySeverancePays></militarySeverancePays><militaryTheatres></militaryTheatres></return>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findmilitaryrecordbyptcpntidresponse"));


    //MOD Response, Compensation Response,
    xmlstring = '<return></return>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findmonthofdeathresponse"));
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findawardcompensationresponse"));

    //Other Award Response
    xmlstring = '<awardReason></awardReason><receivable></receivable><deduction></deduction>' +
        '<accountBalance></accountBalance><awardLines></awardLines><awardInfo></awardInfo>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findotherawardinformationresponse"));

    //Denials Response
    xmlstring = '<denials></denials>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_finddenialsresponse"));

    //Tracked Items Response
    xmlstring = '<dvlpmtItems></dvlpmtItems>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findtrackeditemsresponse"));

    //Claimant Letters Response
    xmlstring = '<letters></letters>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findclaimantlettersresponse"));

    //Contentions Response
    xmlstring = '<contentions></contentions>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findcontentionsresponse"));

    //Payment History Response
    xmlstring = '<PaymentRecord></PaymentRecord><Payments></Payments><ReturnPayments></ReturnPayments>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findpaymenthistoryresponse"));


    //Benefit Claim Response
    xmlstring = '<selection></selection>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_benefitclaimresponse"));

    //Benefit Claim Details Response
    xmlstring = '<BenefitClaims><lifeCycleRecords></lifeCycleRecords><suspenceRecords></suspenceRecords></BenefitClaims>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findbenefitdetailresponse"));

    //Benefit Claim Status Response
    xmlstring = '<claimLifecycleStatusList></claimLifecycleStatusList>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findclaimstatusresponse"));

    //Development Notes Response
    xmlstring = '<notes></notes>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_finddevelopmentnotesresponse"));

    //Evidence Response
    xmlstring = '<UnsolicitedEvidence></UnsolicitedEvidence>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findunsolvedevidenceresponse"));

    //Rating Response
    xmlstring = '<return><deathRatingRecord><ratings></ratings></deathRatingRecord><disabilityRatingRecord><ratings></ratings>'
        + '</disabilityRatingRecord><familyMemberRatingRecord><ratings></ratings></familyMemberRatingRecord>'
        + '<otherRatingRecord><ratings></ratings></otherRatingRecord><specialMonthlyCompensationRatingRecord>'
        + '<smcParagraphRatings></smcParagraphRatings><smcRatings></smcRatings></specialMonthlyCompensationRatingRecord></return>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findratingdataresponse"));

    //Income/Expense Response
    xmlstring = '<return><incomeSummaryRecords><expenseRecords></expenseRecords>'
        + '<incomeRecords></incomeRecords></incomeSummaryRecords></return>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute("va_findincomeexpenseresponse"));

    //Appeals Response
    xmlstring = '<AppealIdentifier></AppealIdentifier>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute('va_findappealsresponse'));

    //Individual Appeals Response
    xmlstring = '<AppealRecord></AppealRecord><Appellant></Appellant><AppellantAddress></AppellantAddress>'
        + '<Issue></Issue><RemandReason></RemandReason><AppealVeteran></AppealVeteran><Diary></Diary>'
        + '<AppealDecision></AppealDecision><SpecialContentions></SpecialContentions><AppealDate></AppealDate>'
        + '<HearingRequest></HearingRequest>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute('va_findindividualappealsresponse'));

    //Denials - Reason by RBA Id
    xmlstring = '<return></return>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute('va_findreasonsbyrbaissueidresponse'));
    //Requests/Exams
    //    xmlstring = '<?xml version="1.0" encoding="UTF-8"?><examsdata:RequestsAndExamsData xmlns:examsdata="Examsdata"><templateId>RequestsAndExamsRead1</templateId><requestId>TEST_REQUEST_ID_1-1-1-2345-6999-0000</requestId><patients><patient><requestedNationalId>1234567890V123456</requestedNationalId><resultantIdentifiers><resultantIdentifier><identity>123</identity><assigningFacility>578</assigningFacility><assigningAuthority>USVHA</assigningAuthority></resultantIdentifier><resultantIdentifier><identity>456</identity><assigningFacility>580</assigningFacility><assigningAuthority>USVHA</assigningAuthority></resultantIdentifier></resultantIdentifiers><examRequests><examRequest><recordIdentifier><identity>45</identity><namespaceId>578_396.3</namespaceId></recordIdentifier><patient><identifier><identity>123</identity><assigningFacility>578</assigningFacility><assigningAuthority>USVHA</assigningAuthority></identifier><name><given>John</given><middle>H</middle><family>Doe</family></name></patient><requestDate><literal>20090612230027</literal></requestDate><regionalOfficeNumber><identifier><identity>500</identity><name>CAMP MASTER</name></identifier><shortName>CAMP</shortName><stationNumber>500</stationNumber><officialVAName>ALBANY VA MEDICAL CENTER</officialVAName></regionalOfficeNumber><requestor><identifier><identity>test_requestor_id</identity><assigningFacility>visn 6</assigningFacility></identifier><name><given>SOME_GIVEN_NAME</given><family>PROVIDER</family></name></requestor><dateReportedToMas><literal>20090612230027</literal></dateReportedToMas><dateSchedulingCompleted><literal>20090612230027</literal></dateSchedulingCompleted><dateCompleted><literal>20090618230027</literal></dateCompleted><priorityOfExam><code>E</code><displayText>Emergency</displayText></priorityOfExam><otherDisabilitiesLine1>KNEE</otherDisabilitiesLine1><otherDisabilitiesLine2>HIP</otherDisabilitiesLine2><otherDisabilitiesLine3>NECK</otherDisabilitiesLine3><transcriptionDate><literal>20090612230027</literal></transcriptionDate><dateApproved><literal>20090912230027</literal></dateApproved><dateReleased><literal>20090612230027</literal></dateReleased><releasedBy><identifier><identity>11289</identity><assigningFacility>visn18</assigningFacility></identifier><name><given>SeventySeven</given><family>Wardclerk</family></name></releasedBy><datePrintedByRO><literal>20090612230027</literal></datePrintedByRO><printedBy><identifier><identity>11289</identity><assigningFacility>visn18</assigningFacility></identifier><name><given>SeventySeven</given><family>Wardclerk</family></name></printedBy><status><code>O</code><displayText>OPEN</displayText></status><elapsedTime>0</elapsedTime><cancellationDate><literal>20090612230027</literal></cancellationDate><cancelledBy><given>Smith</given><middle>S</middle><family>Mary</family></cancelledBy><claimFolderRequired><code>N</code><displayText>No</displayText></claimFolderRequired><otherDocumentsRequired><code>N</code><displayText>No</displayText></otherDocumentsRequired><remarks>ONE REQUEST/ONE EXAM, DATE REPORTED EDITED TO BE AUG 15.TRANFER TO SMA (REMOTE SITE)... CHECK OUT 3-DAY CLOCK ONBOTH SIDES.  CHECK OUT REQUEST AND EXAM STATUS.  CAPTUREMAIL MESSAGES ALSO</remarks><lastExamAddDate><literal>20090609230027</literal></lastExamAddDate><lastPersonToAddExam><identifier><identity>11289</identity><assigningFacility>visn 6</assigningFacility></identifier><name><family>WardClerk</family></name></lastPersonToAddExam><remarksModificationDate><literal>20090609230027</literal></remarksModificationDate><remarksModifiedBy/><routingLocation><identity>1</identity><name>VEHU DIVISION</name></routingLocation><approvedBy><name><prefix>Sgt</prefix><family>Nagy</family></name></approvedBy><approvalDateTime><literal>20090609230027</literal></approvalDateTime><ownerDomain><code>262</code><displayText>Camp.Domain.Gov</displayText></ownerDomain><lastRatingExamDate><literal>20090609230027</literal></lastRatingExamDate><originalRequestPointer>25</originalRequestPointer><transferredToAnotherSite><code>Y</code><displayText>Yes</displayText></transferredToAnotherSite><dateTransferredToOtherSite><literal>20090609230027</literal></dateTransferredToOtherSite><dateTransferredInFromRemoteSite><literal>20090609230027</literal></dateTransferredInFromRemoteSite><dateAllTransfersReturned><literal>20090609230027</literal></dateAllTransfersReturned><original2507ProcessingTime>2</original2507ProcessingTime></examRequest></examRequests><exams><exam><recordIdentifier><identity>91</identity><namespaceId>580_396.4</namespaceId></recordIdentifier><examReferenceNumber>91</examReferenceNumber><patient><identifier><identity>456</identity><assigningFacility>580</assigningFacility><assigningAuthority>USVHA</assigningAuthority></identifier><name><given>Johnny</given><middle>H</middle><family>Doe</family></name></patient><request2507><identity>45</identity><namespaceId>578_396.3</namespaceId></request2507><examType><code>3</code><displayText>General Medical</displayText></examType><status><code>C</code><displayText>COMPLETED</displayText></status><workSheetPrinted><code>Y</code><displayText>Yes</displayText></workSheetPrinted><dateOfExam><literal>20090609230027</literal></dateOfExam><examiningPhysician><identifier><identity>565656</identity><assigningFacility>visn 8</assigningFacility></identifier><name><given>Bob</given><family>Hameed</family></name></examiningPhysician><feeExam><code>N</code><displayText>No</displayText></feeExam><examPlace><code>C</code><displayText>Clinic</displayText></examPlace><insufficientReason><code>5</code><displayText>FAILED TO ADDRESS ALL CONDITIONS REQUESTED</displayText></insufficientReason><originalProvider><identifier><identity>test_requestor_id</identity><assigningFacility>visn 6</assigningFacility></identifier><name><given>SOME_GIVEN_NAME</given><family>PROVIDER</family></name></originalProvider><cancellationDateTime><literal>20090609230027</literal></cancellationDateTime><cancelledBy><family>WardClerk</family></cancelledBy><cancellationReason><code>3</code><displayText>Failed To Report</displayText><codingSystem>String</codingSystem></cancellationReason><dateTransferredOut><literal>20090609230027</literal></dateTransferredOut><transferredOutBy><given>Linda</given><middle>M</middle><family>Provider</family></transferredOutBy><transferredOutTo><identity>47</identity><name>FO-ALBANY.MED.VA.GOV</name></transferredOutTo><dateTransferredIn><literal>20090609230027</literal></dateTransferredIn><dateReturnedToOwnerSite><literal>20090609230027</literal></dateReturnedToOwnerSite><insufficientRemarks>INSUFFICINET/CANCELLED/RE ENTERED BY RO AS INSUFFICIENT</insufficientRemarks><dateTranscriptionComplete><literal>20090609230027</literal></dateTranscriptionComplete></exam></exams></patient></patients><errorSection/></examsdata:RequestsAndExamsData>';
    xmlstring = '<examRequest></examRequest><exam></exam>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute('va_readdataexamresponse'));
    //Appointment
    //    xmlstring = '<?xml version="1.0" encoding="UTF-8"?><appointmentsdata:AppointmentsData xmlns:appointmentsdata="Appointmentsdata"><templateId>AppointmentsRead1</templateId><requestId>TEST_APPTS_REQUEST_ID_1-1-1-2345-6999-0012</requestId><patients><patient><requestedNationalId>1234567890V123456</requestedNationalId><resultantIdentifiers><resultantIdentifier><identity>123</identity><assigningFacility>578</assigningFacility><assigningAuthority>USVHA</assigningAuthority></resultantIdentifier><resultantIdentifier><identity>456</identity><assigningFacility>580</assigningFacility><assigningAuthority>USVHA</assigningAuthority></resultantIdentifier></resultantIdentifiers><appointments><appointment><recordIdentifier><identity>8</identity><namespaceId>578_409.68</namespaceId></recordIdentifier><patient><identifier><identity>123</identity><assigningFacility>578</assigningFacility><assigningAuthority>USVHA</assigningAuthority></identifier><name><given>John</given><middle>H</middle><family>Doe</family></name></patient><appointmentDateTime><literal>20090712230027</literal></appointmentDateTime><location><identifier><identity>23</identity><name>General Medicine</name></identifier><telephone>202-829-8787</telephone><institution><identifier><identity>500</identity><name>CAMP MASTER</name></identifier><shortName>CAMP</shortName><stationNumber>500</stationNumber><officialVAName>ALBANY VA MEDICAL CENTER</officialVAName></institution></location><appointmentStatus><code>14</code><displayText>ACTION REQUIRED</displayText></appointmentStatus><appointmentType><code>9</code><displayText>Regular</displayText></appointmentType><ekgDateTime><literal>20090618230027</literal></ekgDateTime><xrayDateTime><literal>20090618230027</literal></xrayDateTime><labDateTime><literal>20090619230027</literal></labDateTime><status><code>NS: NOT SURE WHAT THE VALUE SHOULD BE</code><displayText>NS: NOT SURE WHAT THE VALUE SHOULD BE</displayText></status></appointment></appointments></patient></patients><errorSection/></appointmentsdata:AppointmentsData>';
    xmlstring = '<appointment></appointment>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute('va_readdataappointmentresponse'));


    xmlstring = '<PaymentDetailResponse><awardAdjustments><awardAdjustmentList><awardAdjustmentVO></awardAdjustmentVO></awardAdjustmentList>'
        + '<awardReasonList><awardReasonVO></awardReasonVO></awardReasonList></awardAdjustments><paymentAdjustments><paymentAdjustmentList><paymentAdjustmentVO>'
        + '</paymentAdjustmentVO></paymentAdjustmentList></paymentAdjustments></PaymentDetailResponse>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute('va_retrievepaymentdetailresponse'));

    xmlstring = '<PaymentSummaryResponse><payments><payment></payment></payments><response></response></PaymentSummaryResponse>';
    HardcodeResponse(xmlstring, Xrm.Page.getAttribute('va_retrievepaymentsummaryresponse'));

}
function HardcodeResponse(xmlString, field) {
    if (field && field != undefined) {
        field.setValue(xmlString);
    }
}
//=============================================
//  LookupId()
//=============================================
function LookupId(attr) {
    if (attr && attr.getValue()) return attr.getValue()[0].id;
    return null;
}
//=============================================
//  performCrmAction()
//=============================================
function performCrmAction(actions) {
    var allKorrect = true;
    for (i in actions) {
        var action = actions[i];
        var res = action.performAction();
        if (!res) {
            allKorrect = false;
            break;
        }
        if (action.context.endState) {
            break;
        }
    }
    return allKorrect;
}
//=============================================
//  MarkAsRelatedVeteran()
//=============================================
function MarkAsRelatedVeteran(rtContact) {
    var idValue = null, textValue = null, searchIn = null, searchInValue = null, isContact = (Xrm.Page.data.entity.getEntityName() == 'contact');

    if (rtContact.isaveteran == '') {
        rtContact.isaveteran = false;
    }
    Xrm.Page.getAttribute('va_isaveteran').setValue(rtContact.isaveteran);
    Xrm.Page.getAttribute('va_isaveteran').setSubmitMode('always');

    rtContact.va_searchPathways = Xrm.Page.getAttribute('va_searchpathways').getValue();
    if (rtContact.va_searchPathways) {
        var fromDate = Xrm.Page.getAttribute('va_appointmentfromdate').getValue();
        var toDate = Xrm.Page.getAttribute('va_appointmenttodate').getValue();
        if (fromDate && fromDate != undefined) {
            rtContact.va_AppointmentFromDate = new Date(fromDate);
        }
        if (toDate && toDate != undefined) {
            rtContact.va_AppointmentToDate = new Date(toDate);
        }
    }

    rtContact.WebServiceExecutionStatus = Xrm.Page.getAttribute('va_webserviceexecutionstatus').getValue();
    rtContact.SearchOptionsObject = Xrm.Page.getAttribute('va_searchoptionsobject').getValue();
    rtContact.SearchActionsCompleted = Xrm.Page.getAttribute('va_searchactionscompleted').getValue();
    rtContact.LastExecutedSearch = Xrm.Page.getAttribute('va_lastexecutedsearch').getValue();

    // if Birls record didn't supply vet's sens. level, use PCR's level as default
    if (!_UserSettings) {
        _UserSettings = GetUserSettingsForWebservice(exCon);
    }
    if (!rtContact.sensitiveLevelOfRecord) rtContact.sensitiveLevelOfRecord = _UserSettings.pcrSensitivityLevel;
    if (!rtContact.sensitivityLevelValue) rtContact.sensitivityLevelValue = _UserSettings.pcrSensitivityLevel;
    // Update Phone with data returned by ws
    var participantId = '';
    if (rtContact.ssn) {
        Xrm.Page.getAttribute("va_ssn").setValue(rtContact.ssn);
    }
    if (rtContact.participantId && rtContact.participantId.length > 0) {
        participantId = rtContact.participantId;
        Xrm.Page.getAttribute("va_participantid").setValue(participantId);
    }
    if (rtContact.sensitiveLevelOfRecord) {
        Xrm.Page.getAttribute("va_veteransensitivitylevel").setValue(parseInt(rtContact.sensitiveLevelOfRecord));
        Xrm.Page.getAttribute("va_sensitivitylevelvalue").setValue(parseInt(rtContact.sensitiveLevelOfRecord));
        Xrm.Page.getAttribute("va_veteransensitivitylevel").setSubmitMode('always');
        Xrm.Page.getAttribute("va_sensitivitylevelvalue").setSubmitMode('always');
    }
    //Field names don't match, so.....
    if (isContact) {   //contact screen
        if (rtContact.firstName) {
            Xrm.Page.getAttribute('firstname').setValue(rtContact.firstName);
        }
        if (rtContact.lastName) {
            Xrm.Page.getAttribute('lastname').setValue(rtContact.lastName);
        }
        if (rtContact.middleName) {
            Xrm.Page.getAttribute('middlename').setValue(rtContact.middleName);
        }
        if (rtContact.email && !Xrm.Page.getAttribute("emailaddress1").getValue()) Xrm.Page.getAttribute("emailaddress1").setValue(rtContact.email);
        if (rtContact.dobtext && rtContact.dobtext != '') {
            Xrm.Page.getAttribute('va_dobtext').setValue(rtContact.dobtext);
        }
    } else {                      //phone call screen
        if (rtContact.firstName) {
            Xrm.Page.getAttribute('va_firstname').setValue(rtContact.firstName);
        }
        if (rtContact.lastName) {
            Xrm.Page.getAttribute('va_lastname').setValue(rtContact.lastName);
        }
        if (rtContact.middleName) {
            Xrm.Page.getAttribute('va_middleinitial').setValue(rtContact.middleName);
        }
        if (rtContact.email && !Xrm.Page.getAttribute("va_email").getValue()) Xrm.Page.getAttribute("va_email").setValue(rtContact.email);
        if (rtContact.dobtext && rtContact.dobtext != '') {
            Xrm.Page.getAttribute('va_dobtext').setValue(rtContact.dobtext);
            if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE) {
                if (Xrm.Page.getAttribute('va_identifycallerphone').getValue() == null) {
                    Xrm.Page.getAttribute('va_identifycallerphone').setValue(getPhoneNumberFromContact(rtContact));
                }
            }
        }
        if (rtContact.gender && Xrm.Page.getAttribute('va_genderset')) {
            var genderValue = _unknown;
            var genderStr = rtContact.gender;
            genderStr = genderStr.toUpperCase();
            if (genderStr == 'M') {
                genderValue = _male;
            }
            else if (genderStr == 'F') {
                genderValue = _female;
            }
            Xrm.Page.getAttribute('va_genderset').setValue(parseInt(genderValue));
        }

        Xrm.Page.getAttribute('phonenumber').setValue(getPhoneNumberFromContact(rtContact));

        //Set Header Folder Location, Format SOJ: XXX; RPO: XXX
        if ((rtContact.va_StaionofJurisdictionId) || (rtContact.rpo)) {
            var folderLocationText = '';
            //scenario 1: SOJ and RPO Exist
            if ((rtContact.va_StaionofJurisdictionId) && (rtContact.rpo)) {
                folderLocationText = 'SOJ: ' + rtContact.va_StaionofJurisdictionId + '; RPO: ' + rtContact.rpo;
            }
            //scenario 2: SOJ Exists and RPO Does not exist
            if ((rtContact.va_StaionofJurisdictionId) && (!rtContact.rpo)) {
                folderLocationText = 'SOJ: ' + rtContact.va_StaionofJurisdictionId;
            }
            //scenario 3:  SOJ does not exist, RPO does exist
            if ((!rtContact.va_StaionofJurisdictionId) && (rtContact.rpo)) {
                folderLocationText = 'RPO: ' + rtContact.rpo;
            }
            Xrm.Page.getAttribute("va_sojrpo").setValue(folderLocationText);
        }

        //Set Header ID field:
        if ((rtContact.ssn) || (rtContact.fileNumber) || (rtContact.CLAIM_NUMBER)) {
            //Declare IDs
            var IDssn;
            var IDclaim;
            var IDheader = '';

            if (rtContact.ssn) {
                IDssn = rtContact.ssn;
            }
                //            else if ((!IDssn) && (rtContact.socialSecurityNumber)) {
                //                IDssn = rtContact.socialSecurityNumber;
                //            }
            else {
                IDssn = 'n/a';
            }

            if (rtContact.fileNumber) {
                IDclaim = rtContact.fileNumber;
            }
            else if ((!IDclaim) && (rtContact.CLAIM_NUMBER)) {
                IDclaim = rtContact.CLAIM_NUMBER;
            }
            else {
                IDclaim = 'n/a';
            }

            //Set SSN and Claim #
            if ((IDssn) && (IDclaim)) {
                IDheader = "SSN: " + IDssn + "; Claim #: " + IDclaim;
            }

            Xrm.Page.getAttribute("va_headerid").setValue(IDheader);
        }
        //ECC: Add the RPO if ECC userRole
        //TODO: Change to use udo_CRMCommonJS
        _isECC = (UserHasRole("ECC Case Manager") || UserHasRole("ECC Phone Tech"));
        if ((_isECC == true) && (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE)) {
            //Check for EDU value
            var varOption = '';
            if (rtContact.rpo) {
                switch (rtContact.rpo) {
                    case '316': //ATL
                        varOption = '953850000';
                        break;
                    case '307': //BUF
                        varOption = '953850001';
                        break;
                    case '351': //MUS
                        varOption = '953850002';
                        break;
                    case '331': //STL
                        varOption = '953850003';
                        break;
                    default:  //N/A
                        varOption = '953850004';
                        break;
                }
                if (varOption) {
                    Xrm.Page.getAttribute("va_rpo").setValue(varOption);
                }
            }
            else {
                //No RPO, so set it to 'N/A'
                Xrm.Page.getAttribute("va_rpo").setValue(953850004);
            }
        }
    }
    Xrm.Page.getAttribute("va_branchofservice").setValue(rtContact.branchOfService);
    ShowFlagsAndTooltips(rtContact);
    var columns = ['va_SSN', 'FullName', 'ContactId', 'va_SMSPaymentNotices', 'va_EmailPaymentNotices', 'va_EmailClaimNotices',
        'PreferredContactMethodCode', 'PreferredAppointmentDayCode', 'PreferredAppointmentTimeCode', 'va_SelfServiceNotifications',
        'EMailAddress1', 'va_PrimaryPhone', 'va_HasEBenefitsAccount', 'va_TimeZone',
        'Address1_Telephone1', 'Address1_Telephone2', 'Address1_Telephone3', 'Address2_Telephone1', 'Address2_Telephone2'];
    var filter = participantId != '' ? "va_ParticipantID eq '" + participantId + "'" : "va_SSN eq '" + rtContact.ssn + "'";
    // only update XML response fields if allowed to cash responses
    if (_allowToCashXMLResponses) {
        rtContact.CorpResponse = Xrm.Page.getAttribute('va_findcorprecordresponse').getValue();
        rtContact.BirlsResponse = Xrm.Page.getAttribute('va_findbirlsresponse').getValue();
        rtContact.VeteranResponse = Xrm.Page.getAttribute("va_findveteranresponse").getValue();
        rtContact.GeneralInformationResponse = Xrm.Page.getAttribute("va_generalinformationresponse").getValue();
        rtContact.GeneralInformationResponsePK = Xrm.Page.getAttribute("va_generalinformationresponsebypid").getValue();
        rtContact.AddressResponse = Xrm.Page.getAttribute("va_findaddressresponse").getValue();
        rtContact.BenefitClaimResponse = Xrm.Page.getAttribute("va_benefitclaimresponse").getValue();
        rtContact.BenefitDetailResponse = Xrm.Page.getAttribute("va_findbenefitdetailresponse").getValue();
        rtContact.ClaimStatusResponse = Xrm.Page.getAttribute("va_findclaimstatusresponse").getValue();
        rtContact.ClaimantLettersResponse = Xrm.Page.getAttribute("va_findclaimantlettersresponse").getValue();
        rtContact.ContentionsResponse = Xrm.Page.getAttribute("va_findcontentionsresponse").getValue();
        rtContact.DependentsResponse = Xrm.Page.getAttribute("va_finddependentsresponse").getValue();
        rtContact.AllRelationsResponse = Xrm.Page.getAttribute("va_findallrelationshipsresponse").getValue();
        rtContact.DevNotesResponse = Xrm.Page.getAttribute("va_finddevelopmentnotesresponse").getValue();
        rtContact.FiduciaryPOAResponse = Xrm.Page.getAttribute("va_findfiduciarypoaresponse").getValue();
        rtContact.MilitaryRecordResponse = Xrm.Page.getAttribute("va_findmilitaryrecordbyptcpntidresponse").getValue();
        rtContact.PayHistoryResponse = Xrm.Page.getAttribute("va_findpaymenthistoryresponse").getValue();
        rtContact.TrackedItemsResponse = Xrm.Page.getAttribute('va_findtrackeditemsresponse').getValue();
        rtContact.UnsolvEvidenceResponse = Xrm.Page.getAttribute("va_findunsolvedevidenceresponse").getValue();
        rtContact.DenialsResponse = Xrm.Page.getAttribute('va_finddenialsresponse').getValue();
        rtContact.AwardCompResponse = Xrm.Page.getAttribute('va_findawardcompensationresponse').getValue();
        rtContact.OtherAwardInfoResponse = Xrm.Page.getAttribute('va_findotherawardinformationresponse').getValue();
        rtContact.MonthofDeathResponse = Xrm.Page.getAttribute('va_findmonthofdeathresponse').getValue();
        rtContact.IncomeExpenseResponse = Xrm.Page.getAttribute("va_findincomeexpenseresponse").getValue();
        rtContact.RatingDataResponse = Xrm.Page.getAttribute('va_findratingdataresponse').getValue();
        rtContact.AppealsResponse = Xrm.Page.getAttribute('va_findappealsresponse').getValue();
        rtContact.IndividualAppealsResponse = Xrm.Page.getAttribute('va_findindividualappealsresponse').getValue();
        rtContact.AppellantAddressResponse = Xrm.Page.getAttribute("va_appellantaddressresponse").getValue();
        rtContact.UpdateAppellantAddressResponse = Xrm.Page.getAttribute("va_updateappellantaddressresponse").getValue();
        rtContact.CreateNoteResponse = Xrm.Page.getAttribute("va_createnoteresponse").getValue();
        rtContact.ReasonsByRbaIssueIdResponse = Xrm.Page.getAttribute("va_findreasonsbyrbaissueidresponse").getValue();
        rtContact.IsAliveResponse = Xrm.Page.getAttribute("va_isaliveresponse").getValue();
        rtContact.MVIResponse = Xrm.Page.getAttribute("va_mviresponse").getValue();
        rtContact.ReadDataExamResponse = Xrm.Page.getAttribute('va_readdataexamresponse').getValue();
        rtContact.ReadDataAppointmentResponse = Xrm.Page.getAttribute('va_readdataappointmentresponse').getValue();
        rtContact.AwardFiduciaryResponse = Xrm.Page.getAttribute('va_awardfiduciaryresponse').getValue();
        rtContact.RetrievePaymentSummaryResponse = Xrm.Page.getAttribute('va_retrievepaymentsummaryresponse').getValue();
        rtContact.RetrievePaymentDetailResponse = Xrm.Page.getAttribute('va_retrievepaymentdetailresponse').getValue();
        rtContact.WSUpdateDate = Xrm.Page.getAttribute("va_webserviceresponse").getValue();
    }
    if (Xrm.Page.data.entity.getEntityName() == 'phonecall') {
        CallerRelationToVeteranOnClick(true);
    }
    //debugger;
    var soj = null, sojResponse = GetSOJ(null);
    if (sojResponse.id) {
        rtContact.va_StaionofJurisdictionId = sojResponse.id;
    }
    // find record by SSN or part. id
    var collectionResults = CrmRestKit2011.ByQueryAll('Contact', columns, filter);
    collectionResults.fail(function (err) {
        UTIL.restKitError(err, 'Failed to retrieve contact/ veteran response:');
    }
    )
        .done(function (collection) {
            if (collection && collection.length > 0) {
                idValue = collection[0].ContactId;
                textValue = collection[0].FullName;
                rtContact.id = idValue;

                rtContact.update();

                // show CRM-only contact fields in Outreach tab on Phone screen; load them from matching Contact
                Xrm.Page.getAttribute("va_smspaymentnotices").setValue(collection[0].va_SMSPaymentNotices);
                Xrm.Page.getAttribute("va_emailclaimnotices").setValue(collection[0].va_EmailClaimNotices);
                Xrm.Page.getAttribute("va_emailpaymentnotices").setValue(collection[0].va_EmailPaymentNotices);
                if (!isContact) {   //phone screen, contact=2
                    if (collection[0].PreferredContactMethodCode) Xrm.Page.getAttribute("va_preferredcontactmethod").setValue(collection[0].PreferredContactMethodCode.Value);
                    if (collection[0].PreferredAppointmentDayCode) Xrm.Page.getAttribute("va_preferredday").setValue(collection[0].PreferredAppointmentDayCode.Value);
                    if (collection[0].PreferredAppointmentTimeCode) Xrm.Page.getAttribute("va_preferredtime").setValue(collection[0].PreferredAppointmentTimeCode.Value);
                    if (collection[0].va_PrimaryPhone) {
                        Xrm.Page.getAttribute("va_preferredphonetype").setValue(collection[0].va_PrimaryPhone.Value);
                        // based on value, copy appropriate phone number
                        if (Xrm.Page.getAttribute("va_preferredphonetype").getSelectedOption()) {
                            switch (Xrm.Page.getAttribute("va_preferredphonetype").getSelectedOption().text) {
                                case 'Home':
                                    Xrm.Page.getAttribute("va_preferredphone").setValue(collection[0].Address1_Telephone1);
                                    break;
                                case 'Mobile':
                                    Xrm.Page.getAttribute("va_preferredphone").setValue(collection[0].Address1_Telephone2);
                                    break;
                                case 'Work':
                                    Xrm.Page.getAttribute("va_preferredphone").setValue(collection[0].Address1_Telephone3);
                                    break;
                                case 'International':
                                    Xrm.Page.getAttribute("va_preferredphone").setValue(collection[0].Address2_Telephone1);
                                    break;
                                case 'Emergency':
                                    Xrm.Page.getAttribute("va_preferredphone").setValue(collection[0].Address2_Telephone2);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                if (collection[0].va_SelfServiceNotifications) Xrm.Page.getAttribute("va_selfservicenotifications").setValue(collection[0].va_SelfServiceNotifications.Value);
                if (collection[0].va_TimeZone) Xrm.Page.getAttribute("va_timezone").setValue(collection[0].va_TimeZone.Value);
            }
            else {
                //create new contact and assign this contact to the caller
                // most of data comes from ws, but some fields (contact prefs) are set by PCR on Phone screen. Also could be updated during Save
                if (Xrm.Page.getAttribute("va_smspaymentnotices").getIsDirty()) rtContact.SMSPaymentNotices = Xrm.Page.getAttribute("va_smspaymentnotices").getValue();
                if (Xrm.Page.getAttribute("va_emailclaimnotices").getIsDirty()) rtContact.EmailClaimNotices = Xrm.Page.getAttribute("va_emailclaimnotices").getValue();
                if (Xrm.Page.getAttribute("va_emailpaymentnotices").getIsDirty()) rtContact.EmailPaymentNotices = Xrm.Page.getAttribute("va_emailpaymentnotices").getValue();
                if (!isContact) {
                    if (Xrm.Page.getAttribute("va_preferredcontactmethod").getIsDirty()) rtContact.PreferredMethodofContact = Xrm.Page.getAttribute("va_preferredcontactmethod").getValue();
                    if (Xrm.Page.getAttribute("va_preferredday").getIsDirty()) rtContact.PreferredDay = Xrm.Page.getAttribute("va_preferredday").getValue();
                    if (Xrm.Page.getAttribute("va_preferredtime").getIsDirty()) rtContact.PreferredTime = Xrm.Page.getAttribute("va_preferredtime").getValue();
                    if (collection && collection.length > 0 && collection[0].EMailAddress1 && (
                        !Xrm.Page.getAttribute("va_email").getIsDirty() || !Xrm.Page.getAttribute("va_email").getValue())) {
                        Xrm.Page.getAttribute("va_email").setValue(collection[0].EMailAddress1);
                    }
                    rtContact.email = Xrm.Page.getAttribute("va_email").getValue();
                    if (Xrm.Page.getAttribute("va_preferredphone").getIsDirty()) rtContact.PreferredPhone = Xrm.Page.getAttribute("va_preferredphone").getValue();
                    if (Xrm.Page.getAttribute("va_preferredphonetype").getIsDirty()) rtContact.PreferredPhoneType = Xrm.Page.getAttribute("va_preferredphonetype").getValue();
                }
                if (Xrm.Page.getAttribute("va_selfservicenotifications").getIsDirty()) rtContact.SelfServiceNotifications = Xrm.Page.getAttribute("va_selfservicenotifications").getValue();

                if (Xrm.Page.getAttribute("va_timezone").getIsDirty()) rtContact.TimeZone = Xrm.Page.getAttribute("va_timezone").getValue();

                rtContact.MorningPhone = Xrm.Page.getAttribute("va_morningphone").getValue();
                rtContact.AfternoonPhone = Xrm.Page.getAttribute("va_afternoonphone").getValue();
                rtContact.EveningPhone = Xrm.Page.getAttribute("va_eveningphone").getValue();

                // if no ssn and no partid, we didn't find a person - no need to create contact
                if (!isContact && (participantId && participantId.length > 0) || (rtContact.ssn && rtContact.ssn.length > 0)) {
                    var crmContact = rtContact.create();
                    if (crmContact != null) {
                        idValue = crmContact.ContactId;
                        textValue = crmContact.FullName;
                    }
                }
            }
            if (!isContact) {
                if (idValue) {
                    Xrm.Page.getAttribute("regardingobjectid").setValue([
                        { id: idValue, name: textValue, entityType: "contact" }
                    ]);
                    var newHeaderVal = textValue;
                    var phoneNo = Xrm.Page.getAttribute("phonenumber").getValue();
                    if (phoneNo && phoneNo.length > 0) newHeaderVal = phoneNo + '/' + newHeaderVal;
                    //SetHeaderField('phonenumber', newHeaderVal, 'underline', 'solid 1px blue', 'blue', 'x-small', 'pointer', null);
                    updateSubject();
                    // if type of caller is self, update caller lookup with same vet id
                    if (Xrm.Page.getAttribute("va_callerrelationtoveteran").getSelectedOption() != null && Xrm.Page.getAttribute("va_callerrelationtoveteran").getSelectedOption().text == 'Self') {
                        Xrm.Page.getAttribute("from").setValue([
                            { id: idValue, name: textValue, entityType: "contact" }
                        ]);
                    }
                    CheckPriorCalls();
                }
            }
        });
    //CloseProgress();
}
//=============================================
//  ShowFlagsAndTooltips()
//=============================================
function ShowFlagsAndTooltips(rtContact) {
    // go through claims and figure out status
    if (!_claimStatus) {
        _claimStatus = new ClaimStatus();
    }
    var ga = Xrm.Page.getAttribute;
    _claimStatus.AnalyzeClaimRecordset(ga('va_benefitclaimresponse').getValue());
    //_claimStatus.AnalyzeTrackedItems(Xrm.Page.getAttribute('va_findtrackeditemsresponse').getValue()); //done on tracked items grid
    //    if (!_paymentHistoryStatus) {_paymentHistoryStatus = new PaymentHistoryStatus();}
    //    _paymentHistoryStatus.AnalyzePaymentHistory(Xrm.Page.getAttribute('va_findpaymenthistoryresponse').getValue());
    //    var summary = _paymentHistoryStatus.Summary;
    rtContact.claimStatus = _claimStatus;
    //VTRIGILI - 2015-05-05 - Added Try-Catch wrapper as this occasionally fails 
    //         - Root cause of failure somes to be that rtContact isnot always avaible
    try
    {
        var flags = rtContact.getCorpFlags(1); // will return one line with most important flags
    var tooltip = rtContact.getCorpFlags(2); // will mult lines with all flags

    //Edit out the POA from the flags and the tooltip
    if (flags) {
        var varPOALocation = flags.indexOf('POA');
        if (varPOALocation > 0) {
            //flags = flags.substring(0, varPOALocation + 4) + flags.substring(varPOALocation + 10);
            flags = flags.substring(0, varPOALocation + 8);
        }
    }
    if (tooltip) {
        var varPOALocation = tooltip.indexOf('POA');
        if (varPOALocation > 0) {
            tooltip = tooltip.substring(0, varPOALocation + 4) + tooltip.substring(varPOALocation + 10);
        }
    }
    SetHeaderField('va_flags', flags, null, 'solid 1px blue', 'blue', null, null, tooltip);
    var flashes = rtContact.flashes;
    tooltip = (flashes ? flashes.replace(new RegExp('; ', 'g'), '\n') : '');
    //SetHeaderField('va_flashes', flashes, null, 'solid 1px blue', 'blue', null, null, tooltip);
    if ((flashes) && (tooltip)) {
        if (flashes.length > 16) {
            flashes = flashes.slice(0, 14) + '[...]';
        }
        SetHeaderField('va_flashes', flashes, null, 'solid 1px blue', 'blue', null, null, tooltip);
    }
    } catch (e) {
        return;
    }
    //Field names don't match, so.....
    if (isContact = (Xrm.Page.data.entity.getEntityName() == 'contact')) {   //contact screen
        document.getElementById("va_ssn_c").title = tooltip;
        document.getElementById("va_ssn_d").title = tooltip;
    } else {                            //phone call screen
        var fullname = '';
        if (Xrm.Page.getAttribute("va_lastname").getValue() != null) {
            fullname += Xrm.Page.getAttribute("va_lastname").getValue();
        }
        if (Xrm.Page.getAttribute("va_firstname").getValue() != null) {
            if (fullname != '') {
                fullname += ', ';
            }

            fullname += Xrm.Page.getAttribute("va_firstname").getValue();
        }

        //$('#header_va_headername_d').text(fullname);
        SetHeaderField('va_headername', fullname, null, 'solid 1px blue', 'blue', null, null, '');

        document.getElementById("regardingobjectid_c").title = tooltip;
        document.getElementById("regardingobjectid_d").title = tooltip;
        //SetHeaderField('va_firstname', ga('va_firstname').getValue(), null, 'solid 1px blue', 'blue', null, null, '');
        //SetHeaderField('va_lastname', ga('va_lastname').getValue(), null, 'solid 1px blue', 'blue', null, null, '');
        SetHeaderField('phonenumber', ga('phonenumber').getValue(), null, 'solid 1px blue', 'blue', null, null, '');
        SetHeaderField('va_headerid', ga('va_headerid').getValue(), null, 'solid 1px blue', 'blue', null, null, Xrm.Page.getAttribute('va_headerid').getValue());
        SetHeaderField('va_headerid', ga('va_headerid').getValue(), null, 'solid 1px blue', 'blue', null, null, Xrm.Page.getAttribute('va_headerid').getValue(), true);
        SetHeaderField('va_sojrpo', ga('va_sojrpo').getValue(), null, 'solid 1px blue', 'blue', null, null, '');
    }
}
//=============================================
//  SetHeaderField()
// SetHeaderField('va_flags', 'bla', 'underline', 'solid 1px blue', 'blue', 'x-small', 'pointer');
//=============================================
function SetHeaderField(fieldId, newText, textDecoration, underline, color, fontsize, cursor, tooltipText, isFooter) {
    if (typeof isFooter == "undefined") {
        isFooter = false;
    }
    var id = (isFooter ? 'footer_' : 'header_') + fieldId + '_d';
    var element = document.getElementById(id);
    if (!element) return;
    var field = element.childNodes[0];
    if (newText) {
        field.innerText = newText;
    }
    else {
        field.innerText = '';
    }
    if (color) field.style.color = color;
    if (!isFooter) {
        field.style.fontSize = 'small';
    }
    if (tooltipText && tooltipText.length > 0) field.title = tooltipText;

    if ((Xrm.Page.getAttribute(fieldId)) && (!Xrm.Page.getAttribute(fieldId).getValue())) {
        Xrm.Page.getAttribute(fieldId).setValue(newText);
    }
}
//=============================================
//  updateSearchResultsSection()
//=============================================
//function updateSearchResultsSection() {
//    _IFRAME_SOURCE_MULTIPLE = 'ISV/ext-4.0.1/VA' + _vrmVersion + '/VA-phone/phone-multiple.html';
//    _IFRAME_SOURCE_SINGLE = 'ISV/ext-4.0.1/VA' + _vrmVersion + '/VA-contact/contact.html';

//    var orgname = Xrm.Page.context.getOrgUniqueName();
//    var scriptRoot = Xrm.Page.context.getServerUrl().replace(orgname, '');
//    var sourceURL = '';
//    var searchIndividual;
//    // TODO: enhance logic in the line below by checking SSN from both ends when both birls and corp return single record
//    if (_CORP_RECORD_COUNT > 1 || _BIRLS_RECORD_COUNT > 1) {
//        sourceURL = '/' + _IFRAME_SOURCE_MULTIPLE;
//        searchIndividual = '';
//        if (Xrm.Page.getAttribute('va_firstname').getValue() && Xrm.Page.getAttribute('va_lastname').getValue()) {
//            searchIndividual = Xrm.Page.getAttribute('va_firstname').getValue() + ' '
//			+ Xrm.Page.getAttribute('va_lastname').getValue();
//        }
//        else {
//            if (_birlsResult) {
//                searchIndividual = GetBirlsSectionName();
//            }
//        }
//        searchIndividual += '. Corp: ' + _CORP_RECORD_COUNT + '; BIRLS: ' + _BIRLS_RECORD_COUNT;
//    }
//    else {
//        sourceURL = '/' + _IFRAME_SOURCE_SINGLE;
//        if (Xrm.Page.data.entity.getEntityName() == 'contact') {
//            if (Xrm.Page.getAttribute('firstname').getValue() && Xrm.Page.getAttribute('lastname').getValue()) {
//                searchIndividual = Xrm.Page.getAttribute('firstname').getValue() + ' '
//					+ Xrm.Page.getAttribute('lastname').getValue();
//            }
//            else {
//                if (_birlsResult) {
//                    searchIndividual = GetBirlsSectionName();
//                }
//            }
//        } else {
//            if (Xrm.Page.getAttribute('va_firstname').getValue() && Xrm.Page.getAttribute('va_lastname').getValue()) {
//                searchIndividual = Xrm.Page.getAttribute('va_firstname').getValue() + ' '
//					+ Xrm.Page.getAttribute('va_lastname').getValue();
//            }
//            else {
//                if (_birlsResult) {
//                    searchIndividual = GetBirlsSectionName();
//                }
//            }
//        }
//        var ssn = Xrm.Page.getAttribute('va_ssn').getValue();
//        if (ssn) { searchIndividual += ' (' + ssn + ')'; }
//    }
//    var totalExecutionTime = _totalWebServiceExecutionTime;
//    var executionSeconds = (totalExecutionTime / 1000);
//    var searchLabelText = 'Search Results for ' + searchIndividual
//		+ '; Execution time:  ' + executionSeconds + ' seconds';
//    if (Xrm.Page.data.entity.getEntityName() == 'contact') {            //contact screen
//        Xrm.Page.getControl('IFRAME_ro').setSrc(_iframesrc);
//        Xrm.Page.ui.tabs.get('tab_search').sections.get('search_results').setVisible(true);
//    } else {
//        Xrm.Page.ui.tabs.get('tab_search').sections.get('Categorize Call_section_idproofing').setVisible(true);
//        Xrm.Page.ui.tabs.get('tab_search').sections.get('phonecall_section_idprotocol').setVisible(true);
//        Xrm.Page.ui.tabs.get('tab_search').sections.get('callerdetails').setVisible(true);
//        Xrm.Page.ui.tabs.get('tab_search').sections.get('phone searchresults').setVisible(true);
//        Xrm.Page.ui.tabs.get('tab_search').sections.get('phone searchresults').setLabel(searchLabelText);
//        Xrm.Page.getControl('IFRAME_search').setSrc(sourceURL);
//        //if (1 == 0 && _FrameLoader) {_FrameLoader(); }
//    }
//}
//=============================================
//  GetWarningMessages()
//=============================================
function GetWarningMessages(separator, verbose) {
    var msg = '';
    if (_VRMMESSAGE && _VRMMESSAGE.length > 0) {
        for (var i = 0; i < _VRMMESSAGE.length; i++) {
            if (_VRMMESSAGE[i].warningFlag) {
                if (verbose == undefined || verbose == false)
                    msg += separator + _VRMMESSAGE[i].description;
                else
                    msg += separator + FMM(_VRMMESSAGE[i]);
            }
        }
    }
    return msg;
}
function FMM(msg) {
    return (msg.friendlyServiceName && msg.friendlyServiceName != undefined ? 'Service: ' + msg.friendlyServiceName + '; ' : '') +
        'Message: ' + msg.description +
        (msg.methodName && msg.methodName != undefined ? '; Method: ' + msg.methodName : '');
}
//=============================================
//  GetErrorMessages()
//=============================================
function GetErrorMessages(separator, verbose) {
    var msg = '';
    if (_VRMMESSAGE && _VRMMESSAGE.length > 0) {
        for (var i = 0; i < _VRMMESSAGE.length; i++) {
            if (_VRMMESSAGE[i].errorFlag) {
                if (verbose == undefined || verbose == false)
                    msg += separator + _VRMMESSAGE[i].description;
                else
                    msg += separator + FMM(_VRMMESSAGE[i]);
            }
        }
    }
    return msg;
}
function IsSensitiveFileAccessFail() {
    var fail = false;
    if (_VRMMESSAGE && _VRMMESSAGE.length > 0) {
        for (var i = 0; i < _VRMMESSAGE.length; i++) {
            if (_VRMMESSAGE[i].accessViolation) {
                fail = true;
                break;
            }
        }
    }
    return fail;
}
//=============================================
//  LogMessages()
//=============================================
function LogMessages(error, warning, summaryOnly) {
    //ShowProgress('Updating Query Log');
    var msg = '';
    var request = '';
    if (_VRMMESSAGE && _VRMMESSAGE.length > 0) {
        var query = GetQueryXML();
        for (var i = 0; i < _VRMMESSAGE.length; i++) {
            // skip diag messages
            if (!_VRMMESSAGE[i].serviceName && !_VRMMESSAGE[i].methodName && (_VRMMESSAGE[i].description && _VRMMESSAGE[i].description.indexOf('did not execute correctly') != -1)) {
                continue;
            }
            msg += (msg.length > 0 ? ';' : '') + _VRMMESSAGE[i].serviceName;
            request += (request.length > 0 ? '\n' : '') + _VRMMESSAGE[i].methodName;
            if (!summaryOnly && ((_VRMMESSAGE[i].errorFlag && error) || (_VRMMESSAGE[i].warningFlag && warning))) {
                var cols = {
                    va_Error: error,
                    va_Warning: warning,
                    va_name: _VRMMESSAGE[i].serviceName,
                    va_Request: _VRMMESSAGE[i].xmlRequest,
                    va_Description: _VRMMESSAGE[i].description,
                    va_Query: query,
                    va_Response: _VRMMESSAGE[i].xmlResponse,
                    va_StackTrace: _VRMMESSAGE[i].stackTrace
                };
                CreateQueryLogEntry(cols);
            }
        }
        if (summaryOnly) {
            var cols = {
                va_Error: error,
                va_Warning: warning,
                va_Summary: true,
                va_name: msg,
                va_Request: request,
                va_Query: query,
                va_Duration: parseFloat(_totalWebServiceExecutionTime) //{Value: _totalWebServiceExecutionTime}
            };
            CreateQueryLogEntry(cols);
        }
    }
    //CloseProgress();
}
//=============================================
//  CreateQueryLogEntry()
//=============================================
function CreateQueryLogEntry(cols) {
    if (Xrm.Page.data.entity.getEntityName() == 'contact') {          //contact
        var id = Xrm.Page.data.entity.getId();
        if (id) {
            cols.va_VeteranContactId = {
                Id: id,
                LogicalName: "contact",
                Name: Xrm.Page.getAttribute("firstname").getValue() + ' ' + Xrm.Page.getAttribute("lastname").getValue()
            };
        }
    }
    else {                                      //phone call
        var id = Xrm.Page.data.entity.getId();
        if (id) {
            cols.va_PhoneCallId = {
                Id: id,
                LogicalName: "phonecall",
                Name: Xrm.Page.getAttribute("subject").getValue()
            };
        }
        if (Xrm.Page.getAttribute("regardingobjectid").getValue()) {
            cols.va_VeteranContactId = {
                Id: Xrm.Page.getAttribute("regardingobjectid").getValue()[0].id,
                LogicalName: "contact",
                Name: Xrm.Page.getAttribute("regardingobjectid").getValue()[0].name
            };
        }
    }
    if (cols.va_Description && cols.va_Description.length > 4000) {
        cols.va_Description = cols.va_Description.substring(0, 4000);
    }
    var log = CrmRestKit2011.Create("va_querylog", cols, false)
        .fail(function (error) {
            UTIL.restKitError(err, 'Failed to create dev note');
        });
}
//=============================================
//  GetQueryXML()
//=============================================
function GetQueryXML() {
    var query = null;
    var option = null;
    if (Xrm.Page.data.entity.getEntityName() == 'contact') {            //contact screen
        if (Xrm.Page.getAttribute("va_participantid").getValue()) {
            query = '<pid>' + (Xrm.Page.getAttribute("va_participantid").getValue() ? Xrm.Page.getAttribute("va_participantid").getValue() : '') + '</pid>';
        } else {
            query = '<ssn>' + (Xrm.Page.getAttribute("va_ssn").getValue() ? Xrm.Page.getAttribute("va_ssn").getValue() : '') + '</ssn>';
        }
    } else {                                      // phone call
        option = Xrm.Page.getAttribute("va_searchtype").getValue();
        if (!option) return query;
        switch (parseInt(option)) {
            case 2:
                query = '<pid>' + (Xrm.Page.getAttribute("va_participantid").getValue() ? Xrm.Page.getAttribute("va_participantid").getValue() : '') + '</pid>';
                break;
            case 3:
                query = '<edipi>' + (Xrm.Page.getAttribute("va_edipi").getValue() ? Xrm.Page.getAttribute("va_edipi").getValue() : '') + '</edipi>';
                break;
            case 1:
                var DOBDate = (Xrm.Page.getAttribute("va_dobtext") && Xrm.Page.getAttribute("va_dobtext").getValue() != '') ? new Date(Xrm.Page.getAttribute("va_dobtext").getValue()) : null;
                query =
                    (Xrm.Page.getAttribute("va_ssn").getValue() ? '<ssn>' + Xrm.Page.getAttribute("va_ssn").getValue() + '</ssn>\n' : '') +
                    (Xrm.Page.getAttribute("va_servicenumber").getValue() ? '<serviceno>' + Xrm.Page.getAttribute("va_servicenumber").getValue() + '</serviceno>\n' : '') +
                    (Xrm.Page.getAttribute("va_firstname").getValue() ? '<fn>' + Xrm.Page.getAttribute("va_firstname").getValue() + '</ln>\n' : '') +
                    (Xrm.Page.getAttribute("va_lastname").getValue() ? '<ln>' + Xrm.Page.getAttribute("va_lastname").getValue() + '</ln>\n' : '') +
                    (DOBDate && DOBDate != null ? '<dob>' + DOBDate + '</dob>\n' : '') +
                    (Xrm.Page.getAttribute("va_dod").getValue() ? '<dod>' + Xrm.Page.getAttribute("va_dod").getValue() + '</dod>\n' : '') +
                    (Xrm.Page.getAttribute("va_insurancenumber").getValue() ? '<insno>' + Xrm.Page.getAttribute("va_insurancenumber").getValue() + '</insno>\n' : '') +
                    (Xrm.Page.getAttribute("va_middleinitial").getValue() ? '<mi>' + Xrm.Page.getAttribute("va_middleinitial").getValue() + '</mi>' : '');
                break;
        }
    }
    return query;
}
//=============================================
//  CreateClaimServiceRequest()
//=============================================
function CreateClaimServiceRequest(selection, srType, alreadyPrompted, defaultActionText, defaultActionId, callIssue, doOpen, doAlert) {
    //debugger;
    if (doOpen == undefined || doOpen == null) {
        doOpen = true;
    }
    if (doAlert == undefined || doAlert == null) {
        doAlert = false;
    }

    var id = Xrm.Page.data.entity.getId(), reqNumber = '', isContact = (Xrm.Page.data.entity.getEntityName() == 'contact');
    var ptcpntId = null;
    var servicerequestId = null;
    if (!id || Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE) {
        CloseProgress();
        var save = confirm('Record must be saved prior to creating a new Service Request. Would you like to save it now?');
        if (!save) return;
        if (!Xrm.Page.data.entity.save()) return;
    }
    if (!srType || (srType != 'Claim' && srType != 'Dependent' && srType != 'Appeals')) srType = 'Claim';
    var isClaim = (srType == 'Claim');
    var isDependent = (srType == 'Dependent');
    if (!alreadyPrompted) {
        if (!confirm('Please confirm that you want to create a new ' + srType + ' Service Request.')) {
            return;
        }
    }
    try {
        ShowProgress('Creating new Service Request');
        var claimDetails = '', claimSOJ = '';
        var relatedRecordInfo = null, dobtext = null, dod = null;
        var srType = isClaim ? 953850005 : (srType != 'Appeals' ? 953850023 : 953850000);  // initClaim : add dependent or appeals
        var claimHasAllTrackedItemsReceivedOrClosed = false;
        var dispositionText = (Xrm.Page.getAttribute('va_disposition') && Xrm.Page.getAttribute('va_disposition').getText() ? Xrm.Page.getAttribute('va_disposition').getText() : "") +
            (Xrm.Page.getAttribute('va_dispositionsubtype') && Xrm.Page.getAttribute('va_dispositionsubtype').getText() ? " - " + Xrm.Page.getAttribute('va_dispositionsubtype').getText() : "") +
            (Xrm.Page.getAttribute('va_dispositioncomments') && Xrm.Page.getAttribute('va_dispositioncomments').getValue() ? " - " + Xrm.Page.getAttribute('va_dispositioncomments').getSelectedOption().text : "");
        if (!dispositionText) dispositionText = '';
        var devNoteText = '';
        var claimNumber = null;
        if (callIssue == undefined) {
            callIssue = null;
        }
        if (!callIssue && !isContact) {
            // map to the first recorded issue
            var attr = Xrm.Page.getAttribute('va_disposition');
            if (attr) callIssue = attr.getValue();
        }
        if (selection) {
            srType = isClaim ? 953850004 : 953850024;  //Claim Status Request : Update Dependent Information
            if (isClaim) {
                claimNumber = selection.benefitClaimID;
                ptcpntId = selection.ptcpntId;
                relatedRecordInfo = (selection.benefitClaimID ? selection.benefitClaimID + " - " : "") +
                    (selection.claimTypeName ? selection.claimTypeName + " - " : "") +
                    (selection.statusTypeCode ? selection.statusTypeCode : "") + "; " + dispositionText;
                // find if claim has all tracked items received or closed
                if (selection.benefitClaimID && _claimStatus && _claimStatus.TrackedItemStatusList &&
                    _claimStatus.TrackedItemStatusList[selection.benefitClaimID.toString()]) {
                    var status = _claimStatus.TrackedItemStatusList[selection.benefitClaimID.toString()];
                    claimHasAllTrackedItemsReceivedOrClosed =
                        (status.Count == status.Count_Closed || status.Count == status.Count_Received);
                }
                claimDetails =
                    (selection.claimReceiveDate ? "Claim Receive Date: " + selection.claimReceiveDate + "\n" : "") +
                    (selection.lastActionDate ? "Last Action Date: " + selection.lastActionDate + "\n" : "") +
                    (selection.claimantFirstName ? "Claimant FN: " + selection.claimantFirstName + "\n" : "") +
                    (selection.claimantLastName ? "Claimant LN: " + selection.claimantLastName + "\n" : "") +
                    (selection.claimTypeName ? "Claim Type: " + selection.claimTypeName + "\n" : "") +
                    (selection.payeeTypeCode ? "Payee Code: " + selection.payeeTypeCode + "\n" : "") +
                    (selection.statusTypeCode ? "Status Code: " + selection.statusTypeCode + "\n" : "") +
                    (selection.programTypeCode ? "Program Code: " + selection.programTypeCode + "\n" : "") +
                    (selection.endProductTypeCode ? "End Product: " + selection.endProductTypeCode + "\n" : "") +
                    (selection.personOrOrganizationIndicator ? "Person or Org: " + selection.personOrOrganizationIndicator + "\n" : "") +
                    (selection.organizationName ? "Organization: " + selection.organizationName + "\n" : "") +
                    (claimHasAllTrackedItemsReceivedOrClosed ? "\nThis claim has all tracked items received or closed and should have the status changed to 'Ready for Decision'.\n" : "") +
                    (selection.contentions ? "\nCONTENTIONS:\n" + selection.contentions : "");
            }
            if (selection.claimSoj != undefined && selection.claimSoj) {
                claimSOJ = selection.claimSoj;
            }

            if (isDependent) {
                relatedRecordInfo =
                    (selection.firstName ? selection.firstName + " " : "") +
                    (selection.middleName ? selection.middleName + " " : "") +
                    (selection.lastName ? selection.lastName : "") +
                    (selection.ssn ? "; " + selection.ssn : "") + '; ' + dispositionText;
                claimDetails =
                    (selection.cityOfBirth ? "City Of Birth: " + selection.cityOfBirth + "\n" : "") +
                    (selection.currentRelateStatus ? "Current Relate Status: " + selection.currentRelateStatus + "\n" : "") +
                    (selection.dobtext ? "Date Of Birth: " + selection.dobtext + "\n" : "") +
                    (selection.dateOfDeath ? "Date Of Death: " + selection.dateOfDeath + "\n" : "") +
                    (selection.deathReason ? "Death Reason: " + selection.deathReason + "\n" : "") +
                    (selection.emailAddress ? "Email Address: " + selection.emailAddress + "\n" : "") +
                    (selection.gender ? "Gender: " + selection.gender + "\n" : "") +
                    (selection.proofOfDependency ? "Proof Of Dependency: " + selection.proofOfDependency + "\n" : "") +
                    (selection.ptcpntId ? "PtcpntId: " + selection.ptcpntId + "\n" : "") +
                    (selection.relatedToVet ? "Related To Vet: " + selection.relatedToVet + "\n" : "") +
                    (selection.relationship ? "Relationship: " + selection.relationship + "\n" : "") +
                    (selection.ssnVerifyStatus ? "SSN VerifyStatus: " + selection.ssnVerifyStatus + "\n" : "") +
                    (selection.stateOfBirth ? "State Of Birth: " + selection.stateOfBirth + "\n" : "");
                if (selection.dobtext) dobtext = selection.dobtext;
                if (selection.dateOfDeath) dod = selection.dateOfDeath;
            }
        }

        var searchResponses = new Array();
        var vetXml = Xrm.Page.getAttribute('va_findcorprecordresponse').getValue();
        var vetXmlObject;
        var birlsXml = Xrm.Page.getAttribute('va_findbirlsresponse').getValue();
        var birlsXmlObject;
        var genInfoXml = Xrm.Page.getAttribute('va_generalinformationresponsebypid').getValue();
        var genInfoXmlObject;
        if (vetXml && vetXml != '') {
            vetXmlObject = _XML_UTIL.parseXmlObject(vetXml);
            searchResponses.push(vetXmlObject);
        }
        if (birlsXml && birlsXml != '') {
            birlsXmlObject = _XML_UTIL.parseXmlObject(birlsXml);
            searchResponses.push(birlsXmlObject);
        }
        if (genInfoXml && genInfoXml != '') {
            genInfoXmlObject = _XML_UTIL.parseXmlObject(genInfoXml);
        }

        var rtContact = new contact();
        var searchResults = _XML_UTIL.concatenateDocs(searchResponses, 'SearchResults');

        if (searchResults && searchResults.xml && searchResults.xml != '') {
            rtContact.parseCorpRecord(searchResults, genInfoXmlObject, null);
            // if it is not for dependent, set birth date from vet's info
            if (!isDependent) {
                dobtext = rtContact.dobtext;
                dod = rtContact.dod;
            }
            // get folder info
            var folderInfo = '';
            rtContact.getFolderInfo(searchResults);
            var folder = rtContact.folderInfo;
            if (folder) {
                for (var key in folder) {
                    if (folder[key] && folder[key].text && folder[key].text.length > 0) folderInfo += (folderInfo.length > 0 ? '\n' : '') + key + ': ' + folder[key].text;
                }
                claimDetails += (claimDetails.length > 0 ? '\n' : '') + 'Folder Information:\n' + folderInfo;
            }
        }
        if (ptcpntId && ptcpntId != undefined && ptcpntId != '') {
            rtContact.participantId = ptcpntId;
        }

        var dependentNames = '';
        var dependentAddresses = '';
        // set default action id
        if (defaultActionText && defaultActionText.length > 0 && (!defaultActionId || defaultActionId == undefined)) {
            switch (defaultActionText) {
                case '0820':
                    defaultActionId = 1;
                    break;
                case '0820a':
                    defaultActionId = 953850001;
                    break;
                case '0820d':
                    defaultActionId = 953850002;
                    break;
                case '0820f':
                    defaultActionId = 953850003;
                    break;
                case 'VAI':
                    defaultActionId = 2;
                    break;
                case '0820 & VAI':
                    defaultActionId = 953850005;
                    break;
                case 'Letter':
                    defaultActionId = 953850006;
                    break;
                case 'Non Emergency Email':
                    defaultActionId = 953850007;
                    break;
                case 'Other':
                    defaultActionId = 953850008;
                    break;
                default:
                    defaultActionId = 953850008; // Other
            }
        }
        if (!defaultActionId || defaultActionId == undefined) defaultActionId = 953850008; // Other

        // calculate number of existing requests for person
        var personId = Xrm.Page.getAttribute("va_ssn").getValue();
        if (!personId || personId.length == 0) {
            var nameFld = isContact ? 'lastname' : 'subject';
            personId = Xrm.Page.getAttribute(nameFld).getValue();
        }
        var searchCol = 'va_SSN';
        if (!personId) {
            personId = Xrm.Page.getAttribute("va_participantid").getValue();
            searchCol = 'va_ParticipantID';
        }
        var priorSRs = CrmRestKit2011.ByQueryAll('va_servicerequest', ['CreatedOn'], searchCol + " eq '" + personId + "' ", false);
        var priorCount = 0;
        priorSRs.fail(function (err) {
            UTIL.restKitError(err, 'Failed to retrieve Service Request response: test');
        })
            .done(function (data) {
                if (data) priorCount = data.length;
                priorCount++;
                // loop up till got unique name
                reqNumber = personId + ": " + priorCount.toString();
                var inUse = true;
                while (inUse) {
                    priorSRs = CrmRestKit2011.ByQueryAll('va_servicerequest', ['CreatedOn'], "va_reqnumber eq '" + reqNumber + "' ", false);
                    priorSRs.fail(function (err) {
                        UTIL.restKitError(err, 'Failed to retrieve Service Request response: test 1');
                    })
                        .done(function (data1) {
                            if (data1 && data1.length > 0) {
                                priorCount++;
                                reqNumber = personId + ": " + priorCount.toString();
                            }
                            else {
                                inUse = false;
                            }
                        });
                }
            });
        var defaultDescription = null, desc = '';

        if (!isContact) {
            var scripts = GetCallScripts();
            for (var i = 0; i < scripts.length; i++) {
                if (scripts[i].va_Issue && scripts[i].va_SubIssue && scripts[i].va_Issue.Value == Xrm.Page.getAttribute('va_disposition').getValue() &&
                    scripts[i].va_SubIssue.Value == Xrm.Page.getAttribute('va_dispositionsubtype').getValue()) {
                    defaultDescription = scripts[i].va_ServiceRequestDescription;
                    break;
                }
            }
            desc = (Xrm.Page.getAttribute('va_disposition').getText() ? Xrm.Page.getAttribute('va_disposition').getText() + ' - ' : '') +
                (Xrm.Page.getAttribute('va_dispositionsubtype').getText() ? Xrm.Page.getAttribute('va_dispositionsubtype').getText() : '') +
                (Xrm.Page.getAttribute('va_dispositioncomments').getValue() ? ': ' + Xrm.Page.getAttribute('va_dispositioncomments').getValue() : '') +
                (Xrm.Page.getAttribute('va_pcrterminated').getValue() == true ? '; PCR - Terminated Call' : '') +
                (defaultDescription ? '\n' + defaultDescription : '');
        }
        devNoteText = "Service Request '" + reqNumber + "' created. " + (Xrm.Page.getAttribute('va_ssn').getValue() ? "File number: " + Xrm.Page.getAttribute('va_ssn').getValue() : "") +
            " \nDesc.: " + desc +
            (relatedRecordInfo ? ".\n Rel.Info: " + relatedRecordInfo : "");

        //GET SELECTED AWARD DATA
        var SelectedAwardData = {
            va_CurrentMonthlyRate: '',
            va_AwardBenefitType: ''
        };
        if (Xrm.Page.getAttribute("va_generalinformationresponsebypid") || Xrm.Page.getAttribute("va_generalinformationresponse")) {
            var genInfoXmlObject = null, awMain = Xrm.Page.getAttribute("va_generalinformationresponse").getValue(),
                genInfoByPIDXmlObject = null, awPID = Xrm.Page.getAttribute("va_generalinformationresponsebypid").getValue();
            if (awMain && awMain.length > 0) {
                genInfoXmlObject = _XML_UTIL.parseXmlObject(awMain);
            }
            if (awPID && awPID.length > 0) {
                genInfoByPIDXmlObject = _XML_UTIL.parseXmlObject(awPID);
            }
            if (genInfoXmlObject && genInfoXmlObject.selectSingleNode('//currentMonthlyRate')) {
                SelectedAwardData.va_CurrentMonthlyRate = genInfoXmlObject.selectSingleNode('//currentMonthlyRate').text;
            } else if (genInfoByPIDXmlObject && genInfoByPIDXmlObject.selectSingleNode('//currentMonthlyRate')) {
                SelectedAwardData.va_CurrentMonthlyRate = genInfoByPIDXmlObject.selectSingleNode('//currentMonthlyRate').text;
            }
            if (genInfoXmlObject && genInfoXmlObject.selectSingleNode('//benefitTypeName')) {
                SelectedAwardData.va_AwardBenefitType = genInfoXmlObject.selectSingleNode('//benefitTypeName').text;
            } else if (genInfoByPIDXmlObject && genInfoByPIDXmlObject.selectSingleNode('//benefitTypeName')) {
                SelectedAwardData.va_AwardBenefitType = genInfoByPIDXmlObject.selectSingleNode('//benefitTypeName').text;
            }
        }

        //Obtain Date of Death field
        var vetDateOfDeath = null;
        if (Xrm.Page.getAttribute('va_findcorprecordresponse').getValue()) {
            var vetXmlObject = _XML_UTIL.parseXmlObject(Xrm.Page.getAttribute('va_findcorprecordresponse').getValue());
            if (SingleNodeExists(vetXmlObject, '//dateOfDeath')) {
                vetDateOfDeath = vetXmlObject.selectSingleNode('//dateOfDeath').text;
            }
        }
        if (Xrm.Page.getAttribute('va_findbirlsresponse').getValue()) {
            var birlsXmlObject = _XML_UTIL.parseXmlObject(Xrm.Page.getAttribute('va_findbirlsresponse').getValue());
            if (SingleNodeExists(birlsXmlObject, '//DATE_OF_DEATH') && vetDateOfDeath == null) {
                vetDateOfDeath = birlsXmlObject.selectSingleNode('//DATE_OF_DEATH').text;
            }
        }

        //Get Fiduciary and POA info
        var poaData = {
            va_HasPOA: false,
            va_POAData: ''
        };
        var fiduciaryData = {
            va_HasFiduciary: false,
            va_FiduciaryData: ''
        };
        if (Xrm.Page.getAttribute('va_findfiduciarypoaresponse').getValue()) {
            var POAFidXmlObject = _XML_UTIL.parseXmlObject(Xrm.Page.getAttribute('va_findfiduciarypoaresponse').getValue());
            //FID
            fiduciaryData.va_FiduciaryData += 'Name: ' + (SingleNodeExists(POAFidXmlObject, '//currentFiduciary/personOrgName') ? POAFidXmlObject.selectSingleNode('//currentFiduciary/personOrgName').text + '\n' : '');

            fiduciaryData.va_FiduciaryData += 'From: ' + (SingleNodeExists(POAFidXmlObject, '//currentFiduciary/beginDate') ? POAFidXmlObject.selectSingleNode('//currentFiduciary/beginDate').text + '\n' : '');

            fiduciaryData.va_FiduciaryData += 'To: ' + (SingleNodeExists(POAFidXmlObject, '//currentFiduciary/endDate') ? POAFidXmlObject.selectSingleNode('//currentFiduciary/endDate').text + '\n' : '');

            fiduciaryData.va_FiduciaryData += 'Relation: ' + (SingleNodeExists(POAFidXmlObject, '//currentFiduciary/relationshipName') ? POAFidXmlObject.selectSingleNode('//currentFiduciary/relationshipName').text + '\n' : '');

            fiduciaryData.va_FiduciaryData += 'Person Or Org: ' + (SingleNodeExists(POAFidXmlObject, '//currentFiduciary/personOrOrganizationIndicator') ? POAFidXmlObject.selectSingleNode('//currentFiduciary/personOrOrganizationIndicator').text + '\n' : '');

            fiduciaryData.va_FiduciaryData += 'Temp Custodian: ' + (SingleNodeExists(POAFidXmlObject, '//currentFiduciary/temporaryCustodianIndicator') ? POAFidXmlObject.selectSingleNode('//currentFiduciary/temporaryCustodianIndicator').text + '\n' : '');

            if (SingleNodeExists(POAFidXmlObject, '//currentFiduciary/personOrgName')) {
                fiduciaryData.va_HasFiduciary = true;
            }

            //POA
            poaData.va_POAData += 'Name: ' + (SingleNodeExists(POAFidXmlObject, '//currentPowerOfAttorney/personOrgName') ? POAFidXmlObject.selectSingleNode('//currentPowerOfAttorney/personOrgName').text + '\n' : '');

            poaData.va_POAData += 'From: ' + (SingleNodeExists(POAFidXmlObject, '//currentPowerOfAttorney/beginDate') ? POAFidXmlObject.selectSingleNode('//currentPowerOfAttorney/beginDate').text + '\n' : '');

            poaData.va_POAData += 'To: ' + (SingleNodeExists(POAFidXmlObject, '//currentPowerOfAttorney/endDate') ? POAFidXmlObject.selectSingleNode('//currentPowerOfAttorney/endDate').text + '\n' : '');

            poaData.va_POAData += 'Relation: ' + (SingleNodeExists(POAFidXmlObject, '//currentPowerOfAttorney/relationshipName') ? POAFidXmlObject.selectSingleNode('//currentPowerOfAttorney/relationshipName').text + '\n' : '');

            poaData.va_POAData += 'Person Or Org: ' + (SingleNodeExists(POAFidXmlObject, '//currentPowerOfAttorney/personOrOrganizationIndicator') ? POAFidXmlObject.selectSingleNode('//currentPowerOfAttorney/personOrOrganizationIndicator').text + '\n' : '');

            poaData.va_POAData += 'Temp Custodian: ' + (SingleNodeExists(POAFidXmlObject, '//currentPowerOfAttorney/temporaryCustodianIndicator') ? POAFidXmlObject.selectSingleNode('//currentPowerOfAttorney/temporaryCustodianIndicator').text + '\n' : '');

            if (SingleNodeExists(POAFidXmlObject, '//currentPowerOfAttorney/personOrgName')) {
                poaData.va_HasPOA = true;
            }
        }

        //GET VETERAN MAILING ADDRESS
        var VetMailingAddress = {
            va_mailing_address1: '',
            va_mailing_address2: '',
            va_mailing_address3: '',
            va_mailing_city: '',
            va_mailing_state: '',
            va_mailing_zip: '',
            va_MailingCountry: '',
            // foreign
            territory: '',
            province: '',
            foreignZip: '',
            // Overseas
            milPO: '',
            milPostal: ''
        };

        if (Xrm.Page.getAttribute('va_findaddressresponse').getValue()) {
            var getAllAddress_xmlObject = _XML_UTIL.parseXmlObject(Xrm.Page.getAttribute('va_findaddressresponse').getValue());
            var mailingAddressNode = new ActiveXObject("Microsoft.XMLDOM"); //initialized so selectSingleNode does not fail after for loop
            var addresses = getAllAddress_xmlObject.selectNodes('//return');
            for (var i = 0; i < addresses.length; i++) {
                var node = _XML_UTIL.parseXmlObject(addresses[i].xml);
                if (node.selectSingleNode('//ptcpntAddrsTypeNm') && node.selectSingleNode('//ptcpntAddrsTypeNm').text == 'Mailing') {
                    VetMailingAddress.va_mailing_address1 = node.selectSingleNode('//addrsOneTxt') ? node.selectSingleNode('//addrsOneTxt').text : '';
                    VetMailingAddress.va_mailing_address2 = node.selectSingleNode('//addrsTwoTxt') ? node.selectSingleNode('//addrsTwoTxt').text : '';
                    VetMailingAddress.va_mailing_address3 = node.selectSingleNode('//addrsThreeTxt') ? node.selectSingleNode('//addrsThreeTxt').text : '';
                    VetMailingAddress.va_mailing_city = node.selectSingleNode('//cityNm') ? node.selectSingleNode('//cityNm').text : '';
                    VetMailingAddress.va_mailing_state = node.selectSingleNode('//postalCd') ? node.selectSingleNode('//postalCd').text : '';
                    VetMailingAddress.va_mailing_zip = node.selectSingleNode('//zipPrefixNbr') ? node.selectSingleNode('//zipPrefixNbr').text : '';
                    VetMailingAddress.va_MailingCountry = node.selectSingleNode('//cntryNm') ? node.selectSingleNode('//cntryNm').text : '';

                    VetMailingAddress.territory = node.selectSingleNode('//trtryNm') ? node.selectSingleNode('//trtryNm').text : '';
                    VetMailingAddress.province = node.selectSingleNode('//prvncNm') ? node.selectSingleNode('//prvncNm').text : '';
                    VetMailingAddress.foreignZip = node.selectSingleNode('//frgnPostalCd') ? node.selectSingleNode('//frgnPostalCd').text : '';

                    VetMailingAddress.milPO = node.selectSingleNode('//mltyPostOfficeTypeCd') ? node.selectSingleNode('//mltyPostOfficeTypeCd').text : '';
                    VetMailingAddress.milPostal = node.selectSingleNode('//mltyPostalTypeCd') ? node.selectSingleNode('//mltyPostalTypeCd').text : '';
                    break;
                }
            }
            mailingAddressNode = null;
        }

        // if Foreign zip is present, use it in zip field
        if (VetMailingAddress.foreignZip && VetMailingAddress.foreignZip.length > 0) {
            //VetMailingAddress.va_mailing_zip = (VetMailingAddress.va_mailing_zip && VetMailingAddress.va_mailing_zip.length > 0 ? VetMailingAddress.va_mailing_zip
            //                            + '/' : '') + VetMailingAddress.foreignZip;
            VetMailingAddress.va_mailing_zip = VetMailingAddress.foreignZip;
        }

        // if terr/province OR APO AE fields are provided, put them in one of address fields
        // Yoshi's comments - use State field
        var terrProvince = '';
        var apoae = '';

        if (VetMailingAddress.territory && VetMailingAddress.territory.length > 0) {
            terrProvince = VetMailingAddress.territory;
        }
        if (VetMailingAddress.province && VetMailingAddress.province.length > 0) {
            terrProvince = (terrProvince.length > 0 ? terrProvince + ' / ' : '') + VetMailingAddress.province;
        }
        if (VetMailingAddress.milPO && VetMailingAddress.milPO.length > 0) {
            apoae = VetMailingAddress.milPO;
        }
        if (VetMailingAddress.milPostal && VetMailingAddress.milPostal.length > 0) {
            apoae = (apoae.length > 0 ? apoae + ' ' : '') + VetMailingAddress.milPostal;
        }
        if (apoae.length > 0) {
            // uncomment below to use Terr/Province lines for
            //terrProvince = (terrProvince.length > 0 ? terrProvince + ' / ' : '') + apoae;

            // use State line for apoae
            VetMailingAddress.va_mailing_state = (VetMailingAddress.va_mailing_state && VetMailingAddress.va_mailing_state.length > 0 ? VetMailingAddress.va_mailing_state + ' / ' : '') + apoae;
        }

        if (terrProvince.length > 0) {
            if (!VetMailingAddress.va_mailing_address1 || VetMailingAddress.va_mailing_address1.length == 0) {
                VetMailingAddress.va_mailing_address1 = terrProvince;
            }
            else if (!VetMailingAddress.va_mailing_address2 || VetMailingAddress.va_mailing_address2.length == 0) {
                VetMailingAddress.va_mailing_address2 = terrProvince;
            }
            else if (!VetMailingAddress.va_mailing_address3 || VetMailingAddress.va_mailing_address3.length == 0) {
                VetMailingAddress.va_mailing_address3 = terrProvince;
            }
            else {
                VetMailingAddress.va_mailing_address3 = VetMailingAddress.va_mailing_address3 + '; ' + terrProvince;
                if (VetMailingAddress.va_mailing_address3.length > 100) VetMailingAddress.va_mailing_address3 = VetMailingAddress.va_mailing_address3.substring(0, 100);
            }
        }

        //Get the phone numbers and file number from CORP RECORD RESPONSE
        var PhoneNumbers = {
            dayPhone: "",
            eveningPhone: ""
        };
        var fileNumber = null;
        if (vetXml) {
            var vetXml_Object = _XML_UTIL.parseXmlObject(vetXml);
            var returnNode = vetXml_Object.selectSingleNode('//return');
            if (returnNode.selectSingleNode('//areaNumberOne') && returnNode.selectSingleNode('//phoneNumberOne')) {
                PhoneNumbers.dayPhone = FormatPhone(returnNode.selectSingleNode('//areaNumberOne').text, returnNode.selectSingleNode('//phoneNumberOne').text);
            }
            if (returnNode.selectSingleNode('//areaNumberTwo') && returnNode.selectSingleNode('//phoneNumberTwo')) {
                PhoneNumbers.eveningPhone = FormatPhone(returnNode.selectSingleNode('//areaNumberTwo').text, returnNode.selectSingleNode('//phoneNumberTwo').text);
            }
            if (returnNode.selectSingleNode('//fileNumber')) {
                fileNumber = returnNode.selectSingleNode('//fileNumber').text;
            }
        }

        //GET AWARD INFO
        var AwardInfo = {
            DependentAmount: "",
            NetAmountPaid: "",
            AAAmount: "",
            PensionBenefitAmount: "",
            EffectiveDate: "",
            va_BenefitType: ""
        };
        if (Xrm.Page.getAttribute("va_findotherawardinformationresponse").getValue()) {
            var awardXml_Object = _XML_UTIL.parseXmlObject(Xrm.Page.getAttribute("va_findotherawardinformationresponse").getValue());
            var awardLineNode = new ActiveXObject("Microsoft.XMLDOM"); //will store the Award Line with the most recent effective date
            var awardLines_Nodes = awardXml_Object.selectNodes('//awardLines');
            for (i = 0; i < awardLines_Nodes.length; i++) {
                node = _XML_UTIL.parseXmlObject(awardLines_Nodes[i].xml); //get the current node xml
                if (node.selectSingleNode('//effectiveDate') && node.selectSingleNode('//effectiveDate').text != '') {
                    //if first node, assign to awardLineNode
                    if (awardLineNode.xml == undefined || awardLineNode.xml == null || awardLineNode.xml == '') {
                        awardLineNode = node;
                        continue;
                    }
                    if (FormatAwardDate(node.selectSingleNode('//effectiveDate').text) < FormatAwardDate(awardLineNode.selectSingleNode('//effectiveDate').text)) {
                        awardLineNode = node;
                    }
                }
            }
            AwardInfo.DependentAmount = awardLineNode.selectSingleNode('//spouse') ? awardLineNode.selectSingleNode('//spouse').text : '';
            AwardInfo.NetAmountPaid = awardLineNode.selectSingleNode('//netAward') ? awardLineNode.selectSingleNode('//netAward').text : '';
            AwardInfo.AAAmount = awardLineNode.selectSingleNode('//aaHbInd') ? awardLineNode.selectSingleNode('//aaHbInd').text : '';
            AwardInfo.PensionBenefitAmount = awardLineNode.selectSingleNode('//altmnt') ? awardLineNode.selectSingleNode('//altmnt').text : '';
            AwardInfo.EffectiveDate =
                    awardLineNode.selectSingleNode('//effectiveDate') && awardLineNode.selectSingleNode('//effectiveDate').text != '' ? FormatAwardDate(awardLineNode.selectSingleNode('//effectiveDate').text) : '';
            awardLineNode = null;
        }

        //GET RATING DATA
        var RatingData = {
            va_RatingEffectiveDate: '',
            va_RatingDegree: '',
            va_ServiceConnectedDisability: false,
            //va_FutureExamDate: '',
            va_DisabilityList: '',
            va_DisabilityPercentages: ''
        };
        if (Xrm.Page.getAttribute('va_findratingdataresponse').getValue()) {
            var ratingDataResponse_xmlObject = _XML_UTIL.parseXmlObject(Xrm.Page.getAttribute('va_findratingdataresponse').getValue());
            var disabilityNode = ratingDataResponse_xmlObject.selectSingleNode('//disabilityRatingRecord');
            if (disabilityNode != null) {
                disabilityNode = _XML_UTIL.parseXmlObject(disabilityNode.xml);
                //var latestDate = null;

                if (disabilityNode.selectSingleNode('//combinedDegreeEffectiveDate')) {
                    RatingData.va_RatingEffectiveDate = FormatExtjsDate(disabilityNode.selectSingleNode('//combinedDegreeEffectiveDate').text);
                }
                if (disabilityNode.selectSingleNode('//serviceConnectedCombinedDegree')) {
                    RatingData.va_RatingDegree = disabilityNode.selectSingleNode('//serviceConnectedCombinedDegree').text;
                    if (RatingData.va_va_RatingDegree != '') {
                        RatingData.va_ServiceConnectedDisability = true;
                    }
                }

                var disabilityNodes = disabilityNode.selectNodes("//ratings");
                for (var i = 0; i < disabilityNodes.length; i++) {
                    //var tempdate = null;
                    var node = _XML_UTIL.parseXmlObject(disabilityNodes[i].xml);
                    if (node.selectSingleNode("//disabilityDecisionTypeName") && node.selectSingleNode("//disabilityDecisionTypeName").text == "Service Connected") {
                        RatingData.va_DisabilityList += node.selectSingleNode("//diagnosticText") ? node.selectSingleNode("//diagnosticText").text + "\n" : "\n";
                        RatingData.va_DisabilityPercentages += node.selectSingleNode("//diagnosticPercent") ? node.selectSingleNode("//diagnosticPercent").text + "\n" : "\n";
                    }
                    /*tempdate = node.selectSingleNode("//beginDate") ? FormatAwardDate(node.selectSingleNode("//beginDate").text) : null;
                     if (latestDate == null || (tempdate >= latestDate)) {
                     latestDate = tempdate;
                     RatingData.va_FutureExamDate = node.selectSingleNode("//futureExamDate") ? FormatAwardDate(node.selectSingleNode("//futureExamDate").text) : null;
                     }*/
                }
            }
        }

        //GET MILITARY DATA
        var MilitaryData = {
            //this is the recent tour info
            va_branchofservice: "",
            va_discharge: "",
            va_servicedates: "",
            //this is the tour info list
            va_militaryservicebranch: "",
            va_militaryserviceeoddate: "",
            va_militaryserviceraddate: "",
            va_characterofdischarge: ""
        };
        if (Xrm.Page.getAttribute("va_findmilitaryrecordbyptcpntidresponse").getValue()) {
            var militaryResponse_xml = Xrm.Page.getAttribute("va_findmilitaryrecordbyptcpntidresponse").getValue();
            var militaryResponse_xmlObject = null;
            var branches = '';
            var eod = '';
            var rad = '';
            var discharges = '';
            var returnNodes = null;
            var latestTourDate;

            if (militaryResponse_xml != null) {
                militaryResponse_xmlObject = _XML_UTIL.parseXmlObject(militaryResponse_xml);
                if (MultipleNodesExist(militaryResponse_xmlObject, '//return'))
                    returnNodes = militaryResponse_xmlObject.selectNodes('//militaryTours');
            }
            if (returnNodes && returnNodes.length > 0) {
                tourNodes = returnNodes[0].childNodes;

                for (var i = 0; i < tourNodes.length; i++) {
                    if (tourNodes[i].nodeName == 'militaryPersonTours') {
                        var tempdate;
                        if (SingleNodeExists(tourNodes[i], 'militarySvcBranchTypeName')) {
                            branches += tourNodes[i].selectSingleNode('militarySvcBranchTypeName').text + '\n';
                        }
                        if (SingleNodeExists(tourNodes[i], 'eodDate')) {
                            tempdate = tourNodes[i].selectSingleNode('eodDate').text;
                            eod += tempdate + '\n';
                            if (latestTourDate == null) {
                                latestTourDate = new Date(tempdate.split("/")[2], tempdate.split("/")[0] - 1, tempdate.split("/")[1]);
                            }
                        }
                        if (SingleNodeExists(tourNodes[i], 'radDate')) {
                            rad += tourNodes[i].selectSingleNode('radDate').text + '\n';
                        }
                        if (SingleNodeExists(tourNodes[i], 'mpDischargeCharTypeName')) {
                            discharges += tourNodes[i].selectSingleNode('mpDischargeCharTypeName').text + '\n';
                        }
                        if (SingleNodeExists(tourNodes[i], 'eodDate') && (new Date(tempdate.split("/")[2], tempdate.split("/")[0] - 1, tempdate.split("/")[1]) >= latestTourDate)) {
                            MilitaryData.va_branchofservice = tourNodes[i].selectSingleNode('militarySvcBranchTypeName').text;
                            MilitaryData.va_discharge = tourNodes[i].selectSingleNode('mpDischargeCharTypeName').text;
                            MilitaryData.va_servicedates = tourNodes[i].selectSingleNode('eodDate').text + "-" + tourNodes[i].selectSingleNode('radDate').text;
                        }
                    }
                }

                //set list of all military tours
                MilitaryData.va_militaryservicebranch = branches;
                MilitaryData.va_militaryserviceeoddate = eod;
                MilitaryData.va_militaryserviceraddate = rad;
                MilitaryData.va_characterofdischarge = discharges;
            }
        }

        //GET PAYMENT DATA
        var PaymentData = {
            va_PaymentAmount: "",
            va_BenefitType: "",
            va_PayDate: ""
        };
        if (Xrm.Page.getAttribute("va_findpaymenthistoryresponse").getValue()) {
            var paymentXml_Object = _XML_UTIL.parseXmlObject(Xrm.Page.getAttribute("va_findpaymenthistoryresponse").getValue());
            var paymentNodes = paymentXml_Object.selectNodes("//payments");
            var latestDate = null;
            for (var i = 0; i < paymentNodes.length; i++) {
                var node = _XML_UTIL.parseXmlObject(paymentNodes[i].xml);
                var currentDate = new Date((node.selectSingleNode("//payCheckDt").text).split("/")[2], (node.selectSingleNode("//payCheckDt").text).split("/")[0] - 1, (node.selectSingleNode("//payCheckDt").text).split("/")[1]);
                if (latestDate == null || currentDate >= latestDate) {
                    latestDate = currentDate;
                    PaymentData.va_PayDate = latestDate;
                    PaymentData.va_BenefitType = node.selectSingleNode("//payCheckType").text;
                    var payment = node.selectSingleNode("//payCheckAmount").text;
                    PaymentData.va_PaymentAmount = (payment.substring(1) + "00").replace(",", "");
                }
            }
            if (paymentNodes.length == 0) {
                PaymentData.va_PaymentAmount = "00";
            }
        }

        //GET DEPENDENT INFO - SPOUSE DEFAULT
        var DependentData = {
            va_DepAddress: VetMailingAddress.va_mailing_address1 + ' ' + VetMailingAddress.va_mailing_address2,
            va_DepCity: VetMailingAddress.va_mailing_city,
            va_DepState: VetMailingAddress.va_mailing_state,
            va_DepZipcode: VetMailingAddress.va_mailing_zip,
            va_DepSSN: '',
            va_DepDateofBirth: null,
            va_DepEmail: '',
            va_DepFirstName: '',
            va_DepLastName: '',
            va_DepRelation: '',
            va_DepGender: '',
            va_DependentNames: '',
            va_DependentAddresses: '',
            va_SRFirstName: '',
            va_SRLastName: '',
            va_SRRelation: '',
            va_SRSSN: '',
            va_SRGender: '',
            va_SRDOBText: null
        };
        if (Xrm.Page.getAttribute("va_finddependentsresponse").getValue()) {
            var dependentXml_Object = _XML_UTIL.parseXmlObject(Xrm.Page.getAttribute("va_finddependentsresponse").getValue());
            var personNodes = dependentXml_Object.selectNodes("//persons");
            for (var i = 0; i < personNodes.length; i++) {
                var node = _XML_UTIL.parseXmlObject(personNodes[i].xml);
                if (node.selectSingleNode("//relationship") && node.selectSingleNode("//relationship").text == "Spouse") {
                    DependentData.va_DepSSN = node.selectSingleNode("//ssn") ? node.selectSingleNode("//ssn").text : null;
                    DependentData.va_DepEmail = node.selectSingleNode("//emailAddress") ? node.selectSingleNode("//emailAddress").text : null;
                    DependentData.va_DepDateofBirth = node.selectSingleNode("//dateOfBirth") ? new Date((node.selectSingleNode("//dateOfBirth").text).split("/")[2], (node.selectSingleNode("//dateOfBirth").text).split("/")[0] - 1, (node.selectSingleNode("//dateOfBirth").text).split("/")[1]) : null;
                    DependentData.va_DepFirstName = node.selectSingleNode("//firstName") ? node.selectSingleNode("//firstName").text : null;
                    DependentData.va_DepLastName = node.selectSingleNode("//lastName") ? node.selectSingleNode("//lastName").text : null;
                    DependentData.va_DepRelation = node.selectSingleNode("//relationship") ? node.selectSingleNode("//relationship").text : null;
                    DependentData.va_DepGender = node.selectSingleNode("//gender") ? node.selectSingleNode("//gender").text : null;
                }
                DependentData.va_DependentNames += node.selectSingleNode("//firstName").text + ' ' + node.selectSingleNode("//lastName").text + '\n';
                DependentData.va_DependentAddresses += node.selectSingleNode("//cityOfBirth").text + ' ' + node.selectSingleNode("//stateOfBirth").text + '\n';
            }
        }

        if (Xrm.Page.getAttribute("va_generalinformationresponsebypid").getValue()) {
            var generalPIDXML_Object = _XML_UTIL.parseXmlObject(Xrm.Page.getAttribute("va_generalinformationresponsebypid").getValue());
            var veteranPidNode = generalPIDXML_Object.selectNodes("//return");
            var vetNode = _XML_UTIL.parseXmlObject(veteranPidNode[0].xml);
            DependentData.va_SRSSN = vetNode.selectSingleNode("//vetSSN") ? vetNode.selectSingleNode("//vetSSN").text : null;
            DependentData.va_SRFirstName = vetNode.selectSingleNode("//vetFirstName") ? vetNode.selectSingleNode("//vetFirstName").text : null;
            DependentData.va_SRLastName = vetNode.selectSingleNode("//vetLastName") ? vetNode.selectSingleNode("//vetLastName").text : null;
            DependentData.va_SRRelation = vetNode.selectSingleNode("//relationship") ? vetNode.selectSingleNode("//relationship").text : null;
            DependentData.va_SRGender = vetNode.selectSingleNode("//vetSex") ? vetNode.selectSingleNode("//vetSex").text : null;

            var vetBirthDate = vetNode.selectSingleNode("//vetBirthDate") ? vetNode.selectSingleNode("//vetBirthDate").text : null;
            var vetBirthMonth = vetBirthDate.substring(0, 2);
            var vetBirthDay = vetBirthDate.substring(2, 4);
            var vetBirthYear = vetBirthDate.substring(4, 8);
            var birthDate = vetBirthMonth + "/" + vetBirthDay + "/" + vetBirthYear;

            DependentData.va_SRDOBText = birthDate;
        }

        //GET FUTURE EXAM DATE FROM APPOINTMENTS
        var AppointmentData = {
            va_FutureExamDate: null
        };
        if (Xrm.Page.getAttribute("va_readdataappointmentresponse").getValue()) {
            var apptXml_Object = _XML_UTIL.parseXmlObject(Xrm.Page.getAttribute("va_readdataappointmentresponse").getValue());
            var apptNodes = apptXml_Object.selectNodes("//appointment");
            var latestDate = null;
            for (var i = 0; i < apptNodes.length; i++) {
                var node = _XML_UTIL.parseXmlObject(apptNodes[i].xml);
                var dn = node.selectSingleNode("//appointmentDateTime//literal");
                var currentDate = dn && dn.text && dn.text.length > 0 ? FormatPathwaysDate(dn.text) : null;
                if (latestDate == null || (currentDate > latestDate)) {
                    if (node.selectSingleNode("//status//displayText") && node.selectSingleNode("//status//displayText").text == "FUTURE") {
                        latestDate = currentDate;
                        AppointmentData.va_FutureExamDate = latestDate;
                    }
                }
            }
        }

        var callerrelation = Xrm.Page.getAttribute("va_callerrelationtoveteran");
        if (callerrelation != undefined && callerrelation) {
            callerrelation = callerrelation.getValue();
            if (callerrelation == '953850001') {
                callerrelation = '3';
            } else {
                callerrelation = '1';
            }
        } else {
            callerrelation = '1';
        }

        // verify dates
        try {
            if (vetDateOfDeath != null && new Date(vetDateOfDeath) < new Date('1/1/1900')) {
                vetDateOfDeath = null;
            }
        } catch (doe) {
        }
        // format
        if (vetDateOfDeath) {
            vetDateOfDeath = (new Date(vetDateOfDeath)).format("M/dd/yyyy");
        }
        var origCallId = null, va_Phone = null, va_Email = null, va_FirstName = null, va_LastName = null, va_RelationtoVeteran = null, va_RelationshipDetails = null, va_Address1 = null, va_Address2 = null, va_City = null,
            va_State = null, va_ZipCode = null, va_CompensationClaim = null, va_PensionClaim = null, va_Disposition = null, va_FNODReportingFor = null, va_RelatedVeteranId = null;
        var sourcePage = null;

        if (isContact) {
            if (window.parent && window.parent.opener && window.parent.opener.Xrm.Page) {
                sourcePage = window.parent.opener.Xrm.Page;
                try {
                    if (!sourcePage.data) {
                        sourcePage = window.top.opener.parent.Xrm.Page;
                    }
                }
                catch (der) {
                    debugger;
                }
            }
        }
        else {
            sourcePage = Xrm.Page;
        }

        if (sourcePage) {
            if (sourcePage.data.entity.getId()) {
                origCallId = { Id: sourcePage.data.entity.getId(), LogicalName: "phonecall", Name: sourcePage.getAttribute("subject").getValue() };
            }
            va_Phone = sourcePage.getAttribute("phonenumber").getValue();
            va_Email = sourcePage.getAttribute("va_email").getValue();
            va_FirstName = sourcePage.getAttribute("va_callerfirstname").getValue();
            va_LastName = sourcePage.getAttribute("va_callerlastname").getValue();
            va_RelationtoVeteran = { Value: sourcePage.getAttribute("va_callerrelationtoveteran").getValue() };
            va_RelationshipDetails = sourcePage.getAttribute("va_callerinformation").getValue();
            va_Address1 = sourcePage.getAttribute("va_calleraddress1").getValue();
            va_Address2 = sourcePage.getAttribute("va_calleraddress2").getValue();
            va_City = sourcePage.getAttribute("va_callercity").getValue();
            va_State = sourcePage.getAttribute("va_callerstate").getValue();
            va_ZipCode = sourcePage.getAttribute("va_callerzipcode").getValue();
            va_CompensationClaim = sourcePage.getAttribute("va_compensationclaim").getValue();
            va_PensionClaim = sourcePage.getAttribute("va_pensionclaim").getValue();
            va_ZipCode = sourcePage.getAttribute("va_callerzipcode").getValue();
            va_Disposition = { Value: sourcePage.getAttribute("va_dispositionsubtype").getValue() };
            va_FNODReportingFor = { Value: sourcePage.getAttribute("va_fnodreportingfor").getValue() };
        }
        //debugger;
        var callerFullName = '';
        var callerFirstName = sourcePage.getAttribute("va_callerfirstname").getValue();
        var callerLastName = sourcePage.getAttribute("va_callerlastname").getValue();
        if (callerFirstName == null) {
            callerFullName = callerLastName;
        }
        else if (callerFirstName != null) {
            callerFullName = callerFirstName + ' ' + callerLastName;
        }

        if (isContact) { //if inside Contact form
            va_RelatedVeteranId = {
                Id: id, //'id' is the current form GUID
                LogicalName: "contact",
                Name: Xrm.Page.getAttribute("lastname").getValue()
            };
        }
        else { // if inside PhoneCall form
            if (Xrm.Page.getAttribute("regardingobjectid").getValue()) {
                va_RelatedVeteranId = {
                    Id: LookupId(Xrm.Page.getAttribute("regardingobjectid")),
                    LogicalName: "contact",
                    Name: Xrm.Page.getAttribute("regardingobjectid").getValue()[0].name
                };
                devNoteText += ". \nVeteran: " + Xrm.Page.getAttribute("regardingobjectid").getValue()[0].name;
            } else va_RelatedVeteranId = null;
        }
        //debugger;
        var cols = {
            va_reqnumber: reqNumber, //Request Number = SSN/File Number
            va_RelatedVeteranId: va_RelatedVeteranId,
            va_NameofReportingIndividual: callerFullName,
            va_Description: desc,
            va_Phone: va_Phone,
            va_ClaimDetails: claimDetails,
            va_Claim: relatedRecordInfo,
            va_Email: va_Email,
            va_FirstName: va_FirstName,
            va_LastName: va_LastName,
            va_RelationtoVeteran: va_RelationtoVeteran,
            va_RelationshipDetails: va_RelationshipDetails,
            va_Address1: va_Address1,
            va_Address2: va_Address2,
            va_City: va_City,
            va_State: va_State,
            va_ZipCode: va_ZipCode,
            va_SSN: Xrm.Page.getAttribute("va_ssn").getValue(),
            va_ParticipantID: rtContact.participantId,
            va_CompensationClaim: va_CompensationClaim,
            va_PensionClaim: va_PensionClaim,
            va_AllTrackedItemsReceivedOrClosed: claimHasAllTrackedItemsReceivedOrClosed,
            va_DateOpened: new Date(),
            va_Type: { Value: srType },
            va_Issue: { Value: callIssue },
            va_RequestStatus: { Value: 953850000 }, // In Progress
            va_Action: { Value: defaultActionId },
            va_OriginatingCallId: origCallId,
            va_HasFiduciary: fiduciaryData.va_HasFiduciary,
            va_FiduciaryData: fiduciaryData.va_FiduciaryData,
            va_HasPOA: poaData.va_HasPOA,
            va_POAData: poaData.va_POAData,
            va_DateofDeath: vetDateOfDeath,
            va_VetDOBText: dobtext,
            va_DepFirstName: selection ? (isDependent && selection && selection.firstName ? selection.firstName : DependentData.va_DepFirstName) : DependentData.va_DepFirstName,
            va_DepLastName: selection ? (isDependent && selection && selection.lastName ? selection.lastName : DependentData.va_DepLastName) : DependentData.va_DepLastName,
            va_DepSSN: selection ? (isDependent && selection && selection.ssn ? selection.ssn : DependentData.va_DepSSN) : DependentData.va_DepSSN,
            va_DepRelation: selection ? (isDependent && selection && selection.relationship ? selection.relationship : DependentData.va_DepRelation) : DependentData.va_DepRelation,
            va_DepGender: selection ? (isDependent && selection && selection.gender ? selection.gender : DependentData.va_DepGender) : DependentData.va_DepGender,
            va_DepEmail: selection ? (isDependent && selection && selection.emailAddress ? selection.emailAddress : DependentData.va_DepEmail) : DependentData.va_DepEmail,
            va_DepCity: DependentData.va_DepCity,
            va_DepState: DependentData.va_DepState,
            va_DepAddress: DependentData.va_DepAddress,
            va_DepZipcode: DependentData.va_DepZipcode,
            va_DepDateofBirth: (DependentData.va_DepDateofBirth >= new Date('1/1/1900') ? DependentData.va_DepDateofBirth : null),
            va_ClaimNumber: claimNumber,
            va_mailing_address1: VetMailingAddress.va_mailing_address1,
            va_mailing_address2: VetMailingAddress.va_mailing_address2,
            va_mailing_address3: VetMailingAddress.va_mailing_address3,
            va_mailing_city: VetMailingAddress.va_mailing_city,
            va_mailing_state: VetMailingAddress.va_mailing_state,
            va_mailing_zip: VetMailingAddress.va_mailing_zip,
            va_MailingCountry: VetMailingAddress.va_MailingCountry,
            va_DayPhone: PhoneNumbers.dayPhone,
            va_EveningPhone: PhoneNumbers.eveningPhone,
            va_FileNumber: fileNumber,
            va_RatingDegree: RatingData.va_RatingDegree,
            va_RatingEffectiveDate: RatingData.va_RatingEffectiveDate,
            va_ServiceConnectedDisability: RatingData.va_ServiceConnectedDisability,
            va_DisabilityList: RatingData.va_DisabilityList,
            va_DisabilityPercentages: RatingData.va_DisabilityPercentages,
            va_BranchofService: MilitaryData.va_branchofservice,
            va_CharacterOfDischarge: MilitaryData.va_characterofdischarge,
            va_Discharge: MilitaryData.va_discharge,
            va_MilitaryServiceBranch: MilitaryData.va_militaryservicebranch,
            va_MilitaryServiceEODDate: MilitaryData.va_militaryserviceeoddate,
            va_MilitaryServiceRADDate: MilitaryData.va_militaryserviceraddate,
            va_ServiceDates: MilitaryData.va_servicedates,
            va_BenefitType: PaymentData.va_BenefitType,
            va_Disposition: va_Disposition,
            va_ContactPrefix: { Value: callerrelation },
            va_PaymentAmount: {
                Value: PaymentData.va_PaymentAmount == "00" || PaymentData.va_PaymentAmount == "" ? "0.0000" : PaymentData.va_PaymentAmount
            },
            va_DependentAmount: {
                Value: AwardInfo.DependentAmount != "" ? GetNumber(AwardInfo.DependentAmount) + ".0000" : "0.0000"
            },
            va_NetAmountPaid: {
                Value: AwardInfo.NetAmountPaid != "" ? GetNumber(AwardInfo.NetAmountPaid) + ".0000" : "0.0000"
            },
            va_AAAmount: {
                Value: AwardInfo.AAAmount != "" ? GetNumber(AwardInfo.AAAmount) + ".0000" : "0.0000"
            },
            va_PensionBenefitAmount: {
                Value: AwardInfo.PensionBenefitAmount != "" ? GetNumber(AwardInfo.PensionBenefitAmount) + ".0000" : "0.0000"
            },
            va_CurrentMonthlyRate: {
                Value: SelectedAwardData.va_CurrentMonthlyRate != "" ? GetNumber(SelectedAwardData.va_CurrentMonthlyRate) + ".0000" : "0.0000"
            },
            va_PayDate: PaymentData.va_PayDate == "" ? null : PaymentData.va_PayDate,
            va_AwardBenefitType: SelectedAwardData.va_AwardBenefitType,
            va_EffectiveDate: AwardInfo.EffectiveDate == "" ? null : AwardInfo.EffectiveDate,
            va_FutureExamDate: AppointmentData.va_FutureExamDate,
            va_DependentNames: DependentData.va_DependentNames,
            va_DependentAddresses: DependentData.va_DependentAddresses,
            va_FNODReportingFor: va_FNODReportingFor,
            va_SRFirstName: DependentData.va_SRFirstName,
            va_SRLastName: DependentData.va_SRLastName,
            va_SRRelation: DependentData.va_SRRelation,
            va_SRSSN: DependentData.va_SRSSN,
            va_SRGender: DependentData.va_SRGender,
            va_SRDOBText: DependentData.va_SRDOBText
        };
        //Set the default currency to USD (in case they don't have it set in their own settings)
        var transactionCurrencyObj = GetTransactionCurrencyInfo();
        if (transactionCurrencyObj) {
            cols.TransactionCurrencyId = {
                Id: '{' + transactionCurrencyObj.id + '}',
                Name: transactionCurrencyObj.name,
                LogicalName: "transactioncurrency"
            };
        }
        //Set the PCR of Record to the CreatedBy of the phonecall or contact.
        cols.va_PCROfRecordId = {
            Id: Xrm.Page.getAttribute("createdby").getValue()[0].id,
            Name: Xrm.Page.getAttribute("createdby").getValue()[0].name,
            LogicalName: "systemuser"
        };

        //Set va_RelatedVeteranId to be the same as RegardingObjectId if it exists.
        if (!isContact) {
            // same for special issue
            if (Xrm.Page.getAttribute("va_specialissueid") && Xrm.Page.getAttribute("va_specialissueid").getValue()) {
                cols.va_SpecialIssueId = {
                    Id: Xrm.Page.getAttribute("va_specialissueid").getValue()[0].id, LogicalName: "va_specialissue",
                    Name: Xrm.Page.getAttribute("va_specialissueid").getValue()[0].name
                };
                devNoteText += ". \nSpecial Issue: " + Xrm.Page.getAttribute("va_specialissueid").getValue()[0].name;
            }
        }
        //debugger;
        // if SOJ is present, try to get matching Regional Office
        var soj = null, sojResponse = GetSOJ(claimSOJ);
        // pass to SR  claimSOJ;
        // and manySOJ = (sojResponse ? !sojResponse.singleHit : false);

        // Defect 96157. If multiple responses for SOJ found, do not populate value. During screen open, users will have to select
        if (sojResponse.id /*&& sojResponse.singleHit*/) {
            cols.va_RegionalOfficeId = { Id: sojResponse.id, LogicalName: "va_regionaloffice", Name: sojResponse.val };
            devNoteText += ". \nSOJ: " + sojResponse.val;
        }

        var ServiceRequest = CrmRestKit2011.Create("va_servicerequest", cols, false);
        ServiceRequest.fail(function (error) {
            UTIL.restKitError(err, 'Error occurred when creating a new service request');
            CloseProgress();
            return null;
        })
            .done(function (data) {
                servicerequestId = data.d.va_servicerequestId;
                var url = '';
                var scCode = '10012';

                if (_currentEnv != null && _currentEnv != undefined && _currentEnv.srCode != null && _currentEnv.srCode != undefined) {
                    scCode = _currentEnv.srCode;
                }
                if (_usingIFD) {
                    url = GetServerUrl() + "/main.aspx?etc=" + scCode + "&extraqs=%3f_CreateFromId%3d" + id +
                        "%26_CreateFromType%3d4210%26_gridType%3d" + scCode + "%26etc%3d" + scCode + "%26id%3d%257b" + data.d.va_servicerequestId +
                        "%257d&pagetype=entityrecord";
                } else {
                    url = GetServerUrl() + "/main.aspx?etc=10004&extraqs=%3f_CreateFromId%3d" +
                        id + "%26_CreateFromType%3d4210%26_gridType%3d10004%26etc%3d10004%26id%3d" +
                        data.d.va_servicerequestId + "%26rskey%3d302341238&pagetype=entityrecord";
                }

                if (doOpen == true) {
                    var width = 1024;
                    var height = 768;
                    var top = (screen.height - height) / 2;
                    var left = (screen.width - width) / 2;
                    var params = "width=" + width + ",height=" + height + ",location=0,menubar=0,toolbar=0,top=" + top + ",left=" + left + ",status=0,titlebar=no,resizable=yes";
                    var win = window.open(url, 'ServiceRequest', params);
                    if (win) {
                        try {
                            win.focus();
                        } catch (err) {
                        }
                    }
                }
                else if (doAlert == true) {
                    alert("New Service Request '" + reqNumber + "' created.");
                }

            });
    }
    catch (e) {
        CloseProgress();
        alert("Error occurred.\n " + e.description);
        return null;
    }
    finally {
        CloseProgress();
        return servicerequestId;
    }
}
_CreateClaimServiceRequest = CreateClaimServiceRequest;
function FormatPhone(areaCode, phoneNumber) {
    var concatPhoneNumber = '';
    var sTmp = '';
    if (areaCode && areaCode != undefined && areaCode != '') {
        sTmp += areaCode;
    }
    if (phoneNumber && phoneNumber != undefined && phoneNumber != '') {
        sTmp += phoneNumber;
    }
    if (sTmp.length == 10) {
        concatPhoneNumber = "(" + sTmp.substr(0, 3) + ") " + sTmp.substr(3, 3) + "-" + sTmp.substr(6, 4);
    }
    else if (sTmp.length == 7) {
        concatPhoneNumber = sTmp.substr(0, 3) + '-' + sTmp.substr(3, 4);
    }
    return concatPhoneNumber;
}

function FormatPathwaysDate(dateStr) {
    if (dateStr && dateStr != undefined && (dateStr.length == 8 || dateStr.length > 8)) {
        dateStr = dateStr.substr(0, 8);
        dateStr = dateStr.substr(4, 4) + dateStr.substr(0, 4);
    }
    return FormatAwardDate(dateStr);
}
function FormatAwardDate(date) {
    if (date && date.length == 8) {
        var month = date.substring(0, 2);
        var day = date.substring(2, 4);
        var year = date.substring(4, 8);
        return new Date(year, month - 1, day);
    }
    else return new Date();
}
function GetNumber(string) {
    var parsedString = parseInt(string);
    if (isNaN(parsedString)) {
        return 0;
    }
    else return parsedString;
}

//=============================================
//  GetSOJ() - Returns the current vet's SOJ guid and name from CRM (award selection necessary if multiple awards).
//=============================================
function GetSOJ(sojVal) {
    var soj = sojVal;

    // Obtain SOJ info.  Check both findGeneralInformationByFileNumber and findGeneralInformationByPtcpntIds for SOJ data
    if (soj == undefined || soj.length == 0) {
        if (Xrm.Page.getAttribute("va_generalinformationresponsebypid") || Xrm.Page.getAttribute("va_generalinformationresponse")) {
            var genInfoXml = Xrm.Page.getAttribute("va_generalinformationresponse").getValue(),
                genInfoXmlObject = genInfoXml ? _XML_UTIL.parseXmlObject(genInfoXml) : null,
                genInfoByPIDXml = Xrm.Page.getAttribute("va_generalinformationresponsebypid").getValue(),
                genInfoByPIDXmlObject = genInfoByPIDXml ? _XML_UTIL.parseXmlObject(genInfoByPIDXml) : null,
                sojCode, resultSet;

            if (genInfoXmlObject && genInfoXmlObject.selectSingleNode('//stationOfJurisdiction') && genInfoXmlObject.selectSingleNode('//stationOfJurisdiction').text != '') {
                soj = genInfoXmlObject.selectSingleNode('//stationOfJurisdiction').text;
            }
            else if (genInfoByPIDXmlObject && genInfoByPIDXmlObject.selectSingleNode('//stationOfJurisdiction')
                && genInfoByPIDXmlObject.selectSingleNode('//stationOfJurisdiction').text != '') {
                soj = genInfoByPIDXmlObject.selectSingleNode('//stationOfJurisdiction').text;
            }

            //but we shouldn't set it as the primary SOJ
            if (soj == '' && Xrm.Page.getAttribute("va_findbirlsresponse") && Xrm.Page.getAttribute("va_findbirlsresponse").getValue()) {
                var birlsXml = _XML_UTIL.parseXmlObject(Xrm.Page.getAttribute("va_findbirlsresponse").getValue());
                if (birlsXml.selectSingleNode('//CLAIM_FOLDER_LOCATION') && birlsXml.selectSingleNode('//CLAIM_FOLDER_LOCATION').text != '') {
                    soj = birlsXml.selectSingleNode('//CLAIM_FOLDER_LOCATION').text;
                }
            }
        }
    }

    var resp = { singleHit: true, val: soj, id: null };

    // data comes as '317 - St. Petersburg'; try to use code
    if (soj && soj.length >= 3) {
        sojCode = soj.substring(0, 3);
        resultSet = CrmRestKit2011.ByQueryAll('va_regionaloffice', ['va_name', 'va_regionalofficeId'], "va_Code eq '" + sojCode + "' ", false);
        resultSet.fail(function (err) {
            UTIL.restKitError(err, 'Failed to retrieve regional office response:');
        })
            .done(function (data) {
                if (data && data.length > 0) {
                    //Return the result set for SOJ
                    resp.val = data[0].va_name;
                    resp.id = data[0].va_regionalofficeId;
                    resp.singleHit = (data.length === 1);
                }
            });
    }

    return resp;
}

//=============================================
//  ChangeOfAddressOnClick()
//=============================================
function ChangeOfAddressOnClick(selection) {
    // set default call type to CAD, if not set already
    if (!selection.ro && typeof _SetPrimaryTypeSubtype == 'function') {
        _SetPrimaryTypeSubtype('CAD', true, false);
    }

    // validate selection
    if (selection && !selection.appealsOnly) {
        if (!selection.openedFromClaimTab) {
            if (!selection.awardTypeCd || !selection.ptcpntVetID || !selection.ptcpntBeneID || !selection.ptcpntRecipID) {
                alert('Failed to receive key information describing selected award recepient.');
                return;
            }
        } else {
            if (!selection.participantClaimantID || !selection.participantVetID || !selection.payeeTypeCode || !selection.programTypeCode) {
                alert('Failed to receive key information describing selected claim recipient.');
                return;
            }
        }

        if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE && !selection.ro) {
            alert('The Phone Call record must be saved prior to initiating Change of Address. Please save the record and try again.');
            return;
        }
        // if call closed, cannot run cad
        if (Xrm.Page.context.getQueryStringParameters().etc != 2 && Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_COMPLETED_ACTIVITY) {
            alert('This Phone Call is closed and Change of Address cannot be executed. To initiate a Change of Address, please open another Phone Call.');
            return;
        }

        // If there is fiduciary activity (anything in Fiduciary name field under contact) or Competency = Incompetent
        var hasPOA = false, hasFid = false, incomp = false;

        var rtContact = new contact();
        var POAFidXml = Xrm.Page.getAttribute('va_findfiduciarypoaresponse').getValue();

        // do not check Fid if it is read only data
        var haveFidAnswer = false;

        if (selection.hasFid != undefined) {
            hasFid = selection.hasFid;
            haveFidAnswer = true;
        }
        else {
            if (POAFidXml && !selection.ro) {
                var POAFidXmlObject = _XML_UTIL.parseXmlObject(POAFidXml);
                rtContact.getFidPOAData(POAFidXmlObject);
                hasPOA = (rtContact.currentPowerOfAttorney != null);
                //hasFid = (rtContact.currentFiduciary != null);
                if (!hasFid) {
                    var searchResponses = new Array();
                    var vetXml = Xrm.Page.getAttribute('va_findcorprecordresponse').getValue();
                    var vetXmlObject;
                    var birlsXml = Xrm.Page.getAttribute('va_findbirlsresponse').getValue();
                    var birlsXmlObject;
                    var genInfoXml = Xrm.Page.getAttribute('va_generalinformationresponsebypid').getValue();
                    var genInfoXmlObject;
                    if (vetXml && vetXml != '') {
                        vetXmlObject = _XML_UTIL.parseXmlObject(vetXml);
                        searchResponses.push(vetXmlObject);
                    }
                    if (birlsXml && birlsXml != '') {
                        birlsXmlObject = _XML_UTIL.parseXmlObject(birlsXml);
                        searchResponses.push(birlsXmlObject);
                    }
                    if (genInfoXml && genInfoXml != '') {
                        genInfoXmlObject = _XML_UTIL.parseXmlObject(genInfoXml);
                    }
                    var searchResults = _XML_UTIL.concatenateDocs(searchResponses, 'SearchResults');
                    if (searchResults && searchResults.xml && searchResults.xml != '') {
                        rtContact.parseCorpRecord(searchResults, genInfoXmlObject, null);
                        incomp = rtContact.IsIncompetent();
                    }
                }
            }
        }

        if (hasFid || incomp) {

            if (confirm('Fiduciary involvement or veteran is incompetent. Change of Address process cannot be initiated.  Please send VAI to the Fiduciary Department.\n\nWould you like to create new VAI Service Request now and open Service Request screen?')) {
                CreateClaimServiceRequest(null, 'Address', true, 'VAI', 2);
            }
            return;
        }
    }

    //VT 2014-12-11: Break out contact check
    //    if (Xrm.Page.data.entity.getEntityName() != 'contact') {            //phone screen
    if (Xrm.Page.data.entity.getEntityName() == 'contact') {
        alert('Please open this screen from the Phone Call page. Calling this feature from the Contact screen is not supported yet.');
        return;
    }
    //VT 2014-12-11: Expand to handle entities other than just phone
    var regardingobjectid = null;

    if ((Xrm.Page.getAttribute("va_regardingobjectid") != null) &&
        (Xrm.Page.getAttribute("va_regardingobjectid").getValue() != null) &&
        (Xrm.Page.getAttribute("va_regardingobjectid").getValue().length > 0) &&
        (Xrm.Page.getAttribute("va_regardingobjectid").getValue()[0] != null)) {
        regardingobjectid = Xrm.Page.getAttribute("va_regardingobjectid").getValue()[0].id.toString().replace('{', '').replace('}', '');
    } else if ((Xrm.Page.getAttribute("regardingobjectid") != null) &&
        (Xrm.Page.getAttribute("regardingobjectid").getValue() != null) &&
        (Xrm.Page.getAttribute("regardingobjectid").getValue().length > 0) &&
        (Xrm.Page.getAttribute("regardingobjectid").getValue()[0] != null)
        ) {
        regardingobjectid = Xrm.Page.getAttribute("regardingobjectid").getValue()[0].id.toString().replace('{', '').replace('}', '');
    } else {
        alert("Please identify a Veteran prior to starting Change of Address process.\n'Regarding' field is blank.");
        return;
    }

    _changeOfAddressData = selection;
            var url = '';
            var id = Xrm.Page.data.entity.getId();

            var cadCode = '10001';
            if (_currentEnv != null && _currentEnv != undefined && _currentEnv.cadCode != null && _currentEnv.cadCode != undefined) {
                cadCode = _currentEnv.cadCode;
            }
            if (_usingIFD) { //
                /*
                 Phone: 4210
                 SR:    10012
                 cad:    10001
                 fnod:   10006
                 callscript        10004
                 */
                if (id != null && id != undefined) {
                    url = GetServerUrl() + '/main.aspx?etc=' + cadCode + '&extraqs=%3f_CreateFromId%3d%257b' +
                        id.toString().replace('{', '').replace('}', '') +
                        '%257d%26_CreateFromType%3d4210%26etc%3d' + cadCode + '&pagetype=entityrecord';
                }
                else {
                    url = GetServerUrl() + '/main.aspx?etc=' + cadCode + '&pagetype=entityrecord';
                }
            }
            else {
                url = GetServerUrl() + '/main.aspx?etc=10001&extraqs=%3f_CreateFromId%3d%257b' + Xrm.Page.getAttribute("regardingobjectid").getValue()[0].id.toString().replace('{', '').replace('}', '') +
                    '%257d%26_CreateFromType%3d2%26etc%3d10001&pagetype=entityrecord';
            }

            window.open(url, 'addresschange', "width=1024,height=768,location=0,menubar=0,toolbar=0,scrollbars=1,resizable=1");
            //            SDK.MetaData.RetrieveEntityAsync(SDK.MetaData.EntityFilters.Entity, 'va_bankaccountt', null, true,
            //                function (entityMetadata) {
            //                    var url = '';
            //                    var id = Xrm.Page.data.entity.getId();
            //                    var objectTypeCode;
            //                    var logicalName;
            //                    if (entityMetadata) {
            //                        objectTypeCode = entityMetadata.ObjectTypeCode;
            //                        logicalName = entityMetadata.LogicalName;
            //                    }
            //                    if (_usingIFD) {
            //                        url = Xrm.Page.context.getServerUrl() + 'main.aspx?etc=' + objectTypeCode + '&extraqs=%3f_CreateFromId%3d%257b' +
            //id.toString().replace('{', '').replace('}', '') +
            //'%257d%26_CreateFromType%3d4210%26etc%3d' + ojbectTypeCode + '&pagetype=entityrecord';
            //                    }
            //                    else {
            //                        url = Xrm.Page.context.getServerUrl() + '/main.aspx?etc=' + objectTypeCode + '&extraqs=%3f_CreateFromId%3d%257b' + Xrm.Page.getAttribute("regardingobjectid").getValue()[0].id.toString().replace('{', '').replace('}', '') +
            //                    '%257d%26_CreateFromType%3d2%26etc%3d' + objectTypeCode + '&pagetype=entityrecord';
            //                    }
            //                    window.open(url, 'addresschange', "width=1024,height=768,location=0,menubar=0,toolbar=0,scrollbars=1,resizable=1");
            //                },
            //                function () {
            //                    alert("Error occurred when creating a new change of address request.");
            //                    return null;
            //                });


}


_ChangeOfAddressOnClick = ChangeOfAddressOnClick;
function TranslateSearchType(continueSearch) {
    // default search
    _searchPathways = false;
    _searchOldPayments = false;


    var findOldPay = Xrm.Page.getAttribute('va_searchmonthofdeath');
    _searchOldPayments = (findOldPay && findOldPay.getValue() == true);

    // check if requested to search all
    if (Xrm.Page.getAttribute('va_searchcorpall').getValue() == true) {
        _searchPathways = true;
        return;
    }

    // final option is Specify Systems to Search. User picks where to search
    //_searchPathways = (crmForm.all.va_searchpathways.DataValue == true);
}

function PathwaysSearchOnChange(tabName, sectionName, showSection) {
    try {
        Xrm.Page.ui.tabs.get(tabName).sections.get(sectionName).setVisible(showSection);
    }
    catch (te) {
    }
}
function AppealsSearchOnChange(tabName, sectionName, showSection) {
    try {
        Xrm.Page.ui.tabs.get(tabName).sections.get(sectionName).setVisible(showSection);
    }
    catch (te) {
    }
}

function IsSingleAward() {
    var numAwards = 0;
    var generalInfoResponse_xmlObject;
    var generalInfoResponseXml = Xrm.Page.getAttribute("va_generalinformationresponse").getValue();
    if (generalInfoResponseXml) {
        generalInfoResponse_xmlObject = _XML_UTIL.parseXmlObject(generalInfoResponseXml);
    }
    if (generalInfoResponse_xmlObject) {
        if (generalInfoResponse_xmlObject && generalInfoResponse_xmlObject.selectSingleNode('//numberOfAwardBenes')) {
            numAwards = generalInfoResponse_xmlObject.selectSingleNode('//numberOfAwardBenes').text;
        }
    }
    return numAwards > 1 ? false : true;
}
function executePostSearchOperations(searchForBIRLS) {
    //Declaration of XML objects from CRM WS response fields
    var searchResponses = new Array();
    var rtContact;
    var vetXml = Xrm.Page.getAttribute('va_findcorprecordresponse').getValue();
    var vetXmlObject;
    var birlsXml = Xrm.Page.getAttribute('va_findbirlsresponse').getValue();
    var birlsXmlObject;
    var genXml = Xrm.Page.getAttribute('va_generalinformationresponsebypid').getValue();
    var genXmlObject = null;
    var poaXml = Xrm.Page.getAttribute('va_findfiduciarypoaresponse').getValue();
    var poaXmlObject = null;
    var vadirPersonXml = Xrm.Page.getAttribute('va_findpersonresponsevadir').getValue();
    var vadirPersonXmlObject;
    var vadirContactXml = Xrm.Page.getAttribute('va_getcontactinfovadir').getValue();
    var vadirContactXmlObject;

    setContext();
    //if XML values exists, parse into XML objects
    if (vetXml && vetXml != '') {
        vetXmlObject = _XML_UTIL.parseXmlObject(vetXml);
        searchResponses.push(vetXmlObject);
    }
    if (birlsXml && birlsXml != '') {
        birlsXmlObject = _XML_UTIL.parseXmlObject(birlsXml);
        searchResponses.push(birlsXmlObject);
    }
    if (genXml && genXml != '') {
        genXmlObject = _XML_UTIL.parseXmlObject(genXml);
    }
    if (poaXml && poaXml != '') {
        poaXmlObject = _XML_UTIL.parseXmlObject(poaXml);
    }
    if (vadirPersonXml && vadirPersonXml != '') {
        vadirPersonXmlObject = _XML_UTIL.parseXmlObject(vadirPersonXml);
        //searchResponses.push(vadirPersonXmlObject);
    }

    if (vadirContactXml && vadirContactXml != '') {
        vadirContactXmlObject = _XML_UTIL.parseXmlObject(vadirContactXml);
        //searchResponses.push(vadirContactXmlObject);
    }

    var searchResults = _XML_UTIL.concatenateDocs(searchResponses, 'SearchResults');

    //If EXISTS: anything in search array
    if ((searchResults && searchResults.xml && searchResults.xml != '') || (vadirPersonXmlObject)) {

        //Create contact records
        rtContact = new contact();
        var rtContactCorp = new contact();
        var rtContactBirls = new contact();
        var rtContactVadir = new contact();

        //Try parsing all record
        var parsedCorpRecord = null;
        var parsedBirlsRecord = null;

        //VTRIGILI 2015-02-10 - This  occasionally throws undefined error 
        //TODO : Need root cause disovery on error,for now just fail silently
        try {
            parsedCorpRecord = rtContactCorp.parseCorpRecord(searchResults, genXmlObject, poaXmlObject);
        } catch (err) {
            parsedCorpRecord = null;
        }
        //VTRIGILI 2015-02-10 - This  occasionally throws undefined error 
        //TODO : Need root cause disovery on error,for now just fail silently
        try {
            if (searchForBIRLS) {
                parsedBirlsRecord = rtContactBirls.parseBIRLSRecord(searchResults);
        }
        } catch (err) {
            parsedBirlsRecord = null;
        }
        if (vadirPersonXmlObject) {
            var parsedVadirRecord = rtContactVadir.parseVadirRecord(vadirPersonXmlObject, vadirContactXmlObject);
        }
        //Scenario 1: CORP exists and BIRLS exists
        if ((parsedCorpRecord) && (searchForBIRLS)) {  //(parsedBirlsRecord)) {
            rtContact = rtContactCorp;
            //Add BIRLS fields to CORP dataset
            if (parsedBirlsRecord) {
                rtContact.BIRLSEmployeeStation = rtContactBirls.BIRLSEmployeeStation;
                rtContact.va_StaionofJurisdictionId = rtContactBirls.va_StaionofJurisdictionId;
                rtContact.rpo = rtContactBirls.rpo;
                rtContact.CLAIM_NUMBER = rtContactBirls.fileNumber;
            }
        }
            //Scenario 2: CORP exists and No BIRLS
        else if ((parsedCorpRecord) && (!parsedBirlsRecord)) {
            rtContact = rtContactCorp;
        }
            //Scenario 3: No CORP and BIRLS exists
        else if ((!parsedCorpRecord) && (parsedBirlsRecord)) {
            rtContact = rtContactBirls;
        }
            //Scenario 4: No CORP; NO BIRLS; VADIR exists
        else if ((!parsedCorpRecord) && (!parsedBirlsRecord) && (parsedVadirRecord)) {
            rtContact = rtContactVadir;
        }
            //Scenario 5: No CORP; NO BIRLS; no VADIR
        else if ((!parsedCorpRecord) && (!parsedBirlsRecord) && (!parsedVadirRecord)) {

            var PID = rtContact.participantId ? rtContact.participantId : '';
            var social = rtContact.ssn ? rtContact.ssn : Xrm.Page.getAttribute('va_ssn').getValue();
            var columns = ['va_SSN', 'FullName', 'ContactId', 'va_SMSPaymentNotices', 'va_EmailPaymentNotices', 'va_EmailClaimNotices',
                'PreferredContactMethodCode', 'PreferredAppointmentDayCode', 'PreferredAppointmentTimeCode', 'va_SelfServiceNotifications',
                'EMailAddress1', 'va_PrimaryPhone', 'va_HasEBenefitsAccount', 'va_TimeZone',
                'Address1_Telephone1', 'Address1_Telephone2', 'Address1_Telephone3', 'Address2_Telephone1', 'Address2_Telephone2', 'FirstName', 'LastName'];
            var filter = (PID != '') ? "va_ParticipantID eq '" + PID + "'" : "va_SSN eq '" + social + "'";
            var collectionResults = CrmRestKit2011.ByQueryAll('Contact', columns, filter);
            collectionResults.fail(function (err) {
                UTIL.restKitError(err, 'Failed to retrieve Contact/ Veteran response:');
            })
                .done(function (collection) {
                    if (collection && collection.length > 0) {
                        idValue = collection[0].ContactId;
                        textValue = collection[0].FullName;

                        //VTRIGILI - 04-08-2015
                        // ADD IF branching to support VIP Entity
                        if (Xrm.Page.getAttribute('regardingobjectid') != null) {
                            Xrm.Page.getAttribute('regardingobjectid').setValue([{ id: idValue, name: textValue, entityType: 'contact' }]);
                        } else if (Xrm.Page.getAttribute('va_regardingobjectid') != null) {
                            Xrm.Page.getAttribute('va_regardingobjectid').setValue([{ id: idValue, name: textValue, entityType: 'contact' }]);
                        }
                        Xrm.Page.getAttribute('va_firstname').setValue(collection[0].FirstName);
                        Xrm.Page.getAttribute('va_lastname').setValue(collection[0].LastName);

                    }
                    else if (collection.length === 0) {
                        Xrm.Page.getAttribute('va_ssn').setRequiredLevel('required');
                        Xrm.Page.getAttribute('va_firstname').setRequiredLevel('required');
                        Xrm.Page.getAttribute('va_lastname').setRequiredLevel("required");
                        //VTRIGILI 04-08-2015
                        //ADD: If condition to support VIP Entity
                        if (Xrm.Page.getAttribute('va_createcontact') != null) {
                            Xrm.Page.getAttribute('va_createcontact').setSubmitMode("always");
                            Xrm.Page.getAttribute('va_createcontact').setValue(1);
                            Xrm.Page.ui.controls.get('va_createcontact').setDisabled(false);
                        }
                    }
                });
        }
        if (!_isLoading) {
            MarkAsRelatedVeteran(rtContact);
        }

        return rtContact;
    }
}

function defineResponseAttributes() {
    //Please make sure the order of array elements matches the order of fields on Phone/Contact forms
    _responseAttributes = [
        Xrm.Page.getAttribute("va_webserviceresponse"),
        Xrm.Page.getAttribute("va_findcorprecordresponse"),
        Xrm.Page.getAttribute("va_findbirlsresponse"),
        Xrm.Page.getAttribute("va_findveteranresponse"),
        Xrm.Page.getAttribute("va_generalinformationresponse"),
        Xrm.Page.getAttribute("va_generalinformationresponsebypid"),
        Xrm.Page.getAttribute("va_findaddressresponse"),
        Xrm.Page.getAttribute("va_benefitclaimresponse"),
        Xrm.Page.getAttribute("va_findbenefitdetailresponse"),
        Xrm.Page.getAttribute("va_findclaimstatusresponse"),
        Xrm.Page.getAttribute("va_findclaimantlettersresponse"),
        Xrm.Page.getAttribute("va_findcontentionsresponse"),
        Xrm.Page.getAttribute("va_finddependentsresponse"),
        Xrm.Page.getAttribute("va_findallrelationshipsresponse"),
        Xrm.Page.getAttribute("va_finddevelopmentnotesresponse"),
        Xrm.Page.getAttribute("va_findfiduciarypoaresponse"),
        Xrm.Page.getAttribute("va_findmilitaryrecordbyptcpntidresponse"),
        Xrm.Page.getAttribute("va_findpaymenthistoryresponse"),
        Xrm.Page.getAttribute("va_findtrackeditemsresponse"),
        Xrm.Page.getAttribute("va_findunsolvedevidenceresponse"),
        Xrm.Page.getAttribute("va_finddenialsresponse"),
        Xrm.Page.getAttribute("va_findawardcompensationresponse"),
        Xrm.Page.getAttribute("va_findotherawardinformationresponse"),
        Xrm.Page.getAttribute("va_findmonthofdeathresponse"),
        Xrm.Page.getAttribute("va_findincomeexpenseresponse"),
        Xrm.Page.getAttribute("va_findratingdataresponse"),
        Xrm.Page.getAttribute("va_findappealsresponse"),
        Xrm.Page.getAttribute("va_findindividualappealsresponse"),
        Xrm.Page.getAttribute("va_appellantaddressresponse"),
        Xrm.Page.getAttribute("va_updateappellantaddressresponse"),
        Xrm.Page.getAttribute("va_createnoteresponse"),
        Xrm.Page.getAttribute("va_findreasonsbyrbaissueidresponse"),
        Xrm.Page.getAttribute("va_isaliveresponse"),
        Xrm.Page.getAttribute("va_mviresponse"),
        Xrm.Page.getAttribute("va_readdataexamresponse"),
        Xrm.Page.getAttribute("va_readdataappointmentresponse"),
        Xrm.Page.getAttribute("va_awardfiduciaryresponse"),
        Xrm.Page.getAttribute("va_retrievepaymentsummaryresponse"),
        Xrm.Page.getAttribute("va_retrievepaymentdetailresponse"),
        Xrm.Page.getAttribute("va_getregistrationstatus"),
        Xrm.Page.getAttribute("va_findgetdocumentlist"),
        Xrm.Page.getAttribute("va_findpersonresponsevadir"),
        Xrm.Page.getAttribute("va_getcontactinfovadir")
    ];

    _responseAttributesWithAggregation = [
        //"va_generalinformationresponsebypid", // we only care about selected one, which is the one cached
        "va_findbenefitdetailresponse",
        "va_findclaimstatusresponse",
        "va_findclaimantlettersresponse",
        "va_findcontentionsresponse",
        "va_findindividualappealsresponse",
        "va_retrievepaymentdetailresponse"
    ];
}
function QueryDevNotes(columns, filter) {
    var queryResults = null;
    CrmRestKit2011.ByQueryAll('va_querylog', columns, filter, false)
        .fail(function (err) {
            UTIL.restKitError(err, 'Failed to retrieve Query Log response:');
        })
        .done(function (data) {
            queryResults = data[0];
        });
    return queryResults;
}
_QueryDevNotes = QueryDevNotes;
function CreateDevNoteLogEntry(createNoteDetailresponseXml, devNoteText) {
    var createNote_xmlObject = _XML_UTIL.parseXmlObject(createNoteDetailresponseXml);
    var noteId = null;
    if (createNote_xmlObject.selectSingleNode('//noteId') != null) {
        noteId = createNote_xmlObject.selectSingleNode('//noteId').text;
    }
    //if (noteId) {
    // add a log entry, also used to validate permission to edit note
    var msg = 'SR Note create; ID: ' + noteId;

    var cols = {
        va_Error: false,
        va_Warning: false,
        va_Summary: true,
        va_name: msg,
        va_Description: devNoteText,
        va_NoteId: noteId,
        va_Request: '',
        va_Query: '',
        va_Duration: 0,
        va_PhoneCallId: { 'Id': Xrm.Page.data.entity.getId() }
    };
    var log = CrmRestKit2011.Create("va_querylog", cols, false)
        .fail(function (error) {
            UTIL.restKitError(err, 'Failed to create dev note');
        });

    //}
    //
}
_CreateDevNoteLogEntry = CreateDevNoteLogEntry;
// TODO: complete
function ValidatePermissionToEditNote(noteId) {
}
_ValidatePermissionToEditNote = ValidatePermissionToEditNote;
function GetTransactionCurrencyInfo() {
    var currencyObj = null;
    var columns = ['TransactionCurrencyId', 'ISOCurrencyCode', 'CurrencyName'];
    var transactionCurrencyObjs = CrmRestKit2011.ByQuery('TransactionCurrency', columns);
    transactionCurrencyObjs.fail(function (err) {
        UTIL.restKitError(err, 'Failed to retrieve Transaction Currency response:');
    }
    )
    transactionCurrencyObjs.done(function (data) {
        if (data && data.d.results && data.d.results.length > 0) {
            var transactionCurrencyObj = data.d.results[0];
            currencyObj = { id: data.d.results[0].TransactionCurrencyId, code: data.d.results[0].ISOCurrencyCode, name: data.d.results[0].CurrencyName };
        }
    });
    return currencyObj;
}
function IsFileNumber(number) {
    var isFileNumber = false;
    if (number && number != undefined && number.length > 0 && number.length < 9) {
        isFileNumber = true;
    }
    return isFileNumber;
}
function GetBirlsSectionName() {
    var birlsName = '';
    var birlsXml = Xrm.Page.getAttribute('va_findbirlsresponse').getValue();
    if (birlsXml && birlsXml != undefined && birlsXml != '') {
        var birlsXmlObject = _XML_UTIL.parseXmlObject(birlsXml);
        var firstName = SingleNodeExists(birlsXmlObject, '//FIRST_NAME') ? birlsXmlObject.selectSingleNode('//FIRST_NAME').text : null;
        var lastName = SingleNodeExists(birlsXmlObject, '//LAST_NAME') ? birlsXmlObject.selectSingleNode('//LAST_NAME').text : null;
        if (firstName && firstName != undefined && firstName != '') {
            birlsName += firstName;
        }
        if (lastName && lastName != undefined && lastName != '') {
            birlsName += ' ' + lastName;
        }
    }
    return birlsName;
}
function GetServerUrl() {
    var url = Xrm.Page.context.getClientUrl();
    if (url && url.match(/\/$/)) {
        url = url.substring(0, url.length - 1);
    }
    return url;
}
function UpdateSearchOptionsObject(ctx) {
    SearchOptionsList.clear();

    SearchOptionsList.add({
        searchPathways: Xrm.Page.getAttribute('va_searchcorpall').getValue(),
        fileNumber: ctx.parameters['fileNumber'], ssn: ctx.parameters['ssn'], ptcpntId: ctx.parameters['ptcpntId'],
        ptcpntVetId: ctx.parameters['ptcpntVetId'], ptcpntBeneId: ctx.parameters['ptcpntBeneId'],
        ptcpntRecipId: ctx.parameters['ptcpntRecipId'], awardTypeCd: ctx.parameters['awardTypeCd'],
        awardKey: ctx.parameters['awardKey'], payeeSSN: ctx.parameters['payeeSSN']
    }, false);
}

function UpdateSearchListObject(searchObj) {
    SearchList.add(searchObj, true);
}

function JSONStore() {
    this.rawData;
};

JSONStore.prototype = {
    read: function () {
        throw 'Read function not implemented.';
    },

    write: function () {
        throw 'Write function not implemented.';
    }
};

// RU12 the class is built wrong

var CRMJSONStore = function (formAttributeName) {
    this.rawData = '';
    this.formAttributeName = formAttributeName;
};
CRMJSONStore.prototype = new JSONStore;
CRMJSONStore.prototype.constructor = CRMJSONStore;

CRMJSONStore.prototype.read = function () {
    if (!this.formAttributeName || this.formAttributeName == '') {
        throw 'Form attribute name must be set';
        return;
    }
    if (!Xrm.Page.getAttribute(this.formAttributeName)) {
        throw 'Form attribute does not exist';
        return;
    }

    this.rawData = Xrm.Page.getAttribute(this.formAttributeName) && Xrm.Page.getAttribute(this.formAttributeName).getValue() ? Xrm.Page.getAttribute(this.formAttributeName).getValue() : '';

    if (this.rawData == '') {
        return [];
    }
    else {
        return JSON.parse(this.rawData);
    }
};

CRMJSONStore.prototype.overWrite = function (index, data, append) {
    if (index == -1) {
        return;
    }

    if (!this.formAttributeName || this.formAttributeName == '') {
        throw 'Form attribute name must be set';
        return;
    }
    if (!Xrm.Page.getAttribute(this.formAttributeName)) {
        throw 'Form attribute does not exist';
        return;
    }

    if (append) {
        var storedData = this.read();
        if (storedData[index] && storedData[index] != undefined) {
            storedData[index] = data;
        }
        this.rawData = JSON.stringify(storedData);
    }
    else {
        this.rawData = JSON.stringify(data);
    }

    Xrm.Page.getAttribute(this.formAttributeName).setValue(this.rawData);
};

CRMJSONStore.prototype.write = function (data, append) {
    if (!this.formAttributeName || this.formAttributeName == '') {
        throw 'Form attribute name must be set';
        return;
    }
    if (!Xrm.Page.getAttribute(this.formAttributeName)) {
        throw 'Form attribute does not exist';
        return;
    }

    if (append) {
        var storedData = this.read();
        storedData.push(data);
        this.rawData = JSON.stringify(storedData);
    }
    else {
        this.rawData = JSON.stringify(data);
    }

    Xrm.Page.getAttribute(this.formAttributeName).setValue(this.rawData);
};

CRMJSONStore.prototype.clear = function () {
    this.rawData = '';
    Xrm.Page.getAttribute(this.formAttributeName).setValue(this.rawData);
};

var WebServiceExecutionStatusList =
    (function () {
        var crmJsonSource = new CRMJSONStore('va_webserviceexecutionstatus');

        function exists(webServiceName) {
            var webServiceExecutionStatusList = crmJsonSource.read();
            var found = false;

            for (var i in webServiceExecutionStatusList) {
                var webService = webServiceExecutionStatusList[i];

                if (webService.name == webServiceName) {
                    found = true;
                }
            }

            return found;
        }

        function getIndex(webServiceName) {
            var webServiceExecutionStatusList = crmJsonSource.read();
            var index = -1;

            for (var i in webServiceExecutionStatusList) {
                var webService = webServiceExecutionStatusList[i];

                if (webService.name == webServiceName) {
                    index = i;
                    break;
                }
            }

            return index;
        }

        function get() {
            return crmJsonSource.read();
        };

        function add(webServiceConfig, append) {
            if (!append) {
                append = false;
            }
            if (webServiceConfig instanceof Array) {
                for (var i in webServiceConfig) {
                    if (!exists(webServiceConfig[i].name)) {
                        crmJsonSource.write(webServiceConfig[i], append);
                    }
                }
            }
            else {
                if (!exists(webServiceConfig.name)) {
                    crmJsonSource.write(webServiceConfig, append);
                }
                else {
                    crmJsonSource.overWrite(getIndex(webServiceConfig.name), webServiceConfig, append);
                }
            }
        };

        function clear() {
            crmJsonSource.clear();
        }

        return {
            get: get,
            add: add,
            clear: clear,
            exists: exists
        };
    }());

var SearchOptionsList =
    (function () {
        var crmJsonSource = new CRMJSONStore('va_searchoptionsobject');

        function exists(webServiceName) {
            var searchOptionsList = crmJsonSource.read();
            var found = false;

            for (var i in searchOptionsList) {
                var search = searchOptionsList[i];

                if (search.name == webServiceName) {
                    found = true;
                }
            }

            return found;
        }

        function get() {
            return crmJsonSource.read();
        };

        function add(searchConfig, append) {
            if (!append) {
                append = false;
            }
            if (searchConfig instanceof Array) {
                for (var i in searchConfig) {
                    crmJsonSource.write(searchConfig[i], append);
                }
            }
            else {
                crmJsonSource.write(searchConfig, append);
            }
        };

        function clear() {
            crmJsonSource.clear();
        }

        return {
            get: get,
            add: add,
            clear: clear
        };
    }());

var SearchList =
    (function () {
        var crmJsonSource = new CRMJSONStore('va_searchactionscompleted');

        function exists(searchName) {
            var searchList = crmJsonSource.read();
            var found = false;

            for (var i in searchList) {
                var search = searchList[i];

                if (search.name == searchName && search.complete == true) {
                    found = true;
                }
            }

            return found;
        }

        function get() {
            return crmJsonSource.read();
        };

        function add(searchConfig, append) {
            if (!append) {
                append = false;
            }
            if (searchConfig instanceof Array) {
                for (var i in searchConfig) {
                    crmJsonSource.write(searchConfig[i], append);
                }
            }
            else {
                crmJsonSource.write(searchConfig, append);
            }
        };

        function clear() {
            crmJsonSource.clear();
        }

        return {
            get: get,
            add: add,
            clear: clear,
            exists: exists
        };
    }());

var SearchIn =
    (function () {
        return {
            Corp_Min: 953850000,
            Corp_All: 953850002,
            Sys_All: 953850007,
            Sys_Spec: 953850001
        };
    }());

function SearchActionsComplete(searchObj) {
    var searchComplete = false;
    if (searchObj.pathwaysComplete) {
        searchComplete = true;
    }
    return searchComplete;
}

function GetSearchCounter() {
    var searchCounter = 0;
    var lastExecutedSearch = JSON.parse(Xrm.Page.getAttribute('va_lastexecutedsearch').getValue());

    if (lastExecutedSearch && lastExecutedSearch != undefined) {
        searchCounter = lastExecutedSearch.searchCounter;
    }

    return searchCounter;
}


function getPhoneNumberFromContact(rtContact) {
    if (!rtContact) return "";

    var phone = parseContactPhoneNumber(rtContact.areaNumberOne, rtContact.phoneNumberOne);

    if (!phone)
        phone = parseContactPhoneNumber(rtContact.areaNumberTwo, rtContact.phoneNumberTwo);

    return phone ? formatContactPhoneNumber(phone.number, phone.isDomestic) : "";
}

function parseContactPhoneNumber(area, number) {
    var phone = null;

    if (area || number) {
        phone = { number: "", isDomestic: false };

        if (area) {
            phone.isDomestic = true;
            phone.number = area;
        }

        phone.number += (number || "");
    }

    return phone;
}

function formatContactPhoneNumber(phoneNumber, isDomestic) {
    if (!phoneNumber) return "";

    var cleanNumber = phoneNumber.replace(/\W|_|[a-z]|[A-Z]/g, '');

    if (isDomestic) {
        if (cleanNumber.length === 10)
            return "(" + cleanNumber.substr(0, 3) + ") " + cleanNumber.substr(3, 3) + "-" + cleanNumber.substr(6);

        return cleanNumber;
    }

    return phoneNumber;
}

