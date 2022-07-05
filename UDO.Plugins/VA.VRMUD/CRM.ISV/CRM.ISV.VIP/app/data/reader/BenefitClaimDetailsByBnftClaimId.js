Ext.define('VIP.data.reader.BenefitClaimDetailsByBnftClaimId', {
    extend: 'Ext.data.reader.Xml',
    alias: 'reader.BenefitClaimDetailsByBnftClaimId',
    requires: [
        'VIP.util.Xml'
    ],
    read: function (response) {
        var me = this;

        VIP.util.Xml.wrap("bnftClaimLcStatus", "bnftClaimLcStatus", "BenefitClaimDetailsDTO", response);

        return me.callParent([response]);
    }
});