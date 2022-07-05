_issueMap = null;
_suppressClaimScriptPopup = false;
_noAutoPopups = true;

// Handle no of dispositions change
function NoOfDispositionsOnClick() {
    showAndHideDispositions();
    if (!_isLoading) updateSubject();
}

function RefreshDispAndSubject(dispControl, dispSub, dispComment) {
    var d = Xrm.Page.ui.controls.get(dispControl);
    var sc = Xrm.Page.ui.controls.get(dispSub);
    var dcom = Xrm.Page.ui.controls.get(dispComment);

    updateDispositionSubType(d, sc, dcom, dispControl, dispSub);

    if (_isLoading) return;
    ClaimsAddSectionVisibility();
    updateSubject();

    if (_noAutoPopups || _suppressClaimScriptPopup) { return; }

    PopupScript(dispControl, dispSub);
}

function ClaimsAddSectionVisibility() {
    var disp = new Array();
    disp[0] = Xrm.Page.getAttribute('va_disposition');
    disp[1] = Xrm.Page.getAttribute('va_disposition2');
    disp[2] = Xrm.Page.getAttribute('va_disposition3');
    disp[3] = Xrm.Page.getAttribute('va_disposition4');
    disp[4] = Xrm.Page.getAttribute('va_disposition5');

    // show/hide claim additional info section

    var showClaimsInfoSection = false;
    for (var i = 0; i < disp.length; i++) {
        if (disp[i].getText() == 'Claim') {
            showClaimsInfoSection = true;
            break;
        }
    }
}

//_dispStructureAndPopup is a collection of all data for Call Scripts
function GetCallScripts(forceRefresh) {
    if (!_dispStructureAndPopup || _dispStructureAndPopup.length == 0 || forceRefresh == true) {
        //ShowProgress('Retrieving Call Script Settings');
        var columns = ['va_name', 'va_Issue', 'va_SubIssue', 'va_ServiceRequestDescription', 'va_RequiresServiceRequest', 'va_DefaultSRAction', 'va_FireEventToChildController'];
        var filter = "&$orderby=va_Issue asc,va_SubIssue asc "; // "va_Issue/Value eq " + newValue.toString() + " and va_SubIssue/Value eq " + (subTypeValue != -1 ? subTypeValue.toString() : "null") + " ";

        //NEW CODE
        filter = '';
        var callScripts = CrmRestKit2011.ByQueryAll('va_callscript', columns, filter, false);
        //var scripts = CrmRestKit.RetrieveMultiple('va_callscript', columns, filter, null, 250);
        callScripts.fail(
            function (err) {
            })
        .done(
        function (scripts) {

            if (scripts && scripts.length > 0) {

                _dispStructureAndPopup = scripts;
                //var nextCol = null;

                //do {
                //    if (scripts.next && scripts.next != undefined) {
                //        nextCol = scripts.LoadNext();
                //        _dispStructureAndPopup = _dispStructureAndPopup.concat(nextCol.results);

                //        if (nextCol.next && nextCol.next != undefined) {
                //            nextCol = nextCol.LoadNext();
                //        }
                //        else { nextCol = null; }
                //    }
                //}
                //while (nextCol && nextCol.results && nextCol.results.length > 0);
            }
        });
        //CloseProgress();
    }

    return _dispStructureAndPopup;
}

//=============================================
// START FUNCTION PopupScript()
// Description: Pops up script based on info provided from call type and sub type
function PopupScript(dispositionControl, subTypeControl, notifyIfNoFileFound, onlyFnodCheck, doPopup) {
    var callType;
    var subType;
    var newValue = -1;

    var disp = new Array();
    disp['va_disposition'] = Xrm.Page.getAttribute('va_disposition');
    disp['va_disposition2'] = Xrm.Page.getAttribute('va_disposition2');
    disp['va_disposition3'] = Xrm.Page.getAttribute('va_disposition3');
    disp['va_disposition4'] = Xrm.Page.getAttribute('va_disposition4');
    disp['va_disposition5'] = Xrm.Page.getAttribute('va_disposition5');
    var dispSub = new Array();
    dispSub['va_dispositionsubtype'] = Xrm.Page.getAttribute('va_dispositionsubtype');
    dispSub['va_dispositionsubtype2'] = Xrm.Page.getAttribute('va_dispositionsubtype2');
    dispSub['va_dispositionsubtype3'] = Xrm.Page.getAttribute('va_dispositionsubtype3');
    dispSub['va_dispositionsubtyp4'] = Xrm.Page.getAttribute('va_dispositionsubtype4');
    dispSub['va_dispositionsubtyp5'] = Xrm.Page.getAttribute('va_dispositionsubtype5');
    if (onlyFnodCheck != true && (!_noAutoPopups || doPopup)) {
        if (disp[dispositionControl].getValue()) newValue = parseInt(disp[dispositionControl].getValue());
        callType = !disp[dispositionControl].getText() ? '' : disp[dispositionControl].getText();
        //subType = !dispSub[subTypeControl].SelectedText ? '' : !dispSub[subTypeControl].SelectedText;
        var subTypeValue = -1; if (dispSub[subTypeControl].getValue()) subTypeValue = parseInt(dispSub[subTypeControl].getValue());

        var haveScript = true;     // make sure both disp and sub disp are selectd prior to showing script
        var scripts = GetCallScripts(false);
        if (!scripts || scripts.length == 0) {
            alert('Call Script settings are empty.');
            haveScript = false;
        }

        var scriptFile = null, openDirectly = true;
        if (haveScript) {
            for (var i = 0; i < scripts.length; i++) {
                if (scripts[i].va_Issue && scripts[i].va_SubIssue && scripts[i].va_Issue.Value == newValue) {
                    // if no subissue defined OR no script for subissue, get script for main issue
                    if (subTypeValue == -1 /*|| */) {
                        if (scripts[i].va_SubIssue.Value == null) { scriptFile = scripts[i].va_name; }
                    }
                    else {
                        if (scripts[i].va_SubIssue.Value == subTypeValue) { scriptFile = scripts[i].va_name; }
                    }
                    if (scriptFile) {
                        if (scripts[i].va_FireEventToChildController != null && scripts[i].va_FireEventToChildController == true) {
                            openDirectly = false;
                        }
                        break;
                    }
                }
            }

            if (!scriptFile || scriptFile.length == 0) {
                if (notifyIfNoFileFound && subTypeValue != -1) {
                    alert('Call Script for the selected combination of Issue and Sub-Issue is not found.');
                }
                haveScript = false;
            }
        }

        if (haveScript) {
            var doOpen = true;
            if (!openDirectly) {
                // get ref to extjs application and fire Open Script event passing script file name
                var vipApp = null, vipContentWindow = document.getElementById('IFRAME_search');
                if (vipContentWindow && vipContentWindow != undefined) { vipContentWindow = document.getElementById('IFRAME_search').contentWindow; }
                if (vipContentWindow && vipContentWindow != undefined) {
                    vipApp = vipContentWindow._extApp;
                    if (vipApp && vipApp != undefined) {
                        doOpen = false;
                        vipApp.fireEvent('crmscriptinvoked', callType, scriptFile);
                    }
                }
            }

            if (doOpen) {
                ShowScriptWindow(scriptFile);
            }
        }
    }

    // show/hide claim FNOD tab
    _fnod = false;
    var i = 1;

    for (s in disp) {
        switch (disp[s].getText()) {
            case 'FNOD':
                _fnod = true;
                break;
            case 'General Questions':
                var di = s.substring(0, 14) + 'subtype' + (i > 1 ? i.toString() : '');
                if (dispSub[di] != undefined && dispSub[di] && dispSub[di].getSelectedOption().text == 'PMC') { _fnod = true; }
                break;
        }
        i++;
    }
    Xrm.Page.ui.tabs.get('tab_fnod').setVisible(_fnod);
    Xrm.Page.getAttribute("va_fnodreportingfor").setRequiredLevel(_fnod ? "required" : "none");
}
// END FUNCTION PopupScript()
//=============================================

//=============================================
// START FUNCTION ShowScriptWindow()
// Description: ShowScriptWindow.
function ShowScriptWindow(scriptFile) {
    /*
    var orgname = Xrm.Page.context.getOrgUniqueName();
    var scriptRoot;
    scriptRoot = Xrm.Page.context.getServerUrl().replace(orgname, '');
    var scriptSource = scriptRoot + 'isv/scripts/' + scriptFile;
    */
    var scriptSource = _KMRoot + scriptFile;

    if (!_scriptWindowHandle || _scriptWindowHandle.closed) {
        _scriptWindowHandle = window.open(scriptSource, "CallScript", "width=600,height=500,scrollbars=1,resizable=1");
    }
    else {
        _scriptWindowHandle.open(scriptSource, "CallScript", "width=600,height=500,scrollbars=1,resizable=1");
    }

    try { _scriptWindowHandle.focus(); }
    catch (er) { }
}
// END FUNCTION ShowScriptWindow()
//=============================================

////////////////////////////////////////////////////////////////////////////////////////////////
///// FROM PROTOTYPE 
////////////////////////////////////////////////////////////////////////////////////////////////

//=============================================
// START FUNCTION setupCreateFormOnLoad()
// Description: Test function to determine order of optionset array.

function setupCreateFormOnLoad() {

    if (Xrm.Page.ui.getFormType() == 1) { return; }

    // Set the required fields and visiblity for the first section
    // Other sections are taken care of by showAndHideDispositions()
    try {
        var subdispAttr = Xrm.Page.getAttribute("va_dispositionsubtype");
        if (subdispAttr.getValue() != null) {
            subdispAttr.setRequiredLevel("required");
        }
        checksubType(Xrm.Page.getAttribute('va_dispositionsubtype'), Xrm.Page.getAttribute('va_dispositioncomments'), 'va_dispositionsubtype', 'va_dispositioncomments');
    } catch (err) {
        alert("An error has occurred in function setupCreateFormOnLoad() in Phone Functions. " +
				 "Please notify your system administrator.\nError:" + err.description);
    }

    bindSubTypes(Xrm.Page.ui.controls.get("va_disposition"),
							   Xrm.Page.ui.controls.get("va_dispositionsubtype"),
							   Xrm.Page.ui.controls.get("va_dispositioncomments"),
							   'va_disposition', 'va_dispositionsubtype');

    var dispPrefix = "va_disposition";
    var subdispPrefix = "va_dispositionsubtype";
    var othercommentPrefix = "va_dispositioncomments";
    var MAX = 5;
    var numOfDispositions = Xrm.Page.getAttribute("va_numberofdispositions").getValue();

    for (count = 2; count <= numOfDispositions; count++) {
        var dispAttrName = dispPrefix + count;
        var subdispAttrName = subdispPrefix + count;
        var othercommentAttrName = othercommentPrefix + count;

        bindSubTypes(Xrm.Page.ui.controls.get(dispAttrName),
								   Xrm.Page.ui.controls.get(subdispAttrName),
								   Xrm.Page.ui.controls.get(othercommentAttrName),
								   'va_disposition', 'va_dispositionsubtype');
    }

}
// END FUNCTION setupCreateFormOnLoad()
//=============================================

function bindSubTypes(dispControl, subdispControl, otherCommentControl, dispControlName, subdispControlName) {

    try {
        var subdispAttr = subdispControl.getAttribute();
        var originalVal = subdispAttr.getValue();
        updateDispositionSubType(dispControl, subdispControl, otherCommentControl, dispControlName, subdispControlName);
        subdispAttr.setValue(originalVal);
    } catch (err) {
        alert("An error has occurred in function bindSubTypes () in Phone Functions. " +
				 "Please notify your system administrator.\nError:" + err.description);
    }
}



//=============================================
// START FUNCTION showAndHideDispositions()

function showAndHideDispositions() {

    var sectionPrefix = "phonecall_section_disp";
    var MAX = 5;
    var numOfDispositions = Xrm.Page.getAttribute("va_numberofdispositions").getValue(); //  Xrm.Page.getAttribute("va_numberofdispositions").getValue();
    var tabObj = Xrm.Page.ui.tabs.get("Categorize Call");
    var manyDisp = (numOfDispositions != null && parseInt(numOfDispositions) >= 2);
    //alert(manyDisp);
    //alert(parseInt(numOfDispositions));
    try {
        for (i = 2; i <= MAX; i++) {
            var sectionName = sectionPrefix + i;
            if (i <= numOfDispositions) {
                // Make these sections visible
                sectionObj = tabObj.sections.get(sectionName);
                if (sectionObj != null && sectionObj.getVisible() == false) {
                    var length = sectionObj.controls.getLength();
                    for (n = 0; n < length; n++) {
                        var controlObj = sectionObj.controls.get(n);
                        //                        var name = sectionObj.controls.get(n).getName();
                        var name = controlObj.getName();
                        var attrValue = controlObj.getAttribute().getValue();

                        // If you can find dispositionsubtype in the control name & it's related attribute value
                        // is not null then go ahead and disable it until the dependent picklist is selected.
                        if (name.search("dispositionsubtype") != -1) {
                            if (attrValue == null) { // If there is no current value saved, disable it
                                controlObj.setDisabled(true);
                                controlAttr = controlObj.getAttribute();
                                controlAttr.setRequiredLevel("none");
                            } else { // Don't disable it.
                                if (!_completed) { controlObj.setDisabled(false); }
                                controlAttr = controlObj.getAttribute();
                                controlAttr.setRequiredLevel("required");
                            }
                        } else if (name.search("disposition") != -1) {
                            if (!_completed) { controlObj.setDisabled(false); }
                            controlAttr = controlObj.getAttribute();
                            controlAttr.setRequiredLevel("required");
                        } else if (name.search("othercomments") != -1) {
                            // First check the value of the corresponding subdisposition type
                            var ds = new Array();
                            ds['va_dispositionsubtype'] = Xrm.Page.getAttribute('va_dispositionsubtype');
                            ds['va_dispositionsubtype2'] = Xrm.Page.getAttribute('va_dispositionsubtype2');
                            ds['va_dispositionsubtype3'] = Xrm.Page.getAttribute('va_dispositionsubtype3');
                            ds['va_dispositionsubtype4'] = Xrm.Page.getAttribute('va_dispositionsubtype4');
                            ds['va_dispositionsubtype5'] = Xrm.Page.getAttribute('va_dispositionsubtype5');
                            /*var subdispPrefix = "va_dispositionsubtype";
                            var subdispAttrName = subdispPrefix + i;
                            var subdispValue = Xrm.Page.getAttribute(subdispAttrName).getValue();
                            */
                            var subdispValue = ds[i - 1].getValue() ? parseInt(ds[i - 1].getValue()) : null;

                            // If it's 953850057 (Other), then this box needs to be visible, enabled and required
                            if (subdispValue == 953850057) {
                                if (!_completed) { controlObj.setDisabled(false); }
                                controlObj.setVisible(true);
                                controlAttr = controlObj.getAttribute();
                                controlAttr.setRequiredLevel("required");
                            } else {
                                // If it's anything else, then it needs to be invisible, disabled and not required.
                                controlAttr = controlObj.getAttribute();
                                controlAttr.setRequiredLevel("none");
                                controlObj.setDisabled(true);
                                controlObj.setVisible(false);
                            }
                        } // END ELSE IF
                    } // END FOR
                    sectionObj.setVisible(true);
                } // END IF INVISIBLE
            } else {
                // Hide these sections
                sectionObj = tabObj.sections.get(sectionName);
                if (sectionObj != null && sectionObj.getVisible() == true) {

                    var length = sectionObj.controls.getLength();

                    // Get each control.  Clear the value, and make it not required and disable it.
                    for (n = 0; n < length; n++) {
                        var controlObj = sectionObj.controls.get(n);
                        controlAttr = controlObj.getAttribute();
                        controlAttr.setRequiredLevel("none");
                        controlAttr.setValue(null);
                        controlObj.setDisabled(true);

                    }
                    //                    dispAttrObj.setRequiredLevel("none");
                    sectionObj.setVisible(false);
                }
            } // END ELSE
        } // END FOR section loop
    } catch (err) {
        alert(err.description);
    }
    //tabObj.setVisible(manyDisp);
}
// END FUNCTION showAndHideDispositions()
//=============================================


function GetBirlsName() {
    var birlsName = '';

    var birlsXml = Xrm.Page.getAttribute('va_findbirlsresponse').getValue();

    if (birlsXml && birlsXml != undefined && birlsXml != '') {
        var birlsXmlObject = _XML_UTIL.parseXmlObject(birlsXml);

        var firstName = SingleNodeExists(birlsXmlObject, '//FIRST_NAME') ? birlsXmlObject.selectSingleNode('//FIRST_NAME').text : null;
        var lastName = SingleNodeExists(birlsXmlObject, '//LAST_NAME') ? birlsXmlObject.selectSingleNode('//LAST_NAME').text : null;
        var middleName = SingleNodeExists(birlsXmlObject, '//MIDDLE_NAME') ? birlsXmlObject.selectSingleNode('//MIDDLE_NAME').text : null;

        if (lastName && lastName != undefined && lastName != '') { birlsName += lastName + ', '; }
        if (firstName && firstName != undefined && firstName != '') { birlsName += firstName; }
        if (middleName && middleName != undefined && middleName != '') { birlsName += ' ' + middleName + '.'; }
    }

    return birlsName;
}
//=============================================
// START FUNCTION updateSubject()
function updateSubject() {
    if (_isLoading) return;

    var numOfDispositions = Xrm.Page.getAttribute("va_numberofdispositions").getValue(); //  Xrm.Page.getAttribute("va_numberofdispositions").getValue();
    var disp = [Xrm.Page.getAttribute('va_disposition'), Xrm.Page.getAttribute('va_disposition2'), Xrm.Page.getAttribute('va_disposition3'),
		Xrm.Page.getAttribute('va_disposition4'), Xrm.Page.getAttribute('va_disposition5')];
    var fieldPrefix = "va_disposition";
    var subjTxt = "", other = 953850007;
    var MAX = 5;
    // When any of the fields change, blow away the current version of the subject
    Xrm.Page.getAttribute("subject").setValue(null);

    // Get the title of the first disposition separately
    try {

        var vetName = '';
        if (Xrm.Page.getAttribute("regardingobjectid").getValue() &&
			Xrm.Page.getAttribute("regardingobjectid").getValue().length > 0 &&
			Xrm.Page.getAttribute("regardingobjectid").getValue()[0]) {
            var tempName = Xrm.Page.getAttribute("regardingobjectid").getValue()[0].name;
            if (tempName && tempName != undefined) {
                vetName = ' - ' + tempName;
            }
        }
        else {
            if (_birlsResult) {
                vetName = 'BIRLS - ' + GetBirlsName();
            } else {
                tempName = Xrm.Page.getAttribute('va_lastname').getValue() +
                    (Xrm.Page.getAttribute('va_firstname').getValue() ? ', ' + Xrm.Page.getAttribute('va_firstname').getValue() : '')
                    + (Xrm.Page.getAttribute('va_middleinitial').getValue()
                        ? ' ' + Xrm.Page.getAttribute('va_middleinitial').getValue() + '.' : '');
                vetName = ' - ' + tempName;
            }
        }



        // Do not auto-populate Subject when Call Type is Other
        if (disp[0].getValue() != other) subjTxt += disp[0].getText();
        if (disp[0].getText() == 'Suicide Call' || disp[0].getText() == 'Threat Call') Xrm.Page.getAttribute("prioritycode").setValue(2);

        if (numOfDispositions == 1) {
            subjTxt += vetName;
            Xrm.Page.getAttribute("subject").setValue(subjTxt);
            return;
        }
    } catch (err) {
        alert(err.description);
    }
    // Then cycle through each of the dispositions and get the value of the name
    try {
        for (i = 2; i <= numOfDispositions; i++) {
            var txt = disp[i - 1].getText(); //  Xrm.Page.getAttribute(fieldPrefix + i).getText();
            if (txt != "" && disp[i - 1].getValue() != other) {
                subjTxt += (subjTxt.length > 0 ? "; " : "") + txt;
                if (txt == 'Suicide Call' || txt == 'Threat Call') Xrm.Page.getAttribute("prioritycode").setValue(2);
            }
        }

        subjTxt += vetName;

        Xrm.Page.getAttribute("subject").setValue(subjTxt);
        Xrm.Page.getAttribute("subject").setSubmitMode("always");
        return;

    } catch (err) {
        alert(err.description);
    }

    // Append it to the message in order, separated by semicolons.
    // Update the Subject


}
// END FUNCTION updateSubject()
//=============================================

//=============================================
// START FUNCTION updateDispositionSubType()
//=============================================
//function updateDispositionSubType() {
function updateDispositionSubType(dispositionControl, subtypeControl, otherCommentsControl,
	dispositionControlName, subtypeControlName) {
    var disp = new Array();
    disp['va_disposition'] = Xrm.Page.getAttribute('va_disposition');
    disp['va_disposition2'] = Xrm.Page.getAttribute('va_disposition2');
    disp['va_disposition3'] = Xrm.Page.getAttribute('va_disposition3');
    disp['va_disposition4'] = Xrm.Page.getAttribute('va_disposition4');
    disp['va_disposition5'] = Xrm.Page.getAttribute('va_disposition5');

    if (!dispositionControl || dispositionControl == undefined) {
        return false;
    }
    var dispositionAttr = dispositionControl.getAttribute();
    var newValue = -1;
    if (disp[dispositionControlName].getValue()) newValue = parseInt(disp[dispositionControlName].getValue()); //  dispositionAttr.getValue();

    var subtypeAttribute = subtypeControl.getAttribute();

    if (!_issueMap) {
        // load a collection that maps issue and subissue
        var subtypeOptions = subtypeControl.getAttribute().getOptions();
        var issueOptions = dispositionControl.getAttribute().getOptions();
        _issueMap = new Array();

        for (var i = 0; i < issueOptions.length; i++) {
            var startTag = issueOptions[i].text + ':';
            var subIssues = new Array();
            var subSort = new Array();
            if (startTag.length > 1) {
                for (var k = 0; k < subtypeOptions.length; k++) {
                    if (subtypeOptions[k].text.indexOf(startTag) == 0) {
                        var option = subtypeOptions[k];
                        option.text = subtypeOptions[k].text.substring(startTag.length);
                        subIssues.push(option);
                        subSort.push(option.text);
                    }
                }

                subSort = subSort.sort();
                var subIssuesSorted = new Array();

                for (var k = 0; k < subSort.length; k++) {
                    var tag = subSort[k];
                    for (var j = 0; j < subIssues.length; j++) {
                        if (subIssues[j].text == tag) { subIssuesSorted.push(subIssues[j]); break; }
                    }
                }

                _issueMap[startTag] = subIssuesSorted;
            }
        }
    }

    var otherCommentsAttribute = otherCommentsControl.getAttribute();

    if (newValue != null && newValue !== -1) {
        if (!_completed) { subtypeControl.setDisabled(false); }
        subtypeAttribute.setRequiredLevel("required");

        if (newValue != 953850005) { // If it's not a general question
            // Clear the other comments, make it not required and hide it.
            otherCommentsAttribute.setValue(null);
            otherCommentsAttribute.setRequiredLevel("none");
            otherCommentsControl.setVisible(false);
        }

        try {
            subtypeControl.clearOptions();

            // suboption starts with main option text + ':'
            if (disp[dispositionControlName].getSelectedOption()) {
                var startTag = disp[dispositionControlName].getSelectedOption().text + ':';
                var subIssuesForCurrentIssue = _issueMap[startTag];
                if (subIssuesForCurrentIssue) {
                    for (i = 0; i < subIssuesForCurrentIssue.length; i++) {
                        subtypeControl.addOption(subIssuesForCurrentIssue[i]);
                    }
                }
            }

            //$('#va_dispositionsubtype option[value=953850210]').css('color', 'red');
            //$('#va_dispositionsubtype option[value=953850216]').css('color', 'red');

        } catch (err) {
            alert(err.description);
        }
    } else {
        //SET TO NULL
        subtypeControl.clearOptions();
        subtypeControl.setDisabled(true);
        subtypeAttribute.setRequiredLevel("none");
        otherCommentsAttribute.setValue(null);
        otherCommentsAttribute.setRequiredLevel("none");
        otherCommentsControl.setVisible(false);
        return;
    } // END IF-ELSE

}
// END FUNCTION updateDispositionSubType()
//=============================================


//=============================================
// START FUNCTION disableDispositionSubType()
// Description: Called onLoad
//=============================================
function disableDispositionSubType() {

    var subtypeControl = Xrm.Page.ui.controls.get("va_dispositionsubtype");
    var subtypeAttribute = subtypeControl.getAttribute();
    var otherCommentsControl = Xrm.Page.ui.controls.get("va_dispositioncomments");
    var otherCommentsAttribute = otherCommentsControl.getAttribute();
    var currentValue = Xrm.Page.getAttribute("va_dispositionsubtype").getValue();

    if (Xrm.Page.ui.getFormType() == 1) {
        subtypeAttribute.setRequiredLevel("none");
        subtypeControl.setDisabled(true);
        otherCommentsAttribute.setRequiredLevel("none");
        otherCommentsControl.setVisible(false);
        return;
    }

    if (Xrm.Page.ui.getFormType() == 2) {
        if (currentValue == null) {
            subtypeControl.clearOptions();
            subtypeControl.setDisabled(true);
        }
        return;
    }

}
// END FUNCTION disableDispositionSubType()
//=============================================


//=============================================
// START FUNCTION checksubType()
//=============================================
//function checksubType() {
function checksubType(subtypeControl, otherCommentsControl, subtypeControlName, otherCommentsControlName) {


    var subtypeAttribute = Xrm.Page.getAttribute(subtypeControlName);
    var otherCommentsAttribute = Xrm.Page.getAttribute(otherCommentsControlName);
    var val = subtypeControl.DataValue ? parseInt(subtypeControl.DataValue) : null;

    if (val != 953850057 && val === null) { // Not equal to Other
        otherCommentsAttribute.setValue(null);
        otherCommentsAttribute.setRequiredLevel("none");
        Xrm.Page.ui.controls.get(otherCommentsControlName).setVisible(false);
        Xrm.Page.ui.controls.get(otherCommentsControlName).setDisabled(true);

    } else { // if it is equal to other
        subtypeAttribute.setRequiredLevel("required");
        //Xrm.Page.ui.controls.get(otherCommentsControlName).setVisible(true);
        //if (!_completed) { Xrm.Page.ui.controls.get(otherCommentsControlName).setDisabled(false); }
        //otherCommentsAttribute.setRequiredLevel("required");
    }
} // END FUNCTION checksubType()
//=============================================

// automatically allocate type/subtype
// Claim: general status (953850001:953850082); CAD (953850015:953850002); Payments-General Status (953850010:953850084); PAYMENT_ANY (953850010:?)
function SetPrimaryTypeSubtype(operation, suppressScript, overrideCurrentSetting, optionalSubtype) {
    try {
        _suppressClaimScriptPopup = suppressScript;

        var typeValue = null; var subtypeValue = null;
        switch (operation.toUpperCase()) {
            case 'CLAIM_GENERAL':
                typeValue = 953850001; subtypeValue = 953850082;
                break;
            case 'CAD':
                typeValue = 953850015; subtypeValue = 953850002;
                break;
            case 'PAYMENT_GENERAL':
                typeValue = 953850010; subtypeValue = 953850084;
                break;
            case 'APPEAL_GENERAL':
                typeValue = 953850000; subtypeValue = 953850035;
                break;
            case 'PAYMENT_ANY':
                typeValue = 953850010;
                if (optionalSubtype != undefined) { subtypeValue = optionalSubtype; }
                break;
            default:
                alert('SetPrimaryType routine cannot be executed: unknown operation ' + operation);
                return;
        }
        if (overrideCurrentSetting || !Xrm.Page.getAttribute("va_disposition").getValue()) {
            Xrm.Page.getAttribute("va_disposition").setValue(typeValue);
            RefreshDispAndSubject("va_disposition", "va_dispositionsubtype", "va_dispositioncomments");
            Xrm.Page.getAttribute("va_dispositionsubtype").setValue(subtypeValue);
        }
    }
    catch (sepe) {
    }
    finally {
        _suppressClaimScriptPopup = false;
    }
}
_SetPrimaryTypeSubtype = SetPrimaryTypeSubtype;


// ECC Call types to Sub call types mapping
var _eccCallTypeMappings = [
    ['General Inquiry', 'Other VA Benefits'],
    ['General Inquiry', 'VA Office Contact Information'],
    ['General Inquiry', 'Ebenefits Questions'],
    ['General Inquiry', 'Wave Questions'],
    ['General Inquiry', 'VONAPP Questions'],
    ['General Inquiry', 'GIBILL Website Questions'],
    ['General Inquiry', 'Wrong Department/Number'],
    ['General Inquiry', 'Homeless'],
    ['General Inquiry', 'Forms Request'],
    ['General Inquiry', 'How to Apply'],
    ['General Inquiry', 'How to change schools'],
    ['General Inquiry', 'How to report hours OTJ 22-6553d-1'],
    ['General Inquiry', 'How to transfer benefits to Dependents -TOE'],
    ['General Inquiry', 'How to Appeal'],
    ['General Inquiry', 'How to file a complaint'],
    ['General Inquiry', 'Other'],
    ['Claim Inquiry', 'Status of Enrollment Certification'],
    ['Claim Inquiry', 'VONAPP'],
    ['Claim Inquiry', 'Correspondence Inquiry'],
    ['Claim Inquiry', 'Processing Time'],
    ['Claim Inquiry', 'Relinquish/Election Date'],
    ['Claim Inquiry', 'Application Inquiry'],
    ['Claim Inquiry', 'Percent of Eligibility'],
    ['Claim Inquiry', 'Remaining Entitlement'],
    ['Claim Inquiry', 'Delimiting Date'],
    ['Claim Inquiry', 'Notification of Withdrawal/Drop'],
    ['Claim Inquiry', 'Advance Pay'],
    ['Claim Inquiry', 'Audit'],
    ['Claim Inquiry', 'Kickers - Buy ups'],
    ['Claim Inquiry', 'Extension Request of Delimiting Date'],
    ['Claim Inquiry', 'Extension Request of Entitlement'],
    ['Claim Inquiry', 'Other'],
    ['Monthly Certification', 'Confirmation of self certification on IVR'],
    ['Monthly Certification', 'Confirmation of self certification on WAVE'],
    ['Monthly Certification', 'Request to certify'],
    ['Update Information', 'Direct Deposit - Set up, Change, Cancel'],
    ['Update Information', 'Address Change'],
    ['Update Information', 'Email Change'],
    ['Update Information', 'Phone Number Change'],
    ['Update Information', 'Name Change'],
    ['Update Information', 'Status Change - No longer Active Duty'],
    ['Payment Inquiry', 'Next payment date/Amount'],
    ['Payment Inquiry', 'Explanation of Amount'],
    ['Payment Inquiry', 'Missing Payment'],
    ['Payment Inquiry', 'Duplicate Payment'],
    ['Payment Inquiry', 'Return Payment'],
    ['Payment Inquiry', 'Status of Refund/Reissued Payment'],
    ['Payment Inquiry', 'Advance Payment/Accelerated Payment'],
    ['Debt Inquiry', 'Explanation'],
    ['Debt Inquiry', 'Status - Existing Waiver Request'],
    ['Debt Inquiry', 'Payment Arrangements'],
    ['Debt Inquiry', 'Dispute/Request Waiver Request'],
    ['Debt Inquiry', 'Transfer Debt from School/Student'],
    ['Debt Inquiry', 'School Debt Recoup'],
    ['Debt Inquiry', 'Refund Status'],
    ['Debt Inquiry', 'Refund Request'],
    ['Debt Inquiry', 'Other'],
    ['Letters/Documents Request', 'Copy of COE'],
    ['Letters/Documents Request', 'Financial Aid Letter'],
    ['Letters/Documents Request', 'Exhaust Entitlement Letter'],
    ['Letters/Documents Request', 'Hazelwood Letter'],
    ['Letters/Documents Request', 'Copy of Award Ltr/Debt Ltr/Relinq Ltr'],
    ['Letters/Documents Request', 'Copy of VONAPP'],
    ['Letters/Documents Request', 'Copy of DD214'],
    ['Letters/Documents Request', 'Other'],
    ['Major Event - Call Tracker', 'Hurricane'],
    ['Major Event - Call Tracker', 'Billing Payment Failure'],
    ['Major Event - Call Tracker', 'Other'],
    ['Emergency/Priority', 'Suicidal Caller'],
    ['Emergency/Priority', 'Bomb/Violence Threat'],
    ['Emergency/Priority', 'Other'],
    ['Other', 'Other']
];

var _optionSetUtility = (function () {

    function generateSubOption(option, subOptions, mappings) {
        var i, option, filteredMappings, newSubOptions = [];

        filteredMappings = $.grep(mappings, function (e) {
            return e[0] === option;
        });

        for (i = 0; i < filteredMappings.length; i++) {
            option = null;
            option = $.grep(subOptions, function (e) {
                return filteredMappings[i][1] === e.text;
            });

            if (option && option.length > 0)
                newSubOptions.push({ text: option[0].text, value: option[0].value });
        }

        return newSubOptions;
    }

    function loadOptionSet(control, options) {
        control.clearOptions();

        for (var i = 0; i < options.length; i++) {
            control.addOption({ text: options[i].text, value: options[i].value });
        }
    }

    return {
        getSubOptionsFromOption: generateSubOption,
        loadOptionSet: loadOptionSet
    };

})();