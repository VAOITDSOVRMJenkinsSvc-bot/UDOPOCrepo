/**
* @author Josh Oliver
* @class VIP.store.claims.Suspense
*
* Store associated with suspenses
*/
Ext.define('VIP.store.claims.Suspense', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.claims.Suspense'
    ],
    model: 'VIP.model.claims.Suspense'
});