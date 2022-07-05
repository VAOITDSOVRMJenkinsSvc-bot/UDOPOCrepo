/**
* @author Josh Oliver
* @class VIP.store.claims.Contention
*
* Store associated with claim contentions
*/
Ext.define('VIP.store.claims.Contentions', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.claims.Contentions'
    ],
    model: 'VIP.model.claims.Contentions'
});