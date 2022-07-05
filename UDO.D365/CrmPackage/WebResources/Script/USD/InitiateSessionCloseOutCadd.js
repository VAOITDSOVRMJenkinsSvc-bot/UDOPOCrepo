//Not used
InitiateSessionCloseOut = function (interactionId, requestId, caddId) {
    if (caddId !== "") {
        var isDirty = Xrm.Page.data.entity.getIsDirty();

        if (isDirty) {
            window.open("http://uii/Global Manager/ShowTab?CADDProcessHost");
            Va.Udo.Crm.Scripts.Popup
                .MsgBox("There are unsaved changes on the CADD form.  Press Ok to continue with close and discard changes on CADD.   Press Cancel to go to CADD.",
                    Va.Udo.Crm.Scripts.Popup.PopupStyles.Question + Va.Udo.Crm.Scripts.Popup.PopupStyles.OKCancel,
                    "Discard CADD Changes",
                    { height: 200, width: 350 })
                .done(function() {
                    //Intitate session close out procedure
                    //Update Request end time
                    CloseSession(interactionId, requestId);
                    return;
                })
                .fail(function() {
                    window.open("http://uii/Global Manager/ShowTab?CADDProcessHost");
                    return;
                });
        } else {
            CloseSession(interactionId, requestId);
        }
    }
}

function CloseSession(interactionId, requestId) {
    var endTime = new Date();
    if (endTime) {
        if (requestId !== "") {

            CrmRestKit2011.Update("udo_request", requestId, { udo_EndTime: endTime }, false)
                .fail(function (xhr, statusCode, code) {
                    Va.Udo.Crm.Scripts.Popup
                        .MsgBox("There was an error while updating the Request before closing the Session.\n\nError: " + xhr.responseText,
                            Va.Udo.Crm.Scripts.Popup.PopupStyles.Critical,
                            "Request Update Error",
                            { height: 200, width: 350 });
                });

        }
        //Update Interaction end time
        if (interactionId !== "") {
            CrmRestKit2011.Update("udo_interaction",
                    interactionId,
                    { udo_EndTime: endTime, udo_Status: false },
                    false)
                .fail(function (xhr, statusCode, code) {
                    Va.Udo.Crm.Scripts.Popup
                        .MsgBox("There was an error while updating the Interaction before closing the Session.\n\nError: " + xhr.responseText,
                            Va.Udo.Crm.Scripts.Popup.PopupStyles.Critical,
                            "Interaction Update Error",
                            { height: 200, width: 350 });
                });
        }
    }
    window.open("http://uii/Session Tabs/CloseSession");
}

InitiateSessionCloseOut("[[Interaction.Id]+]", "[[Request.Id]+]", "[[CADD.Id]+]");