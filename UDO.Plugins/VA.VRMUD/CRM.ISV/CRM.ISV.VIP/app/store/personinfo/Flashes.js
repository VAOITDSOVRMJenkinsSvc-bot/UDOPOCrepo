/**
* @author Ivan Yurisevic
* @class VIP.store.personinfo.Flashes
*
* Store associated with Flashes model
*/
Ext.define('VIP.store.personinfo.Flashes', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.personinfo.Flashes'
    ],
    model: 'VIP.model.personinfo.Flashes'
});