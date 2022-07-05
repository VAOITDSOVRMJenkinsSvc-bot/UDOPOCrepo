/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
var formContext = null
var exCon = null;
function Form_Onsave(executionObj) {
    exCon = executionObj;
    formContext = exCon.getFormContext();
    //if (ValidateZipcode() == false) {

    //      executionObj.getEventArgs().preventDefault(); // RU12 Changed all event.returnValue
    //      return false;
    //}

    var action = Xrm.Page.getAttribute('va_action').getSelectedOption().text, ga = Xrm.Page.getAttribute, gc = Xrm.Page.getControl;
    var vai0820 = (action == '0820' || action == '0820a' || action == '0820f' || action == '0820d' || action == '0820 & VAI' || action == 'VAI');

    if ((!ga('va_readscript').getValue() || ga('va_readscript').getValue() == false) && vai0820) {
        gc('va_readscript').setDisabled(false);
        gc('va_readscript').setFocus();
        //alert("Cannot save this Service Request record because 'Read Script' box is not set.");
        //executionObj.getEventArgs().preventDefault();
        //return false;
        ga('va_readscript').setValue(true);
    }


    // create dev note linked to record; only if vai0820=true AND when clicked on Send Mail
    try {
        var claimNo = ga('va_claimnumber').getValue();
        var noteId = ga('va_devnoteid').getValue(),
            noteExists = (noteId != null && noteId.length > 0),
            doCreateUpdateNote = (noteExists || (vai0820 && _sendMailButtonClicked));

        // Defect 97397 - The agreement with Bryan Broome is to only update the MAPD Note when the user specifically clicks "Save and Close." 
        var mode = executionObj.getEventArgs().getSaveMode();
        doCreateUpdateNote = (mode === 2);  // 2 - save and close

        if (doCreateUpdateNote) {
            if (!ga('va_participantid').getValue()) {
                if (!confirm('Participant ID field on Additional Information tab is blank. Without this field, Development Note cannot be created. Participant ID is present on Phone Call screen.\n\nIf you can provide this value on Service Request screen, press Cancel, update value and click on this button again. To proceed with the current operation without creating a development note, click OK.')) {
                    executionObj.getEventArgs().preventDefault(); // RU12 Changed all event.returnValue
                    return false;
                }
            }

            var reportedForFullName = '';
            var reportedForFirstName = ga("va_depfirstname").getValue();
            var reportedForLastName = ga("va_deplastname").getValue();
            if (reportedForFirstName == null) {
                reportedForFullName = reportedForLastName;
            }
            else if (reportedForFirstName != null) {
                reportedForFullName = reportedForFirstName + ' ' + reportedForLastName;
            }

            var sentFormText = '';
            if (ga('va_pmc').getValue() == 1) {
                sentFormText += 'PMC, ';
            }
            if (ga('va_nokletter').getValue() == 1) {
                sentFormText += 'NOK Letter, ';
            }
            if (ga('va_21530').getValue() == 1) {
                sentFormText += '21-530, ';
            }
            if (ga('va_21534').getValue() == 1) {
                sentFormText += '21-534, ';
            }
            if (ga('va_401330').getValue() == 1) {
                sentFormText += '40-1330, ';
            }
            if (ga('va_other').getValue() == 1) {
                var specificationText = ga('va_otherspecification').getValue() === null ? '' : ga('va_otherspecification').getValue();
                sentFormText += specificationText;
            }

            var fnodVeteranFullName = '';
            fnodVeteranFullName = ga('va_srfirstname').getValue() + ' ' + ga('va_srlastname').getValue();

            ShowProgress((noteExists ? 'Updating' : 'Creating new') + ' Development Note');
            var devNoteText = "Service Request '" + Xrm.Page.getAttribute('va_reqnumber').getValue() +
                ((Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE) ? "' created. " : "' updated. ") +
                (ga('va_ssn').getValue() ? "File #: " + ga('va_ssn').getValue() + ". " : "") +
                (ga('va_issue').getValue() ? "Type: " + ga('va_issue').getText() + ". " : "") +
                (ga('va_action').getValue() ? "Action: " + ga('va_action').getText() + ". " : "") +
                (ga('va_regionalofficeid').getValue() ? "SOJ: " + ga('va_regionalofficeid').getValue()[0].name + ". " : "") +
                (ga('va_description').getValue() ? "\n\nDesc.: " + ga('va_description').getValue() : "") +
                //(ga('va_claimdetails').getValue() ? "\n\n Rel.Info: " + ga('va_claimdetails').getValue() : "") + 
                (ga("va_letterscreated").getValue() ? "\n\nThe following letters were sent: " + ga("va_letterscreated").getValue() : "");
            if (action == '0820d') {
                devNoteText += (ga('va_dateofmissingpayment').getValue() ? ".\n\nMissing Payment Date: " + new Date(ga('va_dateofmissingpayment').getValue()).format('MM/dd/yyyy') : "") +
                        (ga('va_amountofpayments').getValue() ? ".\nMissing Payment Amt: $" + ga('va_amountofpayments').getValue() : "");
            }
            if (action == '0820f') {
                devNoteText = "0820f sent for MOD to ROJ " + (ga('va_regionalofficeid').getValue() ? "'" + ga('va_regionalofficeid').getValue()[0].name + "'\n" : "") +
                (ga('va_nameofreportingindividual').getValue() ? "Reported By " + "'" + ga('va_nameofreportingindividual').getValue() + "'" + "\n" : "") +
                (reportedForFullName ? "Reported for " + "'" + reportedForFullName + "'" + ", " : "") + (ga('va_depdateofbirth').getValue() ? "DOB: " + new Date(ga('va_depdateofbirth').getValue()).format('MM/dd/yyyy') + ", " : "") + (ga('va_depssn').getValue() ? "SSN: " + ga('va_depssn').getValue() : "") + (ga('va_description').getValue() ? "\n\nDesc.: " + ga('va_description').getValue() : "") + (ga("va_letterscreated").getValue() ? "\n\nThe following letters were sent: " + ga("va_letterscreated").getValue() : "");
            }
            if (action == '0820a') {
                devNoteText = "0820a sent for FNOD\n" +
                (ga('va_nameofreportingindividual').getValue() ? "Reported By " + "'" + ga('va_nameofreportingindividual').getValue() + "'" + "\n" : "") +
                (fnodVeteranFullName ? "Reported for " + fnodVeteranFullName + ", " : "") + (ga('va_dateofdeath').getValue() ? "Date of Death: " + new Date(ga('va_dateofdeath').getValue()).format('MM/dd/yyyy') + ", " : "") + (ga('va_srssn').getValue() ? "SSN: " + ga('va_srssn').getValue() + "\n" : "") +
                (sentFormText ? "Sent Forms:  " + sentFormText : "") + (ga('va_description').getValue() ? "\n\nDesc.: " + ga('va_description').getValue() : "") + (ga("va_letterscreated").getValue() ? "\n\nThe following letters were sent: " + ga("va_letterscreated").getValue() : "");
            }

            // call mapdnote web service
            var mapdOptions = {
                nodeId: noteId,
                ptcpntId: ga('va_participantid').getValue(),
                noteText: devNoteText
            };

            var mapdResults = UTIL.mapdNote(mapdOptions);

            ga('va_roxml').setValue(formatXml(mapdResults.responseXml));
            ga('va_roxml').setSubmitMode('always');

            ga('va_notecreaterequest').setValue(formatXml(mapdResults.requestXml));
            ga('va_notecreaterequest').setSubmitMode('always');

            if (!mapdResults.success) {
                CloseProgress();

                // add a log entry, also used to validate permission to edit note
                var msg = 'SR Note ' + (noteExists ? 'update' : 'create') + ' failure' +
                 (noteId ? '; ID: ' + noteId : '');
                var cols = {
                    va_Error: true,
                    va_Warning: false,
                    va_Summary: false,
                    va_name: msg,
                    va_Description: devNoteText,
                    va_NoteId: noteId,
                    va_Request: formatXml(mapdResults.service),
                    va_Query: formatXml(mapdResults.soapBodyXml),
                    va_Duration: 0,
                    va_ServiceRequestId: { 'Id': Xrm.Page.data.entity.getId() },
                    va_Response: mapdResults.responseXml
                };
                var log = CrmRestKit2011.Create('va_querylog', cols, false)
                      .fail(function (err) {
                          UTIL.restKitError(err, 'Failed to create dev note');
                      })
                      .done(function (data, status, xhr) {
                      });


                if (!confirm('Failed to ' + (noteExists ? 'update' : 'create') +
                    ' a Dev Note. Would you like to continue saving Service Request record?\n\nServer returned following error information: ' + GetErrorMessages('\n'))) {
                    executionObj.getEventArgs().preventDefault();
                    return false;
                }
            }
            else if (mapdResults.responseXml != null && mapdResults.responseXml.length > 0) {
                createNote_xmlObject = _XML_UTIL.parseXmlObject(mapdResults.responseXml);
                ga('va_notecreated').setSubmitMode('always');
                ga('va_notecreated').setValue(true);

                if (createNote_xmlObject.selectSingleNode('//noteId') != null) {
                    noteId = createNote_xmlObject.selectSingleNode('//noteId').text;
                    if (noteId) {
                        ga('va_devnoteid').setValue(noteId);
                        ga('va_devnoteid').setSubmitMode('always');
                    }
                }
                // add a log entry, also used to validate permission to edit note
                var msg = 'SR Note ' + (noteExists ? 'update' : 'create') + '; ID: ' + noteId;
                var cols = {
                    va_Error: false,
                    va_Warning: false,
                    va_Summary: true,
                    va_name: msg,
                    va_Description: devNoteText,
                    va_NoteId: noteId,
                    va_Request: formatXml(mapdResults.service),
                    va_Query: formatXml(mapdResults.soapBodyXml),
                    va_Duration: 0,
                    va_ServiceRequestId: { 'Id': Xrm.Page.data.entity.getId() },
                    va_Response: mapdResults.responseXml
                };
                try {
                    CrmRestKit2011.Create('va_querylog', cols, false)
                      .fail(function (err) {
                          UTIL.restKitError(err, 'Failed to create dev note');
                      })
                      .done(function (data, status, xhr) {
                      });

                }
                catch (qle) { }

                vipNoteCreated(claimNo, ga('va_participantid').getValue());
            }

            CloseProgress();
        }

    } catch (err) {
        CloseProgress();
        if (!confirm("Error occurred when creating a Dev Note: " + err.description + '\n\nContinue saving Service Request record?')) {
            executionObj.getEventArgs().preventDefault(); // RU12 changed all event.returnValue
            return false;
        }
    }

    window.status = '+';
    if (_scriptWindowHandle) { try { _scriptWindowHandle.close(); } catch (e) { } }

    return true;
}

function GetErrorMessages(separator) {
    var msg = '';
    if (_VRMMESSAGE && _VRMMESSAGE.length > 0) {
        for (var i = 0; i < _VRMMESSAGE.length; i++) {
            if (_VRMMESSAGE[i].errorFlag) msg += separator + _VRMMESSAGE[i].description;
        }
    }
    return msg;
}

function vipNoteCreated(claimNumber, participantId) {
    var searchIframe = null,
        parentWindow;

    if (window.parent && window.parent.opener && window.parent.opener.Xrm) {
        parentWindow = window.parent.opener;
    }
    else {
        parentWindow = window.top.opener.parent;
    }

    if (parentWindow.Xrm.Page.data.entity.getEntityName() == "phonecall") {
        searchIframe = parentWindow.document.getElementById('IFRAME_search').contentWindow;
    }
    else if (parentWindow.Xrm.Page.data.entity.getEntityName() == "contact") {
        searchIframe = parentWindow.document.getElementById('IFRAME_ro').contentWindow;
    }
    if (searchIframe) {
        searchIframe._extApp.fireEvent('servicerequestnotecreated', claimNumber, participantId);
    }
    else return null;
}

function ValidateZipcode() {
    var va_mailing_zip = Xrm.Page.getAttribute('va_mailing_zip').getValue();
    if (va_mailing_zip != null && va_mailing_zip.match(/[a-zA-Z]/)) {
        alert('Mailing Address Zip field contains invalid alphabetical characters');
        return false;
    }

    return true;
}
