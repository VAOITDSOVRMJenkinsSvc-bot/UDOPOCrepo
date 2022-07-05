/**
* @author Ivan Yurisevic
* @class VIP.model.paymenthistory.Payments
*
* The model for old payment history service
*/
Ext.define('VIP.model.paymenthistory.Payments', {
    extend: 'Ext.data.Model',
    requires: ['VIP.model.paymenthistory.PaymentAddress'],
    fields: [{
        name: 'payCheckType',
        type: 'string',
        mapping: 'payCheckType'
    }, {
        name: 'payCheckAmount',
        type: 'string',
        mapping: 'payCheckAmount'
    }, {
        name: 'payCheckDate',
        type: 'date',
        dateFormat: 'm/d/Y',
        mapping: 'payCheckDt'
    }, {
        name: 'payCheckID',
        type: 'string',
        mapping: 'payCheckID'
    }, {
        name: 'payCheckReturnFiche',
        type: 'string',
        mapping: 'payCheckReturnFiche'
    }],

    hasMany: [
        {
            model: 'VIP.model.paymenthistory.PaymentAddress',
            name: 'paymentAddress',
            storeConfig: {
                filters: []
            }
        }
    ],

    //    belongsTo: 'VIP.model.PaymentHistory',

    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'payments'
        }
    }
});