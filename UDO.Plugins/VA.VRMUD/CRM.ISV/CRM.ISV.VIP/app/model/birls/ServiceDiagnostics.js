/**
* @author Ivan Yurisevic
* @class VIP.model.birls.ServiceDiagnostics
*
* The model for BIRLS Flashes record details
*/
Ext.define('VIP.model.birls.ServiceDiagnostics', {
    extend: 'Ext.data.Model',
//    requires: [
//        'VIP.soap.envelopes.share.vetrecord.FindBirlsRecordByFileNumber'
//    ],
    fields: [{
        name: 'serviceDiagnostics',
        mapping: 'SERVICE_DIAGNOSTICS',
        type: 'string'
    }, {
        name: 'servicePercent1',
        mapping: 'SERVICE_PERCENT1',
        type: 'string'
    }, {
        name: 'servicePercent2',
        mapping: 'SERVICE_PERCENT2',
        type: 'string'
    }, {
        name: 'recurringAnalogusCode',
        mapping: 'RECUR_ANALOGUS_CODE',
        type: 'string'
    }, {
        name: 'recurringServiceConnectedDisability',
        mapping: 'RECUR_SVC_CON_DISABILITY',
        type: 'string'
    }],

    belongsTo: 'VIP.model.Birls',
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'SERVICEDIAGNOSTICS'
        }
    }

//    proxy: {
//        type: 'soap',
//        headers: {
//            "SOAPAction": "",
//            "Content-Type": "text/xml; charset=utf-8"
//        },
//        reader: {
//            type: 'xml',
//            record: 'SERVICEDIAGNOSTICS'
//        },
//        envelopes: {
//            create: '',
//            read: 'VIP.soap.envelopes.share.vetrecord.FindBirlsRecordByFileNumber',
//            update: '',
//            destroy: ''
//        }
//    }
});