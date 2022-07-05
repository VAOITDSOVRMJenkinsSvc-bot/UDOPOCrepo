/**
* @author Jonas Dawson
* @class VIP.store.Persistence
*
* Store associated with Persistence model
*/
Ext.define('VIP.store.Persistence', {
    extend: 'Ext.data.Store',
    requires: 'VIP.model.Persistence',
    model: 'VIP.model.Persistence'
});