/**
* @author Josh Oliver
* @class VIP.store.militaryservice.SeparationPayment
*
* Store associated with separation payments
*/
Ext.define('VIP.store.militaryservice.SeparationPayment', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.militaryservice.SeparationPayment'
    ],
    model: 'VIP.model.militaryservice.SeparationPayment'
});