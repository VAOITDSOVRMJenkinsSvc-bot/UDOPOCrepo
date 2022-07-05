/**
* @author Josh Oliver
* @class VIP.store.militaryservice.SeparationBalance
*
* Store associated with separation balances
*/
Ext.define('VIP.store.militaryservice.SeparationBalance', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.militaryservice.SeparationBalance'
    ],
    model: 'VIP.model.militaryservice.SeparationBalance'
});