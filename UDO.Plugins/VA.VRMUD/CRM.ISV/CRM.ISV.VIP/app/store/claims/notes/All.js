/**
* @author Josh Oliver
* @class VIP.store.claims.notes.All
*
* Store associated with claim notes
*/
Ext.define('VIP.store.claims.notes.All', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.claims.Notes'
    ],
    model: 'VIP.model.claims.Notes',
    sorters: [
        {
            property: 'createDate',
            direction: 'DESC'
        }
    ]
});