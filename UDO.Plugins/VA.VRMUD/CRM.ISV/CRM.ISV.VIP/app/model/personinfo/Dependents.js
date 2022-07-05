/**
* @author Ivan Yurisevic
* @class VIP.model.personinfo.Dependents
*
* The model for dependents associated with the person
*/
Ext.define('VIP.model.personinfo.Dependents', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.share.claimant.FindDependents'
    ],
    fields: [{
        name: 'awardInd',
        mapping: 'awardIndicator',
        type: 'string'
    }, {
        name: 'cityOfBirth',
        mapping: 'cityOfBirth',
        type: 'string'
    }, {
        name: 'currentRelateStatus',
        mapping: 'currentRelateStatus',
        type: 'string'
    }, {
        name: 'dob',
        mapping: 'dateOfBirth',
        type: 'date',
        dateFormat: 'm/d/Y'
    }, {
        name: 'dod',
        mapping: 'dateOfDeath',
        type: 'date',
        dateFormat: 'm/d/Y'
    }, {
        name: 'deathReason',
        mapping: 'deathReason',
        type: 'string'
    }, {
        name: 'emailAddress',
        mapping: 'emailAddress',
        type: 'string'
    }, {
        name: 'firstName',
        mapping: 'firstName',
        type: 'string'
    }, {
        name: 'gender',
        mapping: 'gender',
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
        name: 'proofOfDependency',
        mapping: 'proofOfDependency',
        type: 'string'
    }, {
        name: 'participantId',
        mapping: 'ptcpntId',
        type: 'string'
    }, {
        name: 'relatedToVet',
        mapping: 'relatedToVet',
        type: 'string'
    }, {
        name: 'relationship',
        mapping: 'relationship',
        type: 'string'
    }, {
        name: 'ssn',
        mapping: 'ssn',
        type: 'string'
    }, {
        name: 'ssnVerifyStatus',
        mapping: 'ssnVerifyStatus',
        type: 'string'
    }, {
        name: 'stateOfBirth',
        mapping: 'stateOfBirth',
        type: 'string'
    }],
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'xml',
            record: 'persons'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.claimant.FindDependents', 
            update: '',                                                     
            destroy: ''
        }
    }
});