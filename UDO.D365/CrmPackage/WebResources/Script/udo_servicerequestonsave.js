"use strict";
function Form_Onsave(executionObj) {
    //  Log using Va.Udo.AppInsights
    var appInsightsProps = {method: "Form_Onsave", description: "Called on save of Service Request form."};
    startTrackEvent("UDO Service Request Form_Onsave", appInsightsProps);
    try {
        var formContext = executionObj.getFormContext();
        if (executionObj.getEventArgs().getSaveMode() !== 70) {
            // Not Auto-Save
            formContext.getAttribute('udo_sendnotestomapd').setValue(true);
            var nc = formContext.getAttribute('udo_notecreated');
            if (nc) {
                nc.setSubmitMode('always');
            }
            var action = formContext.getAttribute('udo_action').getSelectedOption().text;
            var ga = formContext.getAttribute;
            var gc = formContext.getControl;
            var vai0820 = (action === '0820' || action === '0820a' || action === '0820f' || action === '0820d');
            //debugger;
            
            if ((!ga('udo_readscript').getValue() || ga('udo_readscript').getValue() === false) && vai0820) {
                gc('udo_readscript').setDisabled(false);
                gc('udo_readscript').setFocus();
                ga('udo_readscript').setValue(true);
            }
            
            //return true;
        }
        else if (executionObj.getEventArgs().getSaveMode() === 70 || executionObj.getEventArgs().getSaveMode() === 2) {
            executionObj.getEventArgs().preventDefault();
        }

        //return true;
    }
    catch(e) {
        trackException(e);
    }
    stopTrackEvent("UDO Service Request Form_Onsave", appInsightsProps);
}