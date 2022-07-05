//Looks up caller first and last name from phone call
function getfullnamefnod() {

    var firstname = parent_page.getAttribute('va_callerfirstname').getValue();
    var lastname = parent_page.getAttribute('va_callerlastname').getValue();
    var FNODfullname = Xrm.Page.getAttribute('va_newpmcrecipname').getValue();
    if (FNODfullname == null) {
        var phonecallfullname = firstname + ' ' + lastname;

        if (firstname != null) {
            Xrm.Page.getAttribute('va_newpmcrecipname').setValue(phonecallfullname);
        }
        else {
            Xrm.Page.getAttribute('va_newpmcrecipname').setValue(lastname);
        }
    }
}


//changes first letter to upper case and all others to lover
//field=Xrm.Page.getAttribute('va_newpmcvetfirstname');
function toUpperCase(field) {
    var str = field.getValue();
    if (field.getValue() != null) {
        str = str.toLowerCase().replace(/\b[a-z]/g, function (letter) {
            return letter.toUpperCase();
        });
        field.setValue(str);
    }
}


function setfullNameOnFNOD(xrm, parentXrm) {
    Xrm.Page.getAttribute('va_newpmcrecipname').setValue(
        getfullName(parentXrm.getAttribute('va_callerfirstname').getValue(),
            parentXrm.getAttribute('va_callerlastname').getValue(),
            xrm.Page.getAttribute('va_newpmcrecipname').getValue())
    );
}


function getfullName(firstname, lastname, FNODfullname) {
    if (FNODfullname === null) {
        var phonecallfullname = firstname + ' ' + lastname;

        if (firstname !== null) {
            return phonecallfullname
        } else {
            return lastname
        }
    }

    return FNODfullname;
}

function setFirstCharacterToUpperCaseOnFNOD(fieldName) {
    xrm.Page.getAttribute(fieldName).setValue(toUppeerCaseFirstCharacter(xrm.Page.getAttribute(fieldName).getValue()));
}

function toUpperCaseFirstCharacter(value) {
    if (value) {
        return value.toLowerCase().replace(/\b[a-z]/g, function (letter) {
            return letter.toUpperCase();
        });
    }
}


//************************************************************************************************************
// Module
// Loads the Awards, CORP, BIRLS tabs on the FNOD form
//************************************************************************************************************
(function (mod) {
    mod.isTest = false;
    var xmlUtils = null;
    var utils = function () {
        if (xmlUtils === null)
            xmlUtils = new XmlUtilities();
        return xmlUtils;
    };

    var hasMultipleAwards = function (xmlDoc) {
        return xmlDoc.selectSingleNode("//return/awardBenes");
    };

    var loadMultiAwards = function (xmlDoc) {
        var award = { 'awardType': '', 'awardStatus': '' },
            awardsXml = xmlDoc.selectSingleNode("//return"),
            code, payee;

        if (awardsXml) {
            for (var i = 0; i < awardsXml.childNodes.length; i++) {
                if (awardsXml.childNodes[i].nodeName === "awardBenes" && awardsXml.childNodes[i].nodeType === 1) {
                    code = awardsXml.childNodes[i].selectSingleNode("awardBenePK/awardTypeCd");
                    payee = awardsXml.childNodes[i].selectSingleNode("payeeCd");

                    if (code && code.text === "CPL" && payee && payee.text === "00") {
                        award.awardType = code.text;
                        award.ptcpntVetId = getText(awardsXml.childNodes[i].selectSingleNode("awardBenePK/ptcpntVetId"));
                        award.ptcpntBeneId = getText(awardsXml.childNodes[i].selectSingleNode("awardBenePK/ptcpntBeneId"));
                        award.ptcpntRecipId = getText(awardsXml.childNodes[i].selectSingleNode("awardBenePK/ptcpntRecipId"));

                        award.awardStatus = getAwardStatus(getSingleAwardInfoFromWs(award));

                        return award;
                    }
                }
            }
        }

        return award;
    };

    var loadSingleAwards = function (xmlDoc) {
        var awardType = xmlDoc.selectSingleNode("//return/awardTypeCode"),
            awardStatus = xmlDoc.selectSingleNode("//return/payStatusTypeName");

        return { 'awardType': getText(awardType), 'awardStatus': getText(awardStatus) };
    };

    var getText = function (object) {
        return object ? object.text : '';
    };

    var getSingleAwardInfoFromWs = function (award) {
        if (mod.isTest) return getAwardInfoFromStaticXml();

        var context, findOtherAwardInformationService;

        try {
            context = new vrmContext();
            context.user = GetUserSettingsForWebservice();
            context.parameters['awardTypeCd'] = award.awardType;
            context.parameters['ptcpntVetId'] = award.ptcpntVetId;
            context.parameters['ptcpntBeneId'] = award.ptcpntBeneId;
            context.parameters['ptcpntRecipId'] = award.ptcpntRecipId;

            findOtherAwardInformationService = new findOtherAwardInformation(context);
            findOtherAwardInformationService.executeRequest();
            
            if (findOtherAwardInformationService.wsMessage.errorFlag) {
                alert("There was an error calling the 'findOtherAwardInformationService' WS to get the Award Status: " + findOtherAwardInformationService.wsMessage.description);
                return '';
            }
            
            return findOtherAwardInformationService.responseXml.replace(/\"/g,"'");
            
        } finally {
             CloseProgress();
        }
    };

    var getAwardInfoFromStaticXml = function () {
        return '';
    };

    var getAwardStatus = function (xml) {
        if (!xml) return '';

        var xmlDoc = utils().parseXmlObject(xml);
        if (xmlDoc)
            return getText(xmlDoc.selectSingleNode("//return/awardInfo/payStatusName"));

        return '';
    };

    var dateSortDesc = function (object1, object2) {
        if (object1.date > object2.date) return -1;
        if (object1.date < object2.date) return 1;
        return 0;
    };

    var fixDate = function (value) {
        if (value.length === 8) {
            return value.substring(0, 2) + '/' + value.substring(2, 4) + '/' + value.substring(4, 8);
        }

        return '01/01/1800';
    };
    
    var isEmptyBirlsServiceNode = function (node) {
        return (!(node.selectSingleNode('BRANCH_OF_SERVICE') ? node.selectSingleNode('BRANCH_OF_SERVICE').text : '') &&
            !(node.selectSingleNode('ENTERED_ON_DUTY_DATE') ? node.selectSingleNode('ENTERED_ON_DUTY_DATE').text : ''));
    };

    mod.getDisabilityRatingInfo = function (xml) {
        if (!xml) return '';

        var xmlDoc = utils().parseXmlObject(xml);
        if (!xmlDoc) return '';

        return getText(xmlDoc.selectSingleNode('//return/disabilityRatingRecord/serviceConnectedCombinedDegree'));
    };

    mod.getAwardInfo = function (xml) {
        var award = { 'awardType': '', 'awardStatus': '' },
            xmlDoc;

        if (!xml) return award;

        xmlDoc = utils().parseXmlObject(xml);
        if (!xmlDoc) return award;

        if (hasMultipleAwards(xmlDoc)) {
            award = loadMultiAwards(xmlDoc);
        } else {
            award = loadSingleAwards(xmlDoc);
        }

        return award;
    };

    mod.getMilitaryCorpInfo = function (xml) {
        var corp = { separationReason: '', dischargeType: '', verified: '' },
            xmlDoc, services, tours = [];

        if (!xml) return corp;

        xmlDoc = utils().parseXmlObject(xml);
        if (!xmlDoc) return corp;

        services = xmlDoc.selectSingleNode('//return/militaryTours');
        if (services) {
            for (var i = 0; i < services.childNodes.length; i++) {
                if (services.childNodes[i].nodeName === "militaryPersonTours" && services.childNodes[i].nodeType === 1) {
                    tours.push({
                        separationReason: getText(services.childNodes[i].selectSingleNode("militarySeperationReasonTypeName")),
                        dischargeType: getText(services.childNodes[i].selectSingleNode("mpDischargeCharTypeName")),
                        verified: getText(services.childNodes[i].selectSingleNode("verifiedInd")),
                        date: services.childNodes[i].selectSingleNode("radDate") ? new Date(services.childNodes[i].selectSingleNode("radDate").text) : new Date('01/01/1800')
                    });
                }
            }

            if (tours.length > 0) {
                tours.sort(dateSortDesc);
                corp = tours.shift();
            }
        }

        return corp;
    };

    mod.getMilitaryBirlsInfo = function (xml) {
        var birls = { multyService: false, bos: '', serviceVerified: '', characterService: '' },
            xmlDoc, services, tours = [], multyService = false;

        if (!xml) return birls;

        xmlDoc = utils().parseXmlObject(xml);
        if (!xmlDoc) return birls;

        services = xmlDoc.selectSingleNode('//return/services');
        if (services) {
            for (var i = 0; i < services.childNodes.length; i++) {
                if (services.childNodes[i].nodeName === "SERVICE" && services.childNodes[i].nodeType === 1) {
                    if (!isEmptyBirlsServiceNode(services.childNodes[i])) {
                    tours.push({
                            index: i,
                        bos: getText(services.childNodes[i].selectSingleNode("BRANCH_OF_SERVICE")),
                        characterService: getText(services.childNodes[i].selectSingleNode("CHAR_OF_SVC_CODE")),
                        date: services.childNodes[i].selectSingleNode("RELEASED_ACTIVE_DUTY_DATE") ? new Date(fixDate(services.childNodes[i].selectSingleNode("RELEASED_ACTIVE_DUTY_DATE").text)) : new Date('01/01/1800')
                    });
                }
            }
            }

            if (tours.length > 0) {
                multyService = tours.length > 1;
                tours.sort(dateSortDesc);

                birls = tours.shift();
                birls.multyService = multyService;
                if (birls.index === 0) {
                    birls.serviceVerified = getText(xmlDoc.selectSingleNode('//return/VERIFIED_SVC_DATA_IND'));
                } else {
                birls.serviceVerified = getText(xmlDoc.selectSingleNode('//return/VERIFIED_SVC_DATA_IND' + (birls.index + 1)));
                }
            }
        }

        return birls;
    };

    mod.setAwardsCorpAndBirlsSections = function (xrm, parentXrm) {

        var award = mod.getAwardInfo(parentXrm.getAttribute('va_generalinformationresponse').getValue());
        var ratings = mod.getDisabilityRatingInfo(parentXrm.getAttribute('va_findratingdataresponse').getValue());
        var militaryCorp = mod.getMilitaryCorpInfo(parentXrm.getAttribute('va_findmilitaryrecordbyptcpntidresponse').getValue());
        var militaryBirls = mod.getMilitaryBirlsInfo(parentXrm.getAttribute('va_findbirlsresponse').getValue());

        // awards
        xrm.Page.getAttribute('va_awardcode').setValue(award.awardType);
        xrm.Page.getAttribute('va_awardatatus').setValue(award.awardStatus);
        xrm.Page.getAttribute('va_awardratings').setValue(ratings);

        // corps
        xrm.Page.getAttribute('va_corpmilitaryseparationreason').setValue(militaryCorp.separationReason);
        xrm.Page.getAttribute('va_corpmilitarydischargetype').setValue(militaryCorp.dischargeType);
        xrm.Page.getAttribute('va_corpverified').setValue(militaryCorp.verified);

        // burils
        xrm.Page.getAttribute('va_birlsmultiperiodservice').setValue(militaryBirls.multyService);
        xrm.Page.getAttribute('va_birlsbos').setValue(militaryBirls.bos);
        xrm.Page.getAttribute('va_birlsservice1verified').setValue(militaryBirls.serviceVerified);
        xrm.Page.getAttribute('va_birlscharacterofservice').setValue(militaryBirls.characterService);
    };

} (window._fnod_ = window._fnod_ || {}));


//************************************************************************************************************
// Module
// MOD addresses for LastKnownAddress operations
// Set Visible 3 sections
//************************************************************************************************************
(function (mod) {
    var xmlUtils = null;
    var utils = function () {
        if (xmlUtils === null)
            xmlUtils = new XmlUtilities();
        return xmlUtils;
    };

    mod.parseAddress = function (address) {
        var addressParse = '';

        if (!address.isEmpty) {
            addressParse += address.address1;
            addressParse += address.address2 ? '\n' + address.address2 : '';
            addressParse += address.address3 ? '\n' + address.address3 : '';
            addressParse += address.city ? '\n' + address.city : '';
            addressParse += address.state ? ', ' + address.state : '';
            addressParse += address.zip ? ' ' + address.zip : '';
            addressParse += address.country ? '\n' + address.country : '';
            addressParse += address.mltyPostOfficeType ? '\n' + address.mltyPostOfficeType : '';
            addressParse += address.mltyPostalType ? ' ' + address.mltyPostalType : '';
        }

        return addressParse;
    };

    mod.getAddress = function (xml, type) {
        var address = { address1: '', address2: '', address3: '', city: '', state: '', zip: '', country: '', forgeinPostalCode: '', mltyPostOfficeType: '', mltyPostalType: '', isEmpty: true },
            addressNodes, i, xmlDoc;

        if (!xml) return address;
        if (!type) return address;

        xmlDoc = utils().parseXmlObject(xml);
        if (!xmlDoc) return address;

        addressNodes = xmlDoc.selectNodes('//return');
        if (!addressNodes) return address;

        for (i = 0; i < addressNodes.length; i++) { //looping through addresses and
            if (addressNodes[i].selectSingleNode('ptcpntAddrsTypeNm') && addressNodes[i].selectSingleNode('ptcpntAddrsTypeNm').text == type) {

                address.address1 = addressNodes[i].selectSingleNode('addrsOneTxt') ? addressNodes[i].selectSingleNode('addrsOneTxt').text : '';
                address.address2 = addressNodes[i].selectSingleNode('addrsTwoTxt') ? addressNodes[i].selectSingleNode('addrsTwoTxt').text : '';
                address.address3 = addressNodes[i].selectSingleNode('addrsThreeTxt') ? addressNodes[i].selectSingleNode('addrsThreeTxt').text : '';
                address.city = addressNodes[i].selectSingleNode('cityNm') ? addressNodes[i].selectSingleNode('cityNm').text : '';
                address.state = addressNodes[i].selectSingleNode('postalCd') ? addressNodes[i].selectSingleNode('postalCd').text : '';
                address.zip = addressNodes[i].selectSingleNode('zipPrefixNbr') ? addressNodes[i].selectSingleNode('zipPrefixNbr').text : '';
                address.country = addressNodes[i].selectSingleNode('cntryNm') ? addressNodes[i].selectSingleNode('cntryNm').text : '';
                address.forgeinPostalCode = addressNodes[i].selectSingleNode('frgnPostalCd') ? addressNodes[i].selectSingleNode('frgnPostalCd').text : '';
                address.mltyPostOfficeType = addressNodes[i].selectSingleNode('mltyPostOfficeTypeCd') ? addressNodes[i].selectSingleNode('mltyPostOfficeTypeCd').text : ''; //APO
                address.mltyPostalType = addressNodes[i].selectSingleNode('mltyPostalTypeCd') ? addressNodes[i].selectSingleNode('mltyPostalTypeCd').text : ''; //AE
                address.isEmpty = false;
                break;
            }
        }

        return address;

    };

    mod.prepareLastKnownAddressForSpouseFields = function (xml, stateOptions) {
        var stateValue = null,
            addressResult = {},
            address, code;

        address = mod.getAddress(xml, 'Mailing');
        if (address.isEmpty) return;

        addressResult.address1 = address.address1 || null;
        addressResult.address2 = address.address2 || null;
        addressResult.address3 = address.address3 || null;
        addressResult.city = address.city || null;

        if (address.state) {
            for (i = 0; i < stateOptions.length; i++) {
                if (stateOptions[i].text === address.state) {
                    stateValue = stateOptions[i].value;
                    break;
                }
            }
        }

        addressResult.state = stateValue;
        addressResult.zip = address.zip || null;
        addressResult.country = address.country ? address.country.toUpperCase() : null;
        addressResult.forgeinPostalCode = address.forgeinPostalCode || null;

        switch (address.mltyPostalType) {
            case 'AA':
                code = 953850000;
                break;
            case 'AE':
                code = 953850001;
                break;
            case 'AP':
                code = 953850002;
                break;
            default:
                code = null;
                break;
        }

        addressResult.mltyPostalType = code;

        switch (address.mltyPostOfficeType) {
            case 'APO':
                code = 953850000;
                break;
            case 'DPO':
                code = 953850001;
                break;
            case 'FPO':
                code = 953850002;
                break;
            default:
                code = null;
                break;
        }

        addressResult.mltyPostOfficeType = code;
        
        addressResult.spouseAddressType = null;

        if (address.mltyPostalType || address.mltyPostOfficeType) {
            // Military
            addressResult.spouseAddressType = 953850002;
        } else {
            if (address.country === 'USA') {
                // Domestic
                addressResult.spouseAddressType = 953850000;
            } else {
                // International
                addressResult.spouseAddressType = 953850001;
            }
        }

        return addressResult;
    };

    mod.copyLastKnownAddressToSpouseFields = function (xrm, xml) {
        var address;

        if (xrm.Page.getAttribute('va_spouseaddress1').getValue() || xrm.Page.getAttribute('va_spouseaddress2').getValue() || xrm.Page.getAttribute('va_spousecity').getValue() || xrm.Page.getAttribute('va_spousezipcode').getValue()) {
            if (!confirm('You are about to overwrite an existing address. Please click OK to confirm.')) {
                return;
            }
        }

        address = mod.prepareLastKnownAddressForSpouseFields(xml, xrm.Page.getAttribute('va_spousestatelist').getOptions());

        if (address) {
            xrm.Page.getAttribute('va_spouseaddress1').setValue(address.address1);
            xrm.Page.getAttribute('va_spouseaddress2').setValue(address.address2);
            xrm.Page.getAttribute('va_spouseaddress3').setValue(address.address3);
            xrm.Page.getAttribute('va_spousecity').setValue(address.city);
            xrm.Page.getAttribute('va_spousestatelist').setValue(address.state);
            xrm.Page.getAttribute('va_spousezipcode').setValue(address.zip);
            xrm.Page.getAttribute('va_spousecountry').setValue(address.country);
            xrm.Page.getAttribute('va_spouseforeignpostalcode').setValue(address.forgeinPostalCode);
            xrm.Page.getAttribute('va_spouseoverseasmilitarypostalcode').setValue(address.mltyPostalType);
            xrm.Page.getAttribute('va_spouseoverseasmilitarypostofficetypecode').setValue(address.mltyPostOfficeType);
            xrm.Page.getAttribute('va_spouseaddresstype').setValue(address.spouseAddressType);
        }
    };

} (window._fnod_ = window._fnod_ || {}));


//************************************************************************************************************
// Module
// MOD Validation Module
//************************************************************************************************************
(function (mod, undefined) {

    var validationActions = {
        
        // If address type is domestic, zip code can not be empty
        zipCodeExists: function () {
            if (arguments.length !== 2) return false;

            var addressType = arguments[0],
                zipCodeValue = arguments[1];

            if (addressType === 953850000 && !zipCodeValue) {
                alert("The zip code is required when the address is of domestic type!")
                return false;
            }

            return true;
        }
        
    };

    mod.validate = function (actions) {
        if (!actions || actions.length === 0) return true;

        var i, action, params;

        for (i = 0; i < actions.length; i++) {

            action = actions[i].action;
            params = actions[i].parameters;

            if (action && validationActions[action] !== undefined) {
                if (!validationActions[action].apply(this, params)) return false;
            }
        }

        return true;
    };

} (window._fnod_ = window._fnod_ || {}));