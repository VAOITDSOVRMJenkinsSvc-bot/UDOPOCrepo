/**
* @author Ivan Yurisevic
* @class VIP.store.Ratings
*
* Store associated with Ratings model
*/
Ext.define('VIP.store.Ratings', {
    extend: 'Ext.data.Store',
    requires: 'VIP.model.Ratings',
    model: 'VIP.model.Ratings'
});