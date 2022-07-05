/**
* @author Josh Oliver
* @class VIP.model.Denial
*
* The model for denial record
*/
Ext.define('VIP.model.Denial', {
    extend: 'Ext.data.Model',
    requires: ['VIP.soap.envelopes.share.claimant.FindDenialsByPtcpntId'],
    fields: [{
        name: 'adminDate',
        type: 'string'
    }, {
        name: 'awardTypeName',
        type: 'string',
        mapping: 'awardTypeNm'
    }, {
        name: 'claimDate',
        type: 'date',
        dateFormat: 'mdY',
        mapping: 'claimDate'
    }, {
        name: 'claimPayeeCode',
        type: 'string',
        mapping: 'claimPayeeCd'
    }, {
        name: 'claimTypeCode',
        type: 'string',
        mapping: 'claimTypeCd'
    }, {
        name: 'claimTypeName',
        type: 'string',
        mapping: 'claimTypeNm'
    }, {
        name: 'decisionDate',
        type: 'date',
        dateFormat: 'mdY',
        mapping: 'decisionDate'
    }, {
        name: 'decisionName',
        type: 'string',
        mapping: 'decisionNm'
    }, {
        name: 'decisionType',
        type: 'string'
    }, {
        name: 'programTypeCode',
        type: 'string',
        mapping: 'programTypeCd'
    }, {
        name: 'rbaId',
        type: 'string'
    }],
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'xml',
            record: 'denials'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.claimant.FindDenialsByPtcpntId',
            update: '',
            destroy: ''
        }
    }
});