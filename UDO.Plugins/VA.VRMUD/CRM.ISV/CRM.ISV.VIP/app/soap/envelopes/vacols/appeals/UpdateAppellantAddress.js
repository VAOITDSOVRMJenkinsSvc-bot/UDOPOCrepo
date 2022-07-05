Ext.define('VIP.soap.envelopes.vacols.appeals.UpdateAppellantAddress', {
    extend: 'VIP.soap.envelopes.vacols.VacolsTemplate',
    alias: 'envelopes.UpdateAppellantAddress',
    config: {
        ssn: '',
        AppellantAddressLine1: '',
        AppellantAddressLine2: '',
        AppellantAddressCityName: '',
        AppellantAddressStateCode: '',
        AppellantAddressZipCode: '',
        AppellantAddressCountryName: '',
        AppellantAddressNotes: '',
        AppellantWorkPhoneNumber: '',
        AppellantHomePhoneNumber: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),
    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('updateAppellantAddress', {
            namespace: 'app',
            updateAddressCriteria: {
                ssn: me.getSsn(),
                AppellantAddress: {
                    namespace: '',
                    AppellantAddressLine1: me.getAppellantAddressLine1(),
                    AppellantAddressLine2: me.getAppellantAddressLine2(),
                    AppellantAddressCityName: me.getAppellantAddressCityName(),
                    AppellantAddressStateCode: me.getAppellantAddressStateCode(),
                    AppellantAddressZipCode: me.getAppellantAddressZipCode(),
                    AppellantAddressCountryName: me.getAppellantAddressCountryName(),
                    AppellantAddressNotes: me.getAppellantAddressNotes(),
                    AppellantWorkPhoneNumber: me.getAppellantWorkPhoneNumber(),
                    AppellantHomePhoneNumber: me.getAppellantHomePhoneNumber()
                }
            }
        });
    }

});
