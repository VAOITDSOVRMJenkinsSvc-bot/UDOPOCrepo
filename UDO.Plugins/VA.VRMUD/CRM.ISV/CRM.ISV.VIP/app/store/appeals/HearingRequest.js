/**
* @author Josh Oliver
* @class VIP.store.appeals.HearingRequest
*
* Store associated with appeal hearing request model
*/
Ext.define('VIP.store.appeals.HearingRequest', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.appeals.HearingRequest'
    ],
    model: 'VIP.model.appeals.HearingRequest'
});