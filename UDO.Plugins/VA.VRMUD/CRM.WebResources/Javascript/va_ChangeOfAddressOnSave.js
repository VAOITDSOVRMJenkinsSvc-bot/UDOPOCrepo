/// <reference path="../Intellisense/XrmPage-vsdoc.js" />

var updateAddresses_xmlObject = null;
_parent = null;
var parent_page = null;
var opener = null;


if (window.parent && window.parent.opener && window.parent.opener.Xrm.Page) {
    parent_page = window.parent.opener.Xrm.Page;
    opener = window.parent.opener;
    if (!parent_page.data && window.top && window.top.opener && window.top.opener.parent) {
        parent_page = window.top.opener.parent.Xrm.Page;
        opener = window.top.opener;
    }
}

function OnSave(context) {
    try {
        if (ValidateZipcode() == false) {
            context.getEventArgs().preventDefault(); // RU12 Changed all event.returnValue
            return false;
        }

        if (ValidateFields() == false) {
            context.getEventArgs().preventDefault(); // RU12 Changed all event.returnValue
            return false;
        }

        if (!parent_page || !parent_page.getAttribute('va_ssn') || Xrm.Page.ui.getFormType() != CRM_FORM_TYPE_CREATE) {
            alert('Change of Address and Bank Account can only be opened from Awards or Appeals view of the Veteran. Record cannot be modified after it is created. To change the address again, open new Change of Address form.\n\nThis record cannot be saved.');
            context.getEventArgs().preventDefault();
            return false;
        }

        if (_errorDuringLoad) {
            alert('Error was detected during screen load. This record cannot be saved.');
            context.getEventArgs().preventDefault();
            return false;
        }

        if (_viewAddressOnly) {
            alert('The Address and Bank Information screen was opened in Read-Only mode. This record cannot be saved.');
            context.getEventArgs().preventDefault();
            return false;
        }

        if (globalcadd.ca) {
            var va_depositaccountnumber = Xrm.Page.getAttribute('va_depositaccountnumber').getValue();
            var va_routingnumber = Xrm.Page.getAttribute('va_routingnumber').getValue();

            if (va_depositaccountnumber != null && va_depositaccountnumber.indexOf('*') == -1 && va_routingnumber != null && va_routingnumber.indexOf('*') != -1) {
                alert('The account number is assigned to a debit card expressly used for federal benefits and is invalid for entry. This account can only be set up by Auto Enrollment through Comerica. Please contact Treasury\'s Go Direct Call Center at 1-800-333-1795 or www.GoDirect.org<http://www.GoDirect.org>.');
                context.getEventArgs().preventDefault();
                return false;
            }

            if (va_depositaccountnumber != null && va_depositaccountnumber.indexOf('*') != -1 && va_routingnumber != null && va_routingnumber.indexOf('*') == -1) {
                alert('The routing number is assigned to a debit card expressly used for federal benefits and is invalid for entry. This account can only be set up by Auto Enrollment through Comerica. Please contact Treasury\'s Go Direct Call Center at 1-800-333-1795 or www.GoDirect.org.');
                context.getEventArgs().preventDefault();
                return false;
            }
        }

        if (Xrm.Page.getAttribute('va_routingnumber').getValue() != null && Xrm.Page.getAttribute('va_routingnumber').getValue() == parent_page.getAttribute('va_crn').getValue()) {
            alert('The routing number is assigned to a debit card expressly used for federal benefits and is invalid for entry. This account can only be set up by Auto Enrollment through Comerica. Please contact Treasury\'s Go Direct Call Center at 1-800-333-1795 or www.GoDirect.org.');
            context.getEventArgs().preventDefault();
            return false;
        }

        // check mailing fields based on address type
        if (_changingAwardAddress) {
            var curCountry = Xrm.Page.getAttribute('va_mailingcountry').getValue();
            var va_mailingaddress1 = Xrm.Page.getAttribute('va_mailingaddress1').getValue();
            var va_mailingaddress2 = Xrm.Page.getAttribute('va_mailingaddress2').getValue();
            var va_mailingaddress3 = Xrm.Page.getAttribute('va_mailingaddress3').getValue();
            var va_paymentaddress1 = Xrm.Page.getAttribute('va_paymentaddress1').getValue();
            var va_paymentaddress2 = Xrm.Page.getAttribute('va_paymentaddress2').getValue();
            var va_paymentaddress3 = Xrm.Page.getAttribute('va_paymentaddress3').getValue();
            var va_mailingcity = Xrm.Page.getAttribute('va_mailingcity').getValue();
            var va_paymentcity = Xrm.Page.getAttribute('va_paymentcity').getValue();
            var va_mailingstate = Xrm.Page.getAttribute('va_mailingstate').getValue();
            var va_paymentstate = Xrm.Page.getAttribute('va_paymentstate').getValue();
            var va_mailingaddresszipcode = Xrm.Page.getAttribute('va_mailingaddresszipcode').getValue();
            var va_paymentzipcode = Xrm.Page.getAttribute('va_paymentzipcode').getValue();
            var showAlert = false;
            var alertMessage = "";

            switch (Xrm.Page.getAttribute('va_addresstype').getValue()) {
                //International
                case 953850001:
                    var va_mailingcountry = Xrm.Page.getAttribute('va_mailingcountry').getValue();
                    var va_paymentcountry = Xrm.Page.getAttribute('va_paymentcountry').getValue();
                    var va_mailingcountrylist = Xrm.Page.getAttribute('va_mailingcountry').getValue();
                    if (!curCountry || curCountry.length == 0) {
                        Xrm.Page.getControl('va_mailingcountrylist').setFocus();
                        alert('You must provide a value for Country.');
                        context.getEventArgs().preventDefault();
                        return false;

                    }
                    else if (curCountry.toUpperCase() == 'USA' || curCountry.toUpperCase() == 'US' || curCountry.toUpperCase() == 'U.S.A.' ||
                                            curCountry.toUpperCase() == 'UNITED STATES') {
                        Xrm.Page.getControl('va_mailingcountrylist').setFocus();
                        alert('For International address, Country value must be provided, and Country cannot be USA. This record cannot be saved.');
                        context.getEventArgs().preventDefault();
                        return false;
                    }
                    var mfz = Xrm.Page.getAttribute('va_mailingforeignpostalcode').getValue();
                    if (!mfz || mfz.length == 0) {
                        Xrm.Page.getControl('va_mailingforeignpostalcode').setFocus();
                        alert('For International address, Foreign Postal Code is required. This record cannot be saved.');
                        context.getEventArgs().preventDefault();
                        return false;
                    }
                    //                if (Xrm.Page.getAttribute('va_mailingaddressexistsindicator').getValue()) {
                    //                    var z = Xrm.Page.getAttribute('va_mailingforeignmailingcode').getValue();
                    //                    if (z == null || z == '') {
                    //                        alert('International Mailing Code is required if Mailing address is International. This record cannot be saved.'); context.getEventArgs().preventDefault(); return false;
                    //                    }
                    //                }
                    //                if (Xrm.Page.getAttribute('va_paymentaddressexistsindicator').getValue()) {
                    //                    var z = Xrm.Page.getAttribute('va_paymentforeignmailingcode').getValue();
                    //                    if (z == null || z == '') {
                    //                        alert('International Mailing Code is required if Payment address is International. This record cannot be saved.'); context.getEventArgs().preventDefault(); return false;
                    //                    }
                    //                } /* */
                    if (isEFT() == false) {
                        if ((va_mailingaddress1 != null && va_paymentaddress1 == null) ||
     (va_mailingaddress1 == null && va_paymentaddress1 != null) ||
     (va_mailingaddress1 != null && va_paymentaddress1 != null && va_mailingaddress1.toUpperCase().trim() != va_paymentaddress1.toUpperCase().trim())) {
                            Xrm.Page.getControl('va_mailingaddress1').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address1", "Payment address1");
                            showAlert = true;
                        } else if ((va_mailingaddress2 != null && va_paymentaddress2 == null) ||
    (va_mailingaddress2 == null && va_paymentaddress2 != null) ||
    (va_mailingaddress2 != null && va_paymentaddress2 != null && va_mailingaddress2.toUpperCase().trim() != va_paymentaddress2.toUpperCase().trim())) {
                            Xrm.Page.getControl('va_mailingaddress2').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address2", "Payment address2");
                            showAlert = true;
                        } else if ((va_mailingaddress3 != null && va_paymentaddress3 == null) ||
    (va_mailingaddress3 == null && va_paymentaddress3 != null) ||
    (va_mailingaddress3 != null && va_paymentaddress3 != null && va_mailingaddress3.toUpperCase().trim() != va_paymentaddress3.toUpperCase().trim())) {
                            Xrm.Page.getControl('va_mailingaddress3').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address3", "Payment address3");
                            showAlert = true;
                        } else if ((va_mailingcity != null && va_paymentcity == null) ||
    (va_mailingcity == null && va_paymentcity != null) ||
    (va_mailingcity != null && va_paymentcity != null && va_mailingcity.toUpperCase().trim() != va_paymentcity.toUpperCase().trim())) {
                            Xrm.Page.getControl('va_mailingcity').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing city", "Payment city");
                            showAlert = true;
                        } else if ((va_mailingcountry != null && va_paymentcountry == null) ||
    (va_mailingcountry == null && va_paymentcountry != null) ||
    (va_mailingcountry != null && va_paymentcountry != null && va_mailingcountry.toUpperCase().trim() != va_paymentcountry.toUpperCase().trim())) {
                            Xrm.Page.getControl('va_mailingcountrylist').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing country", "Payment country");
                            showAlert = true;
                        }
                        if (showAlert) {
                            alert(alertMessage);
                            context.getEventArgs().preventDefault();
                            return false;
                        }
                    }//if isEFT()
                    break;
                    //Overseas
                case 953850002:
                    var va_mailingmilitarypostaltypecode = Xrm.Page.getAttribute('va_mailingmilitarypostaltypecode').getText();
                    var va_mailingmilitarypostofficetypecode = Xrm.Page.getAttribute('va_mailingmilitarypostofficetypecode').getSelectedOption().text;
                    var va_paymentmilitarypostaltypecode = Xrm.Page.getAttribute('va_paymentmilitarypostaltypecode').getText();
                    var va_paymentmilitarypostofficetypecode = Xrm.Page.getAttribute('va_paymentmilitarypostofficetypecode').getSelectedOption();
                    if (va_paymentmilitarypostofficetypecode != null) {
                        va_paymentmilitarypostofficetypecode = Xrm.Page.getAttribute('va_paymentmilitarypostofficetypecode').getSelectedOption().text;
                    }
                    var va_milzipcodelookupid = Xrm.Page.getAttribute('va_milzipcodelookupid').getValue();
                    if (va_milzipcodelookupid != null) {
                        va_milzipcodelookupid = Xrm.Page.getAttribute('va_milzipcodelookupid').getValue()[0].name;
                    }
                    var z = Xrm.Page.getAttribute('va_mailingmilitarypostaltypecode').getText();
                    var z2 = Xrm.Page.getAttribute("va_mailingmilitarypostofficetypecode").getSelectedOption().text;
                    if (z == null || z == '' || z2 == null || z2 == '') {
                        Xrm.Page.getControl('va_mailingmilitarypostaltypecode').setFocus();
                        alert('Overeas Military Postal Type Code and Post Office Type Code are required if Mailing address is Overseas. This record cannot be saved.');
                        context.getEventArgs().preventDefault();
                        return false;
                    }

                    if (curCountry && curCountry.length > 0) {
                        Xrm.Page.getControl('va_mailingcountry').setFocus();
                        alert('For Overseas address, Country value must be blank. This record cannot be saved.');
                        context.getEventArgs().preventDefault();
                        return false;
                    }
                    var mstate = Xrm.Page.getAttribute('va_mailingstate').getValue();
                    if (mstate && mstate.length > 0) {
                        Xrm.Page.getControl('va_mailingstateoptionset').setFocus();
                        alert('For Overseas address, State value must be blank. This record cannot be saved.');
                        context.getEventArgs().preventDefault();
                        return false;
                    }
                    //                        var z = Xrm.Page.getAttribute('va_paymentmilitarypostaltypecode').getText();
                    //                        var z2 = Xrm.Page.getAttribute('va_paymentmilitarypostofficetypecode').getText();
                    //                        if (z == null || z == '' || z2 == null || z2 == '') {
                    //                            alert('Overeas Military Postal Type Code and Post Office Type Code are required if Payment address is Overseas. This record cannot be saved.'); context.getEventArgs().preventDefault(); return false;
                    //                        }
                    if (isEFT() == false) {
                        if ((va_mailingaddress1 != null && va_paymentaddress1 == null) ||
     (va_mailingaddress1 == null && va_paymentaddress1 != null) ||
     (va_mailingaddress1 != null && va_paymentaddress1 != null && va_mailingaddress1.toUpperCase().trim() != va_paymentaddress1.toUpperCase().trim())) {
                            Xrm.Page.getControl('va_mailingaddress1').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address1", "Payment address1");
                            showAlert = true;
                        } else if ((va_mailingaddress2 != null && va_paymentaddress2 == null) ||
    (va_mailingaddress2 == null && va_paymentaddress2 != null) ||
    (va_mailingaddress2 != null && va_paymentaddress2 != null && va_mailingaddress2.toUpperCase().trim() != va_paymentaddress2.toUpperCase().trim())) {
                            Xrm.Page.getControl('va_mailingaddress2').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address2", "Payment address2");
                            showAlert = true;
                        } else if ((va_mailingaddress3 != null && va_paymentaddress3 == null) ||
    (va_mailingaddress3 == null && va_paymentaddress3 != null) ||
    (va_mailingaddress3 != null && va_paymentaddress3 != null && va_mailingaddress3.toUpperCase().trim() != va_paymentaddress3.toUpperCase().trim())) {
                            Xrm.Page.getControl('va_mailingaddress3').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address3", "Payment address3");
                            showAlert = true;
                        } else if ((va_mailingmilitarypostaltypecode != null && va_paymentmilitarypostaltypecode == null) ||
    (va_mailingmilitarypostaltypecode == null && va_paymentmilitarypostaltypecode != null) ||
    (va_mailingmilitarypostaltypecode != null && va_paymentmilitarypostaltypecode != null && va_mailingmilitarypostaltypecode != va_paymentmilitarypostaltypecode)) {
                            Xrm.Page.getControl('va_mailingmilitarypostaltypecode').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing overseas military postal type code", "Payment overseas military postal type code");
                            showAlert = true;
                        } else if ((va_mailingmilitarypostofficetypecode != null && va_paymentmilitarypostofficetypecode == null) ||
    (va_mailingmilitarypostofficetypecode == null && va_paymentmilitarypostofficetypecode != null) ||
    (va_mailingmilitarypostofficetypecode != null && va_paymentmilitarypostofficetypecode != null && va_mailingmilitarypostofficetypecode != va_paymentmilitarypostofficetypecode)) {
                            Xrm.Page.getControl('va_mailingmilitarypostofficetypecode').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing overseas military post office type code", "Payment overseas military post office type code");
                            showAlert = true;
                        } else if ((va_milzipcodelookupid != null && va_paymentzipcode == null) ||
    (va_milzipcodelookupid == null && va_paymentzipcode != null) ||
    (va_milzipcodelookupid != null && va_paymentzipcode != null && va_milzipcodelookupid.toUpperCase().trim() != va_paymentzipcode.toUpperCase().trim())) {
                            Xrm.Page.getControl('va_milzipcodelookupid').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing mil. zip code lookup", "Payment zip code");
                            showAlert = true;
                        }
                        if (showAlert) {
                            alert(alertMessage);
                            context.getEventArgs().preventDefault();
                            return false;
                        }
                    }//if isEFT()
                    break;
                    //Domestic
                case 953850000:
                    if (!curCountry || curCountry.length == 0 || curCountry.toUpperCase() != 'USA') {
                        Xrm.Page.getAttribute('va_mailingcountry').setValue('USA');
                    }

                    if (isEFT() == false) {
                        if ((va_mailingaddress1 != null && va_paymentaddress1 == null) ||
     (va_mailingaddress1 == null && va_paymentaddress1 != null) ||
     (va_mailingaddress1 != null && va_paymentaddress1 != null && va_mailingaddress1.toUpperCase().trim() != va_paymentaddress1.toUpperCase().trim())) {
                            Xrm.Page.getControl('va_mailingaddress1').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address1", "Payment address1");
                            showAlert = true;
                        } else if ((va_mailingaddress2 != null && va_paymentaddress2 == null) ||
    (va_mailingaddress2 == null && va_paymentaddress2 != null) ||
    (va_mailingaddress2 != null && va_paymentaddress2 != null && va_mailingaddress2.toUpperCase().trim() != va_paymentaddress2.toUpperCase().trim())) {
                            Xrm.Page.getControl('va_mailingaddress2').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address2", "Payment address2");
                            showAlert = true;
                        } else if ((va_mailingaddress3 != null && va_paymentaddress3 == null) ||
    (va_mailingaddress3 == null && va_paymentaddress3 != null) ||
    (va_mailingaddress3 != null && va_paymentaddress3 != null && va_mailingaddress3.toUpperCase().trim() != va_paymentaddress3.toUpperCase().trim())) {
                            Xrm.Page.getControl('va_mailingaddress3').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing address3", "Payment address3");
                            showAlert = true;
                        } else if ((va_mailingcity != null && va_paymentcity == null) ||
    (va_mailingcity == null && va_paymentcity != null) ||
    (va_mailingcity != null && va_paymentcity != null && va_mailingcity.toUpperCase().trim() != va_paymentcity.toUpperCase().trim())) {
                            Xrm.Page.getControl('va_mailingcity').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing city", "Payment city");
                            showAlert = true;
                        } else if ((va_mailingstate != null && va_paymentstate == null) ||
    (va_mailingstate == null && va_paymentstate != null) ||
    (va_mailingstate != null && va_paymentstate != null && va_mailingstate.toUpperCase().trim() != va_paymentstate.toUpperCase().trim())) {
                            Xrm.Page.getControl('va_mailingstateoptionset').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing state", "Payment state");
                            showAlert = true;
                        } else if ((va_mailingaddresszipcode != null && va_paymentzipcode == null) ||
    (va_mailingaddresszipcode == null && va_paymentzipcode != null) ||
    (va_mailingaddresszipcode != null && va_paymentzipcode != null && va_mailingaddresszipcode.toUpperCase().trim() != va_paymentzipcode.toUpperCase().trim())) {
                            Xrm.Page.getControl('va_mailingaddresszipcode').setFocus();
                            alertMessage = createMatchingAddressesError("Mailing zip code", "Payment zip code");
                            showAlert = true;
                        }
                        if (showAlert) {
                            alert(alertMessage);
                            context.getEventArgs().preventDefault();
                            return false;
                        }
                    }//end isEFT
                    break;
                default:
                    break;
            }
        }

        if (Xrm.Page.getAttribute('va_depositaccounttype').getText() != globalcadd.ca_t && globalcadd.ca) {
            alert('The account type is assigned to a debit card expressly used for federal benefits and is invalid for entry. This account can only be set up by Auto Enrollment through Comerica. Please contact Treasury\'s Go Direct Call Center at 1-800-333-1795 or www.GoDirect.org<http://www.GoDirect.org>.');
            context.getEventArgs().preventDefault();
            return false;
        }

        var msg = 'Please confirm that you would like to update ';
        if (_changingAwardAddress) msg += 'address/bank account';
        if (_hasAppeals) msg += (_changingAwardAddress ? '/' : '' + 'appeals');
        msg += ' information.';

        if (!confirm(msg)) {
            context.getEventArgs().preventDefault();
            return false;
        }

        var postalCode = Xrm.Page.getAttribute('va_mailingaddresszipcode').getValue() ?
	        Xrm.Page.getAttribute('va_mailingaddresszipcode').getValue() : Xrm.Page.getAttribute('va_mailingforeignpostalcode').getValue();

        var params = [
            { name: 'AddressLine1', value: Xrm.Page.getAttribute('va_mailingaddress1').getValue() },
            { name: 'AddressLine2', value: Xrm.Page.getAttribute('va_mailingaddress2').getValue() },
            { name: 'AddressLine3', value: Xrm.Page.getAttribute('va_mailingaddress3').getValue() },
            { name: 'AddressLine4', value: '' },
            { name: 'City', value: Xrm.Page.getAttribute('va_mailingcity').getValue() },
            { name: 'StateProvince', value: Xrm.Page.getAttribute('va_mailingstate').getValue() },
            { name: 'PostalCode', value: postalCode },
            { name: 'Country', value: Xrm.Page.getAttribute('va_mailingcountry').getValue() }
        ];

        // validate address and ask to confirm if error or score less than 100
        //        var response = validateAddressUsingWS('Mailing', params, 'va_validateaddressresponse', 'va_mailingaddressvalidationscore');
        //        var wsMessage = (
        //            response.error ?
        //            response.message + '\n\nWould you like to disregard the error message and proceed with address update?' :
        //            (
        //                response.score === 100 ? '' :
        //                'Mailing Address Validation Service responded with less than perfect score:\nScore: ' +
        //                    response.score.toString() + (response.message.length > 0 ? '\nMessage: ' + response.message : '') +
        //                    '\n\nWould you like to disregard the validation results and proceed with address update?'
        //            )
        //        );

        //        if (wsMessage.length > 0 && !confirm(wsMessage)) {
        //            context.getEventArgs().preventDefault();
        //            return false;
        //        }

        var updateBenefitClaimAddressCtx = new vrmContext();
        updateBenefitClaimAddressCtx.addressParameters = GetUserSettingsForWebservice();

        if (_changingAwardAddress) {
            //Validating country names against the "official" list obtained from WS
            var mailingCountryValid = false;
            var paymentCountryValid = false;

            if (CountryList_xmlObject) {
                returnNode = CountryList_xmlObject.selectNodes('//return');
                CountryListNodes = returnNode[0].childNodes;

                if (CountryListNodes) {
                    for (var i = 0; i < CountryListNodes.length; i++) {         //looping through countries and
                        if (CountryListNodes[i].nodeName == 'types') {  //making sure we dont check irrelevant nodes

                            if (CountryListNodes[i].selectSingleNode('name').text == Xrm.Page.getAttribute('va_mailingcountry').getValue()) {
                                mailingCountryValid = true;
                            }

                            if (CountryListNodes[i].selectSingleNode('name').text == Xrm.Page.getAttribute('va_paymentcountry').getValue()) {
                                paymentCountryValid = true;
                            }

                        }
                    }
                }
            }

            if (Xrm.Page.getAttribute('va_mailingcountry').getValue() && mailingCountryValid == false) {
                CloseProgress();
                alert('You must provide a value for Country.');
                context.getEventArgs().preventDefault();
                Xrm.Page.getControl("va_mailingcountrylist").setFocus();
                return false;
            }

            if (Xrm.Page.getAttribute('va_paymentcountry').getValue() && paymentCountryValid == false) {
                CloseProgress();
                alert('Failed to update Address/Bank info. Payment country is not valid. Please use the lookup feature to get a valid country name.');
                context.getEventArgs().preventDefault();
                return false;
            }
        }

        var createdNote = false;

        if (_hasAppeals) {
            var appellantAddressCtx = new vrmContext();
            appellantAddressCtx.user = GetUserSettingsForWebservice();
            appellantAddressCtx.parameters['addressKey'] = Xrm.Page.getAttribute('va_appellantaddresskey').getValue();
            if (Xrm.Page.getAttribute('va_apellantssn') != null) {
                appellantAddressCtx.parameters['fileNumber'] = Xrm.Page.getAttribute('va_apellantssn').getValue();
            }
            else {
                appellantAddressCtx.parameters['fileNumber'] = Xrm.Page.getAttribute('va_filenumber').getValue();
            }

            ShowProgress('Updating Vacols Apellant Addresses');

            var updateAppellantAddressService = new updateAppellantAddress(appellantAddressCtx);
            var aaErr = '';
            var aaRes = false;
            try {
                aaRes = updateAppellantAddressService.executeRequest();
            }
            catch (err) {
                CloseProgress();
                aaErr = 'Award Address/Bank info has been updated but Appellant Address Update Service had failed to update Appellant Address. Make sure that the service is reachable.\n' + err.message;
                alert(aaErr);
            }

            if (aaRes) {
                var updateAppellantAddress_xmlObject = _XML_UTIL.parseXmlObject(updateAppellantAddressService.responseXml);

                if (updateAppellantAddress_xmlObject && updateAppellantAddress_xmlObject != undefined
				&& updateAppellantAddress_xmlObject.xml != '') {
                    var currentAddressKey = Xrm.Page.getAttribute('va_appellantaddresskey').getValue();
                    var updateAddressKey = null;

                    if (SingleNodeExists(updateAppellantAddress_xmlObject, '//AddressKey')) {
                        updateAddressKey = updateAppellantAddress_xmlObject.selectSingleNode('//AddressKey').text;

                        if (currentAddressKey != updateAddressKey) {
                            CloseProgress();
                            alert('Failed to update Appellant Address info. Middle-tier server returned following error: ' + GetErrorMessages('\n'));
                            context.getEventArgs().preventDefault();
                            return false;
                        }
                        else {
                            // create dev note
                            createdNote = CreateNote(null);

                            var fName, mName, lName, modName;

                            fName = SingleNodeExists(updateAppellantAddress_xmlObject, '//AppellantFirstName')
							? updateAppellantAddress_xmlObject.selectSingleNode('//AppellantFirstName').text : null;

                            mName = SingleNodeExists(updateAppellantAddress_xmlObject, '//AppellantMiddleInitial')
							? updateAppellantAddress_xmlObject.selectSingleNode('//AppellantMiddleInitial').text : null;

                            lName = SingleNodeExists(updateAppellantAddress_xmlObject, '//AppellantLastName')
							? updateAppellantAddress_xmlObject.selectSingleNode('//AppellantLastName').text : null;

                            modDate = SingleNodeExists(updateAppellantAddress_xmlObject, '//AppellantAddress/AppellantAddressLastModifiedDate')
							? updateAppellantAddress_xmlObject.selectSingleNode('//AppellantAddress/AppellantAddressLastModifiedDate').text : null;

                            modName = SingleNodeExists(updateAppellantAddress_xmlObject, '//AppellantAddress/AppellantAddressLastModifiedByROName')
							? updateAppellantAddress_xmlObject.selectSingleNode('//AppellantAddress/AppellantAddressLastModifiedByROName').text : null;

                            var fullName = (fName ? fName + ' ' : '') + (mName ? mName + '. ' : '') + (lName ? lName + ' ' : '');
                            var msg = 'Update Appellant Address completed for ' + fullName + 'on '
							+ new Date().format("MM/dd/yyyy").toString();

                            Xrm.Page.getAttribute('va_requeststatus').setValue(msg);

                            Xrm.Page.getAttribute('va_apellantmodifiedon').setValue(modDate);
                            Xrm.Page.getAttribute('va_apellantmodifiedby').setValue(modName);

                            var vipApp = getVIPApplication();
                            if (vipApp && vipApp != undefined) {
                                vipApp.fireEvent('crmchangeofaddresscompleted');
                            }

                            CloseProgress();
                        }
                    }
                }
            } else {
                CloseProgress();
                alert('Failed to update Appellant Address info. Middle-tier server returned following error: ' + GetErrorMessages('\n'));
                //context.getEventArgs().preventDefault();
                //return false;
            }
        }

        if (_changingAwardAddress /*|| _hasAppeals*/) {
            ShowProgress('Updating Addresses/Bank Information');
            updateBenefitClaimAddressCtx.user = GetUserSettingsForWebservice();
            var updateBenefitClaimAddressDetail = new updateBenefitClaimAddress(updateBenefitClaimAddressCtx);
            updateBenefitClaimAddressDetail.executeRequest();

            var $xml = $($.parseXML(updateBenefitClaimAddressDetail.soapBodyInnerXml.replace('ser:', '').replace('/ser:', '/')));
            $xml.find('eftAccountNumber').text('**********');
            $xml.find('eftRoutingNumber').text('**********');

            //Xrm.Page.getAttribute('va_updateaddressrequest').setValue(formatXml(updateBenefitClaimAddressDetail.soapBodyInnerXml));

            var xmlString = undefined;
            if (window.ActiveXObject) {
                xmlString = $xml[0].xml;
            }

            if (xmlString === undefined) {
                xmlString = (new XMLSerializer()).serializeToString($xml[0]);
            }

            Xrm.Page.getAttribute('va_updateaddressrequest').setValue(xmlString);

            if (!updateBenefitClaimAddressDetail.wsMessage.errorFlag) {
                updateAddresses_xmlObject = _XML_UTIL.parseXmlObject(Xrm.Page.getAttribute('va_updateaddressresponse').getValue());

                if (!createdNote) {
                    // create dev note
                    CreateNote(null);
                }
            } else {
                Xrm.Page.getAttribute('va_updateaddressresponse').setValue(formatXml(updateBenefitClaimAddressDetail.responseXml));
                CloseProgress();
                alert('Failed to update Address/Bank info. Middle-tier server returned following error: ' + GetErrorMessages('\n'));
                context.getEventArgs().preventDefault();
                return false;
            }

            if (updateAddresses_xmlObject) {
                returnMessage = updateAddresses_xmlObject.selectSingleNode('//returnMessage');
                Xrm.Page.getAttribute('va_requeststatus').setValue(returnMessage.text);

                if (parent_page.data && parent_page.data.entity) {
                    var parentName = parent_page.data.entity.getEntityName();
                    if (parentName == 'phonecall') {
                        var pid = Xrm.Page.getAttribute('va_participantvetid').getValue();
                        var fn = Xrm.Page.getAttribute('va_filenumber').getValue();
                        opener._RedrawCADDfields(pid);
                    }
                }
                // Xrm.Page.getAttribute('createdon').setValue(new Date().format('yyyy-MM-dd\'T\'HH:MM:ss'));
                // Xrm.Page.getAttribute('createdby').setValue(updateBenefitClaimAddressCtx.user.userName);
            } //END UPDATE SUCCESS
        }

        Xrm.Page.ui.tabs.get('General').sections.get('execution_results').setVisible(true);
        CloseProgress();
    }
    catch (err) {
        CloseProgress();
        alert("Error caught during OnSave event: " + err.message);
        context.getEventArgs().preventDefault();
        return false;
    }
    Xrm.Page.getAttribute('va_generalchanged').setValue(0);
    Xrm.Page.getAttribute('va_depositaccountchanged').setValue(0);
    Xrm.Page.getAttribute('va_mailingaddresschanged').setValue(0);
    Xrm.Page.getAttribute('va_paymentaddresschanged').setValue(0);
    Xrm.Page.getAttribute('va_phonenumberschanged').setValue(0);
    Xrm.Page.getAttribute('va_appellantaddresschanged').setValue(0);

}

function ValidateZipcode() {
    var va_addresstype = Xrm.Page.getAttribute('va_addresstype').getValue();
    if (va_addresstype == 953850000 || va_addresstype == 953850002) {
        var va_mailingaddresszipcode = Xrm.Page.getAttribute('va_mailingaddresszipcode').getValue();
        if (va_mailingaddresszipcode != null && va_mailingaddresszipcode.match(/[a-zA-Z]/)) {
            alert('Mailing zip code field contains invalid alphabetical characters');
            return false;
        }

        var va_paymentzipcode = Xrm.Page.getAttribute('va_paymentzipcode').getValue();
        if (va_paymentzipcode != null && va_paymentzipcode.match(/[a-zA-Z]/)) {
            alert('Payment zip code field contains invalid alphabetical characters');
            return false;
        }
    }

    var va_apellantzipcode = Xrm.Page.getAttribute('va_apellantzipcode').getValue();
    if (va_apellantzipcode != null && va_apellantzipcode.match(/[a-zA-Z]/)) {
        alert('Appellant zip code field contains invalid alphabetical characters');
        return false;
    }

    return true;
}

function GetErrorMessages(separator) {
    var msg = '';
    if (_VRMMESSAGE && _VRMMESSAGE.length > 0) {
        for (var i = 0; i < _VRMMESSAGE.length; i++) {
            if (_VRMMESSAGE[i].errorFlag) msg += separator + _VRMMESSAGE[i].description;
        }
    }
    return msg;
}

function CreateNote(serviceRequestId) {

    try {
        var pid = Xrm.Page.getAttribute('va_participantbeneid').getValue();
        if (!pid || pid.length == 0) pid = Xrm.Page.getAttribute('va_participantvetid').getValue();
        if (!pid || pid.length == 0) pid = Xrm.Page.getAttribute('va_participantrecipid').getValue();
        if (!pid || pid.length == 0) {
            alert('Note cannot be created because Participant Id is not available. You can manually create a Note on Claims tab of Phone Call screen.');
            return false;
        }
        //        debugger;
        ShowProgress('Creating new Development Note');

        //MN DD comparison 11/01/12

        var updateflag = "";

        var devNoteText = "";
        // var devNoteText2 = "";

        var failureFlag = "";

        var devNoteVar = "";

        var dobVerified = Xrm.Page.getAttribute('va_dobverified').getValue();
        var addressVerified = Xrm.Page.getAttribute('va_addressofrecord').getValue();
        var monthlyBenefitVerified = Xrm.Page.getAttribute('va_currentmonthlybenefit').getValue();
        var failedIDProofing = Xrm.Page.getAttribute('va_failedidproofing').getValue();

        var General = Xrm.Page.getAttribute('va_generalchanged').getValue();
        var DepositAccount = Xrm.Page.getAttribute('va_depositaccountchanged').getValue();
        var Mailing = Xrm.Page.getAttribute('va_mailingaddresschanged').getValue();
        var Payment = Xrm.Page.getAttribute('va_paymentaddresschanged').getValue();
        var Phones = Xrm.Page.getAttribute('va_phonenumberschanged').getValue();
        var Appellant = Xrm.Page.getAttribute('va_appellantaddresschanged').getValue();

        var pcrName = parent_page.getAttribute('ownerid').getValue()[0].name;
        var fullDateTime = new Date().format('MM/dd/yyyy hh:mm:ss tt');

        //call fulnameformat to split pcr name and reformated as First Name last Name
        pcrName = fullnameFormat(pcrName);
        var devNoteText = (pcrName ? "PCR Name:  " + pcrName + "\n" : "") + "Date:  " + fullDateTime + "\n";

        if (failedIDProofing == true) {
            failureFlag = "FAILED";
            if (dobVerified != true) {
                failureFlag += "DOB";
            }
            else {
                failureFlag += "";
            }
            if (addressVerified != true) {
                failureFlag += "ADD";
            }
            else {
                failureFlag += "";
            }
            if (monthlyBenefitVerified != true) {
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

        //Set flags
        //alert("DD0: " + globalcadd.bankdata + "\n" + "DD1: " + bankinfo2 + "\n\n" + "CD0: " + globalcadd.cadddata + "\n" + "CD1: " + caddinfo2);
        //Gen- General Changed
        //DAT- Deposit Account Changed
        //MAD- Mailing Address Changed
        //PAD- Payment Address Changed
        //Phones- Phone Numbers Changed
        //AppAdd- Appellant Address Changed


        if (General == 1) {
            devNoteVar = "General Section. \n";
        }
        if (DepositAccount == 1) {
            devNoteVar += "Deposit Account Section. \n";
        }
        if (Mailing == 1) {
            devNoteVar += "Mailing Address Section. \n";
        }

        if (Payment == 1) {
            devNoteVar += "Payment Address Section. \n";
        }

        if (Phones == 1) {
            devNoteVar += "Phone Numbers Section. \n";
        }

        if (Appellant == 1) {
            devNoteVar += "Appellant Address Section. \n";
        }


        //General==0 && DepositAccount==0 && Mailing==0 && Payment==0 && Phones==0 && Appellant==0

        if (DepositAccount == 1) {
            updateflag = "Bank";
        }
        else {
            updateflag = "";
        }
        if (Mailing == 1 || General == 1 || Payment == 1 || Phones == 1 || Appellant == 1) {
            updateflag += "CADD";
        }
        else {
            updateflag += "";
        }

        //alert(updateflag);
        switch (updateflag) {
            case "Bank":
                devNoteText += "Direct Deposit information has been updated.  ";
                break;
            case "CADD":
                devNoteText += "Change of Address for '" + Xrm.Page.getAttribute('va_name').getValue() + "' completed. " +
            "The following sections have been changed:  \n" +
            devNoteVar +
		    "File #: " + Xrm.Page.getAttribute('va_filenumber').getValue() + ";\n" +
		    "Bene PID: " + (Xrm.Page.getAttribute('va_participantbeneid').getValue() ? Xrm.Page.getAttribute('va_participantbeneid').getValue() : " ") + ";\n" +
		    "Vet PID: " + (Xrm.Page.getAttribute('va_participantvetid').getValue() ? Xrm.Page.getAttribute('va_participantvetid').getValue() : " ") + ";\n" +
		    "Recip PID: " + (Xrm.Page.getAttribute('va_participantrecipid').getValue() ? Xrm.Page.getAttribute('va_participantrecipid').getValue() : " ");
                break;
            case "BankCADD":
                devNoteText += "Direct Deposit information and Change of Address for '" + Xrm.Page.getAttribute('va_name').getValue() + "' completed. \n" +
            "The following sections have been changed:  \n" +
            devNoteVar +
		    "File #: " + Xrm.Page.getAttribute('va_filenumber').getValue() + ";\n" +
		    "Bene PID: " + (Xrm.Page.getAttribute('va_participantbeneid').getValue() ? Xrm.Page.getAttribute('va_participantbeneid').getValue() : " ") + ";\n" +
		    "Vet PID: " + (Xrm.Page.getAttribute('va_participantvetid').getValue() ? Xrm.Page.getAttribute('va_participantvetid').getValue() : " ") + ";\n" +
		    "Recip PID: " + (Xrm.Page.getAttribute('va_participantrecipid').getValue() ? Xrm.Page.getAttribute('va_participantrecipid').getValue() : " ");
                break;
            default:
                devNoteText += "No Address information has been changed.";
                break;
        }


        var createNoteCtx = new vrmContext();
        createNoteCtx.user = GetUserSettingsForWebservice();
        createNoteCtx.isContactNote = false;
        createNoteCtx.parameters['ptcpntId'] = pid;
        createNoteCtx.parameters['noteText'] =
			devNoteText.replace(new RegExp('<', 'g'), '&lt;').replace(new RegExp('>', 'g'), '&gt;').replace(new RegExp('&', 'g'), '&amp;').replace(new RegExp("'", 'g'), '&quot;');

        var createNoteDetail = new createNote(createNoteCtx);
        createNoteDetail.serviceName = 'createNote';
        createNoteDetail.wsMessage.serviceName = 'createNote';

        var res = createNoteDetail.executeRequest();

        var noteId = null;
        if (res) {
            var createNote_xmlObject = _XML_UTIL.parseXmlObject(createNoteDetail.responseXml);
            if (createNote_xmlObject.selectSingleNode('//noteId') != null) {
                noteId = createNote_xmlObject.selectSingleNode('//noteId').text;
            }
        }

        // add a log entry, also used to validate permission to edit note 
        var msg = (res ? ('CADD Note create success' + (noteId ? '; ID: ' + noteId : '')) : ('CADD Note create failure ' + (noteId ? 'update' : 'create') + '; ID: ' + noteId));
        var cols = {
            va_Error: !res,
            va_Warning: false,
            va_Summary: res,
            va_name: msg,
            va_Description: devNoteText,
            va_NoteId: noteId,
            va_Request: formatXml(createNoteDetail.serviceName),
            va_Query: formatXml(createNoteDetail.soapBodyInnerXml),
            va_Duration: 0,
            va_ServiceRequestId: { 'Id': serviceRequestId },
            va_Response: createNoteDetail.responseXml
        };
        var log = CrmRestKit2011.Create("va_querylog", cols);

        //if (noteId && serviceRequestId) {
        //    CrmRestKit2011.Update('va_servicerequest', serviceRequestId, { va_DevNoteId: noteId });
        //}
    }
    catch (err) {
        CloseProgress();
        alert('Run-time error while creating a note.');
        return false;
    }
    return true;
}

function getVIPApplication() {
    var vipApplication = null,
        vipContentWindow = null;

    if (window.parent && window.parent.opener && window.parent.opener.document) {
        vipContentWindow = window.parent.opener.document.getElementById('IFRAME_search').contentWindow;
        if (vipContentWindow && vipContentWindow != undefined) {
            vipApplication = vipContentWindow._extApp;
        }
    }

    return vipApplication;
}

function isEFT() {
    var va_routingnumber = Xrm.Page.getAttribute('va_routingnumber').getValue();
    var va_depositaccountnumber = Xrm.Page.getAttribute('va_depositaccountnumber').getValue();
    var va_depositbegindate = new Date(Xrm.Page.getAttribute('va_depositbegindate').getValue());
    var va_depositenddate = Xrm.Page.getAttribute('va_depositenddate').getValue();
    var today = new Date();

    if (globalcadd.ca) {
    //if (Xrm.Page.getAttribute('va_ca').getValue()) {
        if (va_routingnumber != null && va_depositaccountnumber != null && (va_routingnumber.indexOf('*') != -1 || va_depositaccountnumber.indexOf('*') != -1)) {
            //va_routingnumber = Xrm.Page.getAttribute('va_ca_r').getValue();
            //va_depositaccountnumber = Xrm.Page.getAttribute('va_ca_a').getValue();
            va_routingnumber = globalcadd.ca_r;
            va_depositaccountnumber = globalcadd.ca_a;

            var routingNumber = va_routingnumber;
            var len = routingNumber.length - 4;
            var masked = '';
            for (i = 0; i < len; i++) {
                masked += '*';
            }
            masked += routingNumber.substr(len, 4);
            routingNumber = masked;

            accountNumber = '**********';

            Xrm.Page.getAttribute('va_routingnumber').setValue(routingNumber);
            Xrm.Page.getAttribute("va_depositaccountnumber").setValue(accountNumber);
        }
        else {
            //Xrm.Page.getAttribute('va_ca').setValue(false);
            globalcadd.ca = false;
        }
    }

    if (va_depositenddate != null) {
        va_depositenddate = new Date(va_depositenddate);
    }

    if (va_depositbegindate == null) {
        return false;
    }

    if (va_routingnumber != null && va_depositaccountnumber != null && va_depositbegindate <= today && (va_depositenddate == null || va_depositenddate >= today)) {
        return true;
    }

    return false;
}

function createMatchingAddressesError(field1, field2) {
    var errorMessage = field1 + " and " + field2 + " should be the same, when Direct Deposit is not set up on this account."
    return errorMessage;
}