var VASS = VASS || {};
VASS.CallAttempt = VASS.CallAttempt || {};

/* Dispositions
 * 752280000 - Successful Contact
 * 752280001 - Unable to Contact/No Voicemail Required
 * 752280002 - Left Voicemail
 * 752280003 - Do Not Contact Request
 * 752280004 - Call Back Request
 * 752280005 - Bad Phone Number
 */

var promptMessages = {
    "752280000": "There is a call back associated with this Veteran. Please clear out if applicable.",
    "752280001": "There is a call back associated with this Veteran. Please clear out or reschedule if applicable.",
    "752280003": "There is a call back associated with this Veteran. Please clear out.",
    "752280002": "There is a call back associated with this Veteran. Please clear out callback date or reschedule if applicable.",
    "752280004": "There is a call back associated with this Veteran. Please update callback date and time if applicable.",
    "752280005": "There is a call back associated with this Veteran. Please clear out or reschedule if applicable."
}

VASS.CallAttempt.onSave = function (executionContext) {
    var formContext = executionContext.getFormContext();
    var disposition = formContext.getAttribute("udo_disposition").getValue();
    var direction = formContext.getAttribute("udo_direction").getValue();


    var interactionLUObj = formContext.getAttribute("udo_interactionid"); //get the Lookup Object 
    var interactionId;
    if (interactionLUObj != null) {
        var luObjValues = interactionLUObj.getValue();//get the Lookup Value 
        if (luObjValues != null) {
            interactionId = luObjValues[0].id.replace('{', '').replace('}', ''); //get the Lookup id            
        }
    }

    var callbackDateTime;
    var reassignmentReason;
    Xrm.WebApi.online.retrieveRecord("udo_interaction", interactionId, "?$select=udo_mheocallbackdatetime,udo_reassignmentreasonnew").then(
        function success(result) {
            callbackDateTime = result["udo_mheocallbackdatetime"];
            reassignmentReason = result["udo_reassignmentreasonnew"];
        },
        function (error) {
            console.log(error.message);
        }
    ).then(
        function () {
            if (callbackDateTime != null) {
                var alertStrings = { confirmButtonLabel: "OK", text: "", title: "Call Back Date/Time" };
                var alertOptions = { height: 120, width: 260 };

                if ((direction == 752280000 && disposition == 752280004) 
                    ||
                    (direction = 752280000
                        && (disposition == 752280001 || disposition == 752280002 || disposition == 752280003 || disposition == 752280005)
                        && reassignmentReason == 752280000))
                {
                    var message = promptMessages[disposition];
                    alertStrings.text = message;
                    Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                        function success(result) {
                            console.log("Alert dialog closed");
                        },
                        function (error) {
                            console.log(error.message);
                        }
                    );
                }                

                var dateNow = new Date();
                var dateCallBackDateTime = new Date(callbackDateTime);

                if (disposition != 752280000 && disposition != 752280004 && dateCallBackDateTime < dateNow && reassignmentReason == 752280001) {
                    var entity = {};
                    entity.udo_mheocallbackdatetime = null;

                    Xrm.WebApi.online.updateRecord("udo_interaction", interactionId, entity).then(
                        function success(result) {
                            var updatedEntityId = result.id;                           
                        },
                        function (error) {
                            console.log(error.message);
                        }
                    );                    
                }
            }
        }
    );
}