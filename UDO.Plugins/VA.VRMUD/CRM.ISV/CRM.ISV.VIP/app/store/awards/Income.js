/**
* @author Ivan Yurisevic
* @class VIP.store.awards.Income
*
* Store associated with Income model
*/
Ext.define('VIP.store.awards.Income', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.awards.incomeexpense.Income'
    ],
    model: 'VIP.model.awards.incomeexpense.Income'
});