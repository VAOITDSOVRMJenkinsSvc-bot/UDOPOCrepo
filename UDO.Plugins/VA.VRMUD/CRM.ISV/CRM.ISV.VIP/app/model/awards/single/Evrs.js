/**
* @author Ivan Yurisevic
* @class VIP.model.awards.single.Evrs
*
* The model for Evrs associated with the person
*/
Ext.define('VIP.model.awards.single.Evrs', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'control',
        mapping: 'control',
        type: 'string'
    }, {
        name: 'exempt',
        mapping: 'exempt',
        type: 'string'
    }, {
        name: 'lastReported',
        mapping: 'lastReported',
        type: 'string'
    }, {
        name: 'status',
        mapping: 'status',
        type: 'string'
    }, {
        name: 'type',
        mapping: 'type',
        type: 'string'
    }],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'evrs'
        }
    }
});