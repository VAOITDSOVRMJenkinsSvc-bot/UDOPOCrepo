/**
* @author Stan Salatov
* @class VIP.model.ebenefits.Ebenefits
*
* The model for Ebenefits
*/
Ext.define('VIP.model.ebenefits.Ebenefits', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.ebenefits.Ebenefits'
    ],
    fields: [
		{ name: 'isRegistered', type: 'boolean' },
		{ name: 'credLevelAtLastLogin', type: 'string' },
		{ name: 'status', type: 'string' }
    ],
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'xml',
            record: 'GetRegistrationStatusResponse'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.ebenefits.Ebenefits',
            update: '',
            destroy: ''
        }
    }
});