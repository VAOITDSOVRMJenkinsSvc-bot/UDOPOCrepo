/**
* @author Josh Oliver
* @class VIP.store.appeals.AppellantAddress
*
* Store associated with appeal issues remand reason model
*/
Ext.define('VIP.store.appeals.AppellantAddress', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.appeals.AppellantAddress'
    ],
    model: 'VIP.model.appeals.AppellantAddress'
});