/**
* @author Ivan Yurisevic
* @class VIP.model.birls.AlternateNames
*
* The model for BIRLS AlternateNames record details
*/
Ext.define('VIP.model.birls.AlternateNames', {
    extend: 'Ext.data.Model',
//    requires: [
//        'VIP.soap.envelopes.share.vetrecord.FindBirlsRecordByFileNumber'
//    ],
    fields: [{
        name: 'altLastName',
        mapping: 'ALT_LAST_NAME',
        type: 'string'
    }, {
        name: 'altFirstName',
        mapping: 'ALT_FIRST_NAME',
        type: 'string'
    }, {
        name: 'altMiddleName',
        mapping: 'ALT_MIDDLE_NAME',
        type: 'string'
    }, {
        name: 'altNameSuffix',
        mapping: 'ALT_NAME_SUFFIX',
        type: 'string'
    }],
    belongsTo: 'VIP.model.Birls',
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'ALTERNATE_NAME'
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
//            record: 'ALTERNATE_NAME'
//        },
//        envelopes: {
//            create: '',
//            read: 'VIP.soap.envelopes.share.vetrecord.FindBirlsRecordByFileNumber',
//            update: '',
//            destroy: ''
//        }
//    }
});
