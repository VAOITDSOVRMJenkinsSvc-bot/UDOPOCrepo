//crme_dependent OnLoad Code
var exCon = null;
var formContext = null;
function OnLoad(execCon) {
    exCon = execCon;
    formContext = exCon.getFormContext();
    //Make sure to show correct tab when being executed by plugin from Dependent Maintenance
    showDependentTabs();

    //if this record was retrieved from a webservice, you can't do anything
    if (Xrm.Page.getAttribute("crme_legacyrecord").getValue() === true) {
        alert('No edits are allowed to this record because it exists in the CORP database');
        Xrm.Page.ui.close();

        ////var attributes = Xrm.Page.data.entity.attributes.get();
        //for (var i in attributes) {
        //    var control = attributes[i].controls.get(0);
        //    control.setDisabled(true);
        //}
    }

    // set the form correctly if is a School Age Child in School
    schoolAgedChildInSchoolChange();
    syncState_MarriageFields();
}

function showDependentTabs() {
    var depType = Xrm.Page.getAttribute('crme_dependentrelationship').getValue();

    //Child: 935,950,000; Spouse: 935,950,001
    switch (depType) {
        case 935950000:
            Xrm.Page.ui.tabs.get('dependentchild_tab').setVisible(true);
            Xrm.Page.ui.tabs.get('dependentspouse_tab').setVisible(false);
            dependentRelationshipOnChange();
            break;
        case 935950001:
            Xrm.Page.ui.tabs.get('dependentspouse_tab').setVisible(true);
            Xrm.Page.ui.tabs.get('dependentchild_tab').setVisible(false);
            dependentRelationshipOnChange();
            break;
        default:
            Xrm.Page.ui.tabs.get('dependentchild_tab').setVisible(false);
            Xrm.Page.ui.tabs.get('dependentspouse_tab').setVisible(false);
            dependentRelationshipOnChange();
            break;
    }

    // run the change script for specific fields to set the screen up for this record.

}

// Added 6-28-2018 AB for Add School Age Child Dependent
// Monitor for a change in Age and Attendance of Dependent. Impacts VA FORM 21-674b, APR 2015
function schoolAgedChildInSchoolChange() {
    var isSchoolAgeChildInSchool = Xrm.Page.getAttribute("crme_childage1823inschool").getValue();
    if (isSchoolAgeChildInSchool === true) {
        Xrm.Page.ui.tabs.get('SchoolAged_Child_Claimant_tab').setVisible(true);
        Xrm.Page.ui.tabs.get('SchoolAgedChild_Student_School').setVisible(true);
        Xrm.Page.ui.tabs.get('SchoolAgedChild_Student_tab').setVisible(true);
    }
    else {
        Xrm.Page.ui.tabs.get('SchoolAged_Child_Claimant_tab').setVisible(false);
        Xrm.Page.ui.tabs.get('SchoolAgedChild_Student_School').setVisible(false);
        Xrm.Page.ui.tabs.get('SchoolAgedChild_Student_tab').setVisible(false);
    }
}


// Added 6-28-2018 AB for Add School Age Child Dependent
// Called by Marital Status change of student
function syncState_MarriageFields() {
    var isMarried = Xrm.Page.getAttribute("udo_child_married").getValue();
    var wasMarried = Xrm.Page.getAttribute("crme_childpreviouslymarried").getValue();

    //alert(isMarried);
    //alert(wasMarried);
    if (isMarried === true || wasMarried === true) {
        Xrm.Page.getAttribute('udo_dateofmarriage').setRequiredLevel('required');
    }
    else {
        Xrm.Page.getAttribute('udo_dateofmarriage').setRequiredLevel('none');
    }

    // Used Yes No Option Set due to issue with Boolean in DocGen
    if (Xrm.Page.getAttribute("crme_childpreviouslymarried").getValue() != null) {
        var wasChildEverMarried = Xrm.Page.getAttribute("crme_childpreviouslymarried").getValue();

        if (wasChildEverMarried) {
            Xrm.Page.getAttribute("udo_cbischildmarriedreportyesno").setValue(752280000); // Yes
        }
        else {
            Xrm.Page.getAttribute("udo_cbischildmarriedreportyesno").setValue(752280001); // No
        }
    }
}
