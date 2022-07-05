/**
* @class VIP.model.personVadir.ContactInfo
*
* The model for email associated with the person
*/
Ext.define('VIP.model.personVadir.ContactInfo', {
    extend: 'Ext.data.Model',
    
    requires: [
        'VIP.model.personVadir.Address',
        'VIP.model.personVadir.Phone',
        'VIP.model.personVadir.Email',
        'VIP.soap.envelopes.vadir.personSearch.GetContactInfo',
        'VIP.data.reader.ContactInfo'
    ],
    
    fields: [{
        name: 'edipi',
        mapping: 'edipi',
        type: 'long'
    }],
    
    hasMany: [
        {
            model: 'VIP.model.personVadir.Address',
            name: 'Addresses',
            associationKey: 'Addresses'
        },
        {
            model: 'VIP.model.personVadir.Phone',
            name: 'Phones',
            associationKey: 'Phones'
        },
        {
            model: 'VIP.model.personVadir.Email',
            name: 'Emails',
            associationKey: 'Emails'
    }],

        idProperty: 'edipi',

    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'contactinfo',
            record: 'ContactInfo'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.vadir.personSearch.GetContactInfo',
            update: '',
            destroy: ''
        }
    }

});