/**
* @author Josh Oliver
* @class VIP.store.claims.Letter
*
* Store associated with claim letters
*/
Ext.define('VIP.store.claims.Letters', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.claims.Letters'
    ],
    model: 'VIP.model.claims.Letters'
});