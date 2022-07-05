/**
* @author Ivan Yurisevic
* @class VIP.model.awards.Expense
*
* The model for Awards Expense record details
*/
Ext.define('VIP.model.awards.incomeexpense.Expense', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'annualAmount',
        mapping: 'annualAmount',
        type: 'float'
    }, {
        name: 'typeName',
        mapping: 'typeName',
        type: 'string'
    }],

    //Start Memory proxy
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'expenseRecords'
        }
    }
});