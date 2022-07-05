/**
* @class VIP.store.personVadir.Addresses
*
* Store associated with Addresses model
*/
Ext.define('VIP.store.personVadir.Address', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.personVadir.Address'
    ],
    model: 'VIP.model.personVadir.Address'
});