/**
* @author Josh Oliver
* @class VIP.store.appeals.Appellant
*
* Store associated with appeal appellant model
*/
Ext.define('VIP.store.appeals.Appellant', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.appeals.Appellant'
    ],
    model: 'VIP.model.appeals.Appellant'
});