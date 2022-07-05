Ext.define('VIP.model.paymentinformation.Comerica', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.share.paymentinfo.FindComericaRoutngTrnsitNbr',
		'VIP.data.reader.Comerica', //Reader
    ],
    fields: [{
        name: 'ComericaRoutngTrnsitNbr',
        type: 'string',
        mapping: 'ComericaRoutngTrnsitNbr'
    }],

    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'comerica',
            record: 'return'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.paymentinfo.FindComericaRoutngTrnsitNbr',
            update: '',
            destroy: ''
        }
    }
});
