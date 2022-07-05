/**
* @author Ivan Yurisevic
* @class VIP.model.awards.awardinfo.Receivables
*
* Submodel for the award info response. Required with the association.
*/
Ext.define('VIP.model.awards.awardinfo.Receivables', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'balance',
        mapping: 'balance',
        type: 'float'
    }, {
        name: 'code',
        mapping: 'code',
        type: 'string'
    }, {
        name: 'discoveryDate',
        mapping: 'discoveryDate',
        type: 'date',
        dateFormat: 'mdY'
    }, {
        name: 'name',
        mapping: 'name',
        type: 'string'
    }],

    //Start Memory Proxy
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'receivables'
        }
    }
});