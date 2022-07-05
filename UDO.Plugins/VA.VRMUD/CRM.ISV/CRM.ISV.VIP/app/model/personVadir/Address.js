/**
* @class VIP.model.personVadir.Address
*
* The model for address associated with the person
*/
Ext.define('VIP.model.personVadir.Address', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'effectiveDate',
        type: 'date',
        dateFormat: 'c',
        mapping: 'effectiveDate'
        
    }, {
        name: 'addressType',
        mapping: 'addressType',
        type: 'string'
    }, {
        name: 'addressLine1',
        mapping: 'addressLine1',
        type: 'string'
    }, {
        name: 'addressLine2',
        mapping: 'addressLine2',
        type: 'string'
    }, {
        name: 'city',
        mapping: 'city',
        type: 'string'
    }, {
        name: 'state',
        mapping: 'state',
        type: 'string'
    }, {
        name: 'zipcode',
        mapping: 'zipcode',
        type: 'string'
    }, {
        name: 'zipcodeExtension',
        mapping: 'zipcodeExtension',
        type: 'string'
    }, {
        name: 'countryCode',
        mapping: 'countryCode',
        type: 'string'
    }],

    //Start Proxy
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'Address'
        }
    }

});