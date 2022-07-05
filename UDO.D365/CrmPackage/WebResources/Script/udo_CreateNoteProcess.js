"use strict";

/// <reference path="va_CrmRestKit_2011" />

if (typeof Va === 'undefined' || !Va) {
    var Va = Va || {};
}

if (typeof Va.Udo === 'undefined' || !Va.Udo) {
    Va.Udo = Va.Udo || {};
}

if (typeof Va.Udo.Crm === 'undefined' || !Va.Udo.Crm) {
    Va.Udo.Crm = Va.Udo.Crm || {};
}

if (typeof Va.Udo.Crm.MapdNote === 'undefined' || !Va.Udo.Crm.MapdNote) {
    Va.Udo.Crm.MapdNote = Va.Udo.Crm.MapdNote || {};
}


Va.Udo.Crm.MapdNote = {
    EntityId: null,
    EntityName: null,
    TypeOfNote: null,
    MessageName: null,
    Message: null,
    Title: null,
    formContext: null,
    webApi: null,
    formHelper: null,
    globeCon: null,

    SetCommonVariables: function (context) {
        formContext = context.getFormContext();
        var globeCon = Xrm.Utility.getGlobalContext();
        var version = globeCon.getVersion();
        webApi = new CrmCommonJS.WebApi(version);
    },

    // You can send in a Override Message Name (i.e. Copy) during the Service Request Copy process
    // else you can pass "" to have the system figure out the Message Name
    Initialize: function (context, overrideMessageName) {
        this.SetCommonVariables(context);

        Va.Udo.Crm.MapdNote.EntityId = formContext.data.entity.getId();
        Va.Udo.Crm.MapdNote.EntityName = formContext.data.entity.getEntityName();

        if (overrideMessageName && overrideMessageName.length > 0) {

            Va.Udo.Crm.MapdNote.MessageName = overrideMessageName;

        } else {
            var selectedTwoOption = null;
            switch (Va.Udo.Crm.MapdNote.EntityName) {
                case "udo_lettergeneration":
                    Va.Udo.Crm.MapdNote.TypeOfNote = "Letter";

                    if (formContext.getAttribute("udo_notecreated") !== null) {
                        // selectedTwoOption = formContext.getAttribute("udo_notecreated").getSelectedOption();
                        selectedTwoOption = formContext.getAttribute("udo_notecreated").getValue();
                        // if (selectedTwoOption.value === 1) {
                        if (selectedTwoOption === true) {
                            Va.Udo.Crm.MapdNote.MessageName = "Update";
                        } else {
                            Va.Udo.Crm.MapdNote.MessageName = "Create";
                        }
                    }

                    break;
                case "udo_servicerequest":
                    Va.Udo.Crm.MapdNote.TypeOfNote = "Service Request";

                    if (formContext.getAttribute("udo_notecreated") !== null) {
                        // selectedTwoOption = formContext.getAttribute("udo_notecreated").getSelectedOption();
                        selectedTwoOption = formContext.getAttribute("udo_notecreated").getValue();
                        // if (selectedTwoOption.value === 1) {
                        if (selectedTwoOption === true) {
                            Va.Udo.Crm.MapdNote.MessageName = "Update";
                        } else {
                            Va.Udo.Crm.MapdNote.MessageName = "Create";
                        }
                    }

                    break;
                case "va_intenttofile":
                    Va.Udo.Crm.MapdNote.TypeOfNote = "ITF";
                    Va.Udo.Crm.MapdNote.MessageName = "Create";

                    break;
                default:
                    Va.Udo.Crm.MapdNote.TypeOfNote = "Unknown";
                    break;
            }
        }

        Va.Udo.Crm.MapdNote.Message = "Would you like to create a note for this " + Va.Udo.Crm.MapdNote.TypeOfNote + "?";
        Va.Udo.Crm.MapdNote.Title = "Create Note";
        Va.Udo.Crm.MapdNote.ClearFormNotification("");
    },

    PromptQuestion: function () {
        return new Promise(function (resolve, reject) {
         //   var confirmStrings = { title: Va.Udo.Crm.MapdNote.Title, subtitle: "", text: Va.Udo.Crm.MapdNote.Message, confirmButtonLabel: "Yes", cancelButtonLabel: "No" };
         //   var confirmOptions = { height: 200, width: 450 };
            var title = Va.Udo.Crm.MapdNote.Title;
            var msg = Va.Udo.Crm.MapdNote.Message
           // Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
            UDO.Shared.openConfirmDialog(msg, title, 200,250, "Yes", "No")
                .then(
                    function (response) {
                        resolve(response.confirmed);
                    },
                    function (error) {
                        reject(error);
                    });
        });
        /* var pq = $.Deferred();        
         var buttons = Va.Udo.Crm.Scripts.Popup.PopupStyles.YesNo;
         var style = Va.Udo.Crm.Scripts.Popup.PopupStyles.Question;      
 
         
         Va.Udo.Crm.Scripts.Popup.MsgBox(Va.Udo.Crm.MapdNote.Message, buttons + style, Va.Udo.Crm.MapdNote.Title)        
             .done(function (data) {
                 pq.resolve(true);
             }).fail(function (data) {
                 pq.resolve(false);
             });
 
         return pq.promise();*/
    },

    CreateNote: function () {
        var cols = {};
        var cn = $.Deferred();


        if (formContext.getAttribute("udo_relatedveteranid") !== null) {
            var selectedItem = formContext.getAttribute("udo_relatedveteranid").getValue();
            cols["udo_veteranid@odata.bind"] = "/contacts" + selectedItem[0].id.replace("{", "(").replace("}", ")");
        }

        if (formContext.getAttribute("crme_relatedveteranid") !== null) {
            var selectedItem = formContext.getAttribute("crme_relatedveteranid").getValue();
            cols["udo_veteranid@odata.bind"] = "/contacts" + selectedItem[0].id.replace("{", "(").replace("}", ")");
        }

        if (formContext.getAttribute("udo_personid") !== null) {
            var erPerson = formContext.getAttribute("udo_personid").getValue();
            cols["udo_personid@odata.bind"] = "/udo_persons" + erPerson[0].id.replace("{", "(").replace("}", ")");
        }

        if (formContext.getAttribute("udo_idproofid") !== null) {
            var erIdProof = formContext.getAttribute("udo_idproofid").getValue();
            cols["udo_idproofid@odata.bind"] = "/udo_idproofs" + erIdProof[0].id.replace("{", "(").replace("}", ")");
        }

        cols.udo_name = "Notes from " + Va.Udo.Crm.MapdNote.TypeOfNote;

        cols.udo_claimid = formContext.getAttribute("udo_claimId") === null
            ? null
            : formContext.getAttribute("udo_claimId").getValue();
        cols.udo_participantid = formContext.getAttribute("udo_participantid") === null
            ? null
            : formContext.getAttribute("udo_participantid").getValue();






        cols.udo_notetext = Va.Udo.Crm.MapdNote.GenerateMapdNotes();
        cols.udo_editable = true;
        cols.udo_fromudo = true;
        cols.udo_createdt = new Date();
        cols.udo_datetime = new Date();
        //todo: update to webapi
        //var note = CrmRestKit2011.Create("udo_note", cols)
        webApi.CreateRecord(cols, "udo_note")
            .then(function (data) {
                Va.Udo.Crm.MapdNote.CreateNoteLog("CreateNote - Note record created");
                var lookup = new Array();
                lookup[0] = new Object();
                lookup[0].id = data.id;
                lookup[0].name = cols.udo_name;
                lookup[0].entityType = "udo_note";
                formContext.getAttribute("udo_note").setValue(lookup);
                formContext.getAttribute("udo_notecreated").setValue(true);
                formContext.data.save().then(function () {
                    cn.resolve("succuess");
                });
            },
                function (err) {
                    Va.Udo.Crm.MapdNote.CreateNoteLog("CreateNote - Error: " + err.message);
                    console.error(err);
                    Va.Udo.Crm.MapdNote.Error("An unexpected error occurred updating the " +
                        Va.Udo.Crm.MapdNote.TypeOfNote);
                    cn.reject(new Error(err));
                });

        return cn.promise();
    },
    CreateITFNote: function () {

        var generalBenefitType = "";
        var claimantFirstName = "";
        var claimantLastName = "";
        var intenttofilestatus = "";
        var noteText = "";

        var cols = {};
        var cn = $.Deferred();


        if (formContext.getAttribute("udo_veteranid") !== null) {
            var selectedItem = formContext.getAttribute("udo_veteranid").getValue();
            cols["udo_veteranid@odata.bind"] = "/contacts" + selectedItem[0].id.replace("{", "(").replace("}", ")");
        }

        if (formContext.getAttribute("udo_personid") !== null) {
            var erPerson = formContext.getAttribute("udo_personid").getValue();
            cols["udo_personid@odata.bind"] = "/udo_persons" + erPerson[0].id.replace("{", "(").replace("}", ")");
        }

        if (formContext.getAttribute("udo_idproofid") !== null) {
            var erIdProof = formContext.getAttribute("udo_idproofid").getValue();
            cols["udo_idproofid@odata.bind"] = "/udo_idproofs" + erIdProof[0].id.replace("{", "(").replace("}", ")");
        }

        cols.udo_name = "Notes from " + Va.Udo.Crm.MapdNote.TypeOfNote;

        cols.udo_participantid = formContext.getAttribute("va_claimantparticipantid") === null
            ? null
            : formContext.getAttribute("va_claimantparticipantid").getValue();


        if (formContext.getAttribute("va_generalbenefittype") !== null) {
            generalBenefitType = formContext.getAttribute("va_generalbenefittype").getText();
        }

        if (formContext.getAttribute("va_claimantfirstname") !== null) {
            claimantFirstName = formContext.getAttribute("va_claimantfirstname").getValue();
        }

        if (formContext.getAttribute("va_claimantlastname") !== null) {
            claimantLastName = formContext.getAttribute("va_claimantlastname").getValue();
        }

        if (formContext.getAttribute("va_intenttofilestatus") !== null) {
            intenttofilestatus = formContext.getAttribute("va_intenttofilestatus").getValue();
        }

        noteText = "ITF Submitted " + "\n";
        noteText = noteText + "Benefit: " + generalBenefitType + "\n";
        noteText = noteText + "Claimant: " + claimantFirstName + " " + claimantLastName + "\n";
        noteText = noteText + "ITF Status: " + intenttofilestatus + "\n";

        cols.udo_notetext = noteText;
        cols.udo_editable = true;
        cols.udo_fromudo = true;
        cols.udo_createdt = new Date();
        if (cols.udo_datetime !== null) {
            cols.udo_datetime = new Date();
        }
        //todo: update to webapi
        //var note = CrmRestKit2011.Create("udo_note", cols)
        webApi.CreateRecord(cols, "udo_note")
            .then(function (data) {
                Va.Udo.Crm.MapdNote.CreateNoteLog("CreateNote - Note record created");
                var lookup = new Array();
                lookup[0] = new Object();
                lookup[0].id = data.id;
                lookup[0].name = cols.udo_name;
                lookup[0].entityType = "udo_note";
                formContext.getAttribute("udo_note").setValue(lookup);
                formContext.getAttribute("udo_notecreated").setValue(true);
                formContext.data.save().then(function () {
                    cn.resolve("succuess");
                });
            },
                function (err) {
                    Va.Udo.Crm.MapdNote.CreateNoteLog("CreateNote - Error: " + err.message);
                    console.error(err);
                    Va.Udo.Crm.MapdNote.Error("An unexpected error occurred updating the " +
                        Va.Udo.Crm.MapdNote.TypeOfNote);
                    cn.reject(new Error(err));
                });

        return cn.promise();
    },
    //done
    GenerateMapdNotes: function () {

        var sentFormText = "";

        sentFormText = Va.Udo.Crm.MapdNote.Shared_AddTwoOptionText(sentFormText, "udo_pmc", "PMC, ");
        sentFormText = Va.Udo.Crm.MapdNote.Shared_AddTwoOptionText(sentFormText, "udo_nokletter", "NOK Letter, ");
        sentFormText = Va.Udo.Crm.MapdNote.Shared_AddTwoOptionText(sentFormText, "udo_21530", "21-530, ");
        sentFormText = Va.Udo.Crm.MapdNote.Shared_AddTwoOptionText(sentFormText, "udo_21534", "21-534, ");
        sentFormText = Va.Udo.Crm.MapdNote.Shared_AddTwoOptionText(sentFormText, "udo_401330", "40-1330, ");
        sentFormText = Va.Udo.Crm.MapdNote.Shared_AddTwoOptionText(sentFormText,
            "udo_other",
            formContext.getAttribute('udo_otherspecification') === null
                ? ""
                : formContext.getAttribute('udo_otherspecification').getValue());

        Va.Udo.Crm.MapdNote.CreateNoteLog("GenerateMapdNotes - " + sentFormText);

        var devNoteText = Va.Udo.Crm.MapdNote.BuildDevNoteText(sentFormText);

        return devNoteText;
    },
    //done
    BuildDevNoteText: function (sentFormText) {

        var devNoteText = "";

        switch (Va.Udo.Crm.MapdNote.EntityName) {
            case "udo_lettergeneration":

                var letterName = formContext.getAttribute('udo_letter') === null ? "" : formContext.getAttribute("udo_letter").getValue();
                devNoteText = "Letter: " + letterName[0].name + " ";

                break;
            case "udo_servicerequest":

                var serviceRequestName = formContext.getAttribute('udo_reqnumber') === null ? "" : formContext.getAttribute("udo_reqnumber").getValue();
                devNoteText = "Service Request: " + serviceRequestName + " ";

                break;
            default:

                break;
        }

        if (Va.Udo.Crm.MapdNote.MessageName === "Create") {
            devNoteText = devNoteText + "created. ";
        } else if (Va.Udo.Crm.MapdNote.MessageName === "Copy") {
            devNoteText = devNoteText + "copied. ";
        } else {
            devNoteText = devNoteText + "updated. ";
        }

        var ssn = formContext.getAttribute("udo_ssn");
        if (ssn !== undefined && ssn !== null) {
            devNoteText = devNoteText + "File #: " + ssn.getValue() + ". ";
        }

        devNoteText = Va.Udo.Crm.MapdNote.Shared_AddOptionSetText(devNoteText, "udo_issue", "Type: ");
        devNoteText = Va.Udo.Crm.MapdNote.Shared_AddOptionSetText(devNoteText, "udo_action", "Action: ");
        devNoteText = Va.Udo.Crm.MapdNote.Shared_AddEntityReferenceText(devNoteText, "udo_regionalofficeid", "SOJ: ");

        switch (Va.Udo.Crm.MapdNote.EntityName) {
            case "udo_lettergeneration":

                if (formContext.getAttribute('udo_description') !== null && formContext.getAttribute("udo_description").getValue() !== null) {
                    devNoteText = devNoteText + "\nDesc: " + formContext.getAttribute("udo_description").getValue() + " ";
                }
                break;

            case "udo_servicerequest":

                switch (formContext.getAttribute("udo_action").getText()) {
                    case "0820f": //0820f

                        devNoteText = devNoteText + "0820f sent for MOD to ROJ ";
                        devNoteText = Va.Udo.Crm.MapdNote.Shared_AddEntityReferenceText(devNoteText, "udo_regionalofficeid", "") + "\n";
                        var reportedby = formContext.getAttribute('udo_nameofreportingindividual') === null ? "" : formContext.getAttribute("udo_nameofreportingindividual").getValue();
                        devNoteText = devNoteText + "Reported By '" + reportedby + "'\n";

                        var depFirstName = formContext.getAttribute('udo_depfirstname').getValue();
                        var depLastName = formContext.getAttribute('udo_deplastname').getValue();
                        var reportedForFullName = (depFirstName === null ? "" : depFirstName) + " " + (depLastName === null ? "" : depLastName);

                        devNoteText = devNoteText + "Reported for '" + reportedForFullName + "',";
                        if (formContext.getAttribute('udo_depdateofbirth') !== null && formContext.getAttribute('udo_depdateofbirth').getValue() !== null) {
                            devNoteText = devNoteText + "DOB:  " + formContext.getAttribute('udo_depdateofbirth').getValue().toLocaleDateString("en-US") + ", ";
                        }

                        if (formContext.getAttribute('udo_depssn') !== null && formContext.getAttribute('udo_depssn').getValue() !== null) {
                            devNoteText = devNoteText + "SSN: " + formContext.getAttribute('udo_depssn').getValue();
                        }

                        if (formContext.getAttribute('udo_description') !== null && formContext.getAttribute("udo_description").getValue() !== null) {
                            devNoteText = devNoteText + "\n\nDesc: " + formContext.getAttribute("udo_description").getValue() + " ";
                        }

                        if (formContext.getAttribute('udo_letterscreated') !== null && formContext.getAttribute("udo_letterscreated").getValue() !== null) {
                            devNoteText = devNoteText + "\n\nThe following letters were sent: " + formContext.getAttribute("udo_letterscreated").getValue() + " ";
                        }

                        break;
                    case "0820d": //0820d

                        if (formContext.getAttribute('udo_dateofmissingpayment') !== null && formContext.getAttribute('udo_dateofmissingpayment').getValue() !== null) {
                            devNoteText = devNoteText + ".\n\nMissing Payment Date: " + formContext.getAttribute('udo_dateofmissingpayment').getValue().toLocaleDateString("en-US") + ". ";
                        }

                        if (formContext.getAttribute('udo_amtofpayments') !== null && formContext.getAttribute('udo_amtofpayments').getValue() !== null) {
                            devNoteText = devNoteText + ".\nMissing Payment Amt: $" + formContext.getAttribute('udo_amtofpayments').getValue().toFixed(2) + ". ";
                        }

                        break;
                    case "0820a": //0820a

                        

                        if (formContext.getAttribute('udo_description') !== null && formContext.getAttribute("udo_description").getValue() !== null) {
                            devNoteText = devNoteText + "\n\nDesc: " + formContext.getAttribute("udo_description").getValue() + " ";
                        }

                       

                        break;

                    case "0820": //0820

                        if (formContext.getAttribute('udo_description') !== null && formContext.getAttribute("udo_description").getValue() !== null) {
                            devNoteText = devNoteText + "\n\nDesc: " + formContext.getAttribute("udo_description").getValue() + " ";
                        }

                        break;
                    case "va_intenttofile":

                        break;
                    default:

                        break;
                }

                break;
            default:

                break;
        }

        Va.Udo.Crm.MapdNote.CreateNoteLog("BuildDevNoteText - " + devNoteText);

        return devNoteText;
    },
    //done
    Shared_AddTwoOptionText: function (sentFormText, twoOptionControlName, addFormText) {

        if (formContext.getAttribute(twoOptionControlName) !== null && formContext.getAttribute(twoOptionControlName).getValue() !== null) {
            var selectedTwoOption = formContext.getAttribute(twoOptionControlName).getValue();
            if (selectedTwoOption === true) {
                sentFormText = sentFormText.concat(addFormText);
            }
        }

        return sentFormText;
    },
    //done
    Shared_AddOptionSetText: function (devNoteText, optionSetControlName, addFormText) {

        if (formContext.getAttribute(optionSetControlName) !== null && formContext.getAttribute(optionSetControlName).getSelectedOption() !== null) {
            var selectedOptionSet = formContext.getAttribute(optionSetControlName).getSelectedOption();
            if (selectedOptionSet !== null) {
                devNoteText = devNoteText.concat(addFormText + formContext.getAttribute(optionSetControlName).getText() + " ");
            }
        }

        return devNoteText;
    },
    //done
    Shared_AddEntityReferenceText: function (devNoteText, entityReferenceControlName, addFormText) {

        if (formContext.getAttribute(entityReferenceControlName) !== null && formContext.getAttribute(entityReferenceControlName).getValue() !== null) {
            var entityReference = formContext.getAttribute(entityReferenceControlName).getValue();
            if (entityReference !== null) {
                devNoteText = devNoteText.concat(addFormText + entityReference[0].name + " ");
            }
        }

        return devNoteText;
    },
    //done
    Shared_AddDateText: function (devNoteText, dateControlName, addFormText) {

        if (formContext.getAttribute(dateControlName) !== null && formContext.getAttribute(dateControlName).getValue() !== null) {
            var dateValue = formContext.getAttribute(dateControlName).getValue();
            if (dateValue !== null) {

                var validDate = new Date();
                validDate.setDate(dateValue.getDate());

                var currentDay = (validDate.getDate() < 10 ? "0" : "") + validDate.getDate();
                var currentMonth = ((validDate.getMonth() + 1) < 10 ? "0" : "") + (validDate.getMonth() + 1);
                var currentYear = validDate.getFullYear();

                var timestamp = currentMonth + "/" + currentDay + "/" + currentYear;
                devNoteText = devNoteText.concat(addFormText + timestamp);
            }
        }

        return devNoteText;
    },
    //done
    Shared_GetNewDateText: function () {

        var fulldate = new Date();

        var currentDay = (fulldate.getDate() < 10 ? "0" : "") + fulldate.getDate();
        var currentMonth = ((fulldate.getMonth() + 1) < 10 ? "0" : "") + (fulldate.getMonth() + 1);
        var currentYear = fulldate.getFullYear();

        var timestamp = currentMonth + "/" + currentDay + "/" + currentYear;

        return timestamp;
    },
    //done
    Error: function (message) {
        Va.Udo.Crm.Scripts.Popup.MsgBox(message, Va.Udo.Crm.Scripts.Popup.PopupStyles.Exclamation, "Error");
    },
    //done
    CreateNoteLog: function (msg) {
        if (window.console && console.log) {

            try {

                var date = new Date();
                var currentDay = (date.getDate() < 10 ? "0" : "") + date.getDate();
                var currentMonth = ((date.getMonth() + 1) < 10 ? "0" : "") + (date.getMonth() + 1);
                var currentYear = date.getFullYear();

                var currentHours = (date.getHours() < 10 ? "0" : "") + date.getHours();
                var currentMinutes = (date.getMinutes() < 10 ? "0" : "") + date.getMinutes();
                var currentSeconds = (date.getSeconds() < 10 ? "0" : "") + date.getSeconds();

                var timestamp = "[ " + currentYear + "/" + currentMonth + "/" + currentDay + " " + currentHours + ":" + currentMinutes + ":" + currentSeconds + " ] - ";
                console.log(timestamp + msg);

            } catch (err) {
                console.error(err);
                console.log("Unexpected error occurred in CreateNoteLog");
            }
        }
    },
    //done
    SetFormNotification: function (fieldname, msg, type) {

        if (fieldname !== null && fieldname !== "") {
            formContext.getControl(fieldname).setNotification(msg);
        } else {
            formContext.ui.setFormNotification(msg, type, "3");
        }

    },
    //done
    ClearFormNotification: function (fieldname) {

        if (fieldname !== null && fieldname !== "") {
            formContext.getControl(fieldname).clearNotification();
        } else {
            formContext.ui.clearFormNotification("3");
        }

    }

}