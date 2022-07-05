/**
* @author Josh Oliver
* @class VIP.store.paymentinformation.PaymentAdjustmentVo
*
* Store associated with paymentinformation payment adjustment vos
*/
Ext.define('VIP.store.paymentinformation.PaymentAdjustmentVo', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.paymentinformation.PaymentAdjustmentVo'
    ],
    model: 'VIP.model.paymentinformation.PaymentAdjustmentVo'
});