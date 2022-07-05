/**
* @author Ivan Yurisevic
* @class VIP.store.awards.Diaries
*
* Store associated with Diaries model
*/
Ext.define('VIP.store.awards.Diaries', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.awards.single.Diaries'
    ],
    model: 'VIP.model.awards.single.Diaries'
});