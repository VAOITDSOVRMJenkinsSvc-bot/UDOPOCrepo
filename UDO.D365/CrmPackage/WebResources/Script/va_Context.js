"use strict";

var vrmContext = function (execContext) {
    var formContext = execContext.getFormContext();    
    this.parameters = new Array();
    this.ignoreBirlsError = false;

    formContext.ui.clearFormNotification("PARTICIPANTIDBLANK");
    formContext.ui.clearFormNotification("AUTHSTORENOTGRANTED");

    this.SetSearchParameters = function () {
        this.environment = _GetEnvironment();

        this.participantId = null;
        this.fileNumber = null;
        this.searchCORPBy = null;
        this.ssn = null;  // search by fileno, get ssn from search results
        this.edpi = null;   // not used yet

        var fileNumber = formContext.getAttribute('va_ssn').getValue();
        var participantId = formContext.getAttribute('va_participantid').getValue(); 

        var searchOption = 1;

        if (formContext.data.entity.getEntityName() === 'contact') {   // contact screen
            if (participantId) {
                searchOption = 2; // search by pid
            } else {
                searchOption = 1; // search by filenumber
            }

        }
        else {
            searchOption = formContext.getAttribute('va_searchtype').getValue();
        }

        switch (parseInt(searchOption)) {
            case 2:
                // SEARCH BY: PID
                if (participantId) {
                    if (formContext.data.entity.getEntityName() !== 'contact') {
                        formContext.getAttribute('va_ssn').setValue(null);
                        formContext.getAttribute('va_firstname').setValue(null);
                        formContext.getAttribute('va_lastname').setValue(null);
                    }
                    this.participantId = participantId;
                }
                else {
                    formContext.ui.setFormNotification("Participant ID field is blank.", "WARNING", "PARTICIPANTIDBLANK");
                    return false;
                }
                this.searchCORPBy = 'PARTICIPANTID'; //SSN or PARTICIPANTID
                break;

            case 3:
                // SEARCH BY: EDIPI
                formContext.ui.setFormNotification("Connection to the authorization store has not yet been granted. Please come back later.", "WARNING", "AUTHSTORENOTGRANTED");
                return false;
            case 1:
            default:
                // SEARCH BY: SSN + other params
                
                if (formContext.data.entity.getEntityName() === 'contact') { 
                    formContext.getAttribute('va_participantid').setValue(null);
                    formContext.getAttribute('va_edipi').setValue(null);
                }
                if (fileNumber) {
                    this.fileNumber = fileNumber;
                }
                break;
        }
        // ES: Per bug #98779 
        var c = formContext.getAttribute('va_firstname'); if (!c) { c = formContext.getAttribute('firstname'); } this.firstName = c.getValue();
        c = formContext.getAttribute('va_lastname'); if (!c) { c = formContext.getAttribute('lastname'); } this.lastName = c.getValue();
        c = formContext.getAttribute('va_middleinitial'); if (!c) { c = formContext.getAttribute('middlename'); } this.middleName = c.getValue();

        //RTC 108417: Use DOB Text field and convert to date/time.
        var dobDate = (formContext.getAttribute('va_dobtext') !== null && formContext.getAttribute('va_dobtext').getValue() !== null) ? new Date(formContext.getAttribute('va_dobtext').getValue()) : null;
        this.dob = dobDate ? dobDate.format("MMddyyyy") : null;
        this.city = formContext.getAttribute('va_citysearch').getValue();
        this.state = formContext.getAttribute('va_statesearch').getValue();
        this.zipCode = formContext.getAttribute('va_zipcodesearch').getValue();

        // Birls
        this.branchOfService = formContext.getAttribute('va_branchofservice').getValue();
        this.serviceNumber = formContext.getAttribute('va_servicenumber').getValue();
        this.insuranceNumber = formContext.getAttribute('va_insurancenumber').getValue();
        this.dod = (formContext.getAttribute('va_dod') !== null && formContext.getAttribute('va_dod').getValue() !== null) ? formContext.getAttribute('va_dod').getValue().format("MMddyyyy") : null;
        this.eod = (formContext.getAttribute('va_enteredondutydate') !== null && formContext.getAttribute('va_enteredondutydate').getValue() !== null) ? formContext.getAttribute('va_enteredondutydate').getValue().format("MMddyyyy") : null;
        this.rad = (formContext.getAttribute('va_releasedactivedutydate') !== null && formContext.getAttribute('va_releasedactivedutydate').getValue() !== null) ? formContext.getAttribute('va_releasedactivedutydate').getValue().format("MMddyyyy") : null;
        this.suffix = formContext.getAttribute('va_suffix').getValue();
        this.folderLocation = formContext.getAttribute('va_folderlocation').getValue();
        this.payeeNumber = formContext.getAttribute('va_participantid').getValue();

        // Appeals        
        var userSelection = formContext.getAttribute('va_findappealsby').getValue();
        if (userSelection === 953850000) { userSelection = 'fileNumber'; }
        else if (userSelection === 953850001) { userSelection = 'ssn'; }
        else if (userSelection === 953850002) { userSelection = 'values'; }
        else { userSelection = 'fileNumber'; }

        this.appealsSearchValue = userSelection;
        if (userSelection === 'values') {
            this.appealsSsn = formContext.getAttribute('va_appealsssn').getValue();
            this.appealsLastName = formContext.getAttribute('va_appealslastname').getValue();
            this.appealsFirstName = formContext.getAttribute('va_appealsfirstname').getValue();
            this.appealsDateOfBirth = formContext.getAttribute('va_appealsdateofbirth').getValue();
            this.appealsCity = formContext.getAttribute('va_appealscity').getValue();
            this.appealsState = formContext.getAttribute('va_appealsstate').getValue();
        }

        // MVI/Pathways
        this.gender = (formContext.getAttribute('va_genderset') !== null) ? formContext.getAttribute('va_genderset').getValue() : 953850002;
        this.appointementFromDate = formContext.getAttribute('va_appointmentfromdate').getValue();
        this.appointementToDate = formContext.getAttribute('va_appointmenttodate').getValue();
        this.doSearchPathways = formContext.getAttribute('va_searchcorpall').getValue();

        return true;
    }
}

vrmContext.prototype.constructor = vrmContext;

vrmContext.prototype.environment;

vrmContext.prototype.user;
vrmContext.prototype.claimIdList;
vrmContext.prototype.awardIdList;
vrmContext.prototype.appealKeyList;
vrmContext.prototype.progressWindow;
vrmContext.prototype.extjsSectionName;
vrmContext.prototype.extjsTabName;
vrmContext.prototype.parameters;
vrmContext.prototype.refreshExtjs;
vrmContext.prototype.endState;
vrmContext.prototype.ignoreBirlsError;

// Possible Search Params
vrmContext.prototype.searchCORPBy; // SSN or PARTICIPANTID
vrmContext.prototype.ssn;
vrmContext.prototype.participantId;
vrmContext.prototype.edpi;
vrmContext.prototype.firstName;
vrmContext.prototype.lastName;
vrmContext.prototype.middleName;
vrmContext.prototype.dob;
vrmContext.prototype.city;
vrmContext.prototype.state;
vrmContext.prototype.zipCode;
// Birls
vrmContext.prototype.branchOfService;
vrmContext.prototype.serviceNumber;
vrmContext.prototype.insuranceNumber;
vrmContext.prototype.dod;
vrmContext.prototype.eod;
vrmContext.prototype.rad;
vrmContext.prototype.suffix;
vrmContext.prototype.folderLocation;
vrmContext.prototype.fileNumber;
vrmContext.prototype.payeeNumber;
// Appeals
vrmContext.prototype.AppealsSearchType; // FileNumber  SSN ThisValue
vrmContext.prototype.appealsSearchValue; // to be used when AppealsSearchType =  ThisValue
vrmContext.prototype.appealsSsn;
vrmContext.prototype.appealsLastName;
vrmContext.prototype.appealsFirstName;
vrmContext.prototype.appealsDateOfBirth;
vrmContext.prototype.appealsCity;
vrmContext.prototype.appealsState;
// MVI/Pathways
vrmContext.prototype.gender;
vrmContext.prototype.appointementFromDate;
vrmContext.prototype.appointementToDate;
vrmContext.prototype.doSearchPathways;