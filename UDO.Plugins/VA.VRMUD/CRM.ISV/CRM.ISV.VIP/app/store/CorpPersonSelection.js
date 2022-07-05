/**
* @author Jonas Dawson
* @class VIP.store.CorpPersonSelection
*
* Store associated with corppersonselection model
*/
Ext.define('VIP.store.CorpPersonSelection', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.CorpPersonSelection'
    ],
    model: 'VIP.model.CorpPersonSelection'
});