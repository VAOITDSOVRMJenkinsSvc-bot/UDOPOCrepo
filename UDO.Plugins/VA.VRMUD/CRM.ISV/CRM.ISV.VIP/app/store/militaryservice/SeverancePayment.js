/**
* @author Josh Oliver
* @class VIP.store.militaryservice.SeverancePayment
*
* Store associated with severance payments
*/
Ext.define('VIP.store.militaryservice.SeverancePayment', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.militaryservice.SeverancePayment'
    ],
    model: 'VIP.model.militaryservice.SeverancePayment'
});