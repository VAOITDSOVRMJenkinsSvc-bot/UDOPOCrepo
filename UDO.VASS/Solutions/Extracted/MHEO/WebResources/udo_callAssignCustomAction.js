var VASS = VASS || {};

VASS.Transfer = {
    // calls the take ownership action
    //configure ribbon button to pass firstprimaryid
    assignFutureInteractions: function (primaryEntityId,primaryControl) {
        try {

            Xrm.Navigation.openAlertDialog({ "confirmButtonLabel": "Yes", "title": "Transfer Ownership", "text": "Would you like to take ownership of current and future VASS interactions for this Veteran?" }, null).then(
                function () {

                    //action name: udo_VASSAssignFutureInteractions
                    var interactionId = primaryEntityId.replace('{', '').replace('}', '');
                    //bound entity
                    var target = {};
                    target.entityType = "udo_interaction";
                    target.id = interactionId;

                    var req = {};
                    req.entity = target;
                    req.getMetadata = function () {
                        return {
                            boundParameter: "entity",
                            parameterTypes: {
                                "entity": {
                                    typeName: "mscrm.udo_interaction",
                                    structuralProperty: 5
                                }
                            },
                            operationType: 0,
                            operationName: "udo_VASSAssignFutureInteractions"
                        };
                    };

                    Xrm.WebApi.online.execute(req).then(
                        function (data) {
                            var e = data;
                            //Xrm.Navigation.openAlertDialog({ "title": "Transfer Ownership", "text": "You have taken the ownership of all future VASS interactions for this veteran." }, null);
                            primaryControl.data.refresh();
                        },
                        function (error) {
                            var errMsg = error.message;
                            Xrm.Navigation.openAlertDialog({ "title": "Transfer Ownership", "text": "Failed to transer all VASS interactions. Please contact your DMO." }, null);
                        }
                    );
                }, null);
        }
        catch (ex) {
            Xrm.Navigation.openAlertDialog({ "title": "Transfer Ownership", "text": "Failed to transer all VASS interactions. Please contact your DMO." }, null);
        }
    },

    callTransferWorkflow: function (primaryEntityId, primaryControl) {
        Xrm.Navigation.openAlertDialog({ "confirmButtonLabel": "Yes", "title": "Transfer Ownership", "text": "Would you like to take ownership of current and future VASS interactions for this Veteran?" }, null).then(
            function () {
                //get workflow id, build an execute workflow request object and trigger the call
                Xrm.WebApi.retrieveMultipleRecords("workflow", "?$select=name&$filter=name eq 'VASS%20-%20Take%20Ownership'&$top=1").then(function (results) {
                    if (results.entities.length > 0) {
                        var wf = results.entities[0];
                        var wfid = "3bf4d7f8-0e96-45cc-b024-d129019d093a";
                        //Create Workflow request
                        var executeWorflowRequest = {};
                        executeWorflowRequest.EntityId = { "guid": primaryEntityId };
                        executeWorflowRequest.entity = { "id": wfid, "entityType": "workflow" };
                        executeWorflowRequest.getMetadata = function () {
                            return {
                                boundParameter: "entity",
                                parameterTypes: {
                                    "entity": { "typeName": "mscrm.workflow", "structuralProperty": 5 },
                                    "EntityId": {"typeName":"Edm.Guid","structuralProperty":1}
                                },
                                operationType: 0,
                                operationName:"ExecuteWorkflow"
                            };
                        };
                        Xrm.WebApi.online.execute(executeWorflowRequest).then(function (result) {
                            if (result.ok) {
                                alert("Assignment Complete");
                            }
                        }, function (err) {
                            alert("Failed Assignment");
                        });
                    }

                }, function (err) {
                        alert("Failed to get workflow;");
                });
            }, null);
    },

    //function to update a flag to trigger a worklow
    updateTransferOwershipFlag: function (primaryEntityId, primaryControl) {
        try {
            var formContext = primaryControl;
            Xrm.Navigation.openConfirmDialog({ "confirmButtonLabel": "Yes","cancelButtonLabel":"No", "title": "Transfer Ownership", "text": "Would you like to take ownership of current and future VASS interactions for this Veteran?" },
                { "height": 100, "width": 150 })
                .then(
                    function (success) {
                        if (success.confirmed) {
                            //action name: udo_VASSAssignFutureInteractions
                            var interactionId = primaryEntityId.replace('{', '').replace('}', '');
                            var data = { "udo_transferownership": true };
                            Xrm.WebApi.updateRecord("udo_interaction", interactionId, data)
                                .then(function (result) {
                                    console.log("Assignment operation complete");
                                    formContext.data.refresh();

                                }, function (error) {
                                    Xrm.Navigation.openAlertDialog({ "title": "Transfer Ownership", "text": "Failed to tranfser all VASS interactions. Please contact your DMO." }, { "height": 100, "width": 150 });
                                    console.log(error.message);
                                });
                        }
                        else {
                            //Xrm.Navigation.openAlertDialog({ "title": "Transfer Ownership", "text": "Transfer operation cancelled." }, { "height": 100, "width": 150 });
                            console.log("Transfer operation Cancelled.");
                        }
                    },
                    function () {
                        Xrm.Navigation.openAlertDialog({ "title": "Transfer Ownership", "text": "Transfer operation failed." }, { "height": 100, "width": 150 });
                    });
                   
        }
        catch (ex) {
            Xrm.Navigation.openAlertDialog({ "title": "Transfer Ownership", "text": "Failed to transfer all VASS interactions. Please contact your DMO." },  { "height": 100, "width": 150 });
        }
    }

};
