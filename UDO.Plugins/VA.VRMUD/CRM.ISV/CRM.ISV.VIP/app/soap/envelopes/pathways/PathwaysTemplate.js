Ext.define('VIP.soap.envelopes.pathways.PathwaysTemplate', {
    extend: 'VIP.soap.envelopes.VARequestBuilder',
    config: {
        nationalId: '',
        startDate: undefined,
        endDate: undefined,
        serviceUrl: 'repositories.med.va.gov/pathways',
        isDacRequest: true,
        vrmDcOpeningTag: '<_vrm.dctag_',
        vrmDcClosingTag: '_vrm.dctag2_>'
    },
    constructor: function (config) {
        var me = this,
            pathwaysBase = me.getEnvironment() ? me.getEnvironment().get('PathwaysBase') : 'http://vahdrtvapp02.aac.va.gov:7251/';

        me.initConfig(config);

        //Base Url config set up in VA Request Builder
        me.setBaseUrl(pathwaysBase);

        me.callParent();
    }
});