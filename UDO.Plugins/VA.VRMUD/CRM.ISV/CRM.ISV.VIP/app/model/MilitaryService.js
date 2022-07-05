/**
* @author Ivan Yurisevic
* @class VIP.model.MilitaryService
*
* The model for BIRLS record details
*/
Ext.define('VIP.model.MilitaryService', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.data.reader.MilitaryService', //reader
        'VIP.soap.envelopes.share.claimant.FindMilitaryRecordByPtcpntId', //soap
        'VIP.model.militaryservice.Decoration', 
        'VIP.model.militaryservice.MilitaryPersons',
        'VIP.model.militaryservice.Pow',
        'VIP.model.militaryservice.ReadjustmentBalance',
        'VIP.model.militaryservice.ReadjustmentPayment',
        'VIP.model.militaryservice.RetirementPayment',
        'VIP.model.militaryservice.SeparationBalance',
        'VIP.model.militaryservice.SeparationPayment',
        'VIP.model.militaryservice.SeveranceBalance',
        'VIP.model.militaryservice.SeverancePayment',
        'VIP.model.militaryservice.Theater', 
        'VIP.model.militaryservice.TourHistory'
    ],

    fields: [{
        name: 'numberOfDecorations',
        mapping: 'numberOfMilitaryPersonDecorationRecords',
        type: 'string'
    }, {
        name: 'numberOfPowRecords',
        mapping: 'numberOfMilitaryPersonPowRecords',
        type: 'string'
    }, {
        name: 'numberOfPersonRecords',
        mapping: 'numberOfMilitaryPersonRecords',
        type: 'string'
    }, {
        name: 'numberOfTours',
        mapping: 'numberOfMilitaryPersonTourRecords',
        type: 'string'
    }, {
        name: 'numberOfReadjustmentBalances',
        mapping: 'numberOfMilitaryReadjustmentBalanceRecords',
        type: 'string'
    }, {
        name: 'numberOfReadjustmentPays',
        mapping: 'numberOfMilitaryReadjustmentPayRecords',
        type: 'string'
    }, {
        name: 'numberOfRetirementPays',
        mapping: 'numberOfMilitaryRetirementPayRecords',
        type: 'string'
    }, {
        name: 'numberOfSeparationBalances',
        mapping: 'numberOfMilitarySeperationBalanceRecords',
        type: 'string'
    }, {
        name: 'numberOfSeparationPays',
        mapping: 'numberOfMilitarySeperationPayRecords',
        type: 'string'
    }, {
        name: 'numberOfSeveranceBalances',
        mapping: 'numberOfMilitarySeveranceBalanceRecords',
        type: 'string'
    }, {
        name: 'numberOfSeverancePays',
        mapping: 'numberOfMilitarySeverancePayRecords',
        type: 'string'
    }, {
        name: 'numberOfMilitaryTheatres',
        mapping: 'numberOfMilitaryTheatreRecords',
        type: 'string'
    }],

    //Associations begin:
    hasMany: [{
        model: 'VIP.model.militaryservice.Decoration',
        name: 'decorations',
        associationKey: 'decorations'
    }, {
        model: 'VIP.model.militaryservice.MilitaryPersons',
        name: 'persons',
        associationKey: 'persons'
    }, {
        model: 'VIP.model.militaryservice.Pow',
        name: 'personPows',
        associationKey: 'personPows'
    }, {
        model: 'VIP.model.militaryservice.ReadjustmentBalance',
        name: 'readjustmentBalances',
        associationKey: 'readjustmentBalances'
    }, {
        model: 'VIP.model.militaryservice.ReadjustmentPayment',
        name: 'readjustmentPayments',
        associationKey: 'readjustmentPayments'
    }, {
        model: 'VIP.model.militaryservice.RetirementPayment',
        name: 'retirementPayments',
        associationKey: 'retirementPayments'
    }, {
        model: 'VIP.model.militaryservice.SeparationBalance',
        name: 'separationBalances',
        associationKey: 'separationBalances'
    }, {
        model: 'VIP.model.militaryservice.SeparationPayment',
        name: 'separationPayments',
        associationKey: 'separationPayments'
    }, {
        model: 'VIP.model.militaryservice.SeveranceBalance',
        name: 'severanceBalances',
        associationKey: 'severanceBalances'
    }, {
        model: 'VIP.model.militaryservice.SeverancePayment',
        name: 'severancePayments',
        associationKey: 'severancePayments'
    }, {
        model: 'VIP.model.militaryservice.Theater',
        name: 'theatres',
        associationKey: 'theatres'
    }, {
        model: 'VIP.model.militaryservice.TourHistory',
        name: 'militaryTours',
        associationKey: 'militaryTours'
    }],

    //Proxy and custom reader begins:
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'militaryservice',
            record: 'return'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.claimant.FindMilitaryRecordByPtcpntId',
            update: '',
            destroy: ''
        }
    }
});