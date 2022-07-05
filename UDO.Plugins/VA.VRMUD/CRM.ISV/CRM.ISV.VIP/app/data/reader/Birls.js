Ext.define('VIP.data.reader.Birls', {
    extend: 'Ext.data.reader.Xml',
    alias: 'reader.birls',
    requires: [
        'VIP.util.Xml'
    ],
    read: function (response) {
        var me = this;

        VIP.util.Xml.wrap("insurancePolicies", "INSURANCE_POLICY", "return", response);
        VIP.util.Xml.wrap("services", "SERVICE", "return", response);
        VIP.util.Xml.wrap("alternateNames", "ALTERNATE_NAME", "return", response);
        VIP.util.Xml.wrap("folders", "FOLDER", "return", response);
        VIP.util.Xml.wrap("flashes", "FLASH", "return", response);
        VIP.util.Xml.wrap("serviceDiagnostic", "SERVICEDIAGNOSTICS", "return", response);
        VIP.util.Xml.wrap("recurringDisclosure", "RECURING_DISCLOSURE", "return", response);

        return me.callParent([response]);
    }
});