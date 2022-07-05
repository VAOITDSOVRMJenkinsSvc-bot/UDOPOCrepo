/**
* @class VIP.model.personVadir.Email
*
* The model for email associated with the person
*/
Ext.define('VIP.model.personVadir.Email', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'emailaddress',
        mapping: 'emailAddress',
        type: 'string'
    }, {
        name: 'emailsource',
        mapping: 'emailSource',
        type: 'string'
    }, {
        name: 'emailaddresspriority',
        mapping: 'emailAddressPriority',
        type: 'string'
    }],

    //Start Proxy
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'Email'
        }
    }

});