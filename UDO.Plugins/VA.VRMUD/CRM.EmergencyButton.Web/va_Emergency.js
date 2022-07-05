var silverlightControl,
    emergencyOptions = window.opener._emergencyOptions,
    emergencyType,
    recipients = [],
    message = '',
    button;

function onPluginLoaded(sender, args) {
    load();
    
    silverlightControl = sender.getHost();
    if (silverlightControl) button.disabled = false;
}

function load() {
    button = document.getElementById("startEmergency");

    addEvent('click', button, function () {
        if (emergencyOptions.isCancel) {
            if (!confirm('Are you sure you want to cancel the Emergency!\r\nPress \'Ok\' to cancel the emergency.'))
                return;
        }

        getRecipients();

        if (recipients.length === 0) {
            alert("No recipients to deliver the emergency notification!")
            return;
        }

        if (silverlightControl)
            silverlightControl.Content.lyncSdk.Execute();
    });

    addEvent('click', document.getElementById("radSuicide"), function () {
        setSelection(arguments[0].srcElement);
    });

    addEvent('click', document.getElementById("radThreats"), function () {
        setSelection(arguments[0].srcElement);
    });

    addEvent('click', document.getElementById("radBomb"), function () {
        setSelection(arguments[0].srcElement);
    });

    addEvent('click', document.getElementById("radEmergency"), function () {
        setSelection(arguments[0].srcElement);
    });

    addEvent('click', document.getElementById("close"), function () {
        window.close();
    });

    setEmergencyType();

    if (emergencyOptions.isCancel) {
        button.value = 'Cancel Emergency';
        setMessage(getCancelEmergencyMesssage());
        setNewRecipients();
        disableAll();
    } else {
        // get list
        button.value = 'Emergency';
        setMessage(getMessage());
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
        newRecipients = document.getElementById("addRecipients").value,
        emails = newRecipients.split('\r\n');

    emergencyOptions.newRecipients = newRecipients;

    for (i = 0; i < emergencyOptions.recipients.length; i++) {
        recipients.push(emergencyOptions.recipients[i]);
    }

    for (j = 0; j < emails.length; j++) {
        email = emails[j].replace(/^\s+|\s+$/g, '');
        if (email)
            recipients.push(email);
    }
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
        case 2:
            return 'I have a caller threatening an employee and need assistance immediately.';
        case 3:
            return 'I have a Bomb threat call and need assistance immediately.';
        default:
            return 'I am having an emergency, Please help.';
    }
}

function setMessage(value) {
    message = value;
    emergencyOptions.message = value;
}

function disableAll() {
    // disabled all controls
    document.getElementById("addRecipients").disabled = 'disable';
    document.getElementById("radSuicide").disabled = 'disable';
    document.getElementById("radBomb").disabled = 'disable';
    document.getElementById("radThreats").disabled = 'disable';
    document.getElementById("radEmergency").disabled = 'disable';
}

function onSilverlightError(sender, args) {
    var appSource = "";
    if (sender != null && sender != 0) {
        appSource = sender.getHost().Source;
    }

    var errorType = args.ErrorType;
    var iErrorCode = args.ErrorCode;

    if (errorType == "ImageError" || errorType == "MediaError") {
        return;
    }

    var errMsg = "Unhandled Error in Emergency Button Application (Silverlight) " + appSource + "\n";

    errMsg += "Code: " + iErrorCode + "    \n";
    errMsg += "Category: " + errorType + "       \n";
    errMsg += "Message: " + args.ErrorMessage + "     \n";

    if (errorType == "ParserError") {
        errMsg += "File: " + args.xamlFile + "     \n";
        errMsg += "Line: " + args.lineNumber + "     \n";
        errMsg += "Position: " + args.charPosition + "     \n";
    }
    else if (errorType == "RuntimeError") {
        if (args.lineNumber != 0) {
            errMsg += "Line: " + args.lineNumber + "     \n";
            errMsg += "Position: " + args.charPosition + "     \n";
        }
        errMsg += "MethodName: " + args.methodName + "     \n";
    }

    throw new Error(errMsg);
}

function onLyncErrors(message) {
    var message = 'There was an error while trying to IM the recipients:\r\n"' + message + '" \r\n\r\n' +
                    'Please, try the following:\r\n' +
                    ' - Start or restart the communicator (Lync), if it\'s not running.\r\n' +
                    ' - Sign in to the communicator (Lync), if it\'s running.\r\n' +
                    ' - Contact your supervisor and HELP DESK, if all the above is working';

    alert(message);
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



