// Marital Information OnLoad

// Only One function to close the form and notify users that the form is not editable.  Will prompt for closure.
var exCon = null;
var formContext = null;
function OnLoad(execCon) {

    exCon = execCon;
    formContext = exCon.getFormContext();
    var crme_marriagestartdate = Xrm.Page.getAttribute('crme_marriagestartdate');

    //If marriagestartdate is null this was created by the Web Service.  Wait for sugrid to load and close the form.
    if (crme_marriagestartdate.getValue() == null) {
        waitForSubGrid("SpouseHistory");
    }

    //Fire the marriageEndCountryChange to set the proper required fields.
    marriageEndDateChange()
}

function waitForSubGrid(subgridName) {
    var subgrid = Xrm.Page.getControl(subgridName); //.getElementById(subgridName);

    if (subgrid == null) {
        setTimeout(function () { waitForSubGrid(); }, 1000);
        return;
    }
    var crme_marriagestartdate = Xrm.Page.getAttribute('crme_marriagestartdate');

    if (crme_marriagestartdate.getValue() == null) {
        alert('No edits are allowed to this record because it exists in the CORP database');
        Xrm.Page.ui.close();


    }


}