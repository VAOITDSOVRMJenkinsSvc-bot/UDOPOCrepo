/**
* @author Ivan Yurisevic
* @class VIP.store.paymenthistory.Payments
*
* Store associated with Payment History model
*/
Ext.define('VIP.store.paymenthistory.Payments', {
    extend: 'Ext.data.Store',
    requires: 'VIP.model.paymenthistory.Payments',
    model: 'VIP.model.paymenthistory.Payments',
    sorters: { property: 'payCheckDate', direction: 'DESC' }
});