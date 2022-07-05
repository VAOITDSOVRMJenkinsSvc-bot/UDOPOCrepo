/**
* @author Ivan Yurisevic
* @class VIP.store.awards.Deductions
*
* Store associated with Deductions model
*/
Ext.define('VIP.store.awards.Deductions', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.awards.awardinfo.Deductions'
    ],
    model: 'VIP.model.awards.awardinfo.Deductions'
});