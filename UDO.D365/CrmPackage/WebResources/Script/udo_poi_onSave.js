"use strict";
var formContext = null;

function onSave(exCon) {


    formContext = exCon.getFormContext();
    console.log("Here");
    // formContext.ui.clearFormNotification("NoteNotGenerated");
    // formContext.ui.clearFormNotification("NoteGenerated");
    //if (exCon.getEventArgs().getSaveMode() === 70) {
    //    exCon.getEventArgs().preventDefault(); // Disabling auto save
    //    return false;
    //}
    //if (exCon.getEventArgs().getSaveMode() !== 70) {


    //   formContext.data.refresh(true);

    //formContext.data.save(true).then(
    //    function (success) {
    //        console.log(success);
    //        formContext.ui.close();
    //        CreateSummaryNote(executionContext);

    //    },
    //    function (error) {
    //        console.log(error);
    //    });

    //formContext.data.save(CreateSummaryNote);

    //setTimeout(() => { CreateSummaryNote()  }, 3000);
    var formType = formContext.ui.getFormType();
    var status = formContext.getAttribute("statecode").getValue();
    if (formType === 1 || status == 1) {
        CreateSummaryNote();
    }

}

//}


function CreateSummaryNote() {
    // var formContext = executionContext.getFormContext();
    var parameters = {};
    var entity = {};
    var veteranentity = {};
    var idproofentity = {};
    entity.id = formContext.data.entity.getId().replace("{", "").replace("}", "");
    entity.entityType = "udo_pointofinteraction";
    parameters.entity = entity;
    parameters.requestedAction = formContext.getAttribute("udo_requestedaction").getValue().toString();
    if (formContext.getAttribute("statecode") == 1) {
        parameters.description = formContext.getAttribute("statuscode").getText();
    }
    else {
        parameters.description = formContext.getAttribute("udo_description").getValue();
    }
    if (formContext.getAttribute("udo_veteran").getValue() !== null) {
        veteranentity.id = formContext.getAttribute("udo_veteran").getValue()[0].id.replace("{", "").replace("}", "");
    }
    else {
        veteranentity.id = "";
    }
    veteranentity.entityType = "contact";
    parameters.veteranentity = veteranentity;
    if (formContext.getAttribute("udo_idproof").getValue() !== null) {
        idproofentity.id = formContext.getAttribute("udo_idproof").getValue()[0].id.replace("{", "").replace("}", "");
    }
    else {
        idproofentity.id = "";
    }

    idproofentity.entityType = "udo_idproof";
    parameters.idproofentity = idproofentity;


    var udo_CreateNoteRequest = {

        ParentEntityReference: parameters.entity,
        VeteranPOI: parameters.veteranentity,
        IdProofPOI: parameters.idproofentity,
        POI_Description: parameters.description,
        POIRequestedAction: parameters.requestedAction,

        getMetadata: function () {
            return {

                parameterTypes: {
                    "ParentEntityReference": {
                        "typeName": "mscrm.crmbaseentity",
                        "structuralProperty": 5
                    },
                    "VeteranPOI": {
                        "typeName": "mscrm.crmbaseentity",
                        "structuralProperty": 5
                    },
                    "IdProofPOI": {
                        "typeName": "mscrm.crmbaseentity",
                        "structuralProperty": 5
                    },
                    "POI_Description": {
                        "typeName": "Edm.String",
                        "structuralProperty": 1
                    },
                    "POIRequestedAction": {
                        "typeName": "Edm.String",
                        "structuralProperty": 1
                    }
                },
                operationType: 0,
                operationName: "udo_CreateNote"
            };
        }
    };

    if (veteranentity.id !== "" && idproofentity.id !== "") {

        UDO.Shared.ExecuteAction(udo_CreateNoteRequest)
            .then(function (data) {
                create_callback(data);
            })
            .catch(function (err) {
                UDO.Shared.CloseProgressIndicator();
                UDO.Shared.openAlertDialog(err.message, "", 120, 260);
            });
    }


}

function create_callback(data) {
    if (data.DataIssue === true || data.Timeout === true || data.Exception === true) {
        formContext.ui.setFormNotification("System failed to generate a note for this request.", "INFO", "NoteNotGenerated");
    }
    else {
        if (data !== null) {
            if (data.NoteID !== null) {
                //add code to show that note was created.
                formContext.ui.setFormNotification("System generated a note for this request.", "INFO", "NoteGenerated");
            }
        }

    }
}