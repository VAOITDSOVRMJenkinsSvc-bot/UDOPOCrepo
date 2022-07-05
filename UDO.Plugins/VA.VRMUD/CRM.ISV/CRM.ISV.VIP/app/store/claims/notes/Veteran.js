/**
* @author Josh Oliver
* @class VIP.store.claims.notes.Veteran
*
* Store associated with claim notes
*/
Ext.define('VIP.store.claims.notes.Veteran', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.claims.Notes'
    ],
    model: 'VIP.model.claims.Notes'
});