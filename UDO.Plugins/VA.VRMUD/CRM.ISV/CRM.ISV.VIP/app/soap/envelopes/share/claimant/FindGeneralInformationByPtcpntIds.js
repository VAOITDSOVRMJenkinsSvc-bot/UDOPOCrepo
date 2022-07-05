Ext.define('VIP.soap.envelopes.share.claimant.FindGeneralInformationByPtcpntIds', {
    extend: 'VIP.soap.envelopes.share.ClaimantTemplate',
    requires: [
        'VIP.util.Xml'
    ],
    config: {
        ptcpntVetId: '',
        ptcpntBeneId: '',
        ptpcntRecipId: '',
        awardTypeCd: ''
    },
    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

//    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer', {
//        customReadRecords: function (response, currentResultSet, reader) {
//            var newResultSet;
//            debugger;
//            //TODO: gets undefined response, crashes. **** .responseXML is not a field!! it is .xml.
//            // Also - reader.read(response.xml) - this doesn't properly return a resultset.  Wrapping 
//            // is not necessary here either. Models read xml fine without it.
//            if (Ext.isEmpty(response) || Ext.isEmpty(response.responseXML)) return null;

//            VIP.util.Xml.wrap("flash", "flashes", "return", response);
//            VIP.util.Xml.wrap("evr", "evrs", "return", response);
//            VIP.util.Xml.wrap("diary", "diaries", "return", response);

//            newResultSet = reader.read(response.responseXML);

//            return newResultSet;
//        }
//    }),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findGeneralInformationByPtcpntIds', {
            namespace: 'ser',
            ptcpntVetId: {
                namespace: '',
                value: me.getPtcpntVetId()
            },
            ptcpntBeneId: {
                namespace: '',
                value: me.getPtcpntBeneId()
            },
            ptpcntRecipId: {
                namespace: '',
                value: me.getPtpcntRecipId()
            },
            awardTypeCd: {
                namespace: '',
                value: me.getAwardTypeCd()
            }
        });
    }
});
