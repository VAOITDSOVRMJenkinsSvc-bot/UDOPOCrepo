/**
* @author Ivan Yurisevic
* @class VIP.model.paymentinformation.PaymentAdjustmentVo
*
* The model for paymentinformation payment adjustment
*/
Ext.define('VIP.model.paymentinformation.PaymentAdjustmentVo', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'adjustmentAmount',
            type: 'float',
            mapping: 'adjustmentAmount'
        },
        {
            name: 'adjustmentCategory',
            type: 'string',
            mapping: 'adjustmentCategory'
        },
        {
            name: 'adjustmentOperation',
            type: 'string',
            mapping: 'adjustmentOperation'
        },
        {
            name: 'adjustmentReason',
            type: 'string',
            mapping: 'adjustmentReason'
        },
        {
            name: 'adjustmentSource',
            type: 'string',
            mapping: 'adjustmentSource'
        },
        {
            name: 'adjustmentType',
            type: 'string',
            mapping: 'adjustmentType'
        }
    ],

     //Start Memory proxy
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'paymentAdjustmentVO'
        }
    }
});