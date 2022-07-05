Ext.define('VIP.store.claims.BenefitClaimDetailsByBnftClaimId', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.claims.BenefitClaimDetailsByBnftClaimId',
        'VIP.soap.envelopes.share.claims.FindBenefitClaimDetailsByBnftClaimId'
    ],
    model: 'VIP.model.claims.BenefitClaimDetailsByBnftClaimId'
});