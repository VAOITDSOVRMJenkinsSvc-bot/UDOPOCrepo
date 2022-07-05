function Jscript_EnableRule() {

    //TRUE will enable the button
    var callType = Xrm.Page.getAttribute('va_disposition').getValue();

    if (callType == 953850027) {
        return true;
    }
    else {
        return false;
    }
}

function blankLetters() {

    var GUIDvalue = Xrm.Page.data.entity.getId();
    var now = new Date();
    var today = (now.getMonth() + 1) + '/' + now.getDate() + '/' + now.getFullYear() + ' ' + now.getHours() + ':' + ((now.getMinutes() < 10 ? '0' : '') + now.getMinutes());
    var parameters = {};
    var address1 = Xrm.Page.getAttribute('va_calleraddress1').getValue();
    var address2 = Xrm.Page.getAttribute('va_calleraddress2').getValue();
    var address3 = Xrm.Page.getAttribute('va_calleraddress3').getValue();
    var city = Xrm.Page.getAttribute('va_callercity').getValue();
    var state = Xrm.Page.getAttribute('va_callerstate').getValue();
    var zip = Xrm.Page.getAttribute('va_callerzipcode').getValue();
    var country = Xrm.Page.getAttribute('va_callercountry').getValue();
    var participantid = Xrm.Page.getAttribute('va_participantid').getValue();
    var PCRName = Xrm.Page.getAttribute('createdby').getValue()[0].id;
    var firstName = Xrm.Page.getAttribute('va_callerfirstname').getValue();
    var lastName = Xrm.Page.getAttribute('va_callerlastname').getValue();
    var callerRelation = Xrm.Page.getAttribute('va_callerrelationtoveteran').getSelectedOption().text;
    var ssn = Xrm.Page.getAttribute('va_ssn').getValue();
    var email = Xrm.Page.getAttribute('va_calleremail').getValue();
    var PID = Xrm.Page.getAttribute('va_participantid').getValue();
    var sojrpo = Xrm.Page.getAttribute('va_sojrpo').getValue();

    parameters["va_originatingcallid"] = GUIDvalue;
    parameters["va_participantid"] = participantid;
    parameters["va_reqnumber"] = 'Blank Forms ' + today;
    parameters["va_issue"] = '953850027';
    parameters["va_action"] = '953850004';
    parameters["va_mailing_address1"] = address1;
    parameters["va_mailing_address2"] = address2;
    parameters["va_mailing_address3"] = address3;
    parameters["va_mailing_city"] = city;
    parameters["va_mailing_zip"] = zip;
    parameters["va_mailing_state"] = state;
    parameters["va_mailingcountry"] = country;
    parameters["va_pcrofrecordid"] = PCRName;
    parameters["va_srfirstname"] = firstName;
    parameters["va_srlastname"] = lastName;
    parameters["va_srrelation"] = callerRelation;
    parameters["va_srssn"] = ssn;
    parameters["va_emailofveteran"] = email;
    parameters["va_sremail"] = email;
    parameters["va_participantid"] = PID;
    parameters["va_sendemailtoveteran"] = true;
    parameters["va_sojrpo"] = sojrpo;

    //VT 2014-12-19 Add functionality to allow embedding into more entity types
    var va_vetContactId = Xrm.Page.getAttribute('va_vetcontactid');
    if (va_vetContactId != null) {
        parameters["va_vetcontactid"] = va_vetContactId.getValue();
    } else {
        parameters["va_vetcontactid"] = null;
    }

    // Open the window.

    //Xrm.Utility.openEntityForm("va_servicerequest", null, parameters);

    //VTRIGILI 2014-02-10 - convert to window.open
    var args = "";
    for (fieldName in parameters) {
        fieldValue = parameters[fieldName];
        args = args + fieldName + "=" + fieldValue + "&";
    }
    args = args.slice(0, -1); //remove last &
    var orgName = Xrm.Page.context.getOrgUniqueName();
    targetURL = "/" + orgName +  "/main.aspx?etn=va_servicerequest&pagetype=entityrecord&extraqs=" + encodeURIComponent(args);
    window.open(targetURL, "_blank", null, false);


}
