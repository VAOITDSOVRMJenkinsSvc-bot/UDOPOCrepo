/**
* @author Ivan Yurisevic
* @class VIP.store.awards.AwardLines
*
* Store associated with AwardLines model
*/
Ext.define('VIP.store.awards.AwardLines', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.awards.awardinfo.AwardLines'
    ],
    model: 'VIP.model.awards.awardinfo.AwardLines'
});