/**
* @author Josh Oliver
* @class VIP.store.militaryservice.Decoration
*
* Store associated with service decorations
*/
Ext.define('VIP.store.militaryservice.Decoration', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.militaryservice.Decoration'
    ],
    model: 'VIP.model.militaryservice.Decoration'
});