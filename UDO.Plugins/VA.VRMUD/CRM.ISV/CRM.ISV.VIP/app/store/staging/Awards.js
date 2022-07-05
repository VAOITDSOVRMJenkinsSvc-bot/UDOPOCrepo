/**
* @author Jonas Dawson
* @class VIP.store.staging.Awards
*
* Staging store in the event of a multiple response
*/
Ext.define('VIP.store.staging.Awards', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.Awards'
    ],
    model: 'VIP.model.Awards',
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'xml',
            record: 'return'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.claimant.FindGeneralInformationByPtcpntIds',
            update: '',
            destroy: ''
        }
    }
});

