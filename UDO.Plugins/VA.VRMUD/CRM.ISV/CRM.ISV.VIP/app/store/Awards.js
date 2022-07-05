/**
* @author Ivan Yurisevic
* @class VIP.store.Awards
*
* Store associated with Awards model
*/
Ext.define('VIP.store.Awards', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.awards.Multiple'
    ],
    model: 'VIP.model.awards.Multiple'
});