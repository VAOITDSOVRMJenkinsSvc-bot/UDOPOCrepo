Ext.define('VIP.soap.envelopes.ebenefits.Ebenefits', {
    extend: 'VIP.soap.envelopes.ebenefits.EbenefitsTemplate',
    requires: [
        'VIP.util.Xml'
    ],
    alias: 'envelopes.Ebenefits',
    config: {
        edipi: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('getRegistrationStatus', {
            namespaces: {
                'ret': 'http://www.ebenefits.va.gov/wsdl/RetrieveAccountActivityService/'
            },
            namespace: 'ret',
            edipi: {
                namespace: '',
                value: me.getEdipi()
            }
        });
    }

});
