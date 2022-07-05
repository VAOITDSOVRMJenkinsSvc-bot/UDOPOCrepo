/**
* @author Josh Oliver
* @class VIP.store.militaryservice.Pow
*
* Store associated with pows
*/
Ext.define('VIP.store.militaryservice.Pow', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.militaryservice.Pow'
    ],
    model: 'VIP.model.militaryservice.Pow'
});