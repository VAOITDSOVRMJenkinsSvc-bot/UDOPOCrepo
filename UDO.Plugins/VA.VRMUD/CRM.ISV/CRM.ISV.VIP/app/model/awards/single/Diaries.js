/**
* @author Ivan Yurisevic
* @class VIP.model.awards.single.Diaries
*
* The model for diaries associated with the person
*/
Ext.define('VIP.model.awards.single.Diaries', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'date',
        mapping: 'date',
        type: 'date',
        dateFormat: 'mdY'
    }, {
        name: 'description',
        mapping: 'description',
        type: 'string'
    }, {
        name: 'id',
        mapping: 'id',
        type: 'string'
    }, {
        name: 'reasonCode',
        mapping: 'reasonCd',
        type: 'string'
    }, {
        name: 'reasonName',
        mapping: 'reasonName',
        type: 'string'
    }],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'diaries'
        }
    }
});