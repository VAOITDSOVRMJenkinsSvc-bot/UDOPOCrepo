/**
 * Created by VHAISDBLANCW on 2/5/2015.
 */
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
ClaimantInfo.Ssn = "";
ClaimantInfo.MilitaryPostalTypeCode = "";
ClaimantInfo.MilitaryPostOfficeTypeCode = "";

var VeteranInfo = {};
VeteranInfo.Ssn = "";
VeteranInfo.FileNumber = "";

var exCon = null;
var FormContext = null;
var globCon = null;
var webApi = null;

function initializeContext() {
    exCon = GetExecutionContext();
    formContext = exCon.getFormContext();
    globCon = Xrm.Utility.getGlobalContext();
    var version = globCon.getVersion();
    var lib = new CrmCommonJS(version, exCon);
    webApi = lib.webApi;
}

//Create 0820a Service Request when Call Type = FNOD
function createFNODServiceRequest() {

    var varFNODtype = '';
    if (Xrm.Page.getAttribute('va_dispositionsubtype').getValue() != null) {
        varFNODtype = Xrm.Page.getAttribute('va_dispositionsubtype').getValue();
    }

    var varMsg = 'Call type FNOD has been Selected.\n\n';
    var var0820x = '0820a';

    switch (varFNODtype) {
        //FNOD:Death of a Dependent
        case 953850020:
            varMsg += 'Would you like to create 0820 for the Dependent?';
            var0820x = '0820';
            break;
            //FNOD:MOD Payment
        case 953850033:
            varMsg += 'Would you like to create 0820f for the Beneficiary?';
            var0820x = '0820f';
            break;
            //Generic FNOD SR
        default:
            return;
    }
    varMsg += '\n\nPlease click OK to confirm.';

    if (Xrm.Page.ui.getFormType() == 2) {
        if (confirm(varMsg)) {
            CreateClaimServiceRequest(null, 'FNOD', true, var0820x, null, Xrm.Page.getAttribute('va_disposition').getValue());
        }
    }
}

//No Phone Number Available checkbox
//1-15-13
function fnIdentifyCallerPhone() {
    var va_nophonenumberavailable = Xrm.Page.getAttribute('va_nophonenumberavailable').getValue();

    if (va_nophonenumberavailable) {
        Xrm.Page.getAttribute('va_identifycallerphone').setRequiredLevel('none');
        Xrm.Page.getAttribute('va_identifycallerphone').setValue(null);

        $('#va_identifycallerphone').prop('disabled', true);
    }
    else {
        Xrm.Page.getAttribute('va_identifycallerphone').setRequiredLevel('required');

        $('#va_identifycallerphone').prop('disabled', false);
    }
}


function veteranVAI() {
    //var org = Xrm.Page.context.getServerUrl();
    var org = Xrm.Page.context.getClientUrl();
    var veteran = Xrm.Page.getAttribute('regardingobjectid').getValue();
    var vaiIframe = Xrm.Page.ui.controls.get("IFRAME_vai");
    if (veteran != null && Xrm.Page.ui.getFormType() != CRM_FORM_TYPE_CREATE) {
        var veteranid = veteran[0].id;
        var url = org + "/userdefined/areas.aspx?&navItemName=VAIs%20%20&oId=%7b" + veteranid.replace("{", "").replace("}", "") + "%7d&oType=2&pagetype=entitylist&security=852023&tabSet=va_contact_va_vai_Veteran"

        vaiIframe.setSrc(url);
    }
    else {
        return "about:blank";
    }

}

function getClaimantInfo() {
    var claimantXml = Xrm.Page.getAttribute('va_findcorprecordresponse').getValue();
    if (claimantXml) {
        var claimantXmlObject = _XML_UTIL.parseXmlObject(claimantXml);
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

function openItfForm() {
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

function CanCreateIntentToFile() {
    var id = Xrm.Page.data.entity.getId();
    var canCreateIntentToFile = false;
    //var serverUrl = Xrm.Page.context.getServerUrl();
    //var serverUrl = location.protocol + "//" + location.host //+ "/" + Xrm.Page.context.getOrgUniqueName();
    var serverUrl = Xrm.Page.context.getClientUrl();
    var odataSelect = serverUrl + "/XRMServices/2011/OrganizationData.svc" + "/" + "va_intenttofileSet?$filter=statecode/Value eq 0 and va_PhoneCallId/Id eq guid'" + id + "'";
    var roleName = null;
    $.ajax(
        {
            type: "GET",
            async: false,
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: odataSelect,
            beforeSend: function (XMLHttpRequest) {
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
            },
            success: function (data, textStatus, XmlHttpRequest) {
                canCreateIntentToFile = data.d.results.length == 0;
            },
            error: function (XmlHttpRequest, textStatus, errorThrown) {
                alert('OData Select Failed: ' + textStatus + errorThrown + odataSelect);
            }
        }
    );
    return canCreateIntentToFile;
}

//Get Rolename based on RoleId
function GetRoleName(roleId) {
    //var serverUrl = Xrm.Page.context.getServerUrl();
    //var serverUrl = location.protocol + "//" + location.host //+ "/" + Xrm.Page.context.getOrgUniqueName();
    var serverUrl = Xrm.Page.context.getClientUrl();
    var odataSelect = serverUrl + "/XRMServices/2011/OrganizationData.svc" + "/" + "RoleSet?$filter=RoleId eq guid'" + roleId + "'";
    var roleName = null;
    $.ajax(
        {
            type: "GET",
            async: false,
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: odataSelect,
            beforeSend: function (XMLHttpRequest) {
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
            },
            success: function (data, textStatus, XmlHttpRequest) {
                roleName = data.d.results[0].Name;
            },
            error: function (XmlHttpRequest, textStatus, errorThrown) {
                alert('OData Select Failed: ' + textStatus + errorThrown + odataSelect);
            }
        }
    );
    return roleName;
}

function SetItfRibbonEnableDisable() {
    var callTypeObject = Xrm.Page.getAttribute("va_disposition");
    if (callTypeObject == null) {
        return false;
    }

    if (callTypeObject.getValue() == null) {
        return false;
    }

    var callType = callTypeObject.getValue();
    var callTypeName = callType[0].name;
    if (callTypeName != "Intent to File") {
        return false;
    }

    var isEnabled = CheckUserRole();
    return isEnabled;
}

function CheckUserRole() {
    var currentUserRoles = Xrm.Page.context.getUserRoles();
    for (var i = 0; i < currentUserRoles.length; i++) {
        var userRoleId = currentUserRoles[i];
        var userRoleName = GetRoleName(userRoleId);
        if (userRoleName == "Intent To File") {
            return true;
        }
    }
    return false;
}

function SetVisibilityOfItfTab() {
    Xrm.Page.ui.tabs.get("tab_IntentToFile").setVisible(false);
}

// Disable a subgrid on a form
function DisableIntentToFileSubgrid() {
    document.getElementById("IntentToFile_span").disabled = "true";
}
