Ext.define('VIP.soap.envelopes.vadir.personSearch.GetContactInfo', {
    extend: 'VIP.soap.envelopes.vadir.PersonSearchTemplate',
    config: {
        edipi: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),
    
    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('GetContactInfoRequest', {
            namespace: 'per',
            edipi: {
                namespace: 'per',
                value: me.getEdipi()
            }
        });

        me.addNamespace('per', 'http://www.va.gov/schema/crm/PersonService');
    }

});
