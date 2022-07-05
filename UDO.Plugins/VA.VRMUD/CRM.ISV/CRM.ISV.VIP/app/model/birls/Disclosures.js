/**
* @author Ivan Yurisevic
* @class VIP.model.birls.Disclosures
*
* The model for BIRLS Disclosures record details
*/
Ext.define('VIP.model.birls.Disclosures', {
    extend: 'Ext.data.Model',
//    requires: [
//        'VIP.soap.envelopes.share.vetrecord.FindBirlsRecordByFileNumber'
//    ],
    fields: [{
        name: 'recurringDisclosureNum',
        mapping: 'RECUR_DISCLOSURE_NUM',
        type: 'string'
    }, {
        name: 'dateOfDisclosure',
        mapping: 'DATE_OF_DISCLOSURE',
        type: 'string'
    }, {
        name: 'recurringDisclosureMonth',
        mapping: 'RECUR_DISCLOSURE_MONTH',
        type: 'string'
    }, {
        name: 'recurringDisclosureYear',
        mapping: 'RECUR_DISCLOSURE_YEAR',
        type: 'string'
    }],

    belongsTo: 'VIP.model.Birls',
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'RECURING_DISCLOSURE'
        }
    }
//    
//    proxy: {
//        type: 'soap',
//        headers: {
//            "SOAPAction": "",
//            "Content-Type": "text/xml; charset=utf-8"
//        },
//        reader: {
//            type: 'xml',
//            record: 'RECURING_DISCLOSURE'
//        },
//        envelopes: {
//            create: '',
//            read: 'VIP.soap.envelopes.share.vetrecord.FindBirlsRecordByFileNumber',
//            update: '',
//            destroy: ''
//        }
//    }
});