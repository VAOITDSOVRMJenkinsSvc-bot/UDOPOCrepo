/**
* @author Jonas Dawson
* @class VIP.store.Person
*
* Store associated with the person context model
*/
Ext.define('VIP.store.Person', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.Person'
    ],
    model: 'VIP.model.Person'
});