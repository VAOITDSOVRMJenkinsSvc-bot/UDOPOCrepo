"use strict";

var exCon = null;
var formContext = null;
var globalContext = Xrm.Utility.getGlobalContext();
var version = globalContext.getVersion();
var lib;
var webApi;
var formHelper;

parent.clearschoolinfo = clearschoolinfo;
parent.clearprevschoolinfo = clearprevschoolinfo;
parent.findschool = findschool;
parent.findprevschool = findprevschool;

function OnLoad(exCon) {
    instantiateCommonScripts(exCon);
    formContext = exCon.getFormContext();

    var wrControlSchoolButtons = formContext.getControl("WebResource_udo_Dependent_Find_School");
    if (wrControlSchoolButtons !== 'undefined' && wrControlSchoolButtons !== null) {
        wrControlSchoolButtons.getContentWindow().then(
            function (contentWindow) {
                contentWindow.setContext(exCon);
            }
        )
    }

    var wrControlClearSchoolButtons = formContext.getControl("WebResource_udo_Dependent_Clear_School");
    if (wrControlClearSchoolButtons !== 'undefined' && wrControlClearSchoolButtons !== null) {
        wrControlClearSchoolButtons.getContentWindow().then(
            function (contentWindow) {
                contentWindow.setContext(exCon);
            }
        )
    }

    var wrControlSchoolButtons = formContext.getControl("WebResource_udo_Dependent_Find_Prev_School");
    if (wrControlSchoolButtons !== 'undefined' && wrControlSchoolButtons !== null) {
        wrControlSchoolButtons.getContentWindow().then(
            function (contentWindow) {
                contentWindow.setContext(exCon);
            }
        )
    }

    var wrControlClearSchoolButtons = formContext.getControl("WebResource_udo_Dependent_Clear_Prev_School");
    if (wrControlClearSchoolButtons !== 'undefined' && wrControlClearSchoolButtons !== null) {
        wrControlClearSchoolButtons.getContentWindow().then(
            function (contentWindow) {
                contentWindow.setContext(exCon);
            }
        )
    }

    // Make sure to show correct tab when being executed by plugin from Dependent Maintenance
    showDependentTabs(exCon);

    formContext.getControl("crme_dependentrelationship").setFocus();

    if (formHelper.getValue("crme_legacyrecord") === true) {
        if (formHelper.getValue("crme_dependentrelationship") === 935950000) {
            formContext.getAttribute("crme_maintenancetype").setValue(752280000);
            formContext.ui.controls.get("crme_firstname").setDisabled(true);
            formContext.ui.controls.get("crme_middlename").setDisabled(true);
            formContext.ui.controls.get("crme_lastname").setDisabled(true);
            formContext.ui.controls.get("crme_dob").setDisabled(true);
            var countryName = 'USA';
            new Promise(function (resolve, reject) {
                Xrm.WebApi.online.retrieveMultipleRecords("crme_countrylookup", "?$select=crme_countrylookupid&$filter=crme_country eq '" + countryName + "'").then(
                    function success(data) {
                        if (data.entities.length > 0) {
                            var lookup = new Array();
                            lookup[0] = new Object();
                            lookup[0].id = data.entities[0].crme_countrylookupid;
                            lookup[0].name = countryName;
                            lookup[0].entityType = 'crme_countrylookup';
                            formContext.getAttribute("crme_childplaceofbirthcountryid").setValue(lookup);
                        } else {
                        }
                        return resolve();
                    },
                    function (error) {
                        return reject();
                    });
                return resolve();
            });
        }

        if (formHelper.getValue("crme_dependentrelationship") === 935950001) {
            if (formHelper.getValue("crme_awardind") === "Y") {
                formHelper.getAttribute("crme_maintenancetype").setValue(935950001);
                var depID = formContext.data.entity.getId().replace("{", "").replace("}", "");
                formContext.ui.controls.get("crme_firstname").setDisabled(true);
                formContext.ui.controls.get("crme_middlename").setDisabled(true);
                formContext.ui.controls.get("crme_lastname").setDisabled(true);
                formContext.ui.controls.get("crme_dob").setDisabled(true);
                formContext.getAttribute("udo_spousedetails").setValue(752280001);
                formContext.ui.controls.get("udo_spousedetails").setDisabled(true);
                spouseDetailsOnChange(exCon);
                formContext.getAttribute("udo_howwasmarriageterminated").setRequiredLevel("required");
                formContext.getAttribute("udo_marriageenddate").setRequiredLevel("required");
                formContext.getAttribute("udo_marriageendcity").setRequiredLevel("required");
                formContext.getAttribute("udo_marriageendstate").setRequiredLevel("required");
                formContext.getAttribute("udo_marriageendcountry").setRequiredLevel("required");
            }
            else {
                Xrm.Navigation.openAlertDialog({
                    text: "No edits are allowed to this spouse because the dependent is not on the award."
                }).then(function success(result) {
                    var windowtoOpen = "http://event/?eventName=CloseDependent";
                    window.open(windowtoOpen);
                });
            }
        }
    }

    if (formHelper.getValue("crme_dependentrelationship") === 935950000) {
        // Set the form correctly if is a School Age Child in School
        // Set fields used on DocGen 674 for dependent "edits"
        schoolAgedChildInSchoolChange();
        schoolInfoManual();
        prevschoolInfo();
        attendedLastSchool();
        syncState_MarriageFields();
        setStepChildFields();
        setSignatureFields("form load");
        setPaidByDEAFields();
    }
}

function instantiateCommonScripts(exCon) {
    lib = new CrmCommonJS.CrmCommon(version, exCon);
    webApi = lib.WebApi;
    formHelper = new CrmCommonJS.FormHelper(exCon);
}

function showDependentTabs(exCon) {
    var depType = formHelper.getValue("crme_dependentrelationship");

    // Child: 935,950,000; Spouse: 935,950,001
    switch (depType) {
        case 935950000:
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildPlaceOfBirth").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildInformation").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildCurrentAddress").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildLivingArrangements").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildAddress").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildZipCity").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildStateCountry").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_TuitionPaidByGovt").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SchoolInfoManual").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SearchSchool").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SchoolInfo").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_StudentInfo").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_PrevSchoolInfo").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_PrevSchoolInfoManual").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_Remarks").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_3_section_3").setVisible(false);

            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseInformation").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseMailingAddress").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseAddress").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseZipCity").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseStateCountry").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseDetails").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_MarriageDatePlaceTerminated").setVisible(false);

            dependentRelationshipOnChange(exCon);
            schoolAgedChildInSchoolChange();
            break;
        case 935950001:
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildPlaceOfBirth").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildInformation").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildCurrentAddress").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildLivingArrangements").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildAddress").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildZipCity").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildStateCountry").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_TuitionPaidByGovt").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SchoolInfoManual").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SearchSchool").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SchoolInfo").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_StudentInfo").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_PrevSchoolInfo").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_PrevSchoolInfoManual").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ReportOfIncome").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ValueOfEstate").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_Remarks").setVisible(false);

            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseInformation").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseMailingAddress").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseAddress").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseZipCity").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseStateCountry").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseDetails").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_MarriageDatePlaceTerminated").setVisible(true);

            dependentRelationshipOnChange(exCon);
            schoolAgedChildInSchoolChange();
            break;
        default:
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildPlaceOfBirth").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildInformation").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildCurrentAddress").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildLivingArrangements").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildAddress").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildZipCity").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ChildStateCountry").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_TuitionPaidByGovt").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SchoolInfoManual").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SearchSchool").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SchoolInfo").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_StudentInfo").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_PrevSchoolInfo").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_PrevSchoolInfoManual").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ReportOfIncome").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ValueOfEstate").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_Remarks").setVisible(false);

            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseInformation").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseMailingAddress").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseAddress").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseZipCity").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseStateCountry").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SpouseDetails").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_MarriageDatePlaceTerminated").setVisible(false);

            dependentRelationshipOnChange(exCon);
            schoolAgedChildInSchoolChange();
            break;
    }
}

// Monitor for a change in Age and Attendance of Dependent. Impacts VA FORM 21-674b, APR 2015
function schoolAgedChildInSchoolChange() {
    var isSchoolAgeChildInSchool = formHelper.getValue("crme_childage1823inschool");
    console.log(isSchoolAgeChildInSchool);

    if (isSchoolAgeChildInSchool === true) {
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_TuitionPaidByGovt").setVisible(true);
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SchoolInfoManual").setVisible(true);
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SearchSchool").setVisible(true);
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SchoolInfo").setVisible(true);
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_StudentInfo").setVisible(true);
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_PrevSchoolInfo").setVisible(true);
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_PrevSchoolInfoManual").setVisible(true);
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_Remarks").setVisible(true);

        setSignatureFields("child in school field change"); // Set 674 document signature fields
    }
    else {
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_TuitionPaidByGovt").setVisible(false);
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SchoolInfoManual").setVisible(false);
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SearchSchool").setVisible(false);
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SchoolInfo").setVisible(false);
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_StudentInfo").setVisible(false);
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_PrevSchoolInfo").setVisible(false);
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_PrevSchoolInfoManual").setVisible(false);
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ReportOfIncome").setVisible(false);
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ValueOfEstate").setVisible(false);
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_Remarks").setVisible(false);
    }
}

// Called by Marital Status change of student
function syncState_MarriageFields() {
    var isMarried = formHelper.getValue("udo_child_married");
    var wasMarried = formHelper.getValue("crme_childpreviouslymarried");
    if (isMarried === true || wasMarried === true) {
        formHelper.setRequiredLevel("udo_dateofmarriage", true);
    }
    else {
        formHelper.setRequiredLevel("udo_dateofmarriage", false);
    }

    // Used Yes No Option Set due to issue with Boolean in DocGen
    if (formHelper.getValue("crme_childpreviouslymarried") !== null) {
        var wasChildEverMarried = formHelper.getValue("crme_childpreviouslymarried");
        if (wasChildEverMarried) {
            formHelper.setValue("udo_cbischildmarriedreportyesno", 752280000); // Yes
        }
        else {
            formHelper.setValue("udo_cbischildmarriedreportyesno", 752280001); // No
        }
    }
}

function clearschoolinfo() {
    formHelper.getAttribute("udo_schoolname").setValue(null);
    formHelper.getAttribute("udo_schooladdressline1").setValue(null);
    formHelper.getAttribute("udo_schooladdressline2").setValue(null);
    formHelper.getAttribute("udo_schooladdressline3").setValue(null);
    formHelper.getAttribute("udo_schooladdresscity").setValue(null);
    formHelper.getAttribute("udo_schooladdstate").setValue(null);
    formHelper.getAttribute("udo_schooladdresszip").setValue(null);
}

function clearprevschoolinfo() {
    formHelper.getAttribute("udo_attendedschool").setValue(null);
    formHelper.getAttribute("udo_attendedschooladdress1").setValue(null);
    formHelper.getAttribute("udo_attendedschooladdress2").setValue(null);
    formHelper.getAttribute("udo_attendedschooladdress3").setValue(null);
    formHelper.getAttribute("udo_attendedschoolcity").setValue(null);
    formHelper.getAttribute("udo_attendedschstate").setValue(null);
    formHelper.getAttribute("udo_attendedschoolzip").setValue(null);
}

function findprevschool() {
    var state = "";
    var schoolName = formContext.getAttribute("udo_partialprevschoolname").getValue();
    var facilityID = formContext.getAttribute("udo_searchprevfacilitycode").getValue();

    if (formContext.getAttribute("udo_prevstateprovinceid").getValue() !== null) {
        state = formContext.getAttribute("udo_prevstateprovinceid").getValue()[0].name;
    }


    if ((schoolName === null) && (facilityID === null)) {
        formContext.getControl("udo_partialprevschoolname").setNotification("Please enter Partial/Complete School Name OR Facility ID to perform the search", "PartialSchoolName");
        return;
    }
    else {
        formContext.getControl("udo_partialprevschoolname").clearNotification("PartialSchoolName");
    }

    if (state === "") {
        state = " ";
    }

    if (facilityID === null) {
        facilityID = " ";
    }

    console.log("Finding Prev School");
    ActionFindSchool(schoolName, state, facilityID)
        .then(function (result) {
            if (result.ok) {
                result.json().then(function (response) {
                    Xrm.Utility.closeProgressIndicator();

                    var schoolText = response.SchoolText || " ";
                    var SchoolCode = response.SchoolCode;
                    var SchoolName = response.SchoolName;
                    var AddressLine1 = response.SchoolAddressLine1;
                    var AddressLine2 = response.SchoolAddressLine2;
                    var AddressLine3 = response.SchoolAddressLine3;
                    var SchoolCity = response.SchoolCity;
                    var SchoolStateName = response.SchoolStateName;
                    var SchoolStateID = response.SchoolStateID;
                    var SchoolZip = response.SchoolZip;

                    if (schoolText === " " && SchoolStateName !== " ") // this means unique school was returned.
                    {
                        formContext.getAttribute("udo_prevschoolinfomanual").setValue(false);

                        formContext.getAttribute("udo_prevfacilitycode").setValue(SchoolCode);

                        formContext.getAttribute("udo_attendedschool").setValue(SchoolName);
                        formContext.getAttribute("udo_attendedschooladdress1").setValue(AddressLine1);
                        formContext.getAttribute("udo_attendedschooladdress2").setValue(AddressLine2);
                        formContext.getAttribute("udo_attendedschooladdress3").setValue(AddressLine3);
                        formContext.getAttribute("udo_attendedschoolcity").setValue(SchoolCity);
                        var schoolStateGuid = "{" + response.SchoolStateID + "}";
                        var statelookupValue = new Array();
                        statelookupValue[0] = new Object();
                        statelookupValue[0].id = schoolStateGuid;
                        statelookupValue[0].name = SchoolStateName; // Name of the lookup
                        statelookupValue[0].entityType = "crme_stateorprovincelookup"; // Entity Type of the lookup entity
                        formContext.getAttribute("udo_attendedschstate").setValue(statelookupValue);
                        formContext.getAttribute("udo_attendedschoolzip").setValue(SchoolZip);
                        prevschoolInfo();
                        // Set UI field values as this means you have the school.
                    }
                    if(schoolText === " " && SchoolStateName === " ")
                    {
                        var msg = "\n\r" + "Error retrieving School Information. Please enter the school information manually. ";
                        var title = "Matching Schools";
                        var alertOptions = { height: 150, width: 300 };
                        var alertStrings = { title: title, text: msg, confirmButtonLabel: "OK" };
                        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                            function success(result) {
                                console.log("Alert dialog closed");
                            },
                            function (error) {
                                console.log(error.message);
                            }
                        );
                    }
                    if (schoolText !== " ") {
                        var msg = "\n\r" + schoolText;
                        var title = "Matching Schools";
                        var alertOptions = { height: 300, width: 600 };
                        var alertStrings = { title: title, text: msg, confirmButtonLabel: "OK" };
                        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                            function success(result) {
                                console.log("Alert dialog closed");
                            },
                            function (error) {
                                console.log(error.message);
                            }
                        );
                    }
                });
            }
        }, function (error) {
            Xrm.Utility.closeProgressIndicator();
        });
}

function findschool() {
    var state = "";
    var schoolName = formContext.getAttribute("udo_partialschoolname").getValue();
    var facilityID = formContext.getAttribute("udo_searchfacilitycode").getValue();

    if (formContext.getAttribute("udo_stateprovinceid").getValue() !== null) {
        state = formContext.getAttribute("udo_stateprovinceid").getValue()[0].name;
    }

    if ((schoolName === null) && (facilityID === null)) {
        formContext.getControl("udo_partialschoolname").setNotification("Please enter Partial/Complete School Name OR Facility ID to perform the search", "PartialSchoolName");

        return;
    }
    else {
        formContext.getControl("udo_partialschoolname").clearNotification("PartialSchoolName");
    }

    if (state === "") {
        state = " ";
    }

    if (facilityID === null) {
        facilityID = " ";
    }

    console.log("Finding School");
    ActionFindSchool(schoolName, state, facilityID)
        .then(function (result) {
            if (result.ok) {
                result.json().then(function (response) {
                    Xrm.Utility.closeProgressIndicator();

                    var schoolText = response.SchoolText || " ";
                    var SchoolCode = response.SchoolCode;
                    var SchoolName = response.SchoolName;
                    var AddressLine1 = response.SchoolAddressLine1;
                    var AddressLine2 = response.SchoolAddressLine2;
                    var AddressLine3 = response.SchoolAddressLine3;
                    var SchoolCity = response.SchoolCity;
                    var SchoolStateName = response.SchoolStateName;
                    var SchoolStateID = response.SchoolStateID;
                    var SchoolZip = response.SchoolZip;
                  
                    if (schoolText === " " && SchoolStateName !== " ") // this means unique school was returned.
                    {
                        formContext.getAttribute("udo_schoolinfomanual").setValue(false);

                        formContext.getAttribute("udo_facilitycode").setValue(SchoolCode);

                        formContext.getAttribute("udo_schoolname").setValue(SchoolName);
                        formContext.getAttribute("udo_schooladdressline1").setValue(AddressLine1);
                        formContext.getAttribute("udo_schooladdressline2").setValue(AddressLine2);
                        formContext.getAttribute("udo_schooladdressline3").setValue(AddressLine3);
                        formContext.getAttribute("udo_schooladdresscity").setValue(SchoolCity);

                        var schoolStateGuid = "{" + response.SchoolStateID + "}";
                        var statelookupValue = new Array();
                        statelookupValue[0] = new Object();
                        statelookupValue[0].id = schoolStateGuid;
                        statelookupValue[0].name = SchoolStateName; // Name of the lookup
                        statelookupValue[0].entityType = "crme_stateorprovincelookup"; // Entity Type of the lookup entity
                        formContext.getAttribute("udo_schooladdstate").setValue(statelookupValue);
                        formContext.getAttribute("udo_schooladdresszip").setValue(SchoolZip);
                        schoolInfoManual();
                        // Set UI field values as this means you have the school.
                    }
                    if (schoolText === " " && SchoolStateName === " ")
                    {
                        var msg = "\n\r" + "Error retrieving School Information. Please enter the school information manually. ";
                        var title = "Matching Schools";
                        var alertOptions = { height: 150, width: 300 };
                        var alertStrings = { title: title, text: msg, confirmButtonLabel: "OK" };
                        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                            function success(result) {
                                console.log("Alert dialog closed");
                            },
                            function (error) {
                                console.log(error.message);
                            }
                        );
                    }
                    if (schoolText !== " ") {
                        var msg = "\n\r" + schoolText;
                        var title = "Matching Schools";
                        var alertOptions = { height: 300, width: 600 };
                        var alertStrings = { title: title, text: msg, confirmButtonLabel: "OK" };
                        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                            function success(result) {
                                console.log("Alert dialog closed");
                            },
                            function (error) {
                                console.log(error.message);
                            }
                        );
                    }
                });
            }
        }, function (error) {
            Xrm.Utility.closeProgressIndicator();
        });
}

function ActionFindSchool(schoolName, state, facilityID) {
    var parameters = {};
    var entity = {};

    entity.id = formContext.getAttribute("crme_dependentmaintenance").getValue()[0].id.replace("{", "").replace("}", "");
    entity.entityType = "crme_dependentmaintenance";
    parameters.entity = entity;
    parameters.SchoolName = schoolName;
    parameters.State = state;
    parameters.FacilityID = facilityID;

    var udo_FindSchoolRequest = {
        ParentEntityReference: parameters.entity,
        PartialSchoolName: parameters.SchoolName,
        State: parameters.State,
        FacilityID: parameters.FacilityID,

        getMetadata: function () {
            return {
                boundParameter: null,
                parameterTypes: {
                    "ParentEntityReference": {
                        "typeName": "mscrm.crmbaseentity",
                        "structuralProperty": 5
                    },
                    "PartialSchoolName": {
                        "typeName": "Edm.String",
                        "structuralProperty": 1
                    },
                    "State": {
                        "typeName": "Edm.String",
                        "structuralProperty": 1
                    },
                    "FacilityID": {
                        "typeName": "Edm.String",
                        "structuralProperty": 1
                    }
                },
                operationType: 0,
                operationName: "udo_FindSchool"
            };
        }
    };

    Xrm.Utility.showProgressIndicator("Finding School... Please wait");
    return Xrm.WebApi.online.execute(udo_FindSchoolRequest);
}

function schoolInfoManual() {
    var schoolInfoManualValue = formContext.getAttribute("udo_schoolinfomanual").getValue();

    if (schoolInfoManualValue === false) {
        formContext.getControl("udo_partialschoolname").setVisible(true);
        formContext.getControl("udo_stateprovinceid").setVisible(true);
        formContext.getControl("udo_searchfacilitycode").setVisible(true);
        formContext.getControl("WebResource_udo_Dependent_Find_School").setVisible(true);
        formContext.getControl("WebResource_udo_Dependent_Clear_School").setVisible(true);
        formContext.ui.controls.get("udo_schoolname").clearNotification();
        formContext.ui.controls.get("udo_schooladdressline1").clearNotification();
        formContext.ui.controls.get("udo_schooladdresscity").clearNotification();
        formContext.ui.controls.get("udo_schooladdstate").clearNotification();
        formContext.ui.controls.get("udo_schooladdresszip").clearNotification();
        formContext.ui.controls.get("udo_schoolname").setDisabled(true);
        formContext.ui.controls.get("udo_schooladdressline1").setDisabled(true);
        formContext.ui.controls.get("udo_schooladdressline2").setDisabled(true);
        formContext.ui.controls.get("udo_schooladdressline3").setDisabled(true);
        formContext.ui.controls.get("udo_schooladdresscity").setDisabled(true);
        formContext.ui.controls.get("udo_schooladdstate").setDisabled(true);
        formContext.ui.controls.get("udo_schooladdresszip").setDisabled(true);
        formContext.getAttribute("udo_schoolname").setRequiredLevel("required");
    }
    else {
        formContext.getAttribute("udo_facilitycode").setValue(null);
        formContext.getControl("udo_partialschoolname").setVisible(false);
        formContext.getControl("udo_stateprovinceid").setVisible(false);
        formContext.getControl("udo_searchfacilitycode").setVisible(false);
        formContext.getControl("WebResource_udo_Dependent_Find_School").setVisible(false);
        formContext.getControl("WebResource_udo_Dependent_Clear_School").setVisible(false);
        formContext.ui.controls.get("udo_schoolname").clearNotification();
        formContext.ui.controls.get("udo_schooladdressline1").clearNotification();
        formContext.ui.controls.get("udo_schooladdresscity").clearNotification();
        formContext.ui.controls.get("udo_schooladdstate").clearNotification();
        formContext.ui.controls.get("udo_schooladdresszip").clearNotification();
        formContext.ui.controls.get("udo_schoolname").setDisabled(false);
        formContext.ui.controls.get("udo_schooladdressline1").setDisabled(false);
        formContext.ui.controls.get("udo_schooladdressline2").setDisabled(false);
        formContext.ui.controls.get("udo_schooladdressline3").setDisabled(false);
        formContext.ui.controls.get("udo_schooladdresscity").setDisabled(false);
        formContext.ui.controls.get("udo_schooladdstate").setDisabled(false);
        formContext.ui.controls.get("udo_schooladdresszip").setDisabled(false);
    }
}

function findDepInfo() {
    var parameters = {};
    var entity = {};
    entity.id = formContext.data.entity.getId().replace("{", "").replace("}", "");
    entity.entityType = "crme_dependent";
    parameters.entity = entity;

    var udo_findDepRequest = {
        ParentEntityReference: parameters.entity,

        getMetadata: function () {
            return {
                parameterTypes: {
                    "ParentEntityReference": {
                        "typeName": "mscrm.crmbaseentity",
                        "structuralProperty": 5
                    }
                },
                operationType: 0,
                operationName: "udo_DependentInfo"
            };
        }
    };

    return Xrm.WebApi.online.execute(udo_findDepRequest);
}
function prevschoolInfo() {
    var prevschoolInfoManualValue = formContext.getAttribute("udo_prevschoolinfomanual").getValue();

    // Value is no
    if (prevschoolInfoManualValue === false) {
        formContext.getControl("udo_partialprevschoolname").setVisible(true);
        formContext.getControl("udo_prevstateprovinceid").setVisible(true);
        formContext.getControl("udo_searchprevfacilitycode").setVisible(true);
        formContext.getControl("WebResource_udo_Dependent_Find_Prev_School").setVisible(true);
        formContext.getControl("WebResource_udo_Dependent_Clear_Prev_School").setVisible(true);
        formContext.ui.controls.get("udo_attendedschool").clearNotification();
        formContext.ui.controls.get("udo_attendedschooladdress1").clearNotification();
        formContext.ui.controls.get("udo_attendedschoolcity").clearNotification();
        formContext.ui.controls.get("udo_attendedschstate").clearNotification();
        formContext.ui.controls.get("udo_attendedschoolzip").clearNotification();
        formContext.ui.controls.get("udo_attendedschool").setDisabled(true);
        formContext.ui.controls.get("udo_attendedschooladdress1").setDisabled(true);
        formContext.ui.controls.get("udo_attendedschooladdress2").setDisabled(true);
        formContext.ui.controls.get("udo_attendedschooladdress3").setDisabled(true);
        formContext.ui.controls.get("udo_attendedschoolcity").setDisabled(true);
        formContext.ui.controls.get("udo_attendedschstate").setDisabled(true);
        formContext.ui.controls.get("udo_attendedschoolzip").setDisabled(true);
        formContext.ui.controls.get("udo_attendedsessionsperweek").setDisabled(false);
        formContext.ui.controls.get("udo_attendedhoursperweek").setDisabled(false);
        formContext.ui.controls.get("udo_attendedbegindate").setDisabled(false);
        formContext.ui.controls.get("udo_attendedenddate").setDisabled(false);
    }
    else {
        formContext.getAttribute("udo_prevfacilitycode").setValue(null);
        formContext.getControl("udo_partialprevschoolname").setVisible(false);
        formContext.getControl("udo_prevstateprovinceid").setVisible(false);
        formContext.getControl("udo_searchprevfacilitycode").setVisible(false);
        formContext.getControl("WebResource_udo_Dependent_Find_Prev_School").setVisible(false);
        formContext.getControl("WebResource_udo_Dependent_Clear_Prev_School").setVisible(false);
        formContext.ui.controls.get("udo_attendedschool").clearNotification();
        formContext.ui.controls.get("udo_attendedschooladdress1").clearNotification();
        formContext.ui.controls.get("udo_attendedschoolcity").clearNotification();
        formContext.ui.controls.get("udo_attendedschstate").clearNotification();
        formContext.ui.controls.get("udo_attendedschoolzip").clearNotification();
        formContext.ui.controls.get("udo_attendedschool").setDisabled(false);
        formContext.ui.controls.get("udo_attendedschooladdress1").setDisabled(false);
        formContext.ui.controls.get("udo_attendedschooladdress2").setDisabled(false);
        formContext.ui.controls.get("udo_attendedschooladdress3").setDisabled(false);
        formContext.ui.controls.get("udo_attendedschoolcity").setDisabled(false);
        formContext.ui.controls.get("udo_attendedschstate").setDisabled(false);
        formContext.ui.controls.get("udo_attendedschoolzip").setDisabled(false);
        formContext.ui.controls.get("udo_attendedsessionsperweek").setDisabled(false);
        formContext.ui.controls.get("udo_attendedhoursperweek").setDisabled(false);
        formContext.ui.controls.get("udo_attendedbegindate").setDisabled(false);
        formContext.ui.controls.get("udo_attendedenddate").setDisabled(false);
    }
}

function attendedLastSchool() {
    var attendedLastSchool = formContext.getAttribute("udo_attendedlastterm").getValue();
    if (attendedLastSchool === false) {
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_PrevSchoolInfo").setVisible(false);
        formContext.getControl("udo_partialprevschoolname").setVisible(false);
        formContext.getControl("udo_prevstateprovinceid").setVisible(false);
        formContext.getControl("udo_searchprevfacilitycode").setVisible(false);
        formContext.getControl("WebResource_udo_Dependent_Find_Prev_School").setVisible(false);
        formContext.getControl("WebResource_udo_Dependent_Clear_Prev_School").setVisible(false);
    }
    else {
        formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_PrevSchoolInfo").setVisible(true);
        formContext.getControl("udo_partialprevschoolname").setVisible(true);
        formContext.getControl("udo_prevstateprovinceid").setVisible(true);
        formContext.getControl("udo_searchprevfacilitycode").setVisible(true);
        formContext.getControl("WebResource_udo_Dependent_Find_Prev_School").setVisible(true);
        formContext.getControl("WebResource_udo_Dependent_Clear_Prev_School").setVisible(true);
    }
}

function saveAndClose(executionContext) {
    console.log("Performing save and close event on dependent form...");
    if (formContext === undefined || formContext === null) {
        formContext = executionContext.getFormContext();
    }
    isSaveCloseClicked = true;
    formContext.data.save().then(

        function Success() {
            // USD Event CloseDependentForm will get called from within udo_DependentOnSave.js
            btn_saveAndCloseClicked();

            console.log("Success on Save");
        },
        function Error() {
            console.log("Error on Save");
        });
}

function setStepChildFields() {
    var childRelationship = formHelper.getValue("crme_childrelationship");

    // Step Child
    if (childRelationship === 935950001) {
        formHelper.setFieldVisible("udo_isstepchildbiologicalchildofspouse", true);

        var isStepChild = formHelper.getValue("udo_isstepchildbiologicalchildofspouse");

        if (isStepChild === 1) {
            formHelper.setFieldVisible("udo_datestepchildenteredvethousehold", true);
        }
        else {
            formHelper.setFieldVisible("udo_datestepchildenteredvethousehold", false);
            formHelper.setValue("udo_datestepchildenteredvethousehold", null);
        }
    }
    else {
        formHelper.setFieldVisible("udo_isstepchildbiologicalchildofspouse", false);
        formHelper.setValue("udo_isstepchildbiologicalchildofspouse", false);

        formHelper.setFieldVisible("udo_datestepchildenteredvethousehold", false);
        formHelper.setValue("udo_datestepchildenteredvethousehold", null);
    }
}

function setPaidByDEAFields() {
    if (formHelper.getValue("udo_paidbydea") !== null) {
        var paidByDEA = formHelper.getValue("udo_paidbydea");
        if (paidByDEA) {
            formHelper.setValue("udo_cbtuitionpaidbygovernmentreportyesno", 752280000); // Yes
        }
        else {
            formHelper.setValue("udo_cbtuitionpaidbygovernmentreportyesno", 752280001); // No
            formHelper.setValue("udo_agencyname", "");
            formHelper.setValue("udo_datepaymentsbegan", null);
        }
    }
    else {
        formHelper.setValue("udo_cbtuitionpaidbygovernmentreportyesno", 752280001); // No
        formHelper.setValue("udo_agencyname", "");
        formHelper.setValue("udo_datepaymentsbegan", null);
    }
}