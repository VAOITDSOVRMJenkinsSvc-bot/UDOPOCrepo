/**
* @class VIP.store.personVadir.Email
*
* Store associated with Email model
*/
Ext.define('VIP.store.personVadir.Email', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.personVadir.Email'
    ],
    model: 'VIP.model.personVadir.Email'
});