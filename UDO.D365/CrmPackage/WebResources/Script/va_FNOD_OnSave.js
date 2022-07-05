"use strict";

function OnSave(execContext) {
	try {
		formContext = execContext.getFormContext();
		if (ValidateZipcode(execContext) === false) {
			formContext.getEventArgs().preventDefault(); // RU12 Changed all event.returnValue
		}
	} catch (e) {
		console.log("An error occurred withing OnSave: " + e);
		throw (e);
	} finally {
		rebuildAllButtonsAsync(execContext);
	}
}

function ValidateZipcode(executionContext) {
	var formContext = executionContext.getFormContext();
	var va_spousezipcode = formContext.getAttribute('va_spousezipcode').getValue();
	if (va_spousezipcode !== null && va_spousezipcode.match(/[a-zA-Z]/)) {
		alert('Spouse/Last Known Address Zip Code field contains invalid alphabetical characters');
		return false;
	}

	return true;
}
