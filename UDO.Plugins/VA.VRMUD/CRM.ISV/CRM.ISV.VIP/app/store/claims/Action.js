/**
* @author Josh Oliver
* @class VIP.store.claims.Action
*
* Store associated with claim actions
*/
Ext.define('VIP.store.claims.Action', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.claims.Action'
    ],
    model: 'VIP.model.claims.Action'
});