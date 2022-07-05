/**
* Created by VHAISDBLANCW on 2/6/2015.
*/

var _noaddress = false;

function onFormSave(context) {
    var militaryPostalTypeCodeValue = Xrm.Page.getAttribute('va_militarypostalcodevalue').setValue(null);
    var militaryPostOfficeTypeCodeValue = Xrm.Page.getAttribute('va_militarypostofficetypecodevalue').setValue(null); var veteranPartID = Xrm.Page.getAttribute('va_participantid').getValue();
    if (veteranPartID == null || veteranPartID == "") {
        alert("Veteran Participant ID has not been populated")
        context.getEventArgs().preventDefault();
        return false;
    }

    var claimantPartID = Xrm.Page.getAttribute('va_claimantparticipantid').getValue();

    var generalBenefitTypeOption = Xrm.Page.getAttribute('va_generalbenefittype').getSelectedOption();
    if (generalBenefitTypeOption == null) {
        Xrm.Page.getControl('va_generalbenefittype').setFocus();
        alert("The General Benefit Type has not been selected.")
        context.getEventArgs().preventDefault();
        return false;
    }
    else {
        var generalBenefitType = Xrm.Page.getAttribute('va_generalbenefittype').getText();
        if (((generalBenefitType == "Compensation") || (generalBenefitType == "Pension")) && (veteranPartID != claimantPartID)) { // Compensation and Pension
            Xrm.Page.getControl('va_claimantparticipantid').setFocus();
            alert("Veteran Participant ID and Claimant Participant ID must be same value for Compensation and Pension Intent to File");
            context.getEventArgs().preventDefault();
            return false;
        }

        if ((generalBenefitType == "Survivors pension or DIC") && (veteranPartID == claimantPartID)) { // Survivor
            Xrm.Page.getControl('va_claimantparticipantid').setFocus();
            alert("Veteran Participant ID and Claimant Participant ID must not be same value for Survivor Intent to File");
            context.getEventArgs().preventDefault();
            return false;
        }
    }


    var addressTypeOption = Xrm.Page.getAttribute('va_addresstype').getSelectedOption();
    if (addressTypeOption != null) {

        var addressType = Xrm.Page.getAttribute('va_addresstype').getText();
        var error = false;
        var addressError = "Please enter values for: \n";
        var address1 = Xrm.Page.getAttribute('va_veteranaddressline1').getValue();
        var city = Xrm.Page.getAttribute('va_veterancity').getValue();
        var state = Xrm.Page.getAttribute('va_veteranstate').getValue();
        var zip = Xrm.Page.getAttribute('va_veteranzip').getValue();
        var country = Xrm.Page.getAttribute('va_veterancountry').getValue();
        var va_mailingmilitarypostofficetypecode = Xrm.Page.getAttribute('va_mailingmilitarypostofficetypecode').getValue();
        var va_mailingmilitarypostaltypecode = Xrm.Page.getAttribute('va_mailingmilitarypostaltypecode').getValue();

        if ((addressType == "Domestic")) {
            if (country == null || country == "") {
                Xrm.Page.getAttribute('va_veterancountry').setValue("USA");
                SetCountryList();
            }

            var addressValidationCtxITF = new vrmContext();
            addressValidationCtxITF.addressParameters = GetUserSettingsForWebservice();

            addressValidationCtxITF.user = GetUserSettingsForWebservice();
            var addressValidationDetail = new addressValidationITF(addressValidationCtxITF);
            addressValidationDetail.executeRequest();

            Xrm.Page.getAttribute('va_addressvalidationrequest').setValue(addressValidationDetail.buildSoapEnvelope());
            Xrm.Page.getAttribute('va_addressvalidationresponse').setValue(addressValidationDetail.responseXml);

            CloseProgress();

            var $xml = $($.parseXML(addressValidationDetail.responseXml));

            if ($xml.find('return > status').text() == 'F') {
                var msg = 'The address is not recognized\nDo you want to continue anyway?';
                if (!confirm(msg)) {
                    context.getEventArgs().preventDefault();
                    return false;
                }
            }
            else if ($xml.find('return > stateProvinceResult').text() == 'C') {
                var msg = 'State and ZIP Code are an invalid combination\nThe state for this ZIP Code is: ' +
                    $xml.find('return > stateProvince').text() +
                    '.\nPlease make necessary corrections and click Save again';
                alert(msg);
                context.getEventArgs().preventDefault();
                return false;
            }
            else if ($xml.find('return > statusCode').text() == 'UnableToDPVConfirm') {
                var msg = 'The address entered is not a valid format by United States Postal Service Standards.\n\nDo you want to use the format below instead?\n';

                if ($xml.find('return > addressBlock1').text() != '') msg += '\n' + $xml.find('return > addressBlock1').text();
                if ($xml.find('return > addressBlock2').text() != '') msg += '\n' + $xml.find('return > addressBlock2').text();
                if ($xml.find('return > addressBlock3').text() != '') msg += '\n' + $xml.find('return > addressBlock3').text();
                if ($xml.find('return > addressBlock4').text() != '') msg += '\n' + $xml.find('return > addressBlock4').text();
                if ($xml.find('return > addressBlock5').text() != '') msg += '\n' + $xml.find('return > addressBlock5').text();
                if ($xml.find('return > addressBlock6').text() != '') msg += '\n' + $xml.find('return > addressBlock6').text();
                if ($xml.find('return > addressBlock7').text() != '') msg += '\n' + $xml.find('return > addressBlock7').text();
                if ($xml.find('return > addressBlock8').text() != '') msg += '\n' + $xml.find('return > addressBlock8').text();
                if ($xml.find('return > addressBlock9').text() != '') msg += '\n' + $xml.find('return > addressBlock9').text();

                msg += '\n\nIf Yes click Cancel and please replace on the form or click OK to submit the form as is';

                if (!confirm(msg)) {
                    context.getEventArgs().preventDefault();
                    return false;
                }
            }
        }
        else if ((addressType == "International")) {
            if (country == null || country == "") {
                alert('You must provide a value for Country');
                context.getEventArgs().preventDefault();
                return false;
            }
        }
        else if ((addressType == "Overseas Military")) {
            if (country == null || country == "") {
                alert('You must provide a value for Country');
                context.getEventArgs().preventDefault();
                return false;
            }
        }
        else if (addressType == "Use CORP Mailing Address" && _noaddress) {
            error = true;
            addressError = "Warning: incomplete or no address available from CORP DB"
        }

        if (error) {
            alert(addressError)
            context.getEventArgs().preventDefault();
            return false;
        }
    }
    else if (addressTypeOption == null) {
        var country = Xrm.Page.getAttribute('va_veterancountry').getValue();
        if (country != null && country == "USA") {
            setOptionSetByOptionText("va_addresstype", "Domestic");
        }
    }

    var addressType = Xrm.Page.getAttribute('va_addresstype').getText();
}

function setOptionSetByOptionText(optionsetAttribute, optionText) {
    var options = Xrm.Page.getAttribute(optionsetAttribute).getOptions();
    for (i = 0; i < options.length; i++) {
        if (options[i].text == optionText) Xrm.Page.getAttribute(optionsetAttribute).setValue(options[i].value);
    }
}


function onFormLoad() {
    environmentConfigurations.initalize();
    commonFunctions.initalize();
    ws.shareStandardData.initalize();
    ws.intentToFile.initalize();

    var vetSearchCtx = new vrmContext();
    vetSearchCtx.user = GetUserSettingsForWebservice();
    vetSearchCtx.parameters['fileNumber'] = Xrm.Page.getAttribute('va_veteranfilenumber').getValue();
    vetSearchCtx.parameters['ptcpntId'] = Xrm.Page.getAttribute('va_participantid').getValue();

    GetCountryList(vetSearchCtx);
    SetCountryList();
    CloseProgress();

    window.GeneralToolbar = new InlineToolbar("va_copyclaimantinfo");
    GeneralToolbar.AddButton("btnCopy", "Copy Veteran Info", "100%", copyClaimantInfo, null);
    var addressTypeOption = Xrm.Page.getAttribute('va_addresstype').getSelectedOption();
    var address1 = Xrm.Page.getAttribute('va_veteranaddressline1').getValue();

    var stateCode = Xrm.Page.getAttribute('statecode').getText();

    if (stateCode != 'Inactive' && Xrm.Page.getAttribute('va_addresstype').getText() != "Use CORP Mailing Address") {
        if (address1 == null || address1 == "") {
            _noaddress = true;
            alert("Warning: incomplete or no address available from CORP DB")
        }
    }

    var militaryPostalTypeCodeValue = Xrm.Page.getAttribute('va_militarypostalcodevalue').getValue();
    var militaryPostOfficeTypeCodeValue = Xrm.Page.getAttribute('va_militarypostofficetypecodevalue').getValue();
    if ((militaryPostalTypeCodeValue != null || militaryPostalTypeCodeValue != "") &&
        (militaryPostOfficeTypeCodeValue != null && militaryPostOfficeTypeCodeValue != ""
        || (addressTypeOption != null && Xrm.Page.getAttribute('va_addresstype').getText() == "Overseas Military"))) {

        setOptionSetByOptionText("va_addresstype", "Overseas Military");
        setMailingAddressVisible(false);
        setOptionSetByOptionText("va_mailingmilitarypostaltypecode", militaryPostalTypeCodeValue);
        setOptionSetByOptionText("va_mailingmilitarypostofficetypecode", militaryPostOfficeTypeCodeValue);
        Xrm.Page.getAttribute("va_veterancity").setValue(militaryPostOfficeTypeCodeValue);
        Xrm.Page.getAttribute("va_veteranstate").setValue(militaryPostalTypeCodeValue);

    } else {
        var country = Xrm.Page.getAttribute('va_veterancountry').getValue();
        if (country != null && country == "USA") {
            setOptionSetByOptionText("va_addresstype", "Domestic");
        }
        if (country != null && country != "USA") {
            setOptionSetByOptionText("va_addresstype", "International");
        }
        if (stateCode == 'Inactive' && address1 == null) {
            setOptionSetByOptionText("va_addresstype", "Use CORP Mailing Address");
        }
        setMailingAddressVisible(true);
    }

    checkAddressType();
}

function setMailingAddressVisible(isVisible) {
    var gentab = Xrm.Page.ui.tabs.get('GeneralTab');

    if (isVisible) {
        gentab.sections.get('MailingAddress').setVisible(true);
        gentab.sections.get('MilitaryAddress').setVisible(false);
    }
    else if (!isVisible) {
        gentab.sections.get('MailingAddress').setVisible(false);
        gentab.sections.get('MilitaryAddress').setVisible(true);
    }

    Xrm.Page.getControl('va_veteranzip').setVisible(true);

    var addressTypeOption = Xrm.Page.getAttribute('va_addresstype').getSelectedOption();
    if (addressTypeOption != null) {
        var addressType = Xrm.Page.getAttribute('va_addresstype').getText();
        if (addressType == "International") {
            Xrm.Page.getAttribute("va_veteranstate").setValue(null);
            Xrm.Page.getAttribute("va_veteranzip").setValue(null);
            Xrm.Page.getControl('va_veteranstate').setVisible(false);
            Xrm.Page.getControl('va_veteranzip').setVisible(false);
            Xrm.Page.getAttribute('va_mailingmilitarypostaltypecode').setValue(null);
            Xrm.Page.getAttribute('va_mailingmilitarypostofficetypecode').setValue(null);
        }
        else {
            if (addressType != "Use CORP Mailing Address") {
                Xrm.Page.getAttribute('va_veterancountry').setValue("USA");
                SetCountryList();
            }
            if (addressType != "Overseas Military") {
                Xrm.Page.getControl('va_veteranstate').setVisible(true);
                Xrm.Page.getControl('va_veteranzip').setVisible(true);
                Xrm.Page.getAttribute('va_mailingmilitarypostaltypecode').setValue(null);
                Xrm.Page.getAttribute('va_mailingmilitarypostofficetypecode').setValue(null);
            }
        }
    }
}

function checkAddressType() {
    var addressTypeOption = Xrm.Page.getAttribute('va_addresstype').getSelectedOption();
    if (addressTypeOption != null) {
        var addressType = Xrm.Page.getAttribute('va_addresstype').getText();

        Xrm.Page.getAttribute('va_veteranaddressline1').setRequiredLevel('none');
        Xrm.Page.getAttribute('va_veterancity').setRequiredLevel('none');
        Xrm.Page.getAttribute('va_veteranstate').setRequiredLevel('none');
        Xrm.Page.getAttribute('va_veteranzip').setRequiredLevel('none');
        //Xrm.Page.getAttribute('va_veterancountrylist').setRequiredLevel('none');
        Xrm.Page.getAttribute('va_mailingmilitarypostofficetypecode').setRequiredLevel('none');
        Xrm.Page.getAttribute('va_mailingmilitarypostaltypecode').setRequiredLevel('none');

        //$('#va_veterancountrylist_c').empty().append('<label  for="va_veterancountrylist">Country</label>');

        if (addressType == 'Domestic') {
            Xrm.Page.getAttribute('va_veteranaddressline1').setRequiredLevel('required');
            Xrm.Page.getAttribute('va_veterancity').setRequiredLevel('required');
            Xrm.Page.getAttribute('va_veteranstate').setRequiredLevel('required');
            Xrm.Page.getAttribute('va_veteranzip').setRequiredLevel('required');
            //Xrm.Page.getAttribute('va_veterancountrylist').setRequiredLevel('required');

            //$('#va_veterancountrylist_c').empty().append('<label  for="va_veterancountrylist">Country<img  alt="Required" src="/_imgs/imagestrips/transparent_spacer.gif?ver=821078889" class="ms-crm-ImageStrip-frm_required"/></label>');
        }
        else if ((addressType == "International")) {
            Xrm.Page.getAttribute('va_veteranaddressline1').setRequiredLevel('required');
            Xrm.Page.getAttribute('va_veterancity').setRequiredLevel('required');
            //Xrm.Page.getAttribute('va_veterancountrylist').setRequiredLevel('required');

            //$('#va_veterancountrylist_c').empty().append('<label  for="va_veterancountrylist">Country<img  alt="Required" src="/_imgs/imagestrips/transparent_spacer.gif?ver=821078889" class="ms-crm-ImageStrip-frm_required"/></label>');
        }
        else if ((addressType == "Overseas Military")) {
            Xrm.Page.getAttribute('va_veteranaddressline1').setRequiredLevel('required');
            Xrm.Page.getAttribute('va_mailingmilitarypostofficetypecode').setRequiredLevel('required');
            Xrm.Page.getAttribute('va_mailingmilitarypostaltypecode').setRequiredLevel('required');
            Xrm.Page.getAttribute('va_veteranzip').setRequiredLevel('required');

            //$('#va_veterancountrylist_c').empty().append('<label  for="va_veterancountrylist">Country<img  alt="Required" src="/_imgs/imagestrips/transparent_spacer.gif?ver=821078889" class="ms-crm-ImageStrip-frm_required"/></label>');
        }

        if (addressType == "Use CORP Mailing Address") {
            //var address1 = Xrm.Page.getAttribute('va_veteranaddressline1').getValue();
            //if (address1 == null || address1 == "") {
            //    alert("Warning: incomplete or no address available from CORP DB")
            //    return;
            //}

            Xrm.Page.getAttribute("va_veteranaddressline1").setValue(null);
            Xrm.Page.getControl('va_veteranaddressline1').setVisible(true);

            Xrm.Page.getAttribute("va_veteranaddressline2").setValue(null);
            Xrm.Page.getControl('va_veteranaddressline2').setVisible(true);

            Xrm.Page.getAttribute("va_veteranunitnumber").setValue(null);
            Xrm.Page.getControl('va_veteranunitnumber').setVisible(true);

            Xrm.Page.getAttribute("va_veterancity").setValue(null);
            Xrm.Page.getControl('va_veterancity').setVisible(true);

            Xrm.Page.getAttribute("va_veteranstate").setValue(null);
            Xrm.Page.getControl('va_veteranstate').setVisible(true);

            Xrm.Page.getAttribute("va_veteranzip").setValue(null);
            Xrm.Page.getControl('va_veteranzip').setVisible(true);

            Xrm.Page.getAttribute("va_veterancountry").setValue(null);
            Xrm.Page.getControl('va_veterancountrylist').setVisible(true);
            SetCountryList();

            Xrm.Page.getAttribute('va_mailingmilitarypostofficetypecode').setValue(null);
            //Xrm.Page.getAttribute('va_mailingmilitarypostofficetypecode').setVisible(true);

            Xrm.Page.getAttribute('va_mailingmilitarypostaltypecode').setValue(null);
            //Xrm.Page.getAttribute('va_mailingmilitarypostaltypecode').setVisible(true);

            //var gentab = Xrm.Page.ui.tabs.get('GeneralTab');
            //gentab.sections.get('MailingAddress').setVisible(true);
            //gentab.sections.get('MilitaryAddress').setVisible(true);

            $('#va_veteranaddressline1').prop('readonly', true);
            $('#va_veteranaddressline2').prop('readonly', true);
            $('#va_veteranunitnumber').prop('readonly', true);
            $('#va_veterancity').prop('readonly', true);
            $('#va_veteranstate').prop('readonly', true);
            $('#va_veteranzip').prop('readonly', true);
            $('#va_veterancountrylist').prop('disabled', true);
            $('#va_mailingmilitarypostofficetypecode').prop('readonly', true);
            $('#va_mailingmilitarypostaltypecode').prop('readonly', true);
        }
        else {
            $('#va_veteranaddressline1').prop('readonly', false);
            $('#va_veteranaddressline2').prop('readonly', false);
            $('#va_veteranunitnumber').prop('readonly', false);
            $('#va_veterancity').prop('readonly', false);
            $('#va_veteranstate').prop('readonly', false);
            $('#va_veteranzip').prop('readonly', false);
            $('#va_veterancountrylist').prop('disabled', false);
            $('#va_mailingmilitarypostofficetypecode').prop('readonly', false);
            $('#va_mailingmilitarypostaltypecode').prop('readonly', false);
        }

        if (addressType == "Overseas Military") {

            setMailingAddressVisible(false);
            var militaryPostalCodeOption = Xrm.Page.getAttribute('va_mailingmilitarypostaltypecode').getSelectedOption();
            var militaryPostalOfficeTypeCodeOption = Xrm.Page.getAttribute('va_mailingmilitarypostofficetypecode').getSelectedOption();
            if (militaryPostalCodeOption != null) {
                militaryPostalCode = Xrm.Page.getAttribute('va_mailingmilitarypostaltypecode').getText();
                Xrm.Page.getAttribute("va_veteranstate").setValue(militaryPostalCode);
            }

            if (militaryPostalOfficeTypeCodeOption != null) {
                militaryPostOfficeCode = Xrm.Page.getAttribute('va_mailingmilitarypostofficetypecode').getText();
                Xrm.Page.getAttribute("va_veterancity").setValue(militaryPostOfficeCode);
            }

            //var militaryPostalTypeCodeValue = Xrm.Page.getAttribute('va_militarypostalcodevalue').getValue();
            //var militaryPostOfficeTypeCodeValue = Xrm.Page.getAttribute('va_militarypostofficetypecodevalue').getValue();
            //Xrm.Page.getAttribute('va_militarypostalcodevalue').getValue();
            //Xrm.Page.getAttribute('va_militarypostofficetypecodevalue').getValue();
            //if ((militaryPostalTypeCodeValue != null || militaryPostalTypeCodeValue != "") &&
            //    (militaryPostOfficeTypeCodeValue != null && militaryPostOfficeTypeCodeValue != "")) {

            //    setOptionSetByOptionText("va_addresstype", "Overseas Military");                
            //    setOptionSetByOptionText("va_mailingmilitarypostaltypecode", militaryPostalTypeCodeValue);
            //    setOptionSetByOptionText("va_mailingmilitarypostofficetypecode", militaryPostOfficeTypeCodeValue);
            //    Xrm.Page.getAttribute("va_veterancity").setValue(militaryPostalTypeCodeValue);
            //    Xrm.Page.getAttribute("va_veteranstate").setValue(militaryPostOfficeTypeCodeValue);

            //}
        }
        else {
            setMailingAddressVisible(true)
        }
    }
}

function copyClaimantInfo() {
    var va_veteranssn = Xrm.Page.getAttribute('va_veteranssn').getValue();
    var va_veteranfirstname = Xrm.Page.getAttribute('va_veteranfirstname').getValue();
    var va_veteranlastname = Xrm.Page.getAttribute('va_veteranlastname').getValue();
    var va_veteranmiddleinitial = Xrm.Page.getAttribute('va_veteranmiddleinitial').getValue();

    if (va_veteranfirstname) {
        Xrm.Page.getAttribute('va_claimantfirstname').setValue(va_veteranfirstname);
    }
    if (va_veteranlastname) {
        Xrm.Page.getAttribute('va_claimantlastname').setValue(va_veteranlastname);
    }
    if (va_veteranmiddleinitial) {
        Xrm.Page.getAttribute('va_claimantmiddleinitial').setValue(va_veteranmiddleinitial);
    }
    if (va_veteranssn) {
        Xrm.Page.getAttribute('va_claimantssn').setValue(va_veteranssn);
    }
    return false;
}

function InlineToolbar(containerId) {
    var toolbar = this;
    var container = document.all[containerId];

    if (!container) {
        return alert("Toolbar Field: " + containerId + " is missing");
    }

    crmForm.all[containerId + "_c"].style.display = 'none';

    container.style.display = "none";
    container = container.parentElement;

    toolbar.AddButton = function (id, text, width, callback, imgSrc) {
        var btn = document.createElement("button");
        var btStyle = new StyleBuilder();
        btStyle.Add("font-family", "Arial");
        btStyle.Add("font-size", "12px");
        btStyle.Add("line-height", "16px");
        btStyle.Add("text-align", "center");
        btStyle.Add("cursor", "hand");
        btStyle.Add("border", "1px solid #3366CC");
        btStyle.Add("background-color", "#CEE7FF");
        btStyle.Add("background-image", "url( '/_imgs/btn_rest.gif' )");
        btStyle.Add("background-repeat", "repeat-x");
        btStyle.Add("padding-left", "5px");
        btStyle.Add("padding-right", "5px");
        btStyle.Add("overflow", "visible");
        btStyle.Add("width", width);

        btn.style.cssText = btStyle.ToString();
        btn.attachEvent("onclick", callback);
        btn.id = id;

        if (imgSrc) {
            var img = document.createElement("img");
            img.src = imgSrc;
            img.style.verticalAlign = "middle";
            btn.appendChild(img);
            btn.appendChild(document.createTextNode(" "));
            var spn = document.createElement("span");
            spn.innerText = text;
            btn.appendChild(spn);
        }
        else {
            btn.innerText = text;
        }

        container.appendChild(btn);
        container.appendChild(document.createTextNode(" "));

        return btn;
    }

    toolbar.RemoveButton = function (id) {
        var btn = toolbar.GetButton(id)
        if (btn) {
            btn.parentNode.removeChild(btn);
        }
    }

    toolbar.GetButton = function (id) {
        return document.getElementById(id);
    }

    function StyleBuilder() {
        var cssText = new StringBuilder();
        this.Add = function (key, value) {
            cssText.Append(key).Append(":").Append(value).Append(";");
        }
        this.ToString = function () {
            return cssText.ToString();
        }
    }

    function StringBuilder() {
        var parts = [];
        this.Append = function (text) {
            parts[parts.length] = text;
            return this;
        }
        this.Reset = function () {
            parts = [];
        }
        this.ToString = function () {
            return parts.join("");
        }
    }
}

function GetCountryList(vetSearchCtx) {

    var getCountryList = new findCountries(vetSearchCtx);
    getCountryList.executeRequest();

    CountryList_xmlObject = _XML_UTIL.parseXmlObject(getCountryList.responseXml);
    returnNode = CountryList_xmlObject.selectNodes('//return');
    CountryListNodes = returnNode[0].childNodes;
    var oOption = document.createElement("option");;

    if (CountryListNodes) {
        for (var i = 0; i < CountryListNodes.length; i++) {         //looping through countries and
            if (CountryListNodes[i].nodeName == 'types') {  //making sure we dont check irrelevant nodes

                oOption.value = parseInt(CountryListNodes[i].selectSingleNode('code').text);
                oOption.text = CountryListNodes[i].selectSingleNode('name').text;

                if (oOption.text == 'Israel (Jerusalem)' || oOption.text == 'Turkey (Adana only)' || oOption.text == 'Philippines (restricted payments)') {
                    continue;
                }
                else if (oOption.text == 'Israel (Tel Aviv)') {
                    oOption.text = 'Israel';
                }
                else if (oOption.text == 'Turkey (except Adana)') {
                    oOption.text = 'Turkey';
                }

                Xrm.Page.getControl('va_veterancountrylist').addOption(oOption);
            }
        }
    }
}

function SetCountryList() {
    var x, optionText, countryText;
    countryText = Xrm.Page.getAttribute("va_veterancountry").getValue();
    if (countryText != null && countryText != '') {
        for (var k = 0; k < document.getElementById("va_veterancountrylist").length; k++) {
            x = document.getElementById("va_veterancountrylist").children[k];
            optionText = x.text;
            optionText = optionText.toUpperCase();
            countryText = countryText.toUpperCase();
            if (optionText == countryText) {
                x.setAttribute("selected", "selected");
            }
        }
    }
    else {
        x = document.getElementById("va_veterancountrylist").children[0];
        x.setAttribute("selected", "selected");
    }
}

function onChange_VeteranCountryList() {
    var country = $("#va_veterancountrylist option:selected");
    if (country != null && country.text()) {
        Xrm.Page.getAttribute("va_veterancountry").setValue(country.text());
    }
    else {
        Xrm.Page.getAttribute("va_veterancountry").setValue(null);
    }
    SetCountryList();
}
