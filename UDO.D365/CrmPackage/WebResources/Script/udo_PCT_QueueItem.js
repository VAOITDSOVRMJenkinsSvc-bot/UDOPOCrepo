//not used
var CONSTANTS = {
    STATECODE: { ACTIVE: 0, INACTIVE: 1, 0: "Active", 1: "Inactive" },
    STATUSCODE: {
        ACTIVE: 1, INACTIVE: 2, HOLD: 752280000, COMPLETE: 752280001, WALKEDOUT: 752280002,
        0: "Active", 1: "Inactive", 752280000: "Hold", 752280001: "Complete", 752280002: "Walkedout"
    }
};

function ResolveQueueItem(gridControl, selectedItems, entityTypeCode, queueAction) {

    debugger;

    if ((gridControl) &&
			(selectedItems && selectedItems.length > 0) &&
			(entityTypeCode) &&
			(queueAction && queueAction.length > 0)) {

        var selectedItem = selectedItems[0];
        //alert("Id=" + selectedItem.Id + "\nName=" + selectedItem.Name + "\nTypeCode=" +
        //	selectedItem.TypeCode.toString() + "\nTypeName=" + selectedItem.TypeName);

        if (selectedItem.TypeName == "udo_interaction") {

            GetInteractionOwner(gridControl, selectedItem, entityTypeCode, queueAction);

        } else {
            console.group("ResolveQueueItem - Queue Item not tied to Interaction");
            console.log(queueAction + " - Queue Item not tied to Interaction");
            console.groupEnd();
            alert("You must seleect a Queue Item linked to an Interaction to continue");
        }
    } else {
        console.group("ResolveQueueItem - CRM parameters");
        console.log("At least one of the CRM parameters is either null or blank");
        console.groupEnd();
        alert("At least one of the CRM parameters is either null or blank");
    }
}

function GetInteractionOwner(gridControl, selectedItem, entityTypeCode, queueAction) {
    
    columns = ['OwnerId'];
    filter = "udo_interactionid eq guid'" + selectedItem.Id + "'";

    CrmRestKit2011.ByQuery('udo_interaction', columns, filter, false)
		.fail(function (err) {
		    alert("Unexpected Error Occurred - " + err.responseJSON.error.message.value);
		})
		.done(function (data) {

		    debugger;
		    if (data && data.d.results.length > 0) {

		        var interactionOwnerId = data.d.results[0].OwnerId;
		        if (queueAction == "queueitemresolutioncomplete") {
		            CheckQueueItem(gridControl, selectedItem, entityTypeCode, queueAction, interactionOwnerId);
		        } else if (queueAction == "queueitemcancelinterview") {
		            QueueItemCancelInterview(gridControl, selectedItem, entityTypeCode, queueAction, interactionOwnerId);
		        } else {

		            console.group("ResolveQueueItem - Invalid queueAction");
		            console.log(queueAction + " - is an invalid queueAction paramenter");
		            console.groupEnd();
		            alert("Invalid QueueItem Action");
		        }

		    } else {
		        alert("Unexpected error - Queue Item not found");
		    }
		});

}

function QueueItemCancelInterview(gridControl, selectedItem, entityTypeCode, queueAction, interactionOwnerId) {

    columns = ['WorkerId'];
    filter = "ObjectId/Id eq guid'" + selectedItem.Id + "'";

    CrmRestKit2011.ByQuery('QueueItem', columns, filter, false)
		.fail(function (err) {
		    alert("Unexpected Error Occurred - " + err.responseJSON.error.message.value);
		})
		.done(function (data) {

		    debugger;
		    if (data && data.d.results.length > 0) {
		        if (data.d.results[0].WorkerId.Id == null) {

		            var choice = confirm("Are you sure you want to Cancel this Interview?");
		            if (choice == true) {
		                ChangeStatus($, gridControl, selectedItem, entityTypeCode,
							queueAction, interactionOwnerId, CONSTANTS.STATECODE.INACTIVE, CONSTANTS.STATUSCODE.WALKEDOUT, function () {
							    gridControl.refresh()
							});
		            }

		        } else {
		            alert("This Queue Item is currently being worked on and you not able to Cancel this Interview at this time.");
		        }
		    } else {
		        console.group("QueueItemCancelInterview - Queue Item not found");
		        console.log("Queue Item not found - " + selectedItem.Id);
		        console.groupEnd();
		        alert("Unexpected error - Queue Item not found");
		    }
		});
}

function CheckQueueItem(gridControl, selectedItem, entityTypeCode, queueAction, interactionOwnerId) {

    columns = ['WorkerId'];
    filter = "ObjectId/Id eq guid'" + selectedItem.Id + "'";

    CrmRestKit2011.ByQuery('QueueItem', columns, filter, false)
		.fail(function (err) {
		    alert("Unexpected Error Occurred - " + err.responseJSON.error.message.value);
		})
		.done(function (data) {

		    debugger;
		    if (data && data.d.results.length > 0) {
		        if (data.d.results[0].WorkerId.Id == null) {

		            var choice = confirm("Are you sure you would Complete this Interview?.  If you click OK, then this Interview will be completed and will be removed from the Queue.  If you click CANCEL, then this Interview will not be changed.");
		            if (choice == true) {
		                CheckForHomelessSpecialSituation(gridControl, selectedItem, entityTypeCode, queueAction, interactionOwnerId);
		            }

		        } else {
		            alert("This Queue Item is currently being worked on and you not able to perform a Resolution Complete at this time.");
		        }
		    } else {
		        alert("Unexpected error - Queue Item not found");
		    }
		});
}

function CheckForHomelessSpecialSituation(gridControl, selectedItem, entityTypeCode, queueAction, interactionOwnerId) {

    columns = ['udo_name', 'udo_SpecialSituationId', 'udo_InteractionId'];
    filter = "udo_name eq 'Homeless' and udo_InteractionId/Id eq guid'" + selectedItem.Id + "'";

    CrmRestKit2011.ByQuery('udo_interactionspecialsituation', columns, filter, false)
		.fail(function (err) {
		    alert("Unexpected Error Occurred - " + err.responseJSON.error.message.value);
		})
		.done(function (data) {

		    debugger;
		    if (data && data.d.results.length > 0) {

		        CheckForInteractionDisposition(gridControl, selectedItem, entityTypeCode, queueAction, interactionOwnerId);

		    } else {

		        ChangeStatus($, gridControl, selectedItem, entityTypeCode, queueAction, interactionOwnerId, CONSTANTS.STATECODE.INACTIVE, CONSTANTS.STATUSCODE.COMPLETE, function () {
		            gridControl.refresh();
		        });
		    }
		});
}

function CheckForInteractionDisposition(gridControl, selectedItem, entityTypeCode, queueAction, interactionOwnerId) {

    columns = ['udo_name', 'udo_interactionid'];
    filter = "udo_interactionid/Id eq guid'" + selectedItem.Id + "'";

    CrmRestKit2011.ByQuery('udo_interactiondisposition', columns, filter, false)
		.fail(function (err) {
		    alert("Unexpected Error Occurred - " + err.responseJSON.error.message.value);
		})
		.done(function (data) {

		    debugger;
		    if (data && data.d.results.length > 0) {

		        ChangeStatus($, gridControl, selectedItem, entityTypeCode, queueAction, interactionOwnerId, CONSTANTS.STATECODE.INACTIVE, CONSTANTS.STATUSCODE.COMPLETE, function () {
		            gridControl.refresh();
		        });
		    } else {

		        var vDialogOption = new Xrm.DialogOptions;
		        vDialogOption.width = 600;
		        vDialogOption.height = 350;

		        Xrm.Internal.openDialog("/WebResources/udo_WalkIn_ResolutionComplete.html", vDialogOption, null, null, function (returnValue) {

		            if (returnValue) {
		                if (returnValue.CreateDisposition) {

		                    ChangeStatus($, gridControl, selectedItem, entityTypeCode, queueAction, interactionOwnerId, CONSTANTS.STATECODE.INACTIVE, CONSTANTS.STATUSCODE.COMPLETE, function () {
		                        gridControl.refresh();
		                    });
		                }
		            }
		        });
		    }
		});
}

function ChangeStatus(jQuery, gridControl, selectedItem, entityTypeCode, queueAction, interactionOwnerId, stateCode, statusCode, callBack) {
    if (!jQuery) jQuery = $;
    Sdk.jQ.setJQueryVariable(jQuery);
    var id = selectedItem.Id.toString();
    id = id.replace("{", "");
    id = id.replace("}", "");
    var req = new Sdk.SetStateRequest(new Sdk.EntityReference(selectedItem.TypeName, id), stateCode, statusCode);
    return Sdk.jQ.execute(req).done(callBack);
}

function CreateInteractionDisposition(gridControl, selectedItem, entityTypeCode, queueAction, interactionOwnerId, returnValue) {

    var cols = {};

    cols.udo_name = "Interaction Disposition";
    cols.udo_interactionid = { Id: selectedItem.Id, LogicalName: selectedItem.TypeName };

    if (returnValue.Shelters_Contacted == "1") {
        cols.udo_shelterscontacted = true;
    } else {
        cols.udo_shelterscontacted = false;
    }

    if (returnValue.Agencies_Contacted == "1") {
        cols.udo_agenciescontacted = true;
    } else {
        cols.udo_agenciescontacted = false;
    }

    if (returnValue.ReferredHCMI == "1") {
        cols.udo_referredhcmianddolprograms = true;
    } else {
        cols.udo_referredhcmianddolprograms = false;
    }

    if (returnValue.Seeking_Assistance == "1") {
        cols.udo_seekingassistancefromro = true;
    } else {
        cols.udo_seekingassistancefromro = false;
    }

    return CrmRestKit2011.Create("udo_interactiondisposition", cols);
}