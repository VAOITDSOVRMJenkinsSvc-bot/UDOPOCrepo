InitiateSessionCloseOut = function (interactionId, requestId) {
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
            CrmRestKit2011.Update("udo_interaction", interactionId, { udo_EndTime: endTime, udo_Status: false }, false)
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

InitiateSessionCloseOut("[[Interaction.Id]+]", "[[Request.Id]+]");