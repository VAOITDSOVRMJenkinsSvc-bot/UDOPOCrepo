"use strict";

function RetrieveCustomer() {
    return new Promise(function (resolve, reject) {
        var msg = "Please confirm this selection.  Click OK if you were able to continue.  Or, click Cancel if you would like to release this selection back to the Queue.";
        var title = "Confirm Selection";
        var confirmOptions = { height: 200, width: 450 };
        var confirmStrings = { title: title, text: msg, confirmButtonLabel: "Yes", cancelButtonLabel: "No" };
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
            .then(
                function (response) {
                    if (window.IsUSD) {
                        window.open("http://event/?eventName=InitiateWalkInEvent")
                    }
                    return resolve(response.confirmed);
                },
                function (error) {
                    if (window.IsUSD) {
                        window.open("http://event/?eventName=ReleaseWalkInEvent")
                    }
                    return reject(error.message);
                });
    });
}

function walkinRetrieveCustomer() {
    return new Promise(function (resolve, reject) {
        var msg = "At this time you may go and retrieve the visitor from the waiting room.  Click OK if you were able to continue the interview.  Or, click Cancel if you were not able to find the customer (This will release the record back to the Queue).";
        var title = "Confirm Selection";
        var confirmOptions = { height: 200, width: 450 };
        var confirmStrings = { title: title, text: msg, confirmButtonLabel: "Yes", cancelButtonLabel: "No" };
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
            .then(
                function (response) {
                    if (window.IsUSD) {
                        window.open("http://event/?eventName=InitiateWalkInEvent")
                    }
                    return resolve(response.confirmed);
                },
                function (error) {
                    if (window.IsUSD) {
                        window.open("http://event/?eventName=ReleaseWalkInEvent")
                    }
                    return reject(error.message);
                });
    });
}