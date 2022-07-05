/**
* @author Josh Oliver
* @class VIP.store.claims.Document
*
* Store associated with claim documents
*/
Ext.define('VIP.store.claims.Document', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.claims.Document'
    ],
    model: 'VIP.model.claims.Document'
});