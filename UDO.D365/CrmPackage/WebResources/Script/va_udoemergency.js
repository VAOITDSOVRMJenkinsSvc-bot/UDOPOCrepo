var silverlightControl,
    emergencyOptions = {},
    emergencyType,
    recipients = [],
    message = '',
    recursion = 0,
    button;

function emergOnLoad() {
    button = document.getElementById("startEmergency");    
    var emStatus = window.parent.Xrm.Page.data.entity.attributes.get("va_emergencystatus").getValue();
    if (emStatus === 953850001 || emStatus === 953850002) {
        button.style.visibility = 'hidden';
        return;
    }
    load();
    button.disabled = false;
}

function load() {
    // Hide and show tab workaround in order to get UCI to display page from top instead of first field 
    var tab = window.parent.Xrm.Page.ui.tabs.get("GeneralTab");
    tab.setVisible(false);
    tab.setVisible(true);

    emergencyOptions.isCancel = false;
    emergencyOptions.recipients = [];
    emergencyOptions.isReady = false;
    getConfiguredRecipients();    
    window.parent.Xrm.Page.getControl("va_emergencytype").removeOption(1);    
    window.parent.Xrm.Page.data.entity.attributes.get("va_emergencystatus").setSubmitMode("always");
    button = document.getElementById("startEmergency");
    button.disable = 'disabled';

    // Add Event handler for button click 
    addEvent('click', button, function () {
        //Remove Cancel from CrmRestKit2011
        disableAll();        
        // Gets the list of recipients
        getRecipients();
        //Set emergency type
        getSelectedOption();
        if (emergencyOptions.isCancel) {
            var confirmStrings = { text: "Are you sure you want to cancel the Emergency!\r\nPress \'OK\' to cancel the emergency." };
            var confirmOptions = { height: 160, width: 450 };
             window.parent.Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
                function (success) {
                    if (success.confirmed) {
                        //Process Cancel here
                        setMessage(getCancelEmergencyMesssage());
                        initiateTeamsGroupChat();                        
                        window.parent.Xrm.Page.data.entity.attributes.get("va_emergencystatus").setValue(953850002);
                        window.parent.Xrm.Page.data.entity.save();
                        //setNewRecipients();
                        if (button !== null) {
                            button.style.visibility = false;
                        }
                        disableAll();
                    }
                });
        }
        else {
            //if no recipients show message
            console.log(recipients);
            if (recipients.length === 0) {
                alert("No recipients to deliver the emergency notification!");
                return;
            }

            initiateTeamsGroupChat();
            window.parent.Xrm.Page.data.entity.attributes.get("va_emergencystatus").setValue(953850001);           
            if (window.parent.Xrm.Page.ui.getFormType() == 1) {
                setTimeout(function () {
                    window.parent.Xrm.Page.data.entity.save(); 
                }, 2000);
            }
        }
    });
}

function initiateTeamsGroupChat() {
    // else execute
    var recipientString = "";
    for (var i = 0; i < recipients.length; i++) {
        if (recipients[i]) {
            var pos = recipientString.indexOf(recipients[i]);
            //Exclude duplicate entries
            if (pos < 0)
                recipientString += recipients[i] + ",";
        }
    }
    //Remove the last comma and any whitespaces after it
    if (recipientString)
        recipientString = recipientString.replace(/,$/, "");
    var uri;
    if (checkIsUSD())
        uri = encodeURI("http://event/?eventName=EmergencyButton&recipients=" + recipientString + "&message=" + emergencyOptions.message);
    else
        uri = encodeURI("https://teams.microsoft.com/l/chat/0/0?users=" + recipientString + "&message=" + emergencyOptions.message + "&topicName=Emergency");
    onLyncExecuteReady();    
    var encodedURI = encodeURI(uri);
    window.open(encodedURI);
}

function checkIsUSD() {
    if ((window.IsUSD !== undefined && window.IsUSD !== null && window.IsUSD === true) || (parent.window.IsUSD !== undefined && parent.window.IsUSD !== null && parent.window.IsUSD === true)) {
        return true;
    } else {
        return false;
    }
}

function getConfiguredRecipients() {
    Xrm.WebApi.retrieveRecord("systemuser", window.parent.Xrm.Page.context.getUserId().replace("{","").replace("}",""), "?$select=va_wsloginname,fullname,_siteid_value").then(
        function (data) {
            console.log(data);
            var filter = "?$select=_va_team_value&$filter=_va_site_value eq " + data["_siteid_value"] + " and va_name eq 'NCC Emergency List'";
            //
            Xrm.WebApi.retrieveMultipleRecords("va_sitesteams", filter).then(function reply(results) {
                console.log(results);

                var noRecipientMessage = 'No recipients to deliver the emergency notification were found for site "' + data["_siteid_value@OData.Community.Display.V1.FormattedValue"] + '"';
                var teamFilter = "?$expand=teammembership_association($select=internalemailaddress)&$filter=teamid eq " + results.value[0]["_va_team_value"];

                Xrm.WebApi.retrieveMultipleRecords("team", teamFilter).then(function teamReply(teamResults) {
                    console.log(teamResults);

                    var recipients = teamResults.value[0]["teammembership_association"];
                    for (var i = 0; i < recipients.length; i++) {
                        if (recipients[i] && recipients[i]["internalemailaddress"])
                            emergencyOptions.recipients.push(recipients[i]["internalemailaddress"]);
                    }
                    emergencyOptions.isReady = true;
                    if (emergencyOptions.recipients.length === 0) {
                        alert(noRecipientMessage);
                        disableEmergencyButton();
                        return;
                    }
                    initiatlizeEmergencyButton();
                });
            });
        }
    );
}

function initiatlizeEmergencyButton() {
    var embutton = document.getElementById("startEmergency");
    embutton.disabled = false;
    embutton.value = "Emergency";
}

function disableEmergencyButton() {
    var embutton = document.getElementById("startEmergency");
    embutton.disabled = true;
    embutton.value = "Emergency";
}

function restKitError(err, message) {
    if (err.responseText) {
        alert(message + '\r\n' + $.parseJSON(err.responseText).error.message.value);
    } else {
        alert(message);
    }
}

function getSelectedOption() {
    var emergencyOptionSetType = window.parent.Xrm.Page.data.entity.attributes.get("va_emergencytype").getValue();
    var emValue = getEmergencyValue(emergencyOptionSetType);
    var emId = getEmergencyId(emValue);
    var emTitle = getEmergencyTitle(emValue);
    emergencyType = { value: emValue, id: emId, name: emTitle };
    emergencyOptions.type = emId;
    setMessage(getMessage(emergencyType.value));
}

function getEmergencyId(emValue) {
    switch (emValue) {
        case 1: return "radSuicide";
            break;
        case 2: return "radThreats";
            break;
        case 3: return "radBomb"
            break
        case 4: return "radEmergency"
            break;
    }
}

function getEmergencyTitle(emValue) {
    switch (emValue) {
        case 1: return "Suicide";
            break;
        case 2: return "Threat";
            break;
        case 3: return "Bomb"
            break
        case 4: return "Emergency"
    }
}

function getEmergencyValue(optionSet) {
    switch (optionSet) {
        case 953850000: return 1;
            break;
        case 953850001: return 2;
            break;
        case 953850002: return 3;
            break;
        case 953850003: return 4;
            break;
        case 1: return 5;
            break;
    }
}

function addEvent(evnt, elem, func) {
    if (elem.addEventListener)
        elem.addEventListener(evnt, func, false);
    else if (elem.attachEvent) {
        elem.attachEvent("on" + evnt, func);
    }
    else {
        elem[evnt] = func;
    }
}

function getRecipients() {    
    var i, j, email,
        newRecipients = window.parent.Xrm.Page.data.entity.attributes.get("va_additionalemails").getValue();
    var emails = [];
    if (newRecipients !== null) {
        emails = newRecipients.split(/(?:\r\n|\r|\n|;|,)/g); //Split when the additional email has \r\n or \r or \n or ; or , seperators
    }
    emergencyOptions.newRecipients = newRecipients;

    for (i = 0; i < emergencyOptions.recipients.length; i++) {
        recipients.push(emergencyOptions.recipients[i]);
    }

    for (j = 0; j < emails.length; j++) {
        email = emails[j].replace(/^\s+|\s+$/g, '');
        if (email && validateEmail(email)) //Verify whether the email is valid before adding sending it to teams
            recipients.push(email);
    }
}

function validateEmail(email) {
    const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}

function setSelection(element) {
    emergencyType = { value: parseInt(element.value), id: element.id, name: element.title };
    emergencyOptions.type = element.id;
    setMessage(getMessage(emergencyType.value));
}

function setEmergencyType() {
    var radio = document.getElementById(emergencyOptions.type);
    radio.checked = 'checked';
    setSelection(radio);
}

function getCancelEmergencyMesssage() {
    return 'The ' + (emergencyType.value === 4 ? '' : emergencyType.name.toLowerCase()) + ' emergency was canceled!'
}

function setNewRecipients() {
    document.getElementById("addRecipients").value = emergencyOptions.newRecipients;
}

function getMessage(value) {
    switch (value) {
        case 1:
            return 'I have a suicide call and need assistance immediately.';
            break;
        case 2:
            return 'I have a caller threatening an employee and need assistance immediately.';
            break;
        case 3:
            return 'I have a Bomb threat call and need assistance immediately.';
            break;
        case 4:
            return 'I am having an emergency, Please help.';
            break;
        case 5: return 'The ' + (emergencyType.value === 4 ? '' : emergencyType.name.toLowerCase()) + ' emergency was canceled!';

    }
}

function setMessage(value) {
    message = value;
    emergencyOptions.message = value;
}

function disableAll() {    
    window.parent.Xrm.Page.getControl("va_emergencytype").setDisabled(true);
}

function onLyncExecuteReady() {
    if (!emergencyOptions.isCancel) {
        emergencyOptions.isCancel = true;
        button.value = "Cancel Emergency";
        setMessage(getCancelEmergencyMesssage());
    } else {
        button.disabled = 'disabled'
        emergencyOptions.isCancel = false;
    }

    disableAll();
}
