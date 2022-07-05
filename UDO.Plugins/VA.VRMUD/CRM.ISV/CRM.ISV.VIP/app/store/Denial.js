/**
* @author Josh Oliver
* @class VIP.store.Denial
*
* Store associated with denial model
*/
Ext.define('VIP.store.Denial', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.Denial'
    ],
    model: 'VIP.model.Denial'
});