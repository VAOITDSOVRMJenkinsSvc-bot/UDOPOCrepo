/**
* @author Ivan Yurisevic
* @class VIP.model.awards.Income
*
* The model for Awards Income record details
*/
Ext.define('VIP.model.awards.incomeexpense.Income', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'annualAmount',
        mapping: 'annualAmount',
        type: 'float'
    }, {
        name: 'exclusionAmount',
        mapping: 'exclusionAmount',
        type: 'float'
    }, {
        name: 'exclusionTypeName',
        mapping: 'exclusionTypeName',
        type: 'string'
    }, {
        name: 'firstName',
        mapping: 'firstName',
        type: 'string'
    }, {
        name: 'incomeTypeName',
        mapping: 'incomeTypeName',
        type: 'string'
    }, {
        name: 'lastName',
        mapping: 'lastName',
        type: 'string'
    }, {
        name: 'middleName',
        mapping: 'middleName',
        type: 'string'
    }],

    //Start Memory proxy
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'incomeRecords'
        }
    }
});