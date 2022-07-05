/**
* @author Stan Salatov
* @class VIP.store.ebenefits.Ebenefits
*
* Store associated with Ebenefits model
*/
Ext.define('VIP.store.ebenefits.Ebenefits', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.ebenefits.Ebenefits'
    ],
    model: 'VIP.model.ebenefits.Ebenefits'
});