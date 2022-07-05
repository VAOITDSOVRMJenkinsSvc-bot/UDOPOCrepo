function ChatCoBrowseOnSave(exCon) {
    var formContext = exCon.getFormContext();
    var sessiontype= formContext.getAttribute('crme_sessiontype');

    // ensure all required fields are set
    if (sessiontype!= null && sessiontype.getValue() == null) {
        alert('Session type not specified.');
        event.returnValue = false;
        return true;
    }

    return false;
}