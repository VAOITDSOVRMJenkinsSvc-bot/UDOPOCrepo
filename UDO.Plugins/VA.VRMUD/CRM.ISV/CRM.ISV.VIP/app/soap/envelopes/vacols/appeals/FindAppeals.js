Ext.define('VIP.soap.envelopes.vacols.appeals.FindAppeals', {
    extend: 'VIP.soap.envelopes.vacols.VacolsTemplate',
    alias: 'envelopes.FindAppeals',
    config: {
        ssn: '',
        firstName: '',
        lastName: '',
        dateOfBirth: '',
        city: '',
        state: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this,
            appealCriteriaObject;

        me.initConfig(config);

        me.callParent();
        
        appealCriteriaObject = {
            namespace: 'app'
        };

        if (!Ext.isEmpty(me.getSsn())) {
            appealCriteriaObject.SSN = {
                namespace: '',
                value: me.getSsn()
            };
        }

        if (!Ext.isEmpty(me.getFirstName())) {
            appealCriteriaObject.FirstName = {
                namespace: '',
                attributes: {
                    'Partialflag': {
                        namespace: 'app1',
                        value: 'true',
                        prefix: 'app1'
                    }
                },
                value: me.getFirstName()
            };
        }

        if (!Ext.isEmpty(me.getLastName())) {
            appealCriteriaObject.LastName = {
                namespace: '',
                attributes: {
                    'Partialflag': {
                        namespace: 'app1',
                        value: 'true',
                        prefix: 'app1'
                    }
                },
                value: me.getLastName()
            };
        }

        if (!Ext.isEmpty(me.getDateOfBirth())) {
            var dob = Ext.Date.format(new Date(me.getDateOfBirth()), 'Y-m-d');
            appealCriteriaObject.DateOfBirth = {
                namespace: '',
                value: dob
            };
        }

        if (!Ext.isEmpty(me.getCity())) {
            appealCriteriaObject.City = {
                namespace: '',
                attributes: {
                    'Partialflag': {
                        namespace: 'app1',
                        value: 'true',
                        prefix: 'app1'
                    }
                },
                value: me.getCity()
            };
        }

        if (!Ext.isEmpty(me.getState())) {
            appealCriteriaObject.State = {
                namespace: '',
                value: me.getState()
            };
        }

        me.setBody('findAppeals', {
            namespaces: {
                'app': 'http://www.va.gov/schema/AppealService',
                'app1': 'http://www.va.gov/schema/Appeals_LDM'
            },
            namespace: 'app',
            findAppealCriteria: appealCriteriaObject
        });
    }

});