"use strict";

function onSave(exCon) {
   
    var formContext = exCon.getFormContext();
    console.log("Here");
    if (exCon.getEventArgs().getSaveMode() === 70) {
        exCon.getEventArgs().preventDefault(); // Disabling auto save
        return false;
    }

    if (exCon.getEventArgs().getSaveMode() != 70) {
        var formType = formContext.ui.getFormType();
        if (formType === 1) //create form
        {
//save and call create note
        formContext.data.entity.save();
        }
        else
        {
            //just save the form
            formContext.data.entity.save();
        }
        // Not Auto-Save
       // exCon.getFormContext().getAttribute('udo_sendnotestomapd').setValue(true);
    }

}