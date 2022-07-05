Ext.define('VIP.soap.envelopes.vadir.personSearch.FindPersonByLnameDob', {
    extend: 'VIP.soap.envelopes.vadir.PersonSearchTemplate',
    config: {
        lastName: '',
        dateOfBirth: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('FindPersonByLnameDobRequest', {
            namespace: 'per',
            lastName: {
                namespace: 'per',
                value: me.getLastName()
            },
            dateOfBirth: {
                namespace: 'per',
                value: me.getDateOfBirth()
            }
        });

        me.addNamespace('per', 'http://www.va.gov/schema/crm/PersonService');
    }

});

