/**
* @author Josh Oliver
* @class VIP.store.virtualva.DocumentContent
*
* Store associated with Virtual VA Document Content model
*/
Ext.define('VIP.store.virtualva.DocumentContent', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.virtualva.DocumentContent'
    ],
    model: 'VIP.model.virtualva.DocumentContent'
});