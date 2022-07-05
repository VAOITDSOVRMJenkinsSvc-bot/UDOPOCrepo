/**
* @author Josh Oliver
* @class VIP.store.pathways.Patient
*
* Store associated with patient model
*/
Ext.define('VIP.store.pathways.Patient', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.pathways.Patient'
    ],
    model: 'VIP.model.pathways.Patient'
});