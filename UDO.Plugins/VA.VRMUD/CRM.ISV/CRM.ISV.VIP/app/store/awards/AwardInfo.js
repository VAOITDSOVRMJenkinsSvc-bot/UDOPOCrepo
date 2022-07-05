/**
* @author Ivan Yurisevic
* @class VIP.store.awards.AwardInfo
*
* Store associated with AwardInfo model
*/
Ext.define('VIP.store.awards.AwardInfo', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.awards.AwardInfo'
    ],
    model: 'VIP.model.awards.AwardInfo'
});