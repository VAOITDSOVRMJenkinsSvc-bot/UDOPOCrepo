/**
* @author Jonas Dawson
* @class VIP.model.CorpPersonSelection
*
* The model for person corporate detail information
*/

Ext.define('VIP.model.CorpPersonSelection', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'branchOfService',
        mapping: 'branchOfService1',
        type: 'string'
    }, {
        name: 'dob',
        mapping: 'dateOfBirth',
        type: 'string'
        //dateFormat: 'm/d/Y'
    }, {
        name: 'dod',
        mapping: 'dateOfDeath',
        type: 'string'
        //dateFormat: 'm/d/Y'
    }, {
        name: 'filler',
        mapping: 'filler',
        type: 'string'
    }, {
        name: 'firstName',
        mapping: 'firstName',
        type: 'string'
    }, {
        name: 'lastName',
        mapping: 'lastName',
        type: 'string'
    }, {
        name: 'middleName',
        mapping: 'middleName',
        type: 'string'
    }, {
        name: 'participantId',
        mapping: 'ptcpntId',
        type: 'string'
    }, {
        name: 'ssn',
        mapping: 'ssn',
        type: 'string'
    }, {
        name: 'suffixName',
        mapping: 'suffixName',
        type: 'string'
    }
    ],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'persons'
        }
    }
});
