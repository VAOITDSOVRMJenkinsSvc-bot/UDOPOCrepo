Ext.define('VIP.soap.envelopes.share.benefitclaim.InsertPresidentialMemorialCertificate', {
    extend: 'VIP.soap.envelopes.share.BenefitClaimTemplate',
    alias: 'envelopes.InsertPresidentialMemorialCertificate',
    config: {
        fileNumber: '',
        veteranFirstName: '',
        veteranMiddleInitial: '',
        veteranLastName: '',
        veteranSuffixName: '',
        station: '',
        salutation: '',
        title: '',
        addressLine1: '',
        addressLine2: '',
        city: '',
        state: '',
        zipCode: '',
        realtionshipToVeteran: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('insertPresidentialMemorialCertificate', {
            namespace: 'ser',
            fileNumber: {
                namespace: '',
                value: me.getFileNumber()
            },
            veteranFirstName: {
                namespace: '',
                value: me.getVeteranFirstName()
            },
            veteranMiddleInitial: {
                namespace: '',
                value: me.getVeteranMiddleInitial()
            },
            veteranLastName: {
                namespace: '',
                value: me.getVeteranLastName()
            },
            veteranSuffixName: {
                namespace: '',
                value:  me.getVeteranSuffixName()
            },
            station: {
                namespace: '',
                value: me.getStation()
            },
            salutation: {
                namespace: '',
                value: me.getSalutation()
            },
            title: {
                namespace: '',
                value: me.getTitle()
            },
            addressLine1: {
                namespace: '',
                value: me.getAddressLine1()
            }, 
            addressLine2: {
                namespace: '',
                value: me.getAddressLine2()
            },
            city: {
                namespace: '',
                value: me.getCity()
            },
            state: {
                namespace: '',
                value: me.getState()
            },
            zipCode: {
                namespace: '',
                value: me.getZipCode()
            },
            realtionshipToVeteran: {
                namespace: '',
                value: me.getRealtionshipToVeteran() //misspelled node name in the web service
            } 
        });

        if (Ext.isEmpty(me.getVeteranMiddleInitial())) {
            delete Envelope.Body.MethodName['veteranMiddleInitial'];
        }

        if (Ext.isEmpty(me.getVeteranSuffixName())) {
            delete Envelope.Body.MethodName['veteranSuffixName'];
        }

        if (Ext.isEmpty(me.getAddressLine2())) {
            delete Envelope.Body.MethodName['addressLine2'];
        }
    }

});
