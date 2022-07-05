/**
* @author Ivan Yurisevic
* @class VIP.store.paymenthistory.ReturnedPayments
*
* Store associated with Payment History model
*/
Ext.define('VIP.store.paymenthistory.ReturnedPayments', {
    extend: 'Ext.data.Store',
    requires: 'VIP.model.paymenthistory.ReturnedPayments',
    model: 'VIP.model.paymenthistory.ReturnedPayments',
    sorters: { property: 'checkIssueDate', direction: 'DESC' }
});