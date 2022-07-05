var exCon = null;
var formContext = null;

function onSave(executionObj) {
    var modeSaveAndClose = 2,
        saveMode = executionObj.getEventArgs().getSaveMode(),
        hasClosedDate = Xrm.Page.getAttribute('va_dateclosed').getValue();

    if (!validation()) {
        executionObj.getEventArgs().preventDefault();  // RU12 changed event.returnValue
        return false;
    }

    if (!_UserSettings)
        _UserSettings = GetUserSettingsForWebservice(executionObj);
    

    if (isVaiResolved() && !hasClosedDate) {
        var now = new Date();
        Xrm.Page.getAttribute('va_dateclosed').setValue(now);
        Xrm.Page.getAttribute('va_dateclosed').setSubmitMode('always');
        
        if (!Xrm.Page.getAttribute("va_participantid").getValue()) {
            alert('Participant ID field on Additional Information tab is blank. Without this field, Development Note cannot be created.');
            return true;
        }
        
        var noteText = {
            fullName: _UserSettings.fullName,
            closedDate: Xrm.Page.getAttribute('va_dateclosed').getValue(), 
            openedDate: Xrm.Page.getAttribute('va_dateopened').getValue(),
            vaiNumber: Xrm.Page.getAttribute('va_vainumber').getValue(),
            statusAction: Xrm.Page.getAttribute('statuscode').getText(),
            participantId: Xrm.Page.getAttribute("va_participantid").getValue()
        };
        
        var success = addMapdNoteToVAI(noteText);
        
        if (!success && !confirm('Failed to create a Dev Note.\r\n' +
            'Would you like to continue saving Service Request record?\n\nServer returned following error information: ' + GetErrorMessages('\n'))) {
            executionObj.getEventArgs().preventDefault();
            return false;
        }
    }

    return true;
}

function addMapdNoteToVAI(note) {
    //Requirements for content of Map-D notes: 
    //- agent user name 
    //- date  opened
    //- Time opened
    //- date closed 
    //-  Time closed
    //- Inquiry request #
    //- capture action taken by PCT to resolve VAI
    
    var mapdResults, mapdOptions, mapdNoteTextFormated;

    mapdNoteTextFormated = 'Agent: ' + note.fullName + '\r\n' +
                         'Opened Date: ' + UTIL.dateFormat(note.openedDate, 'MM-dd-yyyy h:mm:ss') + '\r\n' + 
                         'Closed Date: ' + UTIL.dateFormat(note.closedDate, 'MM-dd-yyyy h:mm:ss') + '\r\n' +
                         'Inquiry Request #: ' + note.vaiNumber + '\r\n' +
                         'Action Taken: ' + note.statusAction + '\r\n';

    mapdOptions = {
        ptcpntId: note.participantId,
        noteText: mapdNoteTextFormated
    };
    


    mapdResults = UTIL.mapdNote(mapdOptions);

    return Boolean(mapdResults) && mapdResults.success;
}

function isVaiResolved() {
    // Solved, Solved - Duplicate, Slvd-Too Little Info
    var status = [953850015, 953850016, 953850017],
        value = Xrm.Page.getAttribute('statuscode').getValue();
    
    for (var i = 0; i < status.length; i++) {
        if (status[i] === value)
            return true;
    }
    return false;
}

function validation() {
    // Validate Status Resasons
    var statusReasonNotSet = 953850018;
    if (Xrm.Page.getAttribute('statuscode').getValue() === statusReasonNotSet) {
        alert("The VAI 'Status Reason' has not been set.");
        Xrm.Page.getControl('statuscode').setFocus();
        // RU12 removed event.returnValue
        return false;
    }

    // Other validation go hear
    return true;
}