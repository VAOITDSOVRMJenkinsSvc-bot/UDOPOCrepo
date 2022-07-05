"use strict";

// Global Variables
var _executionContext = null;
var _formContext = null;
var globalcadd = {
    bankdata: "",
    cadddata: "",
    idproof: "",
    ca: false,
    ca_a: "",
    ca_r: "",
    ca_t: ""
};

parent.CADDIdProofingComplete = CADDIdProofingComplete;
parent.CopyAddress = CopyAddress;
parent.CopyAddress2 = CopyAddress2;
parent.va_routingnumberChange = va_routingnumberChange;
parent.globalcadd = globalcadd;

var _changingAwardAddress = true;
var _fromClaims = false;
var _hasAppeals = false;
var _participantId = '';

var _addressTypes = { Domestic: 953850000, International: 953850001, Overseas: 953850002 };

var awardAddresses_xmlObject = null;
var selectedAward_xmlObject = null;
var vetFileNumber = null;
var vetSSN = null;
var veteranInformation_xml = null;
var veteranInformation_xmlObject = null;
var parent_page = null;

function retrieveFormContext(executionContext) {
    if (executionContext && executionContext.getFormContext) {
        return executionContext.getFormContext();
    }
    else {
        return null;
    }
}

function OnLoad(executionContext) {
    try {
        _executionContext = executionContext;
        formContext = retrieveFormContext(executionContext);
        _formContext = formContext;

        var wrControlIDProof = formContext.getControl("WebResource_udo_cadd_idproofcomplete");
        if (wrControlIDProof !== 'undefined' && wrControlIDProof !== null) {
            wrControlIDProof.getContentWindow().then(
                function (contentWindow) {
                    contentWindow.setContext(exCon);
                }
            )
        }

        var wrControlPaymentAddress = formContext.getControl("WebResource_udo_CADD_Payment_Copy_Address");
        if (wrControlPaymentAddress !== 'undefined' && wrControlPaymentAddress !== null) {
            wrControlPaymentAddress.getContentWindow().then(
                function (contentWindow) {
                    contentWindow.setContext(exCon);
                }
            )
        }

        var wrControlAppellantAddress = formContext.getControl("WebResource_udo_CADD_Appellent_Copy_Address");
        if (wrControlAppellantAddress !== 'undefined' && wrControlAppellantAddress !== null) {
            wrControlAppellantAddress.getContentWindow().then(
                function (contentWindow) {
                    contentWindow.setContext(exCon);
                }
            )
        }

        var wrControlFindBank = formContext.getControl("WebResource_udo_CADD_Find_Bank");
        if (wrControlFindBank !== 'undefined' && wrControlFindBank !== null) {
            wrControlFindBank.getContentWindow().then(
                function (contentWindow) {
                    contentWindow.setContext(exCon);
                }
            )
        }

        environmentConfigurations.initalize(executionContext).then(function () {
            commonFunctions.initalize(executionContext);
            ws.vetRecord.initalize(executionContext);
            ws.claimant.initalize(executionContext);
            ws.benefitClaim.initalize(executionContext);
            ws.appeals.initalize(executionContext);
            ws.mapDDevelopmentNotes.initalize(executionContext);
            ws.addressValidation.initalize();

            onFormLoad(executionContext);

            // Disables request/response fields
            formContext.getControl('va_findawardaddressesresponse').setDisabled(false);
            formContext.getControl('va_findveteranbyptcpntidresponse').setDisabled(false);
            formContext.getControl('va_updateaddressrequest').setDisabled(false);
            formContext.getControl('va_updateaddressresponse').setDisabled(false);
            formContext.getControl('va_updateappellantaddressresponse').setDisabled(false);
            formContext.getControl('va_validateaddressresponse').setDisabled(false);
            var currentStatusText = _formContext.getAttribute("udo_caddstatus").getSelectedOption().text;

            // Display CADD has not been submitted message
            if (currentStatusText !== "Complete") {
                formContext.ui.setFormNotification("Current CADD Status: Not Submitted", "WARNING", "CURRENTCADDSTATUS");

                formContext.ui.tabs.get('tabExecutionResults').setVisible(false);
                formContext.ui.tabs.get('tabExecutionResults').sections.get('sectionExecutionResults').setVisible(false);

                formContext.ui.tabs.get('tabCADDMaintenance').sections.get('sectionMailingPaymentAppellantAddresses').setVisible(false);
                formContext.ui.tabs.get('tabCADDMaintenance').sections.get('sectionEmailPhoneNumbers').setVisible(false);
                formContext.ui.tabs.get('tabCADDMaintenance').sections.get('sectionDepositAccount').setVisible(false);
            }
            else {
                formContext.ui.setFormNotification("Current CADD Status: Completed", "INFO", "CURRENTCADDSTATUS");
                DisableAll();

                formContext.ui.tabs.get('tabExecutionResults').setVisible(true);
                formContext.ui.tabs.get('tabExecutionResults').sections.get('sectionExecutionResults').setVisible(true);

                formContext.ui.tabs.get('tabCADDMaintenance').setVisible(true);
                formContext.ui.tabs.get('tabCADDMaintenance').sections.get('sectionMailingPaymentAppellantAddresses').setVisible(true);
                formContext.ui.tabs.get('tabCADDMaintenance').sections.get('sectionEmailPhoneNumbers').setVisible(true);
                formContext.ui.tabs.get('tabCADDMaintenance').sections.get('sectionDepositAccount').setVisible(true);

																																
            }

            formContext.ui.tabs.get('tabExecutionResults').setVisible(false);
            formContext.ui.tabs.get('tabExecutionResults').sections.get('sectionExecutionResults').setVisible(false);
            formContext.ui.tabs.get("tabWebServiceResponse").setVisible(false);
            var userRoles = Xrm.Utility.getGlobalContext().userSettings.roles;
            userRoles.forEach(function (role, index) {
																							   
                if (role.name == "System Administrator") {
                    console.log("User is System Administrator");
                    formContext.ui.tabs.get('tabExecutionResults').setVisible(true);
                    formContext.ui.tabs.get('tabExecutionResults').sections.get('sectionExecutionResults').setVisible(true);
                    formContext.ui.tabs.get("tabWebServiceResponse").setVisible(true);
					 
                }
            });

																	 
        });
    } catch (e) {
        console.log(e);
    }
}

function onFormLoad(executionContext) {
    _formContext = retrieveFormContext(executionContext);

    var optionValue = _formContext.getAttribute("va_addresstype").getValue();
    switch (optionValue) {
        case 953850000: // Domestic
            var mailingState = _formContext.getAttribute("va_verifystate").getValue();
            _formContext.getAttribute('va_mailingstateoptionset').setValue(GetOptionValue('va_mailingstateoptionset', mailingState));

            var mailingZip = _formContext.getAttribute("va_verifyzipcode").getValue();
            _formContext.getAttribute('va_mailingaddresszipcode').setValue(mailingZip);

            _formContext.getAttribute('va_mailingmilitarypostaltypecode').setValue(null);
            _formContext.getAttribute('va_mailingmilitarypostofficetypecode').setValue(null);
            _formContext.getAttribute('va_milzipcodelookupid').setValue(null);

            break;

        case 953850001: // International 
            var mailingCountry = _formContext.getAttribute('va_mailingcountry').getValue();
            if (mailingCountry !== null || mailingCountry !== '') {
                SetOptionSetFromValue('va_mailingcountrylist', mailingCountry);
            }

            var foreignZip = _formContext.getAttribute("va_verifyzipcode").getValue();
            if (foreignZip !== null && foreignZip !== '') {
                _formContext.getAttribute('va_mailingforeignpostalcode').setValue(foreignZip);
            } else {
                MailingCountryList_Onchange(executionContext);
            }

            break;

        case 953850002: // Overseas Military
            var udoPostOfficeTypeCode = _formContext.getAttribute("va_verifycity").getValue();
            SetOptionSetFromValue('va_mailingmilitarypostofficetypecode', udoPostOfficeTypeCode);

            var udoPostalTypeCode = _formContext.getAttribute("va_verifystate").getValue();
            SetOptionSetFromValue('va_mailingmilitarypostaltypecode', udoPostalTypeCode);

            _formContext.getAttribute('va_mailingstateoptionset').setValue(null);
            _formContext.getAttribute('va_mailingaddresszipcode').setValue(null);

            // Country must be blank for Overseas address type
            _formContext.getAttribute('va_verifycountry').setValue(null);
            _formContext.getAttribute('va_mailingcountry').setValue(null);

            var milZipCode = _formContext.getAttribute("va_verifyzipcode").getValue();
            _formContext.getAttribute('va_mailingaddresszipcode').setValue(milZipCode);

            RetrieveMilitaryZipCode(milZipCode);
            break;

        default:  // Null or not selected
            break;
    }

    var CADD_Status = _formContext.getAttribute("udo_caddstatus").getText();
    if (CADD_Status !== "Complete") {
        SetOptionSetValue("udo_caddstatus", "New");

        // Adds onchange event code to capture the name from the selected look up and add to va_mailingaddresszipcode
        // For CADD we will need Mil Zip Code Look up entity
        _formContext.getAttribute('va_milzipcodelookupid').addOnChange(MilZipCodeChange);

        _participantId = _formContext.getAttribute("udo_participantid").getValue(); // Assign participant id

        SetAddlOptionSetFields();
        globalcadd.ca_r = formContext.getAttribute("va_routingnumber").getValue();  // RoutingNumber
        globalcadd.ca_a = formContext.getAttribute("va_depositaccountnumber").getValue(); // AccountNumber
        globalcadd.ca_t = formContext.getAttribute("va_depositaccounttype").getText();  // AccountType

        var routingNumber = globalcadd.ca_r;

        if (routingNumber !== null) {
            var va_crn_x = formContext.getAttribute("udo_crn");

            if ((va_crn_x !== null) && routingNumber === va_crn_x.getValue()) {
                globalcadd.ca = true;

                var len = routingNumber.length - 4;
                var masked = '';
                for (var i = 0; i < len; i++) {
                    masked += '*';
                }
                masked += routingNumber.substr(len, 4);
                routingNumber = masked;

                var accountNumber = '**********';

                _formContext.getAttribute('va_routingnumber').setValue(routingNumber);
                _formContext.getAttribute("va_depositaccountnumber").setValue(accountNumber);
            }
        }
        else {
            globalcadd.ca = false;
        }

        SetMandatoryFields(executionContext);

        fnIDProof();

        _formContext.getAttribute("va_addresstype").addOnChange(AddressType_onChange);
    }
    else {
        DisableAll();

        return;
    }
}

function RetrieveMilitaryZipCode(milZipCode) {
    new Promise(function (resolve, reject) {
        Xrm.WebApi.online.retrieveMultipleRecords("va_milzipcodelookup", "?$select=va_milzipcodelookupid,va_zip&$filter=va_zip eq '" + milZipCode + "'").then(
            function success(data) {
                if (data.entities.length > 0) {
                    var lookup = new Array();
                    lookup[0] = new Object();
                    lookup[0].id = data.entities[0].va_milzipcodelookupid;
                    lookup[0].name = data.entities[0].va_zip;
                    lookup[0].entityType = 'va_milzipcodelookup';
                    _formContext.getAttribute("va_milzipcodelookupid").setValue(lookup);
                } else {
                    var msg = "This person has an invalid military zip code.\n\nPlease perform a CADD to correct the zip code.";
                    var title = "Invalid Zip Code";
                    UDO.Shared.openAlertDialog(msg, title, null, null);
                }
                return resolve();
            },
            function (error) {
                var msg = error.message;
                var title = "Zip Code Lookup Error";
                UDO.Shared.openAlertDialog(msg, title, null, null);
                return reject();
            });
        return resolve();
    });
}

function AddressType_onChange() {
    var addressType = _formContext.getAttribute("va_addresstype").getSelectedOption();
    var curCountry = _formContext.getAttribute('va_mailingcountry').getValue();

    if (addressType.value === _addressTypes.Overseas && curCountry !== null) {
        // For overseas address type, clear out the country value
        formContext.getAttribute('va_mailingcountry').setValue(null);
    }
}

function CADDIdProofingComplete() {
    var attr = [_formContext.getAttribute("va_currentmonthlybenefit"), _formContext.getAttribute("va_addressofrecord"), _formContext.getAttribute("va_dobverified")];
    for (var a in attr) {
        attr[a].setValue(true);
    }

    _formContext.getAttribute("va_failedidproofing").setValue(false);

    return new Promise(function (resolve, reject) {
        var msg = "ID Proofing boxes have been checked to indicate successful Proofing for Change of Address?";
        var title = "ID Proof Complete";
        UDO.Shared.openConfirmDialog(msg, title, "", "")
            .then(
                function (response) {
                    if (response.confirmed) {
                        if (globalcadd.idproof === "passed") {
                            _formContext.getControl("va_addresstype").setFocus();
                        } else {
                            _formContext.getControl("va_failedidproofing").setFocus();
                        }

                        formContext.ui.tabs.get('tabCADDMaintenance').setVisible(true);
                        formContext.ui.tabs.get('tabCADDMaintenance').sections.get('sectionMailingPaymentAppellantAddresses').setVisible(true);
                        formContext.ui.tabs.get('tabCADDMaintenance').sections.get('sectionEmailPhoneNumbers').setVisible(true);
                        formContext.ui.tabs.get('tabCADDMaintenance').sections.get('sectionDepositAccount').setVisible(true);

                        fnIDProof();
                    }

                    return resolve();
                },
                function (error) {
                    return reject();
                });
    });
}

function convertDate(dateString) {
    var p = dateString.split(/\D/g)
    return [p[1], p[2], p[0]].join("/")
}

function MilZipCodeChange() {
    var selectedZip = '';

    if (_formContext.getAttribute("va_milzipcodelookupid").getValue() &&
        _formContext.getAttribute("va_milzipcodelookupid").getValue().length > 0 &&
        _formContext.getAttribute("va_milzipcodelookupid").getValue()[0]) {
        selectedZip = _formContext.getAttribute("va_milzipcodelookupid").getValue()[0].name;
    }

    if (selectedZip) { _formContext.getAttribute('va_mailingaddresszipcode').setValue(selectedZip); }
}

function DisableAll() {
    _formContext.getControl('va_type').setVisible(false);

    _formContext.ui.controls.forEach(function (control, index) {
        if (control && control.getDisabled && !control.getDisabled()) {
            control.setDisabled(true);
        }
    });
}

function CopyAddress() {
    return new Promise(function (resolve, reject) {
        var msg = "Please confirm that you would like to copy mailing address to the payment address section?";
        var title = "Copy to Payment Address";
        UDO.Shared.openConfirmDialog(msg, title, "", "")
            .then(
                function (response) {
                    if (response.confirmed) {
                        if (ValidateFields(_executionContext)) {
                            _formContext.getAttribute('va_paymentaddress1').setValue(_formContext.getAttribute('va_mailingaddress1').getValue());
                            _formContext.getAttribute('va_paymentaddress2').setValue(_formContext.getAttribute('va_mailingaddress2').getValue());
                            _formContext.getAttribute('va_paymentaddress3').setValue(_formContext.getAttribute('va_mailingaddress3').getValue());

                            _formContext.getAttribute('va_paymentaddresschanged').setValue(true);
                            _formContext.getControl("va_paymentaddress1").setFocus();

                            _formContext.getAttribute("va_paymenteffectivedate").setValue(_formContext.getAttribute("va_mailingeffectivedate").getValue());

                            var optionValue = _formContext.getAttribute("va_addresstype").getValue();
                            switch (optionValue) {
                                case _addressTypes.Domestic:
                                    _formContext.getAttribute('va_paymentcity').setValue(_formContext.getAttribute('va_mailingcity').getValue());
                                    _formContext.getAttribute('va_paymentstate').setValue(_formContext.getAttribute('va_mailingstate').getValue());
                                    _formContext.getAttribute('va_paymentstateoptionset').setValue(_formContext.getAttribute('va_mailingstateoptionset').getValue());
                                    _formContext.getAttribute('va_paymentzipcode').setValue(_formContext.getAttribute('va_mailingaddresszipcode').getValue());
                                    break;

                                case _addressTypes.International:
                                    _formContext.getAttribute('va_paymentcity').setValue(_formContext.getAttribute('va_mailingcity').getValue());
                                    _formContext.getAttribute('va_paymentzipcode').setValue(null);
                                    SetOptionSetValue('va_paymentcountrylist', _formContext.getAttribute('va_mailingcountrylist').getText());
                                    _formContext.getAttribute('va_paymentcountry').setValue(_formContext.getAttribute('va_mailingcountry').getValue());
                                    break;

                                case _addressTypes.Overseas:
                                    _formContext.getAttribute('va_paymentcity').setValue(null);
                                    _formContext.getAttribute('va_paymentmilitarypostaltypecode').setValue(_formContext.getAttribute('va_mailingmilitarypostaltypecode').getValue());
                                    _formContext.getAttribute('va_paymentmilitarypostofficetypecode').setValue(_formContext.getAttribute('va_mailingmilitarypostofficetypecode').getValue());
                                    _formContext.getAttribute('va_paymentzipcode').setValue(_formContext.getAttribute("va_milzipcodelookupid").getValue()[0].name);
                                    break;
                            }
                        }
                    }

                    return resolve();
                },
                function (error) {
                    return reject();

                });
    });

}

function ValidateFields(executionContext) {
    var Errors = {};
    var payeeCode = parseInt(_formContext.getAttribute("va_payeetypecode").getValue());
    var addressType = _formContext.getAttribute("va_addresstype").getSelectedOption();
    var ValidationFields;

    // PC 11-29 are children recipients
    if (payeeCode > 10 && payeeCode < 30) {
        ValidationFields = GetAddressArray(true);
    }
    else {
        ValidationFields = GetAddressArray();
    }

    // Applies the max length restriction to fields.
    for (var field in ValidationFields) {
        var input = _formContext.getAttribute(field).getValue();
        if (input && input.length > ValidationFields[field]) {
            Errors[_formContext.getControl(field).getLabel()] = "Length surpassed max value of " + ValidationFields[field] + ";";
        }
        ValidationFields[field] = "";
    }

    if (parseInt(_formContext.getAttribute("va_mailingaddresszipcode").getValue()) === NaN) {
        if (Errors[_formContext.getControl("va_mailingaddresszipcode").getLabel()] !== undefined) {
            Errors[_formContext.getControl("va_mailingaddresszipcode").getLabel()] += 'Must be a number;';
        }
        else {
            Errors[_formContext.getControl("va_mailingaddresszipcode").getLabel()] = 'Must be a number;';
        }
    }

    if (payeeCode >= 0 && payeeCode < 30) {
        for (var field in ValidationFields) {
            if (_formContext.getControl(field).getLabel() === "Address 1") {
                var success = TestForAllowedChars(_formContext.getAttribute(field).getValue(), [14, 27]);
                if (success === false) {
                    if (Errors[_formContext.getControl(field).getLabel()] !== undefined) {
                        Errors[_formContext.getControl(field).getLabel()] += "Field can only contain alphanumeric characters, dashes, slashes, and single spaces;";
                    }
                    else {
                        Errors[_formContext.getControl(field).getLabel()] = "Field can only contain alphanumeric characters, dashes, slashes, and single spaces;";
                    }
                }
            }

            if (_formContext.getControl(field).getLabel() === "Address 2") {
                success = TestForAllowedChars(_formContext.getAttribute(field).getValue(), [33, 24, 5, 7, 9, 23, 11, 12, 15, 31, 14, 29, 27, 25, 4]);
                if (success === false) {
                    if (Errors[_formContext.getControl(field).getLabel()] !== undefined) {
                        Errors[_formContext.getControl(field).getLabel()] += "Field contains a non-allowable character;";
                    }
                    else {
                        Errors[_formContext.getControl(field).getLabel()] = "Field contains a non-allowable character;";
                    }
                }
            }

            if (_formContext.getControl(field).getLabel() === "Address 3") {
                success = TestForAllowedChars(_formContext.getAttribute(field).getValue(), [33, 24, 5, 7, 9, 23, 11, 12, 15, 31, 14, 29, 27, 25, 4]);
                if (success === false) {
                    if (Errors[_formContext.getControl(field).getLabel()] !== undefined) {
                        Errors[_formContext.getControl(field).getLabel()] += "Field contains a non-allowable character;";
                    }
                    else {
                        Errors[_formContext.getControl(field).getLabel()] = "Field contains a non-allowable character;";
                    }
                }
            }

            if (addressType && (addressType.value === _addressTypes.International || addressType.value === _addressTypes.Domestic)) {
                if (_formContext.getControl(field).getLabel() === "City") {
                    success = TestForAllowedChars(_formContext.getAttribute(field).getValue(), [33, 24, 5, 7, 9, 23, 11, 12, 15, 31, 14, 29, 27, 25, 4]);
                    if (success === false) {
                        if (Errors[_formContext.getControl(field).getLabel()] !== undefined) {
                            Errors[_formContext.getControl(field).getLabel()] += "Field contains a non-allowable character;";
                        }
                        else {
                            Errors[_formContext.getControl(field).getLabel()] = "Field contains a non-allowable character;";
                        }
                    }
                }
            }

            if (addressType && (addressType.value === _addressTypes.Overseas || addressType.value === _addressTypes.Domestic)) {
                if (_formContext.getControl(field).getLabel() === "Zip Code") {
                    var zipSize = _formContext.getAttribute(field).getValue();
                    if (zipSize && zipSize.length !== 5) {
                        if (Errors[_formContext.getControl(field).getLabel()] !== undefined) {
                            Errors[_formContext.getControl(field).getLabel()] += "Field must be exactly 5 characters;";
                        }
                        else {
                            Errors[_formContext.getControl(field).getLabel()] = "Field must be exactly 5 characters;";
                        }
                    }
                }
            }
        } // End loop through the validation fields
    } // End payee code between 10 and 30

    var text = "";
    for (var a in Errors) {
        text += a + ": " + Errors[a] + "\n";
    }
    if (text !== "") {

        var msg = "Field Validation Failed: \n\n" + text;
        var title = "Field Validation Failed";
        UDO.Shared.openAlertDialog(msg, title, 200, 450).then(function success(result) {
            return false;
        },
            function (error) {
                console.log(error.message);
            }
        );
        return false;
    }
    else {
        return true;
    }
}

function ValidatePaymentFields() {
    var Errors = {};

    var fields = {
        va_paymentaddress1: 35,
        va_paymentaddress2: 35,
        va_paymentaddress3: 35,
        va_paymentcity: 30,
        va_paymentzipcode: 5
    };

    // Applies the max length restriction to fields.
    for (var field in fields) {
        var input = _formContext.getAttribute(field).getValue();
        if (input && input.length > fields[field]) {
            Errors[_formContext.getControl(field).getLabel()] = "Payment field length surpassed max value of " + fields[field] + ";";
        }

        if (_formContext.getControl(field).getLabel() === "Zip Code") {
            var zipSize = _formContext.getAttribute(field).getValue();
            if (zipSize && zipSize.length !== 5) {
                if (Errors[_formContext.getControl(field).getLabel()] !== undefined) {
                    Errors[_formContext.getControl(field).getLabel()] += "Field must be exactly 5 characters;";
                }
                else {
                    Errors[_formContext.getControl(field).getLabel()] = "Field must be exactly 5 characters;";
                }
            }
        }

        if (_formContext.getControl(field).getLabel() === "Address 1") {
            var success = TestForAllowedChars(_formContext.getAttribute(field).getValue(), [14, 27]);
            if (success === false) {
                if (Errors[_formContext.getControl(field).getLabel()] !== undefined) {
                    Errors[_formContext.getControl(field).getLabel()] += "Field can only contain alphanumeric characters, dashes, slashes, and single spaces;";
                }
                else {
                    Errors[_formContext.getControl(field).getLabel()] = "Field can only contain alphanumeric characters, dashes, slashes, and single spaces;";
                }
            }
        }

        success = TestForAllowedChars(_formContext.getAttribute(field).getValue(), [33, 24, 5, 7, 9, 23, 11, 12, 15, 31, 14, 29, 27, 25, 4]);
        if (success === false) {

            if (Errors[_formContext.getControl(field).getLabel()] !== undefined) {
                Errors[_formContext.getControl(field).getLabel()] += "Field contains a non-allowable character;";
            }
            else {
                Errors[_formContext.getControl(field).getLabel()] = "Field contains a non-allowable character;";
            }
        }
    }

    if (parseInt(_formContext.getAttribute("va_paymentzipcode").getValue()) === NaN) {
        if (Errors[_formContext.getControl("va_paymentzipcode").getLabel()] !== undefined) {
            Errors[_formContext.getControl("va_paymentzipcode").getLabel()] += 'Must be a number;';
        }
        else {
            Errors[_formContext.getControl("va_paymentzipcode").getLabel()] = 'Must be a number;';
        }
    }

    var text = "";
    for (var a in Errors) {
        text += a + ": " + Errors[a] + "\n";
    }
    if (text !== "") {

        var msg = "Field Validation Failed: \n\n" + text;
        var title = "Field Validation Failed";
        UDO.Shared.openAlertDialog(msg, title, 200, 450).then(function success(result) {
            context.getEventArgs().preventDefault();
            _formContext.ui.setFormNotification(text, "WARNING", "CURRENTCADDSTATUS");
            return false;
        },
            function (error) {
                console.log(error.message);
            }
        );

        context.getEventArgs().preventDefault();
        _formContext.ui.setFormNotification(text, "WARNING", "CURRENTCADDSTATUS");
        return false;
    }
    else {
        return true;
    }
}

function CheckForComericaRoutingNumber(executionContext) {
    var va_routingnumber = _formContext.getAttribute("va_routingnumber");
    if (va_routingnumber.getValue() == "072413133") {
       // _formContext.getControl("va_routingnumber").setNotification("Invalid Bank Account", "ERROR", "COMERICAROUTINGERROR");
        var msg = "This account is a Comerica debit card account and can only be updated by Comerica/Treasury. Contact Treasury's Go Direct Call Center at 1-800-333-1795 or www.GoDirect.org.";
        var title = "Invalid Bank Account";
        UDO.Shared.openAlertDialog(msg, title, null, null);
    } else {
       // _formContext.getControl("va_routingnumber").clearNotification("COMERICAROUTINGERROR");

    }

}

function va_routingnumberChange() {
    var va_routingnumber = _formContext.getAttribute("va_routingnumber");
    if (va_routingnumber.getValue() == "072413133") {        
        //_formContext.getControl("va_routingnumber").setNotification("Invalid Bank Account", "ERROR", "COMERICAROUTINGERRORC");       
        var msg = "This account is a Comerica debit card account and can only be updated by Comerica/Treasury. Contact Treasury's Go Direct Call Center at 1-800-333-1795 or www.GoDirect.org.";
        var title = "Invalid Bank Account";
        UDO.Shared.openAlertDialog(msg, title, null, null);
    } else {
        //_formContext.getControl("va_routingnumber").clearNotification("COMERICAROUTINGERRORC");
         SetOptionSetValue("udo_caddstatus", "Find Bank");
        _formContext.ui.setFormNotification("Current CADD Status: " + "Find Bank", "WARNING", "CURRENTCADDSTATUS");       
        _formContext.getAttribute('udo_caddstatus').setSubmitMode('always');
        _formContext.getAttribute('va_routingnumber').setSubmitMode('always');
        _formContext.data.save();
    }    
}

function CopyAddress2() {
    return new Promise(function (resolve, reject) {
        var msg = "Please confirm that you would like to copy mailing address to the appellant address section? \n\NOTE: Mailing Address 2 and Mailing Address 3 will combine into Appellant Address 2. \n(20 character limit enforced)";
        var title = "Copy to Appellent Address";
        UDO.Shared.openConfirmDialog(msg, title, "", "")
            .then(
                function (response) {
                    if (response.confirmed) {
                        if (ValidateFields(_executionContext)) {
                            _formContext.getAttribute('va_apellantaddress1').setValue(formContext.getAttribute('va_mailingaddress1').getValue());

                            var Add2combine = ((_formContext.getAttribute('va_mailingaddress2').getValue() !== null) ? (_formContext.getAttribute('va_mailingaddress2').getValue() + ' ') : '') + ((_formContext.getAttribute('va_mailingaddress3').getValue() !== null) ? (_formContext.getAttribute('va_mailingaddress3').getValue()) : '');

                            if (Add2combine.length >= 20) {
                                Add2combine = Add2combine.trim();
                                Add2combine = Add2combine.slice(0, 20).trim();
                            }

                            _formContext.getAttribute('va_apellantaddress2').setValue(Add2combine);
                            _formContext.getAttribute('va_apellantzipcode').setValue(formContext.getAttribute('va_mailingaddresszipcode').getValue());

                            var addressType = _formContext.getAttribute("va_addresstype").getValue();
                            switch (addressType) {
                                case _addressTypes.Domestic:

                                    _formContext.getAttribute('va_apellantcity').setValue(formContext.getAttribute('va_mailingcity').getValue());
                                    _formContext.getAttribute('va_apellantstate').setValue(formContext.getAttribute('va_mailingstate').getValue());
                                    _formContext.getAttribute('va_appellantstateoptionset').setValue(formContext.getAttribute('va_mailingstateoptionset').getValue());

                                    break;
                                case _addressTypes.International:

                                    _formContext.getAttribute('va_apellantcity').setValue(formContext.getAttribute('va_mailingcity').getValue());
                                    _formContext.getAttribute('va_apellantstate').setValue(null);
                                    _formContext.getAttribute('va_appellantstateoptionset').setValue(null);
                                    SetOptionSetValue('udo_appellantcountrylist', _formContext.getAttribute('va_mailingcountrylist').getText());
                                    _formContext.getAttribute('udo_appellantcountrycode').setValue(_formContext.getAttribute('va_mailingcountry').getValue());

                                    break;
                                case _addressTypes.Overseas:

                                    var city = _formContext.getAttribute('va_mailingmilitarypostofficetypecode').getText();
                                    if (city !== null && city !== '') _formContext.getAttribute('va_apellantcity').setValue(city);

                                    var state = _formContext.getAttribute('va_mailingmilitarypostaltypecode').getText();
                                    if (state !== null && state !== '') {
                                        _formContext.getAttribute('va_apellantstate').setValue(state);
                                        _formContext.getAttribute('va_appellantstateoptionset').setValue(GetOptionValue('va_appellantstateoptionset', state));
                                    }
                                    _formContext.getAttribute('va_apellantzipcode').setValue(_formContext.getAttribute("va_milzipcodelookupid").getValue()[0].name);
                                    break;
                            }

                            _formContext.getAttribute('va_apellanthomephone').setValue(formContext.getAttribute('va_caddphone1').getValue());
                            _formContext.getAttribute('va_apellantworkphone').setValue(formContext.getAttribute('va_caddphone2').getValue());
                            _formContext.getAttribute('va_appellantaddresschanged').setValue(1);

                            _formContext.getControl("va_apellantaddress1").setFocus();
                        }
                    }

                    return resolve();
                },
                function (error) {
                    return reject();

                });
    });

}

function va_mailingcountryChange(executionContext) {
    var countryName = _formContext.getAttribute('va_mailingcountry').getValue();

    var PostOfficeTypeCode = _formContext.getAttribute('va_mailingmilitarypostofficetypecode').getValue();
    var PostalTypeCode = _formContext.getAttribute('va_mailingmilitarypostaltypecode').getValue();

    if ((PostOfficeTypeCode !== null && PostOfficeTypeCode !== '') && (PostalTypeCode !== null && PostalTypeCode !== '')) {
        // OVERSEAS MILITARY, country is blank
        _formContext.getAttribute('va_addresstype').setValue(953850002);
    }
    else if (countryName === 'US' || countryName === 'USA' || countryName === 'U.S.A.' || countryName === 'UNITED STATES' || countryName === 'UNITED STATES OF AMERICA') {
        // DOMESTIC, country is USA
        _formContext.getAttribute('va_addresstype').setValue(953850000);
    }
    else {
        // INTERNATIONAL, country is not blank and not USA
        _formContext.getAttribute('va_addresstype').setValue(953850001);
    }

    // Change required fields
    SetMandatoryFields(executionContext);
}

function SetPaymentMandatoryFields(addressType) {
    switch (addressType) {
        case _addressTypes.Domestic: // Domestic
            _formContext.getControl('va_paymentcity').setVisible(true);
            _formContext.getControl('va_paymentstateoptionset').setVisible(true);
            _formContext.getControl('va_paymentzipcode').setVisible(true);

            _formContext.getControl('va_paymentmilitarypostaltypecode').setVisible(false);
            _formContext.getAttribute("va_paymentmilitarypostaltypecode").setValue(null);

            _formContext.getControl('va_paymentmilitarypostofficetypecode').setVisible(false);
            _formContext.getAttribute("va_paymentmilitarypostofficetypecode").setValue(null);

            _formContext.getControl('va_apellantzipcode').setVisible(true);
            _formContext.getControl('udo_appellantcountrylist').setVisible(false);

            break;

        case _addressTypes.International: // International 
            _formContext.getControl('va_paymentcity').setVisible(true);

            _formContext.getControl('va_paymentstate').setVisible(false);
            _formContext.getAttribute("va_paymentstate").setValue(null);

            _formContext.getControl('va_paymentzipcode').setVisible(false);
            _formContext.getAttribute("va_paymentzipcode").setValue(null);

            _formContext.getControl('va_paymentstateoptionset').setVisible(false);
            _formContext.getAttribute("va_paymentstateoptionset").setValue(null);

            _formContext.getAttribute("va_paymentmilitarypostofficetypecode").setValue(null);
            _formContext.getAttribute("va_paymentmilitarypostaltypecode").setValue(null);

            _formContext.getControl('va_paymentcountrylist').setVisible(true);
            _formContext.getAttribute("va_paymentcountry").setValue(null);

            _formContext.getControl('udo_appellantcountrylist').setVisible(true);
            _formContext.getAttribute("va_apellantcountry").setValue(null);
            _formContext.getAttribute("udo_appellantcountrycode").setValue(null);

            _formContext.getControl('va_apellantzipcode').setVisible(false);

            break;

        case _addressTypes.Overseas: // Overseas Military
            _formContext.getControl('va_paymentcity').setVisible(false);
            _formContext.getControl('va_paymentstateoptionset').setVisible(false);
            _formContext.getControl('va_paymentzipcode').setVisible(true);

            _formContext.getControl('va_paymentmilitarypostofficetypecode').setVisible(true);
            _formContext.getControl('va_paymentmilitarypostaltypecode').setVisible(true);

            _formContext.getControl('va_apellantzipcode').setVisible(true);
            _formContext.getControl('udo_appellantcountrylist').setVisible(false);

            break;

        default:  // Null or not selected
            break;
    }
}

function SetMailingMandatoryFields(addressType) {
    switch (addressType) {
        case _addressTypes.Domestic: // Domestic
            _formContext.getAttribute("va_mailingaddress1").setRequiredLevel("required");

            _formContext.getControl('va_mailingcity').setVisible(true);
            _formContext.getAttribute("va_mailingcity").setRequiredLevel("required");

            _formContext.getAttribute("va_mailingstate").setRequiredLevel("required");

            _formContext.getControl("va_mailingstateoptionset").setVisible(true);
            _formContext.getAttribute("va_mailingstateoptionset").setRequiredLevel("required");

            _formContext.getControl('va_mailingaddresszipcode').setVisible(true);
            _formContext.getAttribute("va_mailingaddresszipcode").setRequiredLevel("required");

            _formContext.getControl('va_mailingmilitarypostofficetypecode').setVisible(false);
            _formContext.getAttribute("va_mailingmilitarypostofficetypecode").setRequiredLevel("none");
            _formContext.getAttribute("va_mailingmilitarypostofficetypecode").setValue(null);

            _formContext.getControl('va_mailingmilitarypostaltypecode').setVisible(false);
            _formContext.getAttribute("va_mailingmilitarypostaltypecode").setRequiredLevel("none");
            _formContext.getAttribute("va_mailingmilitarypostaltypecode").setValue(null);

            _formContext.getControl('va_milzipcodelookupid').setVisible(false);
            _formContext.getAttribute("va_milzipcodelookupid").setRequiredLevel("none");

            _formContext.getControl('va_mailingcountrylist').setVisible(false);
            _formContext.getAttribute("va_mailingcountrylist").setRequiredLevel("none");

            break;

        case _addressTypes.International: // International 
            _formContext.getAttribute("va_mailingaddress1").setRequiredLevel("required");

            _formContext.getControl('va_mailingcity').setVisible(true);
            _formContext.getAttribute("va_mailingcity").setRequiredLevel("required");

            _formContext.getAttribute("va_mailingstate").setRequiredLevel("none");
            _formContext.getAttribute("va_mailingstate").setValue(null);

            _formContext.getControl("va_mailingstateoptionset").setVisible(false);
            _formContext.getAttribute("va_mailingstateoptionset").setRequiredLevel("none");
            _formContext.getAttribute("va_mailingstateoptionset").setValue(null);

            _formContext.getControl('va_mailingaddresszipcode').setVisible(false);
            _formContext.getAttribute("va_mailingaddresszipcode").setRequiredLevel("none");
            _formContext.getAttribute("va_mailingaddresszipcode").setValue(null);

            _formContext.getControl('va_mailingmilitarypostofficetypecode').setVisible(false);
            _formContext.getAttribute("va_mailingmilitarypostofficetypecode").setRequiredLevel("none");
            _formContext.getAttribute("va_mailingmilitarypostofficetypecode").setValue(null);

            _formContext.getControl('va_mailingmilitarypostaltypecode').setVisible(false);
            _formContext.getAttribute("va_mailingmilitarypostaltypecode").setRequiredLevel("none");
            _formContext.getAttribute("va_mailingmilitarypostaltypecode").setValue(null);

            _formContext.getControl('va_milzipcodelookupid').setVisible(false);
            _formContext.getAttribute("va_milzipcodelookupid").setRequiredLevel("none");

            _formContext.getControl('va_mailingcountrylist').setVisible(true);
            _formContext.getAttribute("va_mailingcountrylist").setRequiredLevel("required");

            break;

        case _addressTypes.Overseas: // Overseas Military
            _formContext.getAttribute("va_mailingaddress1").setRequiredLevel("required");

            _formContext.getControl('va_mailingcity').setVisible(false);
            _formContext.getAttribute("va_mailingcity").setRequiredLevel("none");
            _formContext.getAttribute("va_mailingcity").setValue(null);

            _formContext.getAttribute("va_mailingstate").setRequiredLevel("none");
            _formContext.getAttribute("va_mailingstate").setValue(null);

            _formContext.getControl("va_mailingstateoptionset").setVisible(false);
            _formContext.getAttribute("va_mailingstateoptionset").setRequiredLevel("none");
            _formContext.getAttribute("va_mailingstateoptionset").setValue(null);

            _formContext.getControl('va_mailingaddresszipcode').setVisible(false);
            _formContext.getAttribute("va_mailingaddresszipcode").setRequiredLevel("none");

            _formContext.getControl('va_mailingmilitarypostofficetypecode').setVisible(true);
            _formContext.getAttribute("va_mailingmilitarypostofficetypecode").setRequiredLevel("required");

            _formContext.getControl('va_mailingmilitarypostaltypecode').setVisible(true);
            _formContext.getAttribute("va_mailingmilitarypostaltypecode").setRequiredLevel("required");

            _formContext.getControl('va_milzipcodelookupid').setVisible(true);
            _formContext.getAttribute("va_milzipcodelookupid").setRequiredLevel("required");

            _formContext.getControl('va_mailingcountrylist').setVisible(false);
            _formContext.getAttribute("va_mailingcountrylist").setRequiredLevel("none");
            _formContext.getAttribute("va_mailingcountrylist").setValue(null);
            _formContext.getAttribute("va_mailingcountry").setValue(null);

            break;
    }
}

function SetMandatoryFields(executionContext) {
    var optionValue = _formContext.getAttribute("va_addresstype").getValue();
    SetMailingMandatoryFields(optionValue);
    SetPaymentMandatoryFields(optionValue);
}

function GetAddressArray(type) {
    var GeneralValidation = {
        va_mailingaddress1: '20',
        va_mailingaddress2: '20',
        va_mailingaddress3: '20',
        va_mailingcity: '20',
        va_mailingaddresszipcode: '5'
    };

    var ChildValidation = {
        va_mailingaddress1: '35',
        va_mailingaddress2: '35',
        va_mailingaddress3: '35',
        va_mailingcity: '30',
        va_mailingaddresszipcode: '5'
    };

    if (type) {
        return ChildValidation;
    }
    else {
        return GeneralValidation;
    }
}

function TestForAllowedChars(text, nums) {
    var chars = {
        1: "[~]+",
        2: "[`]+",
        3: "[!]+",
        4: "[@]+",
        5: "[#]+",
        6: "[$]+",
        7: "[%]+",
        8: "[\\^]+",
        9: "[&]+",
        10: "[*]+",
        11: "[(]+",
        12: "[)]+",
        13: "[_]+",
        14: "[-]+",
        15: "[+]+",
        16: "[=]+",
        17: "[|]+",
        18: "[\\\\]+",
        19: "[}]+",
        20: "[]]+",
        21: "[{]+",
        22: "[[]+",
        23: "[']+",
        24: "[\"]+",
        25: "[:]+",
        26: "[;]+",
        27: "[\\/]+",
        28: "[?]+",
        29: "[.]+",
        30: "[>]+",
        31: "[,]+",
        32: "[<]+",
        33: "[  ]{2,}"
    };

    for (var remove in nums) {
        chars[nums[remove]] = null;
    }

    var success;
    for (var invalid in chars) {
        if (chars[invalid] !== null) {
            var myregex = new RegExp(chars[invalid]);
            success = !myregex.test(text);
        }
        if (success === false) break;
    }

    return success;
}

function MailingState_Onchange() {
    var option = _formContext.getAttribute("va_mailingstateoptionset").getSelectedOption();
    var val = '';
    if (option && option !== undefined) {
        val = option.text;
    }

    _formContext.getAttribute("va_mailingstate").setValue(val);
}

function MailingCountryList_Onchange(executionContext) {
    var option = _formContext.getAttribute("va_mailingcountrylist").getSelectedOption();
    var txt = '';
    var val = '';

    if (option && option !== undefined) {
        txt = option.text;
        val = option.value;
    }

    _formContext.getAttribute("va_mailingcountry").setValue(txt);
    _formContext.getAttribute("va_mailingforeignpostalcode").setValue(val.toString());
}

function AppellantState_Onchange() {
    var option = _formContext.getAttribute("va_appellantstateoptionset").getSelectedOption();

    if (option !== undefined && option) {
        _formContext.getAttribute("va_apellantstate").setValue(option.text);
    }
    else {
        _formContext.getAttribute("va_apellantstate").setValue(null);
    }
}

function PaymentState_Onchange() {
    var option = _formContext.getAttribute("va_paymentstateoptionset").getSelectedOption();

    if (option !== undefined && option) {
        _formContext.getAttribute("va_paymentstate").setValue(option.text);
    }
    else {
        _formContext.getAttribute("va_paymentstate").setValue(null);
    }
}

function PaymentCountryList_Onchange() {
    var option = _formContext.getAttribute("va_paymentcountrylist").getSelectedOption();
    var val = '';

    if (option && option !== undefined) {
        val = option.value;
    }

    _formContext.getAttribute("va_paymentcountry").setValue(val);
    _formContext.getAttribute("va_paymentforeignpostalcode").setValue(val.toString());
}

function AppellantCountryList_Onchange() {
    var option = _formContext.getAttribute("udo_appellantcountrylist").getSelectedOption();
    var val = '';

    if (option && option !== undefined) {
        val = option.text;
    }

    _formContext.getAttribute("va_apellantcountry").setValue(val);
}

function SetAddlOptionSetFields() {
    // This takes into account extra option set fields which are set from other values.

    // Set payment state option set
    var Statep = _formContext.getAttribute("va_paymentstate").getValue();
    if (Statep && Statep.length > 0) { _formContext.getAttribute('va_paymentstateoptionset').setValue(GetOptionValue('va_paymentstateoptionset', Statep)); }

    var PayPostOfficeTypeCode = _formContext.getAttribute("udo_paypostofficetypecode").getValue();
    var PayPostalTypeCode = _formContext.getAttribute("udo_paypostaltypecode").getValue();

    if (PayPostOfficeTypeCode) { SetOptionSetFromValue('va_paymentmilitarypostofficetypecode', PayPostOfficeTypeCode); }
    if (PayPostalTypeCode) { SetOptionSetFromValue('va_paymentmilitarypostaltypecode', PayPostalTypeCode); }

    var PayMilZipCode = _formContext.getAttribute("udo_paymilzip").getValue();
    if (PayMilZipCode !== null && PayMilZipCode !== '') {
        _formContext.getAttribute("va_paymentzipcode").setValue(PayMilZipCode);
    }

    // Mailing Optionset Update
    var PostOfficeTypeCode = _formContext.getAttribute("udo_postalofficetypecode").getValue();
    var PostalTypeCode = _formContext.getAttribute("udo_postaltypecode").getValue();

    if (PostOfficeTypeCode) { SetOptionSetFromValue('va_mailingmilitarypostofficetypecode', PostOfficeTypeCode); }
    if (PostalTypeCode) { SetOptionSetFromValue('va_mailingmilitarypostaltypecode', PostalTypeCode); }

																			

					 
								
													  

																								   
								   
												
											 
											 
																		   
															  
																 
																						
						
																																												   
				 
			   
	 

    // Set appellant state option set
    var appState = _formContext.getAttribute('va_apellantstate').getValue();
    _formContext.getAttribute('va_appellantstateoptionset').setValue(GetOptionValue('va_appellantstateoptionset', appState));
}

function GetOptionValue(optionsetSchema, labelText) {
    var optionList = _formContext.getAttribute(optionsetSchema).getOptions();
    var optionValue;

    for (var o in optionList) {
        if (optionList[o].text === labelText) {
            optionValue = optionList[o].value;
            break;
        }
    }
    return optionValue;
}

function SetOptionSetFromValue(controlName, optionText) {
    var options = _formContext.getAttribute(controlName).getOptions();
    if (!options || !optionText) { return; }

    optionText = optionText.toUpperCase();
    for (var k = 0; k < options.length; k++) {
        if (options[k].text.toUpperCase() === optionText) {
            _formContext.getAttribute(controlName).setValue(options[k].value);
            return;
        }
    }
}

function setChangeCheckBox(executionContext, field) {
    var checkbox = _formContext.getAttribute(field);
    if (checkbox !== 1) {
        checkbox.setValue(true);
    }
}

function fnphoneOnChange1(executionContext) {

    var Phone1 = _formContext.getAttribute('va_caddphone1').getValue();

    if (Phone1 !== null) {
        Phone1 = Phone1.replace(/[^\d]/gi, '');
    }

    var option = _formContext.getAttribute('va_addresstype').getValue();
    if (option === 953850000 && Phone1 !== null) {
        // Write back to WS fields   
        _formContext.getAttribute('va_area1').setValue(Phone1.slice(0, 3));
        _formContext.getAttribute('va_phone1').setValue(Phone1.slice(3));

        // Reformat the same fields
        _formContext.getAttribute('va_caddphone1').setValue(FormatTelephone(Phone1));
    }
    else {
        _formContext.getAttribute('va_caddphone1').setValue(Phone1);
        _formContext.getAttribute('va_area1').setValue(null);
        _formContext.getAttribute('va_phone1').setValue(Phone1);
    }
}

function fnphoneOnChange2(executionContext) {
    var Phone2 = _formContext.getAttribute('va_caddphone2').getValue();

    if (Phone2 !== null) {
        Phone2 = Phone2.replace(/[^\d]/gi, '');
    }

    var option = _formContext.getAttribute('va_addresstype').getValue();
    if (option === 953850000 && Phone2 !== null) {
        // Write back to WS fields   
        _formContext.getAttribute('va_area2').setValue(Phone2.slice(0, 3));
        _formContext.getAttribute('va_phone2').setValue(Phone2.slice(3));

        // Reformat the same fields
        _formContext.getAttribute('va_caddphone2').setValue(FormatTelephone(Phone2));
    }
    else {
        _formContext.getAttribute('va_caddphone2').setValue(Phone2);
        _formContext.getAttribute('va_area2').setValue(null);
        _formContext.getAttribute('va_phone2').setValue(Phone2);
    }
}

function FormatTelephone(telephoneNumber) {
    var Phone = telephoneNumber;
    var ext = '';
    var result;

    if (0 !== Phone.indexOf('+')) {
        if (1 < Phone.lastIndexOf('x')) {
            ext = Phone.slice(Phone.lastIndexOf('x'));
            Phone = Phone.slice(0, Phone.lastIndexOf('x'));
        }

        Phone = Phone.replace(/[^\d]/gi, '');
        result = Phone;
        if (7 === Phone.length) {
            result = Phone.slice(0, 3) + '-' + Phone.slice(3)
        }
        if (10 === Phone.length) {
            result = '(' + Phone.slice(0, 3) + ') ' + Phone.slice(3, 6) + '-' + Phone.slice(6);
        }
        if (0 < ext.length) {
            result = result + ' ' + ext;
        }
        return result;
    }
}

// ID Proofing: ToggleTabs (called from button or click of each checkbox)
function fnIDProof() {
    if (_formContext.getAttribute("va_failedidproofing").getValue() === true) {
        _formContext.getAttribute("va_currentmonthlybenefit").setValue(false);
        _formContext.getAttribute("va_addressofrecord").setValue(false);
        _formContext.getAttribute("va_dobverified").setValue(false);
        globalcadd.idproof = "failed";
    } else if ((_formContext.getAttribute("va_currentmonthlybenefit").getValue() === true) && (_formContext.getAttribute("va_addressofrecord").getValue() === true) && (_formContext.getAttribute("va_dobverified").getValue() === true)) {
        _formContext.getAttribute("va_failedidproofing").setValue(false);
        globalcadd.idproof = "passed";
    } else if ((_formContext.getAttribute("va_currentmonthlybenefit").getValue() === false) || (_formContext.getAttribute("va_addressofrecord").getValue() === false) || (_formContext.getAttribute("va_dobverified").getValue() === false)) {
        globalcadd.idproof = "";
    }

    if (globalcadd.idproof === "passed") {
        var addressKey = _formContext.getAttribute("va_appellantaddresskey").getValue();
    }
}

// This function simplifies setting the option set value.
function SetOptionSetValue(optionsetAttribute, optionText) {
    var options = _formContext.getAttribute(optionsetAttribute).getOptions();
    for (var i = 0; i < options.length; i++) {
        if (options[i].text === optionText) {
            var optionsetCtrl = _formContext.getAttribute(optionsetAttribute);
            if (optionsetCtrl !== undefined && optionsetCtrl !== null) {
                optionsetCtrl.setValue(options[i].value);
            }
        }
    }
}

function changeForm(name) {
    var newIndex = -1;
    if (_formContext.ui.formSelector.getCurrentItem().getLabel().toLowerCase() === name.toLowerCase())
        _formContext.ui.formSelector.items.forEach(function (item, index) {
            if (item.getLabel().toLowerCase() === lblShow.toLowerCase()) newIndex = index;
        });

    if (newIndex !== -1) _formContext.ui.formSelector.items.get(newIndex).navigate()
}

function proceedWithCADDClose(formContext) {
    var isDirty = formContext.data.getIsDirty();

    if (isDirty) {
        var dataXml = formContext.data.entity.getDataXml();
        console.log("Dirty fields: " + dataXml);

        var msg = "There are unsaved changes on the form.  Press OK to continue with close and discard changes on CADD.  Press Cancel to stay on CADD.";
        var title = "Unsaved Changes";
        UDO.Shared.openConfirmDialog(msg, title, "", "")
            .then(
                function (response) {
                    if (response.confirmed) {
                        window.open("http://event/?EventName=CloseCADDProcessHost");
                    }
                });
    } else {
        window.open("http://event/?EventName=CloseCADDProcessHost");
    }
}

function IsCaddFormDirty() {
    var IsDirty = _formContext.data.entity.getIsDirty();
    if (IsDirty) {
        window.open("http://uii/Global Manager/CopyToContext?CaddFormIsDirty=true");
    }
    else {
        window.open("http://uii/Global Manager/CopyToContext?CaddFormIsDirty=false");
    }
}

function CADDCheckIsDirty() {
    var isDirty = _formContext.data.entity.getIsDirty();
    return isDirty.toString();
}

function InitiateSessionCloseOut(interactionId, requestId, caddId) {
    if (caddId !== "") {
        var isDirty = _formContext.data.entity.getIsDirty();

        if (isDirty) {
            var msg = "There are unsaved changes on the CADD form.  Press Ok to continue with close and discard changes on CADD.   Press Cancel to go to CADD.";
            var title = "Discard CADD Changes";
            var confirmOptions = { height: 200, width: 450 };
            var confirmStrings = { title: title, text: msg, confirmButtonLabel: "Ok", cancelButtonLabel: "Cancel" };
            Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
                .then(function (response) {
                    if (response.confirmed) {
                        window.open("http://event/?eventname=SetSessionCloseToValid");
                        CloseSession(interactionId, requestId);
                        return;
                    } else {
                        window.open("http://event/?eventname=SetSessionCloseToInvalid");
                        window.open("http://uii/Global Manager/ShowTab?CADDProcessHost");
                        return;
                    }
                },
                    function (error) {
                        window.open("http://event/?eventname=SetSessionCloseToInvalid");
                        window.open("http://uii/Global Manager/ShowTab?CADDProcessHost");
                        return;
                    });
        } else {
            window.open("http://event/?eventname=SetSessionCloseToValid");
            CloseSession(interactionId, requestId);
        }
    }
}

function CloseSession(interactionId, requestId) {
    var endTime = new Date();
    if (endTime) {
        if (requestId !== "") {

           Xrm.webApi.UpdateRecord(requestId, "udo_request", { udo_EndTime: endTime })
                .then(function () {
                    // Nothing
                })
                .catch(function (err) {
                    Xrm.Navigation.openAlertDialog({ text: "There was an error while updating the Request before closing the Session.\n\nError: " + err.message });
                });
        }
        // Update Interaction end time
        if (interactionId !== "") {

            Xrm.webApi.UpdateRecord(interactionId, "udo_interaction", { udo_EndTime: endTime })
                .then(function () {
                    // Nothing
                })
                .catch(function (err) {
                    Xrm.Navigation.openAlertDialog({ text: "There was an error while updating the Interaction before closing the Session.\n\nError: " + err.message });
                });
        }
    }
}

function keyboardShortcutToFocusCADD() {
    window.open("http://uii/Global Manager/ShowTab?CADD");
    setTimeout('_formContext.ui.controls.get("va_verifyaddress1").setFocus()', 1000);
}

function requestToCloseCADD() {
    var msg = "There are unsaved changes on the CADD form.  Press Ok to continue with close and discard changes on CADD.   Press Cancel to go to CADD.";
    var title = "Discard CADD Changes";
    var confirmOptions = { height: 200, width: 450 };
    var confirmStrings = { title: title, text: msg, confirmButtonLabel: "Ok", cancelButtonLabel: "Cancel" };
    Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
        .then(function (response) {
            window.open("http://uii/Global Manager/CopyToContext?CADDSessionClose=" + response.confirmed.toString());

            window.open("http://event/?EventName=SetSessionCloseValue");
            //window.open("http://event/?EventName=SetSessionCloseValue&PromptValue=" + response.confirmed.toString());
            return response.confirmed.toString();
        },
        function (error) {
            window.open("http://uii/Global Manager/CopyToContext?CADDSessionClose=false");
            window.open("http://event/?EventName=SetSessionCloseValue");
            //window.open("http://event/?EventName=SetSessionCloseValue&PromptValue=false");
            return "False";
        });
}

function SessionCloseCADDValidation(caddId) {
    if (caddId !== "") {
        var isDirty = _formContext.data.entity.getIsDirty();

        if (isDirty) {
            window.open("http://uii/Global Manager/ShowTab?CADDProcessHost");

            var msg = "There are unsaved changes on the CADD form.  Press Ok to continue with close and discard changes on CADD.   Press Cancel to go to CADD.";
            var title = "Discard CADD Changes";
            var confirmOptions = { height: 200, width: 450 };
            var confirmStrings = { title: title, text: msg, confirmButtonLabel: "Ok", cancelButtonLabel: "Cancel" };
            Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
                .then(
                    function (response) {
                        if (response.confirmed) {
                            window.open("http://uii/Session Controller/IncrementCloseSessionValidations");
                        }
                    },
                    function (error) {
                        window.open("http://uii/Session Controller/IncrementCloseSessionFailures");
                        window.open("http://uii/Global Manager/ShowTab?CADDProcessHost");
                        return;
                    });
        } else {
            // PASS:
            window.open("http://uii/Session Controller/IncrementCloseSessionValidations");
        }
    } else {
        // PASS:
        window.open("http://uii/Session Controller/IncrementCloseSessionValidations");
    }
}

