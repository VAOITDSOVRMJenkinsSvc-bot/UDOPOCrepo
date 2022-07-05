/**
* @author Ivan Yurisevic
* @class VIP.store.PaymentHistory
*
* Store associated with Payment History model
*/
Ext.define('VIP.store.PaymentHistory', {
    extend: 'Ext.data.Store',
    requires: 'VIP.model.PaymentHistory',
    model: 'VIP.model.PaymentHistory',
    sorters: { property: 'payCheckDate', direction: 'DESC' }
});