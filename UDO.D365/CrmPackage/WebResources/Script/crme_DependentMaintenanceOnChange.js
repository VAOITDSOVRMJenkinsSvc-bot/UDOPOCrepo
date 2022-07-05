function SetFormIsDirty() {
    Xrm.Page.getAttribute('crme_formisdirty').setValue(true);
    Xrm.Page.getAttribute('crme_formisdirty').setSubmitMode("always");
}

function showSCRatigAlert(executionContext) {
    formContext = executionContext.getFormContext();
    var award_type = formContext.getAttribute("udo_awardtype").getValue();

    if (award_type === 751880001) { // Compensation
        //check if sc rating is less than 30
        console.log("Checking Valid Pension Submission..........");
        var dep_main_id = formContext.data.entity.getId().replace("{", "").replace("}", "");       
        Xrm.WebApi.online.retrieveRecord("crme_dependentmaintenance", dep_main_id, "?$select=activityid,_regardingobjectid_value").then(
            function success(depmaintenance) {
                if (depmaintenance != null) {
                    var vetid = depmaintenance["_regardingobjectid_value"];
                    var vetsnapShotQuery = "?$select=createdon,modifiedon,udo_effectivedate,udo_sccombinedrating,_udo_veteranid_value&$filter=_udo_veteranid_value eq " + vetid + "&$orderby=createdon desc&$top=1";
                    Xrm.WebApi.online.retrieveMultipleRecords("udo_veteransnapshot", vetsnapShotQuery).then(
                        function success(results) {
                            if (results != null && results.entities.length > 0 && results.entities[0] != null) {
                                var lastSnapshot = results.entities[0];
                                console.log("Last Snapshot is found: ");
                                console.log(lastSnapshot);
                                var SC_rating = Number(lastSnapshot["udo_sccombinedrating"]);                                
                                if (SC_rating < 30) {//compensation
                                    console.log("INVALID award type...");
                                    var scTitle = "Veteran's Rating is Below 30%";
                                    var scMessage = "\n\r" + "This Veteran's compensation rating percentage is below 30%, and the Dependent Maintenance cannot be submitted.";
                                    UDO.Shared.openAlertDialog(scMessage, scTitle, 300, 450);
                                } 
                            } 
                        },
                        function (error) {
                            console.log("Error on reading most recent vet snapshot of related veteran" + error.message);
                            console.log(error.message);                           
                        }
                    );
                } 
            },
            function (error) {
                console.log("Error on reading dependent maintenance type of related veteran" + error.message);
                console.log(error.message);                
            }
        );
    }
}

function setFormRemarks(executionContext) {
    // Get form context and current dependent maintainance record id
    formContext = executionContext.getFormContext();
    var depMaintRecordId = formContext.data.entity.getId();

    // Create instance of webApi used for updating remarks fields
    lib = new CrmCommonJS.CrmCommon(version, executionContext);
    webApi = lib.WebApi;

    // Determine what type of award is present
    var awardType = "Compensation";

    var control_awardType = formContext.getAttribute("udo_awardtype");
    if (control_awardType !== null) {
        if (control_awardType.getValue() !== null) {
            if (control_awardType.getValue() === 751880000) { // Pension
                awardType = "Pension"
            }
        }
    }

    // If award is Pension, compose dynamic remarks
    var remarks = "No remarks."

    if (awardType === "Pension") {

        // Get fields to build remarks
        var dependentHasIncomeInLast365Days = "No.";

        var control_dependentHasIncomeInLast365Days = formContext.getAttribute("udo_dependentshaveincome");
        if (control_dependentHasIncomeInLast365Days !== null && control_dependentHasIncomeInLast365Days.getValue() !== null) {
            if (control_dependentHasIncomeInLast365Days.getValue() === 751880000) { //'Yes
                dependentHasIncomeInLast365Days = "Yes."
            }
        } else {
            // If we have a blank dependent income in last 365 days, we need to stop updating the remarks.
            console.log("Dependent income in last 365 days was blank, aborting remarks update.");
            return;
        }

        var houseHoldNetWorthGreaterThanThreshold = "No.";

        var control_houseHoldNetWorthGreaterThanThreshold = formContext.getAttribute("udo_ishouseholdgreaterthanthreshhold");
        if (control_houseHoldNetWorthGreaterThanThreshold !== null && control_houseHoldNetWorthGreaterThanThreshold.getValue() !== null) {
            if (control_houseHoldNetWorthGreaterThanThreshold.getValue() === 751880000) { //'Yes'
                houseHoldNetWorthGreaterThanThreshold = "Yes."
            }
        } else {
            // If we have a blank net worth threshhold flag, we need to stop updating the remarks.
            console.log("Household net worth threshold flag was blank, aborting remarks update.");
            return;
        }

        var houseHoldNetWorthThreshold = "$0.00";

        var control_houseHoldNetWorthThreshold = formContext.getAttribute("udo_householdnetworththreshold");
        if (control_houseHoldNetWorthThreshold !== null && control_houseHoldNetWorthThreshold.getValue() !== null) {
            houseHoldNetWorthThreshold = control_houseHoldNetWorthThreshold.getValue();
        } else {
            // If we have a blank dependent income in last 365 days, we need to stop updating the remarks.
            console.log("Dependent income in last 365 days was blank, aborting remarks update.");
            return;
        }

        remarks = `Did the dependent that you are submitting this claim for have income in the last 365 days? ${dependentHasIncomeInLast365Days}     `;
        remarks = remarks + `Household net worth threshold is less than ${houseHoldNetWorthThreshold}? ${houseHoldNetWorthGreaterThanThreshold}   `;
    }

    // Update remarks in record object
    var dependentRecord = {};

    dependentRecord.udo_incomeremarks = remarks;

    // Make call to update remarks on the record
    var filter = `$filter=crme_dependentmaintenance/activityid eq '${depMaintRecordId.replace('{', '').replace('}', '')}'`;

    webApi.RetrieveMultiple("crme_dependent", null, filter)
        .then(
            function reply(data) {
                if (data && data.length > 0) {
                    data.forEach(dependent => {
                        webApi.UpdateRecord(dependent["crme_dependentid"], "crme_dependent", dependentRecord)
                            .then(function reply(response) {
                                console.log(response);
                            });
                    });
                }
            }
        );
}