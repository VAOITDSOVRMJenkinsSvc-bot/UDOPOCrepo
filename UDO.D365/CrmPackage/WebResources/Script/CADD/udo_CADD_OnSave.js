"use strict";

var addressValidated = false;
var _executionContext = null;
var _UserSettings = null;
var _isLastUpdate = false;

function retrieveFormContext(executionContext) {
    if (executionContext && executionContext.getFormContext !== undefined) {
        return executionContext.getFormContext();
    }
    else {
        return null;
    }
}

function onSave_USD(formContext) {
    formContext.data.save();
}

function OnSave(executionContext) {
    _executionContext = executionContext;
    var formContext = retrieveFormContext(executionContext);

    if (executionContext.getEventArgs().getSaveMode() === 70) {
        executionContext.getEventArgs().preventDefault(); // Disabling auto save
        return false;
    }

    // Add code for CADD plugin to fire for Find Bank
    var CADDStatus = formContext.getAttribute("udo_caddstatus").getSelectedOption().text;
    console.log("udo_CADD_OnSave Started / Status - " + CADDStatus);

    if (CADDStatus === "Find Bank") {
        console.log("udo_CADD_OnSave status - Find Bank");
        return true;
    }

    if (CADDStatus === "Complete") {
        console.log("udo_CADD_OnSave status - Complete");

        formContext.ui.tabs.get('tabCADDMaintenance').sections.get('sectionMailingPaymentAppellantAddresses').setVisible(true);
        formContext.ui.tabs.get('tabCADDMaintenance').sections.get('sectionEmailPhoneNumbers').setVisible(true);
        formContext.ui.tabs.get('tabCADDMaintenance').sections.get('sectionDepositAccount').setVisible(true);
        formContext.ui.tabs.get('tabCADDMaintenance').sections.get('sectionDepositAccount').setVisible(true);
        formContext.ui.tabs.get('tabExecutionResults').setVisible(true);
        formContext.ui.tabs.get('tabExecutionResults').sections.get('sectionExecutionResults').setVisible(true);

        if (_isLastUpdate) {
            return true;
        }

        Xrm.Utility.closeProgressIndicator();
        var msg = "Record cannot be modified after it is complete.\nTo change the address again, open new Change of Address form.\n\nThis record cannot be modified.";
        var title = "Already ID Proof Complete";
        UDO.Shared.openAlertDialog(msg, title, 200, 450).then(
            function (response) {
                _executionContext.getEventArgs().preventDefault();
                return true;
            },
            function (error) {
                return false;
            });

        _executionContext.getEventArgs().preventDefault();
        return true;
    }

    if ((globalcadd.idproof === "") || (globalcadd.idproof === "failed")) {
        console.log("udo_CADD_OnSave IdProof - Failed");

        // Mark the status to complete and return.
        SetOptionSetValue("udo_caddstatus", "Complete");
        return true;
    }

    try {
        var addressKey = formContext.getAttribute("va_appellantaddresskey").getValue();
        console.log("udo_CADD_OnSave Appellant Address Key - " + addressKey);

        _hasAppeals = (addressKey !== null && addressKey.length > 0);

        if (ValidateZipcode(_executionContext) === false) {
            _executionContext.getEventArgs().preventDefault();
            return false;
        }

        if (ValidateFields() === false) {
            _executionContext.getEventArgs().preventDefault();
            return false;
        }

        var va_depositaccountnumber = formContext.getAttribute('va_depositaccountnumber').getValue();
        var va_routingnumber = formContext.getAttribute('va_routingnumber').getValue();
        var va_crn = formContext.getAttribute('udo_crn').getValue();

        // Check mailing fields based on address type
        // SS - This is always true for UDO so this code will be executed.
        if (_changingAwardAddress) {
            var curCountry = formContext.getAttribute('va_mailingcountry').getValue();
            var curCountylist = formContext.getAttribute('va_mailingcountrylist').getValue();
            var va_mailingaddress1 = formContext.getAttribute('va_mailingaddress1').getValue();
            var va_mailingaddress2 = formContext.getAttribute('va_mailingaddress2').getValue();
            var va_mailingaddress3 = formContext.getAttribute('va_mailingaddress3').getValue();
            var va_paymentaddress1 = formContext.getAttribute('va_paymentaddress1').getValue();
            var va_paymentaddress2 = formContext.getAttribute('va_paymentaddress2').getValue();
            var va_paymentaddress3 = formContext.getAttribute('va_paymentaddress3').getValue();
            var va_mailingcity = formContext.getAttribute('va_mailingcity').getValue();
            var va_paymentcity = formContext.getAttribute('va_paymentcity').getValue();
            var va_mailingstate = formContext.getAttribute('va_mailingstate').getValue();
            var va_paymentstate = formContext.getAttribute('va_paymentstate').getValue();
            var va_mailingaddresszipcode = formContext.getAttribute('va_mailingaddresszipcode').getValue();
            var va_paymentzipcode = formContext.getAttribute('va_paymentzipcode').getValue();
            var showAlert = false;
            var alertMessage = "";

            switch (formContext.getAttribute('va_addresstype').getValue()) {
                case _addressTypes.International:
                    var va_mailingcountry = formContext.getAttribute('va_mailingcountry').getValue();
                    var va_paymentcountry = formContext.getAttribute('va_paymentcountry').getValue();
                    var va_mailingcountrylist = formContext.getAttribute('va_mailingcountrylist').getValue();

                    if (!curCountry || curCountry.length === 0) {
                        var msg = "You must provide a value for Country";
                        var title = "Missing Country";
                        UDO.Shared.openAlertDialog(msg, title, 200, 450).then(
                            function success(result) {
                                _executionContext.getEventArgs().preventDefault();
                                formContext.ui.setFormNotification(msg, "ERROR", "CURRENTCADDSTATUS");
                                return false;
                            },
                            function (error) {
                                console.log(error.message);
                            }
                        );

                        _executionContext.getEventArgs().preventDefault();
                        formContext.ui.setFormNotification(msg, "ERROR", "CURRENTCADDSTATUS");
                        return false;
                    }
                    else if (curCountry.toUpperCase() === 'USA' || curCountry.toUpperCase() === 'US' || curCountry.toUpperCase() === 'U.S.A.' ||
                        curCountry.toUpperCase() === 'UNITED STATES') {

                        msg = "For International address, Country value must be provided, and Country cannot be USA. This record cannot be saved.";
                        title = "Invalid Country";
                        UDO.Shared.openAlertDialog(msg, title, 200, 450).then(
                            function success(result) {
                                _executionContext.getEventArgs().preventDefault();
                                formContext.ui.setFormNotification(msg, "ERROR", "CURRENTCADDSTATUS");
                                return false;
                            },
                            function (error) {
                                console.log(error.message);
                            }
                        );

                        _executionContext.getEventArgs().preventDefault();
                        formContext.ui.setFormNotification(msg, "ERROR", "CURRENTCADDSTATUS");
                        return false;
                    }

                    if (isEFT() === false) {
                        if ((va_mailingaddress1 !== null && va_paymentaddress1 === null) ||
                            (va_mailingaddress1 === null && va_paymentaddress1 !== null) ||
                            (va_mailingaddress1 !== null && va_paymentaddress1 !== null && va_mailingaddress1.toUpperCase().trim() !== va_paymentaddress1.toUpperCase().trim())) {

                            formContext.getControl('va_mailingaddress1').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address1", "Payment address1");
                            showAlert = true;
                        } else if ((va_mailingaddress2 !== null && va_paymentaddress2 === null) ||
                            (va_mailingaddress2 === null && va_paymentaddress2 !== null) ||
                            (va_mailingaddress2 !== null && va_paymentaddress2 !== null && va_mailingaddress2.toUpperCase().trim() !== va_paymentaddress2.toUpperCase().trim())) {

                            formContext.getControl('va_mailingaddress2').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address2", "Payment address2");
                            showAlert = true;
                        } else if ((va_mailingaddress3 !== null && va_paymentaddress3 === null) ||
                            (va_mailingaddress3 === null && va_paymentaddress3 !== null) ||
                            (va_mailingaddress3 !== null && va_paymentaddress3 !== null && va_mailingaddress3.toUpperCase().trim() !== va_paymentaddress3.toUpperCase().trim())) {

                            formContext.getControl('va_mailingaddress3').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address3", "Payment address3");
                            showAlert = true;
                        } else if ((va_mailingcity !== null && va_paymentcity === null) ||
                            (va_mailingcity === null && va_paymentcity !== null) ||
                            (va_mailingcity !== null && va_paymentcity !== null && va_mailingcity.toUpperCase().trim() !== va_paymentcity.toUpperCase().trim())) {

                            formContext.getControl('va_mailingcity').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing city", "Payment city");
                            showAlert = true;
                        } else if ((va_mailingcountry !== null && va_paymentcountry === null) ||
                            (va_mailingcountry === null && va_paymentcountry !== null) ||
                            (va_mailingcountry !== null && va_paymentcountry !== null && va_mailingcountry.toUpperCase().trim() !== va_paymentcountry.toUpperCase().trim())) {
                            alertMessage = createMatchingAddressesError("Mailing country", "Payment country");
                            showAlert = true;
                        }
                        if (showAlert) {
                            msg = alertMessage;
                            title = "Invalid Value";
                            UDO.Shared.openAlertDialog(msg, title, 200, 450).then(
                                function success(result) {
                                    _executionContext.getEventArgs().preventDefault();
                                    formContext.ui.setFormNotification(msg, "ERROR", "CURRENTCADDSTATUS");
                                    return false;
                                },
                                function (error) {
                                    console.log(error.message);
                                }
                            );

                            _executionContext.getEventArgs().preventDefault();
                            formContext.ui.setFormNotification(msg, "ERROR", "CURRENTCADDSTATUS");
                            return false;
                        }
                    }
                    break;

                case _addressTypes.Overseas: // Overseas
                    var va_mailingmilitarypostaltypecode = formContext.getAttribute('va_mailingmilitarypostaltypecode').getText();
                    var va_mailingmilitarypostofficetypecode = formContext.getAttribute('va_mailingmilitarypostofficetypecode').getSelectedOption().text;
                    var va_paymentmilitarypostaltypecode = formContext.getAttribute('va_paymentmilitarypostaltypecode').getText();
                    var va_paymentmilitarypostofficetypecode = formContext.getAttribute('va_paymentmilitarypostofficetypecode').getSelectedOption();
                    if (va_paymentmilitarypostofficetypecode !== null) {
                        va_paymentmilitarypostofficetypecode = formContext.getAttribute('va_paymentmilitarypostofficetypecode').getSelectedOption().text;
                    }
                    var va_milzipcodelookupid = formContext.getAttribute('va_milzipcodelookupid').getValue();
                    if (va_milzipcodelookupid !== null) {
                        va_milzipcodelookupid = formContext.getAttribute('va_milzipcodelookupid').getValue()[0].name;
                    }
                    var z = formContext.getAttribute('va_mailingmilitarypostaltypecode').getText();
                    var z2 = formContext.getAttribute("va_mailingmilitarypostofficetypecode").getSelectedOption().text;
                    if (z === null || z === '' || z2 === null || z2 === '') {
                        formContext.getControl('va_mailingmilitarypostaltypecode').setFocus();

                        msg = "Overseas Military Postal Type Code and Post Office Type Code are required if Mailing address is Overseas. This record cannot be saved.";
                        title = "Invalid Value";
                        UDO.Shared.openAlertDialog(msg, title, 200, 450).then(
                            function success(result) {
                                _executionContext.getEventArgs().preventDefault();
                                formContext.ui.setFormNotification(msg, "ERROR", "CURRENTCADDSTATUS");
                                return false;
                            },
                            function (error) {
                                console.log(error.message);
                            }
                        );

                        _executionContext.getEventArgs().preventDefault();
                        formContext.ui.setFormNotification(msg, "ERROR", "CURRENTCADDSTATUS");
                        return false;
                    }

                    if (curCountylist && curCountylist.length > 0) {
                        formContext.getControl('va_mailingcountrylist').setFocus();

                        msg = "For Overseas address, Country value must be blank. This record cannot be saved.";
                        title = "Invalid Value";
                        UDO.Shared.openAlertDialog(msg, title, 200, 450).then(
                            function success(result) {
                                _executionContext.getEventArgs().preventDefault();
                                formContext.ui.setFormNotification(msg, "ERROR", "CURRENTCADDSTATUS");
                                return false;
                            },
                            function (error) {
                                console.log(error.message);
                            }
                        );

                        _executionContext.getEventArgs().preventDefault();
                        formContext.ui.setFormNotification(msg, "ERROR", "CURRENTCADDSTATUS");
                        return false;
                    }

                    var mstate = formContext.getAttribute('va_mailingstate').getValue();
                    if (mstate && mstate.length > 0) {
                        formContext.getControl('va_mailingstateoptionset').setFocus();

                        msg = "For Overseas address, State value must be blank. This record cannot be saved.";
                        title = "Invalid Value";
                        UDO.Shared.openAlertDialog(msg, title, 200, 450).then(
                            function success(result) {
                                _executionContext.getEventArgs().preventDefault();
                                formContext.ui.setFormNotification(msg, "ERROR", "CURRENTCADDSTATUS");
                                return false;
                            },
                            function (error) {
                                formContext.ui.setFormNotification(error.message, "ERROR", "CURRENTCADDSTATUS");
                                return false;
                            }
                        );
                    }

                    if (isEFT() === false) {
                        if ((va_mailingaddress1 !== null && va_paymentaddress1 === null) ||
                            (va_mailingaddress1 === null && va_paymentaddress1 !== null) ||
                            (va_mailingaddress1 !== null && va_paymentaddress1 !== null && va_mailingaddress1.toUpperCase().trim() !== va_paymentaddress1.toUpperCase().trim())) {

                            formContext.getControl('va_mailingaddress1').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address1", "Payment address1");
                            showAlert = true;
                        } else if ((va_mailingaddress2 !== null && va_paymentaddress2 === null) ||
                            (va_mailingaddress2 === null && va_paymentaddress2 !== null) ||
                            (va_mailingaddress2 !== null && va_paymentaddress2 !== null && va_mailingaddress2.toUpperCase().trim() !== va_paymentaddress2.toUpperCase().trim())) {

                            formContext.getControl('va_mailingaddress2').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address2", "Payment address2");
                            showAlert = true;
                        } else if ((va_mailingaddress3 !== null && va_paymentaddress3 === null) ||
                            (va_mailingaddress3 === null && va_paymentaddress3 !== null) ||
                            (va_mailingaddress3 !== null && va_paymentaddress3 !== null && va_mailingaddress3.toUpperCase().trim() !== va_paymentaddress3.toUpperCase().trim())) {

                            formContext.getControl('va_mailingaddress3').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address3", "Payment address3");
                            showAlert = true;
                        } else if ((va_mailingmilitarypostaltypecode !== null && va_paymentmilitarypostaltypecode === null) ||
                            (va_mailingmilitarypostaltypecode === null && va_paymentmilitarypostaltypecode !== null) ||
                            (va_mailingmilitarypostaltypecode !== null && va_paymentmilitarypostaltypecode !== null && va_mailingmilitarypostaltypecode !== va_paymentmilitarypostaltypecode)) {

                            formContext.getControl('va_mailingmilitarypostaltypecode').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing overseas military postal type code", "Payment overseas military postal type code");
                            showAlert = true;
                        } else if ((va_mailingmilitarypostofficetypecode !== null && va_paymentmilitarypostofficetypecode === null) ||
                            (va_mailingmilitarypostofficetypecode === null && va_paymentmilitarypostofficetypecode !== null) ||
                            (va_mailingmilitarypostofficetypecode !== null && va_paymentmilitarypostofficetypecode !== null && va_mailingmilitarypostofficetypecode !== va_paymentmilitarypostofficetypecode)) {

                            formContext.getControl('va_mailingmilitarypostofficetypecode').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing overseas military post office type code", "Payment overseas military post office type code");
                            showAlert = true;
                        } else if ((va_milzipcodelookupid !== null && va_paymentzipcode === null) ||
                            (va_milzipcodelookupid === null && va_paymentzipcode !== null) ||
                            (va_milzipcodelookupid !== null && va_paymentzipcode !== null && va_milzipcodelookupid.toUpperCase().trim() !== va_paymentzipcode.toUpperCase().trim())) {

                            formContext.getControl('va_milzipcodelookupid').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing mil. zip code lookup", "Payment zip code");
                            showAlert = true;
                        }
                        if (showAlert) {
                            msg = alertMessage;
                            title = "Invalid Value";
                            UDO.Shared.openAlertDialog(msg, title, 200, 450).then(
                                function success(result) {
                                    _executionContext.getEventArgs().preventDefault();
                                    formContext.ui.setFormNotification(msg, "ERROR", "CURRENTCADDSTATUS");
                                    return false;
                                },
                                function (error) {
                                    console.log(error.message);
                                }
                            );

                            _executionContext.getEventArgs().preventDefault();
                            formContext.ui.setFormNotification(msg, "ERROR", "CURRENTCADDSTATUS");
                            return false;
                        }
                    }
                    break;

                case _addressTypes.Domestic: // Domestic
                    if (!curCountry || curCountry.length === 0 || curCountry.toUpperCase() !== 'USA') {
                        formContext.getAttribute('va_mailingcountry').setValue('USA');
                    }
                    if (isEFT() === false) {
                        if ((va_mailingaddress1 !== null && va_paymentaddress1 === null) ||
                            (va_mailingaddress1 === null && va_paymentaddress1 !== null) ||
                            (va_mailingaddress1 !== null && va_paymentaddress1 !== null && va_mailingaddress1.toUpperCase().trim() !== va_paymentaddress1.toUpperCase().trim())) {

                            formContext.getControl('va_mailingaddress1').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address1", "Payment address1");
                            showAlert = true;
                        } else if ((va_mailingaddress2 !== null && va_paymentaddress2 === null) ||
                            (va_mailingaddress2 === null && va_paymentaddress2 !== null) ||
                            (va_mailingaddress2 !== null && va_paymentaddress2 !== null && va_mailingaddress2.toUpperCase().trim() !== va_paymentaddress2.toUpperCase().trim())) {

                            formContext.getControl('va_mailingaddress2').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address2", "Payment address2");
                            showAlert = true;
                        } else if ((va_mailingaddress3 !== null && va_paymentaddress3 === null) ||
                            (va_mailingaddress3 === null && va_paymentaddress3 !== null) ||
                            (va_mailingaddress3 !== null && va_paymentaddress3 !== null && va_mailingaddress3.toUpperCase().trim() !== va_paymentaddress3.toUpperCase().trim())) {

                            formContext.getControl('va_mailingaddress3').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address3", "Payment address3");
                            showAlert = true;
                        } else if ((va_mailingcity !== null && va_paymentcity === null) ||
                            (va_mailingcity === null && va_paymentcity !== null) ||
                            (va_mailingcity !== null && va_paymentcity !== null && va_mailingcity.toUpperCase().trim() !== va_paymentcity.toUpperCase().trim())) {

                            formContext.getControl('va_mailingcity').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing city", "Payment city");
                            showAlert = true;
                        } else if ((va_mailingstate !== null && va_paymentstate === null) ||
                            (va_mailingstate === null && va_paymentstate !== null) ||
                            (va_mailingstate !== null && va_paymentstate !== null && va_mailingstate.toUpperCase().trim() !== va_paymentstate.toUpperCase().trim())) {

                            formContext.getControl('va_mailingstateoptionset').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing state", "Payment state");
                            showAlert = true;
                        } else if ((va_mailingaddresszipcode !== null && va_paymentzipcode === null) ||
                            (va_mailingaddresszipcode === null && va_paymentzipcode !== null) ||
                            (va_mailingaddresszipcode !== null && va_paymentzipcode !== null && va_mailingaddresszipcode.toUpperCase().trim() !== va_paymentzipcode.toUpperCase().trim())) {

                            formContext.getControl('va_mailingaddresszipcode').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing zip code", "Payment zip code");
                            showAlert = true;
                        }
                        if (showAlert) {
                            msg = alertMessage;
                            title = "Invalid Value";
                            UDO.Shared.openAlertDialog(msg, title, 200, 450).then(
                                function success(result) {
                                    _executionContext.getEventArgs().preventDefault();
                                    formContext.ui.setFormNotification(msg, "ERROR", "CURRENTCADDSTATUS");
                                    return false;
                                },
                                function (error) {
                                    console.log(error.message);
                                }
                            );

                            _executionContext.getEventArgs().preventDefault();
                            formContext.ui.setFormNotification(msg, "ERROR", "CURRENTCADDSTATUS");
                            return false;
                        }
                    }
                    break;
                default:
                    break;
            }

            if (isEFT() === true) {
                va_paymentaddress1 = "";
                va_paymentaddress2 = "";
                va_paymentaddress3 = "";
                va_paymentcity = "";
                va_paymentstate = "";
                va_paymentzipcode = "";
                va_paymentcountry = "";
                va_paymentmilitarypostaltypecode = "";
                va_paymentmilitarypostofficetypecode = "";
            }
        }

        if (!addressValidated) {
            var msg = 'Please confirm that you would like to update ';
            if (_changingAwardAddress) msg += 'address/bank account';

            if (_hasAppeals) msg += (_changingAwardAddress ? '/' : '') + 'appeals';
            msg += ' information.';

            var title = "Confirm Address Update";
            var confirmOptions = { height: 200, width: 450 };
            var confirmStrings = { title: title, text: msg, confirmButtonLabel: "Yes", cancelButtonLabel: "No" };

            Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
                .then(
                    function (response) {
                        if (response.confirmed) {
                            var va_addresstype = formContext.getAttribute('va_addresstype').getValue();

                            // Only validate address if Address Type is equal to Domestic. This may change in future, but for now CADD only does Domestic
                            if (va_addresstype === 953850000) {
                                addressValidation(executionContext);

                                if (processCADD(executionContext)) {
                                    _isLastUpdate = true;

                                    return true;
                                }
                            }
                            else {
                                addressValidated = true;
                                if (processCADD(executionContext)) {
                                    return true;
                                }
                                else {
                                    return false;
                                }
                            }
                        } else {
                            _executionContext.getEventArgs().preventDefault();
                            return false;
                        }
                    },
                    function (error) {
                        return false;
                    });
        }
        else {
            if (processCADD(executionContext)) {
                _isLastUpdate = true;

                return true;
            } else {
                var currentStatusText = formContext.getAttribute("udo_caddstatus").getSelectedOption().text
                formContext.ui.setFormNotification("Current CADD Status: " + currentStatusText, "ERROR", "CURRENTCADDSTATUS");
                _executionContext.getEventArgs().preventDefault();
                return false;
            }
        }
    }
    catch (err) {
        currentStatusText = formContext.getAttribute("udo_caddstatus").getSelectedOption().text
        formContext.ui.setFormNotification("Current CADD Status: " + currentStatusText + " Error: " + err.message, "ERROR", "CURRENTCADDSTATUS");

        console.log("Error caught during OnSave event: " + err.message);

        formContext.getAttribute('va_generalchanged').setValue(false);
        formContext.getAttribute('va_depositaccountchanged').setValue(false);
        formContext.getAttribute('va_mailingaddresschanged').setValue(false);
        formContext.getAttribute('va_paymentaddresschanged').setValue(false);
        formContext.getAttribute('va_phonenumberschanged').setValue(false);
        formContext.getAttribute('va_appellantaddresschanged').setValue(false);

        _executionContext.getEventArgs().preventDefault();

        return false;
    }
}

function processCADD(executionContext) {
    var formContext = retrieveFormContext(executionContext);
    var va_milzipcodelookupid = formContext.getAttribute("va_milzipcodelookupid").getValue();

    var city = null;
    var state = null;
    var postalCode = null;
    var country = null;

    Xrm.Utility.showProgressIndicator("Processing CADD... Please wait");

    formContext.ui.clearFormNotification("CURRENTCADDSTATUS");
    formContext.ui.clearFormNotification("UPDATEAPPELLANTADDRESSERROR");
    formContext.ui.clearFormNotification("CADDERROR");

    var optionValue = formContext.getAttribute("va_addresstype").getValue();

    switch (optionValue) {
        case _addressTypes.Domestic:
            city = formContext.getAttribute('va_mailingcity').getValue();
            state = formContext.getAttribute('va_mailingstate').getValue();
            postalCode = formContext.getAttribute('va_mailingaddresszipcode').getValue();

            break;
        case _addressTypes.International:
            country = formContext.getAttribute('va_mailingcountry').getValue();
            postalCode = formContext.getAttribute('va_mailingforeignpostalcode').getValue();
            formContext.getAttribute('va_paymentforeignpostalcode').setValue(formContext.getAttribute('va_mailingforeignpostalcode').getValue());

            break;
        case _addressTypes.Overseas:
            if (va_milzipcodelookupid !== null) {
                postalCode = formContext.getAttribute("va_milzipcodelookupid").getValue()[0].name;
            }
            break;
    }

    var params = [
        { name: 'AddressLine1', value: formContext.getAttribute('va_mailingaddress1').getValue() },
        { name: 'AddressLine2', value: formContext.getAttribute('va_mailingaddress2').getValue() },
        { name: 'AddressLine3', value: formContext.getAttribute('va_mailingaddress3').getValue() },
        { name: 'AddressLine4', value: '' },
        { name: 'City', value: city },
        { name: 'StateProvince', value: state },
        { name: 'PostalCode', value: postalCode },
        { name: 'Country', value: country }
    ];

    var updateBenefitClaimAddressCtx = new vrmContext(executionContext);
    GetUserSettingsForWebService().then(function (userData) {
        _UserSettings = userData;
        updateBenefitClaimAddressCtx.addressParameters = _UserSettings;

        if (_hasAppeals) {
            var appellantAddressCtx = new vrmContext(executionContext);
            appellantAddressCtx.user = _UserSettings;

            appellantAddressCtx.parameters['addressKey'] = formContext.getAttribute('va_appellantaddresskey').getValue();
            if (formContext.getAttribute('va_apellantssn') !== null) {
                appellantAddressCtx.parameters['fileNumber'] = formContext.getAttribute('va_apellantssn').getValue();
            }
            else {
                appellantAddressCtx.parameters['fileNumber'] = formContext.getAttribute('va_filenumber').getValue();
            }

            var updateAppellantAddressService = new updateAppellantAddress(appellantAddressCtx);
            updateAppellantAddressService.suppressProgressDlg = true;
            var aaErr = '';
            var aaRes = false;

            try {
                console.log("Calling Appellant Address update service...");
                aaRes = updateAppellantAddressService.executeRequest();
                console.log("Appellant Address update service call complete.");

                formContext.getAttribute("va_updateapellantaddressrequest").setValue(formatXml(updateAppellantAddressService.soapBodyInnerXml));

                if (aaRes) {
                    var parser = new DOMParser();
                    var updateAppellantAddress_xmlObject = parser.parseFromString(updateAppellantAddressService.responseXml, "text/xml");

                    if (updateAppellantAddress_xmlObject && updateAppellantAddress_xmlObject !== undefined
                        && updateAppellantAddress_xmlObject.xml !== '') {

                        var currentAddressKey = formContext.getAttribute('va_appellantaddresskey').getValue();

                        var updateAddressKey = null;
                        if (updateAppellantAddress_xmlObject.getElementsByTagName("AddressKey").length > 0) {
                            updateAddressKey = updateAppellantAddress_xmlObject.getElementsByTagName("AddressKey")[0].childNodes[0].nodeValue;

                            if (currentAddressKey !== updateAddressKey) {
                                var msg = "Failed to update Appellant Address info.  Middle-tier server returned following error: " + GetErrorMessages('\n');
                                formContext.ui.setFormNotification(msg, "ERROR", "UPDATEAPPELLANTADDRESSERROR");
                            }
                            else {
                                formContext.getAttribute('va_appellantaddresschanged').setValue(true);

                                // Create dev note
                                CreateNote(_executionContext, null);

                                var fName, mName, lName, modName;

                                if (updateAppellantAddress_xmlObject.getElementsByTagName("AppellantFirstName").length > 0) {
                                    fName = updateAppellantAddress_xmlObject.getElementsByTagName("AppellantFirstName")[0].childNodes[0].nodeValue;
                                }

                                if (updateAppellantAddress_xmlObject.getElementsByTagName("AppellantMiddleInitial").length > 0) {
                                    mName = updateAppellantAddress_xmlObject.getElementsByTagName("AppellantMiddleInitial")[0].childNodes[0].nodeValue;
                                }

                                if (updateAppellantAddress_xmlObject.getElementsByTagName("AppellantLastName").length > 0) {
                                    lName = updateAppellantAddress_xmlObject.getElementsByTagName("AppellantLastName")[0].childNodes[0].nodeValue;
                                }

                                if (updateAppellantAddress_xmlObject.getElementsByTagName("AppellantAddressLastModifiedDate").length > 0) {
                                    modDate = updateAppellantAddress_xmlObject.getElementsByTagName("AppellantAddressLastModifiedDate")[0].childNodes[0].nodeValue;
                                }

                                if (updateAppellantAddress_xmlObject.getElementsByTagName("AppellantAddressLastModifiedByROName").length > 0) {
                                    modName = updateAppellantAddress_xmlObject.getElementsByTagName("AppellantAddressLastModifiedByROName")[0].childNodes[0].nodeValue;
                                }

                                var fullName = (fName ? fName + ' ' : '') + (mName ? mName + '. ' : '') + (lName ? lName + ' ' : '');
                                var msg = 'Update Appellant Address completed for ' + fullName + 'on '
                                    + new Date().format("MM/dd/yyyy").toString();

                                formContext.getAttribute('va_requeststatus').setValue(msg);
                                if (typeof modDate !== 'undefined' && modDate) {
                                    formContext.getAttribute('va_apellantmodifiedon').setValue(modDate);
                                }
                                formContext.getAttribute('va_apellantmodifiedby').setValue(modName);
                            }
                        }
                    }
                } else {
                    var msg = "Failed to update Appellant Address info.  Middle-tier server returned following error: " + GetErrorMessages('\n');
                    formContext.ui.setFormNotification(msg, "ERROR", "UPDATEAPPELLANTADDRESSERROR");
                }
            }
            catch (err) {
                var msg = 'Award Address/Bank info has been updated but Appellant Address Update Service had failed to update Appellant Address. Make sure that the service is reachable.\n' + err.message;
                formContext.ui.setFormNotification(msg, "ERROR", "UPDATEAPPELLANTADDRESSERROR");
            }
        }

        if (_changingAwardAddress) {
            updateBenefitClaimAddressCtx.user = _UserSettings;
            var updateBenefitClaimAddressDetail = new updateBenefitClaimAddress(updateBenefitClaimAddressCtx);
            updateBenefitClaimAddressDetail.suppressProgressDlg = true;

            var updateBenefitClaimResponse = false;
            console.log("Calling Benefit Claim Address update service...");
            updateBenefitClaimResponse = updateBenefitClaimAddressDetail.executeRequest();
            console.log("Benefit Claim Address update service call complete.");

            formContext.getAttribute('va_updateaddressrequest').setValue(formatXml(updateBenefitClaimAddressDetail.soapBodyInnerXml));

            if (updateBenefitClaimResponse) {
                var $xml = $($.parseXML(updateBenefitClaimAddressDetail.soapBodyInnerXml.replace('ser:', '').replace('/ser:', '/')));
                $xml.find('eftAccountNumber').text('**********');
                $xml.find('eftRoutingNumber').text('**********');

                var updateAddresses_xmlObject = null;
                var xmlString = undefined;
                xmlString = (new XMLSerializer()).serializeToString($xml[0]);

                formContext.getAttribute('va_updateaddressrequest').setValue(xmlString);

                if (!updateBenefitClaimAddressDetail.wsMessage.errorFlag) {
                    formContext.getAttribute('va_mailingaddresschanged').setValue(true);

                    // Create dev note
                    CreateNote(_executionContext, null);

                    if (updateBenefitClaimAddressDetail.responseXml !== null) {
                        formContext.getAttribute('va_updateaddressresponse').setValue(formatXml(updateBenefitClaimAddressDetail.responseXml));
                    }

                    var parser = new DOMParser();
                    updateAddresses_xmlObject = parser.parseFromString(updateBenefitClaimAddressDetail.responseXml, "text/xml");

                    if (updateAddresses_xmlObject) {
                        try {
                            var returnMessage = updateAddresses_xmlObject.getElementsByTagName("returnMessage")[0].innerHTML;
                        }
                        catch (e) {
                            try {
                                var returnMessage = updateAddresses_xmlObject.getElementsByTagName("message")[0].innerHTML;
                            }
                            catch (e) {
                                console.log("No return message found after updating address.");
                            }
                        }
                        console.log("Update Address Stats - " + returnMessage);
                        formContext.getAttribute('va_requeststatus').setValue(returnMessage);
                    }

                    SectionsUpdated(_executionContext);

                    formContext.ui.tabs.get('tabExecutionResults').setVisible(true);
                    formContext.ui.tabs.get('tabExecutionResults').sections.get('sectionExecutionResults').setVisible(true);
                    SetOptionSetValue("udo_caddstatus", "Complete");

                    if (window.IsUSD) {
                        window.open("http://event/?eventName=CallVeteranSnapShotRefresh");
                    }

                    var currentStatusText = formContext.getAttribute("udo_caddstatus").getSelectedOption().text;
                    formContext.ui.setFormNotification("Current CADD Status: " + currentStatusText, "INFO", "CURRENTCADDSTATUS");

                    _isLastUpdate = true;
                } else {
                    console.log("Update Address Error - " + updateBenefitClaimAddressDetail.wsMessage.description);
                    formContext.ui.setFormNotification("CADD Error. " + updateBenefitClaimAddressDetail.wsMessage.description, "ERROR", "CADDERROR");

                    _executionContext.getEventArgs().preventDefault();
                }
            } else {
                var msg = "Failed to update Address info.  Middle-tier server returned following error: " + GetErrorMessages('\n');
                formContext.ui.setFormNotification(msg, "ERROR", "CADDERROR");
            }
        }

        Xrm.Utility.closeProgressIndicator();
        formContext.data.save();
    });

    return true;
}

function SectionsUpdated(executionContext) {
    var formContext = retrieveFormContext(executionContext);

    var DepositAccount = formContext.getAttribute('va_depositaccountchanged').getValue();
    var Mailing = formContext.getAttribute('va_mailingaddresschanged').getValue();
    var Payment = formContext.getAttribute('va_paymentaddresschanged').getValue();

    if ((Mailing === true || Payment === true) && DepositAccount === true)
        SetOptionSetValue("udo_sectionsupdated", "Updated Both");
    else if (Mailing === true || Payment === true)
        SetOptionSetValue("udo_sectionsupdated", "Updated Address");
    else if (DepositAccount === true)
        SetOptionSetValue("udo_sectionsupdated", "Updated Account");
    else
        SetOptionSetValue("udo_sectionsupdated", "n/a");
}

function ValidateZipcode(executionContext) {
    var formContext = retrieveFormContext(executionContext);

    var va_addresstype = formContext.getAttribute('va_addresstype').getValue();
    if (va_addresstype === 953850000 || va_addresstype === 953850002) {
        var va_mailingaddresszipcode = formContext.getAttribute('va_mailingaddresszipcode').getValue();
        if (va_mailingaddresszipcode !== null && va_mailingaddresszipcode.match(/[a-zA-Z]/)) {
            var msg = "Mailing zip code field contains invalid alphabetical characters";
            var title = "Invalid Zip Code";
            UDO.Shared.openAlertDialog(msg, title, 200, 450).then(
                function (response) {
                    return false;
                },
                function (error) {
                    return false;
                });
        }

        var va_paymentzipcode = formContext.getAttribute('va_paymentzipcode').getValue();
        if (va_paymentzipcode !== null && va_paymentzipcode.match(/[a-zA-Z]/)) {
            var msg = "Payment zip code field contains invalid alphabetical characters";
            var title = "Invalid Zip Code";
            UDO.Shared.openAlertDialog(msg, title, 200, 450).then(
                function (response) {
                    return false;
                },
                function (error) {
                    return false;
                });
        }
    }

    var va_apellantzipcode = formContext.getAttribute('va_apellantzipcode').getValue();
    if (va_apellantzipcode !== null && va_apellantzipcode.match(/[a-zA-Z]/)) {
        var msg = "Appellant zip code field contains invalid alphabetical characters";
        var title = "Invalid Zip Code";
        UDO.Shared.openAlertDialog(msg, title, 200, 450).then(
            function (response) {
                return false;
            },
            function (error) {
                return false;
            });
    }
    else {
        return true;
    }
}

function GetErrorMessages(separator) {
    var msg = '';
    if (_VRMMESSAGE && _VRMMESSAGE.length > 0) {
        for (var i = 0; i < _VRMMESSAGE.length; i++) {
            if (_VRMMESSAGE[i].errorFlag) msg += separator + _VRMMESSAGE[i].description;
        }
    }
    _VRMMESSAGE = new Array();
    return msg;
}

function CreateNote(executionContext, serviceRequestId) {
    try {
        var noteCreated = false;
        var formContext = retrieveFormContext(executionContext);

        var pid = formContext.getAttribute('va_participantbeneid').getValue();
        if (!pid || pid.length === 0) pid = formContext.getAttribute('va_participantvetid').getValue();
        if (!pid || pid.length === 0) pid = formContext.getAttribute('va_participantrecipid').getValue();
        if (!pid || pid.length === 0) {
            var msg = "Note cannot be created because Participant Id is not available. You can manually create a Note on Claims tab of Phone Call screen.";
            var title = "Cannot Create Note";
            UDO.Shared.openAlertDialog(msg, title, 200, 450).then(
                function (response) {
                    return false;
                },
                function (error) {
                    return false;
                });
        }

        var updateflag = "";
        var devNoteText = "";
        var failureFlag = "";
        var devNoteVar = "";

        var dobVerified = formContext.getAttribute('va_dobverified').getValue();
        var addressVerified = formContext.getAttribute('va_addressofrecord').getValue();
        var monthlyBenefitVerified = formContext.getAttribute('va_currentmonthlybenefit').getValue();
        var failedIDProofing = formContext.getAttribute('va_failedidproofing').getValue();

        var General = formContext.getAttribute('va_generalchanged').getValue();
        var DepositAccount = formContext.getAttribute('va_depositaccountchanged').getValue();
        var Mailing = formContext.getAttribute('va_mailingaddresschanged').getValue();
        var Payment = formContext.getAttribute('va_paymentaddresschanged').getValue();
        var Phones = formContext.getAttribute('va_phonenumberschanged').getValue();
        var Appellant = formContext.getAttribute('va_appellantaddresschanged').getValue();

        var pcrName = formContext.getAttribute('ownerid').getValue()[0].name;
        var fullDateTime = new Date().format('MM/dd/yyyy hh:mm:ss tt');

        pcrName = fullnameFormat(pcrName);
        devNoteText = (pcrName ? "PCR Name:  " + pcrName + "\n" : "") + "Date:  " + fullDateTime + "\n";

        if (failedIDProofing === true) {
            failureFlag = "FAILED";
            if (dobVerified !== true) {
                failureFlag += "DOB";
            }
            else {
                failureFlag += "";
            }
            if (addressVerified !== true) {
                failureFlag += "ADD";
            }
            else {
                failureFlag += "";
            }
            if (monthlyBenefitVerified !== true) {
                failureFlag += "BENE";
            }
            else {
                failureFlag += "";
            }
        }

        switch (failureFlag) {
            case "FAILEDDOB":
                devNoteText += "ID Proofing failed.  Date of Birth unverified.\n";
                break;
            case "FAILEDADD":
                devNoteText += "ID Proofing failed.  Address of Record/Direct Deposit unverified.\n";
                break;
            case "FAILEDBENE":
                devNoteText += "ID Proofing failed.  Monthly Benefit Amount unverified.\n";
                break;
            case "FAILEDDOBADD":
                devNoteText += "ID Proofing failed.  Date of Birth and Address of Record/Direct Deposit unverified.\n";
                break;
            case "FAILEDDOBBENE":
                devNoteText += "ID Proofing failed.  Date of Birth and Monthly Benefit Amount unverified.\n";
                break;
            case "FAILEDADDBENE":
                devNoteText += "ID Proofing failed.  Address of Record/Direct Deposit and Monthly Benefit Amount unverified.\n";
                break;
            case "FAILEDDOBADDBENE":
                devNoteText += "ID Proofing failed.  Date of Birth, Address of Record/Direct Deposit and Monthly Benefit Amount unverified.\n";
                break;
            case "FAILED":
                devNoteText += "ID Proofing failed.  All verification checkboxes were checked, but 'ID Proofing failed' checkbox was also checked.\n";
                break;
        }

        if (General === true) {
            devNoteVar = "General Section. \n";
        }
        if (DepositAccount === true) {
            devNoteVar += "Deposit Account Section. \n";
        }
        if (Mailing === true) {
            devNoteVar += "Mailing Address Section. \n";
        }
        if (Payment === true) {
            devNoteVar += "Payment Address Section. \n";
        }
        if (Phones === true) {
            devNoteVar += "Phone Numbers Section. \n";
        }
        if (Appellant === true) {
            devNoteVar += "Appellant Address Section. \n";
        }
        if (DepositAccount === true) {
            updateflag = "Bank";
        }
        else {
            updateflag = "";
        }
        if (Mailing === true || General === true || Payment === true || Phones === true || Appellant === true) {
            updateflag += "CADD";
        }
        else {
            updateflag += "";
        }

        switch (updateflag) {
            case "Bank":
                devNoteText += "Direct Deposit information has been updated.  ";
                break;
            case "CADD":
                devNoteText += "Change of Address for '" + formContext.getAttribute('va_name').getValue() + "' completed. " +
                    "The following sections have been changed:  \n" +
                    devNoteVar +
                    "File #: " + formContext.getAttribute('va_filenumber').getValue() + ";\n" +
                    "Bene PID: " + (formContext.getAttribute('va_participantbeneid').getValue() ? formContext.getAttribute('va_participantbeneid').getValue() : " ") + ";\n" +
                    "Vet PID: " + (formContext.getAttribute('va_participantvetid').getValue() ? formContext.getAttribute('va_participantvetid').getValue() : " ") + ";\n" +
                    "Recip PID: " + (formContext.getAttribute('va_participantrecipid').getValue() ? formContext.getAttribute('va_participantrecipid').getValue() : " ");
                break;
            case "BankCADD":
                devNoteText += "Direct Deposit information and Change of Address for '" + formContext.getAttribute('va_name').getValue() + "' completed. \n" +
                    "The following sections have been changed:  \n" +
                    devNoteVar +
                    "File #: " + formContext.getAttribute('va_filenumber').getValue() + ";\n" +
                    "Bene PID: " + (formContext.getAttribute('va_participantbeneid').getValue() ? formContext.getAttribute('va_participantbeneid').getValue() : " ") + ";\n" +
                    "Vet PID: " + (formContext.getAttribute('va_participantvetid').getValue() ? formContext.getAttribute('va_participantvetid').getValue() : " ") + ";\n" +
                    "Recip PID: " + (formContext.getAttribute('va_participantrecipid').getValue() ? formContext.getAttribute('va_participantrecipid').getValue() : " ");
                break;
            default:
                devNoteText += "No Address information has been changed.";
                break;
        }

        var createNoteCtx = new vrmContext(executionContext);
        GetUserSettingsForWebService().then(function (userData) {
            createNoteCtx.user = userData;
            createNoteCtx.isContactNote = false;
            createNoteCtx.parameters['ptcpntId'] = pid;
            createNoteCtx.parameters['noteText'] =
                devNoteText.replace(new RegExp('<', 'g'), '&lt;').replace(new RegExp('>', 'g'), '&gt;').replace(new RegExp('&', 'g'), '&amp;').replace(new RegExp("'", 'g'), '&quot;');

            var createNoteDetail = new createNote(createNoteCtx);
            createNoteDetail.suppressProgressDlg = true;
            createNoteDetail.serviceName = 'createNote';
            createNoteDetail.wsMessage.serviceName = 'createNote';

            var res = createNoteDetail.executeRequest();
            var noteId = null;
            if (res) {
                var parser = new DOMParser();
                var createNote_xmlObject = parser.parseFromString(createNoteDetail.responseXml, "text/xml");

                if (createNote_xmlObject.getElementsByTagName("noteId").length > 0) {
                    noteId = createNote_xmlObject.getElementsByTagName("noteId")[0].childNodes[0].nodeValue;

                    noteCreated = true;
                    console.log("Note Id " + noteId + " created.  \n" + devNoteText);
                }
            }
        });
    }
    catch (error) {
        reportFailure("Error creating Note - " + error, requestName);
        noteCreated = false;
    }

    return noteCreated;
}

function isEFT() {
    var va_routingnumber = _formContext.getAttribute('va_routingnumber').getValue();

    // Read from globals instead of field
    var va_depositaccountnumber = _formContext.getAttribute('va_depositaccountnumber').getValue();
    var va_depositbegindate = new Date(_formContext.getAttribute('va_depositbegindate').getValue());
    var va_depositenddate = _formContext.getAttribute('va_depositenddate').getValue();
    var today = new Date();

    if (va_depositenddate !== null) {
        va_depositenddate = new Date(va_depositenddate);
    }

    if (va_depositbegindate === null) {
        return false;
    }

    if (va_routingnumber !== null && va_depositaccountnumber !== null && va_depositbegindate <= today && (va_depositenddate === null || va_depositenddate >= today)) {
        return true;
    }

    return false;
}

function createMatchingAddressesError(field1, field2) {
    var errorMessage = field1 + " and " + field2 + " should be the same, when Direct Deposit is not set up on this account."
    return errorMessage;
}

function addressValidation(executionContext) {
    var formContext = retrieveFormContext(executionContext);
    var calling = false;
    var addresses;
    var entity = {};

    if (formContext.getAttribute("va_mailingaddresschanged").getValue()
        && (formContext.getAttribute("va_mailingaddress1").getValue() !== null && formContext.getAttribute("va_mailingaddress1").getValue() !== "")
        && (formContext.getAttribute("va_mailingcity").getValue() !== null && formContext.getAttribute("va_mailingcity").getValue() !== "")
        && (formContext.getAttribute("va_mailingstateoptionset").getValue() !== null && formContext.getAttribute("va_mailingstateoptionset").getValue() !== "")) {
        calling = true;

        entity = {};
        entity.udo_validateaddressid = "00000000-0000-0000-0000-000000000000";
        entity["@odata.type"] = "Microsoft.Dynamics.CRM.udo_validateaddress";
        entity.udo_addresstype = "752280000";
        entity.udo_addressline1 = formContext.getAttribute("va_mailingaddress1").getValue();
        entity.udo_addressline2 = formContext.getAttribute("va_mailingaddress2").getValue();
        entity.udo_addressline3 = formContext.getAttribute("va_mailingaddress3").getValue();
        entity.udo_city = formContext.getAttribute("va_mailingcity").getValue();
        entity.udo_stateprovince = formContext.getAttribute("va_mailingstateoptionset").getValue().toString();
        entity.udo_postalcode = formContext.getAttribute("va_mailingaddresszipcode").getValue();

        var mailingAddress = [entity];

        if (addresses === undefined)
            addresses = mailingAddress;
        else
            addresses = $.merge(mailingAddress, addresses);
    }

    if (formContext.getAttribute("va_paymentaddresschanged").getValue()
        && (formContext.getAttribute("va_paymentaddress1").getValue() !== null && formContext.getAttribute("va_paymentaddress1").getValue() !== "")
        && (formContext.getAttribute("va_paymentcity").getValue() !== null && formContext.getAttribute("va_paymentcity").getValue() !== "")
        && (formContext.getAttribute("va_paymentstateoptionset").getValue() !== null && formContext.getAttribute("va_paymentstateoptionset").getValue() !== "")) {
        calling = true;

        entity = {};
        entity.udo_validateaddressid = "00000000-0000-0000-0000-000000000000";
        entity["@odata.type"] = "Microsoft.Dynamics.CRM.udo_validateaddress";
        entity.udo_addresstype = "752280001";
        entity.udo_addressline1 = formContext.getAttribute("va_paymentaddress1").getValue();
        entity.udo_addressline2 = formContext.getAttribute("va_paymentaddress2").getValue();
        entity.udo_addressline3 = formContext.getAttribute("va_paymentaddress3").getValue();
        entity.udo_city = formContext.getAttribute("va_paymentcity").getValue();
        entity.udo_stateprovince = formContext.getAttribute("va_paymentstateoptionset").getValue().toString();
        entity.udo_postalcode = formContext.getAttribute("va_paymentzipcode").getValue();

        var paymentAddress = [entity];

        if (addresses === undefined)
            addresses = paymentAddress;
        else
            addresses = $.merge(paymentAddress, addresses);
    }

    if (formContext.getAttribute("va_appellantaddresschanged").getValue()
        && (formContext.getAttribute("va_apellantaddress1").getValue() !== null && formContext.getAttribute("va_apellantaddress1").getValue() !== "")
        && (formContext.getAttribute("va_apellantcity").getValue() !== null && formContext.getAttribute("va_apellantcity").getValue() !== "")
        && (formContext.getAttribute("va_appellantstateoptionset").getValue() !== null && formContext.getAttribute("va_appellantstateoptionset").getValue() !== "")) {
        calling = true;

        entity = {};
        entity.udo_validateaddressid = "00000000-0000-0000-0000-000000000000";
        entity["@odata.type"] = "Microsoft.Dynamics.CRM.udo_validateaddress";
        entity.udo_addresstype = "752280002";
        entity.udo_addressline1 = formContext.getAttribute("va_apellantaddress1").getValue();
        entity.udo_addressline2 = formContext.getAttribute("va_apellantaddress2").getValue();
        entity.udo_addressline3 = formContext.getAttribute("va_apellantaddress3").getValue();
        entity.udo_city = formContext.getAttribute("va_apellantcity").getValue();
        entity.udo_stateprovince = formContext.getAttribute("va_appellantstateoptionset").getValue().toString();
        entity.udo_postalcode = formContext.getAttribute("va_apellantzipcode").getValue();

        var appellantAddress = [entity];

        if (addresses === undefined)
            addresses = appellantAddress;
        else
            addresses = $.merge(appellantAddress, addresses);
    }

    if (calling) {
        var entityId = formContext.data.entity.getId().replace("{", "").replace("}", "");
        executeValidateAddressAction(entityId, "va_bankaccount", "udo_ValidateAddress", addresses);
    }
    else {
        addressValidated = true;
    }
}

function executeValidateAddressAction(parentEntityId, parentEntityName, requestName, addressCollectionParam) {

    // NOTE: !!! 9/30/2020 - it was determine that the address validation service at the VA has been disabled.  Until further notice, validating addresses will not occur for CADD !!!
    addressValidated = true;
    return;

    _formContext.ui.setFormNotification("Validating updated addresses. Please wait...", "INFO", requestName);

    try {
        var parameters = {};

        var parententityreference = {};
        parententityreference.id = parentEntityId;
        parententityreference.entityType = parentEntityName;

        parameters.ParentEntityReference = parententityreference;
        parameters.addresses = addressCollectionParam;

        var udo_ValidateAddressRequest = {
            ParentEntityReference: parameters.ParentEntityReference,
            Addresses: parameters.addresses,

            getMetadata: function () {
                return {
                    boundParameter: null,
                    parameterTypes: {
                        "ParentEntityReference": {
                            "typeName": "mscrm.crmbaseentity",
                            "structuralProperty": 5
                        },
                        "Addresses": {
                            "typeName": "Collection(mscrm.crmbaseentity)",
                            "structuralProperty": 4
                        }
                    },
                    operationType: 0,
                    operationName: requestName
                };
            }
        };

        _formContext.ui.clearFormNotification(requestName);

        Xrm.WebApi.online.execute(udo_ValidateAddressRequest).then(
            function success(result) {
                if (result.ok) {
                    result.json().then(function (response) {
                        var confidenceThreshold = 99;

                        if (!response.Exception) {
                            var results = response["ValidatedAddresses"];
                            var valid = true;

                            for (var i = 0; i < results.length; i++) {
                                if (results[i].udo_exceptionoccurred !== undefined) {
                                    valid = false;
                                    break;
                                }
                                else {
                                    var confidence = results[i].udo_confidence;

                                    if (typeof (confidence) === 'undefined' || confidence < confidenceThreshold) {
                                        valid = false;
                                        break;
                                    }
                                }
                            }

                            if (valid) {
                                addressValidated = true;
                            }
                            else {
                                // Prompt user with status of each address that was validated
                                promptAddressValidationResults(results);
                            }
                        }
                        else {
                            reportFailure(response.ResponseMessage, requestName);
                        }
                    });
                }
                else {
                    console.log("Error calling address validation action.");
                }

            },
            function (error) {
                var msg = error.message;
                var title = "Error Validating Address";
                UDO.Shared.openAlertDialog(msg, title, 300, 400).then(
                    function (response) {
                        _executionContext.getEventArgs().preventDefault();
                        return resolve();
                    },
                    function (error) {
                        return reject();
                    });
            }
        );
    }
    catch (error) {
        reportFailure(error, requestName);
    }
}

function reportFailure(error, requestName) {
    _formContext.ui.setFormNotification("Error occurred trying to validate addresses. Proceeding with CADD.", "INFO", requestName);

    console.log("Error validating addresses: " + error);
    setTimeout(function () { _formContext.ui.clearFormNotification(requestName); }, 10000);
    addressValidated = true;
}

function promptAddressValidationResults(addresses) {
    var addressValidationMessage = "";
    var invalidCount = 0;
    var exceptioncount = 0;
    var addHeader = false;
    var msg = "";

    for (var i = 0; i < addresses.length; i++) {
        var confidence = addresses[i].udo_confidence;
        var addressType = addresses[i].udo_addresstype;
        var status = addresses[i].udo_status;
        var statusCode = addresses[i].udo_statuscode;
        var stateProvinceResult = addresses[i].udo_stateprovinceresult;
        var exceptionOccurred = addresses[i].udo_exceptionoccurred;
        var exceptionMessage = addresses[i].udo_exceptionmessage;

        _formContext.ui.clearFormNotification("ADDRESSVALIDATIONWARNING" + i);

        if (exceptionOccurred) {
            msg = "Address[" + (i + 1) + "] validation from service within the VA returned the following message: " + exceptionMessage;
            console.log(msg);
            _formContext.ui.setFormNotification(msg, "WARNING", "ADDRESSVALIDATIONWARNING" + i);
            exceptioncount++;
        } else if (status === "F") {
            if (addressValidationMessage.length > 0)
                addressValidationMessage += "\n\n";

            addressValidationMessage += "Address Type: " + addressType + "\n\nThe address is not recognized\nDo you want to continue anyway?";

            invalidCount++;
        } else if (stateProvinceResult === 'C') {
            var stateOrProvince = addresses[i].udo_stateprovince;

            addressValidationMessage = 'State and ZIP Code are an invalid combination\nThe state for this ZIP Code is: ' +
                stateOrProvince + '.\nPlease make necessary corrections and click Save again';

            var msg = addressValidationMessage;
            var title = "Address Validation Error";
            UDO.Shared.openAlertDialog(msg, title, 200, 450).then(
                function (response) {
                    return resolve();
                },
                function (error) {
                    return reject();
                });
        } else if (statusCode === "UnableToDPVConfirm" || confidence < 95) {
            addHeader = true;
            if (addressValidationMessage.length > 0)
                addressValidationMessage += "\n\n";

            addressValidationMessage += "Do you want to use the format below instead?\n\nAddress Type: " + addressType;

            // Add Returned Address to Message
            var block1 = addresses[i].udo_addressblock1;
            var block2 = addresses[i].udo_addressblock2;
            var block3 = addresses[i].udo_addressblock3;

            if (block1 !== null && block2 !== null) {
                addressValidationMessage += "\n\n" + block1 + "\n" + block2;

                if (block3 !== null)
                    addressValidationMessage += "\n" + block3;
            }
            invalidCount++;
        }
    }

    if (addHeader)
        addressValidationMessage = "The {0} entered {1} not a valid {2} by United States Postal Service Standards.\n\n" + addressValidationMessage;

    addressValidationMessage += "\n\nPress OK to submit the form as is or Cancel to return back to the form and make changes.";

    if (exceptioncount > 0) {
        var msg = "Address validation was not completed for one or more addresses. Would you like to continue?";
        var title = "Unabled To Validate Address At This Time";
        UDO.Shared.openConfirmDialog(msg, title, "Yes", "No").then(
            function (response) {
                if (response.confirmed) {
                    if (processCADD(_executionContext)) {
                        _isLastUpdate = true;
                        addressValidated = true;
                    }

                    return resolve();
                }
                else {
                    addressValidated = false;
                    return reject();
                }
            },
            function (error) {
                addressValidated = false;
                return reject();
            });
    }

    if (invalidCount === 0) {
        addressValidated = true;
        return;
    }
    else {
        if (invalidCount > 1) {
            addressValidationMessage = addressValidationMessage.replace('{0}', 'addresses');
            addressValidationMessage = addressValidationMessage.replace('{1}', 'are');
            addressValidationMessage = addressValidationMessage.replace('{2}', 'formats');
        }
        else {
            addressValidationMessage = addressValidationMessage.replace('{0}', 'address');
            addressValidationMessage = addressValidationMessage.replace('{1}', 'is');
            addressValidationMessage = addressValidationMessage.replace('{2}', 'format');
        }

        var title = "Address Validation";
        UDO.Shared.openConfirmDialog(addressValidationMessage, title, "Yes", "No").then(
            function (response) {
                if (response.confirmed) {
                    addressValidated = true;
                    return resolve();
                }
                else {
                    addressValidated = false;
                    return reject();
                }
            },
            function (error) {
                addressValidated = false;
                return reject();
            });
    }
}
