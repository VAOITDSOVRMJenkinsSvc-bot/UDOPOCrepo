Mheo = window.Mheo || {};
Mheo.Configuration = {};
Mheo.UserDetails = {};

Mheo.CallVeteran = function (context, phoneAttribute) {
    Xrm.WebApi.retrieveRecord("systemuser", Xrm.Utility.getGlobalContext().userSettings.userId, "?$select=udo_finesseagentid,udo_finesseextension").then(function (value) {
        Mheo.UserDetails = value;
        if (!Mheo.UserDetails.udo_finesseagentid || Mheo.UserDetails.udo_finesseagentid === 0) {
            Xrm.Navigation.openErrorDialog({ message: 'Your account does not have a Finesse agent id configured. Please contact your D365 adminsitrator to update your account.' });
            return;
        }
        if (!Mheo.UserDetails.udo_finesseextension || Mheo.UserDetails.udo_finesseextension === 0) {
            Xrm.Navigation.openErrorDialog({ message: 'Your account does not have a Finesse extension number configured. Please contact your D365 adminsitrator to update your account.' });
            return;
        }
        Xrm.WebApi.retrieveMultipleRecords("mcs_setting", "?$select=udo_finesseendpoint").then(function (value) {
            Mheo.Configuration = value;
            if (!Mheo.Configuration.entities || Mheo.Configuration.entities.length === 0 || !Mheo.Configuration.entities[0].udo_finesseendpoint || Mheo.Configuration.entities[0].udo_finesseendpoint.length === 0) {
                Xrm.Navigation.openErrorDialog({ message: 'Failed to retrieve Finesse configuration. Please contact your D365 administrator.' });
                return;
            }
            if (!context || !phoneAttribute || phoneAttribute.length === 0) {
                Xrm.Navigation.openErrorDialog({ message: 'Unexpected Error!' });
                return;
            }
            var toPhone = context.getAttribute(phoneAttribute).getValue().replace(/\D/g, '');
            if (!toPhone || toPhone.length === 0) {
                Xrm.Navigation.openErrorDialog({ message: 'Unexpected error. No phone number available.' });
                return;
            }
            //At this stage all the configuration details are loaded.
            finesseUrl = Mheo.Configuration.entities[0].udo_finesseendpoint + "/finesse/api/User/" + Mheo.UserDetails.udo_finesseagentid + "/Dialogs";
            var credentials = btoa(Mheo.UserDetails.udo_finesseagentid + ":" + Mheo.UserDetails.udo_finesseagentid);
            data = "<Dialog><requestedAction>MAKE_CALL</requestedAction><fromAddress>" + Mheo.UserDetails.udo_finesseextension + "</fromAddress><toAddress>" + toPhone + "</toAddress></Dialog>";
            var request = new XMLHttpRequest();
            request.open("POST", encodeURI(finesseUrl), true);
            request.setRequestHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            request.setRequestHeader("Content-Type", "application/xml");
            request.setRequestHeader("Authorization", "Basic " + credentials);
            request.onreadystatechange = function () {
                if (this.readyState === 4) {
                    request.onreadystatechange = null;
                    switch (this.status) {
                        case 200: // Operation success with content returned in response body.
                        case 201: // Create success. 
                        case 202: //???
                        case 204: // Operation success with no content returned in response body.                            
                            var alertStrings = { confirmButtonLabel: "OK", text: "Dialing...", title: "Outbound Call Confirmation" };
                            var alertOptions = { height: 120, width: 260 };
                            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                                function success(result) {
                                    //console.log("Alert dialog closed");
                                },
                                function (error) {
                                    console.log(error.message);
                                }
                            );

                            break;
                        default: // All other statuses are unexpected so are treated like errors.
                            Xrm.Navigation.openErrorDialog({ message: 'Automatic dialing failed. Please contact your D365 administrator', details: request.responseText });
                            break;
                    }
                }
            };
            request.send(data);
        },
            function (reason) {
                Xrm.Navigation.openErrorDialog({
                    message: 'Failed to retrieve Finesse configuration. Contact your D365 adminsitrator if the issue persists.', details: reason
                });
            }).catch(function (error) {
                Xrm.Navigation.openErrorDialog({ message: 'Unexpected Error. Contact your D365 adminsitrator if the issue persists.', details: error.message });
            });
    },
        function (reason) {
            Xrm.Navigation.openErrorDialog({
                message: 'Failed to retrieve account details. Contact your D365 adminsitrator if the issue persists.', details: reason
            });
        }).catch(function (error) {
            Xrm.Navigation.openErrorDialog({ message: 'Unexpected Error. Contact your D365 adminsitrator if the issue persists.', details: error.message });
        });
}
