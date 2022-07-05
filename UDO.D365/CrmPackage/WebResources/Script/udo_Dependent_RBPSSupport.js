var executionContext = null;
var formContext = null;
var globalContext = Xrm.Utility.getGlobalContext();
var version = globalContext.getVersion();
var lib;
var webApi;
var formHelper;
var isSaveCloseClicked = false;
var validationPopUp = false;


function showHideFields(exCon) {
    console.log("RBPS support function");
    executionContext = exCon;
    instantiateCommonScripts(exCon);

    if (formContext == undefined || formContext == null) {
        formContext = exCon.getFormContext();
    }
	
    var depType = formHelper.getValue("crme_dependentrelationship");
    var isSchoolAgeChildInSchool = formHelper.getValue("crme_childage1823inschool");
    if ((depType === 935950000) && (isSchoolAgeChildInSchool === true)){
    formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ReportOfIncome").setVisible(true);
    formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ValueOfEstate").setVisible(true);
}
  
}
