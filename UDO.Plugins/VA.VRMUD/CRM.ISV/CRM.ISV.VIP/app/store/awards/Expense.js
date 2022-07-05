/**
* @author Ivan Yurisevic
* @class VIP.store.awards.Expense
*
* Store associated with Expense model
*/
Ext.define('VIP.store.awards.Expense', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.awards.incomeexpense.Expense'
    ],
    model: 'VIP.model.awards.incomeexpense.Expense'
});