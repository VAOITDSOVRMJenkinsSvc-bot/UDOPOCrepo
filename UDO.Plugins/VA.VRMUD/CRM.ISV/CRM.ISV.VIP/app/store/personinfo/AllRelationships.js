/**
* @author Dmitri Riz
* @class VIP.store.personinfo.AllRelationships
*
* Store associated with AllRelationships model
*/
Ext.define('VIP.store.personinfo.AllRelationships', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.personinfo.AllRelationships'
    ],
    model: 'VIP.model.personinfo.AllRelationships'
});