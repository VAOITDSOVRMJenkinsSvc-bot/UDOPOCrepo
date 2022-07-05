﻿/// <reference path="XrmPage-vsdoc.js" />
_issueMap = null;
_exCon = null;
_formContext = null;

function OnLoad(execCon) {
    _exCon = execCon;
    _formContext = _exCon.getFormContext();
    Xrm.Page.data.entity.attributes.get('va_issue').addOnChange(RefreshIssue);
    Xrm.Page.data.entity.attributes.get('va_requiresservicerequest').addOnChange(RequiresSR);
    
    Xrm.Page.ui.controls.get('va_subissue').clearOptions();
    Xrm.Page.ui.controls.get('va_subissue').getAttribute().setRequiredLevel("none");
    //if form is being updated, refresh the sub-issue value and service request required level
    if (Xrm.Page.ui.getFormType() == 2) {
        var subId = Xrm.Page.getAttribute("va_subissue").getValue();
        RefreshIssue();
        if (subId) Xrm.Page.getAttribute("va_subissue").setValue(subId);
        RequiresSR();
    }
}

function RefreshIssue() {
    updateDispositionSubType(Xrm.Page.ui.controls.get('va_issue'), Xrm.Page.ui.controls.get('va_subissue'), 
        'va_issue', 'va_subissue');
}

function RequiresSR() {
    var req = (Xrm.Page.getAttribute("va_requiresservicerequest").getValue() == true);
    Xrm.Page.ui.controls.get('va_defaultsraction').getAttribute().setRequiredLevel(req ? "required" : "recommended");
}

//=============================================
// START FUNCTION updateDispositionSubType()
//=============================================
//function updateDispositionSubType() {
function updateDispositionSubType(dispositionControl, subtypeControl, 
    dispositionControlName, subtypeControlName) {
    var disp = new Array();
    disp['va_issue'] = Xrm.Page.getAttribute("va_issue");
    //debugger
    var dispositionAttr = dispositionControl.getAttribute();
    var newValue = -1;
    if (disp[dispositionControlName].getValue()) newValue = parseInt(disp[dispositionControlName].getValue());
    
    var subtypeAttribute = subtypeControl.getAttribute();
    
    if (!_issueMap) {
        // load a collection that maps issue and subissue
        var subtypeOptions = subtypeControl.getAttribute().getOptions();
        var issueOptions = dispositionControl.getAttribute().getOptions();
        _issueMap = new Array();

        for (var i = 0; i < issueOptions.length; i++) {
            var startTag = issueOptions[i].text + ':';
            var subIssues = new Array();
            if (startTag.length > 1) {
                for (var k = 0; k < subtypeOptions.length; k++) {
                    if (subtypeOptions[k].text.indexOf(startTag) == 0) {
                        var option = subtypeOptions[k];
                        option.text = subtypeOptions[k].text.substring(startTag.length);
                        subIssues.push(option);
                    }
                }
                _issueMap[startTag] = subIssues;
            }
        }
    }

    if (newValue != null) {
        subtypeControl.setDisabled(false);
        //subtypeAttribute.setRequiredLevel("required");

        try {
            subtypeControl.clearOptions();
            
            // suboption starts with main option text + ':'
            if (disp[dispositionControlName]) {
                var startTag = disp[dispositionControlName].getText() + ':';
                var subIssuesForCurrentIssue = _issueMap[startTag];
                for (i = 0; i < subIssuesForCurrentIssue.length; i++) {
                    subtypeControl.addOption(subIssuesForCurrentIssue[i]);
                }
            }
        } catch (err) {
            alert(err.description);
        }
    } else {
        //SET TO NULL
        subtypeControl.clearOptions();
        subtypeControl.setDisabled(true);
        subtypeAttribute.setRequiredLevel("none");
        return;
    } 
}
// END FUNCTION updateDispositionSubType()
//=============================================