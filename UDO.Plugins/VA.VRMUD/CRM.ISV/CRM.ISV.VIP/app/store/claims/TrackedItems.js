/**
* @author Josh Oliver
* @class VIP.store.claims.TrackedItem
*
* Store associated with tracked items
*/
Ext.define('VIP.store.claims.TrackedItems', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.claims.TrackedItems'
    ],
    model: 'VIP.model.claims.TrackedItems'
});