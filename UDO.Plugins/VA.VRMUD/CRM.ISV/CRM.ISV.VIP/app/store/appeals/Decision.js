/**
* @author Josh Oliver
* @class VIP.store.appeals.Decision
*
* Store associated with appeal decision model
*/
Ext.define('VIP.store.appeals.Decision', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.appeals.Decision'
    ],
    model: 'VIP.model.appeals.Decision'
});