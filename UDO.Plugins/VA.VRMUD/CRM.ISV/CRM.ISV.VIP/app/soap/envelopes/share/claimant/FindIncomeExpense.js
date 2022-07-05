Ext.define('VIP.soap.envelopes.share.claimant.FindIncomeExpense', {
    extend: 'VIP.soap.envelopes.share.ClaimantTemplate',
    alias: 'envelopes.FindIncomeExpense',
    requires: [
        'VIP.util.Xml'
    ],
    config: {
        ptcpntVetId: '',
        ptcpntBeneId: '',
        isDacRequest: true
    },


    /* @analyzeResponse customRecords - Ivan
    * The purpose of this function is the need to create custom XML response so that we reduce number of Web Service 
    * calls allow the hasMany associations to work.
    */
    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findIncomeExpense', {
            namespace: 'ser',
            ptcpntVetId: {
                namespace: '',
                value: me.getPtcpntVetId()
            },
            ptcpntBeneId: {
                namespace: '',
                value: me.getPtcpntBeneId()
            }
        });
    }

});
