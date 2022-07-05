"use strict";

window.LOCID_JUMP_TO_RIBBON = "[";
window.top.LOCID_JUMP_TO_RIBBON_CONTROL = "]";

var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.PeopleList = Va.Udo.Crm.Scripts.PeopleList || {};
//var webApi;
//var globalCon = GetGlobalContext();
window["ENTITY_SET_NAMES"] = window["ENTITY_SET_NAMES"] || JSON.stringify({
    "udo_person": "udo_persons",
    "va_bankaccount": "va_bankaccounts",
    "udo_letter": "udo_letters",
    "udo_servicerequest": "udo_servicerequests",
    "va_intenttofile": "va_intenttofiles",
    "va_fnod": "va_fnods",
    "udo_claimestablishment": "udo_claimestablishments"

});

window["ENTITY_PRIMARY_KEYS"] = window["ENTITY_PRIMARY_KEYS"] || JSON.stringify({
    "udo_person": "udo_personid",
    "va_bankaccount": "va_bankaccountid",
    "udo_letter": "udo_letterid",
    "udo_servicerequest": "udo_servicerequestid",
    "va_intenttofile": "va_intenttofileid",
    "va_fnod": "va_fnodid",
    "udo_claimestablishment": "udo_claimestablishmentid"
});

Va.Udo.Crm.Scripts.PeopleList.Config = {
    "CADD": {
        MessageName: "udo_InitiateCadd",
        ConfigName: "CADD",
        EntityName: "va_bankaccount",
        Name: "CADD",
        Title: "CADD - PEOPLE LIST",
        Header: "CADD - Select Person",
        RequireSelection: true,
        Filter: "$filter=_udo_idproofid_value eq /idproofid/ and udo_ptcpntid ne null",
        Select: "crme_SearchType, crme_url, crme_ReturnMessage",
        SelectionTypes: ["(payeecode!=null && payeecode!='')"], //All
        FilterSelection: function (row, data) {
            if (data.fidExists) {
                //row.className +=" disabled";
                $(row).find("td:first").append(" (has fiduciary)");

                var fidpopup = function () {
                    Va.Udo.Crm.Scripts.Popup.Error("Fiduciary involvement or veteran is incompetent. Change of Address process cannot be initiated.  Please send VAI to the Fiduciary Department.");
                };

                row.ondblclick = function () { fidpopup() };

                row.onkeypress = function (e) {
                    if (e.keyCode === 13 || e.keyCode === 32) {
                        fidpopup();
                    }
                };
            }
        }
    },
    "LETTERS": {
        MessageName: "udo_InitiateLetter",
        ConfigName: "LETTERS",
        EntityName: "udo_letter",
        Name: "Letter",
        Title: "LETTERS - PEOPLE LIST",
        Header: "Letters - Select Person",
        RequireSelection: true,
        Filter: "$filter=_udo_idproofid_value eq /idproofid/ and udo_ptcpntid ne null",
        Select: "crme_SearchType, crme_url, crme_ReturnMessage",
        SelectionTypes: [411] //All
    },
    "SERVICEREQUEST": {
        MessageName: "udo_InitiateServiceRequest",
        ConfigName: "SERVICEREQUEST",
        EntityName: "udo_servicerequest",
        Name: "Service Request",
        Title: "SERVICE REQUEST - PEOPLE LIST",
        Header: "Letters - Select Person",
        RequireSelection: false,  // Allow the creation of a service request without a person selected
        CreateWithoutPersonLabel: "Create Service Request without Payee details",
        CreateWithoutPersonLegend: "If you can't find the person/payee in the list below, you can create a service request without Payee details.",
        Filter: "$filter=_udo_idproofid_value eq /idproofid/",
        Select: "*",
        SelectionTypes: [411] //All
    },
    "ITF": {
        MessageName: "udo_InitiateITF",
        ConfigName: "ITF",
        EntityName: "va_intenttofile",
        Name: "Intent To File",
        Title: "ITF - PEOPLE LIST",
        Header: "ITF - Select Person",
        RequireSelection: true,
        Filter: "$filter=_udo_idproofid_value eq /idproofid/ and udo_type ne 752280003 and udo_ptcpntid ne null",
        Select: "crme_SearchType, crme_itfparameters, crme_ReturnMessage",
        SelectionTypes: [411] //All
    },
    "FNOD": {
        MessageName: "udo_InitiateFNOD",
        ConfigName: "FNOD",
        EntityName: "va_fnod",
        Name: "First Notice of Death (FNOD)",
        Title: "FNOD - PEOPLE LIST",
        Header: "FNOD - Select Person",
        RequireSelection: true,
        Filter: "$filter=_udo_idproofid_value eq /idproofid/ and udo_type ne 752280003",
        Select: "*",
        //Select: "crme_SearchType, crme_ReturnMessage",
        SelectionTypes: [752280000]
    },
    "CLAIMESTABLISHMENT": {
        MessageName: "udo_InitiateClaimEstablishment",
        ConfigName: "CLAIMESTABLISHMENT",
        EntityName: "udo_ClaimEstablishment",
        Name: "ClaimEstablishment",
        Title: "CLAIM ESTABLISHMENT - PEOPLE LIST",
        Header: "Claim Establishment - Select Person",
        RequireSelection: true,
        Filter: "$filter=_udo_idproofid_value eq /idproofid/ and udo_ptcpntid ne null",
        Select: "crme_SearchType, crme_url, crme_ReturnMessage",
        SelectionTypes: [411] //All
    },
    "DEPENDENTVERIFICATION": {
        MessageName: "udo_InitiateServiceRequest",
        ConfigName: "SERVICEREQUEST",
        EntityName: "udo_servicerequest",
        Name: "Service Request",
        Title: "SERVICE REQUEST - PEOPLE LIST",
        Header: "Dependent Verification - Select Person",
        RequireSelection: true,
        CreateWithoutPersonLabel: "",
        CreateWithoutPersonLegend: "",
        Filter: "$filter=_udo_idproofid_value eq /idproofid/",
        Select: "*",
        SelectionTypes: [411] //All
    }
};
//Va.Udo.Crm.Scripts.PeopleList.GetWebApi = function () {
//    var version = globalCon.getVersion();
//    webApi = new CrmCommonJS.WebApi(version);
//}
Va.Udo.Crm.Scripts.PeopleList.Select = function (data) {
    var Config = {};
    Config = Va.Udo.Crm.Scripts.PeopleList.Config[Va.Udo.Crm.Scripts.PeopleList.SearchType];

    function create_callback(data) {
        $('div#tmpDialog').hide(100);
        var url = "";
        if (data.DataIssue != false || data.Timeout != false || data.Exception != false) {
            Va.Udo.Crm.Scripts.Popup.Error("an issue occurred while initializing the " + Config.Name + ": " + data.ResponseMessage);
        }
        else {

            console.log(Va.Udo.Crm.Scripts.PeopleList.SearchType);
            if (Va.Udo.Crm.Scripts.PeopleList.SearchType == "ITF") {

                var getUrl = function (etn, parameters) {
                    var paramArray = Object.keys(parameters).map(function (key) {
                        return [key, parameters[key]];
                    });

                    var url = "";
                    for (var i = 0; i < paramArray.length; i++) {
                        var KeyValue = paramArray[i][0] + "=" + paramArray[i][1];
                        if (i < paramArray.length - 1) {
                            KeyValue = KeyValue + "&";
                        }
                        url = url + KeyValue;
                    }

                    // var url = "";
                    // for (var key in parameters) {
                    //     if (parameters.hasOwnProperty(key)) {
                    //         url = url + key + "=" + parameters[key] + "&";
                    //     }
                    // }

                    //url = encodeURIComponent(url);
                    // url = "../main.aspx?appid=b2acc5aa-c267-ea11-a99d-001dd8009f4b&newWindow=true&etn=" + etn + "&pagetype=entityrecord&extraqs=" + url;
                    //url = globalCon.getClientUrl() + "/main.aspx?appid=b2acc5aa-c267-ea11-a99d-001dd8009f4b&etn=va_intenttofile&pagetype=entityrecord&extraqs=" + url;
                    // console.log(url);
                    //   console.log("data is: " + data);
                    //  alert(url);                            

                    url = "http://event/?eventname=OpenITF&" + url;

                    return url;
                };

                var result = data;
                console.log(data);
                var parameters = setITFParameters(result.itfparameters);
                var url = getUrl("va_intenttofile", parameters);
                console.log(url);
                window.open(url);
                debugger;
            }
            else {

                var result = data.result;
                console.log(data);
                var context = Xrm.Utility.getGlobalContext();
                context.then(function (context) {
                    switch (Va.Udo.Crm.Scripts.PeopleList.SearchType) {
                        case "CADD":
                            //url = context.getClientUrl() + "/main.aspx?etn=va_bankaccount&pagetype=entityrecord&id=" + result["va_bankaccountid"];
                            url = "http://uii/CADDProcessHost/Navigate?url=" + context.getClientUrl() + "/main.aspx?cmdbar=false&navbar=off&newWindow=true&pagetype=entityrecord&etn=va_bankaccount&id=" + result["va_bankaccountid"];
                            window.open(url);
                            window.open("http://uii/Global Manager/ShowTab?CADDProcessHost");
                            window.open("http://uii/CADD People List/Close");

                            break;
                        case "LETTERS":
                            //url = context.getClientUrl() + "/main.aspx?etn=udo_lettergeneration&pagetype=entityrecord&id=" + result["udo_lettergenerationid"];
                            url = "http://uii/LetterGeneration/Navigate?url=" + context.getClientUrl() + "/main.aspx?cmdbar=false&navbar=off&newWindow=true&pagetype=entityrecord&etn=udo_lettergeneration&id=" + result["udo_lettergenerationid"];
                            window.open(url);
                            window.open("http://uii/Global Manager/ShowTab?LetterGeneration");
                            window.open("http://uii/LETTERS People List/Close");

                            break;
                        case "SERVICEREQUEST":
                            //url = context.getClientUrl() + "/main.aspx?etn=udo_servicerequest&pagetype=entityrecord&id=" + result["udo_servicerequestid"];
                            url = "http://uii/UdoServiceRequest/Navigate?url=" + context.getClientUrl() + "/main.aspx?cmdbar=false&navbar=off&newWindow=true&pagetype=entityrecord&etn=udo_servicerequest&id=" + result["udo_servicerequestid"];
                            window.open(url);
                            window.open("http://uii/Global Manager/ShowTab?UdoServiceRequest");
                            window.open("http://uii/SERVICEREQUEST People List/Close");

                            break;
                        case "CLAIMESTABLISHMENT":
                            //url = context.getClientUrl() + "/main.aspx?etn=udo_claimestablishment&pagetype=entityrecord&id=" + result["udo_claimestablishmentid"];
                            //url = context.getClientUrl() + "/main.aspx?etn=udo_claimestablishment&pagetype=entityrecord&id=" + result["udo_claimestablishmentid"];
                            url = "http://uii/Claim Establishment/Navigate?url=" + context.getClientUrl() + "/main.aspx?cmdbar=false&navbar=off&newWindow=true&pagetype=entityrecord&etn=udo_claimestablishment&id=" + result["udo_claimestablishmentid"];
                            window.open(url);
                            window.open("http://uii/Global Manager/ShowTab?Claim Establishment");
                            window.open("http://uii/Claim Establishment - People List/Close");

                            break;
                        case "FNOD":
                            //url = context.getClientUrl() + "/main.aspx?etn=va_fnod&pagetype=entityrecord&id=" + result["va_fnodid"];
                            url = "http://uii/FNOD Form/Navigate?url=" + context.getClientUrl() + "/main.aspx?cmdbar=false&navbar=off&newWindow=true&pagetype=entityrecord&etn=va_fnod&id=" + result["va_fnodid"];
                            window.open(url);
                            window.open("http://uii/Global Manager/ShowTab?FNOD Form");
                            window.open("http://uii/FNOD People List/Close");

                            break;
                        case "DEPENDENTVERIFICATION":
                            //url = context.getClientUrl() + "/main.aspx?etn=udo_servicerequest&pagetype=entityrecord&id=" + result["udo_servicerequestid"];
                            url = "http://uii/UdoServiceRequest/Navigate?url=" + context.getClientUrl() + "/main.aspx?cmdbar=false&navbar=off&newWindow=true&pagetype=entityrecord&etn=udo_servicerequest&id=" + result["udo_servicerequestid"];
                            window.open(url);
                            window.open("http://uii/Global Manager/ShowTab?UdoServiceRequest");

                        default:
                            break;
                    }
                    //var url = globalCon.getClientUrl() + "/main.aspx?etn=" + result.entityType + "&pagetype=entityrecord&id=" + result.id;

                    // url = "www.google.com";
                    //  window.open("http://event/?eventname=TestEventITF&url=" + url);
                    console.log(url);
                    window.open(url);
                });
            }
        }
    }

    function create(obj) {
        $('div#tmpDialog').show();

        //var requestParams = null;
        //var paramTypes = null;

        var metaDataObj = {};
        //metaDataObj.boundParameter = null;
        //metaDataObj.operationName = Config.MessageName
        //metaDataObj.operationType = 0;

        if (Va.Udo.Crm.Scripts.PeopleList.SearchType === "SERVICEREQUEST" || Va.Udo.Crm.Scripts.PeopleList.SearchType === "DEPENDENTVERIFICATION") {
            metaDataObj = {};
            var parameters = {};
            parameters.NoPayeeDetails = obj !== null && typeof obj != "undefined" ? false : true;
            var parententityreference = {};
            parententityreference.udo_personid = (obj !== null && obj !== undefined) ? obj.getAttribute('personid') : Va.Udo.Crm.Scripts.PeopleList.VeteranPersonId;
            parententityreference["@odata.type"] = "Microsoft.Dynamics.CRM.udo_person";
            parameters.ParentEntityReference = parententityreference;
            parameters.RequestType = (Config.Data.RequestType !== null && Config.Data.RequestType !== undefined) ? Config.Data.RequestType : null;
            parameters.RequestSubType = (Config.Data.RequestSubType !== null && Config.Data.RequestSubType !== undefined) ? Config.Data.RequestSubType : null;

            var udo_InitiateServiceRequestRequest = {
                NoPayeeDetails: parameters.NoPayeeDetails,
                ParentEntityReference: parameters.ParentEntityReference,
                RequestType: parameters.RequestType,
                RequestSubType: parameters.RequestSubType,

                getMetadata: function () {
                    return {
                        boundParameter: null,
                        parameterTypes: {
                            "NoPayeeDetails": {
                                "typeName": "Edm.Boolean",
                                "structuralProperty": 1
                            },
                            "ParentEntityReference": {
                                "typeName": "mscrm.crmbaseentity",
                                "structuralProperty": 5
                            },
                            "RequestType": {
                                "typeName": "Edm.String",
                                "structuralProperty": 1
                            },
                            "RequestSubType": {
                                "typeName": "Edm.String",
                                "structuralProperty": 1
                            }
                        },
                        operationType: 0,
                        operationName: "udo_InitiateServiceRequest"
                    };
                }
            };

            metaDataObj = udo_InitiateServiceRequestRequest;
            //requestParams = null;
        }
        else {
            metaDataObj = {
                ParentEntityReference: {
                    udo_personid: (obj !== null && obj !== undefined) ? obj.getAttribute('personid')
                        : Va.Udo.Crm.Scripts.PeopleList.VeteranPersonId,
                    "@odata.type": "Microsoft.Dynamics.CRM.udo_person"
                },

                getMetadata: function () {
                    return {
                        boundParameter: null,
                        operationType: 0,
                        operationName: Config.MessageName,
                        parameterTypes: {
                            "ParentEntityReference": {
                                "typeName": "mscrm.crmbaseentity",
                                "structuralProperty": 5
                            }
                        }
                    };
                }
            };
        }
        Xrm.WebApi.online.execute(metaDataObj)
            .then(function (response) {
                response.json().then(function (body) { create_callback(body); create_complete(); });
            })
            .catch(function (err) {
                Va.Udo.Crm.Scripts.Popup.Error("Error initiating " +
                    Config.Name +
                    " record; please try refreshing the page: " +
                    err.responseText);
                create_complete();
            });
    }

    function setITFParameters(xmlData) {
        //alert('In set ITF params');
        var parameters = {};
        var relationship = "";

        parameters["udo_idproofid"] = Config.Data.IDProofId;
        //parameters["udo_idproofidname"] = '[[Id Proof.udo_title]+]';
        //parameters["udo_idproofidtype"] = '[[Id Proof.LogicalName]+]';

        parameters["udo_veteranid"] = Config.Data.VeteranId;
        //parameters["udo_veteranidname"] = '[[Veteran.fullname]+]';
        //parameters["udo_veteranidtype"] = '[[Veteran.LogicalName]+]';

        var relStr = Config.Data.Relationship;
        if (relStr != "") {
            var relArr = relStr.split(',');
            relationship = relArr[1];
        }


        //alert(parameters["udo_veteranid"]);

        var xmlDoc = $.parseXML(xmlData);
        var $xml = $(xmlDoc);

        parameters["udo_personid"] = $xml.find('udo_personid').text();
        //parameters["va_phonecallid"] = $xml.find('va_phonecallid').text();
        //parameters["va_phonecallidname"] = $xml.find('va_phonecallidname').text();
        parameters["va_participantid"] = $xml.find('va_participantid').text();


        //if (($xml.find('va_claimantparticipantid').text() != "") && (relationship == "Veteran(Self)" || relationship == "VSO/POA/AA" || relationship == "Fiduciary" || relationship || "Other")) {
        //    parameters["va_claimantparticipantid"] = $xml.find('va_participantid').text();
        //}
        //else {
        parameters["va_claimantparticipantid"] = $xml.find('va_claimantparticipantid').text();
        //}
        if ($xml.find('va_veteranfirstname').text() != "") { parameters["va_veteranfirstname"] = $xml.find('va_veteranfirstname').text(); }

        //if (($xml.find('va_veteranfirstname').text() != "") && (relationship == "Veteran(Self)")) { parameters["va_claimantfirstname"] = $xml.find('va_veteranfirstname').text(); }
        //else {
        parameters["va_claimantfirstname"] = $xml.find('va_claimantfirstname').text();
        //}
        if ($xml.find('va_veteranlastname').text() != "") { parameters["va_veteranlastname"] = $xml.find('va_veteranlastname').text(); }
        //if (($xml.find('va_veteranlastname').text() != "") && (relationship == "Veteran(Self)")) { parameters["va_claimantlastname"] = $xml.find('va_veteranlastname').text(); }
        //else {
        parameters["va_claimantlastname"] = $xml.find('va_claimantlastname').text();
        //}
        if ($xml.find('va_veteranmiddleinitial').text() != "") { parameters["va_veteranmiddleinitial"] = $xml.find('va_veteranmiddleinitial').text(); }
        //if (($xml.find('va_veteranmiddleinitial').text() != "") && (relationship == "Veteran(Self)")) { parameters["va_claimantmiddleinitial"] = $xml.find('va_veteranmiddleinitial').text(); }
        //else {
        parameters["va_claimantmiddleinitial"] = $xml.find('va_claimantmiddleinitial').text();
        //}
        if ($xml.find('va_veteranssn').text() != "") { parameters["va_veteranssn"] = $xml.find('va_veteranssn').text(); }
        //if (($xml.find('va_veteranssn').text() != "") && (relationship == "Veteran(Self)")) { parameters["va_claimantssn"] = $xml.find('va_veteranssn').text(); }
        //else {
        parameters["va_claimantssn"] = $xml.find('va_claimantssn').text();
        //}
        if ($xml.find('va_filenumber').text() != "") { parameters["va_veteranfilenumber"] = $xml.find('va_filenumber').text(); }
        if ($xml.find('va_veteranphone').text() != "") { parameters["va_veteranphone"] = $xml.find('va_veteranphone').text(); }
        if ($xml.find('va_veterandateofbirth').text() != "") {
            var dob = $xml.find('va_veterandateofbirth').text();
            var dbparts = dob.split('/');
            if (dbparts.length == 1) dbparts = dob.toUpperCase().split('%2F');
            if (dbparts.length == 3) {
                var dbyear = Number(dbparts[2]);
                if (dbyear >= 1900) {
                    parameters["va_veterandateofbirth"] = $xml.find('va_veterandateofbirth').text();
                } else {
                    Va.Udo.Crm.Scripts.Popup.Warning("WARNING: Unable to set the date of birth for the ITF.  DOB before 1900.");
                }
            }
        }
        if ($xml.find('va_veteranaddressline1').text() != "") { parameters["va_veteranaddressline1"] = $xml.find('va_veteranaddressline1').text(); }
        if ($xml.find('va_veteranaddressline2').text() != "") { parameters["va_veteranaddressline2"] = $xml.find('va_veteranaddressline2').text(); }
        if ($xml.find('va_veteranunitnumber').text() != "") { parameters["va_veteranunitnumber"] = $xml.find('va_veteranunitnumber').text(); }
        if ($xml.find('va_veterancity').text() != "") { parameters["va_veterancity"] = $xml.find('va_veterancity').text(); }
        if ($xml.find('va_veteranstate').text() != "") { parameters["va_veteranstate"] = $xml.find('va_veteranstate').text(); }
        if ($xml.find('va_veteranzip').text() != "") { parameters["va_veteranzip"] = $xml.find('va_veteranzip').text(); }
        if ($xml.find('va_veterancountry').text() != "") { parameters["va_veterancountry"] = $xml.find('va_veterancountry').text(); }
        if ($xml.find('va_veteranemail').text() != "") { parameters["va_veteranemail"] = $xml.find('va_veteranemail').text(); }
        if ($xml.find('va_militarypostalcodevalue').text() != "") { parameters["va_militarypostalcodevalue"] = $xml.find('va_militarypostalcodevalue').text(); }
        if ($xml.find('va_militarypostofficetypecodevalue').text() != "") { parameters["va_militarypostofficetypecodevalue"] = $xml.find('va_militarypostofficetypecodevalue').text(); }
        if ($xml.find('va_gender').text() != "") {
            if ($xml.find('va_gender').text() == "Male") { parameters["va_veterangender"] = "0"; }
            else parameters["va_veterangender"] = "1";

            // parameters["va_veterangender"] = $xml.find('va_gender').text(); }
        }
        return parameters;
    }

    function create_complete() {
        $('div#tmpDialog').hide();
    }
    create(data);
};

Va.Udo.Crm.Scripts.PeopleList.VeteranPersonId = "";
Va.Udo.Crm.Scripts.PeopleList.InitializeStatus = "";
Va.Udo.Crm.Scripts.PeopleList.Initialize = function () {
    var Config = {};

    function execute() {

        Va.Udo.Crm.Scripts.PeopleList.InitializeStatus = "Initializing";
        $('div#tmpDialog').show();
        $("#resultsFieldSetDiv").hide();
        $("#notFoundDiv").hide();
        //Va.Udo.Crm.Scripts.PeopleList.GetWebApi();
        Va.Udo.Crm.Scripts.PeopleList.SearchType = getParamValue("type");
        //Va.Udo.Crm.Scripts.PeopleList.SearchType = "LETTERS";
        Config = Va.Udo.Crm.Scripts.PeopleList.Config[Va.Udo.Crm.Scripts.PeopleList.SearchType];

        //Test data
        /*_udo_idproofid_value@OData.Community.Display.V1.FormattedValue:"IdProof VRMHAYES, IRA",
         _udo_idproofid_value:"65798a87-40c0-e511-942c-00155d14d840",
         _udo_veteranid_value@Microsoft.Dynamics.CRM.associatednavigationproperty:"udo_veteranid",
         _udo_veteranid_value@Microsoft.Dynamics.CRM.lookuplogicalname:"contact",
         _udo_veteranid_value:"188053ee-36ae-e511-9423-0050568d0564",
         udo_personid:"bc798a87-40c0-e511-942c-00155d14d840"
            Config.Data = {
            VeteranId: "188053ee-36ae-e511-9423-0050568d0564",
            IDProofId: "65798a87-40c0-e511-942c-00155d14d840",
            InteractionId: "",
            Relationship: "",
            RequestType: "",
            RequestSubType: ""*/

        Config.Data = {
            VeteranId: getParamValue("vetid"),
            IDProofId: getParamValue("idproofid"),
            InteractionId: getParamValue("intrid"),
            Relationship: getParamValue("relstr"),
            RequestType: getParamValue("udo_requesttype"),
            RequestSubType: getParamValue("udo_requestsubtype")
        };

        if (!Config.RequireSelection) {
            var buttontext = Config.CreateWithoutPersonLabel;
            var legendtext = Config.CreateWithoutPersonLegend;
            var btn = $("#createWithoutPersonButton");
            btn.attr('title', buttontext);
            btn.text(buttontext);
            btn.bind("click", function () {
                Va.Udo.Crm.Scripts.PeopleList.Select(null);
            });

            $("#createWithoutPersonFieldset").prepend("<legend>" + legendtext + "</legend>");
            $("#createWithoutPerson").css('display', 'block');
        } else {
            $("#createWithoutPerson").remove();
        }
        $("#ListTitle").text(Config.Title);
        $(document).attr("title", Config.Title);

        getPeople();
    }

    function getParamValue(name) {
        var par = Va.Udo.Crm.Scripts.Utility.getUrlParams();

        if (typeof par[name] !== "undefined")
            return par[name];

        return par.data[name];
    };

    function getPeople() {
        var filter = Config.Filter;
        console.log(filter);
        console.log(Config.Data.IDProofId);
        //return;
        filter = filter.replace("/idproofid/", Config.Data.IDProofId.replace("{", "").replace("}", ""));

        //filter = filter.replace("/vetid/", Config.Data.VeteranId.replace("{", "").replace("}", ""));
        var cols = ["udo_personid", "udo_name", "udo_dobstr", "udo_type", "udo_ssn", "udo_ptcpntid",
            "udo_payeecode", "udo_awardtypecode", "udo_fidexists", "udo_awardsexist", "udo_pendingclaimsexist"];
        filter = "?$orderby=udo_type asc, udo_payeecode, udo_name&" + "$select=" + cols.join(",") + "&" + filter;

        Xrm.WebApi.retrieveMultipleRecords("udo_person", filter)
            .then(
                function (data) {
                    console.log(data);
                    getPeople_callback(data.value);

                }).catch(function (error) {
                    Va.Udo.Crm.Scripts.Popup.Error("Error Retrieving People:" + error.message);
                });
    }

    function getPeople_callback(data) {
        $('div#tmpDialog').show();

        // get the table
        var table = $("#personSearchResultsTable");

        // reset the table by removing all data rows
        $("#personSearchResultsTable").find("thead, tr, th").remove();
        $("#personSearchResultsTable").find("tr:gt(0)").remove();
        $("#resultsFieldSetDiv").hide();

        // If not CADD, remove duplicate people
        if (Va.Udo.Crm.Scripts.PeopleList.SearchType !== "CADD") {
            data = data.filter((value, index, self) =>
            index === self.findIndex((t) => (
                t.udo_ptcpntid === value.udo_ptcpntid
            ))
        );
        }

        if (data != null && data.length > 0) {

            var thead = document.createElement('thead');
            var theadRow = document.createElement('tr');

            for (var i = 0; i < data.length; i++) {
                var personid = data[i].udo_personid == null ? "" : data[i].udo_personid;
                var name = data[i].udo_name == null ? "" : data[i].udo_name;
                var DOB = data[i].udo_dobstr == null ? "" : data[i].udo_dobstr;
                var personType = data[i].udo_type == null ? "0" : data[i].udo_type;
                var personTypeName = "";
                switch (personType) {
                    case 752280000:
                        personTypeName = "Veteran";
                        Va.Udo.Crm.Scripts.PeopleList.VeteranPersonId = personid.toString();
                        break;
                    case 752280001:
                        personTypeName = "Dependent";
                        break;
                    case 752280002:
                        personTypeName = "Beneficiary";
                        break;
                    case 752280003:
                        personTypeName = "Associated Contact";
                        break;
                }
                var SSN = data[i].udo_ssn == null ? "" : data[i].udo_ssn;
                var participantId = data[i].udo_ptcpntid == null ? "" : data[i].udo_ptcpntid;
                var payeecode = data[i].udo_payeecode == null ? "" : data[i].udo_payeecode;
                //var Type = data[i].udo_type == null ? "" : data[i].udo_type;
                var Type = data[i].udo_awardtypecode == null ? "" : data[i].udo_awardtypecode;
                var fidExists = data[i].udo_fidexists == null ? false : data[i].udo_fidexists;
                var awardExist = data[i].udo_awardsexist == null ? false : data[i].udo_awardsexist;
                var pendingClaimsExists = data[i].udo_pendingclaimsexist == null ? false : data[i].udo_pendingclaimsexist;

                var cleanData = {
                    personid: personid,
                    name: name,
                    DOB: DOB,
                    personType: personType,
                    personTypeName: personTypeName,
                    SSN: SSN,
                    participantId: participantId,
                    payeecode: payeecode,
                    type: Type,
                    fidExists: fidExists,
                    awardExist: awardExist,
                    pendingClaimsExists: pendingClaimsExists
                };

                if (i == 0) {
                    var newTH = function (title, id) {
                        var th = document.createElement('th');
                        th.appendChild(document.createTextNode(title));
                        th.id = id;
                        th.title = title;
                        th.setAttribute("SCOPE", "col");
                        return th;
                    }

                    theadRow.appendChild(newTH('Name', 'name'));
                    theadRow.appendChild(newTH('Type', 'personType'));
                    theadRow.appendChild(newTH('DOB', 'dob'));
                    theadRow.appendChild(newTH('SSN', 'ssn'));
                    theadRow.appendChild(newTH('Participant Id', 'participantId'));
                    theadRow.appendChild(newTH('Payee Code', 'payeeCode'));
                    theadRow.appendChild(newTH('Award Type', 'awardType'));

                    thead.appendChild(theadRow);
                    table.append(thead);
                }

                // Table rows
                var row = document.createElement('tr');

                var newCell = function (content) {
                    var cell = document.createElement('td');
                    cell.appendChild(document.createTextNode(content));
                    cell.setAttribute("SCOPE", "row");
                    return cell;
                }

                var col1 = newCell(name);

                row.appendChild(col1);

                row.appendChild(newCell(personTypeName));
                row.appendChild(newCell(DOB));
                row.appendChild(newCell(SSN));
                row.appendChild(newCell(participantId));
                row.appendChild(newCell(payeecode));
                row.appendChild(newCell(Type));

                row.setAttribute('personid', personid);
                row.setAttribute('fidExists', fidExists);
                row.setAttribute('awardExist', awardExist);
                row.setAttribute('pendingClaimsExist', pendingClaimsExists);

                row.tabIndex = 100 + i;
                row.className = (i % 2 == 0) ? "even" : "odd";

                row.ondblclick = function () { Va.Udo.Crm.Scripts.PeopleList.Select(this); };

                row.onkeypress = function (e) {
                    if (e.keyCode === 13 || e.keyCode === 32) {
                        Va.Udo.Crm.Scripts.PeopleList.Select(this);
                    }
                }

                var disabled = false;
                var valid = true;
                if ($.inArray(411, Config.SelectionTypes) == -1) {
                    var num = Config.SelectionTypes[0].toString().replace(/\D/g, '');
                    if (Config.SelectionTypes.length == 1) {
                        if (Config.SelectionTypes[0].toString().length != num.toString().length) {
                            //not num, must be expression
                            valid = eval(Config.SelectionTypes[0]);
                        }
                        if (Type != null) {
                            if (Type == "BUR" || Type == "ACC") {
                                valid = false;
                            }
                        }
                    }
                    if ((num.length > 0 && $.inArray(personType, Config.SelectionTypes) == -1) || !valid) {
                        // not selectable
                        disabled = true;
                        row.className += " disabled";
                        $(row).find("td:first").append(" (not selectable)");
                        row.ondblclick = null;
                        row.onkeypress = null;
                    }

                    if (Config.FilterSelection) Config.FilterSelection(row, cleanData);
                }


                row.title = "Name - " + name + ", Type - " + personTypeName + ", DOB - " + DOB +
                    ", SSN - " + SSN + ", Participant Id - " + participantId +
                    ", Payee Code - " + payeecode + ", Award Type - " + Type;
                col1.title = row.title;

                if (!disabled) {
                    row = $(row);

                    row.hover(function () {
                        $(this).addClass("hover");
                    }, function () {
                        $(this).removeClass("hover");
                    });
                }

                table.append(row);

                $("#resultsFieldSetDiv").show();
                if (i == 1)
                    row.focus = true;
            }
        }
        else {
            $("#notFoundDiv").show();
        }
        var searchMessage = $("#searchResultsMessageInput");
        if (data != null && data.length > 0) {
            searchMessage.val(data.length + " records found.");
        }
        else {
            searchMessage.val("No valid people found.");
        }
        $("#searchResultsMessageInput").show();
        var fieldLength = searchMessage.val().length;
        searchMessage.attr('size', fieldLength);
        Va.Udo.Crm.Scripts.PeopleList.InitializeStatus = "Initialized";
        $('div#tmpDialog').hide();
    }

    /*function getPeople_complete() {
        $('div#tmpDialog').hide();
    }*/

    execute();
}
