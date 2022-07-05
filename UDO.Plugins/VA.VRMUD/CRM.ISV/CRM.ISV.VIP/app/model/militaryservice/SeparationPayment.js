/**
* @author Ivan Yurisevic
* @class VIP.model.militaryservice.SeparationPayment
*
* The model service separation payments
*/
Ext.define('VIP.model.militaryservice.SeparationPayment', {
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
        name: 'receiptDate',
        type: 'string',
        mapping: 'receiptDate'
    }],

    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'militarySeperationPays'
        }
    }
});