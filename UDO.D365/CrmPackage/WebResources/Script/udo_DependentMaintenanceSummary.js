"use strict";

function onLoad() {
   var dmId = getDataParams();

    retrieveDependentSummary(dmId);
}

function getDataParams() {
    var id = "";
    var vals = [];
    if (location.search !== "") {
        vals = location.search.substr(1).split("&");
        for (var i in vals) {
            vals[i] = vals[i].replace(/\+/g, " ").split("=");
        }
        for (var i in vals) {
            if (vals[i][0].toLowerCase() === "id") {
                id = vals[i][1].replace("{", "").replace("}", "");
                break;
            }
        }
    }
    return id;
}

function retrieveDependentSummary(activityId) {
    console.log("Retrieving dependent summary for DM: " + activityId);

    var tableBody = document.getElementById("dmSummaryTableBody");
    if (tableBody === null) {
        console.log("Summary html table body does not exist on form.");
        return;
    }

    if (activityId === undefined) {
        createNoDependentsRow(tableBody);
    }
    var fetchXml = "<fetch>" +
        "<entity name='crme_dependent'>" +
        "<attribute name='statecode' />" +
        "<attribute name='crme_name'/>" +
        "<order attribute='crme_dependentrelationship' descending='true'/>" +
        "<filter type='and'>" +
            "<condition attribute='crme_maintenancetype' operator='ne' value='935950002'/>" + // display all but "System" types
        "</filter>" +
        "<attribute name='crme_lastname'/>" +
        "<attribute name='crme_firstname'/>" +
        "<attribute name='crme_dependentrelationship'/>" +
        "<attribute name='crme_ssn'/>" +
        "<attribute name='crme_dob'/>" +
        "<attribute name='crme_maintenancetype'/>" +
        "<attribute name='crme_awardind'/>" +
        "<attribute name='crme_dependentid'/>" +
        "<attribute name='crme_childage1823inschool'/>" +
        "<link-entity name='crme_dependentmaintenance' from='activityid' to='crme_dependentmaintenance' alias='dm'>" +
            "<filter type='and'>" +
                "<condition attribute='activityid' operator='eq' uitype='crme_dependentmaintenance' value='" + activityId + "'/>" +
            "</filter>" +
        "</link-entity>" +
        "</entity>" +
        "</fetch > ";
        
        fetchXml = "?fetchXml="+encodeURIComponent(fetchXml);

    Xrm.WebApi.retrieveMultipleRecords("crme_dependent", fetchXml)
        .then(function success(data) {
            if (data.value === null || data.value.length === 0) {
                createNoDependentsRow();
            } else {            
                var dependentRowCnt = data.value.length;
                var lastDependentWasChild = false;

                for (var i = dependentRowCnt - 1; i >= 0; i--) {
                    console.log(data.value[i]);
                    var maintenanceType = data.value[i]["crme_maintenancetype"];
                    console.log(data.value[i]["crme_firstname"] + " "  + maintenanceType);

                    if (maintenanceType === 935950000 || maintenanceType === 935950001 || maintenanceType === 752280000) { // Add, Edit, Remove dependent
                        var relationshipType = data.value[i]["crme_dependentrelationship"];

                        if (lastDependentWasChild && relationshipType === 935950001) {
                            tableBody = createChildHeaderRow(tableBody);
                        }

                        var row = tableBody.insertRow(0);

                        if (relationshipType !== 935950001) {
                            // In School
                            var inSchool = data.value[i]["crme_childage1823inschool"];
                            var cell3 = row.insertCell(0);
                            cell3.setAttribute("SCOPE", "row");
                            
                            var isinSchool = "";
                            if (inSchool === true) {
                                isinSchool = "Yes";
                            } else {
                                isinSchool = "No";
                            }

                            cell3.innerHTML =
                            "<div class='dependentRow'>" +
                            isinSchool
                            "</div>";
                        }

                        // Maintenance Type
                        var cell6 = row.insertCell(0);
                        cell6.setAttribute("SCOPE", "row");
                        
                        var maintType;
                        switch (data.value[i]["crme_maintenancetype"]) {
                            case 935950000:
                                maintType = "Add";
                                break;
                            case 752280000:
                                maintType = "Edit";
                                break;
                            case 935950001:
                                maintType = "Remove";
                                break;
                            default:
                                maintType = "";
                        }
                        
                        cell6.innerHTML =
                            "<div class='dependentRow'>" +
                               maintType +
                            "</div>";

                        // SSN
                        var cell5 = row.insertCell(0);
                        cell5.setAttribute("SCOPE", "row");

                        var ssn = data.value[i]["crme_ssn"];
                        if (ssn === undefined) {
                            ssn = "--";
                        } else {
                            ssn = ssn.substr(5); // Last 4 digits only displayed
                        }

                        cell5.innerHTML =
                            "<div class='dependentRow'>" +
                                ssn +
                            "</div>";
                        
                        // DOB
                        var cell4 = row.insertCell(0);
                        cell4.setAttribute("SCOPE", "row");

                        var dob = new Date(data.value[i]["crme_dob"]);
                        var dt = + (dob.getUTCMonth() + 1) + '/' + dob.getUTCDate() + '/' + dob.getUTCFullYear();

                        cell4.innerHTML =
                            "<div class='dependentRow'>" +
                                dt +
                            "</div>";

                        // Relationship Type
                        var cell2 = row.insertCell(0);
                        cell2.setAttribute("SCOPE", "row");

                        var rt = "";
                        if (relationshipType === 935950000) {
                            rt = "Dependent Child";

                            lastDependentWasChild = true;
                        } else {
                            rt = "Dependent Spouse";

                            lastDependentWasChild = false;
                        }

                        cell2.innerHTML =
                            "<div class='dependentRow'>" +
                            rt
                        "</div>";

                        // Dependent Full Name
                        var cell1 = row.insertCell(0);
                        cell1.setAttribute("SCOPE", "row");

                        cell1.innerHTML =
                            "<div class='dependentRow'>" +
                            data.value[i]["crme_firstname"] + " " + data.value[i]["crme_lastname"]; +
                            "</div>";
                        
                        if (i === 0 && relationshipType === 935950001) {
                            tableBody = createSpouseHeaderRow(tableBody);
                        }

                        if (i === 0 && relationshipType === 935950000) {
                            tableBody = createChildHeaderRow(tableBody);
                        }
                    }
                }
            }
        })
        .catch(function (error) {
            console.log("Exception occurred while retrieving listing of dependents: " + error);
        });
}

function createNoDependentsRow(tableBody) {
    var row = tableBody.insertRow(0);
    var cell1 = row.insertCell(0);
    cell1.setAttribute("SCOPE", "row");

    var announcementName = "No Dependents have been added at this time.";

    cell1.innerHTML =
        "<div class='headerRow'>" +
            announcementName +
        "' /></div>";
}

function createSpouseHeaderRow(tableBody) {
    var hdrRow = tableBody.insertRow(0);

    // Maintenance Type header
    var hdrCell6 = hdrRow.insertCell(0);
    hdrCell6.setAttribute("SCOPE", "row");
    hdrCell6.innerHTML =
        "<div class='headerRow'>" +
            "Maintenance Type" +
        "</div>";

    // SSN header
    var hdrCell5 = hdrRow.insertCell(0);
    hdrCell5.setAttribute("SCOPE", "row");
    hdrCell5.innerHTML =
        "<div class='headerRow'>" +
            "Last 4 of SSN" +
        "</div>";

    // DOB header
    var hdrCell4 = hdrRow.insertCell(0);
    hdrCell4.setAttribute("SCOPE", "row");
    hdrCell4.innerHTML =
        "<div class='headerRow'>" +
            "Date of Birth" +
        "</div>";

    // Relationship Type header
    var hdrCell2 = hdrRow.insertCell(0);
    hdrCell2.setAttribute("SCOPE", "row");
    hdrCell2.innerHTML =
        "<div class='headerRow'>" +
            "Relationship Type" +
        "</div>";

    // Full Name header
    var hdrCell1 = hdrRow.insertCell(0);
    hdrCell1.setAttribute("SCOPE", "row");

    hdrCell1.innerHTML =
        "<div class='headerRow'>" +
            "Spouse Full Name" +
        "</div>";

    return tableBody;
}

function createChildHeaderRow(tableBody) {
    var hdrRow = tableBody.insertRow(0);

    // In School header
    var hdrCell3 = hdrRow.insertCell(0);
    hdrCell3.setAttribute("SCOPE", "row");
    hdrCell3.innerHTML =
        "<div class='headerRow'>" +
            "(18-23) In School" +
        "</div>";

    // Maintenance Type header
    var hdrCell6 = hdrRow.insertCell(0);
    hdrCell6.setAttribute("SCOPE", "row");
    hdrCell6.innerHTML =
        "<div class='headerRow'>" +
            "Maintenance Type" + 
        "</div>";

    // SSN header
    var hdrCell5 = hdrRow.insertCell(0);
    hdrCell5.setAttribute("SCOPE", "row");
    hdrCell5.innerHTML =
        "<div class='headerRow'>" +
            "Last 4 of SSN" +
        "</div>";

    // DOB header
    var hdrCell4 = hdrRow.insertCell(0);
    hdrCell4.setAttribute("SCOPE", "row");
    hdrCell4.innerHTML =
        "<div class='headerRow'>" +
            "Date of Birth" +
        "</div>";

    // Relationship Type header
    var hdrCell2 = hdrRow.insertCell(0);
    hdrCell2.setAttribute("SCOPE", "row");
    hdrCell2.innerHTML =
        "<div class='headerRow'>" +
            "Relationship Type" +
        "</div>";

    // Full Name header
    var hdrCell1 = hdrRow.insertCell(0);
    hdrCell1.setAttribute("SCOPE", "row");
    hdrCell1.innerHTML =
        "<div class='headerRow'>" +
            "Child Full Name" +
        "</div>";  

    createSpacerRow(tableBody);
    
    return tableBody;
}

function createSpacerRow(tableBody) {
    var spacerRow = tableBody.insertRow(0);
    var spacerCell = spacerRow.insertCell(0);
    spacerCell.innerHTML =
        "<div class='headerRow'>" +
            " " +
        "</div>";
}
