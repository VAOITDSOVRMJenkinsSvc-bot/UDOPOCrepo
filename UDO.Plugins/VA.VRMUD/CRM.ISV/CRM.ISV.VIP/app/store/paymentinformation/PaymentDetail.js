/**
* @author Ivan Yurisevic
* @class VIP.store.paymentinformation.PaymentDetail
*
* Store associated with paymentinformation payment details
*/
Ext.define('VIP.store.paymentinformation.PaymentDetail', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.paymentinformation.PaymentDetail'
    ],
    model: 'VIP.model.paymentinformation.PaymentDetail'
});