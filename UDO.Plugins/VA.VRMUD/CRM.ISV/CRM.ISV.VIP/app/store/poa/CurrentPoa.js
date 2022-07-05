/**
* @author Ivan Yurisevic
* @class VIP.store.poa.CurrentPoa
*
* Store associated with CorpDetails model
*/
Ext.define('VIP.store.poa.CurrentPoa', {
    extend: 'Ext.data.Store',
    requires: 'VIP.model.poa.CurrentPoa',
    model: 'VIP.model.poa.CurrentPoa'
});