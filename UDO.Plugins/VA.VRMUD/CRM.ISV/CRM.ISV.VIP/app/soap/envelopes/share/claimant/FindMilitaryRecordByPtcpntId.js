Ext.define('VIP.soap.envelopes.share.claimant.FindMilitaryRecordByPtcpntId', {
    extend: 'VIP.soap.envelopes.share.ClaimantTemplate',
    alias: 'envelopes.FindMilitaryRecordByPtcpntId',
    config: {
        ptcpntId: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),
    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();
        //http://vbmscert.vba.va.gov/ClaimantServiceBean/ClaimantWebService
        me.setBody('findMilitaryRecordByPtcpntId', {
            namespace: 'ser',
            ptcpntId: {
                namespace: '',
                value: me.getPtcpntId()
            }
        });
    }

});
