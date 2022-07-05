/**
* @author Josh Oliver
* @class VIP.store.staging.Claims
*
* Staging store in the event of a multiple response
*/
Ext.define('VIP.store.staging.Claims', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.Claims'
    ],
    model: 'VIP.model.Claims'
});

