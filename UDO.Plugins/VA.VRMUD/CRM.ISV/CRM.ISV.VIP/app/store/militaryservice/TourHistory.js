/**
* @author Josh Oliver
* @class VIP.store.militaryservice.TourHistory
*
* Store associated with tour history
*/
Ext.define('VIP.store.militaryservice.TourHistory', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.militaryservice.TourHistory'
    ],
    model: 'VIP.model.militaryservice.TourHistory'
});