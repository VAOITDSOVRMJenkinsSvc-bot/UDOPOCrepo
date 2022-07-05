/// <reference path="../Intellisense/XrmPage-vsdoc.js" />
var exCon;
var formContext;
// This method will be call from CRM form
function OnLoad(execCon) {
    exCon = execCon
    formContext = exCon.getFormContext();
    environmentConfigurations.initalize(exCon);
    commonFunctions.initalize(exCon);
    ws.vetRecord.initalize();
    ws.claimant.initalize();
    ws.benefitClaim.initalize();
    ws.appeals.initalize();
    ws.mapDDevelopmentNotes.initalize(exCon);
    ws.shareStandardData.initalize();
    ws.addressValidation.initalize();
    // RU12 Form
    onFormLoad();
    Xrm.Page.getControl('va_findawardaddressesresponse').setDisabled(false);
    Xrm.Page.getControl('va_findveteranbyptcpntidresponse').setDisabled(false);
    Xrm.Page.getControl('va_updateaddressrequest').setDisabled(false);
    Xrm.Page.getControl('va_updateaddressresponse').setDisabled(false);
    Xrm.Page.getControl('va_appellantaddressresponse').setDisabled(false);
    Xrm.Page.getControl('va_updateappellantaddressresponse').setDisabled(false);
    Xrm.Page.getControl('va_validateaddressresponse').setDisabled(false);
}
//Global Variables
var globalcadd = {};
globalcadd.bankdata = "";
globalcadd.cadddata = "";
globalcadd.idproof = "";
globalcadd.ca = false;
globalcadd.ca_a = "";
globalcadd.ca_r = "";
globalcadd.ca_t = "";
_changingAwardAddress = true;
_fromClaims = false;
_hasAppeals = false;
_participantId = '';

var awardAddresses_xmlObject = null;
var selectedAward_xmlObject = null;
var CountryList_xmlObject = null;
var vetFileNumber = null;
var vetSSN = null;
var veteranInformation_xml = null;
var veteranInformation_xmlObject = null;
var parent_page = null;

function onFormLoad() {
    _isLoading = true;
    _errorDuringLoad = false;
    _viewAddressOnly = false;

    window.GeneralToolbar = new InlineToolbar("va_suffix"); GeneralToolbar.AddButton("btnCP", "C & P", "100%", CPLink, null);
    window.GeneralToolbar = new InlineToolbar("va_firstname"); GeneralToolbar.AddButton("btnEd", "Education", "100%", EdLink, null);
    window.GeneralToolbar = new InlineToolbar("va_idproofingcompleteforcadd"); GeneralToolbar.AddButton("btnSuper", "ID Proofing Complete for CADD", "100%", CADDIdProofingComplete, null);

    var webResourceUrl = parent.Xrm.Page.context.getClientUrl() + '/WebResources/va_';
    window.GeneralToolbar = new InlineToolbar("va_middlename"); GeneralToolbar.AddButton("btnScript", "View Script", "100%", ViewScript, webResourceUrl + 'status_online.png');

    var parent_page = null;
    var opener = null;


    //VTRIGILI - 2015-01-24 - add Try/Catch to handle IE9 failure to parse the if correctly 
    try {
        if (window.parent && window.parent.opener && window.parent.opener.Xrm.Page) {
            parent_page = window.parent.opener.Xrm.Page;
            opener = window.parent.opener;
            if (!parent_page.data && window.top && window.top.opener && window.top.opener.parent) {
                parent_page = window.top.opener.parent.Xrm.Page;
                opener = window.top.opener;
    }
        }
    } catch (err) {
        //The expected function of the IF is that nothing happens if 
        //the IF evalutes to false, as it will in IE 10/11. So the 
        //empty catch replicates that in IE9
    }

    if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE) {
        vetFileNumber = null;
        vetSSN = null;

        Xrm.Page.getAttribute('va_milzipcodelookupid').addOnChange(MilZipCodeChange);

        try {
            if (opener._changeOfAddressData && opener._changeOfAddressData.openedFromClaimTab == undefined) opener._changeOfAddressData.openedFromClaimTab = false;

            if (parent_page) {
                var vetId = null;
                //if (parent_page.getAttribute("regardingobjectid").getValue()) vetId = parent_page.getAttribute("regardingobjectid").getValue()[0].id;
                //VTRIGILI 2014-12-30 Adjust for aditional entities calling 
                if (parent_page.getAttribute("va_vetcontactid") != null) {
                    vetId = parent_page.getAttribute("va_vetcontactid").getValue();
                } else if (parent_page.getAttribute("regardingobjectid") != null && parent_page.getAttribute("regardingobjectid").getValue() != null) {
                    vetId = parent_page.getAttribute("regardingobjectid").getValue()[0].id;
                }


                if (vetId && !Xrm.Page.getAttribute("va_accountownerid").getValue()) {
                    Xrm.Page.getAttribute('va_accountownerid').setValue([{ id: vetId, name: parent_page.getAttribute("regardingobjectid").getValue()[0].name, entityType: 'contact' }]);
                    Xrm.Page.getAttribute('va_accountownerid').setSubmitMode('always');
                }

                veteranInformation_xml = parent_page.getAttribute('va_findcorprecordresponse').getValue();
                if (veteranInformation_xml != null) { veteranInformation_xmlObject = _XML_UTIL.parseXmlObject(veteranInformation_xml); }

                vetFileNumber = SingleNodeExists(veteranInformation_xmlObject, '//return/fileNumber')
					? veteranInformation_xmlObject.selectSingleNode('//return/fileNumber').text : null;

                if (vetFileNumber == null) {
                    vetFileNumber = SingleNodeExists(veteranInformation_xmlObject, '//return/vetCorpRecord/fileNumber')
                        ? veteranInformation_xmlObject.selectSingleNode('//return/vetCorpRecord/fileNumber').text : null;
                }

                vetSSN = SingleNodeExists(veteranInformation_xmlObject, '//return/ssn')
					? veteranInformation_xmlObject.selectSingleNode('//return/ssn').text : null;

                //CADD ID PROTOCOL fields
                //Addition of new DOB Text Field:  RTC 108417 - Handle Birth Dates before 1900
                var idDOBText = parent_page.getAttribute('va_dobtext').getValue();
                if (idDOBText) {
                    Xrm.Page.getAttribute('va_dateofbirthtext').setValue(idDOBText);
                    Xrm.Page.getAttribute('va_dateofbirthtext').setSubmitMode('always');
                }

                //Populate CADD ID Proofing fields: Latest Pay Date and Net Payment
                //Do we need to populate Gross Payment field?
                pay_xml = parent_page.getAttribute('va_retrievepaymentsummaryresponse').getValue();
                if (pay_xml != null) {
                    pay_xmlObject = _XML_UTIL.parseXmlObject(pay_xml);
                    var payNode = pay_xmlObject.selectSingleNode('//PaymentSummaryResponse/payments');
                    if ((payNode) && (payNode.hasChildNodes)) {
                        for (var i = 0; i < payNode.childNodes.length; i++) {
                            if (payNode.childNodes[i].selectSingleNode('paymentDate') != null) {
                                var latestpaydate = payNode.childNodes[i].selectSingleNode('paymentDate').text;
                                var latestPayAmount = payNode.childNodes[i].selectSingleNode('paymentAmount').text;

                                Xrm.Page.getAttribute('va_netamount').setValue(parseFloat(latestPayAmount));

                                if (latestpaydate.indexOf('T') > 0) {
                                    var payArray = latestpaydate.split('T');
                                    var payArray2 = payArray[0].split('-');
                                    var varDate = payArray2[1] + "/" + payArray2[2] + "/" + payArray2[0];
                                    latestPayDate = new Date(varDate);
                                    Xrm.Page.getAttribute('va_paydate').setValue(latestPayDate);
                                    Xrm.Page.getAttribute('va_paydate').setSubmitMode('always');

                                    Xrm.Page.getAttribute('va_account').setValue(payNode.childNodes[i].selectSingleNode('addressEFT/accountNumber').text);
                                    Xrm.Page.getAttribute('va_account').setSubmitMode('always');
                                }
                                break;
                            }
                        }
                    }
                }
            }

            if (!parent_page || !vetFileNumber || !opener._changeOfAddressData
				|| (opener._changeOfAddressData.openedFromClaimTab == undefined)) {
                Xrm.Page.ui.tabs.get('General').setVisible(false);
                Xrm.Page.ui.tabs.get('Notes').setVisible(false);
                Xrm.Page.ui.tabs.get('tab_3').setVisible(false);
                throw "Change of Address and Bank Account screen can only be opened from Awards, Appeals or Claims tab after successful Veteran Search.";
            }

            if (opener._changeOfAddressData.openedFromClaimTab) {
                _changingAwardAddress = true;
            }
            else {
                // awardTypeCd is in context if coming from Award; not if from Appeals or other place
                _changingAwardAddress = (opener._changeOfAddressData && opener._changeOfAddressData.awardTypeCd && opener._changeOfAddressData.awardTypeCd.length > 0);
            }
            _fromClaims = opener._changeOfAddressData.openedFromClaimTab;

            var webResourceUrl = parent.Xrm.Page.context.getClientUrl() + '/WebResources/va_';

            Xrm.Page.getAttribute('va_webserviceresponse').setSubmitMode('always');
            Xrm.Page.getAttribute('va_findawardaddressesresponse').setSubmitMode('always');
            Xrm.Page.getAttribute('va_updateaddressresponse').setSubmitMode('always');
            Xrm.Page.getAttribute('va_openedfromclaimtab').setSubmitMode('always');

            if (_changingAwardAddress) {
                window.GeneralToolbar = new InlineToolbar("va_payment");
                GeneralToolbar.AddButton("btnAddCopy", "Copy Mailing Address to Payment", "100%", CopyAddress, webResourceUrl + 'comment_edit.png');

                window.GeneralToolbar = new InlineToolbar("va_validatefields");
                GeneralToolbar.AddButton("btnValidate", "Validate Address Fields", "100%", ValidateMailingAddress, webResourceUrl + 'comment_edit.png');

                window.GeneralToolbar = new InlineToolbar("va_validatepaymentfields");
                GeneralToolbar.AddButton("btnValidatePayment", "Validate Payment Address Fields", "100%", ValidatePaymentAddress, webResourceUrl + 'comment_edit.png');
            }
            else {
                // hide award address sections
                Xrm.Page.ui.tabs.get('General').sections.get('deposit').setVisible(false);
                Xrm.Page.ui.tabs.get('General').sections.get('mailing_address').setVisible(false);
                Xrm.Page.ui.tabs.get('General').sections.get('General_section_17').setVisible(false);
                Xrm.Page.ui.tabs.get('General').sections.get('General_section_18').setVisible(false);
                Xrm.Page.ui.tabs.get('General').sections.get('CountryFields').setVisible(false); // renamed to "CountryFields" from "General_section_19"
                Xrm.Page.ui.tabs.get('General').sections.get('MilitaryFields').setVisible(false); // renamed to "MilitaryFields" from "General_section_20"
                Xrm.Page.ui.tabs.get('General').sections.get('adcopy').setVisible(false);
                Xrm.Page.ui.tabs.get('General').sections.get('payment_address').setVisible(false);
                Xrm.Page.ui.tabs.get('General').sections.get('General_section_13').setVisible(false);
                Xrm.Page.ui.tabs.get('General').sections.get('General_section_14').setVisible(false);
                Xrm.Page.ui.tabs.get('General').sections.get('General_section_16').setVisible(false);
                Xrm.Page.ui.tabs.get('General').sections.get('name_section').setVisible(false);
                Xrm.Page.ui.tabs.get('General').sections.get('vacols').setVisible(false);
            }

            var parentId = parent_page.data.entity.getId();
            var parentName = parent_page.data.entity.getEntityName();
            if (parentId && parentName == 'phonecall') {
                Xrm.Page.getAttribute('va_originatingphonecallid').setValue([{
                    id: parentId,
                    name: parent_page.getAttribute('subject').getValue(), entityType: parentName
                }]);
                Xrm.Page.getAttribute('va_originatingphonecallid').setSubmitMode('always');
            }

            //Create ActionContext
            Xrm.Page.getAttribute('va_filenumber').setValue(vetFileNumber);
            Xrm.Page.getAttribute('va_apellantssn').setValue(vetSSN);

            _viewAddressOnly = opener._changeOfAddressData.ro;

            var vetSearchCtx = new vrmContext(exCon);
            _UserSettings = GetUserSettingsForWebservice(exCon);
            vetSearchCtx.user = _UserSettings;
            vetSearchCtx.parameters['fileNumber'] = Xrm.Page.getAttribute('va_filenumber').getValue();

            var participantIDParameter = '';
            if (opener._changeOfAddressData.participantId || !opener._changeOfAddressData.participantId == undefined) {
                participantIDParameter = opener._changeOfAddressData.participantId;
            }
            else {
                participantIDParameter = opener._changeOfAddressData.ptcpntVetID;
            }

            var ptcpntIdSearchCtx = new vrmContext(exCon);
            ptcpntIdSearchCtx.user = _UserSettings;
            ptcpntIdSearchCtx.parameters['ptcpntId'] = participantIDParameter;

            Xrm.Page.ui.tabs.get('General').sections.get('vacols').setVisible(true);
            Xrm.Page.ui.tabs.get('General').sections.get('vacols2').setVisible(true);

            Xrm.Page.data.entity.attributes.get('va_findappealsby').addOnChange(function () {
                var userSelection = Xrm.Page.getAttribute("va_findappealsby").getValue();
                //'These Values' == 953850002
                Xrm.Page.ui.controls.get('va_appealsssn').setVisible((userSelection == 953850002));
                Xrm.Page.ui.controls.get('va_appealslastname').setVisible((userSelection == 953850002));
                Xrm.Page.ui.controls.get('va_appealsfirstname').setVisible((userSelection == 953850002));
            });

            _participantId = opener._changeOfAddressData.participantId;
            retrieveAppellantAddress();
            if (opener._changeOfAddressData.appealsOnly == true) {
                RetrievePersonalInfo(ptcpntIdSearchCtx);
            }
            CloseProgress();

            if (!_hasAppeals) {
                Xrm.Page.ui.tabs.get('General').sections.get('vacols').setLabel('Appeals address information does not exist for ' + vetFileNumber);
            }

            //VTRIGILI - 2015-05-18 - Gave unquie name to button
            window.GeneralToolbar = new InlineToolbar("va_type");
            GeneralToolbar.AddButton("btnFindBank", "Find Bank", "100%", va_routingnumberChange, webResourceUrl + 'money_add.png');

            window.GeneralToolbar = new InlineToolbar("va_copyvacols");
            GeneralToolbar.AddButton("btnAddCopy2", "Copy Mailing Address to Appellant", "100%", CopyAddress2, webResourceUrl + 'comment_edit.png');

            window.GeneralToolbar = new InlineToolbar("va_searchappellantaddress");
            GeneralToolbar.AddButton("btnSearchAppellantAddress", "Search Appellant Address", "100%", retrieveAppellantAddress, webResourceUrl + 'find.png');

            //Only if opened from Award, otherwise Mailing section is hidden
            //if (_changingAwardAddress) Xrm.Page.getControl('va_copyvacols').setVisible(false);

            if (_changingAwardAddress) {
                if (opener._changeOfAddressData.openedFromClaimTab) {
                    vetSearchCtx.parameters['ptcpntId'] = opener._changeOfAddressData.participantClaimantID;
                    Xrm.Page.getAttribute('va_openedfromclaimtab').setValue(true);
                } else {
                    vetSearchCtx.parameters['ptcpntId'] = opener._changeOfAddressData.ptcpntRecipID;
                    Xrm.Page.getAttribute('va_openedfromclaimtab').setValue(false);
                }

                //if (Xrm.Page.getAttribute('va_openedfromclaimtab').getValue()) {
                //    $('#va_routingnumber').prop('readonly', true);
                //    $('#va_depositaccountnumber').prop('readonly', true);
                //}

                vetSearchCtx.ignoreBirlsError = true;
                if (!vetSearchCtx.parameters['ptcpntId'] || vetSearchCtx.parameters['ptcpntId'].length == 0) {
                    Xrm.Page.ui.tabs.get('General').setVisible(false);
                    Xrm.Page.ui.tabs.get('Notes').setVisible(false);
                    Xrm.Page.ui.tabs.get('tab_3').setVisible(false);
                    throw "Participant ID field is blank. Please execute Search on the parent screen and ensure that Participant ID is filled.";
                }

                var fn = null, ln = null, mi = null, em = null, suf = '';

                RetrievePersonalInfo(vetSearchCtx);

                if (!opener._changeOfAddressData.openedFromClaimTab) {
                    RetrieveAwardAddresses(vetSearchCtx, opener);
                }

                Xrm.Page.getAttribute('va_mailingcountry').addOnChange(va_mailingcountryChange);

                va_routingnumberChange();
                va_mailingcountryChange();

                GetCountryList(vetSearchCtx);

                Xrm.Page.getAttribute("va_mailingcountry").setSubmitMode('always');

                onMailingCountryListChange = function () {
                    var varMyValue = $("#va_mailingcountrylist option:selected");
                    if (varMyValue != null) {
                        Xrm.Page.getAttribute("va_mailingcountry").setValue(varMyValue.text());
                        Xrm.Page.getAttribute("va_mailingforeignpostalcode").setValue(varMyValue.val());
                    }
                };

                onPaymentCountryListChange = function () {
                    var varMyValue = $("#va_paymentcountrylist option:selected");
                    if (varMyValue != null) {
                        Xrm.Page.getAttribute("va_paymentcountry").setValue(varMyValue.text());
                        // RTC 238877
                        Xrm.Page.getAttribute("va_paymentforeignpostalcode").setValue(varMyValue.val());
                    }
                };

                Xrm.Page.getAttribute('va_mailingcountrylist').addOnChange(onMailingCountryListChange);
                Xrm.Page.getAttribute('va_paymentcountrylist').addOnChange(onPaymentCountryListChange);
            }

            if (_viewAddressOnly) DisableAll();
            if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE && opener != null && parent_page != null)
                setRoutingAndAccountNumber(opener, parent_page);
        }

        catch (err) {
            _errorDuringLoad = true;
            CloseProgress();

            DisableAll();

            if (err.Description) {
                alert("Invalid Operation.\n" + err.Description);
                Xrm.Page.getAttribute('va_requeststatus').setValue(err.Description);
            } else {
                alert("Invalid Operation.\n" + err);
                Xrm.Page.getAttribute('va_requeststatus').setValue(err);
            }
            Xrm.Page.ui.tabs.get('General').sections.get('execution_results').setVisible(true);
        }
        finally {
            CloseProgress();
        }
    }
    else {
        DisableAll();
        //VTRIGILI 2015-01-29 - This code removed as it was tripping the DIRTY flag in CRM2013 and would not allow the form to be closed with out error
        //Xrm.Page.ui.tabs.get('General').sections.get('execution_results').setVisible(true);
        //if (Xrm.Page.getAttribute('va_requeststatus').getValue() == 'Successful on Corporate.') Xrm.Page.ui.tabs.get('General').sections.get('execution_results').setLabel('Address/Bank Information had been successfully updated');
        //Xrm.Page.ui.tabs.get('General').sections.get('adcopy').setVisible(false);
        return;
    }

    setHeaderData(parent_page);

    SetMandatoryFields();

    // RTC 238877
    var x, optionText, countryText;
    countryText = Xrm.Page.getAttribute("va_mailingcountry").getValue();
    if (countryText != null) {
        for (var k = 0; k < document.getElementById("va_mailingcountrylist").length; k++) {
            x = document.getElementById("va_mailingcountrylist").children[k];
            optionText = x.text;
            optionText = optionText.toUpperCase();
            countryText = countryText.toUpperCase();
            if (optionText == countryText) {
                x.setAttribute("selected", "selected");
            }
        }
    }

    countryText = Xrm.Page.getAttribute("va_paymentcountry").getValue();
    if (countryText != null) {
        for (var k = 0; k < document.getElementById("va_paymentcountrylist").length; k++) {
            x = document.getElementById("va_paymentcountrylist").children[k];
            optionText = x.text;
            optionText = optionText.toUpperCase();
            countryText = countryText.toUpperCase();
            if (optionText == countryText) {
                x.setAttribute("selected", "selected");
            }
        }
    }

    _isLoading = false;
}

function DisableAll() {
    Xrm.Page.getControl('va_type').setVisible(false);
    Xrm.Page.ui.controls.forEach(function (control, index) {
        control.setDisabled(true);
    });

}

function MailingState_Onchange() {
    var option = Xrm.Page.getAttribute("va_mailingstateoptionset").getSelectedOption(); var val = '';
    if (option && option != undefined) {
        val = option.text;
    }
    Xrm.Page.getAttribute("va_mailingstate").setValue(val);
}

function AppellantState_Onchange() {
    var option = Xrm.Page.getAttribute("va_appellantstateoptionset").getSelectedOption(); var val = '';
    if (option != undefined && option) {
        val = option.text;
        Xrm.Page.getAttribute("va_apellantstate").setValue(option.text);
    }
    else {
        Xrm.Page.getAttribute("va_apellantstate").setValue(null);
    }
}

function PaymentState_Onchange() {
    var option = Xrm.Page.getAttribute("va_paymentstateoptionset").getSelectedOption(); var val = '';
    if (option != undefined && option) {
        val = option.text;
        Xrm.Page.getAttribute("va_paymentstate").setValue(option.text);
    }
    else {
        Xrm.Page.getAttribute("va_paymentstate").setValue(null);
    }
}

function GetOptionValue(optionsetSchema, labelText) {
    var optionList = Xrm.Page.getAttribute(optionsetSchema).getOptions();
    var optionValue;
    for (var o in optionList) {
        if (optionList[o].text == labelText) {
            optionValue = optionList[o].value;
            break;
        }
    }
    return optionValue;
}

function CopyAddress() {
    if (confirm('Please confirm that you would like to copy mailing address to the payment address section below.')) {
        if (ValidateFields()) {
            //            Xrm.Page.getAttribute('va_paymentaddressexistsindicator').setValue(true);
            Xrm.Page.getAttribute('va_paymentaddress1').setValue(Xrm.Page.getAttribute('va_mailingaddress1').getValue());
            Xrm.Page.getAttribute('va_paymentaddress2').setValue(Xrm.Page.getAttribute('va_mailingaddress2').getValue());
            Xrm.Page.getAttribute('va_paymentaddress3').setValue(Xrm.Page.getAttribute('va_mailingaddress3').getValue());
            Xrm.Page.getAttribute('va_paymentcity').setValue(Xrm.Page.getAttribute('va_mailingcity').getValue());
            Xrm.Page.getAttribute('va_paymentstate').setValue(Xrm.Page.getAttribute('va_mailingstate').getValue());
            Xrm.Page.getAttribute('va_paymentstateoptionset').setValue(Xrm.Page.getAttribute('va_mailingstateoptionset').getValue());
            Xrm.Page.getAttribute('va_paymentzipcode').setValue(Xrm.Page.getAttribute('va_mailingaddresszipcode').getValue());
            Xrm.Page.getAttribute('va_paymentcountry').setValue(Xrm.Page.getAttribute('va_mailingcountry').getValue());

            // RTC 238877
            var x, optionText, countryText;
            countryText = Xrm.Page.getAttribute("va_mailingcountry").getValue();
            if (countryText != null) {
                for (var k = 0; k < document.getElementById("va_paymentcountrylist").length; k++) {
                    x = document.getElementById("va_paymentcountrylist").children[k];
                    optionText = x.text;
                    optionText = optionText.toUpperCase();
                    countryText = countryText.toUpperCase();
                    if (optionText == countryText) {
                        x.setAttribute("selected", "selected");
                    }
                }
            }

            Xrm.Page.getAttribute('va_paymentforeignpostalcode').setValue(Xrm.Page.getAttribute('va_mailingforeignpostalcode').getValue());
            Xrm.Page.getAttribute('va_paymentaddresschanged').setValue(1);

            //Defect 224151
            Xrm.Page.getAttribute('va_paymentmilitarypostaltypecode').setValue(Xrm.Page.getAttribute('va_mailingmilitarypostaltypecode').getValue());
            Xrm.Page.getAttribute('va_paymentmilitarypostofficetypecode').setValue(Xrm.Page.getAttribute('va_mailingmilitarypostofficetypecode').getValue());

        }
    }
}

function CopyAddress2() {
    if (confirm('Please confirm that you would like to copy mailing address to the appellant address section below. \n\nNote: Mailing Address 2 and Mailing Address 3 will combine into Appellant Address 2. \n(20 character limit enforced.)')) {
        if (ValidateFields()) {
            Xrm.Page.getAttribute('va_apellantaddress1').setValue(Xrm.Page.getAttribute('va_mailingaddress1').getValue());

            var Add2combine = ((Xrm.Page.getAttribute('va_mailingaddress2').getValue() != null) ? (Xrm.Page.getAttribute('va_mailingaddress2').getValue() + ' ') : '') + ((Xrm.Page.getAttribute('va_mailingaddress3').getValue() != null) ? (Xrm.Page.getAttribute('va_mailingaddress3').getValue()) : '');

            if (Add2combine.length >= 20) {
                Add2combine = Add2combine.trim();
                Add2combine = Add2combine.slice(0, 20).trim();
            }
            Xrm.Page.getAttribute('va_apellantaddress2').setValue(Add2combine);
            Xrm.Page.getAttribute('va_apellantcity').setValue(Xrm.Page.getAttribute('va_mailingcity').getValue());
            Xrm.Page.getAttribute('va_apellantstate').setValue(Xrm.Page.getAttribute('va_mailingstate').getValue());
            Xrm.Page.getAttribute('va_appellantstateoptionset').setValue(Xrm.Page.getAttribute('va_mailingstateoptionset').getValue());
            Xrm.Page.getAttribute('va_apellantzipcode').setValue(Xrm.Page.getAttribute('va_mailingaddresszipcode').getValue());
            Xrm.Page.getAttribute('va_apellantcountry').setValue(Xrm.Page.getAttribute('va_mailingcountry').getValue());
            Xrm.Page.getAttribute('va_apellanthomephone').setValue(Xrm.Page.getAttribute('va_caddphone1').getValue());
            Xrm.Page.getAttribute('va_apellantworkphone').setValue(Xrm.Page.getAttribute('va_caddphone2').getValue());
            Xrm.Page.getAttribute('va_appellantaddresschanged').setValue(1);
        }
    }
}

function va_mailingcountryChange() {
    var countryName = Xrm.Page.getAttribute('va_mailingcountry').getValue();

    /*
    //OVERSEAS MILITARY, country is blank
    if (countryName == null || countryName == '') {
        Xrm.Page.getAttribute('va_addresstype').setValue(953850002);
    }
    else {
        countryName = countryName.toUpperCase();
        //DOMESTIC, country is USA
        if (countryName == 'US' || countryName == 'USA' || countryName == 'U.S.A.' || countryName == 'UNITED STATES' || countryName == 'UNITED STATES OF AMERICA') {
            Xrm.Page.getAttribute('va_addresstype').setValue(953850000);
        }
            //INTERNATIONAL, country is not blank and not USA
        else {
            Xrm.Page.getAttribute('va_addresstype').setValue(953850001);
        }
    }
    */

    var PostOfficeTypeCode = Xrm.Page.getAttribute('va_mailingmilitarypostofficetypecode').getValue();
    var PostalTypeCode = Xrm.Page.getAttribute('va_mailingmilitarypostaltypecode').getValue();

    if ((countryName == 'US' || countryName == 'USA' || countryName == 'U.S.A.' || countryName == 'UNITED STATES' || countryName == 'UNITED STATES OF AMERICA')
        && (PostOfficeTypeCode == null || PostOfficeTypeCode == '') && (PostalTypeCode == null || PostalTypeCode == '')) {
        //DOMESTIC, country is USA
        Xrm.Page.getAttribute('va_addresstype').setValue(953850000);
    }
    else if ((PostOfficeTypeCode != null && PostOfficeTypeCode != '') && (PostalTypeCode != null && PostalTypeCode != '')) {
        //OVERSEAS MILITARY, country is blank
        Xrm.Page.getAttribute('va_addresstype').setValue(953850002);
    }
    else {
        //INTERNATIONAL, country is not blank and not USA
        Xrm.Page.getAttribute('va_addresstype').setValue(953850001);
    }

    //Change required fields
    SetMandatoryFields();
}

function va_routingnumberChange() {
    var no = Xrm.Page.getAttribute('va_routingnumber').getValue();

    if ((no != null) && (no.indexOf('*') != -1)) {
        no = globalcadd.ca_r;
    }

    var bankInfo = '';
    if (no && no.length > 0) {
        var columns = ['va_Address', 'va_City', 'va_CustomerName', 'va_State', 'va_TelephoneAreaCode', 'va_TelephonePrefixNumber', 'va_TelephoneSuffixNumber'];
        var filter = "va_name eq '" + no.toString() + "'"; // or va_NewRoutingNumber eq '" +  + "')";

        CrmRestKit2011.ByQuery('va_bankandroutingnolookup', columns, filter)
        .fail(function (err) {
            if (!_isLoading && confirm('Failed to locate Bank Information.\nWould you like to open Greg Thatcher web site and locate the information manually?')) {
                var width = 1024;
                var height = 768;
                var top = (screen.height - height) / 2;
                var left = (screen.width - width) / 2;
                var params = "width=" + width + ",height=" + height + ",location=0,menubar=0,toolbar=0,top=" + top + ",left=" + left + ",status=0,titlebar=no,resizable=yes";
                var win = window.open('http://www.gregthatcher.com/Financial/Default.aspx', 'GregThatcher', params);
            }
        }).done(function (data) {
            if (data && data.d.results && data.d.results.length > 0 && data.d.results[0].va_CustomerName) {
                bankInfo = data.d.results[0].va_CustomerName +
                                (data.d.results[0].va_Address ? '; ' + data.d.results[0].va_Address : '') +
                                (data.d.results[0].va_City ? '; ' + data.d.results[0].va_City + (data.d.results[0].va_State ? ', ' + data.d.results[0].va_State : '') : '') +
                                (data.d.results[0].va_TelephoneAreaCode ? '; ' + data.d.results[0].va_TelephoneAreaCode +
                                    (data.d.results[0].va_TelephonePrefixNumber ? '-' + data.d.results[0].va_TelephonePrefixNumber : '') +
                                    (data.d.results[0].va_TelephoneSuffixNumber ? '-' + data.d.results[0].va_TelephoneSuffixNumber : '')
                                : '');
                Xrm.Page.getAttribute('va_bankname').setValue(bankInfo);
            }
                //else if (!_isLoading && confirm('Failed to locate Bank Information based on provided Routing Number.\nWould you like to open Greg Thatcher web site and locate the information manually?')) {
            else if (!_isLoading && !globalcadd.ca && confirm('Failed to locate Bank Information based on provided Routing Number.\nWould you like to open Greg Thatcher web site and locate the information manually?')) {
                var width = 1024;
                var height = 768;
                var top = (screen.height - height) / 2;
                var left = (screen.width - width) / 2;
                var params = "width=" + width + ",height=" + height + ",location=0,menubar=0,toolbar=0,top=" + top + ",left=" + left + ",status=0,titlebar=no,resizable=yes";
                var win = window.open('http://www.gregthatcher.com/Financial/Default.aspx', 'GregThatcher', params);
            }
        });
    }
}

function SetMandatoryFields() {
    var optionValue = Xrm.Page.getAttribute("va_addresstype").getValue();
    switch (optionValue) {
        case 953850000: //Domestic
            Xrm.Page.getAttribute("va_mailingaddress1").setRequiredLevel("required");
            Xrm.Page.getAttribute("va_mailingcity").setRequiredLevel("required");
            //      Xrm.Page.getAttribute("va_mailingstate").setRequiredLevel("required"); 
            Xrm.Page.getAttribute("va_mailingaddresszipcode").setRequiredLevel("required");
            Xrm.Page.getAttribute("va_mailingcountry").setRequiredLevel("none");
            Xrm.Page.getAttribute("va_mailingmilitarypostofficetypecode").setRequiredLevel("none");
            Xrm.Page.getAttribute("va_mailingmilitarypostaltypecode").setRequiredLevel("none");
            Xrm.Page.getControl('va_milzipcodelookupid').setVisible(false);
            Xrm.Page.getControl("va_mailingcity").setDisabled(false);
            Xrm.Page.getControl("va_mailingstateoptionset").setDisabled(false);

            // RTC 238877
            Xrm.Page.getControl('va_mailingcountry').setVisible(false);
            Xrm.Page.getAttribute("va_mailingcountrylist").setRequiredLevel("none");
            Xrm.Page.getAttribute("va_mailingstateoptionset").setRequiredLevel("required");

            Xrm.Page.getAttribute("va_mailingmilitarypostofficetypecode").setValue(null);
            Xrm.Page.getAttribute("va_mailingmilitarypostaltypecode").setValue(null);
            Xrm.Page.getAttribute("va_mailingforeignpostalcode").setValue(null);
            Xrm.Page.getAttribute("va_paymentmilitarypostofficetypecode").setValue(null);
            Xrm.Page.getAttribute("va_paymentmilitarypostaltypecode").setValue(null);
            Xrm.Page.getAttribute("va_paymentforeignpostalcode").setValue(null);
            var gentab = Xrm.Page.ui.tabs.get('General');
            gentab.sections.get('General_section_16').setVisible(false); // hide payment country fields
            var x = document.getElementById("va_mailingcountrylist").children[0]; // select first blank entry of mailing 
            x.setAttribute("selected", "selected");                                                      // country list
            Xrm.Page.getAttribute("va_mailingcountry").setValue(null);  // zot mailing country field
            Xrm.Page.getAttribute("va_paymentcountry").setValue(null);  // zot payment country field 2-4-14 fix
            Xrm.Page.getControl('va_paymentstateoptionset').setVisible(true); // expose payment state field
            Xrm.Page.getControl('va_paymentzipcode').setVisible(true); // expose payment zip code field
            x = document.getElementById("va_paymentcountrylist").children[0]; // select first blank entry of payment 
            x.setAttribute("selected", "selected");                                                      // country list
            Xrm.Page.getControl('va_paymentcity').setVisible(true); // expose payment city field
            gentab.sections.get('General_section_13').setVisible(true); // expose payment military
            Xrm.Page.getControl('va_paymentmilitarypostaltypecode').setVisible(false);
            Xrm.Page.getControl('va_paymentmilitarypostofficetypecode').setVisible(false);
            Xrm.Page.getControl('va_paymenteffectivedate').setVisible(true);

            //           var gentab = Xrm.Page.ui.tabs.get('General');
            gentab.sections.get('MilitaryFields').setVisible(false); // disable military
            gentab.sections.get('ProvinceFields').setVisible(false); // disable province
            gentab.sections.get('CountryFields').setVisible(false); // disable country codes
            gentab.sections.get('General_section_18').setVisible(true); // show the city/state/zip fields
            Xrm.Page.getControl('va_mailingcity').setVisible(true);
            Xrm.Page.getControl('va_mailingstateoptionset').setVisible(true);
            Xrm.Page.getControl('va_mailingaddresszipcode').setVisible(true);
            break;
        case 953850001: //International 
            Xrm.Page.getAttribute("va_mailingaddress1").setRequiredLevel("required");
            Xrm.Page.getAttribute("va_mailingcity").setRequiredLevel("required");
            Xrm.Page.getAttribute("va_mailingstate").setRequiredLevel("none");
            Xrm.Page.getAttribute("va_mailingaddresszipcode").setRequiredLevel("none");
            //      Xrm.Page.getAttribute("va_mailingcountry").setRequiredLevel("required");
            Xrm.Page.getAttribute("va_mailingmilitarypostofficetypecode").setRequiredLevel("none");
            Xrm.Page.getAttribute("va_mailingmilitarypostaltypecode").setRequiredLevel("none");
            Xrm.Page.getControl('va_milzipcodelookupid').setVisible(false);
            Xrm.Page.getControl("va_mailingcity").setDisabled(false);
            Xrm.Page.getControl("va_mailingstateoptionset").setDisabled(false);
            Xrm.Page.getControl("va_mailingcountry").setDisabled(false);
            Xrm.Page.getControl("va_mailingcountrylist").setDisabled(false);

            // RTC 238877
            Xrm.Page.getControl('va_mailingcountry').setVisible(false);
            Xrm.Page.getAttribute("va_mailingstate").setValue(null);
            Xrm.Page.getAttribute("va_paymentstate").setValue(null);
            Xrm.Page.getAttribute("va_mailingstateoptionset").setRequiredLevel("none");

            Xrm.Page.getAttribute("va_mailingmilitarypostofficetypecode").setValue(null);
            Xrm.Page.getAttribute("va_mailingmilitarypostaltypecode").setValue(null);
            Xrm.Page.getAttribute("va_mailingaddresszipcode").setValue(null);
            Xrm.Page.getAttribute("va_paymentmilitarypostofficetypecode").setValue(null);
            Xrm.Page.getAttribute("va_paymentmilitarypostaltypecode").setValue(null);
            Xrm.Page.getAttribute("va_paymentzipcode").setValue(null);
            var gentab = Xrm.Page.ui.tabs.get('General');
            gentab.sections.get('General_section_16').setVisible(true); // expose payment country fields
            gentab.sections.get('General_section_13').setVisible(true); // expose payment military fields
            Xrm.Page.getControl('va_paymentmilitarypostofficetypecode').setVisible(false); // but hide mil postal ofice code 
            Xrm.Page.getControl('va_paymentmilitarypostaltypecode').setVisible(false); // and hide mil postal type fields
            Xrm.Page.getControl('va_paymentstateoptionset').setVisible(false); // hide payment state field
            Xrm.Page.getControl('va_paymentzipcode').setVisible(false); // hide payment zip code field
            Xrm.Page.getControl('va_paymentcity').setVisible(true); // expose payment city field
            var x = Xrm.Page.getAttribute('va_paymentforeignpostalcode').getValue();
            if (x != null && x.length > 2 && x.substring(0, 2) == 'FZ') {
                x = x.substring(2);
                Xrm.Page.getAttribute('va_paymentforeignpostalcode').setValue(x);
            }
            Xrm.Page.getControl("va_paymentcountrylist").setVisible(true);
            Xrm.Page.getControl('va_mailingforeignpostalcode').setVisible(false);
            Xrm.Page.getControl('va_paymentforeignpostalcode').setVisible(false);
            Xrm.Page.getAttribute("va_mailingstateoptionset").setValue(null); //  2-4-14 fix
            Xrm.Page.getAttribute("va_paymentstateoptionset").setValue(null); //  2-4-14 fix

            //            var gentab = Xrm.Page.ui.tabs.get('General');
            gentab.sections.get('MilitaryFields').setVisible(false); // disable military
            gentab.sections.get('ProvinceFields').setVisible(false); // hide province
            gentab.sections.get('CountryFields').setVisible(true); // enable country codes
            gentab.sections.get('General_section_18').setVisible(true); // show the city/state/zip fields
            Xrm.Page.getControl('va_mailingstateoptionset').setVisible(false);
            Xrm.Page.getControl('va_mailingaddresszipcode').setVisible(false);
            Xrm.Page.getControl('va_mailingcity').setVisible(true);
            var x = Xrm.Page.getAttribute('va_mailingforeignpostalcode').getValue();
            if (x != null && x.length > 2 && x.substring(0, 2) == 'FZ') {
                x = x.substring(2);
                Xrm.Page.getAttribute('va_mailingforeignpostalcode').setValue(x);
            }
            break;
        case 953850002: //Overseas Military
            Xrm.Page.getAttribute("va_mailingaddress1").setRequiredLevel("required");
            Xrm.Page.getAttribute("va_mailingcity").setRequiredLevel("none");
            Xrm.Page.getAttribute("va_mailingstate").setRequiredLevel("none");
            Xrm.Page.getAttribute("va_mailingcountry").setRequiredLevel("none");
            Xrm.Page.getAttribute("va_mailingmilitarypostofficetypecode").setRequiredLevel("required");
            Xrm.Page.getAttribute("va_mailingmilitarypostaltypecode").setRequiredLevel("required");
            Xrm.Page.getAttribute("va_mailingaddresszipcode").setRequiredLevel("required");
            Xrm.Page.getControl('va_milzipcodelookupid').setVisible(true);

            //Clear and Disable City, State, Country Per RTC 108712
            Xrm.Page.getAttribute("va_mailingcity").setValue(null);
            Xrm.Page.getAttribute("va_mailingstate").setValue(null);
            Xrm.Page.getAttribute("va_mailingstateoptionset").setValue(null);
            Xrm.Page.getAttribute("va_mailingcountry").setValue(null);
            Xrm.Page.getAttribute("va_mailingcountrylist").setValue(null);

            Xrm.Page.getControl("va_mailingcity").setDisabled(true);
            Xrm.Page.getControl("va_mailingstateoptionset").setDisabled(true);
            Xrm.Page.getControl("va_mailingcountry").setDisabled(true);
            Xrm.Page.getControl("va_mailingcountrylist").setDisabled(true);
            /*
            if (Xrm.Page.getAttribute("va_mailingcity").getValue() == null) {
            Xrm.Page.getAttribute("va_mailingcity").setValue('Overseas Military'); no longer needed as now OVR is passed 
            } */
            var gentab = Xrm.Page.ui.tabs.get('General');
            gentab.sections.get('MilitaryFields').setVisible(true); // enable military
            gentab.sections.get('ProvinceFields').setVisible(false); // disable province
            gentab.sections.get('CountryFields').setVisible(false); // disable country codes
            gentab.sections.get('General_section_18').setVisible(false); // hide the city/state/zip fields
            Xrm.Page.getControl('va_mailingstateoptionset').setVisible(false);
            Xrm.Page.getControl('va_mailingaddresszipcode').setVisible(false);
            Xrm.Page.getControl('va_mailingcity').setVisible(false);

            // RTC 238877
            Xrm.Page.getControl('va_mailingcountry').setVisible(false);
            Xrm.Page.getAttribute("va_mailingcountrylist").setRequiredLevel("none");
            Xrm.Page.getAttribute("va_paymentcity").setValue(null);
            Xrm.Page.getAttribute("va_paymentcountry").setValue(null);

            gentab.sections.get('General_section_13').setVisible(true); // expose payment military fields section
            Xrm.Page.getControl('va_paymentmilitarypostofficetypecode').setVisible(true); // expose mil postal ofice code 
            Xrm.Page.getControl('va_paymentmilitarypostaltypecode').setVisible(true); // and expose mil postal type fields
            gentab.sections.get('General_section_14').setVisible(true); // expose payment city state zip
            Xrm.Page.getControl('va_paymentcity').setVisible(false);
            Xrm.Page.getControl('va_paymentstateoptionset').setVisible(false);
            Xrm.Page.getControl('va_paymentzipcode').setVisible(true);
            x = document.getElementById("va_paymentcountrylist").children[0]; // select first blank entry of payment 
            x.setAttribute("selected", "selected");                                                      // country list
            Xrm.Page.getControl('va_paymentcountrylist').setVisible(false);
            Xrm.Page.getControl('va_paymentforeignpostalcode').setVisible(false);

            break;
        default:  //null or not selected
            Xrm.Page.getAttribute("va_mailingaddress1").setRequiredLevel("none");
            Xrm.Page.getAttribute("va_mailingcity").setRequiredLevel("none");
            Xrm.Page.getAttribute("va_mailingstate").setRequiredLevel("none");
            Xrm.Page.getAttribute("va_mailingaddresszipcode").setRequiredLevel("none");
            Xrm.Page.getAttribute("va_mailingcountry").setRequiredLevel("none");
            Xrm.Page.getAttribute("va_mailingmilitarypostofficetypecode").setRequiredLevel("none");
            Xrm.Page.getAttribute("va_mailingmilitarypostaltypecode").setRequiredLevel("none");
            // mil zip code is only for overseas addresses
            Xrm.Page.getControl('va_milzipcodelookupid').setVisible(false);

            Xrm.Page.getControl("va_mailingcity").setDisabled(false);
            Xrm.Page.getControl("va_mailingstateoptionset").setDisabled(false);
            Xrm.Page.getControl("va_mailingcountry").setDisabled(false);
            Xrm.Page.getControl("va_mailingcountrylist").setDisabled(false);
            var gentab = Xrm.Page.ui.tabs.get('General');
            gentab.sections.get('MilitaryFields').setVisible(true); // enable military
            gentab.sections.get('ProvinceFields').setVisible(true); // disable province
            gentab.sections.get('CountryFields').setVisible(true); // disable country codes
            Xrm.Page.getControl('va_mailingstateoptionset').setVisible(true);
            Xrm.Page.getControl('va_mailingaddresszipcode').setVisible(true);
            break;
    }
}

function ValidateMailingAddress() {
    var success = ValidateFields();
    if (!success) {
        return false;
    }
    var msg = 'Address Fields successfully checked for absence of invalid symbols and presence of required data.\n\n';

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

    var response = validateAddressUsingWS('Mailing', params, 'va_validateaddressresponse', 'va_mailingaddressvalidationscore');
    var wsMessage = (
        response.error ?
        response.message :
        'Address Validation Web Service Response:\nScore: ' + response.score.toString() + (response.message.length > 0 ? '\nMessage: ' + response.message : '')
    );

    alert(msg + wsMessage);
}

function validateAddressUsingWS(addressType, addressParams, responseFieldName, scoreFieldName) {
    var addressValidationCtx = new vrmContext(exCon);
    addressValidationCtx.user = GetUserSettingsForWebservice(exCon);

    addressValidationCtx.addressParameters = addressParams;

    var addressValidationService = new validateAddress(addressValidationCtx);

    var avErr = '', avRes = false,
        response = {
            error: false,
            message: '',
            score: 0
        };

    try {
        avRes = addressValidationService.executeRequest();
    }
    catch (err) {
        response.error = true;
        response.message = 'Address validation run-time error.\n\n' + err.message;
    }
    finally {
        CloseProgress();
    }

    if (addressValidationService.wsMessage != null && addressValidationService.wsMessage.errorFlag === true) {
        avRes = false;
        response.error = true;
        response.message = 'Address validation service failed. Make sure that the service is reachable.\n\n' +
            addressValidationService.wsMessage.description;
    }

    if (!avRes) {
        return response;
    }

    var addressValidationXmlObject = _XML_UTIL.parseXmlObject(addressValidationService.responseXml);

    Xrm.Page.getAttribute(responseFieldName).setValue(addressValidationService.responseXml);

    if (addressValidationXmlObject && addressValidationXmlObject != undefined && addressValidationXmlObject.xml != '') {
        if (SingleNodeExists(addressValidationXmlObject, '//Confidence')) {
            var addressValidationScore = addressValidationXmlObject.selectSingleNode('//Confidence').text;
            if (!isNaN(addressValidationScore)) {
                response.score = parseInt(addressValidationScore);
            }
        }

        response.message = (SingleNodeExists(addressValidationXmlObject, '//Status.Description')) ? addressValidationXmlObject.selectSingleNode('//Status.Description').text : '';
    }
    Xrm.Page.getAttribute(scoreFieldName).setValue(response.score.toString());
    return response;
}

function ValidatePaymentAddress() {
    var success = ValidatePaymentFields();
    if (!success) {
        return false;
    }

    var msg = 'Address Fields successfully checked for absence of invalid symbols and presence of required data.\n\n';

    var postalCode = Xrm.Page.getAttribute('va_paymentzipcode').getValue() ?
        Xrm.Page.getAttribute('va_paymentzipcode').getValue() : Xrm.Page.getAttribute('va_paymentforeignpostalcode').getValue();

    var params = [
            { name: 'AddressLine1', value: Xrm.Page.getAttribute('va_paymentaddress1').getValue() },
            { name: 'AddressLine2', value: Xrm.Page.getAttribute('va_paymentaddress2').getValue() },
            { name: 'AddressLine3', value: Xrm.Page.getAttribute('va_paymentaddress3').getValue() },
            { name: 'AddressLine4', value: '' },
            { name: 'City', value: Xrm.Page.getAttribute('va_paymentcity').getValue() },
            { name: 'StateProvince', value: Xrm.Page.getAttribute('va_paymentstate').getValue() },
            { name: 'PostalCode', value: postalCode },
            { name: 'Country', value: Xrm.Page.getAttribute('va_paymentcountry').getValue() }
    ];

    var response = validateAddressUsingWS('Payment', params, 'va_validateapellantaddressresponse', 'va_paymentaddressvalidationscore');
    var wsMessage = (
        response.error ?
        response.message :
        'Address Validation Web Service Response:\nScore: ' + response.score.toString() + (response.message.length > 0 ? '\nMessage: ' + response.message : '')
    );

    alert(msg + wsMessage);
}

function ValidateFields() {
    var Errors = {};

    var payeeCode = parseInt(Xrm.Page.getAttribute("va_payeetypecode").getValue());
    var addressType = Xrm.Page.getAttribute("va_addresstype").getSelectedOption();

    var ValidationFields;
    // PC 11-29 are children recipients
    if (payeeCode > 10 && payeeCode < 30) {
        ValidationFields = GetAddressArray(true);
    }
    else {
        ValidationFields = GetAddressArray();
    }

    //Applies the max length restriction to fields.
    for (var field in ValidationFields) {
        var input = Xrm.Page.getAttribute(field).getValue();
        if (input && input.length > ValidationFields[field]) {
            Errors[Xrm.Page.getControl(field).getLabel()] = "Length surpassed max value of " + ValidationFields[field] + ";";
            //Xrm.Page.getAttribute(field).setValue(input.substring(0, ValidationFields[field]));
        }
        ValidationFields[field] = "";
    }

    if (parseInt(Xrm.Page.getAttribute("va_mailingaddresszipcode").getValue()) == NaN) {
        if (Errors[Xrm.Page.getControl("va_mailingaddresszipcode").getLabel()] != undefined) {
            Errors[Xrm.Page.getControl("va_mailingaddresszipcode").getLabel()] += 'Must be a number;';
        }
        else {
            Errors[Xrm.Page.getControl("va_mailingaddresszipcode").getLabel()] = 'Must be a number;';
        }
    }
    /*
    if (parseInt(Xrm.Page.getAttribute("va_mailingaddresszipcode").getValue()) != Xrm.Page.getAttribute("va_mailingaddresszipcode").getValue()) {
    if (Errors[Xrm.Page.getControl("va_mailingaddresszipcode").getLabel()] != undefined) {
    Errors[Xrm.Page.getControl("va_mailingaddresszipcode").getLabel()] += 'Contains an invalid character;';
    }
    else {
    Errors[Xrm.Page.getControl("va_mailingaddresszipcode").getLabel()] = 'Contains an invalid character;';
    }
    } */


    //if spouse or children recipients
    if (payeeCode >= 00 && payeeCode < 30) {
        for (var field in ValidationFields) {
            if (Xrm.Page.getControl(field).getLabel() == "Address 1") {
                var success = TestForAllowedChars(Xrm.Page.getAttribute(field).getValue(), [14, 27]);
                if (success == false) {
                    if (Errors[Xrm.Page.getControl(field).getLabel()] != undefined) {
                        Errors[Xrm.Page.getControl(field).getLabel()] += "Field can only contain alphanumeric characters, dashes, slashes, and single spaces;";
                    }
                    else {
                        Errors[Xrm.Page.getControl(field).getLabel()] = "Field can only contain alphanumeric characters, dashes, slashes, and single spaces;";
                    }
                }
            }

            if (Xrm.Page.getControl(field).getLabel() == "Address 2") {
                var success = TestForAllowedChars(Xrm.Page.getAttribute(field).getValue(), [33, 24, 5, 7, 9, 23, 11, 12, 15, 31, 14, 29, 27, 25, 4]);
                if (success == false) {
                    if (Errors[Xrm.Page.getControl(field).getLabel()] != undefined) {
                        Errors[Xrm.Page.getControl(field).getLabel()] += "Field contains a non-allowable character;";
                    }
                    else {
                        Errors[Xrm.Page.getControl(field).getLabel()] = "Field contains a non-allowable character;";
                    }
                }
            }

            if (Xrm.Page.getControl(field).getLabel() == "Address 3") {
                var success = TestForAllowedChars(Xrm.Page.getAttribute(field).getValue(), [33, 24, 5, 7, 9, 23, 11, 12, 15, 31, 14, 29, 27, 25, 4]);
                if (success == false) {
                    if (Errors[Xrm.Page.getControl(field).getLabel()] != undefined) {
                        Errors[Xrm.Page.getControl(field).getLabel()] += "Field contains a non-allowable character;";
                    }
                    else {
                        Errors[Xrm.Page.getControl(field).getLabel()] = "Field contains a non-allowable character;";
                    }
                }
            }

            if (addressType && (addressType.text == "International" || addressType.text == "Domestic")) {
                if (Xrm.Page.getControl(field).getLabel() == "City") {
                    var success = TestForAllowedChars(Xrm.Page.getAttribute(field).getValue(), [33, 24, 5, 7, 9, 23, 11, 12, 15, 31, 14, 29, 27, 25, 4]);
                    if (success == false) {
                        if (Errors[Xrm.Page.getControl(field).getLabel()] != undefined) {
                            Errors[Xrm.Page.getControl(field).getLabel()] += "Field contains a non-allowable character;";
                        }
                        else {
                            Errors[Xrm.Page.getControl(field).getLabel()] = "Field contains a non-allowable character;";
                        }
                    }
                }
            }

            if (addressType && (addressType.text == "Domestic" || addressType.text == "Overseas")) {
                if (Xrm.Page.getControl(field).getLabel() == "Zip Code") {
                    var zipSize = Xrm.Page.getAttribute(field).getValue();
                    if (zipSize && zipSize.length != 5) {
                        if (Errors[Xrm.Page.getControl(field).getLabel()] != undefined) {
                            Errors[Xrm.Page.getControl(field).getLabel()] += "Field must be exactly 5 characters;";
                        }
                        else {
                            Errors[Xrm.Page.getControl(field).getLabel()] = "Field must be exactly 5 characters;";
                        }
                    }
                }
            }
        } //end loop through the validation fields
    } //end payee code between 10 and 30

    var text = "";
    for (var a in Errors) {
        text += a + ": " + Errors[a] + "\n";
    }
    if (text != "") {
        alert("Field Validation Failed: \n\n" + text);
        return false;
    }
    else {
        return true;
    }
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
        24: "["]+",
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
        if (chars[invalid] != null) {
            var myregex = new RegExp(chars[invalid]);
            success = !myregex.test(text);
        }
        if (success == false) break;
    }

    return success;
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

    //Applies the max length restriction to fields.
    for (var field in fields) {
        var input = Xrm.Page.getAttribute(field).getValue();
        if (input && input.length > fields[field]) {
            Errors[Xrm.Page.getControl(field).getLabel()] = "Payment field length surpassed max value of " + fields[field] + ";";
            //Xrm.Page.getAttribute(field).setValue(input.substring(0, ValidationFields[field]));
        }
        if (Xrm.Page.getControl(field).getLabel() == "Zip Code") {
            var zipSize = Xrm.Page.getAttribute(field).getValue();
            if (zipSize && zipSize.length != 5) {
                if (Errors[Xrm.Page.getControl(field).getLabel()] != undefined) {
                    Errors[Xrm.Page.getControl(field).getLabel()] += "Field must be exactly 5 characters;";
                }
                else {
                    Errors[Xrm.Page.getControl(field).getLabel()] = "Field must be exactly 5 characters;";
                }
            }
        }
        if (Xrm.Page.getControl(field).getLabel() == "Address 1") {
            var success = TestForAllowedChars(Xrm.Page.getAttribute(field).getValue(), [14, 27]);
            if (success == false) {
                if (Errors[Xrm.Page.getControl(field).getLabel()] != undefined) {
                    Errors[Xrm.Page.getControl(field).getLabel()] += "Field can only contain alphanumeric characters, dashes, slashes, and single spaces;";
                }
                else {
                    Errors[Xrm.Page.getControl(field).getLabel()] = "Field can only contain alphanumeric characters, dashes, slashes, and single spaces;";
                }
            }
        }

        var success = TestForAllowedChars(Xrm.Page.getAttribute(field).getValue(), [33, 24, 5, 7, 9, 23, 11, 12, 15, 31, 14, 29, 27, 25, 4]);
        if (success == false) {
            if (Errors[Xrm.Page.getControl(field).getLabel()] != undefined) {
                Errors[Xrm.Page.getControl(field).getLabel()] += "Field contains a non-allowable character;";
            }
            else {
                Errors[Xrm.Page.getControl(field).getLabel()] = "Field contains a non-allowable character;";
            }
        }
    }

    if (parseInt(Xrm.Page.getAttribute("va_paymentzipcode").getValue()) == NaN) {
        if (Errors[Xrm.Page.getControl("va_paymentzipcode").getLabel()] != undefined) {
            Errors[Xrm.Page.getControl("va_paymentzipcode").getLabel()] += 'Must be a number;';
        }
        else {
            Errors[Xrm.Page.getControl("va_paymentzipcode").getLabel()] = 'Must be a number;';
        }
    }
    /*
    if (parseInt(Xrm.Page.getAttribute("va_paymentzipcode").getValue()) != Xrm.Page.getAttribute("va_paymentzipcode").getValue()) {
    if (Errors[Xrm.Page.getControl("va_paymentzipcode").getLabel()] != undefined) {
    Errors[Xrm.Page.getControl("va_paymentzipcode").getLabel()] += 'Contains an invalid character;';
    }
    else {
    Errors[Xrm.Page.getControl("va_paymentzipcode").getLabel()] = 'Contains an invalid character;';
    }
    }*/

    var text = "";
    for (var a in Errors) {
        text += a + ": " + Errors[a] + "\n";
    }
    if (text != "") {
        alert("Validation Failed: \n\n" + text);
        return false;
    }
    else {
        return true;
    }
}

function ProgressDlg(text) { this.text = text; }

ProgressDlg.prototype.ShowProgress = function (msg) {
    if (!_progressWindow) {
        prepareProgress();
    } else {
        positionProgress();
    }

    if (!_progressWindow) return;
    var elem = _progressWindow.document.getElementById('step');
    if (elem) elem.innerText = msg;
};

function RetrieveAwardAddresses(vetSearchCtx, opener) {
    ShowProgress('Retrieving Addresses...');
    var findAwardAddressesByFilenumber = new findAwardAddresses(vetSearchCtx);
    findAwardAddressesByFilenumber.executeRequest();

    //Parsing XML
    if (!findAwardAddressesByFilenumber.wsMessage.errorFlag) {
        var $xml = $($.parseXML(Xrm.Page.getAttribute('va_findawardaddressesresponse').getValue()));
        $xml.find('directDepositAccountNumber').text('**********');
        $xml.find('directDepositRoutingNumber').text('**********');

        awardAddresses_xmlObject = _XML_UTIL.parseXmlObject(Xrm.Page.getAttribute('va_findawardaddressesresponse').getValue());
        //Xrm.Page.getAttribute('va_findawardaddressesresponse').setValue(formatXml(Xrm.Page.getAttribute('va_findawardaddressesresponse').getValue()));

        var xmlString = undefined;
        if (window.ActiveXObject) {
            xmlString = $xml[0].xml;
        }

        if (xmlString === undefined) {
            xmlString = (new XMLSerializer()).serializeToString($xml[0]);
        }

        Xrm.Page.getAttribute('va_findawardaddressesresponse').setValue(xmlString);

    } else {
        throw findAwardAddressesByFilenumber.wsMessage.description;
    }

    var returnNode = awardAddresses_xmlObject.selectNodes('//return');
    var awardNodes = returnNode[0].childNodes;

    if (returnNode) {
        for (var i = 0; i < awardNodes.length; i++) {         //looping through addresses and
            if (awardNodes[i].nodeName == 'awardAddresses') {  //making sure we dont check irrelevant nodes
                if (awardNodes[i].selectSingleNode('awardTypeCode').text == opener._changeOfAddressData.awardTypeCd &&
							awardNodes[i].selectSingleNode('ptcpntVetID').text == opener._changeOfAddressData.ptcpntVetID &&
							awardNodes[i].selectSingleNode('ptcpntBeneID').text == opener._changeOfAddressData.ptcpntBeneID &&
							awardNodes[i].selectSingleNode('ptcpntRecipID').text == opener._changeOfAddressData.ptcpntRecipID) {

                    selectedAward_xmlObject = _XML_UTIL.parseXmlObject(awardNodes[i].xml);

                    Xrm.Page.getAttribute('va_awardtypecode').setValue(awardNodes[i].selectSingleNode('awardTypeCode').text);
                    Xrm.Page.getAttribute('va_participantbeneid').setValue(awardNodes[i].selectSingleNode('ptcpntBeneID').text);
                    Xrm.Page.getAttribute('va_participantrecipid').setValue(awardNodes[i].selectSingleNode('ptcpntRecipID').text);
                    Xrm.Page.getAttribute('va_participantvetid').setValue(opener._changeOfAddressData.ptcpntVetID);
                    //                  Xrm.Page.getAttribute('va_participantvetid').setValue(awardNodes[i].selectSingleNode('ptcpntVetID').text);
                    Xrm.Page.getAttribute('va_payeetypecode').setValue(awardNodes[i].selectSingleNode('payeeTypeCode').text);
                    break;
                }
            }
        }
    }

    if (!awardAddresses_xmlObject) {
        CloseProgress();
        throw 'Find Award Addresses web service failed to return results matching selected award.';
    }
    //Populating GUI fields

    //mailing fields
    //    if (SingleNodeExists(selectedAward_xmlObject, '//mailingAddressExistsIndicator')) {
    //        if (selectedAward_xmlObject.selectSingleNode('//mailingAddressExistsIndicator').text == 'Y') {
    //            Xrm.Page.getAttribute('va_mailingaddressexistsindicator').setValue(true);
    //        } else {
    //            Xrm.Page.getAttribute('va_mailingaddressexistsindicator').setValue(false);
    //        }
    //    }

    SingleNodeExists(selectedAward_xmlObject, '//mailingAddress/addressLine1') ? Xrm.Page.getAttribute('va_mailingaddress1').setValue(selectedAward_xmlObject.selectSingleNode('//mailingAddress/addressLine1').text) : null;
    SingleNodeExists(selectedAward_xmlObject, '//mailingAddress/addressLine2') ? Xrm.Page.getAttribute('va_mailingaddress2').setValue(selectedAward_xmlObject.selectSingleNode('//mailingAddress/addressLine2').text) : null;
    SingleNodeExists(selectedAward_xmlObject, '//mailingAddress/addressLine3') ? Xrm.Page.getAttribute('va_mailingaddress3').setValue(selectedAward_xmlObject.selectSingleNode('//mailingAddress/addressLine3').text) : null;
    SingleNodeExists(selectedAward_xmlObject, '//mailingAddress/city') ? Xrm.Page.getAttribute('va_mailingcity').setValue(selectedAward_xmlObject.selectSingleNode('//mailingAddress/city').text) : null;
    SingleNodeExists(selectedAward_xmlObject, '//mailingAddress/state') ? Xrm.Page.getAttribute('va_mailingstate').setValue(selectedAward_xmlObject.selectSingleNode('//mailingAddress/state').text) : null;
    SingleNodeExists(selectedAward_xmlObject, '//mailingAddress/state') ? Xrm.Page.getAttribute('va_mailingstateoptionset').setValue(
				   GetOptionValue('va_mailingstateoptionset', selectedAward_xmlObject.selectSingleNode('//mailingAddress/state').text)) : null;
    SingleNodeExists(selectedAward_xmlObject, '//mailingAddress/zipPrefix') ? Xrm.Page.getAttribute('va_mailingaddresszipcode').setValue(selectedAward_xmlObject.selectSingleNode('//mailingAddress/zipPrefix').text) : null;
    SingleNodeExists(selectedAward_xmlObject, '//mailingAddress/countryTypeName') ? Xrm.Page.getAttribute('va_mailingcountry').setValue(selectedAward_xmlObject.selectSingleNode('//mailingAddress/countryTypeName').text) : null;
    SingleNodeExists(selectedAward_xmlObject, '//mailingAddress/forignPostalCode') ? Xrm.Page.getAttribute('va_mailingforeignpostalcode').setValue(selectedAward_xmlObject.selectSingleNode('//mailingAddress/forignPostalCode').text) : null;
    SingleNodeExists(selectedAward_xmlObject, '//mailingAddress/effectiveDate') ? Xrm.Page.getAttribute('va_mailingeffectivedate').setValue(new Date(selectedAward_xmlObject.selectSingleNode('//mailingAddress/effectiveDate').text)) : null;

    // mltyPostOfficeTypeCd and mltyPostalTypeCd
    var PostOfficeTypeCode = SingleNodeExists(selectedAward_xmlObject, '//mailingAddress/militaryPostOfficeTypeCode') ? selectedAward_xmlObject.selectSingleNode('//mailingAddress/militaryPostOfficeTypeCode').text : null;
    var PostalTypeCode = SingleNodeExists(selectedAward_xmlObject, '//mailingAddress/militaryPostalTypeCode') ? selectedAward_xmlObject.selectSingleNode('//mailingAddress/militaryPostalTypeCode').text : null;
    if (PostOfficeTypeCode) { SetOptionSetFromValue('va_mailingmilitarypostofficetypecode', PostOfficeTypeCode); }
    if (PostalTypeCode) { SetOptionSetFromValue('va_mailingmilitarypostaltypecode', PostalTypeCode); }
    ////////////
    var milZipCode = null;
    if (PostOfficeTypeCode) {
        milZipCode = SingleNodeExists(selectedAward_xmlObject, '//mailingAddress/zipPrefix') ? selectedAward_xmlObject.selectSingleNode('//mailingAddress/zipPrefix').text : null;
    }
    if (milZipCode) {
        // rest call to get GUID
        var milzipGuid;
        var filter = 'va_zip eq "' + milZipCode + '"';
        CrmRestKit2011.ByQuery('va_milzipcodelookup', ['va_milzipcodelookupid'], filter, false).done(function (data) {
            milzipGuid = data.d.Results[0].va_milzipcodelookupid;
        });

        var lookup = new Array();
        lookup[0] = new Object();
        lookup[0].id = milzipGuid;
        lookup[0].name = milZipCode;
        lookup[0].entityType = 'va_milzipcodelookup';
        Xrm.Page.getAttribute("va_milzipcodelookupid").setValue(lookup);
    }

    SingleNodeExists(selectedAward_xmlObject, '//mailingAddress/province') ? Xrm.Page.getAttribute('va_provincename').setValue(selectedAward_xmlObject.selectSingleNode('//mailingAddress/province').text) : null;

    SingleNodeExists(selectedAward_xmlObject, '//mailingAddress/territory') ? Xrm.Page.getAttribute('va_territoryname').setValue(selectedAward_xmlObject.selectSingleNode('//mailingAddress/territory').text) : null;

    //payment fields
    //    if (SingleNodeExists(selectedAward_xmlObject, '//paymentAddressExistsIndicator')) {
    //        if (selectedAward_xmlObject.selectSingleNode('//paymentAddressExistsIndicator').text == 'Y') {
    //            Xrm.Page.getAttribute('va_paymentaddressexistsindicator').setValue(true);
    //        } else {
    //            Xrm.Page.getAttribute('va_paymentaddressexistsindicator').setValue(false);
    //        }
    //    }

    SingleNodeExists(selectedAward_xmlObject, '//paymentAddress/addressLine1') ? Xrm.Page.getAttribute('va_paymentaddress1').setValue(selectedAward_xmlObject.selectSingleNode('//paymentAddress/addressLine1').text) : null;
    SingleNodeExists(selectedAward_xmlObject, '//paymentAddress/addressLine2') ? Xrm.Page.getAttribute('va_paymentaddress2').setValue(selectedAward_xmlObject.selectSingleNode('//paymentAddress/addressLine2').text) : null;
    SingleNodeExists(selectedAward_xmlObject, '//paymentAddress/addressLine3') ? Xrm.Page.getAttribute('va_paymentaddress3').setValue(selectedAward_xmlObject.selectSingleNode('//paymentAddress/addressLine3').text) : null;
    SingleNodeExists(selectedAward_xmlObject, '//paymentAddress/city') ? Xrm.Page.getAttribute('va_paymentcity').setValue(selectedAward_xmlObject.selectSingleNode('//paymentAddress/city').text) : null;
    SingleNodeExists(selectedAward_xmlObject, '//paymentAddress/state') ? Xrm.Page.getAttribute('va_paymentstate').setValue(selectedAward_xmlObject.selectSingleNode('//paymentAddress/state').text) : null;
    SingleNodeExists(selectedAward_xmlObject, '//paymentAddress/state') ? Xrm.Page.getAttribute('va_paymentstateoptionset').setValue(
				   GetOptionValue('va_paymentstateoptionset', selectedAward_xmlObject.selectSingleNode('//paymentAddress/state').text)) : null;
    SingleNodeExists(selectedAward_xmlObject, '//paymentAddress/zipPrefix') ? Xrm.Page.getAttribute('va_paymentzipcode').setValue(selectedAward_xmlObject.selectSingleNode('//paymentAddress/zipPrefix').text) : null;
    SingleNodeExists(selectedAward_xmlObject, '//paymentAddress/countryTypeName') ? Xrm.Page.getAttribute('va_paymentcountry').setValue(selectedAward_xmlObject.selectSingleNode('//paymentAddress/countryTypeName').text) : null;
    SingleNodeExists(selectedAward_xmlObject, '//paymentAddress/forignPostalCode') ? Xrm.Page.getAttribute('va_paymentforeignpostalcode').setValue(selectedAward_xmlObject.selectSingleNode('//paymentAddress/forignPostalCode').text) : null;
    SingleNodeExists(selectedAward_xmlObject, '//paymentAddress/effectiveDate') ? Xrm.Page.getAttribute('va_paymenteffectivedate').setValue(new Date(selectedAward_xmlObject.selectSingleNode('//paymentAddress/effectiveDate').text)) : null;

    PostOfficeTypeCode = SingleNodeExists(selectedAward_xmlObject, '//paymentAddress/militaryPostOfficeTypeCode') ? selectedAward_xmlObject.selectSingleNode('//paymentAddress/militaryPostOfficeTypeCode').text : null;
    PostalTypeCode = SingleNodeExists(selectedAward_xmlObject, '//paymentAddress/militaryPostalTypeCode') ? selectedAward_xmlObject.selectSingleNode('//paymentAddress/militaryPostalTypeCode').text : null;
    if (PostOfficeTypeCode) { SetOptionSetFromValue('va_paymentmilitarypostofficetypecode', PostOfficeTypeCode); }
    if (PostalTypeCode) { SetOptionSetFromValue('va_paymentmilitarypostaltypecode', PostalTypeCode); }

    //deposit fields
    //    if (SingleNodeExists(selectedAward_xmlObject, '//directDepositExistsIndicator')) {
    //        if (selectedAward_xmlObject.selectSingleNode('//directDepositExistsIndicator').text == 'Y') {
    //            Xrm.Page.getAttribute('va_directdepositexistsindicator').setValue(true);
    //        } else {
    //            Xrm.Page.getAttribute('va_directdepositexistsindicator').setValue(false);
    //        }
    //    }

    SingleNodeExists(selectedAward_xmlObject, '//statusTypeCode') ? Xrm.Page.getAttribute('va_paystatus').setValue(selectedAward_xmlObject.selectSingleNode('//statusTypeCode').text) : null;
    SingleNodeExists(selectedAward_xmlObject, '//directDepositAccountNumber') ? Xrm.Page.getAttribute('va_depositaccountnumber').setValue(selectedAward_xmlObject.selectSingleNode('//directDepositAccountNumber').text) : null;

    SingleNodeExists(selectedAward_xmlObject, '//directDepositRoutingNumber') ? Xrm.Page.getAttribute('va_routingnumber').setValue(selectedAward_xmlObject.selectSingleNode('//directDepositRoutingNumber').text) : null;

    SingleNodeExists(selectedAward_xmlObject, '//directDepositBeginDate') ? Xrm.Page.getAttribute('va_depositbegindate').setValue(new Date(selectedAward_xmlObject.selectSingleNode('//directDepositBeginDate').text)) : null;

    SingleNodeExists(selectedAward_xmlObject, '//directDepositEndDate') ? Xrm.Page.getAttribute('va_depositenddate').setValue(new Date(selectedAward_xmlObject.selectSingleNode('//directDepositEndDate').text)) : null;

    SingleNodeExists(selectedAward_xmlObject, '//recurringPayableEffectiveDate') ? Xrm.Page.getAttribute('va_recurringpayableeffectivedate').setValue(new Date(selectedAward_xmlObject.selectSingleNode('//recurringPayableEffectiveDate').text)) : null;

    SingleNodeExists(selectedAward_xmlObject, '//netRateAmount') ? Xrm.Page.getAttribute('va_netrateamount').setValue(parseFloat(selectedAward_xmlObject.selectSingleNode('//netRateAmount').text)) : null;
    SingleNodeExists(selectedAward_xmlObject, '//netRateAmount') ? Xrm.Page.getAttribute('va_grossamount').setValue(parseFloat(selectedAward_xmlObject.selectSingleNode('//netRateAmount').text)) : null;

    if (SingleNodeExists(selectedAward_xmlObject, '//directDepositAccountTypeName')) {
        if (selectedAward_xmlObject.selectSingleNode('//directDepositAccountTypeName').text.charAt(0) == 'C') {
            Xrm.Page.getAttribute('va_depositaccounttype').setValue(953850000);
        } else {
            selectedAward_xmlObject.selectSingleNode('//directDepositAccountTypeName').text.charAt(0) == 'S' ? Xrm.Page.getAttribute('va_depositaccounttype').setValue(953850001) : null;
        }
    }

    Xrm.Page.getAttribute('va_webserviceresponse').setSubmitMode('always');

    var routingNumber = Xrm.Page.getAttribute("va_routingnumber").getValue();
    var accountNumber = Xrm.Page.getAttribute("va_depositaccountnumber").getValue();
    var accountType = Xrm.Page.getAttribute("va_depositaccounttype").getText();

    //Xrm.Page.getAttribute('va_ca_r').setValue(routingNumber);
    //Xrm.Page.getAttribute('va_ca_a').setValue(accountNumber);
    //Xrm.Page.getAttribute('va_ca_type').setValue(accountType);
    globalcadd.ca_r = routingNumber;
    globalcadd.ca_a = accountNumber;
    globalcadd.ca_t = accountType;

    //VTRIGILI 2015-01-27 - Added check for null va_crn
    if (parent_page != null && routingNumber != null) {
        var va_crn_x = parent_page.getAttribute("va_crn");

        if ((va_crn_x != null) && routingNumber == va_crn_x.getValue()) {
            //Xrm.Page.getAttribute('va_ca').setValue(true);
            globalcadd.ca = true;

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
        //Xrm.Page.getAttribute("va_account").setValue(accountNumber);
    }
    else {
        //Xrm.Page.getAttribute('va_ca').setValue(false);
        globalcadd.ca = false;
    }
}

// Not used in the OnLoad
function RetrieveAddressesFromParent(parentPage, opener) {

    var addrXml = parentPage.getAttribute('va_findaddressresponse').getValue(),
        addressXmlObject = addrXml && addrXml.length > 0 ? _XML_UTIL.parseXmlObject(addrXml) : null,
	    foundMailing = false,
	    foundPayment = false,
		addressNodes = addressXmlObject ? addressXmlObject.selectNodes('//return') : null,
        ga = Xrm.Page.getAttribute,
        sno = SingleNodeExists;

    ga('va_participantrecipid').setValue(opener._changeOfAddressData.participantClaimantID);
    ga('va_participantvetid').setValue(opener._changeOfAddressData.participantVetID);
    ga('va_payeetypecode').setValue(opener._changeOfAddressData.payeeTypeCode);
    ga('va_awardtypecode').setValue(opener._changeOfAddressData.programTypeCode);

    if (addressNodes) {
        for (var i = 0; i < addressNodes.length; i++) {         //looping through addresses and
            if (addressNodes[i].selectSingleNode('ptcpntAddrsTypeNm').text === 'Mailing' && !foundMailing) {
                //                ga('va_mailingaddressexistsindicator').setValue(true);
                ga('va_mailingaddress1').setValue(sno(addressNodes[i], 'addrsOneTxt') ? addressNodes[i].selectSingleNode('addrsOneTxt').text : null);
                ga('va_mailingaddress2').setValue(sno(addressNodes[i], 'addrsTwoTxt') ? addressNodes[i].selectSingleNode('addrsTwoTxt').text : null);
                ga('va_mailingaddress3').setValue('');
                ga('va_mailingcity').setValue(sno(addressNodes[i], 'cityNm') ? addressNodes[i].selectSingleNode('cityNm').text : null);
                ga('va_mailingstate').setValue(sno(addressNodes[i], 'postalCd') ? addressNodes[i].selectSingleNode('postalCd').text : null);
                ga('va_mailingstateoptionset').setValue(sno(addressNodes[i], 'postalCd') ? GetOptionValue('va_mailingstateoptionset', addressNodes[i].selectSingleNode('postalCd').text) : null);
                ga('va_mailingaddresszipcode').setValue(sno(addressNodes[i], 'zipPrefixNbr') ? addressNodes[i].selectSingleNode('zipPrefixNbr').text : null);
                ga('va_mailingcountry').setValue(sno(addressNodes[i], 'cntryNm') ? addressNodes[i].selectSingleNode('cntryNm').text : null);
                ga('va_mailingforeignpostalcode').setValue(sno(addressNodes[i], 'frgnPostalCd') ? addressNodes[i].selectSingleNode('frgnPostalCd').text.replace("?", "") : null);
                ga('va_mailingeffectivedate').setValue(sno(addressNodes[i], 'efctvDt') ? new Date(convertDate(addressNodes[i].selectSingleNode('efctvDt').text.substr(0, 10))) : null);

                foundMailing = true;
            }

            if (addressNodes[i].selectSingleNode('ptcpntAddrsTypeNm').text === 'CP Payment' && !foundPayment) {
                //                ga('va_paymentaddressexistsindicator').setValue(true);
                ga('va_paymentaddress1').setValue(sno(addressNodes[i], 'addrsOneTxt') ? addressNodes[i].selectSingleNode('addrsOneTxt').text : null);
                ga('va_paymentaddress2').setValue(sno(addressNodes[i], 'addrsTwoTxt') ? addressNodes[i].selectSingleNode('addrsTwoTxt').text : null);
                ga('va_paymentaddress3').setValue('');
                ga('va_paymentcity').setValue(sno(addressNodes[i], 'cityNm') ? addressNodes[i].selectSingleNode('cityNm').text : null);
                ga('va_paymentstate').setValue(sno(addressNodes[i], 'postalCd') ? addressNodes[i].selectSingleNode('postalCd').text : null);
                ga('va_paymentstateoptionset').setValue(sno(addressNodes[i], 'postalCd') ? GetOptionValue('va_paymentstateoptionset', addressNodes[i].selectSingleNode('postalCd').text) : null);
                ga('va_paymentzipcode').setValue(sno(addressNodes[i], 'zipPrefixNbr') ? addressNodes[i].selectSingleNode('zipPrefixNbr').text : null);
                ga('va_paymentcountry').setValue(sno(addressNodes[i], 'cntryNm') ? addressNodes[i].selectSingleNode('cntryNm').text : null);
                ga('va_paymentforeignpostalcode').setValue(sno(addressNodes[i], 'frgnPostalCd') ? addressNodes[i].selectSingleNode('frgnPostalCd').text.replace("?", "") : null);
                ga('va_paymenteffectivedate').setValue(sno(addressNodes[i], 'efctvDt') ? new Date(convertDate(addressNodes[i].selectSingleNode('efctvDt').text.substr(0, 10))) : null);

                foundPayment = true;
            }
        }
    }
}

function retrieveAppellantAddress() {
    var searchByValue = Xrm.Page.getAttribute("va_findappealsby").getValue(),
        searchParam = Xrm.Page.getAttribute('va_filenumber').getValue(),
        appealsSearchContext = new vrmContext(exCon);

    if (!searchParam || searchParam.length == 0) { searchParam = Xrm.Page.getAttribute('va_apellantssn').getValue(); }

    _UserSettings = GetUserSettingsForWebservice(exCon);
    appealsSearchContext.user = _UserSettings;

    if (searchByValue && searchByValue != undefined && searchByValue.length > 0) {
        if (searchByValue == 953850001) { searchParam = Xrm.Page.getAttribute('va_appellantssn').getValue(); }
        else if (searchByValue == 953850002) {
            searchParam = Xrm.Page.getAttribute('va_appealsssn').getValue();
            appealsSearchContext.parameters['appealsFirstName'] = Xrm.Page.getAttribute('va_appealsfirstname').getValue();
            appealsSearchContext.parameters['appealsLastName'] = Xrm.Page.getAttribute('va_appealsfirstname').getValue();
        }
    }

    appealsSearchContext.parameters['fileNumber'] = searchParam;

    return retrieveAppellantAddresses(appealsSearchContext);
}

function retrieveAppellantAddresses(vetSearchCtx) {
    ///*********************
    // TODO: call WS to get Vacols address info
    Xrm.Page.getAttribute('va_participantvetid').setValue(_participantId);
    Xrm.Page.getAttribute('va_participantvetid').setSubmitMode('always');
    Xrm.Page.getAttribute('va_apellantssn').setValue(vetSearchCtx.parameters['fileNumber']);
    Xrm.Page.getAttribute('va_apellantssn').setSubmitMode('always');
    // Xrm.Page.getAttribute('va_apellantssn').getValue()

    // TODO: populate requried Apellant Modified By field
    ShowProgress('Retrieving Appellant Information...');
    var findAppellantAddressService = new getAppellantAddress(vetSearchCtx);
    var aaErr = '';
    try {
        findAppellantAddressService.executeRequest();
    }
    catch (err) {
        aaErr = 'Appellant Address Service had failed to retrieve address. The Web Service may be down or it may be not reachable from your location.\n\n' + err.message;
        _hasAppeals = false;
    }

    if (findAppellantAddressService.wsMessage.errorFlag) {
        aaErr = 'Find Appellant Address web service failed to retrieve the proper appellant address information.\n\nMid-tier components reported this error: ' + findAppellantAddressService.wsMessage.description;
        _hasAppeals = false;
    }

    //    if (aaErr.length > 0) {
    //        CloseProgress();
    //        if (!confirm('Appellant Address Service had failed to retrieve address. Would you like to continue and retrieve Award Address?'))
    //            throw aaErr;
    //    }

    if (aaErr.length == 0) {
        var appellantAddress_xmlObject = _XML_UTIL.parseXmlObject(findAppellantAddressService.responseXml);
        var appLine1 = null, appLine2 = null, appCity = null, appState = null, appZip = null;
        var appCountry = null, appModified = null, appModDate = null, appWorkPhone = null, appHomePhone = null;
        var appFName, appMName, appLName, appAddressKey;

        appAddressKey = SingleNodeExists(appellantAddress_xmlObject, '//AppellantAddress/AddressKey')
				? appellantAddress_xmlObject.selectSingleNode('//AppellantAddress/AddressKey').text : null;

        appLine1 = SingleNodeExists(appellantAddress_xmlObject, '//AppellantAddress/AppellantAddressLine1')
				? appellantAddress_xmlObject.selectSingleNode('//AppellantAddress/AppellantAddressLine1').text : null;

        appLine2 = SingleNodeExists(appellantAddress_xmlObject, '//AppellantAddress/AppellantAddressLine2')
				? appellantAddress_xmlObject.selectSingleNode('//AppellantAddress/AppellantAddressLine2').text : null;

        appCity = SingleNodeExists(appellantAddress_xmlObject, '//AppellantAddress/AppellantAddressCityName')
				? appellantAddress_xmlObject.selectSingleNode('//AppellantAddress/AppellantAddressCityName').text : null;

        appState = SingleNodeExists(appellantAddress_xmlObject, '//AppellantAddress/AppellantAddressStateCode')
				? appellantAddress_xmlObject.selectSingleNode('//AppellantAddress/AppellantAddressStateCode').text : null;

        appZip = SingleNodeExists(appellantAddress_xmlObject, '//AppellantAddress/AppellantAddressZipCode')
				? appellantAddress_xmlObject.selectSingleNode('//AppellantAddress/AppellantAddressZipCode').text : null;

        appCountry = SingleNodeExists(appellantAddress_xmlObject, '//AppellantAddress/AppellantAddressCountryName')
				? appellantAddress_xmlObject.selectSingleNode('//AppellantAddress/AppellantAddressCountryName').text : null;

        appModified = SingleNodeExists(appellantAddress_xmlObject, '//AppellantAddress/AppellantAddressLastModifiedByROName')
				? appellantAddress_xmlObject.selectSingleNode('//AppellantAddress/AppellantAddressLastModifiedByROName').text : null;

        appModDate = SingleNodeExists(appellantAddress_xmlObject, '//AppellantAddress/AppellantAddressLastModifiedDate')
				? appellantAddress_xmlObject.selectSingleNode('//AppellantAddress/AppellantAddressLastModifiedDate').text : null;

        appWorkPhone = SingleNodeExists(appellantAddress_xmlObject, '//AppellantAddress/AppellantWorkPhoneNumber')
				? appellantAddress_xmlObject.selectSingleNode('//AppellantAddress/AppellantWorkPhoneNumber').text : null;

        appHomePhone = SingleNodeExists(appellantAddress_xmlObject, '//AppellantAddress/AppellantHomePhoneNumber')
				? appellantAddress_xmlObject.selectSingleNode('//AppellantAddress/AppellantHomePhoneNumber').text : null;

        appFName = SingleNodeExists(appellantAddress_xmlObject, '//AppellantFirstName')
				? appellantAddress_xmlObject.selectSingleNode('//AppellantFirstName').text : null;

        appMName = SingleNodeExists(appellantAddress_xmlObject, '//AppellantMiddleInitial')
				? appellantAddress_xmlObject.selectSingleNode('//AppellantMiddleInitial').text : null;

        appLName = SingleNodeExists(appellantAddress_xmlObject, '//AppellantLastName')
				? appellantAddress_xmlObject.selectSingleNode('//AppellantLastName').text : null;

        if (appLine1 && appLName && appFName) {
            _hasAppeals = true;

            Xrm.Page.getAttribute('va_appellantaddresskey').setValue(appAddressKey);
            Xrm.Page.getAttribute('va_apellantaddress1').setValue(appLine1);
            Xrm.Page.getAttribute('va_apellantaddress2').setValue(appLine2);
            Xrm.Page.getAttribute('va_apellantcity').setValue(appCity);
            Xrm.Page.getAttribute('va_apellantstate').setValue(appState);
            Xrm.Page.getAttribute('va_appellantstateoptionset').setValue(GetOptionValue('va_appellantstateoptionset', appState));
            Xrm.Page.getAttribute('va_apellantzipcode').setValue(appZip);
            Xrm.Page.getAttribute('va_apellantcountry').setValue(appCountry);
            Xrm.Page.getAttribute('va_apellanthomephone').setValue(appHomePhone);
            Xrm.Page.getAttribute('va_apellantworkphone').setValue(appWorkPhone);
            Xrm.Page.getAttribute('va_apellantmodifiedby').setValue(appModified);
            Xrm.Page.getAttribute('va_apellantmodifiedon').setValue(appModDate);
            Xrm.Page.getAttribute('va_appellantaddresschanged').setValue(1);
            Xrm.Page.getAttribute('va_webserviceresponse').setSubmitMode('always');
            Xrm.Page.getAttribute('va_appellantaddressresponse').setSubmitMode('always');
            Xrm.Page.getAttribute('va_updateappellantaddressresponse').setSubmitMode('always');
            Xrm.Page.getAttribute('va_apellantmodifiedby').setSubmitMode('always');
            Xrm.Page.getAttribute('va_apellantmodifiedon').setSubmitMode('always');

            var name = (appFName ? appFName + ' ' : '') + (appMName ? appMName + '. ' : '')
					+ (appLName ? appLName : '');

            Xrm.Page.getAttribute('va_name').setValue('Appellant Address Change - ' + name);
        } else {
            _hasAppeals = false;
        }
    }
    ///*********************
    // For Award Address, need to call two WS to get person's data and address data
    //award address is not needed if using vacols only
    // Use Recipient
}

function RetrievePersonalInfo(vetSearchCtx) {
    // Now we need to call ws to get most current person's name and email (findawardadresses ws doesn't return email correctly)
    ShowProgress('Retrieving Personal Information...');
    var findVet = new findVeteranByPtcpntId(vetSearchCtx);
    findVet.executeRequest();

    var AccountNumber = '',
	    RoutingNumber = '',
	    AccountType = null,
	    Address1 = '', Address1p = '',
	    Address2 = '', Address2p = '',
	    Address3 = '', Address3p = '',
	    City = '', Cityp = '',
	    State = '', Statep = '',
	    Zip = '', Zipp = '',
	    Country = '', Countryp = '',
	    PostOfficeTypeCode = '', PostalTypeCode = '',
	    ForeignZipp = '', PostOfficeTypeCodep = '', PostalTypeCodep = '',
	    pid = '',
	    phone1 = '', phone2 = '', phoneType1 = '', phoneType2 = '', area1 = '', area2 = '';

    if (findVet.wsMessage.errorFlag) {
        throw ('Find Veteran web service failed to refresh Name and Email information based on Participant ID.\n\nMid-tier components reported this error: ' + findVet.wsMessage.description);
    }
    else {
        var vet_xmlObject = _XML_UTIL.parseXmlObject(findVet.responseXml);

        var $xml = $($.parseXML(vet_xmlObject.selectSingleNode('//vetCorpRecord').xml));
        $xml.find('eftAccountNumber').text('**********');
        $xml.find('eftRoutingNumber').text('**********');

        var xmlString = undefined;
        if (window.ActiveXObject) {
            xmlString = $xml[0].xml;
        }

        if (xmlString === undefined) {
            xmlString = (new XMLSerializer()).serializeToString($xml[0]);
        }

        Xrm.Page.getAttribute('va_findveteranbyptcpntidresponse').setValue(xmlString);

        // Get file number based on pid search
        var recipSSN = (vet_xmlObject.selectSingleNode('//vetCorpRecord/ssn') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/ssn').text : null);
        pid = (vet_xmlObject.selectSingleNode('//vetCorpRecord/ptcpntId') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/ptcpntId').text : null);

        fn = (vet_xmlObject.selectSingleNode('//vetCorpRecord/firstName') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/firstName').text : null);
        ln = (vet_xmlObject.selectSingleNode('//vetCorpRecord/lastName') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/lastName').text : null);
        mi = (vet_xmlObject.selectSingleNode('//vetCorpRecord/middleName') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/middleName').text : null);
        em = (vet_xmlObject.selectSingleNode('//vetCorpRecord/emailAddress') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/emailAddress').text : null);
        suf = (vet_xmlObject.selectSingleNode('//vetCorpRecord/suffixName') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/suffixName').text : null);
        phone1 = (vet_xmlObject.selectSingleNode('//vetCorpRecord/phoneNumberOne') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/phoneNumberOne').text : null);
        phone2 = (vet_xmlObject.selectSingleNode('//vetCorpRecord/phoneNumberTwo') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/phoneNumberTwo').text : null);
        phoneType1 = (vet_xmlObject.selectSingleNode('//vetCorpRecord/phoneTypeNameOne') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/phoneTypeNameOne').text : null);
        phoneType2 = (vet_xmlObject.selectSingleNode('//vetCorpRecord/phoneTypeNameTwo') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/phoneTypeNameTwo').text : null);
        area1 = (vet_xmlObject.selectSingleNode('//vetCorpRecord/areaNumberOne') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/areaNumberOne').text : null);
        area2 = (vet_xmlObject.selectSingleNode('//vetCorpRecord/areaNumberTwo') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/areaNumberTwo').text : null);

        //CADD Phone1
        if (area1) {
            caddphone1 = FormatTelephone(area1 + phone1);
        }
        else { //10 or 7 digit international
            caddphone1 = phone1;
        }

        //CADD Phone2
        if (area2) {
            caddphone2 = FormatTelephone(area2 + phone2);
        }
        else { //10 or 7 digit international
            caddphone2 = phone2;
        }

        AccountNumber = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/eftAccountNumber') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/eftAccountNumber').text : null;
        RoutingNumber = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/eftRoutingNumber') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/eftRoutingNumber').text : null;
        if (SingleNodeExists(vet_xmlObject, '//vetCorpRecord/eftAccountType')) {
            var type = vet_xmlObject.selectSingleNode('//vetCorpRecord/eftAccountType').text;
            if (type == 'Checking') {
                AccountType = 953850000;
            }
            else if (type == 'Savings') {
                AccountType = 953850001;
            }
        }

        // Mailing
        Address1 = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/addressLine1') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/addressLine1').text : null;
        Address2 = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/addressLine2') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/addressLine2').text : null;
        Address3 = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/addressLine3') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/addressLine3').text : null;
        City = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/city') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/city').text : null;
        State = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/state') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/state').text : null;
        Zip = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/zipCode') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/zipCode').text : null;
        Country = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/country') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/country').text : null;
        FileNumber = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/fileNumber') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/fileNumber').text : null;

        // tas
        // mltyPostOfficeTypeCd and mltyPostalTypeCd
        var PostOfficeTypeCode = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/militaryPostOfficeTypeCode') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/militaryPostOfficeTypeCode').text : null;
        var PostalTypeCode = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/militaryPostalTypeCode') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/militaryPostalTypeCode').text : null;
        var milZipCode = null;
        if (PostOfficeTypeCode) {
            milZipCode = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/zipCode') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/zipCode').text : null;
        }


        // Payment
        Address1p = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/cpPaymentAddressLine1') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/cpPaymentAddressLine1').text : null;
        Address2p = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/cpPaymentAddressLine2') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/cpPaymentAddressLine2').text : null;
        Address3p = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/cpPaymentAddressLine3') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/cpPaymentAddressLine3').text : null;
        Cityp = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/cpPaymentCity') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/cpPaymentCity').text : null;
        Statep = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/cpPaymentState') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/cpPaymentState').text : null;
        Zipp = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/cpPaymentZipCode') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/cpPaymentZipCode').text : null;
        Countryp = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/cpPaymentCountry') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/cpPaymentCountry').text : null;
        ForeignZipp = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/cpPaymentForeignZip') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/cpPaymentForeignZip').text : null;
        PostOfficeTypeCodep = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/PostOfficeTypeCodep') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/PostOfficeTypeCodep').text : null;
        PostalTypeCodep = SingleNodeExists(vet_xmlObject, '//vetCorpRecord/cpPaymentPostalTypeCode') ? vet_xmlObject.selectSingleNode('//vetCorpRecord/cpPaymentPostalTypeCode').text : null;

        Xrm.Page.getAttribute('va_paymentaddress1').setValue(Address1p);
        Xrm.Page.getAttribute('va_paymentaddress2').setValue(Address2p);
        Xrm.Page.getAttribute('va_paymentaddress3').setValue(Address3p);
        Xrm.Page.getAttribute('va_paymentcity').setValue(Cityp);
        Xrm.Page.getAttribute('va_paymentstate').setValue(Statep);
        if (Statep && Statep.length > 0) { Xrm.Page.getAttribute('va_paymentstateoptionset').setValue(GetOptionValue('va_paymentstateoptionset', Statep)); }

        Xrm.Page.getAttribute('va_paymentzipcode').setValue(Zipp);
        Xrm.Page.getAttribute('va_paymentcountry').setValue(Countryp);
        Xrm.Page.getAttribute('va_paymentforeignpostalcode').setValue(ForeignZipp);
    }

    //Xrm.Page.getAttribute('va_firstname').setValue(fn);
    Xrm.Page.getAttribute('va_originalfirstname').setValue(fn);
    //Xrm.Page.getAttribute('va_lastname').setValue(ln);
    Xrm.Page.getAttribute('va_originallastname').setValue(ln);
    //Xrm.Page.getAttribute('va_middlename').setValue(mi);
    Xrm.Page.getAttribute('va_originalmiddlename').setValue(mi);
    //Xrm.Page.getAttribute('va_suffix').setValue(suf);
    Xrm.Page.getAttribute('va_originalsuffix').setValue(suf);
    Xrm.Page.getAttribute('va_email').setValue(em);

    Xrm.Page.getAttribute('va_origarea1').setValue(area1);
    Xrm.Page.getAttribute('va_area1').setValue(area1);
    Xrm.Page.getAttribute('va_origphone1').setValue(phone1);
    Xrm.Page.getAttribute('va_phone1').setValue(phone1);
    Xrm.Page.getAttribute('va_origphone1type').setValue(phoneType1);
    Xrm.Page.getAttribute('va_caddphone1').setValue(caddphone1);
    Xrm.Page.getAttribute('va_caddphone2').setValue(caddphone2);

    if (phoneType1 == 'Nighttime' || phoneType1 == 'NIGHTTIME') {
        Xrm.Page.getAttribute('va_phone1type').setValue(953850001);
    } else {
        Xrm.Page.getAttribute('va_phone1type').setValue(953850000);
    }

    Xrm.Page.getAttribute('va_origarea2').setValue(area2);
    Xrm.Page.getAttribute('va_area2').setValue(area2);
    Xrm.Page.getAttribute('va_origphone2').setValue(phone2);
    Xrm.Page.getAttribute('va_phone2').setValue(phone2);
    Xrm.Page.getAttribute('va_origphone2type').setValue(phoneType2);

    if (phoneType2 == 'Daytime' || phoneType2 == 'DAYTIME') {
        Xrm.Page.getAttribute('va_phone2type').setValue(953850000);
    } else {
        Xrm.Page.getAttribute('va_phone2type').setValue(953850001);
    }

    Xrm.Page.getAttribute('va_depositaccountnumber').setValue(AccountNumber);
    //Xrm.Page.getAttribute('va_account').setValue(AccountNumber);
    Xrm.Page.getAttribute('va_routingnumber').setValue(RoutingNumber);
    Xrm.Page.getAttribute('va_depositaccounttype').setValue(AccountType);

    Xrm.Page.getAttribute('va_mailingaddress1').setValue(Address1);
    Xrm.Page.getAttribute('va_mailingaddress2').setValue(Address2);
    Xrm.Page.getAttribute('va_mailingaddress3').setValue(Address3);
    Xrm.Page.getAttribute('va_mailingcity').setValue(City);
    Xrm.Page.getAttribute('va_mailingstateoptionset').setValue(GetOptionValue('va_mailingstateoptionset', State));
    MailingState_Onchange();
    Xrm.Page.getAttribute('va_mailingaddresszipcode').setValue(Zip);
    Xrm.Page.getAttribute('va_mailingcountry').setValue(Country);


    // update military codes
    if (PostOfficeTypeCode) { SetOptionSetFromValue('va_mailingmilitarypostofficetypecode', PostOfficeTypeCode); }
    if (PostalTypeCode) { SetOptionSetFromValue('va_mailingmilitarypostaltypecode', PostalTypeCode); }
    //    Xrm.Page.getAttribute('va_milzipcodelookupid').setValue(FileNumber);
    if (milZipCode) {

        // rest call to get GUID
        var milzipGuid;
        var filter = 'va_zip eq "' + milZipCode + '"';
        CrmRestKit2011.ByQuery('va_milzipcodelookup', ['va_milzipcodelookupid'], filter, false).done(function (data) {
            milzipGuid = data.d.Results[0].va_milzipcodelookupid;
        });

        var lookup = new Array();
        lookup[0] = new Object();
        lookup[0].id = milzipGuid;
        lookup[0].name = milZipCode;
        lookup[0].entityType = 'va_milzipcodelookup';
        Xrm.Page.getAttribute("va_milzipcodelookupid").setValue(lookup);
    }

    Xrm.Page.getAttribute('va_verifyaddress1').setValue(Address1);
    Xrm.Page.getAttribute('va_verifyaddress2').setValue(Address2);
    Xrm.Page.getAttribute('va_verifyaddress3').setValue(Address3);
    Xrm.Page.getAttribute('va_verifycity').setValue(City);
    Xrm.Page.getAttribute('va_verifystate').setValue(State);
    Xrm.Page.getAttribute('va_verifyzipcode').setValue(Zip);
    Xrm.Page.getAttribute('va_verifycountry').setValue(Country);

    //Xrm.Page.getAttribute('va_account').setSubmitMode('always');
    Xrm.Page.getAttribute('va_verifyaddress1').setSubmitMode('always');
    Xrm.Page.getAttribute('va_verifyaddress2').setSubmitMode('always');
    Xrm.Page.getAttribute('va_verifyaddress3').setSubmitMode('always');
    Xrm.Page.getAttribute('va_verifycity').setSubmitMode('always');
    Xrm.Page.getAttribute('va_verifystate').setSubmitMode('always');
    Xrm.Page.getAttribute('va_verifyzipcode').setSubmitMode('always');
    Xrm.Page.getAttribute('va_verifycountry').setSubmitMode('always');

    Xrm.Page.getAttribute('va_name').setValue(Xrm.Page.getAttribute('va_originalfirstname').getValue() + ' ' + Xrm.Page.getAttribute('va_originallastname').getValue() + ' - ' + new Date().format("MM/dd/yyyy").toString());

    // if from claims, set DDEFT exists field
    if (_fromClaims) {
        Xrm.Page.getAttribute('va_participantrecipid').setValue(pid);
        Xrm.Page.getAttribute('va_participantvetid').setValue(pid);

        //        if (AccountNumber && AccountNumber.length > 0 && RoutingNumber && RoutingNumber.length > 0) {
        //            Xrm.Page.getAttribute('va_directdepositexistsindicator').setValue(true);
        //        }
        //        else {
        //            Xrm.Page.getAttribute('va_directdepositexistsindicator').setValue(false);
        //        }

        // address indicators - mailling
        //        if (Address1 && Address1.length > 0 && City && City.length > 0) {
        //            Xrm.Page.getAttribute('va_mailingaddressexistsindicator').setValue(true);
        //        } else {
        //            Xrm.Page.getAttribute('va_mailingaddressexistsindicator').setValue(false);
        //        }

        // address indicators - payment
        //        if (Address1p && Address1p.length > 0 && Cityp && Cityp.length > 0) {
        //            Xrm.Page.getAttribute('va_paymentaddressexistsindicator').setValue(true);
        //        } else {
        //            Xrm.Page.getAttribute('va_paymentaddressexistsindicator').setValue(false);
        //        }
    }
}

function FormatTelephone(telephoneNumber) {
    var Phone = telephoneNumber;
    var ext = '';
    var result;

    if (0 != Phone.indexOf('+')) {
        if (1 < Phone.lastIndexOf('x')) {
            ext = Phone.slice(Phone.lastIndexOf('x'));
            Phone = Phone.slice(0, Phone.lastIndexOf('x'));
        }

        Phone = Phone.replace(/[^\d]/gi, '');
        result = Phone;
        if (7 == Phone.length) {
            result = Phone.slice(0, 3) + '-' + Phone.slice(3)
        }
        if (10 == Phone.length) {
            result = '(' + Phone.slice(0, 3) + ') ' + Phone.slice(3, 6) + '-' + Phone.slice(6);
        }
        if (0 < ext.length) {
            result = result + ' ' + ext;
        }
        return result;
    }
}

//On Change Phone
function fnphoneOnChange1() {
    Phone1 = Xrm.Page.getAttribute('va_caddphone1').getValue();

    if (Phone1 != null) {
        Phone1 = Phone1.replace(/[^\d]/gi, '');
    }

    if (Xrm.Page.getAttribute('va_addresstype').getValue() === 953850000 && Phone1 != null) {
        //write back to WS fields   
        Xrm.Page.getAttribute('va_area1').setValue(Phone1.slice(0, 3));
        Xrm.Page.getAttribute('va_phone1').setValue(Phone1.slice(3));

        //reformat the same fields
        Xrm.Page.getAttribute('va_caddphone1').setValue(FormatTelephone(Phone1));
    }
    else {
        Xrm.Page.getAttribute('va_caddphone1').setValue(Phone1);
        Xrm.Page.getAttribute('va_area1').setValue(null);
        Xrm.Page.getAttribute('va_phone1').setValue(Phone1);
    }
}

function fnphoneOnChange2() {
    Phone2 = Xrm.Page.getAttribute('va_caddphone2').getValue();

    if (Phone2 != null) {
        Phone2 = Phone2.replace(/[^\d]/gi, '');
    }

    if (Xrm.Page.getAttribute('va_addresstype').getValue() === 953850000 && Phone2 != null) {
        //write back to WS fields   
        Xrm.Page.getAttribute('va_area2').setValue(Phone2.slice(0, 3));
        Xrm.Page.getAttribute('va_phone2').setValue(Phone2.slice(3));

        //reformat the same fields
        Xrm.Page.getAttribute('va_caddphone2').setValue(FormatTelephone(Phone2));
    }
    else {
        Xrm.Page.getAttribute('va_caddphone2').setValue(Phone2);
        Xrm.Page.getAttribute('va_area2').setValue(null);
        Xrm.Page.getAttribute('va_phone2').setValue(Phone2);
    }
}

function convertDate(dateString) {
    var p = dateString.split(/\D/g)
    return [p[1], p[2], p[0]].join("/")
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

                Xrm.Page.getControl('va_mailingcountrylist').addOption(oOption);
                Xrm.Page.getControl('va_paymentcountrylist').addOption(oOption);
            }
        }
    }
}

function MilZipCodeChange() {
    var selectedZip = '';
    if (Xrm.Page.getAttribute("va_milzipcodelookupid").getValue() &&
			Xrm.Page.getAttribute("va_milzipcodelookupid").getValue().length > 0 &&
			Xrm.Page.getAttribute("va_milzipcodelookupid").getValue()[0]) {
        selectedZip = Xrm.Page.getAttribute("va_milzipcodelookupid").getValue()[0].name;
    }
    if (selectedZip) { Xrm.Page.getAttribute('va_mailingaddresszipcode').setValue(selectedZip); }
}

function SetOptionSetFromValue(controlName, optionText) {
    var options = Xrm.Page.getAttribute(controlName).getOptions();
    if (!options || !optionText) { return; }

    optionText = optionText.toUpperCase();
    for (var k = 0; k < options.length; k++) {
        if (options[k].text.toUpperCase() == optionText) {
            Xrm.Page.getAttribute(controlName).setValue(options[k].value);
            return;
        }
    }
}

function CPLink() {
    var width = 1024;
    var height = 768;
    var top = (screen.height - height) / 2;
    var left = (screen.width - width) / 2;
    var params = "width=" + width + ",height=" + height + ",location=0,menubar=0,toolbar=0,top=" + top + ",left=" + left + ",status=0,titlebar=no,resizable=yes";
    var win = window.open('https://vaww.vrm.km.va.gov/system/templates/selfservice/va_ka/portal.html?portalid=554400000001001&articleid=554400000002433', 'CP', params);
}

function EdLink() {
    var width = 1024;
    var height = 768;
    var top = (screen.height - height) / 2;
    var left = (screen.width - width) / 2;
    var params = "width=" + width + ",height=" + height + ",location=0,menubar=0,toolbar=0,scrollbars=1,top=" + top + ",left=" + left + ",status=0,titlebar=no,resizable=yes";
    var win = window.open('https://vaww.vrm.km.va.gov/system/templates/selfservice/va_ka/portal.html?portalid=554400000001001&articleid=554400000003847', 'ED', params);
}

function ViewScript() {
    var scriptSource = Xrm.Page.context.getClientUrl().replace(Xrm.Page.context.getOrgUniqueName(), '') + 'isv/scripts_VIP/' + 'changeOfAddress.html';

    if (!_scriptWindowHandle || _scriptWindowHandle.closed) {
        _scriptWindowHandle = window.open(scriptSource, "CallScript", "width=600,height=500,scrollbars=1,resizable=1");
    }
    else {
        _scriptWindowHandle.open(scriptSource, "CallScript", "width=600,height=500,scrollbars=1,resizable=1");
    }

    try { _scriptWindowHandle.focus(); }
    catch (er) { }
}

function CADDIdProofingComplete() {
    var attr = [
			   Xrm.Page.getAttribute("va_currentmonthlybenefit"),
		   	   Xrm.Page.getAttribute("va_addressofrecord"),
			   Xrm.Page.getAttribute("va_dobverified")
    ];
    for (var a in attr) {
        attr[a].setValue(true);
    }
    Xrm.Page.getAttribute("va_failedidproofing").setValue(false);
    alert('ID Proofing boxes have been checked to indicate successful Proofing for Change of Address.');
    // fnIDProof();
}

function setHeaderData(page) {
    if (!page) return;

    var ga = page.getAttribute;

    var getToolTip = function (fieldId) {
        var toolTip = "";
        if (!fieldId) return toolTip;
        var splitValues = ga(fieldId).getValue() ? ga(fieldId).getValue().split(";") : [];
        for (var i = 0; i < splitValues.length; i++) {
            toolTip += splitValues[i] + "\r\n";
        }

        return toolTip;
    };

    var firstname = '';
    var lastname = '';
    var phonenumber = '';
    var flags = '';
    var flags_tooltip = '';
    var flashes = '';
    var flashes_tooltip = '';

    if (page.data.entity.getEntityName() == 'contact') {
        firstname = ga('firstname').getValue();
        lastname = ga('lastname').getValue();
        phonenumber = ga('address1_telephone1').getValue();
        //flags = ga('va_flags').getValue();
        //flags_tooltip = getToolTip('va_flags');
        //flashes = ga('va_flashes').getValue();
        //flashes_tooltip = getToolTip('va_flashes');
    }
    else {
        firstname = ga('va_firstname').getValue();
        lastname = ga('va_lastname').getValue();
        phonenumber = ga('phonenumber').getValue();
        flags = ga('va_flags').getValue();
        flags_tooltip = getToolTip('va_flags');
        flashes = ga('va_flashes').getValue();
        flashes_tooltip = getToolTip('va_flashes');
    }

    setHeaderFieldValue('va_caddfirstname', firstname, null, 'solid 1px blue', 'blue', null, null, '');
    setHeaderFieldValue('va_caddlastname', lastname, null, 'solid 1px blue', 'blue', null, null, '');
    setHeaderFieldValue('va_caddphonenumber', getPhoneNumberFromCadd(phonenumber, Xrm.Page.getAttribute("va_caddphone1").getValue(), Xrm.Page.getAttribute("va_caddphone2").getValue()), null, 'solid 1px blue', 'blue', null, null, '');
    setHeaderFieldValue('va_caddssn', ga('va_ssn').getValue(), null, 'solid 1px blue', 'blue', null, null, '');
    setHeaderFieldValue('va_flags', flags, null, 'solid 1px blue', 'blue', null, null, flags_tooltip);
    setHeaderFieldValue('va_flashes', flashes, null, 'solid 1px blue', 'blue', null, null, flags_tooltip);
}

function setHeaderFieldValue(fieldId, newText, textDecoration, underline, color, fontsize, cursor, tooltipText) {
    var id = "header_" + fieldId + "_d";
    var element = document.getElementById(id);
    if (!element) return;
    var field = element.childNodes[0];
    if (newText) field.innerText = newText;
    if (color) field.style.color = color;
    field.style.fontSize = 'small';
    if (tooltipText) field.title = tooltipText;

    if (!Xrm.Page.getAttribute(fieldId).getValue())
        Xrm.Page.getAttribute(fieldId).setValue(newText);
}

function getPhoneNumberFromCadd(parentPhoneNumber, phone1, phone2) {
    var getPhoneNumber = phone1;
    if (!getPhoneNumber)
        getPhoneNumber = phone2;

    return getPhoneNumber || parentPhoneNumber || "";
}

//ID Proofing: ToggleTabs (called from button or click of each checkbox)
function fnIDProof() {
    //Check bits and set global id var
    if ((Xrm.Page.getAttribute("va_currentmonthlybenefit").getValue() == true) && (Xrm.Page.getAttribute("va_addressofrecord").getValue() == true) && (Xrm.Page.getAttribute("va_dobverified").getValue() == true) && (Xrm.Page.getAttribute("va_failedidproofing").getValue() == false)) {
        globalcadd.idproof = "passed";
    }
    if (Xrm.Page.getAttribute("va_failedidproofing").getValue() == true) {
        globalcadd.idproof = "failed";
    }
    if ((Xrm.Page.getAttribute("va_currentmonthlybenefit").getValue() == false) || (Xrm.Page.getAttribute("va_addressofrecord").getValue() == false) || (Xrm.Page.getAttribute("va_dobverified").getValue() == false)) {
        globalcadd.idproof = "";
    }

    //Toggle Tabs based on global var
    if ((globalcadd.idproof == "") || (globalcadd.idproof == "failed")) {
        Xrm.Page.ui.tabs.get("General").setVisible(false);
        Xrm.Page.ui.tabs.get("Notes").setVisible(false);
        Xrm.Page.ui.tabs.get("tab_3").setVisible(false);
    }
    if (globalcadd.idproof == "passed") {
        Xrm.Page.ui.tabs.get("General").setVisible(true);
        Xrm.Page.ui.tabs.get("Notes").setVisible(true);
        Xrm.Page.ui.tabs.get("tab_3").setVisible(true);
    }
}

//set flag when any of the fields in the sections are changed for notes. Call this function on change from every field we are checking.

function setChangeCheckBox(field) {
    var checkbox = Xrm.Page.getAttribute(field);
    if (checkbox != 1) {
        checkbox.setValue(1);
    }
}

//If CADD intiated from Claim 
//    and Claim payee has any awards
//        Then disable rounting and account number fields

//VTRIGIILI 01-29-2015 - fixed typo in the spelling of opener that was
//         causing IE to throw errors.
function setRoutingAndAccountNumber(opener, Parent_Page) {

    var awardInformation_xml = Parent_Page.getAttribute('va_generalinformationresponse').getValue();

    if (opener._changeOfAddressData.openedFromClaimTab) {
        var claimPayeeCode = opener._changeOfAddressData.payeeTypeCode;

        if (awardInformation_xml != null) {
            var awardInformation_xmlObject = _XML_UTIL.parseXmlObject(awardInformation_xml);

            var numberOfAwardBenes = SingleNodeExists(awardInformation_xmlObject, '//return/numberOfAwardBenes')
                   ? awardInformation_xmlObject.selectSingleNode('//return/numberOfAwardBenes').text : null;

            var singlePayeeCode = SingleNodeExists(awardInformation_xmlObject, '//return/payeeTypeCode')
                   ? awardInformation_xmlObject.selectSingleNode('//return/payeeTypeCode').text : null;

            var payeeCodeMatch = false;

            if (numberOfAwardBenes == '0') { return; }
            else if (singlePayeeCode != null) {
                if (singlePayeeCode == claimPayeeCode) {
                    payeeCodeMatch = true;
                }
            }
            else {
                var returnNode = awardInformation_xmlObject.selectNodes('//return');
                var awardNodes = returnNode[0].childNodes;

                for (var i = 0; i < awardNodes.length; i++) { //looping through awards and
                    if (awardNodes[i].nodeName == 'awardBenes') { //checking relevant Nodes
                        if (awardNodes[i].selectSingleNode('payeeCd').text == claimPayeeCode) {
                            payeeCodeMatch = true;
                            i = awardNodes.length;
                        }
                    }
                }
            }

            if (payeeCodeMatch) {
                Xrm.Page.ui.controls.get("va_depositaccounttype").setDisabled(true);
                Xrm.Page.ui.controls.get("va_routingnumber").setDisabled(true);
                Xrm.Page.ui.controls.get("va_depositaccountnumber").setDisabled(true);
            }
        }

        return;
    }
    return;
}
