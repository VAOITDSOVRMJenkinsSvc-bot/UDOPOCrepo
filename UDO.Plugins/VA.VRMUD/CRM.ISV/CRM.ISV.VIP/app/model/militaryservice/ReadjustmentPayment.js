/**
* @author Ivan Yurisevic
* @class VIP.model.militaryservice.ReadjustmentPayment
*
* The model service readjustment payments
*/
Ext.define('VIP.model.militaryservice.ReadjustmentPayment', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'grossAmount',
        type: 'string',
        mapping: 'grossAmount'
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
        name: 'reasonText',
        type: 'string',
        mapping: 'reasonTxt'
    }, {
        name: 'receiptDate',
        type: 'string',
        mapping: 'receiptDate'
    }, {
        name: 'useCodeReasonText',
        type: 'string',
        mapping: 'usCodeReasonTxt'
    }],

    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'militaryReadjustmentPays'
        }
    }
});