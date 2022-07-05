/**
* @author Josh Oliver
* @class VIP.store.appeals.Detail
*
* Store associated with appeal detail model
*/
Ext.define('VIP.store.appeals.Detail', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.appeals.Detail'
    ],
    model: 'VIP.model.appeals.Detail'
});