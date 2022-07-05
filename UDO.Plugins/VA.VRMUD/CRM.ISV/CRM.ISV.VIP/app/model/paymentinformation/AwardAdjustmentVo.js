/**
* @author Ivan Yurisevic
* @class VIP.model.paymentinformation.AwardAdjustmentVo
*
* The model for paymentinformation award adjustment
*/
Ext.define('VIP.model.paymentinformation.AwardAdjustmentVo', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'adjustmentAmount',
            type: 'float',
            mapping: 'adjustmentAmount'
        },
        {
            name: 'adjustmentEffectiveDate',
            dateFormat: 'c', //ISO 8601 date
            type: 'date',
            mapping: 'adjustmentEffectiveDate'
        },
        {
            name: 'adjustmentOperation',
            type: 'string',
            mapping: 'adjustmentOperation'
        },
        {
            name: 'adjustmentType',
            type: 'string',
            mapping: 'adjustmentType'
        },
        {
            name: 'alloteeRelationship',
            type: 'string',
            mapping: 'alloteeRelationship'
        },
        {
            name: 'allotmentRecipientName',
            type: 'string',
            mapping: 'allotmentRecipientName'
        },
        {
            name: 'allotmentType',
            type: 'string',
            mapping: 'allotmentType'
        }
    ],

    //Start Memory proxy
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'awardAdjustmentVO'
        }
    }
});