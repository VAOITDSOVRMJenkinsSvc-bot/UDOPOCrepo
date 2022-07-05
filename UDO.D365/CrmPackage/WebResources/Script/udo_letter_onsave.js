"use strict";

function Form_Onsave(executionObj) {
	if (executionObj.getEventArgs().getSaveMode()!=70) {
		// Not Auto-Save
		executionObj.getFormContext().getAttribute('udo_sendnotestomapd').setValue(true);
	}
    return true;
}
