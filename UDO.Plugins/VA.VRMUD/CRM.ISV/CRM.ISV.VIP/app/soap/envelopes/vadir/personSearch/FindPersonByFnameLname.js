Ext.define('VIP.soap.envelopes.vadir.personSearch.FindPersonByFnameLname', {
    extend: 'VIP.soap.envelopes.vadir.PersonSearchTemplate',
    config: {
        lastName: '',
        firstName: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('FindPersonByFnameLnameRequest', {
            namespace: 'per',
            lastName: {
                namespace: 'per',
                value: me.getLastName()
            },
            firstName: {
                namespace: 'per',
                value: me.getFirstName()
            }
        });

        me.addNamespace('per', 'http://www.va.gov/schema/crm/PersonService');
    }

});

