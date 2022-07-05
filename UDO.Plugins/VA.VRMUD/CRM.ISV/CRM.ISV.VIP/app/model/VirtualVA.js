/**
* @author Josh Oliver
* @class VIP.model.VirtualVA
*
* The model for denial record
*/
Ext.define('VIP.model.VirtualVA', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'fileName',
        type: 'string'
    }, {
        name: 'url',
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
            record: 'urls'
        },
        envelopes: {
            create: '',
            read: '',
            update: '',
            destroy: ''
        }
    }
});