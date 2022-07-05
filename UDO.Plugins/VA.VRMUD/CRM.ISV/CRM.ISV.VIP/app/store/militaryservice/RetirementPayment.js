/**
* @author Josh Oliver
* @class VIP.store.militaryservice.RetirementPayment
*
* Store associated with retirement payments
*/
Ext.define('VIP.store.militaryservice.RetirementPayment', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.militaryservice.RetirementPayment'
    ],
    model: 'VIP.model.militaryservice.RetirementPayment'
});