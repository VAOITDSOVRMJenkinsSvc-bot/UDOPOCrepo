/**
* @author Josh Oliver
* @class VIP.store.appeals.SpecialContention
*
* Store associated with appeal issues remand reason model
*/
Ext.define('VIP.store.appeals.SpecialContentions', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.appeals.SpecialContentions'
    ],
    model: 'VIP.model.appeals.SpecialContentions'
});