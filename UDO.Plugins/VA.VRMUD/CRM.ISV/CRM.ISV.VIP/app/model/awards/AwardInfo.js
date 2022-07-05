/**
* @author Jonas Dawson
* @class VIP.model.awards.AwardInfo
*
* The model for Awards AwardInfo record details
*/
Ext.define('VIP.model.awards.AwardInfo', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.share.claimant.FindOtherAwardInformation', //SOAP
        'VIP.data.reader.AwardOtherInfo', //Reader
        'VIP.model.awards.awardinfo.Receivables',
        'VIP.model.awards.awardinfo.ClothingAllowances',
        'VIP.model.awards.awardinfo.Deductions',
        'VIP.model.awards.awardinfo.AccountBalances',
        'VIP.model.awards.awardinfo.AwardLines'
    ],
    fields: [{
        name: 'auditRelatedAr',
        mapping: 'awardInfo/auditRelatedAr',
        type: 'string'
    }, {
        name: 'beneCd',
        mapping: 'awardInfo/beneCd',
        type: 'string'
    }, {
        name: 'auditRelatedAr',
        mapping: 'awardInfo/auditRelatedAr',
        type: 'string'
    }, {
        name: 'beneFirstName',
        mapping: 'awardInfo/beneFirstName',
        type: 'string'
    }, {
        name: 'beneLastName',
        mapping: 'awardInfo/beneLastName',
        type: 'string'
    }, {
        name: 'beneMiddleName',
        mapping: 'awardInfo/beneMiddleName',
        type: 'string'
    }, {
        name: 'beneficiaryFullName',
        convert: function (v, record) {
            var firstName = record.get('beneFirstName'),
                middleName = record.get('beneMiddleName'),
                lastName = record.get('beneLastName'),
                fullname = '';

            fullname = (!Ext.isEmpty(lastName) ? lastName : '') + (!Ext.isEmpty(firstName) ? ', ' + firstName : '') + (!Ext.isEmpty(middleName) ? ' ' + middleName : '');
            return fullname;
        }
    }, {
        name: 'beneName',
        mapping: 'awardInfo/beneName',
        type: 'string'
    }, {
        name: 'bnftCd',
        mapping: 'awardInfo/bnftCd',
        type: 'string'
    }, {
        name: 'bnftName',
        mapping: 'awardInfo/bnftName',
        type: 'string'
    }, {
        name: 'fidType',
        mapping: 'awardInfo/fidType',
        type: 'string'
    }, {
        name: 'frequencyCd',
        mapping: 'awardInfo/frequencyCd',
        type: 'string'
    }, {
        name: 'frequencyName',
        mapping: 'awardInfo/frequencyName',
        type: 'string'
    }, {
        name: 'igReferenceNbr',
        mapping: 'awardInfo/igReferenceNbr',
        type: 'string'
    }, {
        name: 'lastPaidDate',
        mapping: 'awardInfo/lastPaidDate',
        type: 'date',
        dateFormat: 'mdY'
    }, {
        name: 'lastPaidDate_F',
        convert: function (v, record) {
            if (!Ext.isEmpty(record.get('lastPaidDate'))) {
                return Ext.Date.format(record.get('lastPaidDate'), "m/d/Y");
            } else return '';
        }
    }, {
        name: 'numberOfAccountBalances',
        mapping: 'awardInfo/numberOfAccountBalances',
        type: 'string'
    }, {
        name: 'numberOfDeductions',
        mapping: 'awardInfo/numberOfDeductions',
        type: 'string'
    }, {
        name: 'numberOfReceivables',
        mapping: 'awardInfo/numberOfReceivables',
        type: 'string'
    }, {
        name: 'payStatusCd',
        mapping: 'awardInfo/payStatusCd',
        type: 'string'
    }, {
        name: 'payStatusName',
        mapping: 'awardInfo/payStatusName',
        type: 'string'
    }, {
        name: 'recipName',
        mapping: 'awardInfo/recipName',
        type: 'string'
    }, {
        name: 'auditRelatedAr',
        mapping: 'awardInfo/auditRelatedAr',
        type: 'string'
    }, {
        name: 'requestedFrequency',
        mapping: 'awardInfo/requestedFrequency',
        type: 'string'
    }, {
        name: 'retroactiveDate',
        mapping: 'awardInfo/retroactiveDate',
        type: 'date',
        dateFormat: 'mdY'
    }, {
        name: 'retroactiveDate_F',
        convert: function (v, record) {
            if (!Ext.isEmpty(record.get('retroactiveDate'))) {
                return Ext.Date.format(record.get('retroactiveDate'), "m/d/Y");
            } else return '';
        }
    }, {
        name: 'statusReasonCd',
        mapping: 'awardInfo/statusReasonCd',
        type: 'string'
    }, {
        name: 'statusReasonDate',
        mapping: 'awardInfo/statusReasonDate',
        type: 'date',
        dateFormat: 'mdY'
    }, {
        name: 'statusReasonDate_F',
        convert: function (v, record) {
            if (!Ext.isEmpty(record.get('statusReasonDate'))) {
                return Ext.Date.format(record.get('statusReasonDate'), "m/d/Y");
            } else return '';
        }
    }, {
        name: 'statusReasonName',
        mapping: 'awardInfo/statusReasonName',
        type: 'string'
    }, {
        name: 'vetFirstName',
        mapping: 'awardInfo/vetFirstName',
        type: 'string'
    }, {
        name: 'vetLastName',
        mapping: 'awardInfo/vetLastName',
        type: 'string'
    }, {
        name: 'vetMiddleName',
        mapping: 'awardInfo/vetMiddleName',
        type: 'string'
    }, {
        name: 'awardTypeCode',
        mapping: 'awardTypeCd',
        type: 'string'
    }, {
        name: 'numberOfAwardLines',
        mapping: 'numberOfAwardLines',
        type: 'string'
    }, {
        name: 'numberOfRecords',
        mapping: 'numberOfRecords',
        type: 'string'
    }, {
        name: 'ptcpntBeneId',
        mapping: 'ptcpntBeneId',
        type: 'string'
    }, {
        name: 'ptcpntRecipId',
        mapping: 'ptcpntRecipId',
        type: 'string'
    }, {
        name: 'ptcpntVetId',
        mapping: 'ptcpntVetId',
        type: 'string'
    }, {
        name: 'returnCode',
        mapping: 'returnCode',
        type: 'string'
    }, {
        name: 'returnMessage',
        mapping: 'returnMessage',
        type: 'string'
    }],

    //Start Associations
    hasMany: [{
        model: 'VIP.model.awards.awardinfo.Receivables',
        name: 'receivables',
        associationKey: 'receivable'
    }, {
        model: 'VIP.model.awards.awardinfo.ClothingAllowances',
        name: 'clothingallowances',
        associationKey: 'parentClothingAllowanceInfo'
    }, {
        model: 'VIP.model.awards.awardinfo.Deductions',
        name: 'deductions',
        associationKey: 'deduction'
    }, {
        model: 'VIP.model.awards.awardinfo.AccountBalances',
        name: 'proceeds',
        associationKey: 'proceeds'
    }, {
        model: 'VIP.model.awards.awardinfo.AwardLines',
        name: 'awardlines',
        associationKey: 'awardline'
    }],
    //Start SOAP Proxy
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'awardotherinfo',
            record: 'return'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.claimant.FindOtherAwardInformation',
            update: '',
            destroy: ''
        }
    }

});