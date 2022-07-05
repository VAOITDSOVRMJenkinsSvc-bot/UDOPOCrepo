/**
* @author Ivan Yurisevic
* @class VIP.model.personinfo.Addresses
*
* The model for addresses associated with the person
*/
Ext.define('VIP.model.personinfo.Addresses', {
    extend: 'Ext.data.Model',
    requires: [
		'VIP.soap.envelopes.share.address.FindAllPtcpntAddrsByPtcpntId'
    ],
    fields: [{
        name: 'participantAddressTypeName',
        mapping: 'ptcpntAddrsTypeNm',
        type: 'string'
    }, {
        name: 'effectiveDate',
        mapping: 'efctvDt',
        type: 'date',
        dateFormat: 'c'
    }, {
        name: 'emailAddress',
        mapping: 'emailAddrsTxt',
        type: 'string'
    }, {
        name: 'address1',
        mapping: 'addrsOneTxt',
        type: 'string'
    }, {
        name: 'address2',
        mapping: 'addrsTwoTxt',
        type: 'string'
    }, {
        name: 'address3',
        mapping: 'addrsThreeTxt',
        type: 'string'
    }, {
        name: 'city',
        mapping: 'cityNm',
        type: 'string'
    }, {
        name: 'postalCode',
        mapping: 'postalCd',
        type: 'string'
    }, {
        name: 'zipPrefix',
        mapping: 'zipPrefixNbr',
        type: 'string'
    }, {
        name: 'country',
        mapping: 'cntryNm',
        type: 'string'
    }, {
        name: 'group1VerifiedTypeCode',
        mapping: 'group1VerifdTypeCd',
        type: 'string'
    }, {
        name: 'participantAddressId',
        mapping: 'ptcpntAddrsId',
        type: 'string'
    }, {
        name: 'participantId',
        mapping: 'ptcpntId',
        type: 'string'
    }, {
        name: 'sharedAddresssInd',
        mapping: 'sharedAddrsInd',
        type: 'string'
    }, {
        name: 'militaryPostalTypeCode',
        mapping: 'mltyPostalTypeCd',
        type: 'string'
    }, {
        name: 'militaryPostOfficeTypeCode',
        mapping: 'mltyPostOfficeTypeCd',
        type: 'string'
    }, {
        name: 'territoryName',
        mapping: 'trtryNm',
        type: 'string'
    }, {
        name: 'providenceName',
        mapping: 'prvncNm',
        type: 'string'
    }, {
        name: 'foreignPostalCode',
        mapping: 'frgnPostalCd',
        type: 'string'
    }, {
        name: 'trsuryAddrsOneTxt',
        mapping: 'trsuryAddrsOneTxt',
        type: 'string'
    }, {
        name: 'trsuryAddrsTwoTxt',
        mapping: 'trsuryAddrsTwoTxt',
        type: 'string'
    }, {
        name: 'trsuryAddrsThreeTxt',
        mapping: 'trsuryAddrsThreeTxt',
        type: 'string'
    }, {
        name: 'trsuryAddrsFourTxt',
        mapping: 'trsuryAddrsFourTxt',
        type: 'string'
    }, {
        name: 'trsuryAddrsFiveTxt',
        mapping: 'trsuryAddrsFiveTxt',
        type: 'string'
    }, {
        name: 'trsuryAddrsSixTxt',
        mapping: 'trsuryAddrsSixTxt',
        type: 'string'
    }, {
        name: 'trsuryAddrsSevenTxt',
        mapping: 'trsuryAddrsSevenTxt',
        type: 'string'
    }, {
        name: 'fullTreasuryAddress',
        convert: function (v, record) {
            var address1 = Ext.isEmpty(record.get('trsuryAddrsOneTxt')) ? '' : record.get('trsuryAddrsOneTxt'),
				address2 = Ext.isEmpty(record.get('trsuryAddrsTwoTxt')) ? '' : record.get('trsuryAddrsTwoTxt'),
                address3 = Ext.isEmpty(record.get('trsuryAddrsThreeTxt')) ? '' : record.get('trsuryAddrsThreeTxt'),
                address4 = Ext.isEmpty(record.get('trsuryAddrsFourTxt')) ? '' : record.get('trsuryAddrsFourTxt'),
                address5 = Ext.isEmpty(record.get('trsuryAddrsFiveTxt')) ? '' : record.get('trsuryAddrsFiveTxt'),
                address6 = Ext.isEmpty(record.get('trsuryAddrsSixTxt')) ? '' : record.get('trsuryAddrsSixTxt'),
                address7 = Ext.isEmpty(record.get('trsuryAddrsSevenTxt')) ? '' : record.get('trsuryAddrsSevenTxt'),
                fullAddress = '';

            fullAddress += address1;
            fullAddress += address2 == '' ? '' : ', ' + address2;
            fullAddress += address3 == '' ? '' : ', ' + address3;
            fullAddress += address4 == '' ? '' : ', ' + address4;
            fullAddress += address5 == '' ? '' : ', ' + address5;
            fullAddress += address6 == '' ? '' : ', ' + address6;
            fullAddress += address7 == '' ? '' : ', ' + address7;
            return fullAddress;
        }
    }, {
        name: 'emailAddrsTxt',
        mapping: 'emailAddrsTxt',
        type: 'string'
    }, {
        name: 'veteranEmail',
        convert: function (v, record) {
            var veteranEmail = Ext.isEmpty(record.get('emailAddrsTxt')) ? '' : record.get('emailAddrsTxt');
            if (veteranEmail != '') {
                parent.Xrm.Page.getAttribute('va_calleremail').setValue(veteranEmail);
            }
            return veteranEmail;
        }
    }

    ],
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'xml',
            record: 'return'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.address.FindAllPtcpntAddrsByPtcpntId',
            update: '',
            destroy: ''
        }
    }
});