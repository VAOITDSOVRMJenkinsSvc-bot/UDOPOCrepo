/**
* @author Josh Oliver
* @class VIP.store.appeals.RemandReason
*
* Store associated with appeal issues remand reason model
*/
Ext.define('VIP.store.appeals.RemandReason', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.appeals.RemandReason'
    ],
    model: 'VIP.model.appeals.RemandReason'
});