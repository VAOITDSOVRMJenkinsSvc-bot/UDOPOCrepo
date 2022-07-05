/**
* @author Ivan Yurisevic
* @class VIP.store.denials.FullDenialReason
*
* Store associated with denial FullDenialReason model
*/
Ext.define('VIP.store.denials.FullDenialReason', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.denials.FullDenialReason'
    ],
    model: 'VIP.model.denials.FullDenialReason'
});