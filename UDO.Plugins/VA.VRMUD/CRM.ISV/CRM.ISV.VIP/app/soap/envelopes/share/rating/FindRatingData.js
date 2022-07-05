Ext.define('VIP.soap.envelopes.share.rating.FindRatingData', {
    extend: 'VIP.soap.envelopes.share.RatingTemplate',
    alias: 'envelopes.FindRatingData',
    config: {
        fileNumber: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),
    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findRatingData', {
            namespace: 'ser',
            fileNumber: {
                namespace: '',
                value: me.getFileNumber()
            }
        });

    }

});
