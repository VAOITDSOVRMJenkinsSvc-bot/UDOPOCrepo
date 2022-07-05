/**
* @author Josh Oliver
* @class VIP.store.pathways.ExamRequest
*
* Store associated with exam request model
*/
Ext.define('VIP.store.pathways.ExamRequest', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.pathways.ExamRequest'
    ],
    model: 'VIP.model.pathways.ExamRequest'
});