/**
* @author Josh Oliver
* @class VIP.store.Claims
*
* Store associated with claims
*/
Ext.define('VIP.store.Claims', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.Claims'
    ],
    pageSize: 5,

    model: 'VIP.model.Claims',
    sorters: { property: 'claimReceiveDate', direction: 'DESC' }
});