/**
* @author Josh Oliver
* @class VIP.model.mvi.CorrespondingIds
*
* The model for appointments
*/
Ext.define('VIP.model.mvi.CorrespondingIds', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.mvi.GetCorrespondingIds'
    ],
    fields: [
        { name: 'edipi', type: 'string', ignoreMappingOnRequest: true },
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
            read: 'VIP.soap.envelopes.mvi.GetCorrespondingIds',
            update: '',
            destroy: ''
        }
    }
});