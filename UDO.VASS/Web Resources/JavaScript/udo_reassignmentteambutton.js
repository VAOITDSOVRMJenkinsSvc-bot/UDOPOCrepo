var VASS = VASS || {};



VASS.Reassign = {
    
    //function to update a flag to trigger a worklow
    updateReassignmentFlag: function (primaryEntityId, primaryControl) {
        try {
            var onAfterHoursMessage = "Would you like to transfer ownership of current and future VASS interactions for this Veteran to the VASS Reassignment team? \n\n *Ensure that the callback request is outside of your NCC's hours of duty";
            var onLanguageMessage = "Would you like to transfer ownership of current and future VASS interactions for this Veteran to the VASS Reassignment team?";
            var formContext = primaryControl;
            var reassignmentReasson = formContext.getAttribute("udo_reassignmentreasonnew").getValue();
            if (reassignmentReasson == null) {
                Xrm.Navigation.openAlertDialog({ "title": "Reassignment Reason Required", "text": "Failed to transfer all VASS interactions. Please select the Reassignment reason and try again." }, { "height": 100, "width": 150 });
            }
            else  { 
                Xrm.Navigation.openConfirmDialog({ "confirmButtonLabel": "Yes", "cancelButtonLabel": "No", "title": "Transfer Ownership", "text": reassignmentReasson == '752280000' ? onAfterHoursMessage : onLanguageMessage },
                    { "height": 100, "width": 150 })
                    .then(
                        function (success) {
                            if (success.confirmed) {
                                formContext.data.save().then(function () {
                                    var interactionId = primaryEntityId.replace('{', '').replace('}', '');
                                    var data = { "udo_updatereassignmentflag": true };
                                    Xrm.WebApi.updateRecord("udo_interaction", interactionId, data)
                                        .then(function (result) {
                                            console.log("Assignment operation complete");
                                            formContext.data.refresh();

                                        }, function (error) {
                                            Xrm.Navigation.openAlertDialog({ "title": "Transfer Ownership", "text": "Failed to tranfser all VASS interactions. Please contact your DMO." }, { "height": 100, "width": 150 });
                                            console.log(error.message);
                                        });
                                });
                            }
                            else {
                               
                                console.log("Transfer operation Cancelled.");
                            }
                        },
                        function () {
                            Xrm.Navigation.openAlertDialog({ "title": "Transfer Ownership", "text": "Transfer operation failed." }, { "height": 100, "width": 150 });
                        });
            }

        }
        catch (ex) {
            Xrm.Navigation.openAlertDialog({ "title": "Transfer Ownership", "text": "Failed to transfer all VASS interactions. Please contact your DMO." }, { "height": 100, "width": 150 });
        }
   }
};