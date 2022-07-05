/**
* @author Ivan Yurisevic
* @class VIP.model.awards.single.Flashes
*
* Submodel for the Awards single response. Required with the association.
*/
Ext.define('VIP.model.awards.single.Flashes', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'assignedIndicator',
        mapping: 'assignedIndicator',
        type: 'string'
    }, {
        name: 'flashName',
        mapping: 'flashName',
        type: 'string'
    }, {
        name: 'flashType',
        mapping: 'flashType',
        type: 'string'
    }],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'flashes'
        }
    }
});