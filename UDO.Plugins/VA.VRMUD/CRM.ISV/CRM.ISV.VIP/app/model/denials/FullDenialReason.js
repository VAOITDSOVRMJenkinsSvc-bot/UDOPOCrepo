/**
* @author Ivan Yurisevic
* @class VIP.model.denials.FullDenialReason
*
* The model for denial FullDenialReason record
*/
Ext.define('VIP.model.denials.FullDenialReason', {
    extend: 'Ext.data.Model',
    requires: ['VIP.soap.envelopes.share.claimant.FindReasonsByRbaIssueId'],
    fields: [{
        name: 'reason',
        type: 'string',
        mapping: 'return'
    }],
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'xml',
            record: 'findReasonsByRbaIssueIdResponse'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.claimant.FindReasonsByRbaIssueId',
            update: '',
            destroy: ''
        }
    }
});