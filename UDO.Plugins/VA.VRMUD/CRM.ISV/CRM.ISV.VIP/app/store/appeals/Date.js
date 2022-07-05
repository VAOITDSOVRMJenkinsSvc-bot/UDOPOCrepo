/**
* @author Josh Oliver
* @class VIP.store.appeals.Date
*
* Store associated with hearing request model
*/
Ext.define('VIP.store.appeals.Date', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.appeals.Date'
    ],
    model: 'VIP.model.appeals.Date'
});