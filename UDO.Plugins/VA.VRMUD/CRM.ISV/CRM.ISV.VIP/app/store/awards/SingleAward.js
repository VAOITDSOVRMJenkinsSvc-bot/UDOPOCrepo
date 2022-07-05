/**
* @author Ivan Yurisevic
* @class VIP.store.Awards
*
* Store associated with Awards model
*/
Ext.define('VIP.store.awards.SingleAward', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.awards.Single'
    ],
    model: 'VIP.model.awards.Single'
});