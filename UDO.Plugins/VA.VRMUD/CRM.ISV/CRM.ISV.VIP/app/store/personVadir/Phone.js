/**
* @class VIP.store.personVadir.Phone
*
* Store associated with Phone model
*/
Ext.define('VIP.store.personVadir.Phone', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.personVadir.Phone'
    ],
    model: 'VIP.model.personVadir.Phone'
});