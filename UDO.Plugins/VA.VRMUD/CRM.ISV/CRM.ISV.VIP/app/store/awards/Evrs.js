/**
* @author Ivan Yurisevic
* @class VIP.store.awards.Evrs
*
* Store associated with Evrs model
*/
Ext.define('VIP.store.awards.Evrs', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.awards.single.Evrs'
    ],
    model: 'VIP.model.awards.single.Evrs'
});