/**
* @author Josh Oliver
* @class VIP.store.militaryservice.ReadjustmentPayment
*
* Store associated with readjustment payments
*/
Ext.define('VIP.store.militaryservice.ReadjustmentPayment', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.militaryservice.ReadjustmentPayment'
    ],
    model: 'VIP.model.militaryservice.ReadjustmentPayment'
});