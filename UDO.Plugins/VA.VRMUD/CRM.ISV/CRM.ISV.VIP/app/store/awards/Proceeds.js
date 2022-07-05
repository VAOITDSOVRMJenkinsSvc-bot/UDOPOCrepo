/**
* @author Ivan Yurisevic
* @class VIP.store.awards.Proceeds
*
* Store associated with Proceeds model
*/
Ext.define('VIP.store.awards.Proceeds', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.awards.awardinfo.AccountBalances'
    ],
    model: 'VIP.model.awards.awardinfo.AccountBalances'
});