// This method will be call from CRM form
_exCon = null;
_formContext = null;
_webApi = null;

function OnLoad(execCon) {
    //Perform any initialization as required.
    _exCon = execCon;
    _formContext = _exCon.getFormContext;
    var version = Xrm.Utility.getGlobalContext().getVersion();
    var lib = new CrmCommonJS(version, _exCon);
    _webApi = lib.webApi;
    onFormLoad();
}

reqStatus = null;
reqStatusVal = null;
_loading = true;
_sendMailButtonClicked = false;
_sendEmailsThroughCode = null;
_runningEmailGenAfterAutoSave = null;
_originalRO = null;
_isECC = false;

// Begin Form onload event - retrieve data from VIP if necessary
function onFormLoad() {
    _runningEmailGenAfterAutoSave = false;
    _sendEmailsThroughCode = true;

    serviceRequest();
    ServiceTypeChange();
}

function serviceRequest() {
    //Adding Functions to OnChange:
    Xrm.Page.getAttribute('udo_quickwriteid').addOnChange(QuickWriteChange);
    Xrm.Page.getAttribute('udo_action').addOnChange(ServiceTypeChange);
    Xrm.Page.getAttribute('udo_action').addOnChange(EmailtoVet);
    Xrm.Page.getAttribute('udo_relatedveteranid').addOnChange(RelatedVetChange);
    Xrm.Page.getAttribute('udo_soj').addOnChange(SOJChange);
    Xrm.Page.getAttribute('udo_requeststatus').addOnChange(StatusChange);
    Xrm.Page.getAttribute("udo_alltrackeditemsreceivedorclosed").addOnChange(udo_AllTrackedItemsReceivedOrClosedChange);
    Xrm.Page.getAttribute('udo_processedfnodinshare').addOnChange(ProcessedInShareChange);
    Xrm.Page.getAttribute('udo_other').addOnChange(FnodOtherChange);

    fnMPCheckAll();
    //Set Service Request Number (If empty)
    if (Xrm.Page.getAttribute('udo_reqnumber').getValue() == null) {
        getUniqueRequestNumber();
    }

    if (Xrm.Page.getAttribute('udo_servicerequesttype') != null) {
        if (Xrm.Page.getAttribute('udo_servicerequesttype').getValue() === 'ECC') {
            Xrm.Page.ui.tabs.get("mailing_address_tab").sections.get("ecc_information").setVisible(true);
        }
        else
            Xrm.Page.ui.tabs.get("mailing_address_tab").sections.get("ecc_information").setVisible(false);
    }

    if (Xrm.Page.getAttribute('udo_rpo').getValue() !== (953850004 || null)) {
        Xrm.Page.getAttribute('udo_rpotext').setValue((Xrm.Page.getAttribute('udo_rpo').getText()))
    }

    /** End event setting **/

    reqStatus = Xrm.Page.getAttribute("udo_requeststatus").getSelectedOption().text;
    reqStatusVal = Xrm.Page.getAttribute("udo_requeststatus").getValue();

    if (reqStatus == 'Sent') { Xrm.Page.getControl("udo_requeststatus").setDisabled(true); }
    var webResourceUrl = Xrm.Page.context.getClientUrl() + '/WebResources/va_';

    //Events to happen when Service Request is New or Existing
    if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE || Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_UPDATE) {

        //Show Script button
        window.GeneralToolbar = new InlineToolbar("udo_script");
        GeneralToolbar.AddButton("btnScript", "Show Script", "60%", ShowScript, webResourceUrl + 'status_online.png');

        //Send E-mail button
        window.GeneralToolbar = new InlineToolbar("udo_robutton");
        GeneralToolbar.AddButton("btnGetRO", (_sendEmailsThroughCode ? "Send E-mail" : "Set Service Request as Sent"), "60%", SendEmailToRO, webResourceUrl + 'email_go.png');

        //Enclosures button
        window.GeneralToolbar = new InlineToolbar("udo_enclosuresbutton");
        GeneralToolbar.AddButton("btnEnclo", "Populate Enclosures", "60%", PopulateEnclosures, webResourceUrl + 'arrow_refresh.png');

        //Multiple Payment button
        window.GeneralToolbar = new InlineToolbar("udo_mpbutton");
        GeneralToolbar.AddButton("btnMP", "Filter Payments", "60%", fnMP, webResourceUrl + 'BankandroutingNoLookupIcon_App');

        //Copy veteran's address
        window.GeneralToolbar = new InlineToolbar("udo_copymailingaddress");
        GeneralToolbar.AddButton("btnCopy", "Copy Mailing Address", "60%", copyMailingAddress, '');

        //If SOJ field is blank, prompt the user
        if (Xrm.Page.getAttribute('udo_action').getValue() != null) {
            var action = Xrm.Page.getAttribute('udo_action').getSelectedOption().text;
            if (action != 'Email Forms') {
                if (!Xrm.Page.getAttribute('udo_regionalofficeid').getValue()) {
                    if (Xrm.Page.getAttribute('udo_eccssn').getValue() != null) {
                        alert('Warning: RPO is empty.\n\nTo select RPO manually, click on the Lookup icon next to Send To box on the top of the screen.');
                    }
                    else {
                        alert('Warning: SOJ is empty.\n\nTo select SOJ manually, click on the Lookup icon next to Send To box on the top of the screen.');
                    }
                }
            }
        }
    } else { Xrm.Page.getControl("udo_robutton").setVisible(false); }

    Xrm.Page.getControl('udo_deathrelatedinformationchecklists').setLabel('Answered questions concerning possible benefit entitlements referring to "Death Related Information Checklist" work aid');
    Xrm.Page.getControl('udo_benefitsstopfirstofmonth').setLabel('Advised the caller the benefits will be stopped the first of the month of death and that any payment issued following that date must be returned (if applicable)');
    Xrm.Page.getControl('udo_willroutereportofdeath').setLabel('Will route this report of death to Regional Office of Jurisdiction or PMC via encrypted email for stop payment processing');

    StatusChange();
    GetSystemSettings();

    _originalRO = UTIL.GetLookupId('udo_regionalofficeid');
    Xrm.Page.ui.tabs.get("tab_WebService").setVisible(false);
    if (Xrm.Page.getAttribute('udo_sendnotestomapd').getValue() == true) {
        disableFormFields(true);
    }

    //Collapse the unused tabs on Form load
    Xrm.Page.ui.tabs.get("tab_ai").setDisplayState('collapsed');
    Xrm.Page.ui.tabs.get("tab_dependent_information").setDisplayState('collapsed');

    _loading = false;
}
/*******************************END ONLOAD EVENT*****************************************/

function GetSystemSettings() {
    var columns = ['va_Description', 'va_name'];
    var filter = "startswith(va_name,'UDO_SR_')";

    CrmRestKit2011.ByQuery('va_systemsettings', columns, filter, false)
         .done(function (data) {
             if (data && data.d.results.length > 0) {
                 for (var i = 0; i < data.d.results.length; i++) {

                     var name = data.d.results[i].va_name;//va_Description;
                     switch (name) {
                         case 'UDO_SR_RepWS':
                             serviceUri = data.d.results[i].va_Description;
                             break;
                         case 'UDO_SR_EmailMessage':
                             emailMessage = data.d.results[i].va_Description;
                             break;
                         case 'UDO_SR_Email0820Message':
                             email0820Message = data.d.results[i].va_Description;
                             break;
                     }
                 }
             }
         })
        .fail(function (err) {
            UTIL.restKitError(err, 'Failed to retrieve station data');
        });
}

//Set the send email to veteran field when the action is set to Email Forms
function EmailtoVet() {
    if (Xrm.Page.getAttribute('udo_action').getValue() != null) {
        if (Xrm.Page.getAttribute('udo_action').getValue() == 953850004) {
            Xrm.Page.getAttribute('udo_sendemailtoveteran').setValue(true);
        }
        else {
            Xrm.Page.getAttribute('udo_sendemailtoveteran').setValue(false);
        }
    }
}

function StatusChange() {
    // if sent or cancelled, cannot change record
    var status = Xrm.Page.getAttribute("udo_requeststatus").getSelectedOption().text;

    var _isStatusSentOrresolved = (status == 'Sent' || status == 'Queued to Send' || status == 'Resolved' || status == 'Cancelled');

    if (_isStatusSentOrresolved) {
        disableFormFields(true);
    }

    // only admins can change status
    // If not 0820, PCRs can change it to Resolved, if current status allows
    var action = (Xrm.Page.getAttribute("udo_action").getSelectedOption() == null ? '' : Xrm.Page.getAttribute("udo_action").getSelectedOption().text);
    var vai0820 = (action == '0820' || action == '0820a' || action == '0820f' || action == '0820d');
    if (!_loading) {
        //TODO: Change to use udo_CRMCommonJS
        var canChangeStatus = (UserHasRole("System Administrator") || UserHasRole("Supervisor") || (!vai0820 && reqStatus != 'Sent' && reqStatus != 'Queued to Send'));
        if (!canChangeStatus) {
            alert("Combination of your security role and current service request status does not allow you to modify status; use 'Send Email' button to send Service Request and attached reports via email.");
            Xrm.Page.getAttribute("udo_requeststatus").setValue(reqStatusVal);
            return;
        }
    }

    if (!_loading && (status == "Queued to Send" || status == "Sent")) {
        alert("'" + status + "' cannot be set manually; use 'Send Email' button instead and/or workflow processing rules.");
        Xrm.Page.getAttribute("udo_requeststatus").setValue(reqStatusVal);
        ro = (reqStatus == 'Sent' || reqStatus == 'Resolved' || reqStatus == 'Cancelled');
        disableFormFields(true);
    }
}

function doesControlHaveAttribute(control) {
    var controlType = control.getControlType();
    return controlType != "iframe" && controlType != "webresource" && controlType != "subgrid" && control.setDisabled != undefined;
}

function disableFormFields(onOff) {
    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (doesControlHaveAttribute(control)) {
            control.setDisabled(onOff);
        }
    });
}

//Multiple Payment functions
//MP Main function
function fnMP() {
    fnMPCheckAll();
    fnMPdate();
    if (Xrm.Page.getAttribute("udo_mpend").getValue() < Xrm.Page.getAttribute("udo_mpstart").getValue()) {
        //clear out the display
        varDisplay = 'Invalid date range. Please verify that the start date is before the end date.';
        Xrm.Page.getAttribute("udo_mpdisplay").setValue(varDisplay);
    }
    else {
        fnMPsort();
    }
}

//MP checkbox function
function fnMPCheckAll() {
    if (Xrm.Page.getAttribute("udo_mpall").getValue() == '1') {
        Xrm.Page.getControl("udo_mpstart").setVisible(false);
        Xrm.Page.getControl("udo_mpend").setVisible(false);
        fnMPsort();
    }
    else {
        Xrm.Page.getControl("udo_mpstart").setVisible(true);
        Xrm.Page.getControl("udo_mpend").setVisible(true);
    }
}
//MP default dates if nothing is entered(6 months ago - today)
function fnMPdate() {
    varToday = new Date();
    //If no start, yes end
    if ((Xrm.Page.getAttribute("udo_mpstart").getValue() == null) && (Xrm.Page.getAttribute("udo_mpend").getValue() !== null)) {
        varAdd = new Date(Xrm.Page.getAttribute("udo_mpend").getValue());
        varAdd.setDate(1);
        varAdd.setMonth(varAdd.getMonth() - 6);
        Xrm.Page.getAttribute("udo_mpstart").setValue(varAdd);
    }
    //If no start, no end (catch)
    if (Xrm.Page.getAttribute("udo_mpstart").getValue() == null) {
        varBegin = new Date();
        varBegin.setDate(1);
        varBegin.setMonth(varToday.getMonth() - 6);
        Xrm.Page.getAttribute("udo_mpstart").setValue(varBegin);
    }
    //If no end
    if (Xrm.Page.getAttribute("udo_mpend").getValue() == null) {
        Xrm.Page.getAttribute("udo_mpend").setValue(varToday);
    }
    //Validate start is before end
    if (Xrm.Page.getAttribute("udo_mpend").getValue() < Xrm.Page.getAttribute("udo_mpstart").getValue()) {
        varValid = new Date(Xrm.Page.getAttribute("udo_mpend").getValue());
        alert('Invalid date range. Please verify that the start date is before the end date.');
    }
}
//MP need to sort through the data now.
function fnMPsort() {
    varBegin = new Date(Xrm.Page.getAttribute("udo_mpstart").getValue());
    varBegin.setHours(0);
    varEnd = new Date(Xrm.Page.getAttribute("udo_mpend").getValue()).setHours(0);
    //varEnd.setHours(0);
    varDisplay = "";

    if (Xrm.Page.getAttribute("udo_mpraw").getValue() !== null) {
        //var SampleString = "1/1/2011_$101.00,2/1/2011_$201.00,3/1/2011_$301.00,3/23/2011_$323.00,4/1/2011_$401.00,5/1/2011_$501.00";
        var SampleString = new String(Xrm.Page.getAttribute("udo_mpraw").getValue());
        if (SampleString.substring(0, 9) == "undefined") {
            SampleString = SampleString.substring(9, SampleString.length);
        }

        var SampleString2 = SampleString.substring(0, SampleString.length - 1);
        //set array to the data to be assigned
        var checkArray = SampleString2.split("#");
        //split each selected check into [payment date, amount] array
        var checkArray2d = new Array();
        for (var i = 0; i < checkArray.length; i++) {
            checkArray2d[i] = checkArray[i].split("_");
        }

        //Filter if "all" check is not true
        if (Xrm.Page.getAttribute("udo_mpall").getValue() == '0') {
            for (var i = 0; i < checkArray2d.length; i++) {
                varCurrPayDate = new Date(checkArray2d[i][0]).setHours(0);

                if ((varCurrPayDate >= varBegin) && (varCurrPayDate <= varEnd)) {
                    var varFormatDate = mpFormatDate(varCurrPayDate);
                    varDisplay += varFormatDate + "\t\t $" + parseFloat(checkArray2d[i][1]).toFixed(2) + "\n";
                }
            }
        }
        else {
            for (var i = 0; i < checkArray2d.length; i++) {

                var varFormatDate = mpFormatDate(checkArray2d[i][0]);
                varDisplay += varFormatDate + "\t\t $" + parseFloat(checkArray2d[i][1]).toFixed(2) + "\n";
            }
        }
    }
    else {
        varDisplay += "No Payment History on the Service Request record to filter.";
    }

    Xrm.Page.getAttribute("udo_mpdisplay").setValue(varDisplay);
}
function mpFormatDate(varDate) {
    var varInput = new Date(varDate);
    var varFormat = "";
    var varM = varInput.getMonth() + 1;
    if (varM < 10) {
        varFormat = "0" + varM + "/";
    }
    else {
        varFormat = "" + varM + "/";
    }
    var varD = varInput.getDate();
    if (varD < 10) {
        varFormat += "0" + varD + "/";
    }
    else {
        varFormat += varD + "/";
    }
    var varY = varInput.getFullYear();
    varFormat += varY;
    return varFormat;
}

function PopulateEnclosures() {
    enclosures = '';
    var columns = ['va_externaldocumentid'];
    var filter = "(udo_servicerequestid eq guid'" + Xrm.Page.data.entity.getId() + "')";
    CrmRestKit2011.ByQuery('udo_udo_servicerequest_va_externaldocument', columns, filter, false)
    .fail(function (err) {
        UTIL.restKitError(err, 'Failed to retrieve service request external document data');
    })
    .done(function (data) {
        if (data && data.d.results.length > 0) {
            for (var i = 0; i < data.d.results.length; i++) {
                var r = data.d.results[i].va_externaldocumentid;

                columns = ['va_name', 'va_DocumentLocation'];
                filter = "(va_externaldocumentId eq guid'" + r + "')";

                CrmRestKit2011.ByQuery('va_externaldocument', columns, filter, false)
                    .fail(function (err) {
                        UTIL.restKitError(err, 'Failed to retrieve external document data');
                    })
                    .done(function (data) {
                        if (data && data.d.results.length > 0) {
                            enclosures += (enclosures.length > 0 ? '\n' : '') + data.d.results[0].va_name;
                        }
                    });
            }
        }
    });

    if (Xrm.Page.getAttribute('udo_enclosures').getValue() != enclosures) {
        Xrm.Page.getAttribute('udo_enclosures').setValue(enclosures);
        // update data to make sure enclosures get on report
        if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_UPDATE) {
            CrmRestKit2011.Update('udo_servicerequest', Xrm.Page.data.entity.getId(), { udo_Enclosures: 'enclosures' })
                .fail(function (err) {
                    UTIL.restKitError(err, 'Failed to update servicerequest enclosures');
                })
                .done(function (data, status, xhr) {
                });
        }
    }
}


function SendEmailToRO() {
    //Check to make sure form is saved before continuing.
    if (Xrm.Page.ui.getFormType() == 1) {
        alert('You have to save the form before proceeding.');
        return;
    }


    if (Xrm.Page.getAttribute("udo_requeststatus").getSelectedOption().value == 953850006) { //Sent
        if (!confirm('Current Request Status is "Sent". Please confirm that you would like to send an email again.')) {
            return;
        }
    }
    var roId = UTIL.GetLookupId('udo_regionalofficeid');
    var action = Xrm.Page.getAttribute("udo_action").getSelectedOption().text;
    var is0820 = (action == '0820' || action == '0820a' || action == '0820f' || action == '0820d' || action == 'Email Forms');

    Xrm.Page.getControl("udo_update").setDisabled(false);
    Xrm.Page.getAttribute("udo_update").setSubmitMode('always');
    if (is0820) {
        var script0 = emailMessage;
        if (action == '0820d') script0 += email0820Message;

        // send mail could be running after automatic save. In this case, script was already prompted
        if (!_runningEmailGenAfterAutoSave) {
            if (!confirm(script0)) return;
        }

        var rs = Xrm.Page.getAttribute('udo_readscript').getValue();
        if (rs == null || rs == false) Xrm.Page.getAttribute('udo_readscript').setValue(true);
    }

    if (_sendEmailsThroughCode) {
        _sendMailButtonClicked = CreateOutlookEmail2();
        if (!_sendMailButtonClicked) return;
    }

    var close = true;
    close = confirm('Would you like to mark this record as Sent?');
    if (close) {
        _sendMailButtonClicked = true;
        Xrm.Page.getAttribute("udo_requeststatus").setValue(953850006); //Sent
        Xrm.Page.data.entity.save("save");
    }
}


function CreateOutlookEmail2() {
    var reportName = ''; // "27-0820 - Report of General Information"; //set this to the report you are trying to download
    var action = null;
    var dispSubType = Xrm.Page.getAttribute("udo_disposition").getText();
    if (dispSubType == undefined) dispSubType = '';
    var arrFiles = null;

    action = Xrm.Page.getAttribute("udo_action").getSelectedOption().text;
    switch (action) {
        case "0820":
            reportName = "27-0820 - Report of General Information";
            break;
        case "0820a":
            reportName = "27-0820a - Report of First Notice of Death";
            break;
        case "0820d":
            reportName = "27-0820d - Report of Non-Receipt of Payment";
            break;
        case "0820f":
            reportName = "27-0820f - Report of Month of Death";
            break;
        default:
            break;
    }

    var doCreatePDF = true;

    if (!reportName || reportName.length == 0) {
        doCreatePDF = false;
    }
    else {
        reportName = reportName + " - UDO";
    }

    var isDirty = Xrm.Page.data.entity.getIsDirty();

    // get forms attached to email
    var attachmentList = '',
        attachmentListx = '',
		enclosures = '';
    var attachUrlList = new Array();
    // get attachments

    var columns = ['va_externaldocumentid'];
    var filter = "(udo_servicerequestid eq guid'" + Xrm.Page.data.entity.getId() + "')";
    CrmRestKit2011.ByQuery('udo_udo_servicerequest_va_externaldocument', columns, filter, false)
        .fail(function (err) {
            UTIL.restKitError(err, 'Failed to retrieve service request external document data');
        })
        .done(function (data) {
            if (data && data.d.results.length > 0) {
                for (var i = 0; i < data.d.results.length; i++) {
                    var r = data.d.results[i].va_externaldocumentid;

                    columns = ['va_name', 'va_DocumentLocation'];
                    filter = "(va_externaldocumentId eq guid'" + r + "')";

                    CrmRestKit2011.ByQuery('va_externaldocument', columns, filter, false)
                        .fail(function (err) {
                            UTIL.restKitError(err, 'Failed to retrieve external document data');
                        })
                        .done(function (data) {
                            if (data && data.d.results.length > 0) {
                                if (data.d.results[0].va_name != 'Home Loans' && data.d.results[0].va_name != 'Education'
                                    && data.d.results[0].va_name != 'VR&E' && data.d.results[0].va_name != 'Life Insurance' && data.d.results[0].va_name != 'Pension') {
                                    attachmentList += ("<a href='" + data.d.results[0].va_DocumentLocation + "'>" + data.d.results[0].va_name + "</a><br/>");
                                    attachUrlList.push(data.d.results[0].va_DocumentLocation);
                                }
                                else {
                                    attachmentListx += ("<a href='" + data.d.results[0].va_DocumentLocation + "'>" + data.d.results[0].va_name + "</a><br/>");
                                }
                                enclosures += (enclosures.length > 0 ? '\n' : '') + data.d.results[0].va_name;
                            }
                        });
                }
            }
        });

    if (Xrm.Page.getAttribute('udo_enclosures').getValue() != enclosures) {
        Xrm.Page.getAttribute('udo_enclosures').setValue(enclosures);
        // update data to make sure enclosures get on report
        if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_UPDATE) {
            CrmRestKit2011.Update('udo_servicerequest', Xrm.Page.data.entity.getId(), { udo_Enclosures: 'enclosures' }, false)
                .fail(function (err) {
                    UTIL.restKitError(err, 'Failed to update servicerequest enclosures');
                })
				.done(function (data, status, xhr) {
				});
        }
    }

    if (doCreatePDF) {
        if (isDirty) {
            if (confirm('Some of the data on the screen has changed and was not saved to the database. Would you like to save the record before sending e-mail?\n\nIf you select OK, the screen will reload after save, and sending e-mail will resume.\n\nSelect Cancel to skip saving and go ahead with email generation.')) {
                var sem = new Array();
                sem.push(Xrm.Page.data.entity.getId().toString());
                window.parent.SEMSet = sem;
                Xrm.Page.data.entity.save("save");
                return false;
            }
        }

        // call ws to get the attachment file names from shared folder
        var names = '';
        var request = null;
        try {
            request = new ActiveXObject('Microsoft.XMLHTTP');
        } catch (err) { }

        if ((request == null) && window.XMLHttpRequest) {
            request = new XMLHttpRequest();
        } else if (request == null) {
            alert('Exception: Failed to create XML HTTP Object. Failed to execute web service request');
            return false;
        }

        var env = '<?xml version="1.0" encoding="utf-8"?><soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">' + '<soap12:Body><DownloadReport xmlns="http://tempuri.org/">' + '<serviceRequestId>' + Xrm.Page.data.entity.getId() + '</serviceRequestId>' + '<action>' + action.replace('&', '&amp;') + '-UDO</action>' + '<dispSubType>' + dispSubType.replace('&', '&amp;') + '</dispSubType>' + '</DownloadReport></soap12:Body></soap12:Envelope>';

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
            var resNode = rx.selectSingleNode('//DownloadReportResult');
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
    }

    var to = '';

    // get RO email
    var sendToRO = true,
		sendToVet = (Xrm.Page.getAttribute('udo_sendemailtoveteran').getValue()),
		sojAddress = '';
    var roId = UTIL.GetLookupId('udo_regionalofficeid');
    if (!roId) {
        roId = _originalRO;
        sendToRO = false;

        sojAddress = udo_crm_udo_getSoj();

    }

    if (roId) {
        var columns = ['EmailAddress', 'va_Address1', 'va_Address2', 'va_Address3', 'va_City', 'va_FaxNumber', 'va_State', 'va_ZipCode', 'va_name', 'va_Alias'];
        var filter = "(va_regionalofficeId eq guid'" + roId + "')";
        CrmRestKit2011.Retrieve('va_regionaloffice', roId, columns, false)
            .fail(function (err) {
                UTIL.restKitError(err, 'Failed to retrieve regional office data');
            })
            .done(function (data) {
                if (data) {
                    var r = data.d;
                    to = r.EmailAddress;
                    sojAddress = (r.va_Alias && r.va_Alias.length > 0 ? r.va_Alias + '<br/>' : '') + (r.va_Address1 && r.va_Address1.length > 0 ? r.va_Address1 + '<br/>' : '') + (r.va_Address2 && r.va_Address2.length > 0 ? r.va_Address2 + '<br/>' : '') + (r.va_Address3 && r.va_Address3.length > 0 ? r.va_Address3 + '<br/>' : '') + (r.va_City && r.va_City.length > 0 ? r.va_City + ', ' : '') + (r.va_State && r.va_State.length > 0 ? r.va_State + ' ' : '') + (r.va_ZipCode && r.va_ZipCode.length > 0 ? r.va_ZipCode : '') + (r.va_FaxNumber && r.va_FaxNumber.length > 0 ? '<br/>FAX: ' + r.va_FaxNumber : '');
                }
            });
    }

    var s = '<BR/>';
    var newReq = (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE ? "New " : "");
    var veteran = (Xrm.Page.getAttribute('udo_relatedveteranid').getValue() ? Xrm.Page.getAttribute('udo_relatedveteranid').getValue()[0].name : 'Unknown');

    var issue = Xrm.Page.getAttribute('udo_issue').getText();
    var subject = newReq + "Request for" /*+ "  " + veteran*/
	+ ": " + (!dispSubType || dispSubType.length == 0 ? issue : dispSubType);

    if (sendToVet) {
        if (action == 'Email Forms') {
            subject = 'Requested VA Forms';
        }
        else {
            subject = 'Requested Information from VA';
        }
    }

    var body = '';
    var nonEmergency = (action == 'Non Emergency Email');

    var PCRLookupValue = Xrm.Page.getAttribute('udo_pcrofrecordid').getValue();
    var PCRName = PCRLookupValue == null ? '' : PCRLookupValue[0].name;
    var PCRId = PCRLookupValue == null ? '' : PCRLookupValue[0].id;

    var StationNumber = null;

    if (PCRId != '') {
        CrmRestKit2011.Retrieve('SystemUser', PCRId, ['va_StationNumber'], false)
                 .done(function (data) {
                     StationNumber = data.d.va_StationNumber;
                 })
                .fail(function (err) {
                    UTIL.restKitError(err, 'Failed to retrieve station data');
                });
    }


    var PCRNameArray = PCRName.split(',');
    var PCRNameFullName = '';
    if (PCRNameArray.length > 0) {
        PCRNameFullName = (PCRNameArray[1] == undefined ? '' : PCRNameArray[1]) + ' ' + (PCRNameArray[0] == undefined ? '' : PCRNameArray[0]);
    }
    var Description = Xrm.Page.getAttribute('udo_description').getValue();
    if (Description == null) {  //ensure no 'null' in email body
        Description = '';
    }

    // For non-emergency email to Vet, using special body
    if ((!nonEmergency || (nonEmergency && sendToVet == false)) && action != 'Email Forms') {
        if (action == '0820' || action == '0820a' || action == '0820f') {
            body = PCRNameFullName.trim() + s + 'Station Number ' + StationNumber;
        }
        else {
            if (nonEmergency && sendToVet == false) {
                var cfirstname = Xrm.Page.getAttribute("udo_firstname").getValue();
                cfirstname = cfirstname == null ? "" : cfirstname;
                var clastname = Xrm.Page.getAttribute("udo_lastname").getValue();
                clastname = clastname == null ? "" : clastname;
                var mfirstname = Xrm.Page.getAttribute("udo_srfirstname").getValue();
                mfirstname = mfirstname == null ? "" : mfirstname;
                var mlastname = Xrm.Page.getAttribute("udo_srlastname").getValue();
                mlastname = mlastname == null ? "" : mlastname;
                var filenumber = Xrm.Page.getAttribute("udo_filenumber").getValue();
                filenumber = filenumber == null ? "" : filenumber;
                var ssn = Xrm.Page.getAttribute("udo_ssn").getValue();
                ssn = ssn == null ? "" : ssn;
                var phone = Xrm.Page.getAttribute("udo_dayphone").getValue();
                phone = phone == null ? "N/A" : phone;

                body = "A " + newReq + "service request has been submitted for the Veteran " + veteran + s + s + (to && to.length > 0 ? "RO Email Address: " + to + s : "") + "Service Request Number: " + Xrm.Page.getAttribute("udo_reqnumber").getValue() + s + "Service Request Action: " + action + s + "Service Request Type: " + issue + s;
                body += "Description: " + s;
                body += "Callers Name: " + cfirstname + " " + clastname + s;
                body += "Veteran/Beneficiary's Name (if different): " + mfirstname + " " + mlastname + s;
                body += "File or Social Security Number: " + (filenumber == "" ? ssn : filenumber) + s;
                body += "Phone Number(s): " + phone + s;
                body += "Best time to reach caller: anytime" + s;
                body += "Brief message: " + s + Description.replace(/\n/g, s) + s + s;
                body += "Attachments: " + (arrFiles ? arrFiles.length : attachUrlList.length);
            }
            else {
                body = Description.replace(/\n/g, s);
            }
        }
    }
    else {
        if (attachmentList.length > 0) {
            attachmentList = "Per your request, we have attached the following forms for your use: <br/>" + attachmentList + "<br/><br/>";
        }
        else {
            // if no attachments, use Description
            attachmentList = Description.replace(/\n/g, s) + s + s;
        }
        body = attachmentList + "Please do not respond to this message.  Unfortunately, replies to this message will be routed to a mailbox which is unable to receive replies.<br/><br/>" + "Please be aware that you can change your direct deposit, create a benefits letter, check the status of your claim, obtain a copy of your DD 214, and access additional VA benefit information via eBenefits at www.ebenefits.va.gov. To register for an account, you can speak with an eBenefits specialist by dialing 1-800-827-1000 (please press #7) or visit your local VA regional office.<br/><br/>" + "We are happy to help you with any questions or concerns you may have.  If you have questions or need additional assistance, please see our contact information listed below.<br/><br/>" + "How to Contact VA:<br/><br/>" + "Online: <br/>" + "<a href='http://www.va.gov'>www.va.gov</a> or <a href='http://www.ebenefits.va.gov'>www.ebenefits.va.gov</a><br/><br/>" + "By Phone:<br/>" + "1-800-827-1000<br/>711 (Federal Relay Service for Hearing Impaired)<br/><br/>" + "By Mail: <br/>";

        if (attachmentListx.length > 0) {
            body += attachmentListx;
        }
        else {
            body += sojAddress;
        }
    }

    var toLine = '', ccLine = '';

    if (sendToVet && action == 'Email Forms') {
        toLine = Xrm.Page.getAttribute('udo_emailofveteran').getValue();
    } else if (sendToVet && sendToRO) {
        toLine = to;
        ccLine = Xrm.Page.getAttribute('udo_emailofveteran').getValue();
    } else if (sendToVet && !sendToRO) {
        toLine = Xrm.Page.getAttribute('udo_emailofveteran').getValue();
    } else if (!sendToVet && sendToRO) {
        toLine = to;
    }

    var emailOptions = {
        subject: subject,
        to: toLine,
        cc: ccLine,
        isHtml: true,
        body: body,
        files: arrFiles || [], // RU12 fixed bug
        attachements: attachUrlList
    };

    UTIL.openOutlookEmail(emailOptions);
}

//To Do: Change to getValue() and use number instead of TEXT.
// change visibility of sections on the form based on type
function ServiceTypeChange() {
    if (Xrm.Page.getAttribute('udo_action').getValue() != null) {
        var action = Xrm.Page.getAttribute('udo_action').getSelectedOption().text;
        if (!action) return;
        Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("0820a_StopPayment").setVisible(false);
        switch (action) {
            case 'Email Forms':
                Xrm.Page.ui.tabs.get("Tab_SRinfo").setVisible(false);
                Xrm.Page.ui.tabs.get("tab_contact").setVisible(false);
                Xrm.Page.ui.tabs.get("tab_dependent_information").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_Notes").setVisible(false);
                Xrm.Page.ui.tabs.get("tab_ai").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_General").sections.get("Tab_General_Vet").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_General").sections.get("Tab_General_section_4").setVisible(false);
                Xrm.Page.getAttribute('udo_sendemailtoveteran').setValue(true);
                break;
            case '0820':
                Xrm.Page.ui.tabs.get("Tab_SRinfo").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820dPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a2").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("0820a_StopPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_VAI").setVisible(false);
                Xrm.Page.getControl("udo_nameofreportingindividual").setVisible(true);
                Xrm.Page.getControl("udo_fnodreportingfor").setVisible(false);
                Xrm.Page.getControl("udo_readscript").setVisible(true);
                break;
            case '0820f':
                Xrm.Page.ui.tabs.get("Tab_SRinfo").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820dPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a2").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("0820a_StopPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_VAI").setVisible(false);
                break;
            case '0820d':
                Xrm.Page.ui.tabs.get("Tab_SRinfo").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820dPayment").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_VAI").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a2").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("0820a_StopPayment").setVisible(false);
                break;
            case '0820a':
                Xrm.Page.ui.tabs.get("Tab_SRinfo").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820dPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_VAI").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("0820a_StopPayment").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a2").setVisible(true);
                ProcessedInShareChange();
                FnodOtherChange();
                break;
            default:
                Xrm.Page.ui.tabs.get("Tab_SRinfo").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820dPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a2").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("0820a_StopPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_VAI").setVisible(false);
                break;
        }

        Xrm.Page.getControl("udo_dateoffirstnotice").setVisible(false);

        var afControls = [Xrm.Page.getControl("udo_placeofdeath"),
            Xrm.Page.getControl("udo_enroutetova"),
            Xrm.Page.getControl("udo_dateofdeath")];
        var vis = (action == '0820a' || action == '0820f');
        for (var c in afControls) {
            afControls[c].setVisible(vis);
        }
    }
}

function RelatedVetChange() {
    // find ssn/part. id of related veteran
    var id = null;
    var attr = Xrm.Page.getAttribute("udo_relatedveteranid");
    if (attr && attr.getValue()[0] && attr.getValue()[0].id.length > 0) id = attr.getValue()[0].id;
    if (!id) return;

    if (RestrictPCRAccess(id, function (restrictAccess) {
        if (restrictAccess)
            return;

        ShowProgress('Getting Contact Data & new Request No');

        CrmRestKit2011.ByQuery('Contact', ['udo_SSN', 'udo_ParticipantId', 'CreatedOn'], "ContactId eq guid'" + id + "' ")
        .done(function (priorSRs) {
            try {
        	    if (priorSRs && priorSRs.d.results && priorSRs.d.results.length > 0) {
        		    Xrm.Page.getAttribute("udo_ssn").setValue(priorSRs.d.results[0].udo_SSN);
        		    Xrm.Page.getAttribute("udo_participantid").setValue(priorSRs.d.results[0].udo_ParticipantId);

        // count number of prior SRs from this person
        		    var personId = Xrm.Page.getAttribute("udo_ssn").getValue();
        		    var searchCol = 'udo_SSN';
        		    if (!personId) {
        			    personId = Xrm.Page.getAttribute("udo_participantid").getValue();
        			    searchCol = 'udo_ParticipantID';
    }
        		    if (personId) {
        		        CrmRestKit2011.ByQuery('udo_servicerequest', ['CreatedOn'], searchCol + " eq '" + personId + "' ")
        		        .done(function (priorSRs) {
                            var priorCount = 0;
                            if (priorSRs && priorSRs.d && priorSRs.d.results)
                                priorCount = priorSRs.d.results.length;

                            priorCount++;

                            Xrm.Page.getAttribute("udo_reqnumber").setValue(personId + ": " + priorCount.toString());
    }).fail(function (err) {
        		            UTIL.restKitError(err, 'Failed to retrieve the prior SR count');
    });

    }
    }
    } catch (e) {
        	    alert("Error occurred.\n" + e.description);
    } finally {
        	    CloseProgress();
    }
    }).fail(function (err) {
            CloseProgress();
            UTIL.restKitError(err, 'Failed to retrieve the contact data and SR number');
    });
    }));
}

function QuickWriteChange() {
    var id = UTIL.GetLookupId('udo_quickwriteid');
    if (!id) return;
    var descAttribute = Xrm.Page.getAttribute('udo_description');
    //if (descAttribute.getValue() && !confirm('Would you like to overwrite Description field with the selected Quick Write?')) return;

    var columns = ['va_QuickWriteText'];
    CrmRestKit2011.Retrieve('va_quickwrite', id, columns)
        .fail(function (err) {
            UTIL.restKitError(err, 'Failed to retrieve text.');
        })
         .done(function (data) {
             if (data) {
                 var qw = data.d.va_QuickWriteText;
                 var val = descAttribute.getValue();

                 // process substition tokens
                 // each one looks like <!udo_ssn!>
                 qw = UTIL.ReplaceFieldTokens(qw);
                 if (!qw) qw = '';

                 descAttribute.setValue((val ? val + ' \n' + qw : qw));
                 Xrm.Page.getAttribute('udo_quickwriteid').setValue(null);
                 Xrm.Page.getControl('udo_description').setFocus();

                 getManager();
             }
         });
}

function SOJChange() {
    // auto-select RO with SOJ matching selected value
    var SOJ = Xrm.Page.getAttribute("udo_soj").getValue();
    if (!SOJ) return;

    var columns = ['va_name', 'va_regionalofficeId'];
    var filter = "va_SpecialIssueJurisdiction/Value eq " + SOJ + " ";
    CrmRestKit2011.ByQuery('va_regionaloffice', columns, filter)
        .fail(function (err) {
            UTIL.restKitError(err, 'Failed to retrieve data');
        })
        .done(function (data) {
            if (data && data.d.results.length > 0) {
                var id = null;
                var attr = Xrm.Page.getAttribute("va_regionalofficeid");
                if (attr && attr.DataValue && attr.DataValue.length > 0) id = '{' + attr.DataValue[0].id.toString() + '}';
                if (id == data.d.results[0].va_regionalofficeId) return;

                Xrm.Page.getAttribute('udo_regionalofficeid').setValue([{
                    id: data.d.results[0].va_regionalofficeId,
                    name: data.d.results[0].va_name,
                    entityType: 'va_RegionalOffice'
                }]);
                alert("SOJ field was automatically updated to '" + data.d.results[0].va_name + "' based on the selected SOJ.");
            }
        });
}

function udo_AllTrackedItemsReceivedOrClosedChange() {
    if (Xrm.Page.getAttribute("udo_alltrackeditemsreceivedorclosed").getValue()) {
        var desc = Xrm.Page.getAttribute("udo_description").getValue();
        var text = "This claim has all tracked items received or closed and should have the status changed to 'Ready for Decision'.";
        if (!desc || desc.indexOf(text) == -1) {
            Xrm.Page.getAttribute('udo_description').setValue(desc + '\n\n' + text);
        }
    }
}

function ProcessedInShareChange() {
    var showExplanation = Xrm.Page.getAttribute('udo_processedfnodinshare').getValue() === false;
    Xrm.Page.getControl('udo_processedfnodinshareexplanation').setVisible(showExplanation);
    if (!showExplanation) {
        Xrm.Page.getAttribute('udo_processedfnodinshareexplanation').setValue(null);
    }
}

function FnodOtherChange() {
    Xrm.Page.getControl('udo_otherspecification').setVisible((Xrm.Page.getAttribute("udo_other").getValue() == true));
}

function RestrictPCRAccess(id, callback) {
    var restrictAccess = false;

    GetContactInformation(id, _exCon, function (rtContact) {

        if (rtContact && rtContact.fileNumber && rtContact.fileNumber.length > 0) {
            var user = GetUserSettingsForWebservice(_exCon);

            if (user && user.pcrId && rtContact.fileNumber == user.pcrId) {
                alert('You do not have permission to view this record because it is your own');

                Xrm.Page.ui.tabs.forEach(function (tab, index) {
                    tab.sections.forEach(function (section, index) {
                        section.controls.forEach(function (control, index) {
                            try {
                                control.setVisible(false);
                            } catch (er) { }
                        });
                        section.setVisible(false);
                    });
                    tab.setVisible(false);
                });

                Xrm.Page.ui.navigation.items.forEach(function (item, index) {
                    try {
                        item.setVisible(false);
                    } catch (er) { }
                });

                try {
                    window.close();
                } catch (er) { } finally {
                    restrictAccess = true;
                }
            }
        }
        callback(restrictAccess);
    });
}

function ShowScript() {
    /*    var scriptSource = _KMRoot + 'jurisdictions_routing.html';
    
        if (!_scriptWindowHandle || _scriptWindowHandle.closed) {
            _scriptWindowHandle = window.open(scriptSource, "CallScript", "width=600,height=500,scrollbars=1,resizable=1");
        } else {
            _scriptWindowHandle.open(scriptSource, "CallScript", "width=600,height=500,scrollbars=1,resizable=1");
        } */
}

function getUniqueRequestNumber() {
    var requestNumber = '[SET REQ NO]',
        vetId,
        ssn;
    var now = new Date();
    var today = (now.getMonth() + 1) + '/' + now.getDate() + '/' + now.getFullYear() + ' ' + now.getHours() + ':' + ((now.getMinutes() < 10 ? '0' : '') + now.getMinutes());
    try {

        if (Xrm.Page.getAttribute('udo_ssn').getValue() != null) {
            ssn = Xrm.Page.getAttribute('udo_ssn').getValue();
        }

        if (Xrm.Page.getAttribute('udo_relatedveteranid').getValue() != null) {
            vetId = Xrm.Page.getAttribute('udo_relatedveteranid').getValue();
        }

        if (vetId === null || vetId === undefined || ssn === null || ssn === undefined) {
            if (ssn != null) {
                requestNumber = ((ssn != null) ? ssn + ': 1' : 'Blank Forms ' + today);
            } else {
                requestNumber = 'Blank Forms ' + today;
            }
        }

        if (vetId[0].id !== null && vetId[0].id !== undefined && ssn !== null && ssn !== undefined) {
            // search by vet id if present, or ssn if present, or generate generic req number if none are available

            var columns = ['udo_reqnumber'];
            var filter = (vetId != null) ? "udo_RelatedVeteranId/Id eq guid'" + vetId[0].id.toString() + "' " : "udo_SSN eq '" + ssn.toString() + "' ";
            var orderby = "&$orderby=" + encodeURIComponent("CreatedOn desc ");
            var query = Xrm.Page.context.getServerUrl() + '/XRMServices/2011/OrganizationData.svc' + "/" + "udo_servicerequestSet"
                        + "?$select=" + columns.join(',') + "&$filter=" + encodeURIComponent(filter) + orderby;
            var priorServiceRequests = CrmRestKit2011.ByQueryUrl(query, false);

            priorServiceRequests.done(function (data) {
                count = 0;

                if (ssn == null) { ssn = 'SR: '; }

                if (data.d.results && data.d.results.length > 0) {
                    for (var i in data.d.results) {
                        if (data.d.results[i].udo_reqnumber != null) {
                            var temp = data.d.results[i].udo_reqnumber.split(":");
                            if (temp[1] != null && parseInt(temp[1]) >= count) {
                                count = parseInt(temp[1]) + 1;
                            }
                        }
                    }
                    requestNumber = ssn + ": " + count;
                } else {
                    requestNumber = ssn + ": 1";
                }
            });

            priorServiceRequests.fail(function (err) {
                UTIL.restKitError(err, 'Failed to prior service requests');
            });
        }
        Xrm.Page.getAttribute('udo_reqnumber').setValue(requestNumber);
    } catch (e) {
        requestNumber = 'Blank Forms ' + today;
        Xrm.Page.getAttribute('udo_reqnumber').setValue(requestNumber);
    }
}

function getManager() {
    var pcrOfRecordId = Xrm.Page.getAttribute('udo_pcrofrecordid').getValue();
    var columns = ['ParentSystemUserId'];
    var filter = "(SystemUserId eq guid'" + pcrOfRecordId[0].id + "')";
    CrmRestKit2011.ByQuery('SystemUser', columns, filter, true)
        .fail(function (err) {
            UTIL.restKitError(err, 'Failed to retrieve manager id');
        })
        .done(function (data) {
            if (data && data.d.results.length == 1) {
                var parentSystemUserId = data.d.results[0].ParentSystemUserId.Id;

                if (parentSystemUserId == null) {
                    alert('It has been detected that you do not have a manager assigned. It is recommended a manager is assigned to you.');

                    var description = Xrm.Page.getAttribute('udo_description');
                    description.setValue(description.getValue().replace('(Signature of NCCM)', ''));

                    return;
                }

                columns = ['FirstName', 'LastName'];
                filter = "(SystemUserId eq guid'" + parentSystemUserId + "')";

                CrmRestKit2011.ByQuery('SystemUser', columns, filter, true)
                    .fail(function (err) {
                        UTIL.restKitError(err, 'Failed to retrieve manager data');
                    })
                    .done(function (data) {
                        if (data && data.d.results.length == 1) {
                            manager = data.d.results[0].FirstName + ' ' + data.d.results[0].LastName;

                            var description = Xrm.Page.getAttribute('udo_description');
                            description.setValue(description.getValue().replace('(Signature of NCCM)', 'Sincerely,<br/>' + manager));
                        }
                    });
            }
        });
}

function copyMailingAddress() {
    var ga = ga = Xrm.Page.getAttribute;

    ga("udo_address1").setValue(ga("udo_mailing_address1").getValue());
    ga("udo_address2").setValue(ga("udo_mailing_address2").getValue());
    ga("udo_address3").setValue(ga("udo_mailing_address3").getValue());
    ga("udo_city").setValue(ga("udo_mailing_city").getValue());
    ga("udo_state").setValue(ga("udo_mailing_state").getValue());
    ga("udo_zipcode").setValue(ga("udo_mailing_zip").getValue());
    ga("udo_country").setValue(ga("udo_mailingcountry").getValue());
    ga("udo_email").setValue(ga("udo_sremail").getValue());
    ga("udo_phone").setValue(ga("udo_dayphone").getValue());
}