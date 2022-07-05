/**
* @author Ivan Yurisevic
* @class VIP.model.paymentinformation.AwardReason
*
* The model for paymentinformation award reason
*/
Ext.define('VIP.model.paymentinformation.AwardReason', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'awardReasonText',
        type: 'string',
        mapping: 'awardReasonText '
    }],

    //Start Memory proxy
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'awardReasonVO'
        }
    }
});