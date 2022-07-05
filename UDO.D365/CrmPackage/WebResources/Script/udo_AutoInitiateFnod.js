function sendRequest(requestType, queryString, requestBody, successCallback) {
    // Set shared variables
    var globalContext = GetGlobalContext();
    var shortVersion = globalContext.getVersion().split(".").slice(0, 2).join(".");
    var apiString = globalContext.getClientUrl() + "/api/data/v" + shortVersion + "/";

    // Build XMLHttpRequest object
    var req = new XMLHttpRequest();
    req.open(requestType, apiString + queryString);
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");

    // Set event handlers
    req.onreadystatechange = function () {
        if (req.readyState === 4) {
            successCallback({ StatusCode: req.status, JSON: JSON.parse(req.responseText) });
        }
    }

    // Send request
    if (requestBody !== null && requestBody !== undefined) {
        req.send(JSON.stringify(requestBody));
    } else {
        req.send();
    }
}

function showErrorAlert() {
    var alertStrings = { confirmButtonLabel: "OK", text: "An error occurred. Please open the FNOD People List to try again.", title: "An Error Occurred" };
    Xrm.Navigation.openAlertDialog(alertStrings)
        .then(function (result) {
            window.open("http://uii/FNODAutoOpenHostedControl/Close");
        });
}

function AutoInitiateFnod(idProofId) {
    var queryString = "udo_persons?$select=udo_personid&$filter=_udo_idproofid_value eq " + idProofId + " and udo_type eq 752280000";

    try {
        sendRequest("GET", queryString, null,
            function (response) {
                if (response.StatusCode === 200) {
                    if (response.JSON.value.length === 1) {
                        var requestBody = {
                            ParentEntityReference: {
                                "@odata.type": "Microsoft.Dynamics.CRM.udo_person",
                                "udo_personid": response.JSON.value[0].udo_personid
                            }
                        }

                        sendRequest("POST", "udo_InitiateFNOD", requestBody,
                            function (response) {
								if (response.StatusCode === 200) {
									if (response.JSON.Exception) {
										showErrorAlert();
									}

                                    //var url = GetGlobalContext().getClientUrl() + "/main.aspx?etn=va_fnod&pagetype=entityrecord&id=" + response.JSON.result.va_fnodid;
                                    //var url = "http://uii/FNOD Form/Navigate?url=" + GetGlobalContext().getClientUrl() + "/main.aspx?appid=" + appId + "&cmdbar=false&navbar=off&newWindow=true&pagetype=entityrecord&etn=va_fnod&id=" + response.JSON.result.va_fnodid;
                                    var url = "http://uii/FNOD Form/Navigate?url=" + GetGlobalContext().getClientUrl() + "/main.aspx?cmdbar=false&navbar=off&newWindow=true&pagetype=entityrecord&etn=va_fnod&id=" + response.JSON.result.va_fnodid;
                                    window.open(url);
                                    window.open("http://uii/FNOD People List/Close");
                                    window.open("http://uii/FNODAutoOpenHostedControl/Close");
                                } else {
                                    showErrorAlert();
                                }
                            });
                    } else {
                        // Handle when the number of Veterans returned is not 1
                        window.open("http://event/?eventname=FNODAutoOpenMultiplePersonsFound");
                    }
                } else {
                    showErrorAlert();
                }
            });
    } catch (e) {
        // Handle exception here
        console.log("An error occurred: " + e.message);
        showErrorAlert();
    }
}
