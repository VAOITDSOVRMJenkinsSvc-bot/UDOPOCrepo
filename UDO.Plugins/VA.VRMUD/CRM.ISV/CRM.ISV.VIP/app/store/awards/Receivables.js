/**
* @author Ivan Yurisevic
* @class VIP.store.awards.Receivables
*
* Store associated with Receivables model
*/
Ext.define('VIP.store.awards.Receivables', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.awards.awardinfo.Receivables'
    ],
    model: 'VIP.model.awards.awardinfo.Receivables'
});