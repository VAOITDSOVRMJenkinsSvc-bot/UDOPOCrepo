"use strict";

var formContext = null;
var globalContext = Xrm.Utility.getGlobalContext();
var version = globalContext.getVersion();
var lib;
var webApi;
var formHelper;

function instantiateCommonScripts(exCon) {
    if (exCon === undefined) {
        exCon = null;
    }
    lib = new CrmCommonJS.CrmCommon(version, exCon);
    webApi = lib.WebApi;
    formHelper = new CrmCommonJS.FormHelper(exCon);
}

function countryChange(locationField, countryField, cityField, stateField) {
    var countryAttribute = formHelper.getValue(countryField);
    formHelper.setRequiredLevel(countryField, true);

    if (countryAttribute !== null && countryAttribute !== '') {
        var thisCountry = countryAttribute;
        if (thisCountry[0].name === "USA" || thisCountry[0].name === "US") {
            formHelper.setRequiredLevel(stateField, true); 
            formHelper.setRequiredLevel(cityField, true); 
        }
        else {
            formHelper.setRequiredLevel(stateField, false);
            formHelper.setRequiredLevel(cityField, false);
        }
    }

    setLocation(locationField, countryField, cityField, stateField);
}

function setLocation(locationField, countryField, cityField, stateField) {
    var countryAttribute = formHelper.getValue(countryField);
    if (countryAttribute !== null && countryAttribute !== '') {
        var thisCountry = countryAttribute;
        if (thisCountry[0].name === "USA" || thisCountry[0].name === "US") {
            var state = formHelper.getValue(stateField);
            var city = formHelper.getValue(cityField);
            var thisCity = "";
            var thisState = "";
            (city !== null && city !== '') ? thisCity = city.toUpperCase() : thisCity = "";
            (state !== null && state !== '') ? thisState = state[0].name.toUpperCase() : thisState = "";
            formHelper.setValue(locationField, thisCity + " " + thisState);
        } else {
            formHelper.setValue(locationField, thisCountry[0].name);
        }
    }
}

function marriageCountryChange(exCon) {
    // Location is a field for the 686 form.  Either is City and State for US, or Country.
    instantiateCommonScripts(exCon);
    countryChange("crme_marriagelocation", "crme_marriagecountryid", "crme_marriagecity", "crme_marriagestateid");
}

function marriageEndCountryChange(exCon) {
    // Marriage end location is a field for the 686 form.  Either is City and State for US, or Country.
    instantiateCommonScripts(exCon);
    countryChange("udo_marriageendlocation", "udo_marriageendcountry", "udo_marriageendcity", "udo_marriageendstate");
}

function setmarriageLocationOnChange(exCon) {
    // Location is a field for the 686 form.  Either is City and State for US, or Country.
    instantiateCommonScripts(exCon);
    setLocation("crme_marriagelocation", "crme_marriagecountryid", "crme_marriagecity", "crme_marriagestateid");
}

function setMarriageEndLocationOnChange(exCon) {
    // Marriage end location is a field for the 686 form.  Either is City and State for US, or Country.
    instantiateCommonScripts(exCon);
    setLocation("udo_marriageendlocation", "udo_marriagecountry", "udo_marriageendcity", "udo_marriageendstate");
}

function birthCountryChange(exCon) {
    // Location is a field for the 686 form.  Either is City and State for US, or Country.
    instantiateCommonScripts(exCon);
    countryChange("crme_birthlocation", "crme_childplaceofbirthcountryid", "crme_childplaceofbirthcity", "crme_childplaceofbirthstateid");
}
function setbirthLocationOnChange(exCon) {
    // Location is a field for the 686 form.  Either is City and State for US, or Country.
    instantiateCommonScripts(exCon);
    setLocation("crme_birthlocation", "crme_childplaceofbirthcountryid", "crme_childplaceofbirthcity", "crme_childplaceofbirthstateid");
}
function dependentRelationshipOnChange(exCon) {
    instantiateCommonScripts(exCon);
    var type = formHelper.getValue("crme_dependentrelationship");
    var currentdate = new Date();

    switch (type) {
        //  Child
        case 935950000:
            formHelper.setRequiredLevel("crme_dob", true);
            formHelper.setRequiredLevel("crme_childrelationship");
            formHelper.setRequiredLevel("crme_ssn", true); //made required as per 2475

            formHelper.setRequiredLevel("crme_marriagedate", false);
            formHelper.setRequiredLevel("crme_marriagecity", false);
            formHelper.setRequiredLevel("crme_marriagestateid", false);
            formHelper.setRequiredLevel("crme_marriagecountryid", false);

            //CSDev Force Required
            formHelper.setRequiredLevel("crme_childplaceofbirthcity", true); 
            formHelper.setRequiredLevel("crme_childplaceofbirthstateid", true); 
            formHelper.setRequiredLevel("crme_childplaceofbirthcountryid", true); 

            //CSDev
            formHelper.setFieldVisible("udo_child_married", false);
            formHelper.setFieldVisible("udo_dateofmarriage", false);

            //CSDev new Req Fields 
            formHelper.setRequiredLevel("crme_address1", true);
            formHelper.setRequiredLevel("crme_zippostalcodeid", true);
            formHelper.setRequiredLevel("crme_city", true);
            formHelper.setRequiredLevel("crme_stateprovinceid", true);
            formHelper.setRequiredLevel("crme_countryid", true);

            birthCountryChange(exCon);
            childLiveswithVetOnChange(exCon);

            formHelper.setDisabled("crme_dependentrelationship");

            // Clear out spouse marriage method fields
            formHelper.setValue("udo_marriagemethod", null);
            formHelper.setValue("udo_marriagemethodotherexplanation", null);

            break;

        //  Spouse
        case 935950001:
            //CSDev
            formHelper.setRequiredLevel("crme_marriagedate", true); 
            formHelper.setRequiredLevel("crme_marriagecity", true); 
            formHelper.setRequiredLevel("crme_marriagestateid", true); 
            formHelper.setRequiredLevel("crme_marriagecountryid", true); 

            formHelper.setRequiredLevel("crme_childplaceofbirthcity", false);
            formHelper.setRequiredLevel("crme_childplaceofbirthstateid", false);
            formHelper.setRequiredLevel("crme_childplaceofbirthcountryid", false);

            //CSDev new Req Fields 
            formHelper.setRequiredLevel("crme_address1", true);
            formHelper.setRequiredLevel("crme_zippostalcodeid", true);
            formHelper.setRequiredLevel("crme_city", true);
            formHelper.setRequiredLevel("crme_stateprovinceid", true);
            formHelper.setRequiredLevel("crme_countryid", true);

            marriageCountryChange(exCon);
            spouseLiveswithVetOnChange(exCon);
            spouseDetailsOnChange(exCon);

            formHelper.setDisabled("crme_dependentrelationship");

            // Default spouse marriage method fields
            if (formHelper.getValue("udo_marriagemethod") === undefined || formHelper.getValue("udo_marriagemethod") === null) {
                formHelper.setValue("udo_marriagemethod", 752280000);
                formHelper.setValue("udo_marriagemethodotherexplanation", null);
            }
            else {
                udo_marriagemethod_OnChange(exCon);
            }

            break;

        // All default behaviors
        default:
            ValidateSsn(exCon);
    }
}

function nameChange(exCon) {
    instantiateCommonScripts(exCon);
    var firstName = formHelper.getValue('crme_firstname');
    var lastName = formHelper.getValue('crme_lastname');
    var thisFirst = "";
    var thisLast = "";

    firstName !== null && firstName !== '' ? thisFirst = firstName.toUpperCase() : thisFirst = "";
    lastName !== null && lastName !== '' ? thisLast = lastName.toUpperCase() : thisLast = "";

    formHelper.setValue("crme_name", thisFirst + " " + thisLast);
}

function childLiveswithVetOnChange(exCon) {
    instantiateCommonScripts(exCon);
    var childLivesWithVet = formHelper.getValue("crme_childliveswithvet");
    var requiredControls = ["crme_childliveswithfirstname", "crme_childliveswithlastname"];
    var otherControls = ["crme_childliveswithmiddlename"];

    switch (childLivesWithVet) {
        case true:
            // If the child lives with the vet, don't require, see, and set to null all of those related fields.
            for (var i = 0; i < requiredControls.length; i++) {
                // Set other dependent type fields not required and null
                formHelper.setRequiredLevel(requiredControls[i]);
                formHelper.setFieldVisible(requiredControls[i]);
            }

            for (var i = 0; i < otherControls.length; i++) {
                // Set other dependent type fields not required and null
                formHelper.setFieldVisible(otherControls[i]);
            }
            // CSDev if they live with the vet than not require and hide
            formHelper.setRequiredLevel("crme_address1", false);
            formHelper.setFieldVisible("crme_address1", false);
            formHelper.setRequiredLevel("crme_zippostalcodeid", false);
            formHelper.setFieldVisible("crme_zippostalcodeid", false);
            formHelper.setRequiredLevel("crme_city", false);
            formHelper.setFieldVisible("crme_city", false);
            formHelper.setRequiredLevel("crme_stateprovinceid", false);
            formHelper.setFieldVisible("crme_stateprovinceid", false);
            formHelper.setRequiredLevel("crme_countryid", false);
            formHelper.setFieldVisible("crme_countryid", false);

            // CSDev Just Hide
            formHelper.setFieldVisible("crme_address2", false);
            formHelper.setFieldVisible("crme_address3", false);
            formHelper.setFieldVisible("crme_zipplus4", false);
            formHelper.setFieldVisible("crme_childliveswithfirstname", false);
            formHelper.setFieldVisible("crme_childliveswithmiddlename", false);
            formHelper.setFieldVisible("crme_childliveswithlastname", false);

            setChildLivesWithVetAddress(true);

            break;
        case false:
            for (var i = 0; i < requiredControls.length; i++) {
                // Set other dependent type fields not required and null
                formHelper.setRequiredLevel(requiredControls[i], false);
                formHelper.setValue(requiredControls[i], null);
                formHelper.setFieldVisible(requiredControls[i], false);
            }

            for (var i = 0; i < otherControls.length; i++) {
                // Set other dependent type fields not required and null
                formHelper.setValue(otherControls[i], null);
                formHelper.setFieldVisible(otherControls[i], false);
            }
            // CSDev if they live with the vet than not require and hide
            formHelper.setRequiredLevel("crme_address1", true);
            formHelper.setFieldVisible("crme_address1", true);
            formHelper.setRequiredLevel("crme_zippostalcodeid", true);
            formHelper.setFieldVisible("crme_zippostalcodeid", true);
            formHelper.setRequiredLevel("crme_city", true);
            formHelper.setFieldVisible("crme_city", true);
            formHelper.setRequiredLevel("crme_stateprovinceid", true);
            formHelper.setFieldVisible("crme_stateprovinceid", true);
            formHelper.setRequiredLevel("crme_countryid", true);
            formHelper.setFieldVisible("crme_countryid", true);

            // CSDev Just Show
            formHelper.setFieldVisible("crme_address2", true);
            formHelper.setFieldVisible("crme_address3", true);
            formHelper.setFieldVisible("crme_zipplus4", true);
            formHelper.setFieldVisible("crme_childliveswithfirstname", true);
            formHelper.setFieldVisible("crme_childliveswithmiddlename", true);
            formHelper.setFieldVisible("crme_childliveswithlastname", true);

            setChildLivesWithVetAddress(false);

            break;
        default:
            break;
    }
}

function setChildLivesWithVetAddress(useVetAddress) {
    if (useVetAddress) {
        if (formContext.getAttribute("crme_dependentmaintenance").getValue() === undefined || formContext.getAttribute("crme_dependentmaintenance").getValue() === null) {
            console.log("Dependent Maintenance ID not available at this time.");
            return;
        }

        var dmId = formContext.getAttribute("crme_dependentmaintenance").getValue()[0].id.replace("{", "").replace("}", "");
        var columns = ['crme_address1', 'crme_address2', 'crme_address3', 'crme_city', '_crme_stateprovinceid_value', '_crme_zippostalcodeid_value'];
        var filter = "$filter=activityid eq '" + dmId + "'";

        webApi.RetrieveMultiple("crme_dependentmaintenance", columns, filter)
            .then(function (data) {
                if (data.length > 0) {
                    // Hidden address fields used on 674 DocGen section 6 - set to Vet's address
                    formHelper.setValue("udo_childliveswithvetaddress1", data[0].crme_address1);
                    formHelper.setValue("udo_childliveswithvetaddress2", data[0].crme_address2);
                    formHelper.setValue("udo_childliveswithvetaddress3", data[0].crme_address3);
                    formHelper.setValue("udo_childliveswithvetcity", data[0].crme_city);

                    var statelookupValue = new Array();
                    statelookupValue[0] = new Object();
                    statelookupValue[0].id = "{" + data[0]["_crme_stateprovinceid_value"] + "}";
                    statelookupValue[0].name = data[0]["_crme_stateprovinceid_value@OData.Community.Display.V1.FormattedValue"];
                    statelookupValue[0].entityType = "crme_stateorprovincelookup";
                    formContext.getAttribute("udo_childliveswithvetstateprovinceid").setValue(statelookupValue);

                    var ziplookupValue = new Array();
                    ziplookupValue[0] = new Object();
                    ziplookupValue[0].id = "{" + data[0]["_crme_zippostalcodeid_value"] + "}";
                    ziplookupValue[0].name = data[0]["_crme_zippostalcodeid_value@OData.Community.Display.V1.FormattedValue"];
                    ziplookupValue[0].entityType = "crme_postalcodelookup";
                    formContext.getAttribute("udo_childliveswithvetzipcodeid").setValue(ziplookupValue);
                }
                else {
                    console.log("Could not find dependence maintenance record: " + dmId);
                }
            })
            .catch(function (ex) {
                console.log("Error reading dependence maintenance record: " + dmId + " - exception: " + ex);
            });
    }
    else {
        // Hidden address fields used on 674 DocGen section 6
        formHelper.setValue("udo_childliveswithvetaddress1", null);
        formHelper.setValue("udo_childliveswithvetaddress2", null);
        formHelper.setValue("udo_childliveswithvetaddress3", null);
        formHelper.setValue("udo_childliveswithvetcity", null);
        formContext.getAttribute("udo_childliveswithvetstateprovinceid").setValue(null);
        formContext.getAttribute("udo_childliveswithvetzipcodeid").setValue(null);
    }
}

function spouseLiveswithVetOnChange(exCon) {
    if (exCon) {
        instantiateCommonScripts(exCon);
    }
    var spouseLivesWithVet = formHelper.getValue("crme_liveswithspouse");
    var requiredControls = ["crme_monthlycontributiontospousesupport", "crme_address11", "crme_city1", "crme_stateprovinceid1", "crme_countryid1", "crme_zippostalcodeid1"];
    var otherControls = ["crme_address21", "crme_address31", "crme_zipplus41"];

    switch (spouseLivesWithVet) {
        case true:
            // If the spouse lives with the vet, don't require, see, and set to null all of those related fields.
            for (var i = 0; i < requiredControls.length; i++) {
                // Set other dependent type fields not required and null
                formHelper.setRequiredLevel(requiredControls[i], "none");
                formHelper.setValue(requiredControls[i], null);
                formHelper.setFieldVisible(requiredControls[i], false);
            }

            for (var i = 0; i < otherControls.length; i++) {
                // Set other dependent type fields not required and null
                formHelper.setValue(otherControls[i], null);
                formHelper.setFieldVisible(otherControls[i], false);
            }

            // CSDev if they live with the vet than not require 
            formHelper.setRequiredLevel("crme_address1", false);
            formHelper.setRequiredLevel("crme_zippostalcodeid", false);
            formHelper.setRequiredLevel("crme_city", false);
            formHelper.setRequiredLevel("crme_stateprovinceid", false);
            formHelper.setRequiredLevel("crme_countryid", false);

            break;
        case false:

            // If the spouse DOES NOT live with the vet, don't require, see, and set to null all of those related fields.
            for (var i = 0; i < requiredControls.length; i++) {
                // Set other dependent type fields not required and null
                formHelper.setRequiredLevel(requiredControls[i], true);
                formHelper.setFieldVisible(requiredControls[i]);
            }

            for (var i = 0; i < otherControls.length; i++) {
                // Set other dependent type fields not required and null
                formHelper.setFieldVisible(otherControls[i], true);
            }

            // CSDev if they live with the vet than require
            formHelper.setRequiredLevel("crme_address1", true);
            formHelper.setRequiredLevel("crme_zippostalcodeid", true);
            formHelper.setRequiredLevel("crme_city", true);
            formHelper.setRequiredLevel("crme_stateprovinceid", true);
            formHelper.setRequiredLevel("crme_countryid", true);

            break;
        default:
            break;
    }
}

function spouseDetailsOnChange(exCon) {
    if (exCon === undefined) {
        exCon = Xrm.Utility.getGlobalContext();
    }

    instantiateCommonScripts(exCon);

    var spouseDetails = formHelper.getValue("udo_spousedetails");

    if (spouseDetails === undefined || spouseDetails === null) {
        spouseDetails = 0;
    }

    if (spouseDetails === 752280000) { // Current Spouse
        formHelper.setFieldVisible("tab_General_section_SpouseMaritalHistory", true);
        formHelper.setFieldVisible("udo_spousetimesmarried", true);
        formHelper.setSectionVisible("tab_General_section_MarriageDatePlaceTerminated", "tab_General", false);

        // Save form to display Marital Spouse History subgrid in UCI on new dependent
        formContext.data.save().then(
            function Success() {
                console.log("Success on Dependent form save");
            },
            function Error() {
                console.log("Error on Dependent form save");
            });
    } else if (spouseDetails === 752280001) { // Previous Spouse
        formHelper.setFieldVisible("tab_General_section_SpouseMaritalHistory", false);
        formHelper.setFieldVisible("udo_spousetimesmarried", false);
        formHelper.setSectionVisible("tab_General_section_MarriageDatePlaceTerminated", "tab_General", true);
    } else {
        formHelper.setFieldVisible("tab_General_section_SpouseMaritalHistory", false);
        formHelper.setFieldVisible("udo_spousetimesmarried", false);
        formHelper.setSectionVisible("tab_General_section_MarriageDatePlaceTerminated", "tab_General", false);
    }
}

//*******************************************************
// Child Is Currently Married OnChange crme_depedent
function isCurrentlyMarried_change(exCon) {
    instantiateCommonScripts(exCon);
    syncState_MarriageFields();
}

//*******************************************************
// Child Previously Married OnChange crme_depedent
function isPreviouslyMarried_change(exCon) {
    instantiateCommonScripts(exCon);
    syncState_MarriageFields();
}

function showDependentTabs_change(exCon) {
    instantiateCommonScripts(exCon);
    showDependentTabs(exCon);
}

//*******************************************************
// Attended Last Term OnChange crme_depedent
function attendedlastterm_ChildDependentOnChange(exCon) {
    instantiateCommonScripts(exCon);
    var didAttendPreviousTerm = formHelper.getValue("udo_attendedlastterm");
    if (didAttendPreviousTerm) {
        formHelper.setValue("udo_cbattendedlasttermreportyesno", 752280000);
    } else {
        formHelper.setValue("udo_cbattendedlasttermreportyesno", 752280001);
    }
}

//*******************************************************
// Enrolled Full Time OnChange crme_depedent
function EnrolledFullTime_ChildDependentOnChange(exCon) {
    instantiateCommonScripts(exCon);

    // Used Yes No Option Set due to issue with Boolean in DocGen
    var isCurrentlyEnrolledFT = formHelper.getValue("udo_enrolledfulltime");
    if (isCurrentlyEnrolledFT) {
        formHelper.setValue("udo_cbisenrolledfulltimereportyesno", 752280000);

        // Clear potentially populated field values when enrolled full time set to 'Yes'
        formHelper.setValue("udo_enrolledsubject", "");
        formHelper.setValue("udo_enrolledsessionsperweek", null);
        formHelper.setValue("udo_enrolledhoursperweek", null);
    } else {
        formHelper.setValue("udo_cbisenrolledfulltimereportyesno", 752280001);
    }
}

//*******************************************************
// Tuition Paid By Government OnChange crme_depedent
function paidbydea_ChildDependentOnChange(exCon) {
    instantiateCommonScripts(exCon);
    // Used Yes No Option Set due to issue with Boolean in DocGen
    var paidbydea = formHelper.getValue("udo_paidbydea");
    if (paidbydea) {
        formHelper.setValue("udo_cbtuitionpaidbygovernmentreportyesno", 752280000);
    }
    else {
        formHelper.setValue("udo_cbtuitionpaidbygovernmentreportyesno", 752280001);
    }
}

// Monitor for a change in Age and Attendance of Dependent. Impacts VA FORM 21-674b, APR 2015
function crme_childage1823inschool_OnChange(exCon) {
    instantiateCommonScripts(exCon);
    var isSchoolAgeChildInSchool = formHelper.getValue("crme_childage1823inschool");
    schoolAgedChildInSchoolChange(isSchoolAgeChildInSchool);
}

function crme_childrelationship_OnChange(exCon) {
    instantiateCommonScripts(exCon);

    var childRelationship = formHelper.getValue("crme_childrelationship");

    // Step Child
    if (childRelationship === 935950001) {
        formHelper.setFieldVisible("udo_isstepchildbiologicalchildofspouse", true);
    }
    else {
        formHelper.setFieldVisible("udo_isstepchildbiologicalchildofspouse", false);
        formHelper.setValue("udo_isstepchildbiologicalchildofspouse", false);

        formHelper.setFieldVisible("udo_datestepchildenteredvethousehold", false);
        formHelper.setValue("udo_datestepchildenteredvethousehold", null);
    }
}

function udo_isstepchildbiologicalchildofspouse_OnChange(exCon) {
    instantiateCommonScripts(exCon);

    var isStepChild = formHelper.getValue("udo_isstepchildbiologicalchildofspouse");

    if (isStepChild === true) {
        formHelper.setFieldVisible("udo_datestepchildenteredvethousehold", true);
    }
    else {
        formHelper.setFieldVisible("udo_datestepchildenteredvethousehold", false);
        formHelper.setValue("udo_datestepchildenteredvethousehold", null);
    }
}

function crme_childpreviouslymarried_OnChange(exCon) {
    instantiateCommonScripts(exCon);

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

function udo_paidbydea_OnChange(exCon) {
    instantiateCommonScripts(exCon);

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
}

function udo_marriagemethod_OnChange(exCon) {
    // Default spouse marriage method fields
    if (formHelper.getValue("udo_marriagemethod") !== undefined || formHelper.getValue("udo_marriagemethod") !== null) {

        // Other [explain]
        if (formHelper.getValue("udo_marriagemethod") === 752280004) {
            formHelper.setFieldVisible("udo_marriagemethodotherexplanation", true);
            formHelper.setRequiredLevel("udo_marriagemethodotherexplanation", true);
        }
        else {
            formHelper.setFieldVisible("udo_marriagemethodotherexplanation", false);
            formHelper.setRequiredLevel("udo_marriagemethodotherexplanation", false);
            formHelper.setValue("udo_marriagemethodotherexplanation", null);
        }
    }
}