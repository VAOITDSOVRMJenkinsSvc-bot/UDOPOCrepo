/**
* @author Josh Oliver
* @class VIP.store.Appeal
*
* Store associated with appeal model
*/
Ext.define('VIP.store.Appeal', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.Appeal'
    ],
    model: 'VIP.model.Appeal'
});