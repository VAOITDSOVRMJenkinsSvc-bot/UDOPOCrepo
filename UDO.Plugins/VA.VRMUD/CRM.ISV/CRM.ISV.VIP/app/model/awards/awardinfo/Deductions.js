/**
* @author Ivan Yurisevic
* @class VIP.model.awards.awardinfo.Deductions
*
* Submodel for the award info response. Required with the association.
*/
Ext.define('VIP.model.awards.awardinfo.Deductions', {
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
        name: 'amount',
        mapping: 'amount',
        type: 'float'
    }, {
        name: 'name',
        mapping: 'name',
        type: 'string'
    }],

    //Start Memory proxy
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'deductions'
        }
    }
});