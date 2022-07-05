
Ext.define('VIP.model.PersonVadir', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.vadir.personSearch.FindPersonBySsn',
        'VIP.data.reader.PersonVadir',
        'VIP.model.personVadir.Alias'
    ],
    
    fields: [
        {
            name: 'ssn',
            type: 'string',
            mapping: 'socialSecurityNumber',
            ignoreMappingOnRequest: true
        },
        {
            name: 'vaId',
            type: 'string',
            mapping: 'vaId'
        }, 
        {
            name: 'firstName',
            type: 'string',
            mapping: 'firstName'
        },
        {
            name: 'middleName',
            type: 'string',
            mapping: 'middleName'
        },
        {
            name: 'lastName',
            type: 'string',
            mapping: 'lastName'
        },
        {
            name: 'cadency',
            type: 'string',
            mapping: 'cadency'
        },
        {
            name: 'dob',
            type: 'date',
            dateFormat: 'c',
            mapping: 'dateOfBirth'
        },
        {
            name: 'dod',
            type: 'date',
            dateFormat: 'c',
            mapping: 'dateOfDeath'
        },
        {
            name: 'deathInd',
            type: 'string',
            mapping: 'deathInd' 
        },
        {
            name: 'gender',
            type: 'string',
            mapping: 'gender'
        }
    ],

    hasMany: [
        {
            model: 'VIP.model.personVadir.Alias',
            name: 'aliases',
            associationKey: 'aliases'
        }
    ],

    idProperty: 'vaId',
    
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'personVadir',
            record: 'person'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.vadir.personSearch.FindPersonBySsn',
            update: '',
            destroy: ''
        }
    }
}); 