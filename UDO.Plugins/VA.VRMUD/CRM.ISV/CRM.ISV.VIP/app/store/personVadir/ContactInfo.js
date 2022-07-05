/**
* @class VIP.store.personVadir.ContactInfo
*
* Store associated with Phone model
*/
Ext.define('VIP.store.personVadir.ContactInfo', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.personVadir.ContactInfo'
    ],
    model: 'VIP.model.personVadir.ContactInfo'
});