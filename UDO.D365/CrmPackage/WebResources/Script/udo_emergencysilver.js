//Not used.
var silverlightControl,
emergencyOptions = {},
emergencyType,
recipients = [],
message = '',

button;

function onPluginLoaded(sender, args) {
    button = document.getElementById("startEmergency");
    var emStatus = window.parent.Xrm.Page.data.entity.attributes.get("va_emergencystatus").getValue();
    if (emStatus == 953850001 || emStatus == 953850002) {
        button.style.visibility = 'hidden';
        return;
    }
    load();
    silverlightControl = sender.getHost();
    if (silverlightControl) button.disabled = false;
}

function load() {

    emergencyOptions.isCancel = false;
    emergencyOptions.recipients = [];
    emergencyOptions.isReady = false;
    getConfiguredRecipients();
    //TODO: convert to form context
    window.parent.Xrm.Page.getControl("va_emergencytype").removeOption(1);
    //TODO: convert to form context
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
            if (!confirm('Are you sure you want to cancel the Emergency!\r\nPress \'Ok\' to cancel the emergency.')) {
                return;
            }
            else {
                //Process Cancel here
                setMessage(getCancelEmergencyMesssage());
                //TODO: convert to form context
                window.parent.Xrm.Page.data.entity.attributes.get("va_emergencystatus").setValue(953850002);
                //TODO: convert to form context
                window.parent.Xrm.Page.data.entity.save();
                //setNewRecipients();
                if (button != null) {
                    button.style.visibility = false;
                }
                disableAll();
            }
        }
        else {
            //if no recipients show message
            if (recipients.length === 0) {
                alert("No recipients to deliver the emergency notification!")
                return;
            }
            //TODO: convert to form context
            window.parent.Xrm.Page.data.entity.attributes.get("va_emergencystatus").setValue(953850001);
            //TODO convert to form context
            window.parent.Xrm.Page.data.entity.save();
        }
        // else execute

        if (silverlightControl)
            silverlightControl.Content.lyncSdk.Execute();
    });
}
function getConfiguredRecipients() {
    //TODO: convert to WebAPI
    //TODO: consider promise chain to replace nested retrieves
    WebApi.prototype.RetrieveRecord(window.parent.Xrm.Page.context.getUserId(), 'SystemUser', ['va_WSLoginName', 'FullName', 'SiteId'])
    //CrmRestKit2011.Retrieve('SystemUser', window.parent.Xrm.Page.context.getUserId(), ['va_WSLoginName', 'FullName', 'SiteId'])
			.done(function (data) {
			    var filter = 'va_Site/Id eq guid\'' + data.d.SiteId.Id + '\' and va_name eq \'NCC Emergency List\'';
			    //TODO: convert to WebAPI
			    WebApi.prototype.RetrieveMultiple('va_sitesteams', ['va_Team'], filter)
			    //CrmRestKit2011.ByQuery('va_sitesteams', ['va_Team'], filter)
				.done(function (data1) {
				    var noRecipientMessage = 'No recipients to deliver the emergency notification were found for site "' + data.d.SiteId.Name + '"';
				    var columns1 = ['teammembership_association/InternalEMailAddress'];
				    var expand1 = 'teammembership_association';
				    try {
				        var filter1 = 'TeamId eq guid\'' + data1.d.results[0].va_Team.Id + '\'';
				    } catch (err) {
				        //TODO: check for WebAPI error handler
				        restKitError(err, 'The PCR Site (NCC) does not have a Team assigned!\r\n' +
							'A Site (NCC) must have an emergency Team assigned in order to get the list of recipients to deliver the emergency notification!');
				        emergencyOptions.isReady = true;
				        return;
				    }
				    //TODO: convert to WebAPI
				    CrmRestKit2011.ByExpandQuery('Team', columns1, expand1, filter1)
					.done(function (data2) {
					    var recipients = data2.d.results[0].teammembership_association.results;
					    for (var i = 0; i < recipients.length; i++) {
					        if (recipients[i] && recipients[i].InternalEMailAddress)
					            emergencyOptions.recipients.push(recipients[i].InternalEMailAddress);
					    }
					    emergencyOptions.isReady = true;
					    if (emergencyOptions.recipients.length === 0) {
					        alert(noRecipientMessage);
					        disableEmergencyButton();
					        return;
					    }
					    initiatlizeEmergencyButton();

					}
					).fail(function (error) {
					    emergencyOptions.isReady = true;
					    disableEmergencyButton();
					    //TODO: check for WebAPI error handler
					    restKitError(error, noRecipientMessage);
					    return;
					});
				}
				).fail(function (err) {
				    emergencyOptions.isReady = true;
				    disableEmergencyButton();
				    //TODO: check for WebAPI error handler
				    restKitError(err, 'The PCR Site (NCC) does not have a Team assigned!\r\n' +
							'A Site (NCC) must have an emergency Team assigned in order to get the list of recipients to deliver the emergency notification!');
				});
			}).fail(function (error) {
			    emergencyOptions.isReady = true;
			    disableEmergencyButton();
			    //TODO: check for WebAPI error handler
			    restKitError(error, 'The PCR does not have a site assigned!\r\n' +
					'A Site must be assigned in order to get the list of recipients to deliver the emergency notification!');
			});

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
    //TODO: convert to Xrm.Navigation
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
    //TODO: convert to Xrm.Navigation
    var i, j, email,
        newRecipients = window.parent.Xrm.Page.data.entity.attributes.get("va_additionalemails").getValue();
    var emails = [];
    if (newRecipients != null) {
        emails = newRecipients.split(/\r\n|\n/g);
    }
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

    //TODO: convert to form context
    window.parent.Xrm.Page.getControl("va_emergencytype").setDisabled(true);
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