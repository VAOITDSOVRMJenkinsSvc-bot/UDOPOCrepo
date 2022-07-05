/**
* @author Josh Oliver
* @class VIP.store.claims.ClaimDetail
*
* Store associated with claim detail
*/
Ext.define('VIP.store.claims.ClaimDetail', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.claims.ClaimDetail'
    ],
    model: 'VIP.model.claims.ClaimDetail'
});