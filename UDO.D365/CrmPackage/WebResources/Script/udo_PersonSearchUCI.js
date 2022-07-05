window["ENTITY_SET_NAMES"] = window["ENTITY_SET_NAMES"] || JSON.stringify({
    "crme_person": "crme_persons",
    "systemuser": "systemusers",
    "udo_idproof": "udo_idproofs",
    "udo_interaction": "udo_interactions"
});

//CRM Ctrl Key Fix for toString Error
if (typeof window.LOCID_JUMP_TO_RIBBON == "undefined") window.LOCID_JUMP_TO_RIBBON = "[";
if (typeof window.LOCID_JUMP_TO_RIBBON_CONTROL == "undefined") window.top.LOCID_JUMP_TO_RIBBON_CONTROL = "]";

var idProofId = null;
var veteranId = null;
var interactionId = null;
var chatSessionLogId = null;
var selectAnotherVet = false;
var ani = null;
var ctissn = null;
var ctiedipi = null;
var ctidob = null;
var sessionid = null;
var conversationid = null;

function formatExecutingSearch() {
    //if ((window.IsUSD === true) || (parent.window.IsUSD === true)) {
    window.open("http://uii/Global Manager/CopyToContext?sessionid=" + sessionid);
    //window.open("http://event/?eventName=VetSearchBegin&sessionid=" + sessionid);
    window.open("http://event/?eventName=VetSearchBegin");
    //}

    $('div#tmpDialog').show();
    $('div#tmpDialog').focus();
    $("#validationFailedDiv").hide();
    $("#resultsFieldSetDiv").hide();
    $("#notFoundDiv").hide();
    $("#birlsSearchDiv").hide();
    $("#searchResultsMessageInput").hide();
    $("#altSearchButton").attr('disabled', true);
    $("#SearchByNameButton").attr('disabled', true);
    $("#SearchByIdentifierButton").attr('disabled', true);
    $("#SearchBirlsButton").attr('disabled', true);
    $('#idProofCompleteButton').attr('disabled', true);
    $("#idProofVeteranCompleteButton").attr('disabled', true);
    $('#idProofFailedButton').attr('disabled', true);
    $("#clearNameFieldsButton").attr('disabled', true);
    $("#clearIdentifierFieldsButton").attr('disabled', true);
    $("#ResetSearchBirlsButton").attr('disabled', true);
    $("#idProofCompleteButton").show();
    $("#idProofVeteranCompleteButton").show();
    $("#idProofFailedButton").show();
}

function onIdProofCheckboxTextClick(id) {
    var checkbox = document.getElementById(id);
    checkbox.checked = !checkbox.checked;
    checkbox.onchange();
}
// Error Messages
var ErrorMessage = function () {
    function error(message, title, options) {
        return popup(message, title, Va.Udo.Crm.Scripts.Popup.PopupStyles.Critical, options);
    }
    function popup(message, title, style, options) {
        if (!title) title = "Error";
        if (!options) options = { height: 200, width: 350 };
        return Va.Udo.Crm.Scripts.Popup.MsgBox(message, style, title, options);
    }

    var messages =
    {
        Message: error,
        PeopleSearch: function (message, title, options) { error("There was an error performing the search.\n\nError:" + message, (title ? title : "People Search Error"), options); },
        Access: function (message, title, options) { error("There was an access error.\n\nError:" + message, (title ? title : "Access Error"), options); },
        IDProof: function (message, title, options) { error(message, (title ? title : "ID Proof Error"), options); },
        Interaction: function (message, title, options) { error(message, (title ? title : "Interaction Error"), options); },
        Warning: function (message, title, options) { popup(message, (title ? title : "Warning"), Va.Udo.Crm.Scripts.Popup.PopupStyles.Exclamation, options); },
        ChatSearch: function (message, title, options) { error("There was an error performing the search.\n\nError:" + message, (title ? title : "Chat Search Error"), options); }
    };

    return messages;
}();

// Number Cleaner ---------------------------------------------------------------
function SetupNumberInputCleaner(jqEl, maxlength) {
    function CleanNumberInput(jqEl, maxlength) {
        var val = jqEl.val();
        val = val.replace(/\D/g, '');
        if (val.length > maxlength) val = val.substring(0, maxlength);
        jqEl.val(val);
    }

    jqEl.keypress(function (e) {
        if (!e) var e = window.event;
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    });
    jqEl.change(function () {
        CleanNumberInput($(this), maxlength);
    });
    jqEl.keyup(function (e) {
        if (e.which == 86) { // Number only input, so if lowercase V was pressed, then clean it on the keyup.
            CleanNumberInput($(this), maxlength);
            //$this = $(this);
            //setTimeout(function() { CleanNumberInput($this, maxlength); },5);
        }
    });
}
// End Number Cleaner ===========================================================

$(document).ready(function () {
    //do not submit form to server with GET
    $("form").bind("submit", function (e) {
        e = e || window.event;
        if (e.preventDefault) { e.preventDefault(); } else { e.returnValue = false; }
    });
    $(".forminputtext").attr("autocomplete", "off");
    $("#altSearchButton").attr('disabled', true);
    SetupNumberInputCleaner($("#SocialSecurityTextBox"), 9);
    SetupNumberInputCleaner($("#SocialSecurityBirlsTextBox"), 9);
    GetQueryParams();
    //if there is a Chat Session Log Id passed with the query parameters, then will auto launch search process
    GetChatDetailsIfAvailable();

    performSearchUsingCTIParameters();

    $('#SocialSecurityTextBox').focus();

    //hit the search under the fields area - attended search
    $("#altSearchButton").bind("click", function () {
        $("#SocialSecurityBirlsTextBox").val($("#SocialSecurityTextBox").val());
        $("#FirstNameBirlsTextBox").val($("#FirstNameTextBox").val());
        $("#LastNameBirlsTextBox").val($("#LastNameTextBox").val());

        var dobyear = $("#BirthYearTextBox").val();
        var dobmonth = $("#BirthMonthTextBox").val();
        var dobday = $("#BirthDayTextBox").val();
        var dob = dobmonth + "/" + dobday + "/" + dobyear;

        $("#DobBirlsTextBox").val(dob);
        $("#birlsSearchDiv").show();
        $("#idProofCompleteButton").hide();
        $("#idProofVeteranCompleteButton").hide();
        $("#idProofFailedButton").hide();
    });

    $("#SearchByNameButton").bind("click", function () {
        $("#searchResultsMessageInput").val("");

        validateSearchByName().done(function () {

            formatExecutingSearch();

            var dobday = $("#BirthDayTextBox").val() == "dd" ? "" : $("#BirthDayTextBox").val();
            var dobyear = $("#BirthYearTextBox").val() == "yyyy" ? "" : $("#BirthYearTextBox").val();
            var dobmonth = $("#BirthMonthTextBox").val() == "mm" ? "" : $("#BirthMonthTextBox").val();
            var dob = dobmonth + "/" + dobday + "/" + dobyear;
            var ssn = $("#SocialSecurityTextBox").val();
            ssn = ssn.replace(/\D/g, ""); // rescrub just incase

            idProofId = null;
            veteranId = null;
            var filter = "?$filter=";
            filter += buildQueryFilter("crme_firstname", $("#FirstNameTextBox").val(), false);
            filter += buildQueryFilter("crme_lastname", $("#LastNameTextBox").val(), true);
            filter += buildQueryFilter("crme_searchtype", 'CombinedSearchByFilter', true);
            if (interactionId != null)
                filter += buildQueryFilter("crme_udointeractionid", interactionId, true);
            filter += " and crme_isattended eq true";

            if (dobyear != "") {
                filter += " and crme_dobstring eq '" + dob + "'";
            }
            if ($("#SocialSecurityTextBox").val() != "") {
                filter += buildQueryFilter("crme_ssn", ssn, true);
            }

            Xrm.WebApi.retrieveMultipleRecords("crme_person", filter)
                .then(
                    function reply(response) {
                        personSearchCallBack(response.value);
                        personSearchComplete();
                    }
                );
        })
            .fail(function () {
                formatValidationFailed();
            });
    });

    $("#SearchByIdentifierButton").bind("click", function () {

        if (validateSearchByIdentifier() == true) {

            formatExecutingSearch();

            if ($("#EdipiTextBox").val() != "") {
                var filter = "?$filter="
                filter += buildQueryFilter("crme_edipi", $("#EdipiTextBox").val(), false);
                filter += buildQueryFilter("crme_classcode", 'MIL', true);
            }
            if (interactionId != null)
                filter += buildQueryFilter("crme_udointeractionid", interactionId, true);
            filter += buildQueryFilter("crme_searchtype", 'CombinedSearchByIdentifier', true);
            filter += " and crme_isattended eq false";

            Xrm.WebApi.retrieveMultipleRecords("crme_person", filter)
                .done(
                    function reply(response) {
                        personSearchCallBack(response.value);
                        personSearchComplete();
                    }
                );

        } else {
            formatValidationFailed();
        }
    });

    $('#idProofCompleteButton').bind("click", function () {
        var table = $("#personSearchResultsTable");
        var veteranResult = table.find("[fullName]");
        var rowNum = 0;
        openSelectedPerson(veteranResult[rowNum], false);
    });

    $('#idProofVeteranCompleteButton').bind("click", function () {
        var table = $("#personSearchResultsTable");
        var veteranResult = table.find("[fullName]");
        var rowNum = 0;
        if (interactionId === "00000000-0000-0000-0000-000000000000") {
            ErrorMessage
                .Interaction("There is no Interaction loaded. Please close this Session and try again.");
            $('div#tmpDialog').hide();
            return;
        }

        var interaction = {};
        if (veteranResult[rowNum]) {
            interaction.udo_firstname = veteranResult[rowNum].getAttribute("firstName") == null ? "" : veteranResult[rowNum].getAttribute("firstName");
            interaction.udo_lastname = veteranResult[rowNum].getAttribute("lastName") == null ? "" : veteranResult[rowNum].getAttribute("lastName");
            interaction.udo_phonenumber = veteranResult[rowNum].getAttribute("phoneNumber") == null ? "" : veteranResult[rowNum].getAttribute("phoneNumber");
            interaction.udo_relationship = 752280000;
            interaction.udo_ani = ani == null ? "" : ani;
            interaction.udo_recordsource = veteranResult[rowNum].getAttribute("recordSource") == null ? "" : veteranResult[rowNum].getAttribute("recordSource");

            var passingparms = {};
            if (veteranResult[rowNum]) {
                passingparms
                    .sessionid = sessionid == null
                        ? ""
                        : sessionid;
                passingparms
                    .udo_IdProofId = veteranResult[rowNum].getAttribute('udoidproofid') == null
                        ? ""
                        : veteranResult[rowNum].getAttribute('udoidproofid');
                passingparms
                    .udo_veteranid = veteranResult[rowNum].getAttribute('veteranId') == null
                        ? ""
                        : veteranResult[rowNum].getAttribute('veteranId');
                passingparms
                    .udo_FirstName = veteranResult[rowNum].getAttribute("firstName") == null
                        ? ""
                        : veteranResult[rowNum].getAttribute("firstName");
                passingparms
                    .udo_LastName = veteranResult[rowNum].getAttribute("lastName") == null
                        ? ""
                        : veteranResult[rowNum].getAttribute("lastName");
                passingparms
                    .udo_PhoneNumber = veteranResult[rowNum].getAttribute("phoneNumber") == null
                        ? ""
                        : veteranResult[rowNum].getAttribute("phoneNumber");
                passingparms
                    .udo_FileNumber = veteranResult[rowNum].getAttribute("fileNumber") == null
                        ? ""
                        : veteranResult[rowNum].getAttribute("fileNumber");
                passingparms.udo_recordsource = veteranResult[rowNum].getAttribute("recordSource") == null ? "" : veteranResult[rowNum].getAttribute("recordSource");
            }

            Xrm.WebApi.updateRecord("udo_interaction", interactionId, interaction)
                .then(
                    function reply(response) {
                        //Send interaction to context and refresh if selecting to ID Caller as Veteran
                        if (window.IsUSD == true) {
                            //window.open("http://uii/Global Manager/CopyToContext?sessionid=" + sessionid + "&RelationshipToVeteran=953850000&vetId=" + passingparms.udo_veteranid + "&idProofId=" + passingparms.udo_IdProofId + "&firstName=" + passingparms.udo_FirstName + "&lastName=" + passingparms.udo_LastName + "&phoneNumber=" + passingparms.udo_PhoneNumber + "&vetFn=" + passingparms.udo_FileNumber + "&recordsource=" + passingparms.udo_recordsource);

                            window.open("http://uii/Global Manager/CopyToContext?sessionid=" + sessionid);
                            window.open("http://uii/Global Manager/CopyToContext?RelationshipToVeteran=" + 953850000);
                            window.open("http://uii/Global Manager/CopyToContext?veteranid=" + passingparms.udo_veteranid);
                            window.open("http://uii/Global Manager/CopyToContext?idproofid=" + passingparms.udo_IdProofId);
                            window.open("http://uii/Global Manager/CopyToContext?firstName=" + passingparms.udo_FirstName);
                            window.open("http://uii/Global Manager/CopyToContext?lastName=" + passingparms.udo_LastName);
                            window.open("http://uii/Global Manager/CopyToContext?phoneNumber=" + passingparms.udo_PhoneNumber);
                            window.open("http://uii/Global Manager/CopyToContext?vetFn=" + passingparms.udo_FileNumber);
                            window.open("http://uii/Global Manager/CopyToContext?recordsource=" + passingparms.udo_recordsource);

                            setTimeout(function () { 
                                window.open("http://event/?eventName=IDProofComplete");
                            }, 1000);

                            //window.open("http://event/?eventName=IDProofComplete&sessionid=" + sessionid + "&RelationshipToVeteran=953850000&vetId=" + passingparms.udo_veteranid + "&idProofId=" + passingparms.udo_IdProofId + "&firstName=" + passingparms.udo_FirstName + "&lastName=" + passingparms.udo_LastName + "&phoneNumber=" + passingparms.udo_PhoneNumber + "&vetFn=" + passingparms.udo_FileNumber + "&recordsource=" + passingparms.udo_recordsource);
                        }
                    });
        }

        openSelectedPerson(veteranResult[rowNum], true);
    });

    $('#idProofFailedButton').bind("click", function () {
        var table = $("#personSearchResultsTable");
        var veteranResult = table.find("[fullName]");
        var rowNum = 0;
        openSelectedPerson(veteranResult[rowNum]);
    });

    $("#SearchBirlsButton").bind("click", function () {

        $("#searchResultsMessageInput").val("");

        if (validateSearchByAlternate() == true) {
            formatExecutingBirlsSearch();
            var ssn = $("#SocialSecurityBirlsTextBox").val();
            ssn = ssn.replace(/\D/g, '');  // Rescrub just incase.

            var dob = $("#DobBirlsTextBox").val() == "MM/DD/YYY" ? "" : $("#DobBirlsTextBox").val();

            var dateOfDeath = $("#DateOfDeathBirlsTextBox").val() == "MM/DD/YYY" ? "" : $("#DateOfDeathBirlsTextBox").val();
            var enteredOnDutyDate = $("#EnteredOnDutyBirlsTextBox").val() == "MM/DD/YYY" ? "" : $("#EnteredOnDutyBirlsTextBox").val();
            var releasedActiveDutyDate = $("#ReleasedActiveDutyBirlsTextBox").val() == "MM/DD/YYY" ? "" : $("#ReleasedActiveDutyBirlsTextBox").val();

            var filter = "?$filter=";
            filter += buildQueryFilter("crme_firstname", $("#FirstNameBirlsTextBox").val(), false);
            filter += buildQueryFilter("crme_lastname", $("#LastNameBirlsTextBox").val(), true);
            filter += buildQueryFilter("crme_ssn", ssn, true);
            filter += buildQueryFilter("crme_dobstring", dob, true);
            if ($("#BranchOfServiceBirlsTextBox").val() != "") filter += buildQueryFilter("crme_branchofservice", $("#BranchOfServiceBirlsTextBox").val(), true);
            if ($("#ServiceNumberBirlsTextBox").val() != "") filter += buildQueryFilter("crme_servicenumber", $("#ServiceNumberBirlsTextBox").val(), true);
            if ($("#InsuranceNumberBirlsTextBox").val() != "") filter += buildQueryFilter("crme_insurancenumber", $("#InsuranceNumberBirlsTextBox").val(), true);
            if (dateOfDeath != "") filter += buildQueryFilter("crme_deceaseddate", dateOfDeath, true);
            if (enteredOnDutyDate != "") filter += buildQueryFilter("crme_enteredondutydate", enteredOnDutyDate, true);
            if (releasedActiveDutyDate != "") filter += buildQueryFilter("crme_releasedactivedutydate", releasedActiveDutyDate, true);
            if ($("#SuffixBirlsTextBox").val() != "") filter += buildQueryFilter("crme_suffix", $("#SuffixBirlsTextBox").val(), true);
            if ($("#PayeeNumberBirlsTextBox").val() != "") filter += buildQueryFilter("crme_payeenumber", $("#PayeeNumberBirlsTextBox").val(), true);
            if ($("#FolderLocationBirlsTextBox").val() != "") filter += buildQueryFilter("crme_folderlocation", $("#FolderLocationBirlsTextBox").val(), true);

            filter += buildQueryFilter("crme_searchtype", 'SearchByBirls', true);
            if (interactionId != null)
                filter += buildQueryFilter("crme_udointeractionid", interactionId, true);
            filter += " and crme_isattended eq true";

            Xrm.WebApi.retrieveMultipleRecords("crme_person", filter)
                .then(
                    function reply(response) {
                        personSearchCallBack(response.value);
                        personSearchComplete();
                    });
        } else {
            formatValidationFailedAlternateSearch();
        }
    });

    $('#clearIdentifierFieldsButton').bind("click", function () {
        if (window.IsUSD == true) {
            window.open("http://uii/Global Manager/CopyToContext?sessionid=" + sessionid);
            window.open("http://event/?eventName=VetSearchBegin");
            //window.open("http://event/?eventName=VetSearchBegin&sessionid=" + sessionid);
        }
        $("#EdipiTextBox").val("");
        $("#validationFailedDiv").hide();
        $('div#tmpDialog').hide();
        $("#altSearchButton").attr('disabled', true);
        $("#SearchByNameButton").attr('disabled', false);
        $("#SearchByIdentifierButton").attr('disabled', false);
        $("#birlsSearchDiv").hide();
        var table = $("#personSearchResultsTable");
        table.find("thead, tr, th").remove();
        table.find("tr:gt(0)").remove();
        $("#resultsFieldSetDiv").hide();
    });

    $('#clearNameFieldsButton').bind("click", function () {
        if (window.IsUSD == true) {
            window.open("http://uii/Global Manager/CopyToContext?sessionid=" + sessionid);
            window.open("http://event/?eventName=VetSearchBegin");
            //window.open("http://event/?eventName=VetSearchBegin&sessionid=" + sessionid);
        }
        $("#FirstNameTextBox").val("");
        $("#MiddleNameTextBox").val("");
        $("#LastNameTextBox").val("");
        $("#BirthMonthTextBox").val("");
        $("#BirthDayTextBox").val("");
        $("#BirthYearTextBox").val("");
        //$("#PhoneNoTextBox").val("");
        $("#SocialSecurityTextBox").val("");
        $("#validationFailedDiv").hide();
        $("#notFoundDiv").hide();
        $("#birlsSearchDiv").hide();

        $('div#tmpDialog').hide();
        $("#altSearchButton").attr('disabled', true);
        $("#SearchByNameButton").attr('disabled', false);
        $("#SearchByIdentifierButton").attr('disabled', false);
        $("#SearchBirlsButton").attr('disabled', false);
        var table = $("#personSearchResultsTable");
        table.find("thead, tr, th").remove();
        table.find("tr:gt(0)").remove();
        $("#resultsFieldSetDiv").hide();
    });

    $("#ResetSearchBirlsButton").bind("click", function () {
        if (window.IsUSD == true) {
            window.open("http://uii/Global Manager/CopyToContext?sessionid=" + sessionid);
            window.open("http://event/?eventName=VetSearchBegin");
        }
        $("#validationFailedDiv").hide();
        $('div#tmpDialog').hide();
        $("#SearchBirlsButton").attr('disabled', false);
        $("#notFoundDiv").hide();
        var table = $("#personSearchResultsTable");
        table.find("thead, tr, th").remove();
        table.find("tr:gt(0)").remove();
        $("#resultsFieldSetDiv").hide();
    });

    //$('#createNewVeteranButton').bind("click", createNewVeteran);

    function formatValidationFailed() {
        $("#validationFailedDiv").show();
        $("#validationFailedDiv").focus();
        $("#notFoundDiv").hide();
        $("#birlsSearchDiv").hide();
        $("#resultsFieldSetDiv").hide();
        $("#searchResultsMessageInput").hide();
        $("#personSearchResultsTable").find("thead, tr, th").remove();
        $("#personSearchResultsTable").find("tr:gt(0)").remove();
    }

    function formatValidationFailedAlternateSearch() {
        $("#validationFailedDiv").show();
        $("#validationFailedDiv").focus();
        $("#resultsFieldSetDiv").hide();
        $("#searchResultsMessageInput").hide();
        $("#personSearchResultsTable").find("thead, tr, th").remove();
        $("#personSearchResultsTable").find("tr:gt(0)").remove();
    }

    function formatExecutingBirlsSearch() {
        if (window.IsUSD == true) {
            window.open("http://uii/Global Manager/CopyToContext?sessionid=" + sessionid);
            window.open("http://event/?eventName=VetSearchBegin");
        }
        $('div#tmpDialog').show();
        $('div#tmpDialog').focus();
        $("#validationFailedDiv").hide();
        $("#resultsFieldSetDiv").hide();
        $("#searchResultsMessageInput").hide();
        $("#altSearchButton").attr('disabled', true);
        $("#SearchByNameButton").attr('disabled', true);
        $("#SearchByIdentifierButton").attr('disabled', true);
        $("#SearchBirlsButton").attr('disabled', true);
        $('#idProofCompleteButton').attr('disabled', true);
        $('#idProofVeteranCompleteButton').attr('disabled', true);
        $('#idProofFailedButton').attr('disabled', true);
        $("#notFoundDiv").show();
    }

    function personSearchComplete() {
        $('div#tmpDialog').hide();
        $("#altSearchButton").attr('disabled', false);
        $("#SearchByNameButton").attr('disabled', false);
        $("#SearchByIdentifierButton").attr('disabled', false);
        $("#SearchBirlsButton").attr('disabled', false);
        $("#clearNameFieldsButton").attr('disabled', false);
        $("#clearIdentifierFieldsButton").attr('disabled', false);
        $("#ResetSearchBirlsButton").attr('disabled', false);
        //$('#idProofCompleteButton').attr('disabled', false);
        if ($("#idProofVeteranCompleteButton").is(":visible"))
            //$("#idProofVeteranCompleteButton").focus();
            $("#personSearchResultsTable").focus();
        else if ($("#idProofCompleteButton").is(":visible"))
            //$("#idProofCompleteButton").focus();
            $("#personSearchResultsTable").focus();
        else
            $("#searchResultsMessageInput").focus();
    }

    function chatSearchComplete() {
        //$('div#tmpDialog').hide();
    }

    function setFocus(ctrl, focus) {
        if (ctrl.val().length == 2) {
            document.getElementById(focus).focus();
        }
    }

    function buildQueryFilter(field, value, and) {
        if (and) {
            return " and " + field + " eq '" + value + "'";
        } else {
            return field + " eq '" + value + "'";
        }
    }

    function openSelectedPersonUCI(obj, idCallerAsVeteran) {
        $('#idProofCompleteButton').attr('disabled', true);
        $('#idProofVeteranCompleteButton').attr('disabled', true);
        $('#idProofFailedButton').attr('disabled', true);
        if (obj == null) {
            $('#idProofCompleteButton').attr('disabled', false);
            $('#idProofVeteranCompleteButton').attr('disabled', false);
            $('#idProofFailedButton').attr('disabled', false);
            return;
        }
        $('div#tmpDialog').show();
        $('div#tmpDialog').focus();

        //start BG code

        Xrm.Utility.getGlobalContext().then(
            function (settings) {
                var userid = settings.userSettings.userId;
                var formatedUserId = userid.replace(/}/gi, "").replace(/{/gi, "");

                var filter = "?$select=_businessunitid_value, va_filenumber&$expand=businessunitid($select=udo_veteransensitivitylevel)&$filter=systemuserid eq " + formatedUserId;

                Xrm.WebApi.retrieveMultipleRecords("systemuser", filter)
                    .then(
                        function reply(response) {
                            if ((response != null) && (response.value != null)) {
                                record = response.value[0];

                                var uSsn = record.va_filenumber;
                                userSL = record.businessunitid.udo_veteransensitivitylevel;
                                userSL = userSL - 752280000;
                                var ssn = obj.getAttribute("ssn");
                                if (uSsn === ssn) {

                                    $('div#tmpDialog').hide();
                                    ErrorMessage.Access("You do not have access to view your own records.");
                                }
                                else {
                                    var url = obj.getAttribute('crme_url');
                                    var udoidproofid = obj.getAttribute('udoidproofid');

                                    if (udoidproofid == "00000000-0000-0000-0000-000000000000") {
                                        ErrorMessage.IDProof("Failed to update the ID Proof status. Please try the search and ID Proof again.");
                                        $('div#tmpDialog').hide();
                                        return;
                                    }
                                    var firstnameVerified = obj.getAttribute('crme_firstnameverified');
                                    var lastnameVerified = obj.getAttribute('crme_lastnameverified');
                                    var dobVerified = obj.getAttribute('crme_dobverified');
                                    var ssidVerified = obj.getAttribute('crme_ssnverified');
                                    var bosVerified = obj.getAttribute('crme_bosverified');
                                    var udoidproofid = obj.getAttribute('udoidproofid');


                                    var idproof = {};

                                    idproof.udo_verifieddob = dobVerified;
                                    idproof.udo_verifiedfirstname = firstnameVerified;
                                    idproof.udo_verifiedlastname = lastnameVerified;
                                    idproof.udo_verifiedbranchofservice = bosVerified;
                                    idproof.udo_verifiedssn = ssidVerified;
                                    idproof.udo_idproofcomplete = true;

                                    Xrm.WebApi.updateRecord("udo_idproof", udoidproofid, idproof)
                                        .then(
                                            function reply(response) {
                                                console.log(response);
                                            });


                                    var parmFileNumber = obj.getAttribute("fileNumber") == null ? "" : obj.getAttribute("fileNumber");
                                    var interaction = {};

                                    if (ani) {
                                        interaction.udo_ani = ani;
                                    }

                                    interaction.udo_recordsource = obj.getAttribute("recordSource") == null ? "" : obj.getAttribute("recordSource");

                                    if (idCallerAsVeteran === true) {
                                        if (window.IsUSD == true) {
                                            setTimeout(function () {
                                                window.open("http://event/?eventName=InteractionUpdateForVetCaller");
                                            }, 1000);
                                        }
                                    }

                                    var interaction = {};
                                    if (obj) {
                                        //If IdCallerAsVeteran button clicked, the InitiateCallerId event is fired from USD in the InteractionUpdateForVetCaller event
                                        //The USD listener appears to have sporadic problems with back to back events as executed from window.open.
                                        $('div#tmpDialog').hide();
                                    }
                                }
                            }

                        });
                //end BG code

            },
            function () {
                console.log("KJ");
            });

        return false;
    }


    function openSelectedPerson(obj, idCallerAsVeteran) {
        $('#idProofCompleteButton').attr('disabled', true);
        $('#idProofVeteranCompleteButton').attr('disabled', true);
        $('#idProofFailedButton').attr('disabled', true);
        if (obj == null) {
            $('#idProofCompleteButton').attr('disabled', false);
            $('#idProofVeteranCompleteButton').attr('disabled', false);
            $('#idProofFailedButton').attr('disabled', false);
            return;
        }
        $('div#tmpDialog').show();
        $('div#tmpDialog').focus();

        //start BG code

        Xrm.Utility.getGlobalContext().then(
            function (settings) {
                var userid = settings.userSettings.userId;
                var formatedUserId = userid.replace(/}/gi, "").replace(/{/gi, "");

                var filter = "?$select=_businessunitid_value, va_filenumber&$expand=businessunitid($select=udo_veteransensitivitylevel)&$filter=systemuserid eq " + formatedUserId;

                Xrm.WebApi.retrieveMultipleRecords("systemuser", filter)
                    .then(
                        function reply(response) {
                            if ((response != null) && (response.value != null)) {
                                record = response.value[0];

                                var uSsn = record.va_filenumber;
                                userSL = record.businessunitid.udo_veteransensitivitylevel;
                                userSL = userSL - 752280000;
                                var ssn = obj.getAttribute("ssn");
                                if (uSsn === ssn) {

                                    $('div#tmpDialog').hide();
                                    ErrorMessage.Access("You do not have access to view your own records.");
                                }
                                else {
                                    var url = obj.getAttribute('crme_url');
                                    var udoidproofid = obj.getAttribute('udoidproofid');

                                    if (udoidproofid == "00000000-0000-0000-0000-000000000000") {
                                        ErrorMessage.IDProof("Failed to update the ID Proof status. Please try the search and ID Proof again.");
                                        $('div#tmpDialog').hide();
                                        return;
                                    }
                                    var firstnameVerified = obj.getAttribute('crme_firstnameverified');
                                    var lastnameVerified = obj.getAttribute('crme_lastnameverified');
                                    var dobVerified = obj.getAttribute('crme_dobverified');
                                    var ssidVerified = obj.getAttribute('crme_ssnverified');
                                    var bosVerified = obj.getAttribute('crme_bosverified');
                                    var udoidproofid = obj.getAttribute('udoidproofid');


                                    var idproof = {};

                                    idproof.udo_verifieddob = dobVerified;
                                    idproof.udo_verifiedfirstname = firstnameVerified;
                                    idproof.udo_verifiedlastname = lastnameVerified;
                                    idproof.udo_verifiedbranchofservice = bosVerified;
                                    idproof.udo_verifiedssn = ssidVerified;
                                    idproof.udo_idproofcomplete = true;

                                    Xrm.WebApi.updateRecord("udo_idproof", udoidproofid, idproof)
                                        .then(
                                            function reply(response) {
                                                console.log(response);
                                            });


                                    var parmFileNumber = obj.getAttribute("fileNumber") == null ? "" : obj.getAttribute("fileNumber");
                                    var interaction = {};

                                    if (ani) {
                                        interaction.udo_ani = ani;
                                    }

                                    interaction.udo_recordsource = obj.getAttribute("recordSource") == null ? "" : obj.getAttribute("recordSource");

                                    Xrm.WebApi.updateRecord("udo_interaction", interactionId, interaction)
                                        .then(
                                            function () {
                                                //Send interaction to context and refresh if selecting to ID Caller as Veteran
                                                if (idCallerAsVeteran === true) {
                                                    if (window.IsUSD == true) {
                                                        window.open("http://uii/Global Manager/CopyToContext?sessionid=" + sessionid);
                                                        window.open("http://uii/Global Manager/CopyToContext?firstName=" + interaction.udo_FirstName);
                                                        window.open("http://uii/Global Manager/CopyToContext?lastName=" + interaction.udo_LastName);
                                                        window.open("http://uii/Global Manager/CopyToContext?phoneNumber=" + interaction.udo_PhoneNumber);
                                                        window.open("http://uii/Global Manager/CopyToContext?vetFn=" + parmFileNumber);
                                                        window.open("http://uii/Global Manager/CopyToContext?recordsource=" + interaction.udo_recordsource);

                                                        //window.open("http://uii/Global Manager/CopyToContext?sessionid=" + sessionid + "&firstName=" + interaction.udo_FirstName + "&lastName=" + interaction.udo_LastName + "&phoneNumber=" + interaction.udo_PhoneNumber + "&vetFn=" + parmFileNumber + "&recordsource=" + interaction.udo_recordsource);
                                                        setTimeout(function () { 
                                                            window.open("http://event/?eventName=InteractionUpdateForVetCaller");
                                                        }, 1000);
                                                    }
                                                }
                                            });

                                    var interaction = {};
                                    if (obj) {
                                        interaction
                                            .udo_IdProofId = obj.getAttribute('udoidproofid') == null
                                                ? ""
                                                : obj.getAttribute('udoidproofid');
                                        interaction
                                            .udo_veteranid = obj.getAttribute('veteranId') == null
                                                ? ""
                                                : obj.getAttribute('veteranId');
                                        interaction
                                            .udo_FirstName = obj.getAttribute("firstName") == null
                                                ? ""
                                                : obj.getAttribute("firstName");
                                        interaction
                                            .udo_LastName = obj.getAttribute("lastName") == null
                                                ? ""
                                                : obj.getAttribute("lastName");
                                        interaction
                                            .udo_PhoneNumber = obj.getAttribute("phoneNumber") == null
                                                ? ""
                                                : obj.getAttribute("phoneNumber");
                                        interaction
                                            .udo_FileNumber = obj.getAttribute("fileNumber") == null
                                                ? ""
                                                : obj.getAttribute("fileNumber");
                                        interaction.udo_recordsource = obj.getAttribute("recordSource") == null ? "" : obj.getAttribute("recordSource");


                                        //If IdCallerAsVeteran button clicked, the InitiateCallerId event is fired from USD in the InteractionUpdateForVetCaller event
                                        //The USD listener appears to have sporadic problems with back to back events as executed from window.open.
                                        if (idCallerAsVeteran !== true)
                                            if (window.IsUSD == true) {
                                                window.open("http://uii/Global Manager/CopyToContext?sessionid=" + sessionid);
                                                window.open("http://uii/Global Manager/CopyToContext?RelationshipToVeteran=" + "");
                                                window.open("http://uii/Global Manager/CopyToContext?veteranid=" + interaction.udo_veteranid);
                                                window.open("http://uii/Global Manager/CopyToContext?idproofid=" + interaction.udo_IdProofId);
                                                window.open("http://uii/Global Manager/CopyToContext?firstName=" + interaction.udo_FirstName);
                                                window.open("http://uii/Global Manager/CopyToContext?lastName=" + interaction.udo_LastName);
                                                window.open("http://uii/Global Manager/CopyToContext?phoneNumber=" + interaction.udo_PhoneNumber);
                                                window.open("http://uii/Global Manager/CopyToContext?vetFn=" + interaction.udo_FileNumber);
                                                window.open("http://uii/Global Manager/CopyToContext?recordsource=" + interaction.udo_recordsource);

                                                //window.open("http://uii/Global Manager/CopyToContext?sessionid=" + sessionid + "&RelationshipToVeteran=&vetId=" + interaction.udo_veteranid + "&idProofId=" + interaction.udo_IdProofId + "&firstName=" + interaction.udo_FirstName + "&lastName=" + interaction.udo_LastName + "&phoneNumber=" + interaction.udo_PhoneNumber + "&vetFn=" + interaction.udo_FileNumber + "&recordsource=" + interaction.udo_recordsource);
                                                setTimeout(function () { 
                                                    window.open("http://event/?eventName=IDProofComplete");
                                                }, 1000);
                    
                                            }
                                        $('div#tmpDialog').hide();
                                    }
                                }
                            }

                        });
                //end BG code

            },
            function () {
                console.log("KJ");
            });

        return false;
    }

    function personSearchCallBack(data) {
        $('div#tmpDialog').show();
        // get the table
        var table = $("#personSearchResultsTable");

        // reset the table by removing all data rows
        table.find("thead, tr, th").remove();
        table.find("tr:gt(0)").remove();
        $("#resultsFieldSetDiv").hide();

        if (data != null && data.length > 0) {
            if (data.length > 1) {
                $('#idProofFailedButton').attr('disabled', true);
                $('#idProofCompleteButton').attr('disabled', true);
                $('#idProofVeteranCompleteButton').attr('disabled', true);
            }
            else {
                $('#idProofCompleteButton').attr('disabled', false);
                //this is a query string parameter for the VSO-like flows in USD
                //Do not enable this button if VSO and selectAnotherVet is true
                if (selectAnotherVet === "true")
                    $('#idProofVeteranCompleteButton').attr('disabled', true);
                else
                    $('#idProofVeteranCompleteButton').attr('disabled', false);
            }

            var thead = document.createElement('thead');
            var theadRow = document.createElement('tr');
            var hasAlias = false;
            var hasDeceasedDate = false;
            var hasIdentityTheft = false;
            var hasServiceNumber = false;
            var hasInsuranceNumber = false;
            var hasEnteredOnDutyDate = false;
            var hasReleasedActiveDutyDate = false;
            var hasMiddleName = false;
            var hasSuffix = false;
            var hasPayeeNumber = false;
            var hasFolderLocation = false;
            for (var i = 0; i < data.length; i++) {
                if (data[i].crme_alias != null && data[i].crme_alias.trim() != "")
                    hasalias = true;
                if (data[i].crme_deceaseddate != null && data[i].crme_deceaseddate.trim() != "")
                    hasdeceaseddate = true;
                if (data[i].crme_identitytheft != null && data[i].crme_identitytheft.trim() != "")
                    hasidentitytheft = true;
                if (data[i].crme_servicenumber != null && data[i].crme_servicenumber.trim() != "")
                    hasservicenumber = true;
                if (data[i].crme_insurancenumber != null && data[i].crme_insurancenumber.trim() != "")
                    hasinsurancenumber = true;
                if (data[i].crme_enteredondutydate != null && data[i].crme_enteredondutydate.trim() != "")
                    hasenteredondutydate = true;
                if (data[i].crme_releasedactivedutydate != null && data[i].crme_releasedactivedutydate.trim() != "")
                    hasreleasedactivedutydate = true;
                if (data[i].crme_middlename != null && data[i].crme_middlename.trim() != "")
                    hasmiddlename = true;
                if (data[i].crme_suffix != null && data[i].crme_suffix.trim() != "")
                    hassuffix = true;
                if (data[i].crme_payeenumber != null && data[i].crme_payeenumber.trim() != "")
                    haspayeenumber = true;
                if (data[i].crme_folderlocation != null && data[i].crme_folderlocation.trim() != "")
                    hasfolderlocation = true;
            }

            for (var i = 0; i < data.length; i++) {

                var fullName = formatName(data[i]);
                //CRMe can return result, even if just ReturnMessage that there were no results...
                //If blank\no name, break
                if (fullName.trim() == "") {
                    $("#resultsFieldSetDiv").show();
                    //If CorpDb down, message could be: "BIRLS communication is down" or "The Tuxedo service is down" or "Communication to the back end services is currently down. Please try again."
                    //If MVI Down: A connection error was encountered performing the MVI search.
                    //Another MVI error: "An unknown error was returned from MVI. "
                    //If cannot communicate with VIMT (plugin gets null response), plugin will output:
                    //An error occured trying to process this request. Please try again.  If it continues to fail, please contact your administator
                    var returnMsg = (data != null && data.length > 0 && data[0].crme_returnmessage != null) ? data[0].crme_returnmessage : "Your search in MVI did not find any records matching the search criteria.";
                    if (returnMsg.indexOf("access violation") > -1 ||
                        returnMsg.indexOf("sensitive record check error") > -1 ||
                        returnMsg.indexOf("BIRLS communication is down") > -1 ||
                        returnMsg.indexOf("The Tuxedo service is down") > -1 ||
                        returnMsg.indexOf("An unknown error was encountered during the CORPDB search") > -1 ||
                        returnMsg.indexOf("An error occured trying to process this request") > -1 ||
                        returnMsg.indexOf("Communication to the back end services is currently down. Please try again.") > -1) {
                        //if (returnMsg.indexOf("access violation") > -1 || returnMsg.indexOf("sensitive record check error") > -1) {
                        $("#idProofCompleteButton").hide();
                        $("#idProofVeteranCompleteButton").hide();
                        $("#idProofFailedButton").hide();
                        //break from for-loop. After breaking, we show the Search Message text
                        //this will not show Alternate Search since we had an error of some sort during search
                        break;
                    }

                    $("#birlsSearchDiv").show();
                    $("#idProofCompleteButton").hide();
                    $("#idProofVeteranCompleteButton").hide();
                    $("#idProofFailedButton").hide();
                    //break from for-loop. After breaking, we show the Search Message text
                    break;
                }
                var dateOfBirth = data[i].crme_dobstring == null ? "" : data[i].crme_dobstring;
                var address = formatAddress(data[i]);
                var icn = data[i].crme_icn == null ? "" : data[i].crme_icn;
                var patientMviIdentifier = data[i].crme_patientmviidentifier == null ? "" : data[i].crme_patientmviidentifier;
                var recordSource = data[i].crme_recordsource == null ? "" : data[i].crme_recordsource;
                var edipi = data[i].crme_edipi == null ? "" : data[i].crme_edipi;
                var ssn = data[i].crme_ssn == null ? "" : data[i].crme_ssn;
                var firstName = data[i].crme_firstname == null ? "" : data[i].crme_firstname;
                var middleName = data[i].crme_middlename == null ? "" : data[i].crme_middlename;
                var lastName = data[i].crme_lastname == null ? "" : data[i].crme_lastname;
                var alias = data[i].crme_alias == null ? "" : data[i].crme_alias;
                var gender = data[i].crme_gender == null ? "" : data[i].crme_gender;
                var deceasedDate = data[i].crme_deceaseddate == null ? "" : data[i].crme_deceaseddate;
                var identityTheft = data[i].crme_identitytheft == null ? "" : data[i].crme_identitytheft;
                var vetSensLevel = data[i].crme_veteransensitivitylevel == null ? "" : data[i].crme_veteransensitivitylevel;
                var branchOfService = data[i].crme_branchofservice == null ? "" : data[i].crme_branchofservice;
                var rank = data[i].crme_rank == null ? "" : data[i].crme_rank;
                var serviceNumber = data[i].crme_servicenumber == null ? "" : data[i].crme_servicenumber;
                var insuranceNumber = data[i].crme_insurancenumber == null ? "" : data[i].crme_insurancenumber;
                var enteredOnDutyDate = data[i].crme_enteredondutydate == null ? "" : data[i].crme_enteredondutydate;
                var releasedActiveDutyDate = data[i].crme_releasedactivedutydate == null ? "" : data[i].crme_releasedactivedutydate;
                var suffix = data[i].crme_suffix == null ? "" : data[i].crme_suffix;
                var payeeNumber = data[i].crme_payeenumber == null ? "" : data[i].crme_payeenumber;
                var folderLocation = data[i].crme_folderlocation == null ? "" : data[i].crme_folderlocation;
                var udoidproofid = data[i].crme_udoidproofid == null ? "" : data[i].crme_udoidproofid;
                var veteranId = data[i].crme_personid == null ? "" : data[i].crme_personid;
                var prefix = data[i].crme_prefix == null ? "" : data[i].crme_prefix;
                var participantId = data[i].crme_participantid == null ? "" : data[i].crme_participantid;
                var fileNumber = data[i].crme_filenumber == null ? "" : data[i].crme_filenumber;
                var phoneNumber = data[i].crme_primaryphone == null ? "" : data[i].crme_primaryphone;


                window.open("http://uii/Global Manager/CopyToContext?sessionid=" + sessionid);
                window.open("http://uii/Global Manager/CopyToContext?veteranid=" + veteranId);
                window.open("http://uii/Global Manager/CopyToContext?idproofid=" + udoidproofid);
                window.open("http://uii/Global Manager/CopyToContext?vetFn=" + fileNumber);
                window.open("http://uii/Global Manager/CopyToContext?interactionid=" + interactionId);

                if (conversationid !== null) {
                    window.open("http://uii/Global Manager/CopyToContext?conversationid=" + conversationid);
                }

                // var url = "http://uii/Global Manager/CopyToContext?sessionid=" + sessionid + "&vetId=" + veteranId + "&idProofId=" + udoidproofid + "&vetFn=" + fileNumber + "&InteractionId=" + interactionId;
                // window.open(url);

                // var windowtoOpen = "http://event/?eventName=VetSearchComplete&sessionid=" +
                //     sessionid +
                //     "&vetId=" +
                //     veteranId +
                //     "&idProofId=" +
                //     udoidproofid +
                //     "&vetFn=" +
                //     fileNumber +
                //     "&InteractionId=" +
                //     interactionId;

                var windowtoOpen = "http://event/?eventName=VetSearchComplete";

                if (!interactionId) {
                    interactionId = data[i].crme_udointeractionid == null ? "" : data[i].crme_udointeractionid;

                    window.open("http://uii/Global Manager/CopyToContext?interactionid=" + interactionId);

                    //windowtoOpen = "http://event/?eventName=VetSearchComplete&sessionid=" + sessionid + "&vetId=" + veteranId + "&idProofId=" + udoidproofid + "&vetFn=" + fileNumber + "&InteractionId=" + interactionId;
                }
                if (data.length === 1) {
                    //only provide warning if single result which can be idProofd
                    if (!fileNumber || !participantId || participantId === 0)
                        ErrorMessage.Warning("Warning: the File Number or Participant ID is empty for the record retrieved!");
                    if (window.IsUSD == true) {
                        window.open(windowtoOpen);
                    }
                    //window.open("http://event/?eventName=VetSearchComplete&vetId=" + veteranId + "&idProofId=" + udoidproofid + "&vetFn=" + fileNumber);

                }
                if (i == 0) {
                    var th0 = document.createElement('th');
                    th0.setAttribute("SCOPE", "col");
                    var th1 = document.createElement('th');
                    th1.setAttribute("SCOPE", "col");
                    var th2 = document.createElement('th');
                    th2.setAttribute("SCOPE", "col");
                    var th2_1 = document.createElement('th');
                    th2_1.setAttribute("SCOPE", "col");
                    var th3 = document.createElement('th');
                    th3.setAttribute("SCOPE", "col");
                    var th3_1 = document.createElement('th');
                    th3_1.setAttribute("SCOPE", "col");
                    var thAlias = document.createElement('th');
                    thAlias.setAttribute("SCOPE", "col");
                    var th4 = document.createElement('th');
                    th4.setAttribute("SCOPE", "col");
                    var th5 = document.createElement('th');
                    th5.setAttribute("SCOPE", "col");
                    var th5_1 = document.createElement('th');
                    th5_1.setAttribute("SCOPE", "col");
                    var th6 = document.createElement('th');
                    th6.setAttribute("SCOPE", "col");
                    var th7 = document.createElement('th');
                    th7.setAttribute("SCOPE", "col");
                    var th8 = document.createElement('th');
                    th8.setAttribute("SCOPE", "col");
                    var th10 = document.createElement('th');
                    th10.setAttribute("SCOPE", "col");
                    var th11 = document.createElement('th');
                    th11.setAttribute("SCOPE", "col");
                    var th12 = document.createElement('th');
                    th12.setAttribute("SCOPE", "col");
                    var th13 = document.createElement('th');
                    th13.setAttribute("SCOPE", "col");
                    var th14 = document.createElement('th');
                    th14.setAttribute("SCOPE", "col");
                    var th15 = document.createElement('th');
                    th15.setAttribute("SCOPE", "col");
                    var th16 = document.createElement('th');
                    th16.setAttribute("SCOPE", "col");
                    var th17 = document.createElement('th');
                    th17.setAttribute("SCOPE", "col");
                    var th18 = document.createElement('th');
                    th18.setAttribute("SCOPE", "col");
                    var th19 = document.createElement('th');
                    th19.setAttribute("SCOPE", "col");
                    var th20 = document.createElement('th');
                    th20.setAttribute("SCOPE", "col");

                    th0.appendChild(document.createTextNode('Select'));
                    th1.appendChild(document.createTextNode('SSN'));
                    th2.appendChild(document.createTextNode('First Name'));
                    th2_1.appendChild(document.createTextNode('Middle Name'));
                    th3.appendChild(document.createTextNode('Last Name'));
                    th3_1.appendChild(document.createTextNode('Suffix'));
                    thAlias.appendChild(document.createTextNode('Alias'));
                    th4.appendChild(document.createTextNode('Date of Birth'));
                    th5.appendChild(document.createTextNode('Br. of Svc'));
                    th5_1.appendChild(document.createTextNode('Rank'));
                    th6.appendChild(document.createTextNode('Gender'));
                    th7.appendChild(document.createTextNode('Deceased Date'));
                    th8.appendChild(document.createTextNode('Address'));
                    th10.appendChild(document.createTextNode('EDIPI'));
                    th11.appendChild(document.createTextNode('Identity Theft'));
                    th12.appendChild(document.createTextNode('Sens. Level'));
                    th13.appendChild(document.createTextNode('Source'));
                    th14.appendChild(document.createTextNode('Service No'));
                    th15.appendChild(document.createTextNode('Insurance No'));
                    th16.appendChild(document.createTextNode('Entered On Duty'));
                    th17.appendChild(document.createTextNode('Released Active Duty'));
                    //th18.appendChild(document.createTextNode('Suffix'));
                    th19.appendChild(document.createTextNode('Payee No'));
                    th20.appendChild(document.createTextNode('Folder Loc'));

                    if (data.length > 1)
                        theadRow.appendChild(th0);
                    theadRow.appendChild(th1);
                    theadRow.appendChild(th2);
                    if (hasMiddleName)
                        theadRow.appendChild(th2_1);
                    theadRow.appendChild(th3);
                    if (hasSuffix)
                        theadRow.appendChild(th3_1);
                    if (hasAlias) {
                        theadRow.appendChild(thAlias);
                    }
                    //theadRow.appendChild(th3);
                    theadRow.appendChild(th4);
                    theadRow.appendChild(th5);
                    theadRow.appendChild(th5_1);
                    theadRow.appendChild(th6);
                    if (hasDeceasedDate) {
                        theadRow.appendChild(th7);
                    }
                    theadRow.appendChild(th8);
                    theadRow.appendChild(th10);
                    if (hasIdentityTheft) {
                        theadRow.appendChild(th11);
                    }
                    theadRow.appendChild(th12);
                    theadRow.appendChild(th13);
                    if (hasServiceNumber)
                        theadRow.appendChild(th14);
                    if (hasInsuranceNumber)
                        theadRow.appendChild(th15);
                    if (hasEnteredOnDutyDate)
                        theadRow.appendChild(th16);
                    if (hasReleasedActiveDutyDate)
                        theadRow.appendChild(th17);
                    if (hasPayeeNumber)
                        theadRow.appendChild(th19);
                    if (hasFolderLocation)
                        theadRow.appendChild(th20);
                    thead.appendChild(theadRow);
                }

                // Table rows
                var row = document.createElement('tr');
                var col0 = document.createElement('td');
                col0.setAttribute("SCOPE", "row");
                var col1 = document.createElement('td');
                col1.setAttribute("SCOPE", "row");
                var col2 = document.createElement('td');
                col2.setAttribute("SCOPE", "row");
                var col2_1 = document.createElement('td');
                col2_1.setAttribute("SCOPE", "row");
                var col3 = document.createElement('td');
                col3.setAttribute("SCOPE", "row");
                var col3_1 = document.createElement('td');
                col3_1.setAttribute("SCOPE", "row");
                var colAlias = document.createElement('td');
                colAlias.setAttribute("SCOPE", "row");
                var col4 = document.createElement('td');
                col4.setAttribute("SCOPE", "row");
                var col5 = document.createElement('td');
                col5.setAttribute("SCOPE", "row");
                var col5_1 = document.createElement('td');
                col5_1.setAttribute("SCOPE", "row");
                var col6 = document.createElement('td');
                col6.setAttribute("SCOPE", "row");
                var col7 = document.createElement('td');
                col7.setAttribute("SCOPE", "row");
                var col8 = document.createElement('td');
                col8.setAttribute("SCOPE", "row");
                var col10 = document.createElement('td');
                col10.setAttribute("SCOPE", "row");
                var col11 = document.createElement('td');
                col11.setAttribute("SCOPE", "row");
                var col12 = document.createElement('td');
                col12.setAttribute("SCOPE", "row");
                var col13 = document.createElement('td');
                col13.setAttribute("SCOPE", "row");
                var col14 = document.createElement('td');
                col14.setAttribute("SCOPE", "row");
                var col15 = document.createElement('td');
                col15.setAttribute("SCOPE", "row");
                var col16 = document.createElement('td');
                col16.setAttribute("SCOPE", "row");
                var col17 = document.createElement('td');
                col17.setAttribute("SCOPE", "row");
                //var col18 = document.createElement('td');
                var col19 = document.createElement('td');
                col19.setAttribute("SCOPE", "row");
                var col20 = document.createElement('td');
                col20.setAttribute("SCOPE", "row");

                var selectorButton = document.createElement("Button");
                //selectorButton.setAttribute("type", "button");
                selectorButton.setAttribute("aria-label", "Veteran Selector button for: " + fullName);
                selectorButton.setAttribute("id", "selectorButton" + i);
                selectorButton.setAttribute("tabindex", 100 + 10 * i - 1);
                selectorButton.setAttribute("rowNum", i);
                selectorButton.innerText = "Select";
                col0.appendChild(selectorButton);
                selectorButton.onclick = onSelectorButtonClick;

                if (data.length == 1) {
                    var ssnIdProofCheckbox = document.createElement("INPUT");
                    ssnIdProofCheckbox.setAttribute("type", "checkbox");
                    ssnIdProofCheckbox.setAttribute("aria-label", "SSN ID Proof checkbox: " + ssn);
                    ssnIdProofCheckbox.setAttribute("id", "nameIdProofCheckbox" + i);
                    ssnIdProofCheckbox.setAttribute("tabindex", 100 + 10 * i);
                    ssnIdProofCheckbox.setAttribute("name", "ssnVerified");
                    ssnIdProofCheckbox.setAttribute("rowNum", i);
                    ssnIdProofCheckbox.setAttribute("checked", "true");
                    ssnIdProofCheckbox.onchange = onIdProofCheckboxCheck;
                    col1.appendChild(ssnIdProofCheckbox);
                    var ssnSpan = document.createElement("SPAN");
                    ssnSpan.setAttribute("onclick", "javascript:onIdProofCheckboxTextClick('nameIdProofCheckbox" + i + "');");
                    ssnSpan.appendChild(document.createTextNode(ssn));
                    col1.appendChild(ssnSpan);
                }
                else
                    col1.appendChild(document.createTextNode(ssn));

                if (data.length == 1) {
                    var firstnameIdProofCheckbox = document.createElement("INPUT");
                    firstnameIdProofCheckbox.setAttribute("type", "checkbox");
                    firstnameIdProofCheckbox.setAttribute("aria-label", "First Name ID Proof checkbox: " + fullName);
                    firstnameIdProofCheckbox.setAttribute("id", "firstnameIdProofCheckbox" + i);
                    firstnameIdProofCheckbox.setAttribute("tabindex", 101 + 10 * i);
                    firstnameIdProofCheckbox.setAttribute("name", "firstnameVerified");
                    firstnameIdProofCheckbox.setAttribute("rowNum", i);
                    firstnameIdProofCheckbox.setAttribute("checked", "true");
                    firstnameIdProofCheckbox.onchange = onIdProofCheckboxCheck;
                    col2.appendChild(firstnameIdProofCheckbox);
                    var firstnameSpan = document.createElement("SPAN");
                    firstnameSpan.setAttribute("onclick", "javascript:onIdProofCheckboxTextClick('firstnameIdProofCheckbox" + i + "');");
                    firstnameSpan.appendChild(document.createTextNode(firstName));
                    col2.appendChild(firstnameSpan);
                }
                else
                    col2.appendChild(document.createTextNode(firstName));

                col2_1.appendChild(document.createTextNode(middleName))

                if (data.length == 1) {
                    var lastnameIdProofCheckbox = document.createElement("INPUT");
                    lastnameIdProofCheckbox.setAttribute("type", "checkbox");
                    lastnameIdProofCheckbox.setAttribute("aria-label", "Last Name ID Proof checkbox: " + fullName);
                    lastnameIdProofCheckbox.setAttribute("id", "lastnameIdProofCheckbox" + i);
                    lastnameIdProofCheckbox.setAttribute("tabindex", 102 + 10 * i);
                    lastnameIdProofCheckbox.setAttribute("name", "lastnameVerified");
                    lastnameIdProofCheckbox.setAttribute("rowNum", i);
                    lastnameIdProofCheckbox.setAttribute("checked", "true");
                    lastnameIdProofCheckbox.onchange = onIdProofCheckboxCheck;
                    col3.appendChild(lastnameIdProofCheckbox);
                    var lastnameSpan = document.createElement("SPAN");
                    lastnameSpan.setAttribute("onclick", "javascript:onIdProofCheckboxTextClick('lastnameIdProofCheckbox" + i + "');");
                    lastnameSpan.appendChild(document.createTextNode(lastName));
                    col3.appendChild(lastnameSpan);
                }
                else
                    col3.appendChild(document.createTextNode(lastName));

                col3_1.appendChild(document.createTextNode(suffix));

                colAlias.appendChild(document.createTextNode(alias));

                if (data.length == 1) {
                    var dobIdProofCheckbox = document.createElement("INPUT");
                    dobIdProofCheckbox.setAttribute("type", "checkbox");
                    dobIdProofCheckbox.setAttribute("aria-label", "Date of Birth ID Proof checkbox: " + dateOfBirth);
                    dobIdProofCheckbox.setAttribute("id", "dobIdProofCheckbox" + i);
                    dobIdProofCheckbox.setAttribute("tabindex", 103 + 10 * i);
                    dobIdProofCheckbox.setAttribute("name", "dobVerified");
                    dobIdProofCheckbox.setAttribute("rowNum", i);
                    dobIdProofCheckbox.setAttribute("checked", "true");
                    dobIdProofCheckbox.onchange = onIdProofCheckboxCheck;
                    col4.appendChild(dobIdProofCheckbox);
                    var dobSpan = document.createElement("SPAN");
                    dobSpan.setAttribute("onclick", "javascript:onIdProofCheckboxTextClick('dobIdProofCheckbox" + i + "');");
                    dobSpan.appendChild(document.createTextNode(dateOfBirth));
                    col4.appendChild(dobSpan);
                }
                else
                    col4.appendChild(document.createTextNode(dateOfBirth));

                var longBranchofService = LongBranchOfService(branchOfService);
                if (data.length == 1) {
                    var bosIdProofCheckbox = document.createElement("INPUT");
                    bosIdProofCheckbox.setAttribute("type", "checkbox");
                    bosIdProofCheckbox.setAttribute("aria-label", "Branch of Service ID Proof checkbox: " + longBranchofService);
                    bosIdProofCheckbox.setAttribute("id", "bosIdProofCheckbox" + i);
                    bosIdProofCheckbox.setAttribute("tabindex", 104 + 10 * i);
                    bosIdProofCheckbox.setAttribute("name", "bosVerified");
                    bosIdProofCheckbox.setAttribute("rowNum", i);
                    bosIdProofCheckbox.setAttribute("checked", "true");
                    bosIdProofCheckbox.onchange = onIdProofCheckboxCheck;
                    col5.appendChild(bosIdProofCheckbox);
                    var bosSpan = document.createElement("SPAN");
                    bosSpan.setAttribute("onclick", "javascript:onIdProofCheckboxTextClick('bosIdProofCheckbox" + i + "');");
                    bosSpan.appendChild(document.createTextNode(longBranchofService));
                    col5.appendChild(bosSpan);
                }
                else
                    col5.appendChild(document.createTextNode(longBranchofService));

                col5_1.appendChild(document.createTextNode(rank));
                col6.appendChild(document.createTextNode(gender));
                col7.appendChild(document.createTextNode(deceasedDate));
                col8.appendChild(document.createTextNode(address));

                col10.appendChild(document.createTextNode(edipi));
                col11.appendChild(document.createTextNode(identityTheft));
                col12.appendChild(document.createTextNode(vetSensLevel));
                col13.appendChild(document.createTextNode(recordSource));
                col14.appendChild(document.createTextNode(serviceNumber));
                col15.appendChild(document.createTextNode(insuranceNumber));
                col16.appendChild(document.createTextNode(enteredOnDutyDate));
                col17.appendChild(document.createTextNode(releasedActiveDutyDate));
                //col18.appendChild(document.createTextNode(suffix));
                col19.appendChild(document.createTextNode(payeeNumber));
                col20.appendChild(document.createTextNode(folderLocation));

                if (data.length > 1)
                    row.appendChild(col0);
                row.appendChild(col1);
                row.appendChild(col2);
                if (hasMiddleName)
                    row.appendChild(col2_1);
                row.appendChild(col3);
                if (hasSuffix)
                    row.appendChild(col3_1);
                if (hasAlias) {
                    row.appendChild(colAlias);
                }

                row.appendChild(col4);
                row.appendChild(col5);
                row.appendChild(col5_1);
                row.appendChild(col6);
                if (hasDeceasedDate) {
                    row.appendChild(col7);
                }
                row.appendChild(col8);
                row.appendChild(col10);
                if (hasIdentityTheft) {
                    row.appendChild(col11);
                }
                row.appendChild(col12);
                row.appendChild(col13);
                if (hasServiceNumber)
                    row.appendChild(col14);
                if (hasInsuranceNumber)
                    row.appendChild(col15);
                if (hasEnteredOnDutyDate)
                    row.appendChild(col16);
                if (hasReleasedActiveDutyDate)
                    row.appendChild(col17);
                //if (hasSuffix)
                //    row.appendChild(col18);
                if (hasPayeeNumber)
                    row.appendChild(col19);
                if (hasFolderLocation)
                    row.appendChild(col20);

                row.setAttribute('fullName', fullName);
                row.setAttribute('dateofbirth', dateOfBirth);
                row.setAttribute('fulladdress', address);
                row.setAttribute('ssn', ssn);
                row.setAttribute('edipi', edipi);
                row.setAttribute('recordSource', recordSource);
                row.setAttribute('firstName', firstName);
                row.setAttribute('middleName', middleName);
                row.setAttribute('lastName', lastName);
                row.setAttribute('suffix', suffix);
                row.setAttribute('gender', gender);
                row.setAttribute('patientMviIdentifier', patientMviIdentifier);
                row.setAttribute('icn', icn);
                row.setAttribute('vetSensLevel', vetSensLevel);
                row.setAttribute('crme_firstnameverified', true);
                row.setAttribute('crme_lastnameverified', true);
                row.setAttribute('crme_dobverified', true);
                row.setAttribute('crme_genderverified', true);
                row.setAttribute('crme_ssnverified', true);
                row.setAttribute('crme_bosverified', true);
                row.setAttribute('udoidproofid', udoidproofid);
                row.setAttribute('veteranId', veteranId);
                row.setAttribute('rowIdNum', i);
                row.setAttribute('alias', alias);
                row.setAttribute('deceasedDate', deceasedDate);
                row.setAttribute('identityTheft', identityTheft);
                row.setAttribute('prefix', prefix);
                row.setAttribute('participantId', participantId);
                row.setAttribute('fileNumber', fileNumber);
                row.setAttribute('phoneNumber', phoneNumber);
                row.className = (i % 2 == 0) ? "even" : "odd";

                row.tabIndex = 100 + 10 * i;
                table.append(thead);
                table.append(row);

                $("#resultsFieldSetDiv").show();
                $("#idProofCompleteButton").show();
                $("#idProofVeteranCompleteButton").show();
                $("#idProofFailedButton").show();

            }
        }
        else {
            //Even if there are no person records as a result of the crme_person retrieve,
            //the return message should be populated as part of the crme_person
            //therefore, the only time we should get here is if there is an error on the VIMT call
            $("#idProofCompleteButton").hide();
            $("#idProofVeteranCompleteButton").hide();
            $("#idProofFailedButton").hide();
            var searchMessage = $("#searchResultsMessageInput");
            searchMessage.val("A connection error was encountered during the search. Please try the search again and contact the helpdesk if the issue is not resolved.");
            $("#resultsFieldSetDiv").show();

            var fieldLength = searchMessage.val().length;
            searchMessage.attr('size', fieldLength);
            searchMessage.show();

        }
        var searchMessage = $("#searchResultsMessageInput");
        searchMessage.val((data != null && data.length > 0 && data[0].crme_returnmessage != null) ? data[0].crme_returnmessage : "Your search in MVI did not find any records matching the search criteria.");
        if (searchMessage.val().indexOf("access violation") > -1 || searchMessage.text().indexOf("sensitive record check error") > -1) {

            var violationSplit = searchMessage.val().split("!");
            var sensitiveLevelMessage;

            if (violationSplit.length > 1) {
                sensitiveLevelMessage = violationSplit[1].charAt(0).toUpperCase() + violationSplit[1].slice(1);
            }

            searchMessage.val("You are trying to access a record above your allocated sensitivity level. Please transfer the call to your supervisor to support the needs of the veteran. " + sensitiveLevelMessage + ".");

            //searchMessage.val("You are trying to access a record above your allocated sensitivity level. Please transfer the call to your supervisor to support the needs of the veteran.");
            //searchMessage.text("Sensitivity Level Error");
        }
        else
            if (searchMessage.val().indexOf("BIRLS communication is down") > -1 || searchMessage.text().indexOf("The Tuxedo service is down") > -1) {
                searchMessage.val("A connection error was encountered during the CORPDB search: " + searchMessage.text());
            }
        var fieldLength = searchMessage.val().length;
        searchMessage.attr('size', fieldLength);
        searchMessage.show();
    }

    function onSelectorButtonClick() {
        $("#searchResultsMessageInput").val("");
        formatExecutingSearch();

        idProofId = null;
        veteranId = null;
        var table = $("#personSearchResultsTable");
        var obj = this;
        var rowNum = obj.getAttribute("rowNum");
        var row = table.find("[rowIdNum=" + rowNum + "]");

        var fullName = row[0].getAttribute('fullName');
        var dob = row[0].getAttribute('dateofbirth');
        var address = row[0].getAttribute('fulladdress');
        var ssn = row[0].getAttribute('ssn');
        var fileNumber = row[0].getAttribute('fileNumber');
        var edipi = row[0].getAttribute('edipi');
        var recordSource = row[0].getAttribute('recordSource');
        var firstName = row[0].getAttribute('firstName');
        var middleName = row[0].getAttribute('middleName');
        var lastName = row[0].getAttribute('lastName');
        var suffix = row[0].getAttribute('suffix');
        var gender = row[0].getAttribute('gender');
        var patientMviIdentifier = row[0].getAttribute('patientMviIdentifier');
        var icn = row[0].getAttribute('icn');

        var alias = row[0].getAttribute('alias');
        var deceasedDate = row[0].getAttribute('deceasedDate');
        var identityTheft = row[0].getAttribute('identityTheft');
        var prefix = row[0].getAttribute('prefix');
        var participantId = row[0].getAttribute('participantId');
        var phoneNumber = row[0].getAttribute('phoneNumber');
        var vetSensitivityLevel = row[0].getAttribute('vetSensLevel');

        var filter = "?$filter=";
        filter += buildQueryFilter("crme_firstname", firstName, false);
        filter += buildQueryFilter("crme_lastname", lastName, true);
        filter += buildQueryFilter("crme_searchtype", 'CombinedSelectedPerson', true);
        if (interactionId != null)
            filter += buildQueryFilter("crme_udointeractionid", interactionId, true);
        filter += " and crme_isattended eq true";
        filter += " and crme_dobstring eq '" + dob + "'";
        filter += buildQueryFilter("crme_filenumber", fileNumber, true);
        filter += buildQueryFilter("crme_ssn", ssn, true);
        filter += buildQueryFilter("crme_patientmviidentifier", patientMviIdentifier, true);
        filter += buildQueryFilter("crme_icn", icn, true);
        filter += buildQueryFilter("crme_middlename", middleName, true);
        filter += buildQueryFilter("crme_fullname", fullName, true);
        filter += buildQueryFilter("crme_alias", alias, true);
        filter += buildQueryFilter("crme_fulladdress", address, true);
        filter += buildQueryFilter("crme_recordsource", recordSource, true);
        filter += buildQueryFilter("crme_edipi", edipi, true);
        filter += buildQueryFilter("crme_suffix", suffix, true);
        filter += buildQueryFilter("crme_gender", gender, true);
        filter += buildQueryFilter("crme_prefix", prefix, true);
        filter += buildQueryFilter("crme_deceaseddate", deceasedDate, true);
        filter += buildQueryFilter("crme_identitytheft", identityTheft, true);
        filter += buildQueryFilter("crme_participantid", participantId, true);
        filter += buildQueryFilter("crme_primaryphone", phoneNumber, true);
        filter += buildQueryFilter("crme_veteransensitivitylevel", vetSensitivityLevel, true);

        Xrm.WebApi.retrieveMultipleRecords("crme_person", filter)
            .then(
                function (data) {
                    if ((data !== null) && (data.value !== null)) {
                        personSearchCallBack(data.value);
                        personSearchComplete();
                    }
                }
            );
    }

    function onIdProofCheckboxCheck() {
        if (idProofCheckboxesChecked(this) == true) {
            $('#idProofCompleteButton').attr('disabled', false);
            if (selectAnotherVet === "true")
                $('#idProofVeteranCompleteButton').attr('disabled', true);
            else
                $('#idProofVeteranCompleteButton').attr('disabled', false);
            $('#idProofFailedButton').attr('disabled', true);
        }
        else {
            $('#idProofCompleteButton').attr('disabled', true);
            $('#idProofVeteranCompleteButton').attr('disabled', true);
            $('#idProofFailedButton').attr('disabled', false);
        }
    }

    function idProofCheckboxesChecked(obj) {
        var table = $("#personSearchResultsTable");
        var rowNum = obj.getAttribute("rowNum");
        var row = table.find("[rowIdNum=" + rowNum + "]");

        switch (obj.name) {
            case "firstnameVerified":
                row[0].setAttribute('crme_firstnameverified', obj.checked);
                break;
            case "lastnameVerified":
                row[0].setAttribute('crme_lastnameverified', obj.checked);
                break;
            case "dobVerified":
                row[0].setAttribute('crme_dobverified', obj.checked);
                break;
            case "ssnVerified":
                row[0].setAttribute('crme_ssnverified', obj.checked);
                break;
            case "bosVerified":
                row[0].setAttribute('crme_bosverified', obj.checked);
                break;
        }
        var checkboxes = $("[id*='IdProofCheckbox" + rowNum + "']");
        for (var key in checkboxes) {
            if (checkboxes[key].checked === false)
                return false;
        }
        return true;
    }

    function formatDatePart(datepart) {
        return datepart.length == 1 ? "0" + datepart : datepart;
    }

    function formatName(data) {
        if (data.crme_fullname != null && data.crme_fullname.trim().length > 0) {
            return data.crme_fullname;
        }

        var firstName = (data.crme_fullname != null && data.crme_firstname.trim().length > 0) ? data.crme_firstname : "";
        var lastName = (data.crme_fullname != null && data.crme_lastname.trim().length > 0) ? data.crme_lastname : "";
        //var firstName = (data.crme_fullname == null && data.crme_firstname.trim().length > 0) ? data.crme_firstname : "";
        //var lastName = (data.crme_fullname == null && data.crme_lastname.trim().length > 0) ? data.crme_lastname : "";

        return firstName + " " + lastName;
    }

    function formatAddress(data) {
        if (data.crme_fulladdress != null) {
            return data.crme_fulladdress;
        }

        var street = data.crme_address1 != null ? data.crme_address1 : "";
        var city = data.crme_city != null ? data.crme_city : "";
        //TODO: JK field not on entity
        //var state = data.crme_stateprovinceid.Name != null ? data.crme_StateProvinceId.Name : "";

        //TODO: JK field not on entity
        //var zip = data.crme_ZIPPostalCodeId.Name != null ? data.crme_ZIPPostalCodeId : "";

        //return street + " " + city + " " + state + " " + zip;
        return street + " " + city;
    }

    var userSL;

    function validateSearchByIdentifier() {
        var edipi = $("#EdipiTextBox").val();

        if (edipi != "") {
            if (((edipi.length < 9 || edipi.length > 10) || isNumeric(edipi) == false)) {
                $("#validationFailedDiv").text("VALIDATION FAILED: EDIPI is invalid.");
                return false;
            }
            return true;
        }
        else {
            $("#validationFailedDiv").text("VALIDATION FAILED: The search requires an EDIPI.");
            return false;
        }
    }

    // Taken from: UDOgetContactRecordsProcessor.cs
    function LongBranchOfService(branchcode) {
        switch (branchcode.trim()) {
            case "AF": return "AIR FORCE (AF)";
            case "A": return "ARMY (ARMY)";
            //ARMY AIR CORPS
            case "CG": return "COAST GUARD (CG)";
            case "CA": return "COMMONWEALTH ARMY (CA)";
            case "GCS": return "GUERRILLA AND COMBINATION SVC (GCS)";
            case "M": return "MARINES (M)";
            case "MM": return "MERCHANT MARINES (MM)";
            case "NOAA": return "NATIONAL OCEANIC & ATMOSPHERIC ADMINISTRATION (NOAA)";
            //NAVY (NAVY)
            case "PHS": return "PUBLIC HEALTH SVC (PHS)";
            case "RSS": return "REGULAR PHILIPPINE SCOUT (RSS)";
            //REGULAR PHILIPPINE SCOUT COMBINED WITH SPECIAL
            case "RPS": return "PHILIPPINE SCOUT OR COMMONWEALTH ARMY SVC (RPS)";
            case "SPS": return "SPECIAL PHILIPPINE SCOUTS (SPS)";
            case "WAC": return "WOMEN'S ARMY CORPS (WAC)";
        }
        return branchcode;
    }

    function validateSearchByName() {
        var result = $.Deferred();
        var errors = [];
        var await = $.when();

        var fname = $("#FirstNameTextBox").val();
        var lname = $("#LastNameTextBox").val();
        var ssn = $("#SocialSecurityTextBox").val();
        // var phone = $("#PhoneNoTextBox").val();
        var dobyear = $("#BirthYearTextBox").val();
        var dobmonth = $("#BirthMonthTextBox").val();
        var dobday = $("#BirthDayTextBox").val();
        var dob = dobyear + dobmonth + dobday;

        var fields = [];

        if (ssn == null || ssn.trim() == "") fields.push("'SSN'");
        if (lname == null || lname == "") fields.push("'last name'");
        if (dob == null || dob.trim() == "" || !isNumeric(dob.trim())) fields.push("'DOB'");

        if (fields.length > 0) {
            var p = fields.length > 1;
            errors.push(fields.join(", ") + " field" + (p ? 's' : '') + ' ' + (p ? 'are' : 'is') + ' required.');
            fields = [];
        }

        ssn = ssn.replace(/-/g, "").trim();

        var ssnDigits = ssn.trim().length;
        if (ssnDigits == 6 || ssnDigits == 7) {
            var message = 'You have entered ' + ssnDigits + ' digits for the SSN.\n\nWould you like to add ' + (ssnDigits == 6 ? '2' : '1') + ' preceeding zeros?';
            await = Va.Udo.Crm.Scripts.Popup.MsgBox(message, Va.Udo.Crm.Scripts.Popup.PopupStyles.YesNo + Va.Udo.Crm.Scripts.Popup.PopupStyles.Question,
                "SSN Validation", { width: 300, height: 165 })
                .done(function () {
                    ssn = "0" + ssn;
                    if (ssnDigits == 6) ssn = "0" + ssn;
                    $("#SocialSecurityTextBox").val(ssn);
                });
        }

        await.then(function () {
            if (ssn.trim().length > 0 && (isNumeric(ssn.trim()) == false || ssn.trim().length < 8 || ssn.trim().length > 9)) errors.push("SSN is invalid.");

            if (!validateDateOfBirth(dobyear, dobmonth, dobday)) errors.push("DOB is invalid.");

            if (errors.length > 0) {
                $("#validationFailedDiv").text("VALIDATION FAILED: " + errors.join(" "));
                result.reject();
            } else {
                result.resolve();
            }
        });

        return result.promise();
    }

    function validateSearchByAlternate() {
        var ssn = $("#SocialSecurityBirlsTextBox").val();
        var dob = $('#DobBirlsTextBox').val();
        var dod = $('#DateOfDeathBirlsTextBox').val();
        var eod = $('#EnteredOnDutyBirlsTextBox').val();
        var rad = $('#ReleasedActiveDutyBirlsTextBox').val();
        var errorMessage = "VALIDATION FAILED: ";
        var errorCount = 0;
        ssn = ssn.replace(/-/g, "").trim();

        //SSN is not required for BIRLS search
        if (ssn != "") {
            if (isNumeric(ssn.trim()) == false) {
                errorMessage += " SSN is invalid.";
                errorCount += 1;
            }
        }

        if (dob != "") {
            if (!validateDateSingleField(dob)) {
                errorMessage += " DOB is invalid.";
                errorCount += 1;
            }
        }
        if (dod != "") {
            if (!validateDateSingleField(dod)) {
                errorMessage += " Date of Death is invalid.";
                errorCount += 1;
            }
        }
        if (eod != "") {
            if (!validateDateSingleField(eod)) {
                errorMessage += " Entered on Duty is invalid.";
                errorCount += 1;
            }
        }
        if (rad != "") {
            if (!validateDateSingleField(rad)) {
                errorMessage += " Released Active Duty is invalid.";
                errorCount += 1;
            }
        }

        if (errorCount > 0) {
            $("#validationFailedDiv").text(errorMessage);
            return false;
        }


        return true;
    }

    function validateDateOfBirth(dobyear, dobmonth, dobday) {

        if ((dobyear == "" || dobyear == "YYYY") && (dobmonth == "" || dobmonth == "MM") && (dobday == "" || dobday == "DD")) {
            return true;
        }

        if (dobyear != "YYYY" || dobmonth != "MM" || dobday != "DD") {
            if ((dobyear != "" && isNumeric(dobyear) == false) || (dobmonth != "" && isNumeric(dobmonth) == false) || (dobday != "" && isNumeric(dobday) == false)) {
                return false;
            }
        }


        if (dobyear.length != 4) {
            return false;
        }

        if (dobyear >= (new Date).getFullYear() + 1) {
            return false;
        }

        if (dobyear < (new Date).getFullYear() - 200) {
            return false;
        }

        if (dobmonth < 1 || dobmonth > 12) {
            return false;
        }

        if (dobday < 1 || dobday > 31) {
            return false;
        }

        var fullDobDate = new Date(dobyear, dobmonth - 1, dobday);
        var currFullDate = Date.now();
        if (fullDobDate >= currFullDate)
            return false;

        return true;
    }

    function validateDateSingleField(date) {

        if (date == null || date == "")
            return false;
        var datemonth;
        var dateday;
        var dateyear;
        var dateSplit = date.split("/");
        if (dateSplit[0] != null && isNumeric(dateSplit[0]))
            datemonth = dateSplit[0];
        if (dateSplit[1] != null && isNumeric(dateSplit[1]))
            dateday = dateSplit[1];
        if (dateSplit[2] != null && isNumeric(dateSplit[2]))
            dateyear = dateSplit[2];


        if (dateyear == null || dateyear.length != 4) {
            return false;
        }

        if (dateyear >= (new Date).getFullYear() + 1) {
            return false;
        }

        if (dateyear < (new Date).getFullYear() - 200) {
            return false;
        }

        if (datemonth == null || datemonth < 1 || datemonth > 12) {
            return false;
        }

        if (dateday == null || dateday < 1 || dateday > 31) {
            return false;
        }

        var fulldateDate = new Date(dateyear, datemonth - 1, dateday);
        var currFullDate = Date.now();
        if (fulldateDate >= currFullDate)
            return false;

        return true;
    }

    function GetQueryParams() {
        var queryParams = Va.Udo.Crm.Scripts.Utility.getDataParameter(Va.Udo.Crm.Scripts.Utility.getQueryParameter("data"));
        if (queryParams.ChatSessionLogId)
            chatSessionLogId = queryParams.ChatSessionLogId;

        interactionId = queryParams.InteractionId;
        selectAnotherVet = queryParams.SelectAnotherVet;
        ani = queryParams.ani;
        ctissn = queryParams.ssn;
        ctidob = queryParams.dob;
        ctiedipi = queryParams.edipi;
        sessionid = queryParams.sessionid;

        if ("conversationid" in queryParams) {
            conversationid = queryParams.conversationid;
        }
    }

    function GetChatDetailsIfAvailable() {
        if (chatSessionLogId) {
            formatExecutingSearch();

            var filter = "ActivityId eq'" + chatSessionLogId + "'";
            Xrm.WebApi.retrieveMultipleRecords("crme_chatbrowsesessionlog", filter)
                .then(
                    function (data) {
                        chatSearchCallBack(data.entites);
                        chatSearchComplete();
                    }
                );
        }
    }

    function chatSearchCallBack(data) {

        if (data != null && data.length > 0) {
            var edipi = data[0].crme_EDIPI;
            if (edipi)
                edipi = edipi.trim();
            var pid = data[0].crme_ParticipantId;
            if (pid)
                pid = pid.trim();
            var ssn = data[0].crme_SSN;
            if (ssn)
                ssn = ssn.trim();
            performSearchUsingChatParameters(edipi, pid, ssn);
        }

    }
    function performSearchUsingChatParameters(edipi, pid, ssn) {
        var filter = buildQueryFilter("crme_SearchType", 'CHAT', false);
        if (edipi) {
            filter += buildQueryFilter("crme_EDIPI", edipi, true);
            $("#EdipiTextBox").val(edipi);
        }
        if (pid)

            filter += buildQueryFilter("crme_ParticipantID", pid, true);
        if (ssn) {
            filter += buildQueryFilter("crme_SSN", ssn, true);
            $("#SocialSecurityTextBox").val(ssn);
        }
        if (interactionId)
            filter += buildQueryFilter("crme_udointeractionid", interactionId, true);

        Xrm.WebApi.retrieveMultipleRecords("crme_person", filter)
            .then(
                function (data) {
                    personSearchCallBack(data.entities);
                    personSearchComplete();
                }
            );

    }
    function performSearchUsingCTIParameters() {
        // alert("here!, ani:" + ani);
        if (ani) {
            formatExecutingSearch();
            var filter = "?$select=*&$filter=";
            filter += buildQueryFilter("crme_searchtype", 'CTI', false);
            if (ani)
                filter += buildQueryFilter("crme_ani", ani, true);
            if (ctidob) {
                filter += buildQueryFilter("crme_dobstring", ctidob, true);
                $("#BirthDayTextBox").val(ctidob.substring(6, 8));
                $("#BirthYearTextBox").val(ctidob.substring(0, 4));
                $("#BirthMonthTextBox").val(ctidob.substring(4, 6));
            }
            if (ctissn) {
                filter += buildQueryFilter("crme_ssn", ctissn, true);
                $("#SocialSecurityTextBox").val(ctissn);
            }
            if (ctiedipi) {
                filter += buildQueryFilter("crme_edipi", ctiedipi, true);
                $("#EdipiTextBox").val(ctiedipi);
            }
            else {
                filter += buildQueryFilter("crme_edipi", "not", true);
            }
            if (interactionId)
                filter += buildQueryFilter("crme_udointeractionid", interactionId, true);

            Xrm.WebApi.retrieveMultipleRecords("crme_person", filter)
                .then(
                    function reply(response) {
                        personSearchCallBack(response.value);
                        personSearchComplete();
                    }).catch(function (error) {

                        console.log(error.message);
                        console.log(error.innerError.message);
                        console.log(error.innerError.stacktrace);
                        personSearchComplete();
                        formatValidationFailed();
                    });
        }
    }


}); // end Document Ready

function isNumeric(value) {

    return !isNaN(parseFloat(value) && isFinite(value));
}

function clearField(obj) {
    if (obj.defaultValue == obj.value) obj.value = '';
}
