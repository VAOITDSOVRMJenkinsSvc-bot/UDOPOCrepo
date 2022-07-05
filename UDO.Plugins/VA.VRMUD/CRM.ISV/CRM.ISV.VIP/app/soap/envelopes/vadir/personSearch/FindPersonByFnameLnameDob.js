Ext.define('VIP.soap.envelopes.vadir.personSearch.FindPersonByFnameLnameDob', {
    extend: 'VIP.soap.envelopes.vadir.PersonSearchTemplate',
    config: {
        lastName: '',
        firstName: '',
        dateOfBirth: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('FindPersonByFnameLnameDobRequest', {
            namespace: 'per',
            lastName: {
                namespace: 'per',
                value: me.getLastName()
            },
            firstName: {
                namespace: 'per',
                value: me.getFirstName()
            },
            dateOfBirth: {
                namespace: 'per',
                value: me.getDateOfBirth()
            }
        });

        me.addNamespace('per', 'http://www.va.gov/schema/crm/PersonService');
    }

});

