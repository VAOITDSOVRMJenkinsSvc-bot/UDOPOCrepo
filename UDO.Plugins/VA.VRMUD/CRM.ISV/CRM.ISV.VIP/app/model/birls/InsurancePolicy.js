/**
* @author Ivan Yurisevic
* @class VIP.model.birls.InsurancePolicy
*
* The model for BIRLS record details
*/
Ext.define('VIP.model.birls.InsurancePolicy', {
    extend: 'Ext.data.Model',
//    requires: [
//        'VIP.soap.envelopes.share.vetrecord.FindBirlsRecordByFileNumber'
//    ],
    fields: [{
        name: 'insurancePolicyPrefix',
        mapping: 'INS_POL_PREFIX',
        type: 'string'
    }, {
        name: 'insurancePolicyNumber',
        mapping: 'INS_POL_NUMBER',
        type: 'string'
    }],
    
    belongsTo: 'VIP.model.Birls',
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'INSURANCE_POLICY'
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
//            record: 'INSURANCE_POLICY'
//        },
//        envelopes: {
//            create: '',
//            read: 'VIP.soap.envelopes.share.vetrecord.FindBirlsRecordByFileNumber',
//            update: '',
//            destroy: ''
//        }
//    }
});