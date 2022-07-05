/**
* @author Josh Oliver
* @class VIP.store.militaryservice.SeveranceBalance
*
* Store associated with severance balances
*/
Ext.define('VIP.store.militaryservice.SeveranceBalance', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.militaryservice.SeveranceBalance'
    ],
    model: 'VIP.model.militaryservice.SeveranceBalance'
});