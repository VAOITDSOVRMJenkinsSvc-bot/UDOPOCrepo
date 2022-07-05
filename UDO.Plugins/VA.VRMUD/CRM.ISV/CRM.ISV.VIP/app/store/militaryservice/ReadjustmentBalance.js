/**
* @author Josh Oliver
* @class VIP.store.militaryservice.ReadjustmentBalance
*
* Store associated with readjustment balances
*/
Ext.define('VIP.store.militaryservice.ReadjustmentBalance', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.militaryservice.ReadjustmentBalance'
    ],
    model: 'VIP.model.militaryservice.ReadjustmentBalance'
});