/**
* @author Josh Oliver
* @class VIP.store.virtualva.DocumentRecordPaging
*
* Store associated with Virtual VA Document Record model
*/
Ext.define('VIP.store.virtualva.DocumentRecordPaging', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.virtualva.DocumentRecord'
    ],
    model: 'VIP.model.virtualva.DocumentRecord',
    pageSize: 50
});