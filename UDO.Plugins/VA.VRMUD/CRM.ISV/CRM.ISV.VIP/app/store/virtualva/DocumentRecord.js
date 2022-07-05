/**
* @author Josh Oliver
* @class VIP.store.virtualva.DocumentRecord
*
* Store associated with Virtual VA Document Record model
*/
Ext.define('VIP.store.virtualva.DocumentRecord', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.virtualva.DocumentRecord'
    ],
    model: 'VIP.model.virtualva.DocumentRecord'
});