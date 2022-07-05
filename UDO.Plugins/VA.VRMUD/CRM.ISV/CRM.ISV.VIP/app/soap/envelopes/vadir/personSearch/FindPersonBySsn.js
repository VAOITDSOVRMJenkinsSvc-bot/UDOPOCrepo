Ext.define('VIP.soap.envelopes.vadir.personSearch.FindPersonBySsn', {
    extend: 'VIP.soap.envelopes.vadir.PersonSearchTemplate',
    config: {
        ssn: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),
    
    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('FindPersonBySsnRequest', {
            namespace: 'per',
            ssn: {
                namespace: 'per',
                value: me.getSsn()
            }
        });

        me.addNamespace('per', 'http://www.va.gov/schema/crm/PersonService');
    }

});
