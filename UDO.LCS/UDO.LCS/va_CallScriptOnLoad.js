﻿/// <reference path="XrmPage-vsdoc.js" />
_issueMap = null;

function OnLoad() {
// CSDev Left Intentionally Blank 
}

function RefreshIssue() {
    updateDispositionSubType(Xrm.Page.ui.controls.get('va_issue'), Xrm.Page.ui.controls.get('va_subissue'), 
        'va_issue', 'va_subissue');
}

function RequiresSR() {
// CSDev Left Intentionally Blank 
}

//=============================================
// START FUNCTION updateDispositionSubType()
//=============================================
//function updateDispositionSubType() {
function updateDispositionSubType(dispositionControl, subtypeControl, 
    dispositionControlName, subtypeControlName) {
  // CSDev Left Intentionally Blank 
}
// END FUNCTION updateDispositionSubType()
//=============================================