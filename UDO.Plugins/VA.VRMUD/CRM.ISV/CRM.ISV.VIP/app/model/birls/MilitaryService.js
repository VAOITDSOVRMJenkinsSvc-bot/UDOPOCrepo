/**
* @author Ivan Yurisevic
* @class VIP.model.birls.MilitaryService
*
* The model for BIRLS Military Service record details
*/
Ext.define('VIP.model.birls.MilitaryService', {
    extend: 'Ext.data.Model',
//    requires: [
//        'VIP.soap.envelopes.share.vetrecord.FindBirlsRecordByFileNumber'
//    ],
    fields: [{
        name: 'shortServiceNumber',
        mapping: 'SHORT_SERVICE_NUMBER',
        type: 'string'
    }, {
        name: 'serviceNumberFill',
        mapping: 'SERVICE_NUMBER_FILL',
        type: 'string'
    }, {
        name: 'branchOfService',
        mapping: 'BRANCH_OF_SERVICE',
        type: 'string'
    }, {
        name: 'enteredOnDutyDate',
        mapping: 'ENTERED_ON_DUTY_DATE',
        type: 'date',
        dateFormat: 'mdY'
    }, {
        name: 'releasedActiveDutyDate',
        mapping: 'RELEASED_ACTIVE_DUTY_DATE',
        type: 'date',
        dateFormat: 'mdY'
    }, {
        name: 'separationReasonCode',
        mapping: 'SEPARATION_REASON_CODE',
        type: 'string'
    }, {
        name: 'nonpayDays',
        mapping: 'NONPAY_DAYS',
        type: 'string'
    }, {
        name: 'payGrade',
        mapping: 'PAY_GRADE',
        type: 'string'
    }, {
        name: 'charOfServiceCode',
        mapping: 'CHAR_OF_SVC_CODE',
        type: 'string'
    }],

    belongsTo: 'VIP.model.Birls',
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'SERVICE'
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
//            record: 'SERVICE'
//        },
//        envelopes: {
//            create: '',
//            read: 'VIP.soap.envelopes.share.vetrecord.FindBirlsRecordByFileNumber',
//            update: '',
//            destroy: ''
//        }
//    }
});