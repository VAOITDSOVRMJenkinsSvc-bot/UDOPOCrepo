/**
* @author Josh Oliver
* @class VIP.store.claims.Status
*
* Store associated with statuses
*/
Ext.define('VIP.store.claims.Status', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.claims.Status'
    ],
    model: 'VIP.model.claims.Status'
});