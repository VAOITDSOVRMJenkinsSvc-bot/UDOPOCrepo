/**
* @author Josh Oliver
* @class VIP.store.appeals.Issue
*
* Store associated with appeal issue model
*/
Ext.define('VIP.store.appeals.Issue', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.appeals.Issue'
    ],
    model: 'VIP.model.appeals.Issue'
});