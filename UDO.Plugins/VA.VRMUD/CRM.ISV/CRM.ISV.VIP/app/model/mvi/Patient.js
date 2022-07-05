/**
* @author Josh Oliver
* @class VIP.model.mvi.Patient
*
* The model for appointments
*/
Ext.define('VIP.model.mvi.Patient', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.mvi.PersonSearch'
    ],
    fields: [
        { name: 'nationalId', type: 'string', ignoreMappingOnRequest: true },
        { name: 'ssn', type: 'string', ignoreMappingOnRequest: true },
        { name: 'edipi', type: 'string', ignoreMappingOnRequest: true },
        { name: 'name', type: 'string', ignoreMappingOnRequest: true },
        { name: 'gender', type: 'string', ignoreMappingOnRequest: true },
        { name: 'dob', type: 'date', dateFormat: 'Ymd', ignoreMappingOnRequest: true },
		{ name: 'city', type: 'string' },
		{ name: 'state', type: 'string' },
		{ name: 'country', type: 'string' },
        { name: 'birthplace', type: 'string', convert:
			function (v, record) {
			    var s = '';
			    if (!Ext.isEmpty(record.data.city)) { s = record.data.city; }
			    if (!Ext.isEmpty(record.data.state)) { s = s + (s.length > 0 ? ', ' : '') + record.data.state; }
			    if (!Ext.isEmpty(record.data.country)) { s = s + (s.length > 0 ? ' ' : '') + record.data.country; }
			    return s;
			}
        },
        { name: 'fullNationalId', type: 'string' },
        { name: 'fullSsn', type: 'string' },
        { name: 'fullEdipi', type: 'string' }
    ],
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'xml',
            record: 'Person'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.mvi.PersonSearch',
            update: '',
            destroy: ''
        }
    }
});