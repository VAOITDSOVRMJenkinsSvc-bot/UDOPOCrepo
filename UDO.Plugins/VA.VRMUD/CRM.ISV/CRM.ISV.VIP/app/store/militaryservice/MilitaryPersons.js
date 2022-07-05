/**
* @author Ivan Yurisevic
* @class VIP.store.militaryservice.MilitaryPersons
*
* Store associated with service MilitaryPersons
*/
Ext.define('VIP.store.militaryservice.MilitaryPersons', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.militaryservice.MilitaryPersons'
    ],
    model: 'VIP.model.militaryservice.MilitaryPersons'
});