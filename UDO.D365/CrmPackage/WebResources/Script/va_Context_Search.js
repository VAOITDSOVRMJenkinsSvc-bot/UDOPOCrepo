var vrmContext = function (exCon) {
    var formContext = exCon.getFormContext();
    this.parameters = new Array();
    this.ignoreBirlsError = false;

    this.SetSearchParameters = function () {
        this.environment = _GetEnvironment();

        this.participantId = null;
        this.fileNumber = null;
        this.searchCORPBy = null;
        this.ssn = null;  // search by fileno, get ssn from search results
        this.edpi = null;   // not used yet

        var fileNumber = Xrm.Page.getAttribute('va_ssn').getValue();
        var participantId = Xrm.Page.getAttribute('va_participantid').getValue();

        var searchOption = 1;

        if (Xrm.Page.data.entity.getEntityName() == 'contact') {   //contact screen
            if (participantId) {
                searchOption = 2; // search by pid
            } else {
                searchOption = 1; //search by filenumber
            }

        }
        else {
            searchOption = Xrm.Page.getAttribute('va_searchtype').getValue();
        }

        switch (parseInt(searchOption)) {
            case 2:
                // SEARCH BY: PID
                if (participantId) {
                    if (Xrm.Page.data.entity.getEntityName() != 'contact') {
                        Xrm.Page.getAttribute('va_ssn').setValue(null);
                        Xrm.Page.getAttribute('va_firstname').setValue(null);
                        Xrm.Page.getAttribute('va_lastname').setValue(null);
                    }
                    this.participantId = participantId;
                }
                else {
                    alert('Participant ID field is blank.');
                    return false;
                }
                this.searchCORPBy = 'PARTICIPANTID'; //SSN or PARTICIPANTID
                break;
            case 3:
                // SEARCH BY: EDIPI
                alert('Connection to the authorization store has not yet been granted. Please come back later.');
                return false;
            case 1:
            default:
                // SEARCH BY: SSN + other params

                if (Xrm.Page.data.entity.getEntityName() == 'contact') { //tas 200232 if (crmForm.ObjectTypeCode != 2) {
                    Xrm.Page.getAttribute('va_participantid').setValue(null);
                    Xrm.Page.getAttribute('va_edipi').setValue(null);
                }
                if (fileNumber) {
                    this.fileNumber = fileNumber;
                }
                break;
        }
        // ES: Per bug #98779 replace Xrm.Page.getAttribute('xxx') with crmForm.all
        var c = Xrm.Page.getAttribute('va_firstname'); if (!c) { c = Xrm.Page.getAttribute('firstname'); } this.firstName = c.getValue();
        c = Xrm.Page.getAttribute('va_lastname'); if (!c) { c = Xrm.Page.getAttribute('lastname'); } this.lastName = c.getValue();
        c = Xrm.Page.getAttribute('va_middleinitial'); if (!c) { c = Xrm.Page.getAttribute('middlename'); } this.middleName = c.getValue();

        //RTC 108417: Use DOB Text field and convert to date/time.
        var dobDate = (Xrm.Page.getAttribute('va_dobtext') != null && Xrm.Page.getAttribute('va_dobtext').getValue() != null) ? new Date(Xrm.Page.getAttribute('va_dobtext').getValue()) : null;
        this.dob = dobDate ? dobDate.format("MMddyyyy") : null;
        ////this.city = Xrm.Page.getAttribute('va_citysearch').getValue();
        ////this.state = Xrm.Page.getAttribute('va_statesearch').getValue();
        ////this.zipCode = Xrm.Page.getAttribute('va_zipcodesearch').getValue();

        // Birls
        ////this.branchOfService = Xrm.Page.getAttribute('va_branchofservice').getValue();
        ////this.serviceNumber = Xrm.Page.getAttribute('va_servicenumber').getValue();
        ////this.insuranceNumber = Xrm.Page.getAttribute('va_insurancenumber').getValue();
        ////this.dod = (Xrm.Page.getAttribute('va_dod') != null && Xrm.Page.getAttribute('va_dod').getValue() != null) ? Xrm.Page.getAttribute('va_dod').getValue().format("MMddyyyy") : null;
        ////this.eod = (Xrm.Page.getAttribute('va_enteredondutydate') != null && Xrm.Page.getAttribute('va_enteredondutydate').getValue() != null) ? Xrm.Page.getAttribute('va_enteredondutydate').getValue().format("MMddyyyy") : null;
        ////this.rad = (Xrm.Page.getAttribute('va_releasedactivedutydate') != null && Xrm.Page.getAttribute('va_releasedactivedutydate').getValue() != null) ? Xrm.Page.getAttribute('va_releasedactivedutydate').getValue().format("MMddyyyy") : null;
        ////this.suffix = Xrm.Page.getAttribute('va_suffix').getValue();
        ////this.folderLocation = Xrm.Page.getAttribute('va_folderlocation').getValue();
        ////this.payeeNumber = Xrm.Page.getAttribute('va_participantid').getValue();

        // Appeals        
        //tas var userSelection = crmForm.all.va_findappealsby').getValue();
        ////var userSelection = Xrm.Page.getAttribute('va_findappealsby').getValue();
        ////if (userSelection == 953850000) { userSelection = 'fileNumber'; }
        ////else if (userSelection == 953850001) { userSelection = 'ssn'; }
        ////else if (userSelection == 953850002) { userSelection = 'values'; }
        ////else { userSelection = 'fileNumber'; }

        ////this.appealsSearchValue = userSelection;
        ////if (userSelection == 'values') {
        ////    this.appealsSsn = Xrm.Page.getAttribute('va_appealsssn').getValue();
        ////    this.appealsLastName = Xrm.Page.getAttribute('va_appealslastname').getValue();
        ////    this.appealsFirstName = Xrm.Page.getAttribute('va_appealsfirstname').getValue();
        ////    this.appealsDateOfBirth = Xrm.Page.getAttribute('va_appealsdateofbirth').getValue();
        ////    this.appealsCity = Xrm.Page.getAttribute('va_appealscity').getValue();
        ////    this.appealsState = Xrm.Page.getAttribute('va_appealsstate').getValue();
        ////}
        // MVI/Pathways
        ////this.gender = (Xrm.Page.getAttribute('va_genderset') != null) ? Xrm.Page.getAttribute('va_genderset').getValue() : 953850002;
        ////this.appointementFromDate = Xrm.Page.getAttribute('va_appointmentfromdate').getValue();
        ////this.appointementToDate = Xrm.Page.getAttribute('va_appointmenttodate').getValue();
        ////this.doSearchPathways = Xrm.Page.getAttribute('va_searchcorpall').getValue();

        return true;
    }

    /*Possible Context Params
    this.parameters['fileNumber'] = null;
    this.parameters['ptcpntId'] = null;
    this.parameters['ptcpntVetId'] = null;
    this.parameters['ptcpntBeneId'] = null;
    this.parameters['ptcpntRecipId'] = null;
    this.parameters['claimId'] = null;
    this.parameters['payeeCode'] = null;
    this.parameters['awardTypeCd'] = null;
    this.parameters['rbaIssueId'] = null;
    this.parameters['ssn'] = null;
    this.parameters['appealKey'] = null;
    this.parameters['documentId'] = null;*/
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

//Possible Search Params
vrmContext.prototype.searchCORPBy; //SSN or PARTICIPANTID
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