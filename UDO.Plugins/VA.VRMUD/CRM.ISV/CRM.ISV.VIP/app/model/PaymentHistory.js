/**
* @author Ivan Yurisevic
* @class VIP.model.PaymentHistory
*
* The model for old payment history service
*/
Ext.define('VIP.model.PaymentHistory', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.share.paymenthistory.FindPayHistoryBySSN',
        'VIP.data.reader.PaymentHistory',
        'VIP.model.paymenthistory.Payments',
        'VIP.model.paymenthistory.PaymentAddress',
        'VIP.model.paymenthistory.ReturnedPayments'
    ],
    fields: [{
        name: 'fileNumber',
        type: 'string',
        mapping: 'fileNumber'
    }, {
        name: 'fullName',
        type: 'string',
        mapping: 'fullName'
    }, {
        name: 'lastActivityDate',
        type: 'string',
        mapping: 'lastActivityDt'
    }, {
        name: 'lastFicheDate',
        type: 'string',
        mapping: 'lastFicheDt'
    }, {
        name: 'payeeCode',
        type: 'string',
        mapping: 'payeeCode'
    }, {
        name: 'priorFicheDate',
        type: 'string',
        mapping: 'priorFicheDt'
    }, {
        name: 'terminalDigit',
        type: 'string',
        mapping: 'terminalDigit'
    },
    {
        name: 'completeFileNumber',
        type: 'string',
        convert: function (v, record) {
            if (!Ext.isEmpty(record.get('terminalDigit'))) {
                return record.get('fileNumber') + record.get('terminalDigit');
            } else return '';
        }
    }],

    hasMany: [
        {
            model: 'VIP.model.paymenthistory.Payments',
            name: 'payments',
            associationKey: 'payment'
        },
        {
            model: 'VIP.model.paymenthistory.PaymentAddress',
            name: 'paymentAddress',
            associationKey: 'paymentAddresses'
        },
        {
            model: 'VIP.model.paymenthistory.ReturnedPayments',
            name: 'returnPayments',
            associationKey: 'returnPayment'
        }
    ],

    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'paymenthistory',
            record: 'paymentRecord'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.paymenthistory.FindPayHistoryBySSN',
            update: '',
            destroy: ''
        }
    }
});