Ext.define('VIP.soap.envelopes.share.vetrecord.FindVeteranByFileNumber', {
    extend: 'VIP.soap.envelopes.share.VetRecordTemplate',
    alias: 'envelopes.FindVeteranByFileNumber', 
    config: {
        fileNumber: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();
        
        me.setBody('findVeteranByFileNumber', {
            namespace: 'ser',
            fileNumber: me.getFileNumber()
        });
    }

});
