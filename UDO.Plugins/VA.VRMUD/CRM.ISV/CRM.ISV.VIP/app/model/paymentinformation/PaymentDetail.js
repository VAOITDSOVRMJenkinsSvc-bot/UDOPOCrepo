/**
* @author Ivan Yurisevic
* @class VIP.model.paymentinformation.PaymentDetail
*
* The model for paymentinformation payment detail
*/
Ext.define('VIP.model.paymentinformation.PaymentDetail', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.share.paymentinfo.RetrievePaymentDetail', //SOAP
        'VIP.model.paymentinformation.AwardAdjustmentVo',
        'VIP.model.paymentinformation.AwardReason',
        'VIP.model.paymentinformation.PaymentAdjustmentVo'
    ],
    fields: [{
        name: 'paymentId',
        type: 'string',
        mapping: 'paymentIdentifier/paymentID'
    }, {
        name: 'transactionId',
        type: 'string',
        mapping: 'paymentIdentifier/transactionID'
    }, {
        name: 'responseCode',
        type: 'string',
        mapping: 'response/responseCode'
    }, {
        name: 'responseText',
        type: 'string',
        mapping: 'response/responseCode'
    }, {
        name: 'awardEffectiveDate',
        dateFormat: 'c', //ISO 8601 date
        type: 'date',
        mapping: 'awardAdjustments/awardEffectiveDate'
    }, {
        name: 'awardEffectiveDate_F',
        convert: function (v, record) {
            var input = record.get('awardEffectiveDate');
            if (!Ext.isEmpty(input)) {
                return _dtZoneless(input.toUTCString());
            } else return '';
        }
    }, {
        name: 'grossAwardAmount',
        type: 'string',
        mapping: 'awardAdjustments/grossAwardAmount',
        convert: toCurrency
    }, {
        name: 'netAwardAmount',
        type: 'string',
        mapping: 'awardAdjustments/netAwardAmount',
        convert: toCurrency
    }, {
        name: 'grossPaymentAmount',
        type: 'string',
        mapping: 'paymentAdjustments/grossPaymentAmount',
        convert: toCurrency
    }, {
        name: 'netPaymentAmount',
        type: 'string',
        mapping: 'paymentAdjustments/netPaymentAmount',
        convert: toCurrency
    }],

    //Start Associations
    hasMany: [{
        model: 'VIP.model.paymentinformation.AwardAdjustmentVo',
        name: 'awardAdjustments',
        associationKey: 'awardAdjustmentList'
    }, {
        model: 'VIP.model.paymentinformation.AwardReason',
        name: 'awardReasons',
        associationKey: 'awardReasonList'
    }, {
        model: 'VIP.model.paymentinformation.PaymentAdjustmentVo',
        name: 'paymentAdjustments',
        associationKey: 'paymentAdjustments'
    }],
    //Start PROXY
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'xml',
            record: 'PaymentDetailResponse'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.paymentinfo.RetrievePaymentDetail',
            update: '',
            destroy: ''
        }
    }
});

function toCurrency(v, record) {
    return Ext.util.Format.usMoney(v);
}