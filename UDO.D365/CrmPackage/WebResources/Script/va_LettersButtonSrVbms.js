/**
 * Created by VHAISDBLANCW on 10/29/2014.
 */

var VbmsFileUpload = {

    CreateNote: function (controlId) {
        var action = "";
        var fileName = "";
        var parameters = "";
        switch (controlId) {
            case 'ISV.Dynamic.AmountLetterGross.ServiceConnectedDisabilityCompensation':
                action = 'AmountLetterGross';
                fileName = 'Amount Letter Gross';
                parameters = 'Type:1|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.AmountLetterGross.NonServiceConnectedVeteransPension':
                action = 'AmountLetterGross';
                fileName = 'Amount Letter Gross';
                parameters = 'Type:2|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.AmountLetterGross.DeathPensionn':
                action = 'AmountLetterGross';
                fileName = 'Amount Letter Gross';
                parameters = 'Type:3|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.AmountLetterGross.DependencyIndemnityCompensation':
                action = 'AmountLetterGross';
                fileName = 'Amount Letter Gross';
                parameters = 'Type:4|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.BlankLetter':
                action = 'BlankLetter';
                fileName = "Blank Letter";
                parameters = 'ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.CommissaryLetter.CurrentRatingDegree':
                action = 'CommissaryLetter';
                fileName = 'Commissary Letter';
                parameters = 'Variation:1|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.CommissaryLetter.IndividualUnemployability':
                action = 'CommissaryLetter';
                fileName = 'Commissary Letter';
                parameters = 'Variation:2|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.CommissaryLetterSurvivingSpouse':
                action = 'CommissaryLetterSurvivingSpouse';
                fileName = 'Commissary Letter Surviving Spouse';
                parameters = 'ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.CommissaryLetterWithFutureExam':
                action = 'CommissaryLetterWithFutureExam';
                fileName = 'Commissary Letter with Future Exam';
                parameters = 'ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.DeathBenefitsLetter':
                action = 'DeathBenefitsLetter';
                fileName = 'Death Benefits Letter';
                parameters = 'ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.DisabilityBreakdownLetter':
                action = 'DisabilityBreakdownLetter';
                fileName = 'Disability Breakdown Letter';
                parameters = 'ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.IncomeAmountLetter.AmountOnly':
                action = 'IncomeAmountLetter';
                fileName = 'Income Amount Letter';
                parameters = 'Type:1|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.IncomeAmountLetter.ApportioneeAmountOnly':
                action = 'IncomeAmountLetter';
                fileName = 'Income Amount Letter';
                parameters = 'Type:8|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.IncomeAmountLetter.Compensation':
                action = 'IncomeAmountLetter';
                fileName = 'Income Amount Letter';
                parameters = 'Type:2|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.IncomeAmountLetter.CompensationAndIu':
                action = 'IncomeAmountLetter';
                fileName = 'Income Amount Letter';
                parameters = 'Type:3|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.IncomeAmountLetter.CompensationAndPercentageDisability':
                action = 'IncomeAmountLetter';
                fileName = 'Income Amount Letter';
                parameters = 'Type:4|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.IncomeAmountLetter.VeteranNonServiceConnectedPension':
                action = 'IncomeAmountLetter';
                fileName = 'Income Amount Letter';
                parameters = 'Type:5|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.IncomeAmountLetter.WidowDeathPension':
                action = 'IncomeAmountLetter';
                fileName = 'Income Amount Letter';
                parameters = 'Type:6|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.IncomeAmountLetter.WidowDependencyAndIndemnityCompensation':
                action = 'IncomeAmountLetter';
                fileName = 'Income Amount Letter';
                parameters = 'Type:7|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.MultiplePaymentLetter':
                action = 'MultiplePaymentLetter';
                fileName = 'Multiple Payment Letter';
                parameters = 'ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.NoBenefitsLetter':
                action = 'NoBenefitsLetter';
                fileName = 'No Benefits Letter';
                parameters = 'ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.NOKLetter':
                action = 'NOKLetter';
                fileName = 'NOK Letter';
                parameters = 'ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.PreferenceLetters.NoSCD':
                action = 'PreferenceLetters';
                fileName = 'Preference Letters';
                parameters = 'PreferenceLetterType:2|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.PreferenceLetters.10To20Percent':
                action = 'PreferenceLetters';
                fileName = 'Preference Letters';
                parameters = 'PreferenceLetterType:3|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.PreferenceLetters.GreaterThan30Percent':
                action = 'PreferenceLetters';
                fileName = 'Preference Letters';
                parameters = 'PreferenceLetterType:4|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.PreferenceLetters.MotherVet100Percent':
                action = 'PreferenceLetters';
                fileName = 'Preference Letters';
                parameters = 'PreferenceLetterType:5|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.PreferenceLetters.MotherOfDeceasedVetFromServiceConnDisability':
                action = 'PreferenceLetters';
                fileName = 'Preference Letters';
                parameters = 'PreferenceLetterType:6|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.PreferenceLetters.NonServiceConnPension':
                action = 'PreferenceLetters';
                fileName = 'Preference Letters';
                parameters = 'PreferenceLetterType:7|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.PreferenceLetters.PurpleHeart':
                action = 'PreferenceLetters';
                fileName = 'Preference Letters';
                parameters = 'PreferenceLetterType:8|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.PreferenceLetters.SpouseOf100PercentServiceConnVet':
                action = 'PreferenceLetters';
                fileName = 'Preference Letters';
                parameters = 'PreferenceLetterType:9|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.PreferenceLetters.SpouseOfDisabledDeceasedVetFromServConnDisability':
                action = 'PreferenceLetters';
                fileName = 'Preference Letters';
                parameters = 'PreferenceLetterType:10|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.PreferenceLetters.0PercentInReceipt':
                action = 'PreferenceLetters';
                fileName = 'Preference Letters';
                parameters = 'PreferenceLetterType:11|ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.PropertyTaxDmvRegistrationMt':
                action = 'PropertyTaxDmvRegistrationMt';
                fileName = 'Property TAX DMV Registration - MT';
                parameters = 'ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.PropertyTaxExemptionCaLa':
                action = 'PropertyTaxExemptionCaLa';
                fileName = 'Property Tax Exemption - CA - LA';
                parameters = 'ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.RealEstateLicenseFeeTaxExemption':
                action = 'RealEstateLicenseFeeTaxExemption';
                fileName = 'Real Estate - License Fee Tax Exemption';
                parameters = 'ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.ServiceVerification':
                action = 'ServiceVerification';
                fileName = 'Service Verification';
                parameters = 'ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.SummaryOfBenefitsSurvivingSpouseLetter':
                action = 'SummaryOfBenefitsSurvivingSpouseLetter';
                fileName = 'Summary of Benefits Surviving Spouse Letter';
                parameters = 'ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.SummaryOfBenefitsVeteransLetter':
                action = 'SummaryOfBenefitsVeteransLetter';
                fileName = 'Summary of Benefits Veterans Letter';
                parameters = 'ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.VeteranPercentageLetter':
                action = 'VeteranPercentageLetter';
                fileName = 'Veteran Percentage Letter';
                parameters = 'ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            case 'ISV.Dynamic.VeteranPTLetter':
                action = 'VeteranPTLetter';
                fileName = 'Veteran PT Letter';
                parameters = 'ServiceRequestGUID:' + Xrm.Page.data.entity.getId();
                break;
            default:
                alert('Button Unknown');
        }
        var ssId = Xrm.Page.getAttribute("va_filenumber").getValue();
        if (ssId != null) {
            if (confirm("Did you intend to upload the \"" + fileName + "\" letter for xxx-xx-" + ssId.substr(ssId.length - 4))) { // call ws to get the attachment file names from shared folder
                var names = '';
                var request = null;
                try {
                    request = new ActiveXObject('Microsoft.XMLHTTP');
                } catch (err) {
                }

                if ((request == null) && window.XMLHttpRequest) {
                    request = new XMLHttpRequest();
                } else if (request == null) {
                    alert('Exception: Failed to create XML HTTP Object. Failed to execute web service request');
                    return false;
                }

                var serviceUri = _currentEnv.RepWS;//"https://crmdac.crmud.dev.crm.vrm.vba.va.gov/reportgen/reportgen.asmx";
                //var action = Xrm.Page.getAttribute("va_action").getSelectedOption().text;
                var disbSubType = "";//Xrm.Page.getAttribute("va_disposition").getText();
                var arrFiles = null;

                if (disbSubType == undefined) disbSubType = "";

                //var env = '<?xml version="1.0" encoding="utf-8"?><soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">' + '<soap12:Body><DownloadReport xmlns="http://tempuri.org/">' + '<serviceRequestId>' + Xrm.Page.data.entity.getId() + '</serviceRequestId>' + '<action>' + action.replace('&', '&amp;') + '</action>' + '<dispSubType>' + disbSubType.replace('&', '&amp;') + '</dispSubType>' + '</DownloadReport></soap12:Body></soap12:Envelope>';
                var env = '<?xml version="1.0" encoding="utf-8"?><soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">' +
                    '<soap12:Body><DownloadReportMultipleParameters xmlns="http://tempuri.org/">' +
                    '<recordId>' + Xrm.Page.data.entity.getId() + '</recordId>' + '<reportName>' +
                    fileName + '</reportName>' + '<parameters>' + parameters + '</parameters>' +
                    '</DownloadReportMultipleParameters></soap12:Body></soap12:Envelope>';

                try {
                    ShowProgress('Generating Report Output');

                    request.open('POST', serviceUri, false);
                    request.setRequestHeader('SOAPAction', '');
                    request.setRequestHeader('Content-Type', 'text/xml; charset=utf-8');
                    request.setRequestHeader('Content-Length', env.length);

                    request.send(env);
                } catch (rex) {
                    request = null;
                    alert('Call to the Web Service to generate the report had failed: ' + rex.description);
                    return false;
                } finally {
                    CloseProgress();
                }

                names = request.responseText;
                if (names) {
                    var rx = parseXmlObject(names);
                    //var resNode = rx.selectSingleNode('//DownloadReportResult');
                    var resNode = rx.selectSingleNode('//DownloadReportMultipleParametersResult');
                    if (!resNode) {
                        names = 'Exception: No response from Report Generation WS.';
                    } else {
                        names = resNode.text;
                    }
                }
                request = null;

                if (!names || names.length == 0 || names.indexOf('Exception') != -1) {
                    alert('Call to Report Generation Web Service had failed or no reports had been generated.\n\n' + names);
                    return false;
                }
                arrFiles = names.split(',');
                var path = arrFiles[0];
                var body = new Array();
                var retrieveEntityReq = new XMLHttpRequest();
                retrieveEntityReq.open("GET", path, false);
                retrieveEntityReq.setRequestHeader("Accept", "*/*");
                retrieveEntityReq.send();
                var dataBinary = new VBArray(retrieveEntityReq.responseBody).toArray();
                var data64 = VbmsFileUpload.Encode64(dataBinary);

                var vbmsDocumentInfo = new Object();
                vbmsDocumentInfo.crme_EntityName = Xrm.Page.data.entity.getEntityName();
                vbmsDocumentInfo.crme_RecordId = Xrm.Page.data.entity.getId();
                vbmsDocumentInfo.crme_name = 'VBMS Upload';
                /*vbmsDocumentInfo.crme_DocumentType = "474";
                 vbmsDocumentInfo.crme_NoteId = noteId;*/
                VbmsFileUpload.CreateEntity(vbmsDocumentInfo, "crme_vbmsdocumentinfo", "");

                var note = Object();
                note.DocumentBody = data64;
                note.Subject = "VbmsFileUploadCreate";
                note.FileName = fileName + ".pdf";
                note.MimeType = "application/pdf";
                note.ObjectId = Object();
                note.ObjectId.LogicalName = Xrm.Page.data.entity.getEntityName();
                note.ObjectId.Id = Xrm.Page.data.entity.getId();
                var noteData = VbmsFileUpload.CreateEntity(note, "Annotation", "");
                var noteId = noteData.AnnotationId;

                setInterval(function () {
                        if (validInput(window.location.href)) {
                            window.location = encodeURI(window.location.href);
                        }
                        else {
                            console.log("Invalid character(s) found in url: " + window.location.href);
                        }

                        location.reload(true);
                    }, 1000
                );
            }
        } else {
            alert("Please enter File/Claim number then save.");
        }
    },

    validInput: function (text) {
        var reg = /^[a-z0-9!?@#\$%\^\&*\)\(+=._-]+$/i;
        if (reg.test(text)) {
            return true;
        }
        else {
            return false;
        }
    },

    CreateEntity: function (entity, entityName, update) {
        var jsonEntity = JSON.stringify(entity);
        var createEntityReq = new XMLHttpRequest();
        var ODataPath = Xrm.Page.context.getServerUrl() + "/XRMServices/2011/OrganizationData.svc";
        createEntityReq.open("POST", ODataPath + "/" + entityName + "Set" + update, false);
        createEntityReq.setRequestHeader("Accept", "application/json");
        createEntityReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        createEntityReq.send(jsonEntity);
        var newEntity = JSON.parse(createEntityReq.responseText).d;
        return newEntity;
    },

    UpdateNotes: function (noteId) {
        // create notes object
        var objNotes = new Object();
        objNotes.Subject = "VbmsFileUploadUpdate";

        // Pass the guid of the annotation.
        var annotationId = noteId.toUpperCase();//'A1A04803-6310-E211-8C0E-78E3B511A681';
        var jsonEntity = window.JSON.stringify(objNotes);

        var serverUrl = Xrm.Page.context.getServerUrl();
        var ODATA_ENDPOINT = "/XRMServices/2011/OrganizationData.svc/AnnotationSet";
        var ODataPath = serverUrl + ODATA_ENDPOINT;
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: ODataPath + "(guid'" + annotationId + "')",
            data: jsonEntity,
            beforeSend: function (XMLHttpRequest) {
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
                XMLHttpRequest.setRequestHeader("X-HTTP-Method", "MERGE");
            },
            error: function (xmlHttpRequest, textStatus, errorThrown) {
                alert("Status: " + textStatus + "; ErrorThrown: " + errorThrown);
            }
        });
    },

    /*UpdateEntity: function (entity, entityName, update) {
     debugger;
     var jsonEntity = JSON.stringify(entity);
     var createEntityReq = new XMLHttpRequest();
     var ODataPath = Xrm.Page.context.getServerUrl() + "/XRMServices/2011/OrganizationData.svc";
     createEntityReq.open("POST", ODataPath + "/" + entityName + "Set" + update, false);
     createEntityReq.setRequestHeader("Accept", "application/json");
     createEntityReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
     createEntityReq.send(jsonEntity);
     var newEntity = JSON.parse(createEntityReq.responseText).d;
     return newEntity;
     },*/

    //get logged on user
    Getinfo: function () {
        var fullname;
        var context;
        var serverUrl;
        var UserID;
        var ODataPath;
        context = Xrm.Page.context;
        serverUrl = context.getServerUrl();
        UserID = context.getUserId();
        ODataPath = serverUrl + "XRMServices/2011/OrganizationData.svc";
        var retrieveUserReq = new XMLHttpRequest();
        retrieveUserReq.open("GET", ODataPath + "/SystemUserSet(guid'" + UserID + "')", false);
        retrieveUserReq.setRequestHeader("Accept", "application/json");
        retrieveUserReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        retrieveUserReq.onreadystatechange = function () {
            fullname = retrieveUserReqCallBack(this);
        };
        retrieveUserReq.send();
        return fullname;
    },

    retrieveUserReqCallBack: function (retrieveUserReq) {
        if (retrieveUserReq.readyState == 4 /* complete */) {
            if (retrieveUserReq.status == 200) {
                var retrievedUser = (this.parent.JSON) ? this.parent.JSON.parse(retrieveUserReq.responseText).d : this.JSON.parse(retrieveUserReq.responseText).d;
                if (retrievedUser.FullName != null)
                    return retrievedUser.FullName;

                else {
                    alert("Error in Fetching User data");
                }
            }
        }
    },

    Letters: function (CommandProperties) {
        var menuXmlAction = '<Menu Id=\"ISV.DynamicMenu.VbmsFileUpload\">' +
            '<MenuSection Id=\"ISV.Dynamic.MenuSection\" Sequence=\"10\">' +
            '<Controls Id=\"ISV.Dynamic.Controls\">' +
            '<Button Id=\"ISV.Dynamic.AmountLetterGross.ServiceConnectedDisabilityCompensation\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"60\" LabelText=\"Amount Letter Gross - Service Connected Disability Compensation\" Alt=\"Amount Letter Gross\" />' +
            '<Button Id=\"ISV.Dynamic.AmountLetterGross.NonServiceConnectedVeteransPension\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"61\" LabelText=\"Amount Letter Gross - Non Service Connected Veterans Pension\" Alt=\"Amount Letter Gross\" />' +
            '<Button Id=\"ISV.Dynamic.AmountLetterGross.DeathPensionn\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"62\" LabelText=\"Amount Letter Gross - Death Pension\" Alt=\"Amount Letter Gross\" />' +
            '<Button Id=\"ISV.Dynamic.AmountLetterGross.DependencyIndemnityCompensation\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"63\" LabelText=\"Amount Letter Gross - Dependency Indemnity Compensation\" Alt=\"Amount Letter Gross\" />' +
            '<Button Id=\"ISV.Dynamic.BlankLetter\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"64\" LabelText=\"Blank Letter\" Alt=\"Blank Letter\" />' +
            '<Button Id=\"ISV.Dynamic.CommissaryLetter.CurrentRatingDegree\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"65\" LabelText=\"Commissary Letter - Current Rating Degree\" Alt=\"Commissary Letter\" />' +
            '<Button Id=\"ISV.Dynamic.CommissaryLetter.IndividualUnemployability\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"66\" LabelText=\"Commissary Letter - Individual Unemployability\" Alt=\"Commissary Letter\" />' +
            '<Button Id=\"ISV.Dynamic.CommissaryLetterSurvivingSpouse\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"67\" LabelText=\"Commissary Letter Surviving Spouse\" Alt=\"Commissary Letter Surviving Spouse\" />' +
            '<Button Id=\"ISV.Dynamic.CommissaryLetterWithFutureExam\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"68\" LabelText=\"Commissary Letter With Future Exam\" Alt=\"Commissary Letter With Future Exam\" />' +
            '<Button Id=\"ISV.Dynamic.DeathBenefitsLetter\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"69\" LabelText=\"Death Benefits Letter\" Alt=\"Death Benefits Letter\" />' +
            '<Button Id=\"ISV.Dynamic.DisabilityBreakdownLetter\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"70\" LabelText=\"Disability Breakdown Letter\" Alt=\"Disability Breakdown Letter\" />' +
            '<Button Id=\"ISV.Dynamic.IncomeAmountLetter.AmountOnly\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"71\" LabelText=\"IncomeAmountLetter - AmountOnly\" Alt=\"IncomeAmountLetter - Income Amount Letter\" />' +
            '<Button Id=\"ISV.Dynamic.IncomeAmountLetter.ApportioneeAmountOnly\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"72\" LabelText=\"Income Amount Letter - Apportionee Amount Only\" Alt=\"Income Amount Letter\" />' +
            '<Button Id=\"ISV.Dynamic.IncomeAmountLetter.Compensation\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"73\" LabelText=\"Income Amount Letter - Compensation\" Alt=\"Income Amount Letter\" />' +
            '<Button Id=\"ISV.Dynamic.IncomeAmountLetter.CompensationAndIu\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"74\" LabelText=\"Income Amount Letter - Compensation And IU\" Alt=\"Income Amount Letter\" />' +
            '<Button Id=\"ISV.Dynamic.IncomeAmountLetter.CompensationAndPercentageDisability\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"75\" LabelText=\"Income Amount Letter - Compensation And Percentage Disability\" Alt=\"Income Amount Letter\" />' +
            '<Button Id=\"ISV.Dynamic.IncomeAmountLetter.VeteranNonServiceConnectedPension\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"76\" LabelText=\"Income Amount Letter - Veteran Non Service Connected Pension\" Alt=\"Income Amount Letter\" />' +
            '<Button Id=\"ISV.Dynamic.IncomeAmountLetter.WidowDeathPension\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"77\" LabelText=\"Income Amount Letter - Widow Death Pension\" Alt=\"Income Amount Letter\" />' +
            '<Button Id=\"ISV.Dynamic.IncomeAmountLetter.WidowDependencyAndIndemnityCompensation\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"78\" LabelText=\"Income Amount Letter - Widow Dependency And Indemnity Compensation\" Alt=\"Income Amount Letter\" />' +
            '<Button Id=\"ISV.Dynamic.MultiplePaymentLetter\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"79\" LabelText=\"Multiple Payment Letter\" Alt=\"Multiple Payment Letter\" />' +
            '<Button Id=\"ISV.Dynamic.NoBenefitsLetter\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"80\" LabelText=\"No Benefits Letter\" Alt=\"No Benefits Letter\" />' +
            '<Button Id=\"ISV.Dynamic.NOKLetter\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"81\" LabelText=\"NOK Letter\" Alt=\"NOK Letter\" />' +
            '<Button Id=\"ISV.Dynamic.PreferenceLetters.NoSCD\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"82\" LabelText=\"Preference Letters - No SCD\" Alt=\"Preference Letters\" />' +
            '<Button Id=\"ISV.Dynamic.PreferenceLetters.10To20Percent\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"83\" LabelText=\"Preference Letters - 10 to 20%\" Alt=\"Preference Letters\" />' +
            '<Button Id=\"ISV.Dynamic.PreferenceLetters.GreaterThan30Percent\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"84\" LabelText=\"Preference Letters - > 30%\" Alt=\"Preference Letters\" />' +
            '<Button Id=\"ISV.Dynamic.PreferenceLetters.MotherVet100Percent\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"85\" LabelText=\"Preference Letters - Mother Vet 100%\" Alt=\"Preference Letters\" />' +
            '<Button Id=\"ISV.Dynamic.PreferenceLetters.MotherOfDeceasedVetFromServiceConnDisability\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"86\" LabelText=\"Preference Letters - Mother of Deceased Vet From Service Conn. Disability\" Alt=\"Preference Letters\" />' +
            '<Button Id=\"ISV.Dynamic.PreferenceLetters.NonServiceConnPension\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"87\" LabelText=\"Preference Letters - Non Service Conn. Pension\" Alt=\"Preference Letters\" />' +
            '<Button Id=\"ISV.Dynamic.PreferenceLetters.PurpleHeart\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"88\" LabelText=\"Preference Letters - Purple Heart\" Alt=\"Preference Letters\" />' +
            '<Button Id=\"ISV.Dynamic.PreferenceLetters.SpouseOf100PercentServiceConnVet\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"89\" LabelText=\"Preference Letters - Spouse of 100% Service Conn. Vet\" Alt=\"Preference Letters\" />' +
            '<Button Id=\"ISV.Dynamic.PreferenceLetters.SpouseOfDisabledDeceasedVetFromServConnDisability\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"90\" LabelText=\"Preference Letters - Spouse of Disabled Deceased Vet From Serv. Conn. Disability\" Alt=\"Preference Letters\" />' +
            '<Button Id=\"ISV.Dynamic.PreferenceLetters.0PercentInReceipt\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"91\" LabelText=\"Preference Letters - 0% in Receipt\" Alt=\"Preference Letters\" />' +
            '<Button Id=\"ISV.Dynamic.PropertyTaxDmvRegistrationMt\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"92\" LabelText=\"Property TAX DMV Registration - MT\" Alt=\"Property TAX DMV Registration - MT\" />' +
            '<Button Id=\"ISV.Dynamic.PropertyTaxExemptionCaLa\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"93\" LabelText=\"Property TAX Exemption - CA - LA\" Alt=\"Property TAX Exemption - CA - LA\" />' +
            '<Button Id=\"ISV.Dynamic.RealEstateLicenseFeeTaxExemption\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"94\" LabelText=\"Real Estate - License Fee Tax Exemption\" Alt=\"Real Estate - License Fee Tax Exemption\" />' +
            '<Button Id=\"ISV.Dynamic.ServiceVerification\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"95\" LabelText=\"Service Verification\" Alt=\"Service Verification\" />' +
            '<Button Id=\"ISV.Dynamic.SummaryOfBenefitsSurvivingSpouseLetter\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"96\" LabelText=\"Summary Of Benefits Surviving Spouse Letter\" Alt=\"Summary Of Benefits Surviving Spouse Letter\" />' +
            '<Button Id=\"ISV.Dynamic.SummaryOfBenefitsVeteransLetter\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"97\" LabelText=\"Summary of Benefits Veterans Letter\" Alt=\"Summary of Benefits Veterans Letter\" />' +
            '<Button Id=\"ISV.Dynamic.VeteranPercentageLetter\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"98\" LabelText=\"Veteran Percentage Letter\" Alt=\"Veteran Percentage Letter\" />' +
            '<Button Id=\"ISV.Dynamic.VeteranPTLetter\" Command=\"ISV.Vbms.SearchCommand\" Sequence=\"99\" LabelText=\"Veteran PT Letter\" Alt=\"Veteran PT Letter\" />' +
            '</Controls>' +
            '</MenuSection></Menu>';

        CommandProperties.PopulationXML = menuXmlAction;

    },

    Search: function (CommandProperties) {
        var controlId = CommandProperties.SourceControlId;
        var GUIDvalue = Xrm.Page.data.entity.getId();
        var serverUrl = Xrm.Page.context.getServerUrl();
        var lettersCreated = Xrm.Page.getAttribute("va_letterscreated");
        var pcr = Getinfo();
        var now = new Date();
        var today = (now.getMonth() + 1) + '/' + now.getDate() + '/' + now.getFullYear() + ' ' + now.getHours() + ':' + ((now.getMinutes() < 10 ? '0' : '') + now.getMinutes());
        VbmsFileUpload.CreateNote(controlId)
        /* switch (controlId) {
         case 'ISV.Dynamic.AmountLetterGross':
         window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Amount Letter Gross') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
         if (lettersCreated.getValue() == null) {
         lettersCreated.setValue('Amount Letter Gross was created by ' + pcr + ' on ' + today);
         }
         else {
         lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Letter Amount Letter Gross was created by ' + pcr + ' on ' + today);
         }
         break;
         case 'ISV.Dynamic.BlankLetter':
         window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Blank Letter') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
         if (lettersCreated.getValue() == null) {
         lettersCreated.setValue('Blank Letter was created by ' + pcr + ' on ' + today);
         }
         else {
         lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Blank Letter was created by ' + pcr + ' on ' + today);
         }
         break;

         default:
         alert('Button Unknown');
         }*/
    },

    StringMaker: function () {
        this.parts = [];
        this.length = 0;
        this.append = function (s) {
            this.parts.push(s);
            this.length += s.length;
        }

        this.prepend = function (s) {
            this.parts.unshift(s);
            this.length += s.length;
        }

        this.toString = function () {
            return this.parts.join('');
        }
    },

    Encode64: function (input) {
        var keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        var output = new VbmsFileUpload.StringMaker();
        var chr1, chr2, chr3;
        var enc1, enc2, enc3, enc4;
        var i = 0;

        while (i < input.length) {
            chr1 = input[i++];
            chr2 = input[i++];
            chr3 = input[i++];

            enc1 = chr1 >> 2;
            enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
            enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
            enc4 = chr3 & 63;

            if (isNaN(chr2)) {
                enc3 = enc4 = 64;
            } else if (isNaN(chr3)) {
                enc4 = 64;
            }

            output.append(keyStr.charAt(enc1) + keyStr.charAt(enc2) + keyStr.charAt(enc3) + keyStr.charAt(enc4));
        }

        return output.toString();
    }
}
