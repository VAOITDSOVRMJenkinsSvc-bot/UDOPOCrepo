/**
* @author Ivan Yurisevic
* @class VIP.store.personinfo.Dependents
*
* Store associated with Dependents model
*/
Ext.define('VIP.store.personinfo.Dependents', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.personinfo.Dependents'
    ],
    model: 'VIP.model.personinfo.Dependents'
});