/**
* @author Jonas Dawson
* @class VIP.model.Person
*
* The unified context for the person returned
*/
Ext.define('VIP.model.Person', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'ssn',
            type: 'string'
        },
        {
            name: 'participantId',
            type: 'string'
        }, 
        {
            name: 'fileNumber',
            type: 'string'
        },
        {
            name: 'firstName',
            type: 'string'
        },
        {
            name: 'lastName',
            type: 'string'
        },
        {
            name: 'middleName',
            type: 'string'
        },
        {
            name: 'dob',
            type: 'string'
        },
        {
            name: 'dod',
            type: 'string'
        },
        {
            name: 'gender',
            type: 'string'
        },
        {
            name: 'branchOfService',
            type: 'string'
        },
        //Why are the two items below available in this model?
        {
            name: 'awardKey',
            type: 'string'
        },
        {
            name: 'payeeSsn',
            type: 'string'
        },
        {
            name: 'edipi',
            type: 'string'
        },
        {
            name: 'payeeCode',
            type: 'string'
        }
    ],
    proxy: {
        type: 'memory',
        reader: 'json'
    }
}); 