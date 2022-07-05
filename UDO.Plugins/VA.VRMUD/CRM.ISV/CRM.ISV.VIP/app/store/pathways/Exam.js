/**
* @author Josh Oliver
* @class VIP.store.pathways.Exam
*
* Store associated with exam model
*/
Ext.define('VIP.store.pathways.Exam', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.pathways.Exam'
    ],
    model: 'VIP.model.pathways.Exam'
});