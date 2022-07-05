/**
* @author Ivan Yurisevic
* @class VIP.store.personinfo.Addresses
*
* Store associated with Addresses model
*/
Ext.define('VIP.store.personinfo.Addresses', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.personinfo.Addresses'
    ],
    model: 'VIP.model.personinfo.Addresses'
});