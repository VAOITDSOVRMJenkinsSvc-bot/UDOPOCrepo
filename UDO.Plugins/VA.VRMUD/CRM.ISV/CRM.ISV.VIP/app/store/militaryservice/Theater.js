/**
* @author Josh Oliver
* @class VIP.store.militaryservice.Theater
*
* Store associated with theaters
*/
Ext.define('VIP.store.militaryservice.Theater', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.militaryservice.Theater'
    ],
    model: 'VIP.model.militaryservice.Theater'
});