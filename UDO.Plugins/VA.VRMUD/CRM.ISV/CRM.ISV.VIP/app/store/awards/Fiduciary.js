/**
* @author Ivan Yurisevic
* @class VIP.store.awards.Fiduciary
*
* Store associated with Awards model
*/
Ext.define('VIP.store.awards.Fiduciary', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.awards.Fiduciary'
    ],
    model: 'VIP.model.awards.Fiduciary'
});