/**
* @author Josh Oliver
* @class VIP.store.claims.LifeCycle
*
* Store associated with claim life cycles
*/
Ext.define('VIP.store.claims.LifeCycle', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.claims.LifeCycle'
    ],
    model: 'VIP.model.claims.LifeCycle'
});