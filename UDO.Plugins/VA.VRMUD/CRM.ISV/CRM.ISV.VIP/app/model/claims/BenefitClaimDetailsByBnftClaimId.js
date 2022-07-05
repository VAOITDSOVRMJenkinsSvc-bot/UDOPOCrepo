Ext.define('VIP.model.claims.BenefitClaimDetailsByBnftClaimId', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.data.reader.BenefitClaimDetailsByBnftClaimId',
        'VIP.soap.envelopes.claims.BenefitClaimDetailsByBnftClaimId'
    ],
    fields: [
        {
            name: 'maxEstClaimCompleteDt',
            type: 'string',
            mapping: 'maxEstClaimCompleteDt'
        },
        {
            name: 'minEstClaimCompleteDt',
            type: 'string',
            mapping: 'minEstClaimCompleteDt'
        },
        {
            name: 'phaseChngdDt',
            type: 'string',
            mapping: 'phaseChngdDt'
        },
        {
            name: 'phaseType',
            type: 'string',
            mapping: 'phaseType'
        },
        {
            name: 'phaseTypeChangeInd',
            type: 'string',
            mapping: 'phaseTypeChangeInd'
        }
    ],
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'xml',
            record: 'bnftClaimLcStatus'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.claims.FindBenefitClaimDetailsByBnftClaimId',
            update: '',
            destroy: ''
        }
    }
});