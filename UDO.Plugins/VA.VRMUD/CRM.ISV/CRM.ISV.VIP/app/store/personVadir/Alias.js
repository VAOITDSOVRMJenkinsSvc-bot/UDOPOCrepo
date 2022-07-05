/**
* @class VIP.store.personVadir.Addresses
*
* Store associated with Alias model
*/
Ext.define('VIP.store.personVadir.Alias', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.personVadir.Alias'
    ],
    model: 'VIP.model.personVadir.Alias'
});