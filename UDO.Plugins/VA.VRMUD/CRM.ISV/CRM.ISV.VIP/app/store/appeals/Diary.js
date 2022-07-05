/**
* @author Josh Oliver
* @class VIP.store.appeals.Diary
*
* Store associated with appeal diary model
*/
Ext.define('VIP.store.appeals.Diary', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.appeals.Diary'
    ],
    model: 'VIP.model.appeals.Diary'
});