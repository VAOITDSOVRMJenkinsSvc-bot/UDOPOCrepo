/**
* @author Jonas Dawson
* @class VIP.store.BirlsPersonSelection
*
* Store associated with birlspersonselection model
*/
Ext.define('VIP.store.BirlsPersonSelection', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.BirlsPersonSelection'
    ],
    model: 'VIP.model.BirlsPersonSelection'
});