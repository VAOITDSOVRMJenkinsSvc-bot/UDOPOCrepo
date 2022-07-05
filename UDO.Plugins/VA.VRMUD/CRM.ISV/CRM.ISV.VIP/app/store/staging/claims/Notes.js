/**
* @author Josh Oliver
* @class VIP.store.staging.claims.Notes
*
* Store associated with claim notes
*/
Ext.define('VIP.store.staging.claims.Notes', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.claims.Notes'
    ],
    model: 'VIP.model.claims.Notes'
});