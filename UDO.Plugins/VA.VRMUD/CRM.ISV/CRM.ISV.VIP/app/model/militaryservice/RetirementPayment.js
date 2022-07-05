/**
* @author Ivan Yurisevic
* @class VIP.model.militaryservice.RetirementPayment
*
* The model service retirement payments
*/
Ext.define('VIP.model.militaryservice.RetirementPayment', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'effectiveDate',
        type: 'string',
        mapping: 'effectiveDate'
    }, {
        name: 'fullWaiverIndicator',
        type: 'string',
        mapping: 'fullWaiverInd'
    }, {
        name: 'grossMonthlyAmount',
        type: 'string',
        mapping: 'grossMonthlyAmount'
    }, {
        name: 'lessFedTaxAmount',
        type: 'string',
        mapping: 'lessFedTaxAmount'
    }, {
        name: 'lineItemNumber',
        type: 'string',
        mapping: 'lineItemNbr'
    }, {
        name: 'participantId',
        type: 'string',
        mapping: 'ptcpntId'
    }, {
        name: 'retirementPayTypeCode',
        type: 'string',
        mapping: 'retirementPayTypeCd'
    }, {
        name: 'retirementWaivedDate',
        type: 'string',
        mapping: 'retirementWaivedDate'
    }, {
        name: 'sbpOverpaymentAmount',
        type: 'string',
        mapping: 'sbpOverpaymentAmount'
    }],

    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'militaryRetirementPays'
        }
    }
});