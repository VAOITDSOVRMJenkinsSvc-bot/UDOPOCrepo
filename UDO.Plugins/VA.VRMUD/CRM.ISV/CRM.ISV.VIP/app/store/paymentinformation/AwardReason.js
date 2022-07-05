/**
* @author Josh Oliver
* @class VIP.store.paymentinformation.AwardReason
*
* Store associated with paymentinformation award reasons
*/
Ext.define('VIP.store.paymentinformation.AwardReason', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.paymentinformation.AwardReason'
    ],
    model: 'VIP.model.paymentinformation.AwardReason'
});