/**
* @author Ivan Yurisevic
* @class VIP.store.paymenthistory.PaymentAddress
*
* Store associated with Payment History model
*/
Ext.define('VIP.store.paymenthistory.PaymentAddress', {
    extend: 'Ext.data.Store',
    requires: 'VIP.model.paymenthistory.PaymentAddress',
    model: 'VIP.model.paymenthistory.PaymentAddress'
});