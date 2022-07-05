Ext.define('VIP.soap.envelopes.share.benefitclaim.FindBenefitClaim', {
    extend: 'VIP.soap.envelopes.share.BenefitClaimTemplate',
    requires: [
        'VIP.model.claims.ClaimDetail'
    ],
    config: {
        fileNumber: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer', {
        customReadRecords: function (response, currentResultSet, reader) {
            if (!currentResultSet.count && currentResultSet.success) {
                var singleClaimsResultSet,
                    singleClaimModel = Ext.create('VIP.model.claims.ClaimDetail');

                singleClaimModel.setProxy({
                    type: 'memory',
                    reader: {
                        type: 'xml',
                        record: 'participantRecord'
                    }
                });

                singleClaimsResultSet = singleClaimModel.getProxy().getReader().read(response);
                singleClaimsResultSet.success = true;
                singleClaimsResultSet['isSingleResponse'] = true;

                return singleClaimsResultSet;
            }
            else {
                return reader.read(response);
            }
        },
        scrubFilter: function (record) {
            var recordHasData = false;

            for (var i in record.data) {
                if (!Ext.isEmpty(record.data[i]) && record.data[i] != 0 && i != 'numberOfCPClaimRecords') {
                    recordHasData = true;
                    break;
                }
            }

            return recordHasData;
        }
    }),

    constructor: function (config) {
        var me = this;
        
        me.initConfig(config);

        me.callParent();

        me.setBody('findBenefitClaim', {
            namespace: 'ser',
            fileNumber: {
                namespace: '',
                value: me.getFileNumber()
            }
        });
    }

});
