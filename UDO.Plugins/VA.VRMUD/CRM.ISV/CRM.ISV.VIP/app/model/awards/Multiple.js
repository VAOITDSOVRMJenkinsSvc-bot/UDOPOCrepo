/**
* @author Jonas Dawson
* @class VIP.model.awards.Multiple
*
* The model for Awards Multiple record details
*/
Ext.define('VIP.model.awards.Multiple', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.share.claimant.FindGeneralInformationByFileNumber'
    ],
    fields: [{
        name: 'PK_awardTypeCode',
        mapping: 'awardBenePK/awardTypeCd',
        type: 'string'
    }, {
        name: 'PK_participantBeneId',
        mapping: 'awardBenePK/ptcpntBeneId',
        type: 'string'
    }, {
        name: 'PK_participantRecipientId',
        mapping: 'awardBenePK/ptcpntRecipId',
        type: 'string'
    }, {
        name: 'PK_participantVetId',
        mapping: 'awardBenePK/ptcpntVetId',
        type: 'string'
    }, {
        name: 'awardBeneTypeCode',
        mapping: 'awardBeneTypeCd',
        type: 'string'
    }, {
        name: 'awardBeneTypeName',
        mapping: 'awardBeneTypeName',
        type: 'string'
    }, {
        name: 'awardTypeCode',
        mapping: 'awardTypeCd',
        type: 'string'
    }, {
        name: 'awardTypeName',
        mapping: 'awardTypeName',
        type: 'string'
    }, {
        name: 'beneficiaryName',
        mapping: 'beneName',
        type: 'string'
    }, {
        name: 'payeeCode',
        mapping: 'payeeCd',
        type: 'string'
    }, {
        name: 'participantBeneId',
        mapping: 'ptcpntBeneId',
        type: 'string'
    }, {
        name: 'participantRecipientId',
        mapping: 'ptcpntRecipId',
        type: 'string'
    }, {
        name: 'participantVetId',
        mapping: 'ptcpntVetId',
        type: 'string'
    }, {
        name: 'recipientName',
        mapping: 'recipName',
        type: 'string'
    }, {
        name: 'vetName',
        mapping: 'vetName',
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
            record: 'awardBenes'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.claimant.FindGeneralInformationByFileNumber',
            update: '',
            destroy: ''
        }
    }
});