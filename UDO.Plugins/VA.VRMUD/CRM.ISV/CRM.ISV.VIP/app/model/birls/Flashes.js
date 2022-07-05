/**
* @author Ivan Yurisevic
* @class VIP.model.birls.Flashes
*
* The model for BIRLS Flashes record details
*/
Ext.define('VIP.model.birls.Flashes', {
    extend: 'Ext.data.Model',
//    requires: [
//        'VIP.soap.envelopes.share.vetrecord.FindBirlsRecordByFileNumber'
//    ],
    fields: [{
        name: 'flashCode',
        mapping: 'FLASH_CODE',
        type: 'string'
    }, {
        name: 'flashStation',
        mapping: 'FLASH_STATION',
        type: 'string'
    }, {
        name: 'flashRoutingSymbol',
        mapping: 'FLASH_ROUTING_SYMBOL',
        type: 'string'
    }],

    belongsTo: 'VIP.model.Birls',
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'FLASH'
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
//            record: 'FLASH'
//        },
//        envelopes: {
//            create: '',
//            read: 'VIP.soap.envelopes.share.vetrecord.FindBirlsRecordByFileNumber',
//            update: '',
//            destroy: ''
//        }
//    }
});