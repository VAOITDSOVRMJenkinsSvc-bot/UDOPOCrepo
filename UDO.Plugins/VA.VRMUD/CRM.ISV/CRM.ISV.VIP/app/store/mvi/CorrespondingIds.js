/**
* @author Josh Oliver
* @class VIP.store.mvi.CorrespondingIds
*
* Store associated with exam model
*/
Ext.define('VIP.store.mvi.CorrespondingIds', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.mvi.CorrespondingIds'
    ],
    model: 'VIP.model.mvi.CorrespondingIds'
});