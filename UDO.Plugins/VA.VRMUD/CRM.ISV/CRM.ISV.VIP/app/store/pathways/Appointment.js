/**
* @author Josh Oliver
* @class VIP.store.pathways.Appointment
*
* Store associated with appointments model
*/
Ext.define('VIP.store.pathways.Appointment', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.pathways.Appointment'
    ],
    model: 'VIP.model.pathways.Appointment'
});