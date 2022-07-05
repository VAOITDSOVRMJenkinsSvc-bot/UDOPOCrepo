/**
* @author Ivan Yurisevic
* @class VIP.store.awards.IncomeExpenseInfo
*
* Store associated with IncomeExpenseInfo model
*/
Ext.define('VIP.store.awards.IncomeExpenseInfo', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.awards.IncomeExpenseInfo'
    ],
    model: 'VIP.model.awards.IncomeExpenseInfo'
});