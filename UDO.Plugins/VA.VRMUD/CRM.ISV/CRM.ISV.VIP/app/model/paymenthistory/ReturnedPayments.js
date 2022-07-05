/**
* @author Ivan Yurisevic
* @class VIP.model.paymenthistory.ReturnedPayments
*
* The model for old payment history service
*/
Ext.define('VIP.model.paymenthistory.ReturnedPayments', {
    extend: 'Ext.data.Model',
//    requires: ['VIP.soap.envelopes.share.paymenthistory.FindPayHistoryBySSN'],
    fields: [{
        name: 'checkAmount',
        type: 'string',
        mapping: 'returnedCheckAmount'
    }, {
        name: 'checkIssueDate',
        type: 'string',
        mapping: 'returnedCheckIssueDt'
    }, {
        name: 'checkCancelDate',
        type: 'string',
        mapping: 'returnedCheckCancelDt'
    }, {
        name: 'checkNum',
        type: 'string',
        mapping: 'returnedCheckNum'
    }, {
        name: 'checkReason',
        type: 'string',
        mapping: 'returnedCheckReason'
    }, {
        name: 'checkType',
        type: 'string',
        mapping: 'returnedCheckType'
    }, {
        name: 'checkRO',
        type: 'string',
        mapping: 'returnedCheckRO'
    }, {
        name: 'checkReturnFiche',
        type: 'string',
        mapping: 'returnedCheckReturnFiche'
    }],

    //belongsTo: 'VIP.model.PaymentHistory',

    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'returnPayments'
        }
    }
});