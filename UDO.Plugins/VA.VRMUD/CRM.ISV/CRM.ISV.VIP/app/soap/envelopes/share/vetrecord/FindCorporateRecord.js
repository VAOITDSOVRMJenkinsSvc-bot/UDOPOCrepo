Ext.define('VIP.soap.envelopes.share.vetrecord.FindCorporateRecord', {
    extend: 'VIP.soap.envelopes.share.VetRecordTemplate',
    alias: 'envelopes.FindCorporateRecord',
    uses: [
        'Ext.util.MixedCollection'
    ],
    config: {
        firstName: '',
        lastName: '',
        middleName: '',
        dateOfBirth: '',
        city: '',
        state: '',
        zipCode: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;
        
        me.initConfig(config);

        me.callParent();

        me.setBody('findCorporateRecord', {
            namespace: 'ser',
            ptcpntSearchPSNInput: {
                namespace: '',
                firstName: me.getFirstName(),
                lastName: me.getLastName(),
                middleName: me.getMiddleName(),
                dateOfBirth: me.getDateOfBirth(),
                city: me.getCity(),
                state: me.getState(),
                zipCode: me.getZipCode()
            }
        });

        if (Ext.isEmpty(me.getDateOfBirth())) {
            delete me.request.Envelope.Body.findCorporateRecord.ptcpntSearchPSNInput['dateOfBirth'];
        }

        if (Ext.isEmpty(me.getCity())) {
            delete me.request.Envelope.Body.findCorporateRecord.ptcpntSearchPSNInput['city'];
        }

        if (Ext.isEmpty(me.getState())) {
            delete me.request.Envelope.Body.findCorporateRecord.ptcpntSearchPSNInput['state'];
        }

        if (Ext.isEmpty(me.getZipCode())) {
            delete me.request.Envelope.Body.findCorporateRecord.ptcpntSearchPSNInput['zipCode'];
        }
    }

});



