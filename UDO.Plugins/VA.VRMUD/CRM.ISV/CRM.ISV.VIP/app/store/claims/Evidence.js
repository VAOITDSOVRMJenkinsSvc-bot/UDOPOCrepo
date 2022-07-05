/**
* @author Josh Oliver
* @class VIP.store.claims.Evidence
*
* Store associated with claim evidence
*/
Ext.define('VIP.store.claims.Evidence', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.claims.Evidence'
    ],
    model: 'VIP.model.claims.Evidence'
});