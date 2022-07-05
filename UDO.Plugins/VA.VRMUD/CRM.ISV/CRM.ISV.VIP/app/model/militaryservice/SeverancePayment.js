/**
* @author Ivan Yurisevic
* @class VIP.model.militaryservice.SeverancePayment
*
* The model service severance payments
*/
Ext.define('VIP.model.militaryservice.SeverancePayment', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'disabilityText',
        type: 'string',
        mapping: 'disabilityTxt'
    }, {
        name: 'grossAmount',
        type: 'float',
        mapping: 'grossAmount'
    }, {
        name: 'lessFedTaxAmount',
        type: 'float',
        mapping: 'lessFedTaxAmount'
    }, {
        name: 'lineItemNumber',
        type: 'string',
        mapping: 'lineItemNbr'
    }, {
        name: 'participantId',
        type: 'string',
        mapping: 'ptcpntId'
    }],

    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'militarySeverancePays'
        }
    }
});