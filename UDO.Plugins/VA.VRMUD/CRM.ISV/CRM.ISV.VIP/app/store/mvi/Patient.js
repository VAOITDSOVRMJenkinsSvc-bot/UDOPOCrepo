/**
* @author Josh Oliver
* @class VIP.store.mvi.Patient
*
* Store associated with exam model
*/
Ext.define('VIP.store.mvi.Patient', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.mvi.Patient'
    ],
    model: 'VIP.model.mvi.Patient'
});